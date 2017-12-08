#!/bin/env python

import sys, argparse
import os, os.path
import re
from datetime import datetime, timedelta
import pytz
import subprocess
import math

default_timezone = pytz.timezone('Europe/Amsterdam')
ffprobe = 'ffmpeg/bin/ffprobe'
ffmpeg = 'ffmpeg/bin/ffmpeg'
font_file = 'FreeSerif.ttf'
vcodec = 'hevc_nvenc'

argparser = argparse.ArgumentParser(description='Tool to process recorded video streams and combine multiple parallel streams into a single video.')
argparser.add_argument('-t', '--timezone', default='Europe/Amsterdam', help='Time zone to process timestamps with.')
argparser.add_argument('--ffprobe', default=ffprobe, help='Path to ffprobe executable.')
argparser.add_argument('--ffmpeg', default=ffmpeg, help='Path to ffmpeg executable.')
argparser.add_argument('-f', '--font_file', default=font_file, help='Font file to use for text overlays.')
argparser.add_argument('-s', '--preferred_stream', default='', help='Name of the stream to give precedence, i.e. that will be placed first and used for sourcing the audio stream if possible.')
argparser.add_argument('-vc', '--video_codec', default=vcodec, help='Codec to use for video encoding.')
argparser.add_argument('-i', '--input_dir', help='Directory to look for input stream video files.', required=True)
argparser.add_argument('-o', '--output_dir', help='Directory to place output video files.', required=True)
argparser.add_argument('--dry-run', action='store_true', help='Whether or not to do a dry run. Dry runs do not actually perform any video encoding.')

def format_timedelta(td):
    secs = td.total_seconds()
    return '{:02}:{:02}:{:02}.{:03}'.format(int(secs) // 3600, int(secs) % 3600 // 60, int(secs) % 60, int((secs - math.floor(secs)) * 1000))

class VideoDescriptor(object):
    def __init__(self, filename):
        self.filename = filename
    
        # Extract stream name and start timestamp from the video filename
        match = re.search(r'(\w+)-(\d{4})(\d{2})(\d{2})-(\d{2})(\d{2})(\d{2})(-(\d{3}))?', filename)
        if not match:
            raise ValueError('Invalid video filename: ' + filename)
        
        self.stream_name = match.group(1)
        (year, month, day, hour, minute, second) = [int(x) for x in match.group(2, 3, 4, 5, 6, 7)]
        millisecond = int(match.group(9)) if match.group(9) else 0
        self.start_time = datetime(year, month, day, hour, minute, second, millisecond * 1000, default_timezone)
        
    def analyze_video(self):
        # Use ffprobe to extract the video's duration and calculate the video's exact end timestamp
        probe = subprocess.check_output([ffprobe, self.filename], stderr=subprocess.STDOUT)
        match = re.search(r'Duration: (\d+):(\d+):(\d+)\.(\d+),', probe)
        if not match:
            raise ValueError('Could not probe video info for file: ' + self.filename)
            
        (hours, minutes, seconds) = [int(x) for x in match.group(1, 2, 3)]
        fraction = match.group(4)
        milliseconds = int(fraction + ('0' * (3 - len(fraction))))
        
        self.duration = timedelta(hours = hours, minutes = minutes, seconds = seconds, milliseconds = milliseconds)
        self.end_time = self.start_time + self.duration
        
    def create_video_input(self, start_time, duration):
        input = []
        
        # Add starting offset (if applicable)
        offset = start_time - self.start_time
        if offset.total_seconds() > 0.0:
            input.extend(['-ss', format_timedelta(offset)])
            
        # Add duration
        input.extend(['-t', format_timedelta(duration)])
        
        # Add input filename
        input.extend(['-i', self.filename])
        return input
        
    def create_filter(self, index, scale, pad, out_name):
        filters = ['setpts=PTS-STARTPTS']
        if scale:
            filters.append('scale={}:{}'.format(*scale))
            
        if font_file:
            filters.append(self._create_text_overlay())
        
        if pad:
            filters.append('pad={}:{}:{}:{}'.format(*pad))
            
        return '[{}:v]'.format(index) + ','.join(filters) + '[{}]'.format(out_name)
        
    def _create_text_overlay(self):
        args = {
            'fontfile': "'{}'".format(font_file),
            'text': "'{}'".format(self.stream_name),
            'fontcolor': 'white',
            'fontsize': 96,
            'box': 1,
            'boxcolor': 'black@0.5',
            'boxborderw': 5,
            'x': 5,
            'y': '(h-text_h-5)',
        }
        
        instructions = ':'.join(['{}={}'.format(k, str(args[k])) for k in args])
        return 'drawtext={}'.format(instructions)
        
    def __str__(self):
        return '{}: {} to {}'.format(self.stream_name, self.start_time, self.end_time)

class VideoSegment(object):
    def __init__(self, start_time, videos, preferred_stream):
        self.start_time = start_time
        self.videos = videos
        self.preferred_stream = preferred_stream
        
    def start_video(self, vid):
        self.end_time = vid.start_time
        new_vids = list(self.videos)
        new_vids.append(vid)
        return VideoSegment(vid.start_time, new_vids, self.preferred_stream)
        
    def end_video(self, vid):
        self.end_time = vid.end_time
        new_vids = list(self.videos)
        new_vids.remove(vid)
        return VideoSegment(vid.end_time, new_vids, self.preferred_stream)
        
    def duration(self):
        if self.start_time and self.end_time:
            return self.end_time - self.start_time
        else:
            return None
        
    def sorted_videos(self):
        # Sort videos so that "my" video stream always comes first (for audio selection purposes)
        return sorted(self.videos, key = lambda v: -1 if v.stream_name == self.preferred_stream else 1)
        
    def create_video_inputs(self):
        inputs = []
        for vid in self.sorted_videos():
            inputs.extend(vid.create_video_input(self.start_time, self.duration()))
            
        return inputs
        
    def create_video_filter(self):
        steps = []
        
        vids = self.sorted_videos()
        num_videos = len(vids)
        
        if num_videos == 1:
            #steps.append(vids[0].create_filter(0, (1920, 1080), None, 'v'))
            steps.append(vids[0].create_filter(0, (3840, 2160), None, 'v'))
        elif num_videos == 2:
            #steps.append(vids[0].create_filter(0, (1920, 1080), (2560, 1080, 0, 0), 'top'))
            #steps.append(vids[1].create_filter(1, (1920, 1080), (2560, 1080, 640, 0), 'bottom'))
            steps.append(vids[0].create_filter(0, (1920, 1080), (3840, 1080, 640, 0), 'top'))
            steps.append(vids[1].create_filter(1, (1920, 1080), (3840, 1080, 1280, 0), 'bottom'))
            steps.append('[top][bottom]vstack[v]')
        elif num_videos == 3:
            steps.append(vids[0].create_filter(0, (1920, 1080), None, 'topleft'))
            steps.append(vids[1].create_filter(1, (1920, 1080), None, 'topright'))
            steps.append(vids[2].create_filter(2, (1920, 1080), (3840, 1080, 960, 960), 'bottom'))
            steps.append('[topleft][topright]hstack[top]')
            steps.append('[top][bottom]vstack[v]')
        elif num_videos >= 4:
            steps.append(vids[0].create_filter(0, (1920, 1080), None, 'topleft'))
            steps.append(vids[1].create_filter(1, (1920, 1080), None, 'topright'))
            steps.append(vids[2].create_filter(2, (1920, 1080), None, 'bottomleft'))
            steps.append(vids[3].create_filter(3, (1920, 1080), None, 'bottomright'))
            steps.append('[topleft][topright]hstack[top]')
            steps.append('[bottomleft][bottomright]hstack[bottom]')
            steps.append('[top][bottom]vstack[v]')
        
        return ['-filter_complex', ';'.join(steps)]
        
    def output_filename(self):
        datestr = self.start_time.strftime('%Y-%m-%d %H-%M-%S')
        names = '-'.join([v.stream_name for v in self.videos])
        return '{} {}.mkv'.format(datestr, names)
        
    def __str__(self):
        result = 'Segment duration {}'.format(self.duration())
        if self.videos:
            for vid in self.videos:
                result += '\n\t' + str(vid)
                
        return result
        
def collect_videos(work_dir):
    result = []
    for filename in os.listdir(work_dir):
        (_, ext) = os.path.splitext(filename)
        if ext != '.flv':
            continue
            
        fullpath = os.path.realpath(os.path.join(work_dir, filename))
        vid = VideoDescriptor(fullpath)
        vid.analyze_video()
        result.append(vid)
        
    return result
    
def create_timeline(vids, preferred_stream = ''):
    segments = []
    if not vids:
        return segments
    
    sorted_by_start = sorted(vids, key = lambda v: v.start_time)
    sorted_by_end = sorted(vids, key = lambda v: v.end_time)
    
    start_vid = sorted_by_start.pop(0)
    current_segment = VideoSegment(start_vid.start_time, [start_vid], preferred_stream)
    segments.append(current_segment)
    
    # Go through the videos in chronological order.
    # Each change in a video's state (i.e. a video starts or ends) creates a new video segment on the timeline.
    while sorted_by_start or sorted_by_end:
        next_start = sorted_by_start[0] if sorted_by_start else None
        next_end = sorted_by_end[0] if sorted_by_end else None
        if next_start and next_end:
            if next_start.start_time < next_end.end_time:
                current_segment = current_segment.start_video(next_start)
                segments.append(current_segment)
                sorted_by_start.remove(next_start)
            else:
                current_segment = current_segment.end_video(next_end)
                segments.append(current_segment)
                sorted_by_end.remove(next_end)
        elif next_start:
            current_segment = current_segment.start_video(next_start)
            segments.append(current_segment)
            sorted_by_start.remove(next_start)
        elif next_end:
            current_segment = current_segment.end_video(next_end)
            segments.append(current_segment)
            sorted_by_end.remove(next_end)
    
    return segments
    
def encode_segment(segment, output_dir, dry_run = False):
    if not segment.videos or not segment.duration():
        raise ValueError("Trying to an encode an empty or unclosed video segment:\n" + str(segment))
        
    print('Encoding video segment: {}'.format(segment.output_filename()))
    
    # Stream inputs and video filtering
    cmd = [ffmpeg, '-y', '-fflags', '+genpts']
    cmd.extend(segment.create_video_inputs())
    cmd.extend(segment.create_video_filter())
    
    # Stream mapping: take the filtered video and copy audio from all input sources
    cmd.extend(['-map', '[v]'])
    for i in range(len(segment.videos)):
        cmd.extend(['-map', '{}:a'.format(i)])
        
    # Video and audio encoding settings
    cmd.extend(['-c:v', vcodec, '-qp', '25', '-r', '60'])#, '-vsync', '0']) # Note: qp 22 produces about the same filesizes as OBS's High Quality preset (higher qp equals lower quality)
    #cmd.extend(['-force_key_frames', "expr:gte(t,n_forced*3)", '-forced-idr', '1'])    # Forced keyframe generation
    cmd.extend(['-c:a', 'aac'])
    
    # Finally, provide the output filename
    cmd.append(os.path.join(output_dir, segment.output_filename()))
    
    print(' '.join(cmd))
    
    if not dry_run:
        subprocess.check_call(cmd)
    
def main(input_dir, output_dir, preferred_stream = '', dry_run = False):
    vids = collect_videos(input_dir)
    for vid in vids:
        print('{}: {} to {} (duration: {})'.format(vid.stream_name, vid.start_time, vid.end_time, vid.duration))
    
    segments = create_timeline(vids, preferred_stream)
    
    # Filter out empty segments and segments shorter than 2 minutes
    segments = [s for s in segments if s.videos and s.duration().total_seconds() >= 120]
    for seg in segments:
        print(str(seg))
        
    for seg in segments:
        encode_segment(seg, output_dir, dry_run)
        print('')
        
    return 0

if __name__ == '__main__':
    args = argparser.parse_args()
    
    # Assign static global variables
    default_timezone = pytz.timezone(args.timezone)
    ffprobe = args.ffprobe
    ffmpeg = args.ffmpeg
    font_file = args.font_file
    vcodec = args.video_codec
    
    res = main(args.input_dir, args.output_dir, args.preferred_stream, args.dry_run)
    sys.exit(res)

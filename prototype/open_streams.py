import requests
import xml.etree.ElementTree as ET
import subprocess

my_stream = 'nico'
stat_url = 'http://nicodepoel.com:8088/stat'
base_url = 'rtmp://raspberrypi.local'
ffplay = 'ffmpeg/bin/ffplay.exe'

def do_the_things():
    r = requests.get(stat_url)
    if r.status_code != 200:
        print('Error loading stream stats')
        return
        
    root = ET.fromstring(r.text)
    for app in root.findall('.//application'):
        appname = app.find('name').text
        for stream in app.findall('.//stream'):
            streamname = stream.find('name').text
            if streamname == my_stream:
                continue
            
            stream_url = '{}/{}/{}'.format(base_url, appname, streamname)
            print('Opening stream: ' + stream_url)
            
            cmd = [ffplay, '-fflags', 'nobuffer', '-i', stream_url, '-an']
            subprocess.Popen(cmd)

if __name__ == '__main__':
    do_the_things()

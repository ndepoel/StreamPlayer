using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using StreamPlayer.Properties;
using System.IO;

namespace StreamPlayer
{
    public class LiveStreamController
    {
        private List<LiveStream> _liveStreams = new List<LiveStream>();

        private Stack<uint> _freeScreenSlots = new Stack<uint>();

        private volatile bool _isActive = false;

        public bool IsActive => _isActive;

        public string[] ActiveStreams => _liveStreams.Select(ls => ls.FullName).ToArray();

        public event Action StreamsChanged;

        public LiveStreamController()
        {
            foreach (int slot in Enumerable.Range(0, 4))
            {
                _freeScreenSlots.Push((uint)(3 - slot));
            }

            UnpackResources();
        }
        
        public async void StartLiveStreams()
        {
            _isActive = true;

            while (_isActive)
            {
                UpdateLiveStreams();

                await Task.Delay(5000);
            }
        }

        public void CloseLiveStreams()
        {
            _isActive = false;

            var liveStreams = _liveStreams.ToArray();
            foreach (var liveStream in liveStreams)
            {
                liveStream.Close();
            }

            _liveStreams.Clear();
        }

        private void UnpackResources()
        {
            string ffplayPath = Path.Combine(Path.GetTempPath(), "ffplay.exe");
            File.WriteAllBytes(ffplayPath, Resources.ffplay);
            StreamConfig.Instance.FFPlay = ffplayPath;

            string fontPath = Path.Combine(Path.GetTempPath(), "Helvetica.ttf");
            File.WriteAllBytes(fontPath, Resources.Helvetica);
            StreamConfig.Instance.FontFile = fontPath;
        }

        private async void UpdateLiveStreams()
        {
            var stats = await RequestServerStats();
            var application = stats?.Server?.Application?.FirstOrDefault(app => app.Name == StreamConfig.Instance.Application);
            if (application == null)
                return;
            
            var streamDatas = application.LiveStreams;
            if (streamDatas == null)
                return;

            // Prevent showing your own stream
            streamDatas.RemoveAll(sd => sd.Name == StreamConfig.Instance.MyStream);

            // Update currently active streams
            foreach (var streamData in streamDatas)
            {
                UpdateLiveStream(application, streamData);
            }

            // Detect active LiveStream objects with missing StreamData (i.e. closed streams)
            var liveStreams = _liveStreams.ToArray();
            foreach (var liveStream in liveStreams)
            {
                var streamData = streamDatas.FirstOrDefault(sd => sd.Name == liveStream.StreamName);
                if (liveStream.AppName != application.Name || streamData == null)
                {
                    liveStream.Close();
                    StreamsChanged?.Invoke();
                }
            }
        }

        private void UpdateLiveStream(ApplicationData application, StreamData streamData)
        {
            if (application == null || streamData == null)
                return;
            
            var liveStream = FindLiveStream(application.Name, streamData.Name);
            if (liveStream == null)
            {
                liveStream = new LiveStream(application.Name, streamData.Name, streamData.Time);
                liveStream.Closed += LiveStream_OnClosed;
                liveStream.Start(AssignScreenSlot());
                _liveStreams.Add(liveStream);
                StreamsChanged?.Invoke();
                return;
            }

            // Detect if the live stream was restarted (i.e. time value differs)
            if (!liveStream.IsTimeConsistent(streamData.Time))
            {
                Console.WriteLine("Detected restart of live stream '{0}'", liveStream.FullName);
                liveStream.Close();
                liveStream.SetTime(streamData.Time);
                liveStream.Start(AssignScreenSlot());
                _liveStreams.Add(liveStream);
                StreamsChanged?.Invoke();
            }
        }

        private void LiveStream_OnClosed(LiveStream liveStream)
        {
            Console.WriteLine("Live stream '{0}' was closed", liveStream.FullName);
            _liveStreams.Remove(liveStream);
            FreeScreenSlot(liveStream.ScreenSlot);
            StreamsChanged?.Invoke();
        }

        public async Task<ServerStats> RequestServerStats()
        {
            try
            {
                var request = WebRequest.Create(StreamConfig.Instance.StatUrl);
                var response = await request.GetResponseAsync();
                using (var stream = response.GetResponseStream())
                {
                    return ServerStats.DeserializeFromXml(stream);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return null;
            }
        }

        private LiveStream FindLiveStream(string appName, string streamName)
        {
            return _liveStreams.FirstOrDefault(ls => ls.AppName == appName && ls.StreamName == streamName);
        }

        private uint AssignScreenSlot()
        {
            if (_freeScreenSlots.Count == 0)
                return 0;

            return _freeScreenSlots.Pop();
        }

        private void FreeScreenSlot(uint slot)
        {
            if (!_freeScreenSlots.Contains(slot))
                _freeScreenSlots.Push(slot);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace StreamPlayer
{
    public class LiveStreamController
    {
        private List<LiveStream> _liveStreams = new List<LiveStream>();

        private Stack<uint> _freeScreenCorners = new Stack<uint>();

        private volatile bool _isActive = false;

        public bool IsActive => _isActive;

        public string[] ActiveStreams => _liveStreams.Select(ls => ls.FullName).ToArray();

        public event Action StreamsChanged;

        public LiveStreamController()
        {
            foreach (int corner in Enumerable.Range(0, 4))
            {
                _freeScreenCorners.Push((uint)(3 - corner));
            }
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

        private async void UpdateLiveStreams()
        {
            var stats = await RequestServerStats();
            var application = stats?.Server?.Application;
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
                liveStream.Start(AssignScreenCorner());
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
                liveStream.Start(AssignScreenCorner());
                _liveStreams.Add(liveStream);
                StreamsChanged?.Invoke();
            }
        }

        private void LiveStream_OnClosed(LiveStream liveStream)
        {
            Console.WriteLine("Live stream '{0}' was closed", liveStream.FullName);
            _liveStreams.Remove(liveStream);
            FreeScreenCorner(liveStream.ScreenCorner);
            StreamsChanged?.Invoke();
        }

        private async Task<ServerStats> RequestServerStats()
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

        private uint AssignScreenCorner()
        {
            if (_freeScreenCorners.Count == 0)
                return 0;

            return _freeScreenCorners.Pop();
        }

        private void FreeScreenCorner(uint corner)
        {
            if (!_freeScreenCorners.Contains(corner))
                _freeScreenCorners.Push(corner);
        }
    }
}

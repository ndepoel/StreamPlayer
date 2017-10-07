using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace StreamPlayer
{
    public class LiveStream
    {
        public string AppName { get; private set; }

        public string StreamName { get; private set; }

        public uint ScreenCorner { get; private set; }

        public string FullName => string.Format("{0}/{1}", AppName, StreamName);

        public string StreamUrl => string.Format("{0}/{1}/{2}", StreamConfig.Instance.StreamBaseUrl, AppName, StreamName);

        public event Action<LiveStream> Closed;

        private Process _process;

        private DateTime _startTime;

        public LiveStream(string appName, string streamName, int time)
        {
            AppName = appName;
            StreamName = streamName;
            SetTime(time);
        }

        public void SetTime(int time)
        {
            _startTime = DateTime.UtcNow.Subtract(TimeSpan.FromMilliseconds(time));
        }

        public void Start(uint corner)
        {
            ScreenCorner = corner;

            InitStreamProcess();
            PositionWindow();
        }

        public void Close()
        {
            if (_process != null)
            {
                _process.Exited -= Process_Exited;
                _process.CloseMainWindow();
                _process.Kill();
                _process = null;
            }

            Closed?.Invoke(this);
        }

        public bool IsTimeConsistent(int time)
        {
            DateTime newTime = DateTime.UtcNow.Subtract(TimeSpan.FromMilliseconds(time));
            return Math.Abs((newTime - _startTime).TotalSeconds) < 10;
        }

        private void InitStreamProcess()
        {
            var args = new List<string>();

            // Disable buffering for minimal latency
            if (StreamConfig.Instance.UseBuffering)
                args.AddRange(new[] { "-fflags", "-nobuffer" });

            // RTMP stream input
            args.AddRange(new[] { "-i", StreamUrl });

            // Disable audio output to prevent causing an audio feedback loop
            args.Add("-an");

            // Disable window border to use more screen real estate
            if (StreamConfig.Instance.UseBorderless)
                args.Add("-noborder");

            // Add text overlay containing the stream's name
            args.AddRange(new[] { "-vf", "drawtext=\"fontfile=FreeSerif.ttf: text='" + StreamName + "': fontcolor=white: fontsize=96: box=1: boxcolor=black@0.5: boxborderw=5: x=5: y=(h-text_h-5)\"" });

            Console.WriteLine("Starting stream for URL: " + StreamUrl);

            var psi = new ProcessStartInfo
            {
                FileName = StreamConfig.Instance.FFPlay,
                Arguments = string.Join(" ", args),
                CreateNoWindow = true,
                UseShellExecute = false,
                WindowStyle = ProcessWindowStyle.Hidden,
            };

            var process = new Process
            {
                StartInfo = psi,
                EnableRaisingEvents = true,
            };
            
            process.Exited += Process_Exited;

            if (process.Start())
            {
                _process = process;
            }
        }

        private void Process_Exited(object sender, EventArgs e)
        {
            // TODO: maybe if the user manually closes the ffplay window, they want it to stay closed. Now it will just be reopened automatically.
            Closed?.Invoke(this);
        }

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);
        
        public async void PositionWindow()
        {
            if (_process == null)
                return;

            while (_process.MainWindowHandle == IntPtr.Zero)
            {
                await Task.Delay(1000);
                if (_process == null)
                    return;
            }
            
            var targetScreen = Screen.AllScreens.FirstOrDefault(s => s != Screen.PrimaryScreen) ?? Screen.PrimaryScreen;
            var workingArea = targetScreen.WorkingArea;

            int halfWidth = workingArea.Width / 2;
            int halfHeight = workingArea.Height / 2;
            uint cornerBits = ScreenCorner % 4;
            int x = ((cornerBits & 0x01) == 0) ? workingArea.Left : workingArea.Right - halfWidth;
            int y = ((cornerBits & 0x02) == 0) ? workingArea.Top : workingArea.Bottom - halfHeight;

            MoveWindow(_process.MainWindowHandle, x, y, halfWidth, halfHeight, true);
        }
    }
}

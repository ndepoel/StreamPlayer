using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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

        public uint ScreenSlot { get; private set; }

        public string FullName => string.Format("{0}/{1}", AppName, StreamName);

        public string StreamUrl => string.Format("{0}/{1}/{2}", StreamConfig.Instance.StreamBaseUrl, AppName, StreamName);

        public event Action<LiveStream> Closed;

        private Screen TargetScreen => Screen.AllScreens.FirstOrDefault(s => s != Screen.PrimaryScreen) ?? Screen.PrimaryScreen;

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

        public void Start(uint screenSlot)
        {
            ScreenSlot = screenSlot;

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
            args.AddRange(new[] { "-vf", CreateTextOverlayFilter() });

            Console.WriteLine("Starting stream for URL: " + StreamUrl);

            var psi = new ProcessStartInfo
            {
                FileName = StreamConfig.Instance.FFPlay,
                Arguments = string.Join(" ", args),
                CreateNoWindow = true,
                UseShellExecute = false,
                WindowStyle = ProcessWindowStyle.Hidden,
            };

            Console.WriteLine("Running: \"{0}\" {1}", psi.FileName, psi.Arguments);

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

        private string CreateTextOverlayFilter()
        {
            var sb = new StringBuilder("scale=1920:1080,drawtext=\"");
            sb.AppendFormat("fontfile='{0}':", StreamConfig.Instance.FontFile.Replace("\\", "\\\\").Replace(":", "\\:"));
            sb.AppendFormat("text='{0}':", StreamName);
            sb.Append("fontcolor=white:");
            sb.Append("fontsize=96:");
            sb.Append("box=1:boxcolor=black@0.5:boxborderw=5:");
            sb.Append("x=5:y=(h-text_h-5)");
            sb.Append("\"");
            return sb.ToString();
        }

        private void Process_Exited(object sender, EventArgs e)
        {
            // TODO: maybe if the user manually closes the ffplay window, they want it to stay closed. Now it will just be reopened automatically.
            Closed?.Invoke(this);
        }

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);

        private async void PositionWindow()
        {
            while (_process != null && _process.MainWindowHandle == IntPtr.Zero)
            {
                await Task.Delay(1000);
            }

            MoveWindowToTargetScreen();

            while (_process != null && !_process.HasExited)
            {
                CheckWindowPosition();
                await Task.Delay(1000);
            }
        }

        private void CheckWindowPosition()
        {
            // TODO: If window is not on correct screen, call MoveWindowToTargetScreen()
        }

        private void MoveWindowToTargetScreen()
        {
            if (_process == null || _process.HasExited || _process.MainWindowHandle == IntPtr.Zero)
                return;

            var workingArea = TargetScreen.WorkingArea;
            float aspect = (float)workingArea.Width / workingArea.Height;

            if (aspect >= 1)
            {
                // Horizontally orientated screen (2x2 pattern)
                int halfWidth = workingArea.Width / 2;
                int halfHeight = workingArea.Height / 2;
                uint slotBits = ScreenSlot % 4;
                int x = ((slotBits & 0x01) == 0) ? workingArea.Left : workingArea.Right - halfWidth;
                int y = ((slotBits & 0x02) == 0) ? workingArea.Top : workingArea.Bottom - halfHeight;

                MoveWindow(_process.MainWindowHandle, x, y, halfWidth, halfHeight, true);
            }
            else
            {
                // Vertically orientated screen (4 screens stacked)
                int quarterHeight = workingArea.Height / 4;
                int x = workingArea.Left;
                int y = workingArea.Top + quarterHeight * (int)ScreenSlot;

                MoveWindow(_process.MainWindowHandle, x, y, workingArea.Width, quarterHeight, true);
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using StreamPlayer.Properties;

namespace StreamPlayer
{
    public partial class MainForm : Form
    {
        private LiveStreamController _liveStreamController = new LiveStreamController();

        public MainForm()
        {
            InitializeComponent();

            _liveStreamController.StreamsChanged += OnStreamsChanged;

            txtStatsURL.Text = StreamConfig.Instance.StatUrl;
            txtStreamServer.Text = StreamConfig.Instance.StreamServer;
            cmbMyStream.Text = StreamConfig.Instance.MyStream;
            chkBorderless.Checked = StreamConfig.Instance.UseBorderless;
            chkBuffering.Checked = StreamConfig.Instance.UseBuffering;

            VerifyStatsURL();
            VerifyStreamServer();
        }

        private async void VerifyStatsURL()
        {
            if (string.IsNullOrWhiteSpace(StreamConfig.Instance.StatUrl))
            {
                picStatsUrl.Visible = false;
                return;
            }

            var stats = await _liveStreamController.RequestServerStats();
            var streams = stats?.Server?.Application?.LiveStreams;
            if (streams == null)
            {
                picStatsUrl.Image = Resources.Failure;
                picStatsUrl.Visible = true;
                return;
            }
            
            cmbMyStream.Items.Clear();
            foreach (var stream in streams)
            {
                cmbMyStream.Items.Add(stream.Name);
            }

            picStatsUrl.Image = Resources.Success;
            picStatsUrl.Visible = true;
        }

        private async void VerifyStreamServer()
        {
            if (string.IsNullOrWhiteSpace(StreamConfig.Instance.StreamServer))
            {
                picStreamServer.Visible = false;
                return;
            }

            try
            {
                var ping = new Ping();
                var pingReply = await ping.SendPingAsync(StreamConfig.Instance.StreamServer);
                if (pingReply.Status == IPStatus.Success)
                {
                    picStreamServer.Image = Resources.Success;
                    picStreamServer.Visible = true;
                    return;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            
            picStreamServer.Image = Resources.Failure;
            picStreamServer.Visible = true;
        }

        private void OnStreamsChanged()
        {
            Invoke((MethodInvoker)delegate
            {
                lstStreams.Items.Clear();
                foreach (var streamName in _liveStreamController.ActiveStreams)
                {
                    lstStreams.Items.Add(streamName);
                }
            });
        }

        private void btnOpenStreams_Click(object sender, EventArgs e)
        {
            if (!_liveStreamController.IsActive)
            {
                _liveStreamController.StartLiveStreams();
                btnOpenStreams.Text = "Stop Live Streams";
            }
            else
            {
                _liveStreamController.CloseLiveStreams();
                btnOpenStreams.Text = "Start Live Streams!";
            }
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            _liveStreamController.CloseLiveStreams();

            StreamConfig.Instance.Save();
        }

        private void txtStatsURL_TextChanged(object sender, EventArgs e)
        {
            StreamConfig.Instance.StatUrl = txtStatsURL.Text;
        }

        private void txtStreamServer_TextChanged(object sender, EventArgs e)
        {
            StreamConfig.Instance.StreamServer = txtStreamServer.Text;
        }

        private void cmbMyStream_TextChanged(object sender, EventArgs e)
        {
            StreamConfig.Instance.MyStream = cmbMyStream.Text;
        }

        private void chkBorderless_CheckedChanged(object sender, EventArgs e)
        {
            StreamConfig.Instance.UseBorderless = chkBorderless.Checked;
        }

        private void chkBuffering_CheckedChanged(object sender, EventArgs e)
        {
            StreamConfig.Instance.UseBuffering = chkBuffering.Checked;
        }

        private void txtStatsURL_Leave(object sender, EventArgs e)
        {
            VerifyStatsURL();
        }

        private void txtStreamServer_Leave(object sender, EventArgs e)
        {
            VerifyStreamServer();
        }
    }
}

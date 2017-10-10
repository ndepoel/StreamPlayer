using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

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
            txtMyStream.Text = StreamConfig.Instance.MyStream;
            chkBorderless.Checked = StreamConfig.Instance.UseBorderless;
            chkBuffering.Checked = StreamConfig.Instance.UseBuffering;
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

        private void txtMyStream_TextChanged(object sender, EventArgs e)
        {
            StreamConfig.Instance.MyStream = txtMyStream.Text;
        }

        private void chkBorderless_CheckedChanged(object sender, EventArgs e)
        {
            StreamConfig.Instance.UseBorderless = chkBorderless.Checked;
        }

        private void chkBuffering_CheckedChanged(object sender, EventArgs e)
        {
            StreamConfig.Instance.UseBuffering = chkBuffering.Checked;
        }
    }
}

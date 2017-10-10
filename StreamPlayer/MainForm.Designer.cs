namespace StreamPlayer
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.btnOpenStreams = new System.Windows.Forms.Button();
            this.lblStatUrl = new System.Windows.Forms.Label();
            this.txtStatsURL = new System.Windows.Forms.TextBox();
            this.txtStreamServer = new System.Windows.Forms.TextBox();
            this.lblStreamServer = new System.Windows.Forms.Label();
            this.lblMyStream = new System.Windows.Forms.Label();
            this.lblStreams = new System.Windows.Forms.Label();
            this.lstStreams = new System.Windows.Forms.ListBox();
            this.chkBuffering = new System.Windows.Forms.CheckBox();
            this.chkBorderless = new System.Windows.Forms.CheckBox();
            this.cmbMyStream = new System.Windows.Forms.ComboBox();
            this.picStatsUrl = new System.Windows.Forms.PictureBox();
            this.picStreamServer = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.picStatsUrl)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picStreamServer)).BeginInit();
            this.SuspendLayout();
            // 
            // btnOpenStreams
            // 
            this.btnOpenStreams.Location = new System.Drawing.Point(12, 296);
            this.btnOpenStreams.Name = "btnOpenStreams";
            this.btnOpenStreams.Size = new System.Drawing.Size(104, 33);
            this.btnOpenStreams.TabIndex = 0;
            this.btnOpenStreams.Text = "Start Live Streams!";
            this.btnOpenStreams.UseVisualStyleBackColor = true;
            this.btnOpenStreams.Click += new System.EventHandler(this.btnOpenStreams_Click);
            // 
            // lblStatUrl
            // 
            this.lblStatUrl.AutoSize = true;
            this.lblStatUrl.Location = new System.Drawing.Point(9, 12);
            this.lblStatUrl.Name = "lblStatUrl";
            this.lblStatUrl.Size = new System.Drawing.Size(56, 13);
            this.lblStatUrl.TabIndex = 1;
            this.lblStatUrl.Text = "Stats URL";
            // 
            // txtStatsURL
            // 
            this.txtStatsURL.Location = new System.Drawing.Point(107, 9);
            this.txtStatsURL.Name = "txtStatsURL";
            this.txtStatsURL.Size = new System.Drawing.Size(325, 20);
            this.txtStatsURL.TabIndex = 2;
            this.txtStatsURL.TextChanged += new System.EventHandler(this.txtStatsURL_TextChanged);
            this.txtStatsURL.Leave += new System.EventHandler(this.txtStatsURL_Leave);
            // 
            // txtStreamServer
            // 
            this.txtStreamServer.Location = new System.Drawing.Point(107, 35);
            this.txtStreamServer.Name = "txtStreamServer";
            this.txtStreamServer.Size = new System.Drawing.Size(325, 20);
            this.txtStreamServer.TabIndex = 4;
            this.txtStreamServer.TextChanged += new System.EventHandler(this.txtStreamServer_TextChanged);
            this.txtStreamServer.Leave += new System.EventHandler(this.txtStreamServer_Leave);
            // 
            // lblStreamServer
            // 
            this.lblStreamServer.AutoSize = true;
            this.lblStreamServer.Location = new System.Drawing.Point(9, 38);
            this.lblStreamServer.Name = "lblStreamServer";
            this.lblStreamServer.Size = new System.Drawing.Size(88, 13);
            this.lblStreamServer.TabIndex = 3;
            this.lblStreamServer.Text = "Streaming Server";
            // 
            // lblMyStream
            // 
            this.lblMyStream.AutoSize = true;
            this.lblMyStream.Location = new System.Drawing.Point(9, 64);
            this.lblMyStream.Name = "lblMyStream";
            this.lblMyStream.Size = new System.Drawing.Size(88, 13);
            this.lblMyStream.TabIndex = 5;
            this.lblMyStream.Text = "My Stream Name";
            // 
            // lblStreams
            // 
            this.lblStreams.AutoSize = true;
            this.lblStreams.Location = new System.Drawing.Point(12, 145);
            this.lblStreams.Name = "lblStreams";
            this.lblStreams.Size = new System.Drawing.Size(78, 13);
            this.lblStreams.TabIndex = 8;
            this.lblStreams.Text = "Active Streams";
            // 
            // lstStreams
            // 
            this.lstStreams.FormattingEnabled = true;
            this.lstStreams.Location = new System.Drawing.Point(12, 158);
            this.lstStreams.Name = "lstStreams";
            this.lstStreams.Size = new System.Drawing.Size(448, 134);
            this.lstStreams.TabIndex = 9;
            // 
            // chkBuffering
            // 
            this.chkBuffering.AutoSize = true;
            this.chkBuffering.Location = new System.Drawing.Point(12, 111);
            this.chkBuffering.Name = "chkBuffering";
            this.chkBuffering.Size = new System.Drawing.Size(140, 17);
            this.chkBuffering.TabIndex = 13;
            this.chkBuffering.Text = "Enable Stream Buffering";
            this.chkBuffering.UseVisualStyleBackColor = true;
            this.chkBuffering.CheckedChanged += new System.EventHandler(this.chkBuffering_CheckedChanged);
            // 
            // chkBorderless
            // 
            this.chkBorderless.AutoSize = true;
            this.chkBorderless.Location = new System.Drawing.Point(12, 87);
            this.chkBorderless.Name = "chkBorderless";
            this.chkBorderless.Size = new System.Drawing.Size(175, 17);
            this.chkBorderless.TabIndex = 12;
            this.chkBorderless.Text = "Use Borderless Stream Displays";
            this.chkBorderless.UseVisualStyleBackColor = true;
            this.chkBorderless.CheckedChanged += new System.EventHandler(this.chkBorderless_CheckedChanged);
            // 
            // cmbMyStream
            // 
            this.cmbMyStream.FormattingEnabled = true;
            this.cmbMyStream.Location = new System.Drawing.Point(107, 61);
            this.cmbMyStream.Name = "cmbMyStream";
            this.cmbMyStream.Size = new System.Drawing.Size(353, 21);
            this.cmbMyStream.TabIndex = 14;
            this.cmbMyStream.TextChanged += new System.EventHandler(this.cmbMyStream_TextChanged);
            // 
            // picStatsUrl
            // 
            this.picStatsUrl.Image = global::StreamPlayer.Properties.Resources.Success;
            this.picStatsUrl.Location = new System.Drawing.Point(438, 9);
            this.picStatsUrl.Name = "picStatsUrl";
            this.picStatsUrl.Size = new System.Drawing.Size(22, 20);
            this.picStatsUrl.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picStatsUrl.TabIndex = 15;
            this.picStatsUrl.TabStop = false;
            this.picStatsUrl.Visible = false;
            // 
            // picStreamServer
            // 
            this.picStreamServer.Image = global::StreamPlayer.Properties.Resources.Failure;
            this.picStreamServer.Location = new System.Drawing.Point(438, 35);
            this.picStreamServer.Name = "picStreamServer";
            this.picStreamServer.Size = new System.Drawing.Size(22, 20);
            this.picStreamServer.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picStreamServer.TabIndex = 16;
            this.picStreamServer.TabStop = false;
            this.picStreamServer.Visible = false;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(472, 338);
            this.Controls.Add(this.picStreamServer);
            this.Controls.Add(this.picStatsUrl);
            this.Controls.Add(this.cmbMyStream);
            this.Controls.Add(this.chkBuffering);
            this.Controls.Add(this.chkBorderless);
            this.Controls.Add(this.lstStreams);
            this.Controls.Add(this.lblStreams);
            this.Controls.Add(this.lblMyStream);
            this.Controls.Add(this.txtStreamServer);
            this.Controls.Add(this.lblStreamServer);
            this.Controls.Add(this.txtStatsURL);
            this.Controls.Add(this.lblStatUrl);
            this.Controls.Add(this.btnOpenStreams);
            this.Name = "MainForm";
            this.Text = "Stream Player";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MainForm_FormClosed);
            ((System.ComponentModel.ISupportInitialize)(this.picStatsUrl)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picStreamServer)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnOpenStreams;
        private System.Windows.Forms.Label lblStatUrl;
        private System.Windows.Forms.TextBox txtStatsURL;
        private System.Windows.Forms.TextBox txtStreamServer;
        private System.Windows.Forms.Label lblStreamServer;
        private System.Windows.Forms.Label lblMyStream;
        private System.Windows.Forms.Label lblStreams;
        private System.Windows.Forms.ListBox lstStreams;
        private System.Windows.Forms.CheckBox chkBuffering;
        private System.Windows.Forms.CheckBox chkBorderless;
        private System.Windows.Forms.ComboBox cmbMyStream;
        private System.Windows.Forms.PictureBox picStatsUrl;
        private System.Windows.Forms.PictureBox picStreamServer;
    }
}


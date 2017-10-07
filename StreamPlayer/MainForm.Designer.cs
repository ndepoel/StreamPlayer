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
            this.txtBaseUrl = new System.Windows.Forms.TextBox();
            this.lblBaseUrl = new System.Windows.Forms.Label();
            this.txtMyStream = new System.Windows.Forms.TextBox();
            this.lblMyStream = new System.Windows.Forms.Label();
            this.lblStreams = new System.Windows.Forms.Label();
            this.lstStreams = new System.Windows.Forms.ListBox();
            this.chkBuffering = new System.Windows.Forms.CheckBox();
            this.chkBorderless = new System.Windows.Forms.CheckBox();
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
            this.txtStatsURL.Size = new System.Drawing.Size(353, 20);
            this.txtStatsURL.TabIndex = 2;
            this.txtStatsURL.TextChanged += new System.EventHandler(this.txtStatsURL_TextChanged);
            // 
            // txtBaseUrl
            // 
            this.txtBaseUrl.Location = new System.Drawing.Point(107, 35);
            this.txtBaseUrl.Name = "txtBaseUrl";
            this.txtBaseUrl.Size = new System.Drawing.Size(353, 20);
            this.txtBaseUrl.TabIndex = 4;
            this.txtBaseUrl.TextChanged += new System.EventHandler(this.txtBaseUrl_TextChanged);
            // 
            // lblBaseUrl
            // 
            this.lblBaseUrl.AutoSize = true;
            this.lblBaseUrl.Location = new System.Drawing.Point(9, 38);
            this.lblBaseUrl.Name = "lblBaseUrl";
            this.lblBaseUrl.Size = new System.Drawing.Size(92, 13);
            this.lblBaseUrl.TabIndex = 3;
            this.lblBaseUrl.Text = "Stream Base URL";
            // 
            // txtMyStream
            // 
            this.txtMyStream.Location = new System.Drawing.Point(107, 61);
            this.txtMyStream.Name = "txtMyStream";
            this.txtMyStream.Size = new System.Drawing.Size(353, 20);
            this.txtMyStream.TabIndex = 6;
            this.txtMyStream.TextChanged += new System.EventHandler(this.txtMyStream_TextChanged);
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
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(472, 338);
            this.Controls.Add(this.chkBuffering);
            this.Controls.Add(this.chkBorderless);
            this.Controls.Add(this.lstStreams);
            this.Controls.Add(this.lblStreams);
            this.Controls.Add(this.txtMyStream);
            this.Controls.Add(this.lblMyStream);
            this.Controls.Add(this.txtBaseUrl);
            this.Controls.Add(this.lblBaseUrl);
            this.Controls.Add(this.txtStatsURL);
            this.Controls.Add(this.lblStatUrl);
            this.Controls.Add(this.btnOpenStreams);
            this.Name = "MainForm";
            this.Text = "Stream Player";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MainForm_FormClosed);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnOpenStreams;
        private System.Windows.Forms.Label lblStatUrl;
        private System.Windows.Forms.TextBox txtStatsURL;
        private System.Windows.Forms.TextBox txtBaseUrl;
        private System.Windows.Forms.Label lblBaseUrl;
        private System.Windows.Forms.TextBox txtMyStream;
        private System.Windows.Forms.Label lblMyStream;
        private System.Windows.Forms.Label lblStreams;
        private System.Windows.Forms.ListBox lstStreams;
        private System.Windows.Forms.CheckBox chkBuffering;
        private System.Windows.Forms.CheckBox chkBorderless;
    }
}


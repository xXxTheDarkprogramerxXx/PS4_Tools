namespace PS4_PKG_Viewer
{
    partial class PS4_Medial_Player
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PS4_Medial_Player));
            this.pnlLoading = new System.Windows.Forms.Panel();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.label2 = new System.Windows.Forms.Label();
            this.btnPlay = new System.Windows.Forms.Button();
            this.txtAt9 = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnAt9 = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.pnlLoading.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // pnlLoading
            // 
            this.pnlLoading.Controls.Add(this.progressBar1);
            this.pnlLoading.Controls.Add(this.label2);
            this.pnlLoading.Location = new System.Drawing.Point(460, 33);
            this.pnlLoading.Name = "pnlLoading";
            this.pnlLoading.Size = new System.Drawing.Size(182, 87);
            this.pnlLoading.TabIndex = 9;
            this.pnlLoading.Visible = false;
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(6, 44);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(173, 23);
            this.progressBar1.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.progressBar1.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 10);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(106, 17);
            this.label2.TabIndex = 0;
            this.label2.Text = "Loading song...";
            // 
            // btnPlay
            // 
            this.btnPlay.Image = global::PS4_PKG_Viewer.Properties.Resources.baseline_play_circle_outline_black_18dp;
            this.btnPlay.Location = new System.Drawing.Point(282, 55);
            this.btnPlay.Name = "btnPlay";
            this.btnPlay.Size = new System.Drawing.Size(95, 65);
            this.btnPlay.TabIndex = 8;
            this.btnPlay.UseVisualStyleBackColor = true;
            this.btnPlay.Click += new System.EventHandler(this.btnPlay_Click);
            // 
            // txtAt9
            // 
            this.txtAt9.Enabled = false;
            this.txtAt9.Location = new System.Drawing.Point(136, 4);
            this.txtAt9.Name = "txtAt9";
            this.txtAt9.Size = new System.Drawing.Size(462, 22);
            this.txtAt9.TabIndex = 7;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(117, 17);
            this.label1.TabIndex = 6;
            this.label1.Text = "Select an at9 File";
            // 
            // btnAt9
            // 
            this.btnAt9.Location = new System.Drawing.Point(604, 4);
            this.btnAt9.Name = "btnAt9";
            this.btnAt9.Size = new System.Drawing.Size(75, 23);
            this.btnAt9.TabIndex = 5;
            this.btnAt9.Text = "...";
            this.btnAt9.UseVisualStyleBackColor = true;
            this.btnAt9.Click += new System.EventHandler(this.btnAt9_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::PS4_PKG_Viewer.Properties.Resources._4862351490_63c3ddf4_94ce_43b2_92d2_e33d3b0f7be2__1_;
            this.pictureBox1.Location = new System.Drawing.Point(12, 33);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(117, 100);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 10;
            this.pictureBox1.TabStop = false;
            // 
            // PS4_Medial_Player
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(682, 133);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.pnlLoading);
            this.Controls.Add(this.btnPlay);
            this.Controls.Add(this.txtAt9);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnAt9);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "PS4_Medial_Player";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "PS4 Media Player";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.PS4_Medial_Player_FormClosing);
            this.Load += new System.EventHandler(this.PS4_Medial_Player_Load);
            this.pnlLoading.ResumeLayout(false);
            this.pnlLoading.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel pnlLoading;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnPlay;
        private System.Windows.Forms.TextBox txtAt9;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnAt9;
        private System.Windows.Forms.PictureBox pictureBox1;
    }
}
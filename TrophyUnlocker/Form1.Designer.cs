namespace TrophyUnlocker
{
    partial class Form1
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.button1 = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.button5 = new System.Windows.Forms.Button();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.txtPreviewTitle = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.label2 = new System.Windows.Forms.Label();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.lblProgress = new System.Windows.Forms.Label();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.pnlProgress = new System.Windows.Forms.Panel();
            this.backgroundWorker2 = new System.ComponentModel.BackgroundWorker();
            this.btnSettings = new System.Windows.Forms.Button();
            this.pnlFTP = new System.Windows.Forms.Panel();
            this.pnlManual = new System.Windows.Forms.Panel();
            this.txtNpBind = new System.Windows.Forms.TextBox();
            this.btnNpBind = new System.Windows.Forms.Button();
            this.label10 = new System.Windows.Forms.Label();
            this.txtNptitle = new System.Windows.Forms.TextBox();
            this.btnNptitle = new System.Windows.Forms.Button();
            this.lblNpTitle = new System.Windows.Forms.Label();
            this.txtTrophy = new System.Windows.Forms.TextBox();
            this.btnTrophy = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.txtParam = new System.Windows.Forms.TextBox();
            this.btnParam = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.pnlPKG = new System.Windows.Forms.Panel();
            this.txtPKG = new System.Windows.Forms.TextBox();
            this.btnPKG = new System.Windows.Forms.Button();
            this.label11 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.pnlProgress.SuspendLayout();
            this.pnlFTP.SuspendLayout();
            this.pnlManual.SuspendLayout();
            this.pnlPKG.SuspendLayout();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(727, 6);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 0;
            this.button1.Text = "Connect";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(2, 10);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(76, 17);
            this.label1.TabIndex = 1;
            this.label1.Text = "PS4 FTP : ";
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(103, 7);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(618, 22);
            this.textBox1.TabIndex = 2;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(12, 164);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(78, 17);
            this.label5.TabIndex = 12;
            this.label5.Text = "Content ID:";
            // 
            // button5
            // 
            this.button5.Location = new System.Drawing.Point(291, 316);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(157, 39);
            this.button5.TabIndex = 15;
            this.button5.Text = "Build Unlocker";
            this.button5.UseVisualStyleBackColor = true;
            this.button5.Click += new System.EventHandler(this.button5_Click);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(12, 235);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(70, 17);
            this.label7.TabIndex = 14;
            this.label7.Text = "Np Bind : ";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(12, 199);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(76, 17);
            this.label6.TabIndex = 13;
            this.label6.Text = "NpTitle Id :";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(12, 135);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(85, 17);
            this.label8.TabIndex = 16;
            this.label8.Text = "Game Title :";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.txtPreviewTitle);
            this.groupBox1.Controls.Add(this.pictureBox1);
            this.groupBox1.Location = new System.Drawing.Point(525, 135);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(287, 159);
            this.groupBox1.TabIndex = 17;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Preview";
            // 
            // txtPreviewTitle
            // 
            this.txtPreviewTitle.AutoSize = true;
            this.txtPreviewTitle.Location = new System.Drawing.Point(7, 124);
            this.txtPreviewTitle.Name = "txtPreviewTitle";
            this.txtPreviewTitle.Size = new System.Drawing.Size(0, 17);
            this.txtPreviewTitle.TabIndex = 1;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::TrophyUnlocker.Properties.Resources.icon0;
            this.pictureBox1.Location = new System.Drawing.Point(7, 22);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(113, 95);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(5, 48);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(58, 17);
            this.label2.TabIndex = 18;
            this.label2.Text = "Game : ";
            // 
            // comboBox1
            // 
            this.comboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Location = new System.Drawing.Point(103, 48);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(618, 24);
            this.comboBox1.TabIndex = 19;
            this.comboBox1.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            // 
            // backgroundWorker1
            // 
            this.backgroundWorker1.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker1_DoWork);
            this.backgroundWorker1.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorker1_RunWorkerCompleted);
            // 
            // lblProgress
            // 
            this.lblProgress.AutoSize = true;
            this.lblProgress.Location = new System.Drawing.Point(3, 12);
            this.lblProgress.Name = "lblProgress";
            this.lblProgress.Size = new System.Drawing.Size(71, 17);
            this.lblProgress.TabIndex = 20;
            this.lblProgress.Text = "Loading : ";
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(3, 32);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(320, 23);
            this.progressBar1.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.progressBar1.TabIndex = 21;
            // 
            // pnlProgress
            // 
            this.pnlProgress.Controls.Add(this.progressBar1);
            this.pnlProgress.Controls.Add(this.lblProgress);
            this.pnlProgress.Location = new System.Drawing.Point(492, 300);
            this.pnlProgress.Name = "pnlProgress";
            this.pnlProgress.Size = new System.Drawing.Size(320, 59);
            this.pnlProgress.TabIndex = 22;
            this.pnlProgress.Visible = false;
            // 
            // backgroundWorker2
            // 
            this.backgroundWorker2.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker2_DoWork);
            this.backgroundWorker2.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorker2_RunWorkerCompleted);
            // 
            // btnSettings
            // 
            this.btnSettings.Location = new System.Drawing.Point(12, 316);
            this.btnSettings.Name = "btnSettings";
            this.btnSettings.Size = new System.Drawing.Size(76, 39);
            this.btnSettings.TabIndex = 23;
            this.btnSettings.Text = "Settings";
            this.btnSettings.UseVisualStyleBackColor = true;
            this.btnSettings.Click += new System.EventHandler(this.btnSettings_Click);
            // 
            // pnlFTP
            // 
            this.pnlFTP.Controls.Add(this.textBox1);
            this.pnlFTP.Controls.Add(this.comboBox1);
            this.pnlFTP.Controls.Add(this.button1);
            this.pnlFTP.Controls.Add(this.label2);
            this.pnlFTP.Controls.Add(this.label1);
            this.pnlFTP.Location = new System.Drawing.Point(15, 5);
            this.pnlFTP.Name = "pnlFTP";
            this.pnlFTP.Size = new System.Drawing.Size(806, 75);
            this.pnlFTP.TabIndex = 24;
            this.pnlFTP.Visible = false;
            // 
            // pnlManual
            // 
            this.pnlManual.Controls.Add(this.txtNpBind);
            this.pnlManual.Controls.Add(this.btnNpBind);
            this.pnlManual.Controls.Add(this.label10);
            this.pnlManual.Controls.Add(this.txtNptitle);
            this.pnlManual.Controls.Add(this.btnNptitle);
            this.pnlManual.Controls.Add(this.lblNpTitle);
            this.pnlManual.Controls.Add(this.txtTrophy);
            this.pnlManual.Controls.Add(this.btnTrophy);
            this.pnlManual.Controls.Add(this.label3);
            this.pnlManual.Controls.Add(this.txtParam);
            this.pnlManual.Controls.Add(this.btnParam);
            this.pnlManual.Controls.Add(this.label4);
            this.pnlManual.Location = new System.Drawing.Point(15, 5);
            this.pnlManual.Name = "pnlManual";
            this.pnlManual.Size = new System.Drawing.Size(806, 124);
            this.pnlManual.TabIndex = 25;
            this.pnlManual.Visible = false;
            // 
            // txtNpBind
            // 
            this.txtNpBind.Location = new System.Drawing.Point(103, 91);
            this.txtNpBind.Name = "txtNpBind";
            this.txtNpBind.Size = new System.Drawing.Size(618, 22);
            this.txtNpBind.TabIndex = 11;
            // 
            // btnNpBind
            // 
            this.btnNpBind.Location = new System.Drawing.Point(727, 90);
            this.btnNpBind.Name = "btnNpBind";
            this.btnNpBind.Size = new System.Drawing.Size(75, 23);
            this.btnNpBind.TabIndex = 9;
            this.btnNpBind.Text = "...";
            this.btnNpBind.UseVisualStyleBackColor = true;
            this.btnNpBind.Click += new System.EventHandler(this.btnNpBind_Click);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(2, 94);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(75, 17);
            this.label10.TabIndex = 10;
            this.label10.Text = "npbind.dat";
            // 
            // txtNptitle
            // 
            this.txtNptitle.Location = new System.Drawing.Point(103, 63);
            this.txtNptitle.Name = "txtNptitle";
            this.txtNptitle.Size = new System.Drawing.Size(618, 22);
            this.txtNptitle.TabIndex = 8;
            // 
            // btnNptitle
            // 
            this.btnNptitle.Location = new System.Drawing.Point(727, 62);
            this.btnNptitle.Name = "btnNptitle";
            this.btnNptitle.Size = new System.Drawing.Size(75, 23);
            this.btnNptitle.TabIndex = 6;
            this.btnNptitle.Text = "...";
            this.btnNptitle.UseVisualStyleBackColor = true;
            this.btnNptitle.Click += new System.EventHandler(this.btnNptitle_Click);
            // 
            // lblNpTitle
            // 
            this.lblNpTitle.AutoSize = true;
            this.lblNpTitle.Location = new System.Drawing.Point(2, 66);
            this.lblNpTitle.Name = "lblNpTitle";
            this.lblNpTitle.Size = new System.Drawing.Size(70, 17);
            this.lblNpTitle.TabIndex = 7;
            this.lblNpTitle.Text = "nptitle.dat";
            // 
            // txtTrophy
            // 
            this.txtTrophy.Location = new System.Drawing.Point(103, 35);
            this.txtTrophy.Name = "txtTrophy";
            this.txtTrophy.Size = new System.Drawing.Size(618, 22);
            this.txtTrophy.TabIndex = 5;
            // 
            // btnTrophy
            // 
            this.btnTrophy.Location = new System.Drawing.Point(727, 34);
            this.btnTrophy.Name = "btnTrophy";
            this.btnTrophy.Size = new System.Drawing.Size(75, 23);
            this.btnTrophy.TabIndex = 3;
            this.btnTrophy.Text = "...";
            this.btnTrophy.UseVisualStyleBackColor = true;
            this.btnTrophy.Click += new System.EventHandler(this.btnTrophy_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(2, 38);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(79, 17);
            this.label3.TabIndex = 4;
            this.label3.Text = "Trophy.Trp";
            // 
            // txtParam
            // 
            this.txtParam.Location = new System.Drawing.Point(103, 7);
            this.txtParam.Name = "txtParam";
            this.txtParam.Size = new System.Drawing.Size(618, 22);
            this.txtParam.TabIndex = 2;
            // 
            // btnParam
            // 
            this.btnParam.Location = new System.Drawing.Point(727, 6);
            this.btnParam.Name = "btnParam";
            this.btnParam.Size = new System.Drawing.Size(75, 23);
            this.btnParam.TabIndex = 0;
            this.btnParam.Text = "...";
            this.btnParam.UseVisualStyleBackColor = true;
            this.btnParam.Click += new System.EventHandler(this.btnParam_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(2, 10);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(81, 17);
            this.label4.TabIndex = 1;
            this.label4.Text = "Param.SFO";
            // 
            // pnlPKG
            // 
            this.pnlPKG.Controls.Add(this.txtPKG);
            this.pnlPKG.Controls.Add(this.btnPKG);
            this.pnlPKG.Controls.Add(this.label11);
            this.pnlPKG.Location = new System.Drawing.Point(15, 5);
            this.pnlPKG.Name = "pnlPKG";
            this.pnlPKG.Size = new System.Drawing.Size(806, 42);
            this.pnlPKG.TabIndex = 25;
            this.pnlPKG.Visible = false;
            // 
            // txtPKG
            // 
            this.txtPKG.Location = new System.Drawing.Point(103, 7);
            this.txtPKG.Name = "txtPKG";
            this.txtPKG.Size = new System.Drawing.Size(618, 22);
            this.txtPKG.TabIndex = 2;
            // 
            // btnPKG
            // 
            this.btnPKG.Location = new System.Drawing.Point(727, 6);
            this.btnPKG.Name = "btnPKG";
            this.btnPKG.Size = new System.Drawing.Size(75, 23);
            this.btnPKG.TabIndex = 0;
            this.btnPKG.Text = "...";
            this.btnPKG.UseVisualStyleBackColor = true;
            this.btnPKG.Click += new System.EventHandler(this.btnPKG_Click);
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(2, 10);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(71, 17);
            this.label11.TabIndex = 1;
            this.label11.Text = "PS4 PKG ";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(833, 367);
            this.Controls.Add(this.pnlPKG);
            this.Controls.Add(this.pnlManual);
            this.Controls.Add(this.btnSettings);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.button5);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.pnlProgress);
            this.Controls.Add(this.pnlFTP);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Trophy Unlocker";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.pnlProgress.ResumeLayout(false);
            this.pnlProgress.PerformLayout();
            this.pnlFTP.ResumeLayout(false);
            this.pnlFTP.PerformLayout();
            this.pnlManual.ResumeLayout(false);
            this.pnlManual.PerformLayout();
            this.pnlPKG.ResumeLayout(false);
            this.pnlPKG.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label txtPreviewTitle;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private System.Windows.Forms.Label lblProgress;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Panel pnlProgress;
        private System.ComponentModel.BackgroundWorker backgroundWorker2;
        private System.Windows.Forms.Button btnSettings;
        private System.Windows.Forms.Panel pnlFTP;
        private System.Windows.Forms.Panel pnlManual;
        private System.Windows.Forms.TextBox txtNpBind;
        private System.Windows.Forms.Button btnNpBind;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox txtNptitle;
        private System.Windows.Forms.Button btnNptitle;
        private System.Windows.Forms.Label lblNpTitle;
        private System.Windows.Forms.TextBox txtTrophy;
        private System.Windows.Forms.Button btnTrophy;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtParam;
        private System.Windows.Forms.Button btnParam;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Panel pnlPKG;
        private System.Windows.Forms.TextBox txtPKG;
        private System.Windows.Forms.Button btnPKG;
        private System.Windows.Forms.Label label11;
    }
}


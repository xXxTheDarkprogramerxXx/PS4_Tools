namespace SaveData_Manager
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.btnSettings = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.textBox3 = new System.Windows.Forms.TextBox();
            this.btnLoadDir = new System.Windows.Forms.Button();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.cmstripgames = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.saveAllGamesToLocalToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToLocalToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.showLocalItemsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copyLocalToUsbToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.cheatsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openHexEditorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.patchMenuToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.cmstripgames.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer1.IsSplitterFixed = true;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.btnSettings);
            this.splitContainer1.Panel1.Controls.Add(this.label1);
            this.splitContainer1.Panel1.Controls.Add(this.textBox3);
            this.splitContainer1.Panel1.Controls.Add(this.btnLoadDir);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.splitContainer2);
            this.splitContainer1.Size = new System.Drawing.Size(1034, 412);
            this.splitContainer1.TabIndex = 1;
            // 
            // btnSettings
            // 
            this.btnSettings.BackgroundImage = global::SaveData_Manager.Properties.Resources.baseline_settings_black_48dp;
            this.btnSettings.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btnSettings.Location = new System.Drawing.Point(13, 12);
            this.btnSettings.Name = "btnSettings";
            this.btnSettings.Size = new System.Drawing.Size(43, 35);
            this.btnSettings.TabIndex = 3;
            this.btnSettings.UseVisualStyleBackColor = true;
            this.btnSettings.Click += new System.EventHandler(this.btnSettings_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(62, 21);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(135, 17);
            this.label1.TabIndex = 2;
            this.label1.Text = "SaveData Directory:";
            // 
            // textBox3
            // 
            this.textBox3.Enabled = false;
            this.textBox3.Location = new System.Drawing.Point(203, 18);
            this.textBox3.Name = "textBox3";
            this.textBox3.Size = new System.Drawing.Size(738, 22);
            this.textBox3.TabIndex = 1;
            // 
            // btnLoadDir
            // 
            this.btnLoadDir.BackgroundImage = global::SaveData_Manager.Properties.Resources.baseline_source_black_48dp;
            this.btnLoadDir.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btnLoadDir.Location = new System.Drawing.Point(947, 12);
            this.btnLoadDir.Name = "btnLoadDir";
            this.btnLoadDir.Size = new System.Drawing.Size(75, 36);
            this.btnLoadDir.TabIndex = 0;
            this.btnLoadDir.UseVisualStyleBackColor = true;
            this.btnLoadDir.Click += new System.EventHandler(this.button1_Click);
            // 
            // splitContainer2
            // 
            this.splitContainer2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer2.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Name = "splitContainer2";
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.textBox1);
            this.splitContainer2.Panel1.Controls.Add(this.textBox2);
            this.splitContainer2.Panel1.Controls.Add(this.pictureBox1);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.dataGridView1);
            this.splitContainer2.Size = new System.Drawing.Size(1034, 358);
            this.splitContainer2.SplitterDistance = 344;
            this.splitContainer2.TabIndex = 0;
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(13, 241);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(316, 95);
            this.textBox1.TabIndex = 4;
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(16, 143);
            this.textBox2.Multiline = true;
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(313, 92);
            this.textBox2.TabIndex = 3;
            this.textBox2.Text = "Made to be a free alternative to SaveWizard";
            this.textBox2.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::SaveData_Manager.Properties.Resources._4792965023_57b20776_4c77_425a_b91c_10608d18cbf3;
            this.pictureBox1.Location = new System.Drawing.Point(16, 3);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(313, 134);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 1;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.Click += new System.EventHandler(this.pictureBox1_Click);
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.BackgroundColor = System.Drawing.Color.White;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.ContextMenuStrip = this.cmstripgames;
            this.dataGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView1.Location = new System.Drawing.Point(0, 0);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.ReadOnly = true;
            this.dataGridView1.RowHeadersVisible = false;
            this.dataGridView1.RowTemplate.Height = 24;
            this.dataGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView1.Size = new System.Drawing.Size(686, 358);
            this.dataGridView1.TabIndex = 0;
            this.dataGridView1.SelectionChanged += new System.EventHandler(this.dataGridView1_SelectionChanged);
            // 
            // cmstripgames
            // 
            this.cmstripgames.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.cmstripgames.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.saveAllGamesToLocalToolStripMenuItem,
            this.saveToLocalToolStripMenuItem,
            this.toolStripSeparator1,
            this.showLocalItemsToolStripMenuItem,
            this.copyLocalToUsbToolStripMenuItem,
            this.toolStripSeparator2,
            this.cheatsToolStripMenuItem});
            this.cmstripgames.Name = "cmstripgames";
            this.cmstripgames.Size = new System.Drawing.Size(240, 136);
            // 
            // saveAllGamesToLocalToolStripMenuItem
            // 
            this.saveAllGamesToLocalToolStripMenuItem.Name = "saveAllGamesToLocalToolStripMenuItem";
            this.saveAllGamesToLocalToolStripMenuItem.Size = new System.Drawing.Size(239, 24);
            this.saveAllGamesToLocalToolStripMenuItem.Text = "Save All Games To Local";
            this.saveAllGamesToLocalToolStripMenuItem.Click += new System.EventHandler(this.saveAllGamesToLocalToolStripMenuItem_Click);
            // 
            // saveToLocalToolStripMenuItem
            // 
            this.saveToLocalToolStripMenuItem.Name = "saveToLocalToolStripMenuItem";
            this.saveToLocalToolStripMenuItem.Size = new System.Drawing.Size(239, 24);
            this.saveToLocalToolStripMenuItem.Text = "Save To Local";
            this.saveToLocalToolStripMenuItem.Click += new System.EventHandler(this.saveToLocalToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(236, 6);
            // 
            // showLocalItemsToolStripMenuItem
            // 
            this.showLocalItemsToolStripMenuItem.Name = "showLocalItemsToolStripMenuItem";
            this.showLocalItemsToolStripMenuItem.Size = new System.Drawing.Size(239, 24);
            this.showLocalItemsToolStripMenuItem.Text = "Show Local Items";
            this.showLocalItemsToolStripMenuItem.Click += new System.EventHandler(this.showLocalItemsToolStripMenuItem_Click);
            // 
            // copyLocalToUsbToolStripMenuItem
            // 
            this.copyLocalToUsbToolStripMenuItem.Name = "copyLocalToUsbToolStripMenuItem";
            this.copyLocalToUsbToolStripMenuItem.Size = new System.Drawing.Size(239, 24);
            this.copyLocalToUsbToolStripMenuItem.Text = "Copy Local To Usb";
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(236, 6);
            // 
            // cheatsToolStripMenuItem
            // 
            this.cheatsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openHexEditorToolStripMenuItem,
            this.patchMenuToolStripMenuItem});
            this.cheatsToolStripMenuItem.Name = "cheatsToolStripMenuItem";
            this.cheatsToolStripMenuItem.Size = new System.Drawing.Size(239, 24);
            this.cheatsToolStripMenuItem.Text = "Cheats";
            // 
            // openHexEditorToolStripMenuItem
            // 
            this.openHexEditorToolStripMenuItem.Name = "openHexEditorToolStripMenuItem";
            this.openHexEditorToolStripMenuItem.Size = new System.Drawing.Size(190, 26);
            this.openHexEditorToolStripMenuItem.Text = "Open HexEditor";
            // 
            // patchMenuToolStripMenuItem
            // 
            this.patchMenuToolStripMenuItem.Name = "patchMenuToolStripMenuItem";
            this.patchMenuToolStripMenuItem.Size = new System.Drawing.Size(190, 26);
            this.patchMenuToolStripMenuItem.Text = "Patch Menu";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(1034, 412);
            this.Controls.Add(this.splitContainer1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "PS4 Save manager";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel1.PerformLayout();
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.cmstripgames.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnLoadDir;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBox3;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.ContextMenuStrip cmstripgames;
        private System.Windows.Forms.ToolStripMenuItem saveAllGamesToLocalToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveToLocalToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem showLocalItemsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem copyLocalToUsbToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem cheatsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openHexEditorToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem patchMenuToolStripMenuItem;
        private System.Windows.Forms.Button btnSettings;
    }
}


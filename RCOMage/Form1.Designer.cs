namespace RCOMage
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
            this.label1 = new System.Windows.Forms.Label();
            this.txtRCOPath = new System.Windows.Forms.TextBox();
            this.btnRCO = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.btnResourceDir = new System.Windows.Forms.Button();
            this.btnDump = new System.Windows.Forms.Button();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(72, 17);
            this.label1.TabIndex = 0;
            this.label1.Text = "RCO FIle :";
            // 
            // txtRCOPath
            // 
            this.txtRCOPath.Location = new System.Drawing.Point(162, 7);
            this.txtRCOPath.Name = "txtRCOPath";
            this.txtRCOPath.Size = new System.Drawing.Size(462, 22);
            this.txtRCOPath.TabIndex = 1;
            // 
            // btnRCO
            // 
            this.btnRCO.Location = new System.Drawing.Point(631, 7);
            this.btnRCO.Name = "btnRCO";
            this.btnRCO.Size = new System.Drawing.Size(75, 23);
            this.btnRCO.TabIndex = 2;
            this.btnRCO.Text = "...";
            this.btnRCO.UseVisualStyleBackColor = true;
            this.btnRCO.Click += new System.EventHandler(this.btnRCO_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(13, 48);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(134, 17);
            this.label2.TabIndex = 3;
            this.label2.Text = "Resource Directory:";
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(162, 45);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(462, 22);
            this.textBox1.TabIndex = 4;
            // 
            // btnResourceDir
            // 
            this.btnResourceDir.Location = new System.Drawing.Point(630, 42);
            this.btnResourceDir.Name = "btnResourceDir";
            this.btnResourceDir.Size = new System.Drawing.Size(75, 23);
            this.btnResourceDir.TabIndex = 5;
            this.btnResourceDir.Text = "...";
            this.btnResourceDir.UseVisualStyleBackColor = true;
            // 
            // btnDump
            // 
            this.btnDump.Location = new System.Drawing.Point(16, 102);
            this.btnDump.Name = "btnDump";
            this.btnDump.Size = new System.Drawing.Size(75, 23);
            this.btnDump.TabIndex = 6;
            this.btnDump.Text = "DUMP";
            this.btnDump.UseVisualStyleBackColor = true;
            // 
            // listBox1
            // 
            this.listBox1.FormattingEnabled = true;
            this.listBox1.HorizontalScrollbar = true;
            this.listBox1.ItemHeight = 16;
            this.listBox1.Location = new System.Drawing.Point(13, 159);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(211, 180);
            this.listBox1.TabIndex = 7;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(722, 361);
            this.Controls.Add(this.listBox1);
            this.Controls.Add(this.btnDump);
            this.Controls.Add(this.btnResourceDir);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.btnRCO);
            this.Controls.Add(this.txtRCOPath);
            this.Controls.Add(this.label1);
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "RCOMage";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtRCOPath;
        private System.Windows.Forms.Button btnRCO;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button btnResourceDir;
        private System.Windows.Forms.Button btnDump;
        private System.Windows.Forms.ListBox listBox1;
    }
}


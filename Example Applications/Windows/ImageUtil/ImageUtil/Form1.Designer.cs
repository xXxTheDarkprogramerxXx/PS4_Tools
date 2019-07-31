namespace ImageUtil
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.txtInput = new System.Windows.Forms.TextBox();
            this.bntImage = new System.Windows.Forms.Button();
            this.btnOutput = new System.Windows.Forms.Button();
            this.txtOutput = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.radioButton1 = new System.Windows.Forms.RadioButton();
            this.radioButton2 = new System.Windows.Forms.RadioButton();
            this.radioButton3 = new System.Windows.Forms.RadioButton();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.txtH = new System.Windows.Forms.TextBox();
            this.txtW = new System.Windows.Forms.TextBox();
            this.btnSave = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.btnGim = new System.Windows.Forms.Button();
            this.txtGimLoc = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.btnGimOut = new System.Windows.Forms.Button();
            this.txtGimOut = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.btnDo = new System.Windows.Forms.Button();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.btnSaveDDS = new System.Windows.Forms.Button();
            this.btnPNGLoc = new System.Windows.Forms.Button();
            this.btnDDSLoc = new System.Windows.Forms.Button();
            this.txtPNGSave = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.txtDDSLoc = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btnSave);
            this.groupBox1.Controls.Add(this.txtW);
            this.groupBox1.Controls.Add(this.txtH);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.radioButton3);
            this.groupBox1.Controls.Add(this.radioButton2);
            this.groupBox1.Controls.Add(this.radioButton1);
            this.groupBox1.Controls.Add(this.btnOutput);
            this.groupBox1.Controls.Add(this.txtOutput);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.bntImage);
            this.groupBox1.Controls.Add(this.txtInput);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(780, 180);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "PS4 Compatible PNG";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(7, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(104, 17);
            this.label1.TabIndex = 0;
            this.label1.Text = "Image Location";
            // 
            // txtInput
            // 
            this.txtInput.Location = new System.Drawing.Point(118, 22);
            this.txtInput.Name = "txtInput";
            this.txtInput.Size = new System.Drawing.Size(599, 22);
            this.txtInput.TabIndex = 1;
            // 
            // bntImage
            // 
            this.bntImage.Location = new System.Drawing.Point(724, 22);
            this.bntImage.Name = "bntImage";
            this.bntImage.Size = new System.Drawing.Size(56, 23);
            this.bntImage.TabIndex = 2;
            this.bntImage.Text = "...";
            this.bntImage.UseVisualStyleBackColor = true;
            this.bntImage.Click += new System.EventHandler(this.bntImage_Click);
            // 
            // btnOutput
            // 
            this.btnOutput.Location = new System.Drawing.Point(724, 50);
            this.btnOutput.Name = "btnOutput";
            this.btnOutput.Size = new System.Drawing.Size(56, 23);
            this.btnOutput.TabIndex = 5;
            this.btnOutput.Text = "...";
            this.btnOutput.UseVisualStyleBackColor = true;
            this.btnOutput.Click += new System.EventHandler(this.btnOutput_Click);
            // 
            // txtOutput
            // 
            this.txtOutput.Location = new System.Drawing.Point(118, 50);
            this.txtOutput.Name = "txtOutput";
            this.txtOutput.Size = new System.Drawing.Size(599, 22);
            this.txtOutput.TabIndex = 4;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(7, 50);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(113, 17);
            this.label2.TabIndex = 3;
            this.label2.Text = "Output Location:";
            // 
            // radioButton1
            // 
            this.radioButton1.AutoSize = true;
            this.radioButton1.Checked = true;
            this.radioButton1.Location = new System.Drawing.Point(6, 88);
            this.radioButton1.Name = "radioButton1";
            this.radioButton1.Size = new System.Drawing.Size(63, 21);
            this.radioButton1.TabIndex = 6;
            this.radioButton1.TabStop = true;
            this.radioButton1.Text = "Icon0";
            this.radioButton1.UseVisualStyleBackColor = true;
            this.radioButton1.CheckedChanged += new System.EventHandler(this.radioButton1_CheckedChanged);
            // 
            // radioButton2
            // 
            this.radioButton2.AutoSize = true;
            this.radioButton2.Location = new System.Drawing.Point(80, 88);
            this.radioButton2.Name = "radioButton2";
            this.radioButton2.Size = new System.Drawing.Size(56, 21);
            this.radioButton2.TabIndex = 7;
            this.radioButton2.Text = "Pic1";
            this.radioButton2.UseVisualStyleBackColor = true;
            this.radioButton2.CheckedChanged += new System.EventHandler(this.radioButton2_CheckedChanged);
            // 
            // radioButton3
            // 
            this.radioButton3.AutoSize = true;
            this.radioButton3.Location = new System.Drawing.Point(142, 88);
            this.radioButton3.Name = "radioButton3";
            this.radioButton3.Size = new System.Drawing.Size(76, 21);
            this.radioButton3.TabIndex = 8;
            this.radioButton3.Text = "Custom";
            this.radioButton3.UseVisualStyleBackColor = true;
            this.radioButton3.CheckedChanged += new System.EventHandler(this.radioButton3_CheckedChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(10, 116);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(53, 17);
            this.label3.TabIndex = 9;
            this.label3.Text = "Height:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(10, 144);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(52, 17);
            this.label4.TabIndex = 10;
            this.label4.Text = "Width :";
            // 
            // txtH
            // 
            this.txtH.Enabled = false;
            this.txtH.Location = new System.Drawing.Point(70, 116);
            this.txtH.Name = "txtH";
            this.txtH.Size = new System.Drawing.Size(66, 22);
            this.txtH.TabIndex = 11;
            this.txtH.Text = "512";
            // 
            // txtW
            // 
            this.txtW.Enabled = false;
            this.txtW.Location = new System.Drawing.Point(70, 144);
            this.txtW.Name = "txtW";
            this.txtW.Size = new System.Drawing.Size(66, 22);
            this.txtW.TabIndex = 12;
            this.txtW.Text = "512";
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(699, 88);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 13;
            this.btnSave.Text = "Convert";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.btnDo);
            this.groupBox2.Controls.Add(this.btnGimOut);
            this.groupBox2.Controls.Add(this.btnGim);
            this.groupBox2.Controls.Add(this.txtGimOut);
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Controls.Add(this.txtGimLoc);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Location = new System.Drawing.Point(12, 211);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(780, 142);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "GIM";
            // 
            // btnGim
            // 
            this.btnGim.Location = new System.Drawing.Point(724, 21);
            this.btnGim.Name = "btnGim";
            this.btnGim.Size = new System.Drawing.Size(56, 23);
            this.btnGim.TabIndex = 16;
            this.btnGim.Text = "...";
            this.btnGim.UseVisualStyleBackColor = true;
            this.btnGim.Click += new System.EventHandler(this.btnGim_Click);
            // 
            // txtGimLoc
            // 
            this.txtGimLoc.Location = new System.Drawing.Point(118, 21);
            this.txtGimLoc.Name = "txtGimLoc";
            this.txtGimLoc.Size = new System.Drawing.Size(599, 22);
            this.txtGimLoc.TabIndex = 15;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(7, 21);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(99, 17);
            this.label5.TabIndex = 14;
            this.label5.Text = "Gim Location :";
            // 
            // btnGimOut
            // 
            this.btnGimOut.Location = new System.Drawing.Point(724, 61);
            this.btnGimOut.Name = "btnGimOut";
            this.btnGimOut.Size = new System.Drawing.Size(56, 23);
            this.btnGimOut.TabIndex = 16;
            this.btnGimOut.Text = "...";
            this.btnGimOut.UseVisualStyleBackColor = true;
            // 
            // txtGimOut
            // 
            this.txtGimOut.Location = new System.Drawing.Point(118, 61);
            this.txtGimOut.Name = "txtGimOut";
            this.txtGimOut.Size = new System.Drawing.Size(599, 22);
            this.txtGimOut.TabIndex = 15;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(7, 61);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(113, 17);
            this.label6.TabIndex = 14;
            this.label6.Text = "Output Location:";
            // 
            // btnDo
            // 
            this.btnDo.Location = new System.Drawing.Point(352, 103);
            this.btnDo.Name = "btnDo";
            this.btnDo.Size = new System.Drawing.Size(75, 23);
            this.btnDo.TabIndex = 14;
            this.btnDo.Text = "Extract";
            this.btnDo.UseVisualStyleBackColor = true;
            this.btnDo.Click += new System.EventHandler(this.btnDo_Click);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.pictureBox1);
            this.groupBox3.Controls.Add(this.btnSaveDDS);
            this.groupBox3.Controls.Add(this.btnPNGLoc);
            this.groupBox3.Controls.Add(this.btnDDSLoc);
            this.groupBox3.Controls.Add(this.txtPNGSave);
            this.groupBox3.Controls.Add(this.label7);
            this.groupBox3.Controls.Add(this.txtDDSLoc);
            this.groupBox3.Controls.Add(this.label8);
            this.groupBox3.Location = new System.Drawing.Point(12, 377);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(780, 144);
            this.groupBox3.TabIndex = 17;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "DDS";
            // 
            // btnSaveDDS
            // 
            this.btnSaveDDS.Location = new System.Drawing.Point(352, 103);
            this.btnSaveDDS.Name = "btnSaveDDS";
            this.btnSaveDDS.Size = new System.Drawing.Size(75, 23);
            this.btnSaveDDS.TabIndex = 14;
            this.btnSaveDDS.Text = "Save";
            this.btnSaveDDS.UseVisualStyleBackColor = true;
            this.btnSaveDDS.Click += new System.EventHandler(this.btnSaveDDS_Click);
            // 
            // btnPNGLoc
            // 
            this.btnPNGLoc.Location = new System.Drawing.Point(724, 61);
            this.btnPNGLoc.Name = "btnPNGLoc";
            this.btnPNGLoc.Size = new System.Drawing.Size(56, 23);
            this.btnPNGLoc.TabIndex = 16;
            this.btnPNGLoc.Text = "...";
            this.btnPNGLoc.UseVisualStyleBackColor = true;
            this.btnPNGLoc.Click += new System.EventHandler(this.btnPNGLoc_Click);
            // 
            // btnDDSLoc
            // 
            this.btnDDSLoc.Location = new System.Drawing.Point(724, 21);
            this.btnDDSLoc.Name = "btnDDSLoc";
            this.btnDDSLoc.Size = new System.Drawing.Size(56, 23);
            this.btnDDSLoc.TabIndex = 16;
            this.btnDDSLoc.Text = "...";
            this.btnDDSLoc.UseVisualStyleBackColor = true;
            this.btnDDSLoc.Click += new System.EventHandler(this.btnDDSLoc_Click);
            // 
            // txtPNGSave
            // 
            this.txtPNGSave.Location = new System.Drawing.Point(118, 61);
            this.txtPNGSave.Name = "txtPNGSave";
            this.txtPNGSave.Size = new System.Drawing.Size(599, 22);
            this.txtPNGSave.TabIndex = 15;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(7, 61);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(100, 17);
            this.label7.TabIndex = 14;
            this.label7.Text = "PNG Location:";
            // 
            // txtDDSLoc
            // 
            this.txtDDSLoc.Location = new System.Drawing.Point(118, 21);
            this.txtDDSLoc.Name = "txtDDSLoc";
            this.txtDDSLoc.Size = new System.Drawing.Size(599, 22);
            this.txtDDSLoc.TabIndex = 15;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(7, 21);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(103, 17);
            this.label8.TabIndex = 14;
            this.label8.Text = "DDS Location :";
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new System.Drawing.Point(13, 88);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(100, 50);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 17;
            this.pictureBox1.TabStop = false;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(804, 531);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "PS4 Tools ImageUtil";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button bntImage;
        private System.Windows.Forms.TextBox txtInput;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtW;
        private System.Windows.Forms.TextBox txtH;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.RadioButton radioButton3;
        private System.Windows.Forms.RadioButton radioButton2;
        private System.Windows.Forms.RadioButton radioButton1;
        private System.Windows.Forms.Button btnOutput;
        private System.Windows.Forms.TextBox txtOutput;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button btnGim;
        private System.Windows.Forms.TextBox txtGimLoc;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button btnGimOut;
        private System.Windows.Forms.TextBox txtGimOut;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button btnDo;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Button btnSaveDDS;
        private System.Windows.Forms.Button btnPNGLoc;
        private System.Windows.Forms.Button btnDDSLoc;
        private System.Windows.Forms.TextBox txtPNGSave;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox txtDDSLoc;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.PictureBox pictureBox1;
    }
}


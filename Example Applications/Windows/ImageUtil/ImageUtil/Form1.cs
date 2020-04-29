using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ImageUtil
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if(radioButton1.Checked == true)
            {
                txtH.Enabled = false;
                txtW.Enabled = false;
                txtH.Text = "512";
                txtW.Text = "512";
            }
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton2.Checked == true)
            {
                txtH.Enabled = false;
                txtW.Enabled = false;
                txtH.Text = "1080";
                txtW.Text = "1920";
            }
        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton3.Checked == true)
            {
                txtH.Enabled = true;
                txtW.Enabled = true;
                txtH.Text = "";
                txtW.Text = "";
            }
        }

        private void bntImage_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            openFileDialog1.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            openFileDialog1.Title = "Select PS4 Image File";

            openFileDialog1.CheckFileExists = true;

            openFileDialog1.CheckPathExists = true;

            openFileDialog1.Filter = "PS4 Image File (*.*)|*.*";

            openFileDialog1.RestoreDirectory = true;

            openFileDialog1.Multiselect = false;

            openFileDialog1.ReadOnlyChecked = true;

            openFileDialog1.ShowReadOnly = true;
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                txtInput.Text = openFileDialog1.FileName;
            }
        }

        private void btnOutput_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "PS4 PNG Image|*.png";
            saveFileDialog1.Title = "Save an PS4 Image File ";
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                txtOutput.Text = saveFileDialog1.FileName;
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            //now we convert 
            try
            {
                PS4_Tools.Image.PNG png = new PS4_Tools.Image.PNG();
                png.Create_PS4_Compatible_PNG(txtInput.Text, txtOutput.Text, Convert.ToInt32(txtH.Text), Convert.ToInt32(txtW.Text));
                MessageBox.Show("Done");
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnGim_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            openFileDialog1.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            openFileDialog1.Title = "Select PS4 Gim File";

            openFileDialog1.CheckFileExists = true;

            openFileDialog1.CheckPathExists = true;

            openFileDialog1.Filter = "PS4 Image File (*.*)|*.*";

            openFileDialog1.RestoreDirectory = true;

            openFileDialog1.Multiselect = false;

            openFileDialog1.ReadOnlyChecked = true;

            openFileDialog1.ShowReadOnly = true;
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                txtGimLoc.Text = openFileDialog1.FileName;
            }
        }

        private void btnDo_Click(object sender, EventArgs e)
        {
            PS4_Tools.Image.GIMImages.GIM gim = new PS4_Tools.Image.GIMImages.GIM(txtGimLoc.Text);
            var listofimages = gim.ConvertToBitmaps();
        }

        public static System.Drawing.Bitmap BytesToBitmap(byte[] ImgBytes)
        {
            System.Drawing.Bitmap result = null;
            if (ImgBytes != null)
            {
                MemoryStream stream = new MemoryStream(ImgBytes);
                result = (System.Drawing.Bitmap)System.Drawing.Bitmap.FromStream(stream);
            }
            return result;
        }

        private void btnDDSLoc_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            openFileDialog1.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            openFileDialog1.Title = "Select PS4 Gim File";

            openFileDialog1.CheckFileExists = true;

            openFileDialog1.CheckPathExists = true;

            openFileDialog1.Filter = "PS4 Image File (*.*)|*.*";

            openFileDialog1.RestoreDirectory = true;

            openFileDialog1.Multiselect = false;

            openFileDialog1.ReadOnlyChecked = true;

            openFileDialog1.ShowReadOnly = true;
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                txtDDSLoc.Text = openFileDialog1.FileName;
                var byteimage = PS4_Tools.Image.DDS.GetBytesFromDDS(openFileDialog1.FileName);
                var item = BytesToBitmap(byteimage);
                pictureBox1.Image = item;
            }
        }

        private void btnPNGLoc_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "PS4 PNG Image|*.png";
            saveFileDialog1.Title = "Save an PS4 Image File ";
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                txtPNGSave.Text = saveFileDialog1.FileName;
            }
        }

        private void btnSaveDDS_Click(object sender, EventArgs e)
        {
            PS4_Tools.Image.DDS.SavePNGFromDDS(txtDDSLoc.Text, txtPNGSave.Text);
            MessageBox.Show("Done");
        }

        public void CreateDDS()
        {
            //PS4_Tools.Image.DDS.Windows.
        }
    }
}

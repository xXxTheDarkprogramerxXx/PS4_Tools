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

namespace TrophyUnlocker
{
    public partial class frmSettings : Form
    {
        public frmSettings()
        {
            InitializeComponent();
        }
        // Returns the human-readable file size for an arbitrary, 64-bit file size 
        // The default format is "0.### XB", e.g. "4.2 KB" or "1.434 GB"
        public string GetBytesReadable(long i)
        {
            // Get absolute value
            long absolute_i = (i < 0 ? -i : i);
            // Determine the suffix and readable value
            string suffix;
            double readable;
            if (absolute_i >= 0x1000000000000000) // Exabyte
            {
                suffix = "EB";
                readable = (i >> 50);
            }
            else if (absolute_i >= 0x4000000000000) // Petabyte
            {
                suffix = "PB";
                readable = (i >> 40);
            }
            else if (absolute_i >= 0x10000000000) // Terabyte
            {
                suffix = "TB";
                readable = (i >> 30);
            }
            else if (absolute_i >= 0x40000000) // Gigabyte
            {
                suffix = "GB";
                readable = (i >> 20);
            }
            else if (absolute_i >= 0x100000) // Megabyte
            {
                suffix = "MB";
                readable = (i >> 10);
            }
            else if (absolute_i >= 0x400) // Kilobyte
            {
                suffix = "KB";
                readable = i;
            }
            else
            {
                return i.ToString("0 B"); // Byte
            }
            // Divide by 1024 to get fractional value
            readable = (readable / 1024);
            // Return formatted number with suffix
            return readable.ToString("0.### ") + suffix;
        }
        private void frmSettings_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.Hide();
            //on close open the form type.
            switch(Properties.Settings.Default.AppType)
            {
                default:
                    Form1 frmm = new Form1();
                    frmm.ShowDialog();
                    //this.Close();
                    break;
            }
        }
        public static string AppCommonPath()
        {
            string returnstring = "";
            if (Properties.Settings.Default.TempPath != string.Empty)
            {
                returnstring = Properties.Settings.Default.TempPath + @"\Ps4Tools\";
            }
            else
            {
                returnstring = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Ps4Tools\";
            }
            return returnstring;
        }
        public static long DirSize(DirectoryInfo d)
        {
            long size = 0;
            // Add file sizes.
            FileInfo[] fis = d.GetFiles();
            foreach (FileInfo fi in fis)
            {
                size += fi.Length;
            }
            // Add subdirectory sizes.
            DirectoryInfo[] dis = d.GetDirectories();
            foreach (DirectoryInfo di in dis)
            {
                size += DirSize(di);
            }
            return size;
        }
        private void frmSettings_Load(object sender, EventArgs e)
        {
            switch (Properties.Settings.Default.AppType)
            {
                case "":
                    comboBox1.SelectedIndex = 0;
                    break;
                case "PKG":
                    comboBox1.SelectedIndex = 1;
                    break;
                case "Manual":
                    comboBox1.SelectedIndex = 2;
                    break;
                default:
                    comboBox1.SelectedIndex = 0;
                    break;
            }
            if (Properties.Settings.Default.TempPath == "")
            {
                textBox1.Text =  AppCommonPath();
            }
            else
            {
                textBox1.Text = Properties.Settings.Default.TempPath;
            }

           var dirsize = DirSize(new DirectoryInfo(textBox1.Text));

            lblSize.Text = "Temp Path Size : " +  GetBytesReadable(dirsize);


        }

        private void btnClean_Click(object sender, EventArgs e)
        {
            System.IO.DirectoryInfo di = new DirectoryInfo(textBox1.Text);

            foreach (FileInfo file in di.GetFiles())
            {
                file.Delete();
            }
            foreach (DirectoryInfo dir in di.GetDirectories())
            {
                dir.Delete(true);
            }
            var dirsize = DirSize(new DirectoryInfo(textBox1.Text));

            lblSize.Text = "Temp Path Size : " + GetBytesReadable(dirsize);

        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            //save settings on screen
            Properties.Settings.Default.AppType = comboBox1.SelectedItem.ToString();
            Properties.Settings.Default.TempPath = textBox1.Text;
            Properties.Settings.Default.Save();
            MessageBox.Show("Settings have been saved", "Saved", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}

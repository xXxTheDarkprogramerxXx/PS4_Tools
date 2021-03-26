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

namespace SaveData_Manager
{
    public partial class frmSettings : Form
    {
        public frmSettings()
        {
            InitializeComponent();
        }

        private void frmSettings_Load(object sender, EventArgs e)
        {
            if(string.IsNullOrEmpty(Properties.Settings.Default.BackupLocation))
            {
                txtSaveBackup.Text = Application.StartupPath + @"\SaveManager";
            }
            else
            {
                txtSaveBackup.Text = Properties.Settings.Default.BackupLocation;
            }
        }

        private void btnSaveLocation_Click(object sender, EventArgs e)
        {
            using (var fbd = new FolderBrowserDialog())
            {
                DialogResult result = fbd.ShowDialog();

                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    string path = fbd.SelectedPath + @"\SaveManager";
                    if(!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }

                    Properties.Settings.Default.BackupLocation = path;
                    Properties.Settings.Default.Save();
                    txtSaveBackup.Text = path;
                }
            }
        }
    }
}

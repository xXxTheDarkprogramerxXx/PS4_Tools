using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Tester
{
    public partial class Main : Form
    {
        public Main()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            openFileDialog1.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            openFileDialog1.Title = "Select PS4 PUP File";

            openFileDialog1.CheckFileExists = true;

            openFileDialog1.CheckPathExists = true;

            openFileDialog1.DefaultExt = "pup";

            openFileDialog1.Filter = "PS4 PUP File (*.pup)|*.pup";

            openFileDialog1.RestoreDirectory = true;

            openFileDialog1.Multiselect = false;

            openFileDialog1.ReadOnlyChecked = true;

            openFileDialog1.ShowReadOnly = true;



            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                FolderBrowserDialog folderDlg = new FolderBrowserDialog();
                folderDlg.ShowNewFolderButton = true;
                // Show the FolderBrowserDialog.  
                if (folderDlg.ShowDialog() == DialogResult.OK)
                {
                    PS4_Tools.PUP pupfunction = new PS4_Tools.PUP();
                    pupfunction.Unpack_PUP(openFileDialog1.FileName, folderDlg.SelectedPath);
                }
            }
        }
    }
}

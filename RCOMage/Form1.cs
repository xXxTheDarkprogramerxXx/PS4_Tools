using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using PS4_Tools;

namespace RCOMage
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void btnRCO_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            openFileDialog1.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            openFileDialog1.Title = "Select PS4 File";

            openFileDialog1.CheckFileExists = true;

            openFileDialog1.CheckPathExists = true;

            openFileDialog1.Filter = "PS4 File (*.*)|*.*";

            openFileDialog1.RestoreDirectory = true;

            openFileDialog1.Multiselect = false;

            openFileDialog1.ReadOnlyChecked = true;

            openFileDialog1.ShowReadOnly = true;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                //read RCO and Validate 
                PS4_Tools.RCO.RCOFile rcofile = PS4_Tools.RCO.ReadRco(openFileDialog1.FileName);
                listBox1.DataSource = rcofile.Header.ToList();
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            MessageBox.Show("This is an open source version of RCOMage v2 (ps4 and psv) this tool is not yet completed");
        }
    }
}

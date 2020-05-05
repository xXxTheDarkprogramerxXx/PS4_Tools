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

namespace PS4_PKG_Viewer
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
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
        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog opendialog = new OpenFileDialog();
            opendialog.Filter = "PS4 PKG File (.PKG)| *.PKG";//file type 
            opendialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyComputer);
            if(opendialog.ShowDialog() == DialogResult.OK)
            {
                PS4_Tools.PKG.SceneRelated.Unprotected_PKG PS4_PKG = PS4_Tools.PKG.SceneRelated.Read_PKG(opendialog.FileName);
                textBox1.Text = opendialog.FileName;
                pictureBox1.Image = BytesToBitmap(PS4_PKG.Icon);
                label2.Text = PS4_PKG.PS4_Title;
                dataGridView3.DataSource = null;
                label3.Text = PS4_PKG.Content_ID;
                DataTable dttemp = new DataTable();
                dttemp.Columns.Add("PARAM");
                dttemp.Columns.Add("VALUE");
                for (int i = 0; i < PS4_PKG.Param.Tables.Count; i++)
                {
                    dttemp.Rows.Add(PS4_PKG.Param.Tables[i].Name, PS4_PKG.Param.Tables[i].Value);
                }
                dataGridView1.DataSource = dttemp;
                dttemp = new DataTable();
                dttemp.Columns.Add("Tropy File");
                try
                {
                    for (int i = 0; i < PS4_PKG.Trophy_File.trophyItemList.Count; i++)
                    {
                        dttemp.Rows.Add(PS4_PKG.Trophy_File.trophyItemList[i].Name);
                    }
                }
                catch(Exception ex)
                {

                }
                dataGridView2.DataSource = dttemp;

                var items = PS4_PKG.Header.DisplayInfo();
                //PS4_Tools.PKG.SceneRelated.ExtarctPKG(opendialog.FileName);
                for (int i = 0; i < items.Count; i++)
                {
                    listBox1.Items.Add(items[i]);
                }

                //for (int i = 0; i < PS4_PKG.Entires.Length; i++)
                //{
                //    listBox2.Items.Add(PS4_PKG.Entires[i].id.ToString() + PS4_PKG.Entires[0].size.ToString() + PS4_PKG.Entires[0].offset.ToString() + PS4_PKG.Entires[i].is_encrypted.ToString() + PS4_PKG.Entires[i].key_index);
                //}
                BindingSource source = new BindingSource();
                List<PS4_Tools.Util.PS4PkgUtil.PackageEntry> listofitems = new List<PS4_Tools.Util.PS4PkgUtil.PackageEntry>();
                for (int i = 0; i < PS4_PKG.Entires.Length; i++)
                {
                    listofitems.Add(PS4_PKG.Entires[i]);
                    source.Add(PS4_PKG.Entires[i]);
                }
          
                dataGridView3.AutoGenerateColumns = true;
               
                dataGridView3.DataSource = source;
            }

           
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void dataGridView3_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {

        }

        private void extarctPKGToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PS4_Tools.PKG.SceneRelated.ExtarctPKG(textBox1.Text);
        }

        private void extractAndDecryptToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }
    }
}

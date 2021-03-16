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

namespace PS4_Trophy_Viewer
{
    public partial class trpmain : Form
    {
        public trpmain()
        {
            InitializeComponent();
        }
        public static string getBetween(string strSource, string strStart, string strEnd)
        {
            int Start, End;
            if (strSource.Contains(strStart) && strSource.Contains(strEnd))
            {
                Start = strSource.IndexOf(strStart, 0) + strStart.Length;
                End = strSource.IndexOf(strEnd, Start);
                return strSource.Substring(Start, End - Start);
            }
            else
            {
                return "";
            }
        }
        PS4_Tools.Trophy_File trophyfile = new PS4_Tools.Trophy_File();
        private void Form1_Load(object sender, EventArgs e)
        {
          
        }

        private void dgvmain_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            //get the spesific item value
            //for (int i = 0; i < trophyfile.trophyItemList.Count; i++)
            //{
            //    if(trophyfile.trophyItemList[i].Name == "TROP" + dgvmain.SelectedRows[0].Cells["ID"].Value.ToString().PadLeft(3, '0') + ".PNG")
            //    {
            //        using (var ms = new MemoryStream(trophyfile.trophyItemList[i].TotalBytes))
            //        {
            //            pictureBox1.Image =  Image.FromStream(ms);
            //        }
            //        int ID = 0;
            //        int.TryParse(dgvmain.SelectedRows[0].Cells["ID"].Value.ToString(), out ID);
            //        for (int ix = 0; ix < trophyfile.trophyconf.trophys.Count; ix++)
            //        {
            //            if(trophyfile.trophyconf.trophys[ix].id == ID)
            //            {
            //                txtDetails.Text = "Details: " + trophyfile.trophyconf.trophys[ix].detail;
            //            }
            //        }
                   
            //    }
            //}
        }

        private void dgvmain_MouseClick(object sender, MouseEventArgs e)
        {

        }

        private void dgvmain_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            //get the spesific item value
            for (int i = 0; i < trophyfile.trophyItemList.Count; i++)
            {
                if (trophyfile.trophyItemList[i].Name == "TROP" + dgvmain.SelectedRows[0].Cells["ID"].Value.ToString().PadLeft(3, '0') + ".PNG")
                {
                    using (var ms = new MemoryStream(trophyfile.trophyItemList[i].TotalBytes))
                    {
                        pictureBox1.Image = Image.FromStream(ms);
                    }
                    int ID = 0;
                    int.TryParse(dgvmain.SelectedRows[0].Cells["ID"].Value.ToString(), out ID);
                    for (int ix = 0; ix < trophyfile.trophyconf.trophys.Count; ix++)
                    {
                        if (trophyfile.trophyconf.trophys[ix].id == ID)
                        {
                            txtDetails.Text = "Details: " + trophyfile.trophyconf.trophys[ix].detail;
                        }
                    }

                }
            }
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            PS4_Tools.TROPHY.TROPCONF tconf = null;
            PS4_Tools.TROPHY.TROPCONF tconmain = null;
            PS4_Tools.TROPHY.TROPTRNS tpsn;
            PS4_Tools.TROPHY.TROPUSR tusr;
            //load trophy file
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            openFileDialog1.Title = "Select PS4 Trophy File";

            openFileDialog1.CheckFileExists = true;

            openFileDialog1.CheckPathExists = true;

            openFileDialog1.Filter = "PS4 File (*.*)|*.*";

            openFileDialog1.RestoreDirectory = true;

            openFileDialog1.Multiselect = false;

            openFileDialog1.ReadOnlyChecked = true;

            openFileDialog1.ShowReadOnly = true;
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                textBox1.Text = openFileDialog1.FileName;
                string NPCOMID = "";
                string Diroftrpy = Path.GetDirectoryName(openFileDialog1.FileName);
                if (File.Exists(Diroftrpy + @"\TRPPARAM.INI"))
                {
                    string inifile = File.ReadAllText(Diroftrpy + @"\TRPPARAM.INI");
                    NPCOMID = getBetween(inifile, "NPCOMMID=", "TROPAPPVER=");
                    if (NPCOMID == "")
                    {
                        MessageBox.Show("Can not find NPCOMMID");
                        return;
                    }
                    NPCOMID = NPCOMID.TrimEnd();
                }

                //string NPCOMID = "NPWR12903_00";
                trophyfile.Load(File.ReadAllBytes(openFileDialog1.FileName));

                for (int i = 0; i < trophyfile.trophyItemList.Count; i++)
                {
                    var itembytes = trophyfile.ExtractFileToMemory(trophyfile.trophyItemList[i].Name);
                    byte[] itemcontainer;
                    if (trophyfile.trophyItemList[i].Name == "TROPCONF.ESFM")
                    {


                        itemcontainer = PS4_Tools.Trophy_File.ESFM.LoadAndDecrypt(itembytes, NPCOMID);
                        tconf = new PS4_Tools.TROPHY.TROPCONF(itemcontainer);
                    }
                    if (trophyfile.trophyItemList[i].Name == "TROP.ESFM")
                    {
                        itemcontainer = PS4_Tools.Trophy_File.ESFM.LoadAndDecrypt(itembytes, NPCOMID);
                        tconmain = new PS4_Tools.TROPHY.TROPCONF(itemcontainer);
                        trophyfile.trophyconf = tconmain;
                    }
                }
                txtTitle.Text = tconmain.title_name;
                lblDetail.Text = tconmain.title_detail;

                DataTable dttemp = new DataTable();
                dttemp.Columns.Add("ID");
                dttemp.Columns.Add("Type");
                dttemp.Columns.Add("Hidden");
                dttemp.Columns.Add("Name");
                dttemp.Columns.Add("Description");
                for (int i = 0; i < tconmain.trophys.Count; i++)
                {
                    string Type = "";
                    switch (tconmain.trophys[i].ttype)
                    {
                        case "P":
                            Type = "Platinum";
                            break;
                        case "B":
                            Type = "Bronze";
                            break;
                        case "G":
                            Type = "Gold";
                            break;
                        case "S":
                            Type = "Silver";
                            break;
                        default:
                            break;
                    }

                    dttemp.Rows.Add(tconmain.trophys[i].id, Type, tconmain.trophys[i].hidden, tconmain.trophys[i].name, tconmain.trophys[i].detail);
                }
                dgvmain.DataSource = dttemp;
                //set first item
                int ID = 0;
                for (int i = 0; i < trophyfile.trophyItemList.Count; i++)
                {
                    if (trophyfile.trophyItemList[i].Name == "TROP" + ID.ToString().PadLeft(3, '0') + ".PNG")
                    {
                        using (var ms = new MemoryStream(trophyfile.trophyItemList[i].TotalBytes))
                        {
                            pictureBox1.Image = Image.FromStream(ms);
                        }
                        ID = 0;
                        int.TryParse(dgvmain.SelectedRows[0].Cells["ID"].Value.ToString(), out ID);
                        for (int ix = 0; ix < trophyfile.trophyconf.trophys.Count; ix++)
                        {
                            if (trophyfile.trophyconf.trophys[ix].id == ID)
                            {
                                txtDetails.Text = "Details: " + trophyfile.trophyconf.trophys[ix].detail;
                                break;
                            }
                        }
                        break;

                    }
                }

            }
        }
    }
}

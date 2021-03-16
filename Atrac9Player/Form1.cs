using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Atrac9Player
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        System.Media.SoundPlayer player;
        bool Playing = false;

        private void btnAt9_Click(object sender, EventArgs e)
        {
            if (Playing == false)
            {
                //Open File Items
                OpenFileDialog openFileDialog1 = new OpenFileDialog();

                openFileDialog1.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

                openFileDialog1.Title = "Select PS4 File";

                openFileDialog1.CheckFileExists = true;

                openFileDialog1.CheckPathExists = true;
                
                openFileDialog1.Filter = "PS4 File (*.*)|*.*|PS4 PKG (*.pkg)|*.pkg";

                openFileDialog1.RestoreDirectory = true;

                openFileDialog1.Multiselect = false;

                openFileDialog1.ReadOnlyChecked = true;

                openFileDialog1.ShowReadOnly = true;

                if (openFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    txtAt9.Text = "";
                    var ps4filetype = PS4_Tools.Tools.Get_PS4_File_Type(openFileDialog1.FileName);
                    //MessageBox.Show("File is a " + ps4filetype.ToString());
                    switch (ps4filetype)
                    {

                        
                        case PS4_Tools.Tools.File_Type.ATRAC9:
                            pnlLoading.Visible = true;
                            Application.DoEvents();
                            new Thread(new ThreadStart(delegate
                            {

                                var bytes = PS4_Tools.Media.Atrac9.LoadAt9(openFileDialog1.FileName);
                                if (this.txtAt9.InvokeRequired)
                                {
                                    this.txtAt9.BeginInvoke((MethodInvoker)delegate () { this.txtAt9.Text = openFileDialog1.FileName; });
                                }
                                else
                                {
                                    this.txtAt9.Text = openFileDialog1.FileName;
                                }
                                //txtAt9.Text = openFileDialog1.FileName;
                                player = new System.Media.SoundPlayer(new MemoryStream(bytes));
                                player.Play();
                                Playing = true;
                                if (this.btnPlay.InvokeRequired)
                                {
                                    this.btnPlay.BeginInvoke((MethodInvoker)delegate () { this.btnPlay.Image = Properties.Resources.baseline_stop_black_18dp; });
                                }
                                else
                                {
                                    this.btnPlay.Image = Properties.Resources.baseline_stop_black_18dp;
                                }
                                if (this.pnlLoading.InvokeRequired)
                                {
                                    this.pnlLoading.BeginInvoke((MethodInvoker)delegate () { this.pnlLoading.Visible = false; });
                                }
                                else
                                {
                                    this.pnlLoading.Visible = false;
                                }
                                //btnPlay.Image = Properties.Resources.baseline_stop_black_18dp;
                                //pnlLoading.Visible = false;
                                Application.DoEvents();
                            })).Start();
                            break;
                        case PS4_Tools.Tools.File_Type.PS4_PKG:
                            pnlLoading.Visible = true;
                            Application.DoEvents();
                            new Thread(new ThreadStart(delegate
                            {

                                PS4_Tools.PKG.SceneRelated.Unprotected_PKG PS4_PKG = PS4_Tools.PKG.SceneRelated.Read_PKG(openFileDialog1.FileName);
                                if (PS4_PKG.Sound != null)
                                {

                                    var bytes = PS4_Tools.Media.Atrac9.LoadAt9(PS4_PKG.Sound);
                                    if (this.txtAt9.InvokeRequired)
                                    {
                                        this.txtAt9.BeginInvoke((MethodInvoker)delegate () { this.txtAt9.Text = openFileDialog1.FileName; });
                                    }
                                    else
                                    {
                                        this.txtAt9.Text = openFileDialog1.FileName;
                                    }
                                    //txtAt9.Text = openFileDialog1.FileName;
                                    player = new System.Media.SoundPlayer(new MemoryStream(bytes));
                                    player.Play();
                                    Playing = true;
                                    if (this.btnPlay.InvokeRequired)
                                    {
                                        this.btnPlay.BeginInvoke((MethodInvoker)delegate () { this.btnPlay.Image = Properties.Resources.baseline_stop_black_18dp; });
                                    }
                                    else
                                    {
                                        this.btnPlay.Image = Properties.Resources.baseline_stop_black_18dp;
                                    }
                                   
                                }
                                if (this.pnlLoading.InvokeRequired)
                                {
                                    this.pnlLoading.BeginInvoke((MethodInvoker)delegate () { this.pnlLoading.Visible = false; });
                                }
                                else
                                {
                                    this.pnlLoading.Visible = false;
                                }
                                //btnPlay.Image = Properties.Resources.baseline_stop_black_18dp;
                                //pnlLoading.Visible = false;
                                Application.DoEvents();
                            })).Start();
                            break;
                    }
                }
            }
            else
            {
                player.Stop();

                btnPlay.Image = Properties.Resources.baseline_play_circle_outline_black_18dp;
                Playing = false;
            }
        }

        private void btnPlay_Click(object sender, EventArgs e)
        {
            if (Playing == false)
            {
                player.Play();
                Playing = true;
                btnPlay.Image = Properties.Resources.baseline_stop_black_18dp;
            }
            else
            {
                player.Stop();

                btnPlay.Image = Properties.Resources.baseline_play_circle_outline_black_18dp;
                Playing = false;
            }
        }
    }
}

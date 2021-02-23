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
using System.IO.Compression;
using System.Diagnostics;
using System.Net;

namespace TrophyUnlocker
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        private List<string> ListFiles(string url)
        {
            try
            {
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(url);
                request.Method = WebRequestMethods.Ftp.ListDirectory;

                request.Credentials = new NetworkCredential("anonymous", "");
                FtpWebResponse response = (FtpWebResponse)request.GetResponse();
                Stream responseStream = response.GetResponseStream();
                StreamReader reader = new StreamReader(responseStream);
                string names = reader.ReadToEnd();

                reader.Close();
                response.Close();

                return names.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries).ToList();
            }
            catch (Exception)
            {
                //   throw;
            }
            return new List<string>();
        }
        public static byte[] ReadFully(Stream input)
        {
            byte[] buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }
        private byte[] DownloadFile(string ftpSourceFilePath)
        {

            // byte[] buffer = new byte[2048];
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(ftpSourceFilePath);
            request.Credentials = new NetworkCredential("anonymous", "janeDoe@contoso.com");
            request.Method = WebRequestMethods.Ftp.DownloadFile;
            request.UsePassive = true;
            // FtpWebRequest request = CreateFtpWebRequest(ftpSourceFilePath, userName, password, true);
            request.Method = WebRequestMethods.Ftp.DownloadFile;

            Stream reader = request.GetResponse().GetResponseStream();
            byte[] buffer = ReadFully(reader);


            return buffer;
        }

        public class UserInfo
        {
            public string UserId { get; set; }
            public string Name { get; set; }
        }

        public class GameInfo
        {
            public string TitleId { get; set; }
            public byte[] npbind { get; set; }
            public byte[] nptitle { get; set; }
            public byte[] param { get; set; }
            public byte[] TrophyFile { get; set; }
            public string Title { get; set; }
            public string ContentID { get; set; }

        }
        List<GameInfo> gamelist = new List<GameInfo>();



        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text == "")
            {
                MessageBox.Show("Please enter an IP");
                return;
            }
            pnlProgress.Visible = true;
            backgroundWorker1.RunWorkerAsync();

        }

        private void button2_Click(object sender, EventArgs e)
        {
            //Open File Items
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
                try
                {
                    var trp = new PS4_Tools.Trophy_File();
                    trp.Load(File.ReadAllBytes(openFileDialog1.FileName));
                    if (trp.FileCount == 0)
                    {
                        MessageBox.Show("Trophy file not valid");
                    }
                    //textBox2.Text = openFileDialog1.FileName;
                }
                catch (Exception ex)
                {

                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            //Open File Items
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
                try
                {
                    PS4_Tools.PKG.SceneRelated.NP_Title titleid = new PS4_Tools.PKG.SceneRelated.NP_Title(openFileDialog1.FileName);
                    //textBox3.Text = openFileDialog1.FileName;
                    label6.Text = "NpTitle Id :" + titleid.Nptitle;
                }
                catch (Exception ex)
                {

                }
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            //Open File Items
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
                try
                {
                    PS4_Tools.PKG.SceneRelated.NP_Bind bind = new PS4_Tools.PKG.SceneRelated.NP_Bind(openFileDialog1.FileName);
                    //textBox4.Text = openFileDialog1.FileName;
                    label7.Text = "Np Bind : " + bind.Nptitle;
                }
                catch (Exception ex)
                {

                }
            }
        }

        public static string AppCommonPath()
        {
            string returnstring = "";
            //if (Properties.Settings.Default.OverwriteTemp == true && Properties.Settings.Default.TempPath != string.Empty)
            //{
            //    returnstring = Properties.Settings.Default.TempPath + @"\Ps4Tools\";
            //}
            //else
            {
                returnstring = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Ps4Tools\";
            }
            return returnstring;
        }
        public void UpdateInfo(string info)
        {
            //if (Properties.Settings.Default.EnableAdvancedMode == true)
            //{
            //   // advanced.Invoke(new Action(() => advanced.LabelText += info + Environment.NewLine));
            //}
        }


        /// <summary>
        /// This function will clean out a directory and then delete the directory
        /// </summary>
        /// <param name="target_dir">Supply the directory you want cleaned</param>
        public static void DeleteDirectory(string target_dir)
        {
            string[] files = Directory.GetFiles(target_dir);
            string[] dirs = Directory.GetDirectories(target_dir);

            foreach (string file in files)
            {
                File.SetAttributes(file, FileAttributes.Normal);
                File.Delete(file);
            }

            foreach (string dir in dirs)
            {
                DeleteDirectory(dir);
            }

            Directory.Delete(target_dir, false);
        }
        public void ExtractAllResources()
        {
            UpdateInfo("Checking Directory Paths");
            if (!Directory.Exists(AppCommonPath()))
            {
                UpdateInfo("Created Directory" + AppCommonPath());
                Directory.CreateDirectory(AppCommonPath());
            }
            if (!Directory.Exists(AppCommonPath() + @"\PS2Emu\"))
            {
                UpdateInfo("Created Directory" + AppCommonPath() + @"\PS2Emu\");
                Directory.CreateDirectory(AppCommonPath() + @"\PS2Emu\");
            }

            UpdateInfo("Writing All Binary Files to Temp Path....");

            //copy byte files
            System.IO.File.WriteAllBytes(AppCommonPath() + "orbis-pub-cmd.exe", Properties.Resources.orbis_pub_cmd);
            UpdateInfo("Writing Binary File to Temp Path " + "\n Written : " + AppCommonPath() + "orbis-pub-cmd.exe");
            System.IO.File.WriteAllBytes(AppCommonPath() + "CONTENTS.zip", Properties.Resources.CONTENTS);
            UpdateInfo("Writing Binary File to Temp Path " + "\n Written : " + AppCommonPath() + "CONTENTS.zip");
            System.IO.File.WriteAllBytes(AppCommonPath() + "ext.zip", Properties.Resources.ext);
            UpdateInfo("Writing Binary File to Temp Path " + "\n Written : " + AppCommonPath() + "ext.zip");
            System.IO.File.WriteAllBytes(AppCommonPath() + "Trophy_Unlocker.gp4", Properties.Resources.Trophy_Unlocker);
            UpdateInfo("Writing Binary File to Temp Path " + "\n Written : " + AppCommonPath() + "Trophy_Unlocker.gp4");


            UpdateInfo("Extracting Zip(s)");
            //extarct zip
            if (Directory.Exists(AppCommonPath() + @"\CONTENTS\"))
            {
                DeleteDirectory(AppCommonPath() + @"\CONTENTS\");
            }
            ZipFile.ExtractToDirectory(AppCommonPath() + "CONTENTS.zip", AppCommonPath() + @"\CONTENTS\");


            if (Directory.Exists(AppCommonPath() + @"\ext\"))
            {
                DeleteDirectory(AppCommonPath() + @"\ext\");
            }
            ZipFile.ExtractToDirectory(AppCommonPath() + "ext.zip", AppCommonPath());

            File.Delete(AppCommonPath() + "ext.zip");
            File.Delete((AppCommonPath() + "CONTENTS.zip"));
        }
        public string Orbis_Pub__GenCMD(string command, string arguments)
        {
            ProcessStartInfo start = new ProcessStartInfo();
            start.FileName = AppCommonPath() + "orbis-pub-gen.exe " + command;
            start.Arguments = arguments;
            start.UseShellExecute = false;
            start.RedirectStandardOutput = false;
            start.CreateNoWindow = false;
            using (Process process = Process.Start(start))
            {
                process.WaitForExit();
            }
            return "";
        }
        public string Orbis_CMD(string command, string arguments)
        {
            ProcessStartInfo start = new ProcessStartInfo();
            start.FileName = AppCommonPath() + "orbis-pub-cmd.exe " + command;
            start.Arguments = arguments;
            start.UseShellExecute = false;
            start.RedirectStandardOutput = true;
            start.CreateNoWindow = true;
            using (Process process = Process.Start(start))
            {
                process.ErrorDataReceived += Process_ErrorDataReceived; ;
                using (StreamReader reader = process.StandardOutput)
                {
                    string result = reader.ReadToEnd();
                    if (result.Contains("already converted from elf file to self file"))
                    {
                        DialogResult dlr = MessageBox.Show("Already Converted From Elf Error Found.... will be using Orbis-pub-gen for this pkg\n\n Simply Click Build and select the save folder", "Error with an alternative", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                        if (dlr == DialogResult.OK)
                        {
                            //this will open up the GP4 Project inside the Utility
                            Orbis_Pub__GenCMD("", AppCommonPath() + "Trophy_Unlocker.gp4");

                        }
                    }
                    else if (result.Contains("[Error]"))
                    {
                        MessageBox.Show(result);
                    }
                    return result;
                }
            }
        }

        private void Process_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            //throw new NotImplementedException();
        }
        FolderBrowserDialog saveFileDialog1 = new FolderBrowserDialog();
        private void button5_Click(object sender, EventArgs e)
        {
            ExtractAllResources();
            saveFileDialog1 = new FolderBrowserDialog();
            //saveFileDialog1.Filter = "PS4 PKG|*.pkg";
            //saveFileDialog1.Title = "Save an PS4 PKG File";
            //saveFileDialog1.ov
            if (DialogResult.OK != saveFileDialog1.ShowDialog())
            {
                return;
            }
            UpdateProgress("");
            pnlProgress.Visible = true;
            backgroundWorker2.RunWorkerAsync();



        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            //var gameselected = gamelist.Select(x => x.Title == comboBox1.SelectedText);
            var result = gamelist.Single(s => s.Title == comboBox1.SelectedItem);
            //poulate date
            label8.Text = "Game Title : " + result.Title;

            label5.Text = "Content ID : " + result.ContentID;

            PS4_Tools.PKG.SceneRelated.NP_Title titleid = new PS4_Tools.PKG.SceneRelated.NP_Title(result.nptitle);
            label6.Text = "NpTitle Id : " + titleid.Nptitle;

            PS4_Tools.PKG.SceneRelated.NP_Bind NP_Bind = new PS4_Tools.PKG.SceneRelated.NP_Bind(result.npbind);
            label7.Text = "Np Bind :  " + NP_Bind.Nptitle;

            txtPreviewTitle.Text = "Unlocker for " + result.Title;
        }

        private void UpdateProgress(string txt)
        {
            if (this.pnlProgress.InvokeRequired)
            {
                this.pnlProgress.BeginInvoke((MethodInvoker)delegate () { this.lblProgress.Text = txt; ; });
            }
            else
            {
                this.pnlProgress.Text = txt; ;
            }
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {


                /*We moved to FTP Alot Easier*/
                UpdateProgress("Connection to ps4");
                // Get the object used to communicate with the server.
                gamelist = new List<GameInfo>();
                string url = "ftp://" + textBox1.Text + ":1337/user/home/";
                UpdateProgress("Checking user directory");
                //var list = ListFiles(url);
                FtpWebRequest ftpRequest = (FtpWebRequest)WebRequest.Create(url);
                ftpRequest.Credentials = new NetworkCredential("anonymous", "janeDoe@contoso.com");
                ftpRequest.Method = WebRequestMethods.Ftp.ListDirectoryDetails;
                ftpRequest.UsePassive = true;
                FtpWebResponse response = (FtpWebResponse)ftpRequest.GetResponse();
                List<UserInfo> result = new List<UserInfo>();
                using (StreamReader streamReader = new StreamReader(response.GetResponseStream()))
                {
                    string line = streamReader.ReadLine();
                    while (!string.IsNullOrEmpty(line))
                    {
                        var splitt = line.Split(' ');
                        if (splitt.Length == 10)//only do this when 10 items in array
                        {
                            UserInfo info = new UserInfo();

                            byte[] userinfo = DownloadFile(url + splitt[9] + "/username.dat");
                            info.UserId = splitt[9];
                            info.Name = System.Text.Encoding.ASCII.GetString(userinfo);
                            UpdateProgress("User found ID" + info.UserId + " Name  " + info.Name);
                            result.Add(info);//save all user folders
                        }
                        line = streamReader.ReadLine();
                    }
                }
                UpdateProgress("Gatehring game info");
                //list all games here
                url = "ftp://" + textBox1.Text + ":1337/system_data/priv/appmeta/";
                ftpRequest = (FtpWebRequest)WebRequest.Create(url);
                ftpRequest.Credentials = new NetworkCredential("anonymous", "janeDoe@contoso.com");
                ftpRequest.Method = WebRequestMethods.Ftp.ListDirectoryDetails;
                ftpRequest.UsePassive = true;
                response = (FtpWebResponse)ftpRequest.GetResponse();

                using (StreamReader streamReader = new StreamReader(response.GetResponseStream()))
                {
                    string line = streamReader.ReadLine();
                    while (!string.IsNullOrEmpty(line))
                    {
                        var splitt = line.Split(' ');
                        if (splitt.Length == 9)//only do this when 10 items in array
                        {
                            if (splitt[8] != "..")
                            {
                                try
                                {
                                    GameInfo info = new GameInfo();


                                    UpdateProgress("Game found " + splitt[8]);
                                    UpdateProgress("Downloading Param.SFO for " + splitt[8]);
                                    byte[] paramsfo = DownloadFile(url + splitt[8] + "/param.sfo");
                                    info.param = paramsfo;
                                    UpdateProgress("Downloading nptitle.dat for " + splitt[8]);
                                    info.nptitle = DownloadFile(url + splitt[8] + "/nptitle.dat"); //System.Text.Encoding.ASCII.GetString(userinfo);
                                    UpdateProgress("Downloading npbind.dat for " + splitt[8]);
                                    info.npbind = DownloadFile(url + splitt[8] + "/npbind.dat");
                                    //get the TrophyFile
                                    UpdateProgress("Downloading TROPHY.TRP for " + splitt[8]);
                                    var bindinfo = new PS4_Tools.PKG.SceneRelated.NP_Bind(info.npbind);
                                    string tmpurl = "ftp://" + textBox1.Text + ":1337/user/trophy/conf/" + bindinfo.Nptitle + "/";
                                    info.TrophyFile = DownloadFile(tmpurl + "TROPHY.TRP");
                                    gamelist.Add(info);
                                    var sfofile = PS4_Tools.PKG.SceneRelated.PARAM_SFO.Get_Param_SFO(info.param);
                                    info.TitleId = sfofile.TitleID;
                                    info.Title = sfofile.Title;
                                    info.ContentID = sfofile.ContentID;


                                }
                                catch (Exception ex)
                                {

                                }
                            }
                        }
                        line = streamReader.ReadLine();
                    }
                }


                //FtpWebRequest request = (FtpWebRequest)WebRequest.Create("ftp://"+textBox1.Text + ":1337");
                //request.Method = WebRequestMethods.Ftp.DownloadFile;

                //// This example assumes the FTP site uses anonymous logon.
                //request.Credentials = new NetworkCredential("anonymous", "");

                //response = (FtpWebResponse)request.GetResponse();

                //Stream responseStream = response.GetResponseStream();
                //StreamReader reader = new StreamReader(responseStream);
                //Console.WriteLine(reader.ReadToEnd());

                //Console.WriteLine($"Download Complete, status {response.StatusDescription}");

                //reader.Close();
                //response.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error " + ex.Message, "Error while loading game data");
            }
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            for (int i = 0; i < gamelist.Count; i++)
            {
                comboBox1.Items.Add(gamelist[i].Title);
            }
            if (gamelist.Count != 0)
            {
                comboBox1.SelectedIndex = 0;
            }

            pnlProgress.Visible = false;
        }
        public static string RemoveSpecialCharacters(string str)
        {
            StringBuilder sb = new StringBuilder();
            foreach (char c in str)
            {
                if ((c >= '0' && c <= '9') || (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || c == '.' || c == '_' || c == ' ')
                {
                    sb.Append(c);
                }
            }
            return sb.ToString();
        }
        private void backgroundWorker2_DoWork(object sender, DoWorkEventArgs e)
        {
            string selectedgame = "";
            if (this.comboBox1.InvokeRequired)
            {
                this.comboBox1.BeginInvoke((MethodInvoker)delegate () { selectedgame = comboBox1.SelectedItem.ToString(); });
            }
            else
            {
                selectedgame = comboBox1.SelectedItem.ToString();
            }
            GameInfo result = new GameInfo();
            for (int i = 0; i < gamelist.Count; i++)
            {
                System.Threading.Thread.Sleep(100);
                if (gamelist[i].Title == selectedgame)
                {
                    result = gamelist[i];
                }
            }
            UpdateProgress("Copying Files to temp dir");
            //var result = gamelist.Single(s => s.Title == selectedgame);
            //First We Do Param.Sfo
            UpdateProgress("Copying Param.sfo");
            if (File.Exists(AppCommonPath() + @"CONTENTS\sce_sys\param.sfo"))
            {
                File.Delete(AppCommonPath() + @"CONTENTS\sce_sys\param.sfo");
            }
            File.WriteAllBytes(AppCommonPath() + @"CONTENTS\sce_sys\param.sfo", result.param);
            //File.Copy(, AppCommonPath() + @"CONTENTS\sce_sys\param.sfo", true);
            //Next Trophy
            UpdateProgress("Copying trophy00.trp");
            if (File.Exists(AppCommonPath() + @"CONTENTS\sce_sys\trophy\trophy00.trp"))
            {
                File.Delete(AppCommonPath() + @"CONTENTS\sce_sys\trophy\trophy00.trp");
            }
            File.WriteAllBytes(AppCommonPath() + @"CONTENTS\sce_sys\trophy\trophy00.trp", result.TrophyFile);
            // File.Copy(textBox2.Text, AppCommonPath() + @"CONTENTS\sce_sys\trophy\trophy00.trp", true);
            //np title
            UpdateProgress("Copying nptitle.dat");
            if (File.Exists(AppCommonPath() + @"CONTENTS\sce_sys\nptitle.dat"))
            {
                File.Delete(AppCommonPath() + @"CONTENTS\sce_sys\nptitle.dat");
            }
            File.WriteAllBytes(AppCommonPath() + @"CONTENTS\sce_sys\nptitle.dat", result.nptitle);
            //File.Copy(textBox3.Text, AppCommonPath() + @"CONTENTS\sce_sys\nptitle.dat", true);
            //np bind
            UpdateProgress("Copying npbind.dat");
            if (File.Exists(AppCommonPath() + @"CONTENTS\sce_sys\npbind.dat"))
            {
                File.Delete(AppCommonPath() + @"CONTENTS\sce_sys\npbind.dat");
            }
            File.WriteAllBytes(AppCommonPath() + @"CONTENTS\sce_sys\npbind.dat", result.npbind);
            //File.Copy(textBox4.Text, AppCommonPath() + @"CONTENTS\sce_sys\npbind.dat", true);
            string tmpcontentid = "EP2165-XDPX30000_00-TROPHYUNLOCKXXXX";
            //string tmpcontentid = "EP2165-CUSA09960_00-F13GAMEDISCXXXXX";

            UpdateProgress("Fixing Param.sfo");
            var sfo = new Param_SFO.PARAM_SFO(AppCommonPath() + @"CONTENTS\sce_sys\param.sfo");
            //save the title as Unlocker For
            for (int i = 0; i < sfo.Tables.Count; i++)
            {
                if (sfo.Tables[i].Name == "TITLE")
                {
                    var tempitem = sfo.Tables[i];
                    tempitem.Value = "Unlocker for " + sfo.Title;
                    sfo.Tables[i] = tempitem;
                }
                if (sfo.Tables[i].Name == "APP_VER")
                {
                    var tempitem = sfo.Tables[i];
                    tempitem.Value = "01.00";
                    sfo.Tables[i] = tempitem;
                }
                //CONTENT_ID
                //if (sfo.Tables[i].Name == "CONTENT_ID")
                //{
                //    var tempitem = sfo.Tables[i];
                //    tempitem.Value = tmpcontentid;
                //    sfo.Tables[i] = tempitem;
                //}

                //if (sfo.Tables[i].Name == "TITLE_ID")
                //{
                //    var tempitem = sfo.Tables[i];
                //    tempitem.Value = "XDPX30000";
                //    sfo.Tables[i] = tempitem;
                //}
            }
            sfo.SaveSFO(sfo, AppCommonPath() + @"CONTENTS\sce_sys\param.sfo");
            //we save it with the new title
            //sfo.Tables = "Unlocker for " + sfo.Title;
            //now build the pkg with the new sfo

            UpdateProgress("Creating GP4 Project");
            var project = PS4_Tools.PKG.SceneRelated.GP4.ReadGP4(AppCommonPath() + "Trophy_Unlocker.gp4");
            UpdateProgress("GP4 content id " + sfo.ContentID);
            project.Volume.Package.Content_id = sfo.ContentID;//this hould be litrally all we need

            PS4_Tools.PKG.SceneRelated.GP4.SaveGP4(AppCommonPath() + "Trophy_Unlocker.gp4", project);


            //now build the pkg
            UpdateProgress("Creating PKG");
            bool BusyCoping = true;
            new System.Threading.Thread(new System.Threading.ThreadStart(delegate
            {
                Orbis_CMD("", "img_create --oformat pkg \"" + AppCommonPath() + "Trophy_Unlocker.gp4\" \"" + saveFileDialog1.SelectedPath + "\"");
                //orbis_pub_cmd.exe img_create --skip_digest --oformat pkg C:\Users\3deEchelon\AppData\Roaming\Ps4Tools\PS2Emu\PS2Classics.gp4 C:\Users\3deEchelon\AppData\Roaming\Ps4Tools\PS2Emu\
                BusyCoping = false;
            })).Start();

            while (BusyCoping == true)
            {
                Application.DoEvents();
                //Thread.Sleep(TimeSpan.FromSeconds(5));//sleep for 5 seconds
            }
            //saved
            //get the app version 
            string version = "";
            for (int i = 0; i < sfo.Tables.Count; i++)
            {
                if (sfo.Tables[i].Name == "VERSION")
                {
                    var tempitem = sfo.Tables[i];
                    version = tempitem.Value;
                }

            }
            UpdateProgress("Renaming PKG");
            if (File.Exists(saveFileDialog1.SelectedPath + @"\" + RemoveSpecialCharacters(sfo.Title) + ".pkg"))
            {
                File.Delete(saveFileDialog1.SelectedPath + @"\" + RemoveSpecialCharacters(sfo.Title) + ".pkg");
            }
            File.Move(saveFileDialog1.SelectedPath + @"\" + sfo.ContentID + "-A" + sfo.APP_VER.Replace(".", "") + "-V" + version.Replace(".", "") + ".pkg", saveFileDialog1.SelectedPath + @"\" + RemoveSpecialCharacters(sfo.Title) + ".pkg");
            // PS4_Tools.PKG.SceneRelated.Rename_pkg_To_Title(saveFileDialog1.SelectedPath +@"\"+ sfo.ContentID +"-A" + sfo.APP_VER.Replace(".","") + "-V"+ version.Replace(".","")+ ".pkg", saveFileDialog1.SelectedPath + @"\",true);
            ////delete the old one 
            //if(File.Exists(saveFileDialog1.SelectedPath + @"\" + sfo.ContentID + "-A" + sfo.APP_VER.Replace(".", "") + "-V" + version.Replace(".", "") + ".pkg"))
            //{
            //    File.Delete(saveFileDialog1.SelectedPath + @"\" + sfo.ContentID + "-A" + sfo.APP_VER.Replace(".", "") + "-V" + version.Replace(".", "") + ".pkg");
            //}
            UpdateProgress("Done");
            Process.Start(saveFileDialog1.SelectedPath);
        }

        private void backgroundWorker2_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            pnlProgress.Visible = false;
        }
    }
}

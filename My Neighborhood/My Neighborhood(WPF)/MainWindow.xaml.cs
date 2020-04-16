using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Ribbon;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using FluentFTP;
using Fakekit_API;
using System.Threading;
using System.Net.Sockets;
using System.IO;
using System.Diagnostics;

namespace My_Neighborhood_WPF_
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : RibbonWindow
    {
        #region << Var's >>

        #region << Enums >>

        enum Verbs
        {
            WILL = 251,
            WONT = 252,
            DO = 253,
            DONT = 254,
            IAC = 255
        }

        enum Options
        {
            SGA = 3
        }

        public enum ConnectionStatus
        {
            Connected,
            NotConnected,
            Connecting,
            Discconecting,
            Unknown,
        }
        public enum SKU_Power_State
        {
            On,
            Off,
            RestMode,
            DEAD
        }

        public enum ReleaseCheck
        {
            Release,
            Debug,
            Unknown
        }

        #endregion << Enums >>

        System.Threading.Thread thread = null;

        private string client_comp_ver = "0.98";

        TelnetConnection tc;
        FtpClient clients = new FtpClient();

        public static List<SKUS> SKU_List = new List<SKUS>();

        FtpReply response;
        #endregion << Var's >>

        #region << Classes >>

        public class TelnetConnection
        {
            TcpClient tcpSocket;


            int TimeOutMs = 100;

            public TelnetConnection(string Hostname, int Port)
            {

                try
                {
                    tcpSocket = new TcpClient(Hostname, Port);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }

            }

            public void Disconnect()
            {
                try
                {
                    tcpSocket.Close();
                    tcpSocket.Dispose();
                }
                catch (Exception ex)
                {
                    //MessageBox.Show(ex.ToString());
                }
            }
            public void Telnetdis()
            {

                try
                {


                    tcpSocket.Close();

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }

            }

            public string Login(string Username, string Password, int LoginTimeOutMs)
            {
                int oldTimeOutMs = TimeOutMs;
                TimeOutMs = LoginTimeOutMs;
                string s = Read();
                if (!s.TrimEnd().EndsWith(":"))
                    throw new Exception("Failed to connect : no login prompt");
                WriteLine(Username);

                s += Read();
                if (!s.TrimEnd().EndsWith(":"))
                    throw new Exception("Failed to connect : no password prompt");
                WriteLine(Password);

                s += Read();
                TimeOutMs = oldTimeOutMs;
                return s;
            }

            public void WriteLine(string cmd)
            {
                Write(cmd + "\n");
            }

            public void Write(string cmd)
            {
                if (!tcpSocket.Connected) return;
                byte[] buf = System.Text.ASCIIEncoding.ASCII.GetBytes(cmd.Replace("\0xFF", "\0xFF\0xFF"));
                tcpSocket.GetStream().Write(buf, 0, buf.Length);
            }


            public void WriteNothing()
            {
                if (!tcpSocket.Connected) return;
                byte[] buf = { 0 };
                tcpSocket.GetStream().Write(buf, 0, buf.Length);
            }

            public string Read()
            {
                if (!tcpSocket.Connected) return null;
                StringBuilder sb = new StringBuilder();
                do
                {
                    ParseTelnet(sb);
                    System.Threading.Thread.Sleep(TimeOutMs);
                } while (tcpSocket.Available > 0);
                return sb.ToString();
            }

            public bool IsConnected
            {
                get { return tcpSocket.Connected; }
            }


            void ParseTelnet(StringBuilder sb)
            {
                while (tcpSocket.Available > 0)
                {
                    int input = tcpSocket.GetStream().ReadByte();
                    switch (input)
                    {
                        case -1:
                            break;
                        case (int)Verbs.IAC:
                            // interpret as command
                            int inputverb = tcpSocket.GetStream().ReadByte();
                            if (inputverb == -1) break;
                            switch (inputverb)
                            {
                                case (int)Verbs.IAC:
                                    //literal IAC = 255 escaped, so append char 255 to string
                                    sb.Append(inputverb);
                                    break;
                                case (int)Verbs.DO:
                                case (int)Verbs.DONT:
                                case (int)Verbs.WILL:
                                case (int)Verbs.WONT:
                                    // reply to all commands with "WONT", unless it is SGA (suppres go ahead)
                                    int inputoption = tcpSocket.GetStream().ReadByte();
                                    if (inputoption == -1) break;
                                    tcpSocket.GetStream().WriteByte((byte)Verbs.IAC);
                                    if (inputoption == (int)Options.SGA)
                                        tcpSocket.GetStream().WriteByte(inputverb == (int)Verbs.DO ? (byte)Verbs.WILL : (byte)Verbs.DO);
                                    else
                                        tcpSocket.GetStream().WriteByte(inputverb == (int)Verbs.DO ? (byte)Verbs.WONT : (byte)Verbs.DONT);
                                    tcpSocket.GetStream().WriteByte((byte)inputoption);
                                    break;
                                default:
                                    break;
                            }
                            break;
                        default:
                            sb.Append((char)input);
                            break;
                    }
                }
            }
        }

        [Serializable]
        public class SKUS
        {
            public string Default { get; set; }
            public string Target { get; set; }
            public string SDK { get; set; }
            public ConnectionStatus Status { get; set; }
            public string Address { get; set; }

            public SKU_Power_State Power { get; set; }

            public string FileSerDir { get; set; }

            public DateTime? Expire { get; set; }

            public ReleaseCheck Release { get; set; }

            public double Progress { get; set; }
            public string ProgressName { get; set; }

            public string DisplayProgress
            {
                get { return ProgressName + Progress.ToString(); }
            }
        }

        #endregion << Classes >>

        #region << Methods >>
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Ribbon_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void btnCopyfiles_Click(object sender, RoutedEventArgs e)
        {
            var selecteditem = lstSKU.SelectedItem as SKUS;
            if (selecteditem != null)
            {
                //now we do some stuff
                MessageBoxResult mesgresult = MessageBox.Show("Are you sure you want to copy files from your serving directory to the app/data/ folder ?\nThis will overwrite any existing data", "Are you sure?", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (mesgresult == MessageBoxResult.Yes)
                {
                    string[] allfiles = Directory.GetFiles(selecteditem.FileSerDir, "*.*", SearchOption.AllDirectories);

                    clients.UploadDirectory(selecteditem.FileSerDir, "/data/app/", FtpFolderSyncMode.Update, FtpRemoteExists.Overwrite);//overwrite existing files
                }
            }
        }

        private void btnPlayGo_Click(object sender, RoutedEventArgs e)
        {
            //API.attachEboot();
        }

        private void btnChangeColor_Click(object sender, RoutedEventArgs e)
        {
            if (true == false)
            {
                API.Change_Light(API.colored_light.Blinking_Blue);//user can change color fo the led bar on console
            }
        }

        private void btnPowerOff_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var mess = MessageBox.Show("Are you sure you want to turn off the device ?", "Power off", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (mess == MessageBoxResult.Yes)
                {
                    API.POWER_OFF();
                }
            }
            catch (Exception ex)
            {

            }
        }

        private void btnReboot_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var mess = MessageBox.Show("Are you sure you want to reboot the device ?", "Reboot", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (mess == MessageBoxResult.Yes)
                {
                    API.REBOOT();
                }
            }
            catch (Exception ex)
            {

            }
        }

        private void btnRestMode_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var mess = MessageBox.Show("Are you sure you want to suspend the device ?", "Suspend", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (mess == MessageBoxResult.Yes)
                {
                    API.SUSPEND_SYSTEM();
                }
            }
            catch (Exception ex)
            {

            }
        }

        private void btnInstallPKG_Click(object sender, RoutedEventArgs e)
        {
            //Install a pkg selected by the user
            System.Windows.Forms.OpenFileDialog opendialog = new System.Windows.Forms.OpenFileDialog();
            opendialog.Filter = "PS4 PKG File (.PKG)| *.PKG";//file type 
            opendialog.Title = "Select a PS4 PKG file";
            opendialog.Multiselect = false;
            opendialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyComputer);
            if (opendialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                PS4_Tools.PKG.SceneRelated.Unprotected_PKG pkg = PS4_Tools.PKG.SceneRelated.Read_PKG(opendialog.FileName);
                if (pkg.PS4_Title != "")
                {
                    //install it 
                    //API.Installpkg();
                }
            }
        }

        private void RibbonWindow_Loaded(object sender, RoutedEventArgs e)
        {
            RefreshData();
            ExtractResources();
        }

        private void lstSKU_SizeChanged(object sender, SizeChangedEventArgs e)
        {

        }

        private void lstSKU_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                //on item selection changed
                //if item is connected 
                //enable all functions
                var selecteditem = (SKUS)lstSKU.SelectedItem;//try casting it to skus
                SetSkuUIItems(selecteditem);
                if (selecteditem.Default == "")
                {
                    btnSetDefault.IsEnabled = true;
                }
                else
                {
                    btnSetDefault.IsEnabled = false;
                }

                AutoSizeColumns();


            }
            catch (Exception ex)
            {

            }
        }

        public void SetProgress(string Text, double Percentage, bool hide = false)
        {
            try
            {
                actualStatus.Dispatcher.Invoke(new Action(() => actualStatus.Width = 0));
                StatusProgress.Dispatcher.Invoke(new Action(() => StatusProgress.Width = 250));
                //just for testing add a progress bar
                SKUS selecteditem = new SKUS();
                lstSKU.Dispatcher.Invoke(new Action(() => selecteditem = (SKUS)lstSKU.SelectedItem));
                selecteditem.Progress = Percentage;
                selecteditem.ProgressName = Text + " - " + (Math.Round(Percentage, 2)) + "%";

                //SKU_List[0].Progress = Percentage;
                //SKU_List[0].ProgressName = Text + " - ";
                //OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));

                if (hide == true)
                {
                    actualStatus.Dispatcher.Invoke(new Action(() => actualStatus.Width = 250));
                    StatusProgress.Dispatcher.Invoke(new Action(() => StatusProgress.Width = 0));
                }

                lstSKU.Dispatcher.Invoke(new Action(() => lstSKU.Items.Refresh()));
            }
            catch (Exception ex)
            {

            }
        }

        public void AutoSizeColumns()
        {
            GridView gv = lstSKU.View as GridView;
            if (gv != null)
            {
                foreach (var c in gv.Columns)
                {
                    // Code below was found in GridViewColumnHeader.OnGripperDoubleClicked() event handler (using Reflector)
                    // i.e. it is the same code that is executed when the gripper is double clicked
                    if (double.IsNaN(c.Width))
                    {
                        c.Width = c.ActualWidth;
                    }
                    c.Width = double.NaN;

                }
                gv.Columns[4].Width = 0;
            }
        }

        private void RibbonButton_Click(object sender, RoutedEventArgs e)
        {
            //actualStatus.Width = 0;
            //StatusProgress.Width = 150;
            ////just for testing add a progress bar
            //var selecteditem = (SKUS)lstSKU.SelectedItem;
            //selecteditem.Progress = 50;
            //selecteditem.ProgressName = "Rebooting - ";

            //SKU_List[0].Progress = 50;
            //SKU_List[0].ProgressName = "Rebooting - ";
            ////OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            //lstSKU.Items.Refresh();


            //Well this will obviously not work 

        }

        private void LoadProcess_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var item = e.Source as RibbonMenuItem;
                //load whatever you need to load here 
                //geussing we attach to the process from here
                API.attach(item.Header.ToString().Split('(')[0]);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void KillProcess_Click(object sender, RoutedEventArgs e)
        {
            var item = e.Source as RibbonMenuItem;
            if (item.Header.ToString() == "Kill all processes")
            {

            }
            else
            {
                API.kill(Convert.ToInt32(item.Tag));
            }
        }
        private void btnConnect_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (clients == null || !clients.IsConnected || !API.isConnected())
                {
                    if (lstSKU.SelectedItem == null)
                    {

                    }
                    else
                    {
                        var currentsku = lstSKU.SelectedItem as SKUS;
                        connect(currentsku);

                        try
                        {
                            //once connected load some information 
                            currentsku.SDK = API.Firmware();
                            currentsku.Power = SKU_Power_State.On;
                            currentsku.Status = ConnectionStatus.Connected;
                            lstSKU.Items.Refresh();
                            SetSkuUIItems(currentsku);
                            if (tc != null)
                            {
                                tc.Disconnect();
                            }
                            tc = new TelnetConnection(currentsku.Address, 777);//this is to be added each time an item is cloecked


                        }
                        catch (Exception ex)
                        {

                        }
                    }
                }
                else
                {
                    MessageBox.Show("Already Connected\n");
                }
            }
            catch (Exception ex)
            {

            }
        }

        private static byte[] ELF_Magic = new byte[] { 127, 69, 76, 70 };

        private void btnLoadApp_Click(object sender, RoutedEventArgs e)
        {
            Action<FtpProgress> progress = delegate (FtpProgress p) {
                if (p.Progress == 1)
                {

                    // all done!
                    SetProgress("Copying files", p.Progress);

                }
                else
                {

                    //Console.WriteLine(p.Progress / 100 + "%");
                    SetProgress("Copying files (" + (p.FileIndex + 1) + "/" + p.FileCount + ") ", p.Progress);
                    // percent done = (p.Progress * 100)

                }
            };

            List<string> UnityExlusions = new List<string>();
            UnityExlusions.Add("configuration.ps4path");
            UnityExlusions.Add("pronunciation.sig");
            UnityExlusions.Add("pronunciation.xml");
            UnityExlusions.Add("shareparam.json");
            FluentFTP.Rules.FtpFileNameRule ftprulesforunity = new FluentFTP.Rules.FtpFileNameRule(false, UnityExlusions);
            var listofrules = new List<FluentFTP.Rules.FtpRule>();
            listofrules.Add(ftprulesforunity);

            //load application this we will need to desciss
            var item = lstSKU.SelectedItem as SKUS;

            SetProgress("Prepping files...", 0);



            //first we need to see how many items have to be signed dam untiy doesn't sign files
            SetProgress("Checking file integrity...", 0);
            var prxfiles = System.IO.Directory.GetFiles(item.FileSerDir, "*.prx", SearchOption.AllDirectories);
            var ebootfiles = System.IO.Directory.GetFiles(item.FileSerDir, "*.bin", SearchOption.AllDirectories);

            for (int i = 0; i < prxfiles.Length; i++)
            {
                BinaryReader prxreader = new BinaryReader(new FileStream(prxfiles[i], FileMode.Open, FileAccess.Read));
                var bytes = prxreader.ReadBytes(4);
                if (Util.CompareBytes(bytes, ELF_Magic))
                {
                    //items are not signed
                    SetProgress("Signing " + System.IO.Path.GetFileName(prxfiles[i]), 50);
                    Orbis_CMD("", "\"" + prxfiles[i] + "\" " + " \"" + prxfiles[i] + "\"");//hopefully it over writes it 
                }

            }


            //API.Launch_APP("XDPX20002"); //Launches a spesific app
            Task.Factory.StartNew(() =>
            {
                List<string> Errors = new List<string>();

                var ftpresult = clients.UploadDirectory(item.FileSerDir, @"/hostapp/app/", FtpFolderSyncMode.Mirror, FtpRemoteExists.Overwrite, FtpVerify.None, listofrules, progress);//all items from file serving directory need to be copied
                for (int i = 0; i < ftpresult.Count; i++)
                {
                    if (ftpresult[i].IsFailed == true)
                    {
                        Errors.Add("Error " + ftpresult[i].Exception.InnerException.Message + " on file " + ftpresult[i].LocalPath);
                    }
                    if (ftpresult[i].IsSkipped == true)
                    {
                        //Errors.Add("Skipped files " + ftpresult[i].LocalPath);
                    }
                }
                if (Errors.Count == 0)
                {
                    SetProgress("", 100, true);
                }
                else
                {
                    SetProgress("Completed with errors..", 0, false);
                    string erro = "";
                    for (int i = 0; i < Errors.Count; i++)
                    {
                        erro += Errors[i].ToString() + "\n";
                    }
                    MessageBox.Show(erro, "Errors with file uploads", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                //quick check untiy has some funky items that causes error on start up 
                //to avoid 
                //we need to delete the following items 
                //Any 

                API.Launch_Eboot();
            });
        }

        private void btnFilterTarget_Click(object sender, RoutedEventArgs e)
        {
            //Filter items
        }

        private void btnAddTarget_Click(object sender, RoutedEventArgs e)
        {
            //Add item to SKU list
            AddSKU skuitems = new AddSKU();
            skuitems.ShowDialog();
            lstSKU.Items.Refresh();
            if (skuitems.SaveSKU == true)
            {
                SaveSKU_S();
            }

        }

        private void btnRemoveTarget_Click(object sender, RoutedEventArgs e)
        {
            var selectedsku = lstSKU.SelectedItem as SKUS;
            if (selectedsku != null)
            {
                MessageBoxResult result = MessageBox.Show("Are you sure you want to delete this target ?", "Delete ?", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    SKU_List.Remove(selectedsku);
                    SaveSKU_S();
                    lstSKU.Items.Refresh();
                }
            }
        }

        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            RefreshData(true);
        }

        private void btnSetDefault_Click(object sender, RoutedEventArgs e)
        {
            var selectedsku = lstSKU.SelectedItem as SKUS;
            if (selectedsku != null)
            {
                //set net default target
                for (int i = 0; i < SKU_List.Count; i++)
                {
                    SKU_List[i].Default = "";
                }
            }
            selectedsku.Default = "★";
        }

        private void btnSetfilserdir_Click(object sender, RoutedEventArgs e)
        {
            //now set the new item 
            var selecteditem = lstSKU.SelectedItem as SKUS;

            //this sets the file serving directory the place of your eboot no need to make it a pkg
            var dialog = new System.Windows.Forms.FolderBrowserDialog();
            dialog.ShowNewFolderButton = false;
            System.Windows.Forms.DialogResult result = dialog.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                selecteditem.FileSerDir = dialog.SelectedPath.ToString();
                SaveSKU_S();
                lstSKU.Items.Refresh();
            }
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            try
            {
                var urlPart = ((Hyperlink)sender).NavigateUri;
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(urlPart.AbsoluteUri));
                e.Handled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void btnExplore_Click(object sender, RoutedEventArgs e)
        {
            var selecteditem = lstSKU.SelectedItem as SKUS;
            if (selecteditem != null)
            {
                OpenFtp("ftp://" + selecteditem.Address + ":998");
            }

        }



        private static void Process_OutputDataReceived(object sender, System.Diagnostics.DataReceivedEventArgs e)
        {
            //throw new NotImplementedException();
        }

        private void btnMapfilesys_Click(object sender, RoutedEventArgs e)
        {
            //note this will not work 
            MessageBox.Show("This function will not work in the current state");
            var selecteditem = lstSKU.SelectedItem as SKUS;
            if (selecteditem != null)
            {
                DriveLetter driveselector = new DriveLetter();
                driveselector.ShowDialog();
                if (driveselector.DriveSelected != "")
                {
                    // RunProcess("net", @"use " + driveselector.DriveSelected + " ftp://" + selecteditem.Address + ":998 /persistent:yes", null);
                    string unc = "ftp://" + selecteditem.Address + ":998";
                    string drive = driveselector.DriveSelected;

                    int status =
                      NetworkDrive.MapNetworkDrive(unc, drive, null, null);

                    if (status == 0)
                    {
                        MessageBox.Show($"{unc} mapped to drive {drive}");
                        Console.WriteLine($"{unc} mapped to drive {drive}");
                    }
                    else
                    {
                        //  https://stackoverflow.com/a/1650868/1911064
                        string errorMessage =
                            new System.ComponentModel.Win32Exception(status).Message;
                        Console.WriteLine($"Failed to map {unc} to drive {drive}!");
                        Console.WriteLine(errorMessage);
                        MessageBox.Show($"Failed to map {unc} to drive {drive}!" + errorMessage);
                    }
                }
            }
        }

        private void btnDisconnect_Click(object sender, RoutedEventArgs e)
        {

            if (clients != null || clients.IsConnected || API.isConnected())
            {
                if (lstSKU.SelectedItem == null)
                {

                }
                else
                {
                    var currentsku = lstSKU.SelectedItem as SKUS;
                    if (tc != null)
                    {
                        tc.Disconnect();
                    }
                    disconnect(currentsku);

                    //once connected load some information 
                    currentsku.SDK = "";
                    currentsku.Power = SKU_Power_State.Off;
                    currentsku.Status = ConnectionStatus.Discconecting;
                    lstSKU.Items.Refresh();
                    SetSkuUIItems(currentsku);
                }
            }
            else
            {
                MessageBox.Show("No SKUS connected\n");
            }
        }

        private void lstSKU_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

            PS4Console consoleoutput = new PS4Console();
            consoleoutput.SKU = lstSKU.SelectedItem as SKUS;
            consoleoutput.clients = clients;
            consoleoutput.tc = tc;
            consoleoutput.Show();
        }

        private void RibbonWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //disconnect any remaining connections here
            if (clients.IsConnected || API.isConnected())
            {
                for (int i = 0; i < SKU_List.Count; i++)
                {
                    if (SKU_List[i].Status == ConnectionStatus.Connected)
                    {
                        var currentsku = SKU_List[i];
                        disconnect(currentsku);

                        //once connected load some information 
                        currentsku.SDK = "";
                        currentsku.Power = SKU_Power_State.Off;
                        currentsku.Status = ConnectionStatus.Discconecting;
                        lstSKU.Items.Refresh();
                        SetSkuUIItems(currentsku);
                    }
                }
            }

        }
        #endregion << Methods >>

        #region << Functions >>

        public string GetResourceHash()
        {
            using (MemoryStream ms = new MemoryStream(Properties.Resources.make_fself))
            {
                using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
                {
                    byte[] hash = md5.ComputeHash(ms);
                    string str = Convert.ToBase64String(hash);
                    // result for example: WgWKWcyl2YwlF/C8yLU9XQ==
                    return str;
                }
            }
        }
        public string GetFileHash(string Path)
        {
            using (Stream ms = new FileStream(Path, FileMode.Open, FileAccess.Read))
            {
                using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
                {
                    byte[] hash = md5.ComputeHash(ms);
                    string str = Convert.ToBase64String(hash);
                    // result for example: WgWKWcyl2YwlF/C8yLU9XQ==
                    return str;
                }
            }
        }
        /// <summary>
        /// Resource extractor class and Md5 has matcher
        /// </summary>
        public void ExtractResources()
        {
            string AppPath = System.AppDomain.CurrentDomain.BaseDirectory;

            if (!File.Exists(AppPath + "\\make_fself.exe"))
            {
                //System.IO
                File.WriteAllBytes(AppPath + "\\make_fself.exe", Properties.Resources.make_fself);
            }
            else
            {
                //we do a has validation 
                if (GetResourceHash() != GetFileHash(AppPath + "\\make_fself.exe"))
                {
                    File.WriteAllBytes(AppPath + "\\make_fself.exe", Properties.Resources.make_fself);
                }
            }
        }


        public string Orbis_CMD(string command, string arguments)
        {
            string AppPath = System.AppDomain.CurrentDomain.BaseDirectory;
            ProcessStartInfo start = new ProcessStartInfo();
            start.FileName = AppPath + "make_fself.exe ";
            start.Arguments = arguments;
            start.UseShellExecute = false;
            start.RedirectStandardOutput = true;
            start.CreateNoWindow = true;
            using (System.Diagnostics.Process process = System.Diagnostics.Process.Start(start))
            {
                process.ErrorDataReceived += Process_ErrorDataReceived;
                using (StreamReader reader = process.StandardOutput)
                {
                    string result = reader.ReadToEnd();
                    if (result.Contains("already converted from elf file to self file"))
                    {
                        System.Windows.Forms.DialogResult dlr = System.Windows.Forms.MessageBox.Show("Already Converted From Elf Error Found.... will be using Orbis-pub-gen for this pkg\n\n Simply Click Build and select the save folder", "Error with an alternative", System.Windows.Forms.MessageBoxButtons.OKCancel, System.Windows.Forms.MessageBoxIcon.Question);
                        if (dlr == System.Windows.Forms.DialogResult.OK)
                        {
                            //this will open up the GP4 Project inside the Utility
                            // Orbis_Pub__GenCMD("", AppCommonPath() + @"\PS2Emu\" + "PS2Classics.gp4");

                        }
                    }
                    else if (result.Contains("ERROR"))
                    {
                        System.Windows.Forms.MessageBox.Show(result);
                    }
                    return result;
                }
            }
        }

        private void Process_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            //  throw new NotImplementedException();
        }

        private bool IsMatchAtIndex(String value, String searchArgument, int startIndex)
        {
            for (int j = 0; j < searchArgument.Length; j++)
            {
                if (value[startIndex + j] != searchArgument[j])
                {
                    return false;
                }
            }
            return true;
        }
        public int StrStr(string value, string searchArgument)
        {
            if (value == null) { throw new ArgumentNullException("value"); }
            if (searchArgument == null) { throw new ArgumentNullException("searchArgument"); }
            if (searchArgument.Length == 0) { return 0; }

            int searchLength = searchArgument.Length;
            int length = value.Length;

            if (searchLength > length) { return -1; }

            for (int i = 0; i < length; i++)
            {
                if (length - i < searchLength) { return -1; }

                if (IsMatchAtIndex(value, searchArgument, i)) { return i; }
            }
            return -1;
        }



        /// <summary>
        /// Refresh Data from the syetm
        /// </summary>
        /// <param name="noReset"> Skip File Load</param>
        private void RefreshData(bool noReset = false)
        {
            if (noReset == false)
            {
                LoadSKU_S();

                //just disable all information for now
                for (int i = 0; i < SKU_List.Count; i++)
                {
                    SKU_List[i].Power = SKU_Power_State.Off;
                    SKU_List[i].SDK = "";
                    SKU_List[i].Status = ConnectionStatus.NotConnected;
                }

            }
            lstSKU.ItemsSource = SKU_List;
            AutoSizeColumns();
        }

        /// <summary>
        /// Save SKU Class
        /// </summary>
        public void SaveSKU_S()
        {
            //delete

            //serialize
            using (Stream stream = File.Open(System.AppDomain.CurrentDomain.BaseDirectory + @"\MNHL.bin", FileMode.OpenOrCreate))
            {
                var bformatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();

                bformatter.Serialize(stream, SKU_List);
            }
        }

        /// <summary>
        /// Load SKU Class
        /// </summary>
        public void LoadSKU_S()
        {
            if (File.Exists(System.AppDomain.CurrentDomain.BaseDirectory + @"\MNHL.bin"))
            {
                //deserialize
                using (Stream stream = File.Open(System.AppDomain.CurrentDomain.BaseDirectory + @"\MNHL.bin", FileMode.Open))
                {
                    var bformatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();

                    SKU_List = (List<SKUS>)bformatter.Deserialize(stream);
                }
            }
        }


        private void VersionCheck(SKUS CurrentItem)
        {


            if (clients == null)
            {

            }
            else if (clients.IsConnected && API.isConnected())
            {


                CurrentItem.Status = ConnectionStatus.Connecting;
                btnConnect.IsEnabled = false;





                string version = API.version();

                Console.WriteLine(version);
                for (int i = 0; i < 2000; i++)
                {

                    if (version != client_comp_ver)
                    {
                        if (i >= 2000)
                        {




                            tc.Telnetdis();
                            MessageBox.Show("This tool is not compatible with the PS4 Fakekit Version\n\nYour Version is: " + client_comp_ver + "\n" + "PS4 Version is: " + version + "\n\n" + "Press OK to Exit");
                            return;
                        }


                    }
                    else
                    {
                        Console.WriteLine("Verions match\n");


                        CurrentItem.Status = ConnectionStatus.Connected;
                        btnConnect.IsEnabled = false;

                        break;
                    }



                }
            }


            /*if (tc == null)
            {

            }
            else if (tc.IsConnected)
            {

                Invoke(new Action(() =>
                {
                    label1.Text = "Connecting";
                    Connect.Enabled = false;
                }));




                tc.WriteLine("fakekit_version");

                for (int i = 0; i < 2000; i++)
                {

                    string  ver = tc.Read();

                    if (StrStr(ver, client_comp_ver) != -1)
                    {
                        if (i >= 2000)
                        {
                            

                            Invoke(new Action(() =>
                            {
                                label1.Text = "Disconnected";
                                Connect.Enabled = false;
                            }));

                            tc.Telnetdis();
                            MessageBox.Show("This tool is not compatible with the PS4 Fakekit Version\n\nYour Version is: " + client_comp_ver + "\n" + "PS4 Version is: " + ver + "\n\n" + "Press OK to Exit");
                            Application.Exit();
                        }


                    }
                    else
                    {
                        Console.WriteLine("Verions match\n");

                        Invoke(new Action(() =>
                        {
                            label1.Text = "Connected";
                            Connect.Enabled = false;
                        }));

                        break;
                    }



                }
            }*/
        }


        private async void connect(SKUS CurrentSKU)
        {

            thread = System.Threading.Thread.CurrentThread;
            //   tc = new TelnetConnection(textBox1.Text, 998);

            try
            {

                clients = new FtpClient(CurrentSKU.Address);
                clients.Port = 998;


                clients.Connect();



                API.IP = CurrentSKU.Address;
                API.Port = 999;
                API.connect();
                API.notify("PS4 Connected to Neighborhood on " + Environment.UserName.ToString());


                //Properties.Settings.Default.DefaultName = CurrentSKU.Default;

                //Properties.Settings.Default.Save();

                VersionCheck(CurrentSKU);
            }
            catch
            {
                // MessageBox.Show(eee.ToString());
                MessageBox.Show("Cannot Connect!");
                throw;

            }





        }

        private async void disconnect(SKUS CurrentSKU)
        {

            thread = System.Threading.Thread.CurrentThread;
            //   tc = new TelnetConnection(textBox1.Text, 998);

            try
            {
                API.notify("PS4 Disconnected From Neighborhood on " + Environment.UserName.ToString());
                if (clients.IsConnected == true)
                {
                    clients.Disconnect();
                }

                if (API.isConnected())
                {
                    API.disconnect();
                }
            }
            catch
            {
                // MessageBox.Show(eee.ToString());
                MessageBox.Show("Cannot disconnect!");
                throw;

            }





        }

        public void LoadExecutables(SKUS CurrentSKU)
        {
            btnLoadExe.Items.Clear();
            btnKillProcess.Items.Clear();
            RibbonMenuItem killallitem = new RibbonMenuItem();
            killallitem.Header = "Kill all processes";
            btnKillProcess.Items.Add(killallitem);
            RibbonSeparator ribbonseparatoritem = new RibbonSeparator();
            btnKillProcess.Items.Add(ribbonseparatoritem);
            API.ErrorCode errorcheck = new API.ErrorCode();
            List<Fakekit_API.Process> acctiveprocesses = API.getProcesses(out errorcheck);
            var temp = API.getModules(out errorcheck);
            //var check2 = API.getRegions(out errorcheck);
            //  var check3 = API.getapps(out errorcheck);
            foreach (var item in acctiveprocesses)
            {
                RibbonMenuItem menuitem = new RibbonMenuItem();
                menuitem.Click += LoadProcess_Click;
                menuitem.Header = item.Name + "(" + item.Id + ")";
                menuitem.Tag = item.Id;
                btnLoadExe.Items.Add(menuitem);

            }
            foreach (var item in acctiveprocesses)
            {
                RibbonMenuItem menuitem = new RibbonMenuItem();
                menuitem.Click += KillProcess_Click;
                menuitem.Header = item.Name + "(" + item.Id + ")";
                menuitem.Tag = item.Id;
                btnKillProcess.Items.Add(menuitem);

            }



        }

        public void SetSkuUIItems(SKUS currentsku)
        {
            if (currentsku.Status == ConnectionStatus.Connected)
            {

                #region << Status >>
                //enable all items that are disabled
                btnDisconnect.IsEnabled = true;
                btnConnect.IsEnabled = true;
                btnReboot.IsEnabled = true;
                btnPowerOff.IsEnabled = true;
                btnPowerOn.IsEnabled = true;
                btnRestMode.IsEnabled = true;
                btnPowerOn.IsEnabled = true;
                #endregion << Status >>

                #region << Run >>

                btnLoadExe.IsEnabled = true;
                LoadExecutables(currentsku);
                btnLoadApp.IsEnabled = true;
                btnKillProcess.IsEnabled = true;
                btnCodeDump.IsEnabled = true;
                btnPkgsEnt.IsEnabled = true;

                #endregion << Run >>

                #region << Data >>

                btnExplore.IsEnabled = true;
                btnCopyfiles.IsEnabled = true;

                #endregion << Data >>

                #region << More >>

                btnPlayGo.IsEnabled = true;
                btnTagetSettings.IsEnabled = true;
                btnMore.IsEnabled = true;

                #endregion << More >>
            }
            else
            {

                #region << Status >>
                //enable all items that are disabled
                btnDisconnect.IsEnabled = false;
                btnConnect.IsEnabled = true;
                btnReboot.IsEnabled = false;
                btnPowerOff.IsEnabled = false;
                btnPowerOn.IsEnabled = false;
                btnRestMode.IsEnabled = false;
                btnPowerOn.IsEnabled = false;
                #endregion << Status >>

                #region << Run >>

                btnLoadExe.IsEnabled = false;
                //LoadExecutables(currentsku);
                btnLoadApp.IsEnabled = false;
                btnKillProcess.IsEnabled = false;
                btnCodeDump.IsEnabled = false;
                btnPkgsEnt.IsEnabled = false;

                #endregion << Run >>

                #region << Data >>

                btnExplore.IsEnabled = false;
                btnCopyfiles.IsEnabled = false;

                #endregion << Data >>

                #region << More >>

                btnPlayGo.IsEnabled = false;
                btnTagetSettings.IsEnabled = false;
                btnMore.IsEnabled = false;

                #endregion << More >>
            }
            AutoSizeColumns();
        }


        private void OpenFtp(string folderPath)
        {
            try
            {
                System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo
                {
                    Arguments = folderPath,
                    FileName = "explorer.exe"
                };
                System.Diagnostics.Process.Start(startInfo);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        internal static int RunProcess(string fileName, string args, string workingDir)
        {
            var startInfo = new System.Diagnostics.ProcessStartInfo
            {
                FileName = fileName,
                Arguments = args,
                UseShellExecute = false,
                RedirectStandardError = true,
                RedirectStandardOutput = true,
                CreateNoWindow = true,
                WorkingDirectory = workingDir
            };

            using (var process = System.Diagnostics.Process.Start(startInfo))
            {
                if (process == null)
                {
                    throw new Exception($"Failed to start {startInfo.FileName}");
                }

                process.OutputDataReceived += Process_OutputDataReceived;
                process.ErrorDataReceived += (s, e) =>
                {
                    if (!string.IsNullOrWhiteSpace(e.Data)) { new Exception(e.Data); }
                };

                process.BeginOutputReadLine();
                process.BeginErrorReadLine();

                process.WaitForExit();

                return process.ExitCode;
            }
        }


        #endregion << Functions >>

    }
}

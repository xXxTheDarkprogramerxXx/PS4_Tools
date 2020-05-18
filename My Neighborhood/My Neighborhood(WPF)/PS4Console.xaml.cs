using Fakekit_API;
using FluentFTP;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
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
using System.Windows.Shapes;
using PrimS.Telnet;
using My_Neighborhood_WPF_.Properties;
using My_Neighborhood_WPF_.SerialApi;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Crashes;
using MahApps.Metro.Controls;
using MahApps.Metro;
using System.Diagnostics;

namespace My_Neighborhood_WPF_
{
    /// <summary>
    /// Interaction logic for PS4Console.xaml
    /// </summary>
    public partial class PS4Console : MetroWindow
    {
        public MainWindow.SKUS SKU = new MainWindow.SKUS();
        public MainWindow.TelnetConnection tc = null;
        public FtpClient clients = new FtpClient();
        public bool isOpen = false;
        bool AutoScroll = true;
        string LogDirectory = "";

        public PS4Console()
        {
            InitializeComponent();
        }
        #region << Serial Integration >>

        private SerialMonitor Monitor = new SerialMonitor();
        private System.Threading.Thread threadcom;
        // Token: 0x0200000C RID: 12
        // (Invoke) Token: 0x06000053 RID: 83
        private delegate void AppendTextCallback(string text);

        private void OpenMonitor()
        {
            this.Monitor.Settings.PortName = Settings.Default.ConsolePrefPort;
            if (this.Monitor.StartListening())
            {
                this.rtbAll.AppendText(string.Format("Connected To Console On Port: {0}\n", this.Monitor.Settings.PortName));
                return;
            }
            this.rtbAll.AppendText(string.Format("Failed To Connect To Console On Port {0}\n", this.Monitor.Settings.PortName));
        }

        private void OnNewSerialDataReceived(object sender, SerialDataEventArgs e)
        {
            string @string = Encoding.UTF8.GetString(e.Data);
            if (!string.IsNullOrWhiteSpace(@string))
            {
                rtbAll.Dispatcher.Invoke(new Action(() => rtbAll.AppendText(@string)));
                try
                {
                    using (StreamWriter sw = File.AppendText(LogDirectory + "/PS4-KLOG-RPC-log.txt"))
                    {
                        sw.Write(@string);
                    }
                }
                catch(Exception ex)
                {

                }
                if (AutoScroll == true)
                    this.Dispatcher.Invoke(() => rtbAll.ScrollToEnd());
            }
        }

        #endregion << Serial Integration >>

        private void RibbonWindow_Loaded(object sender, RoutedEventArgs e)
        {
            isOpen = true;
            rtbAll.Selection.Text = "";
            this.Title = SKU.Target + " [" + SKU.Power + "] - Console Output for PlayStation®4";
            Style noSpaceStyle = new Style(typeof(Paragraph));
            noSpaceStyle.Setters.Add(new Setter(Paragraph.MarginProperty, new Thickness(0)));
            rtbAll.Resources.Add(typeof(Paragraph), noSpaceStyle);


            string AppPath = System.AppDomain.CurrentDomain.BaseDirectory;
            //Log Directory 
            LogDirectory = AppPath + "/Logs/" + SKU.Target;
            try
            {
                if (!Directory.Exists(LogDirectory))
                {
                    Directory.CreateDirectory(LogDirectory);
                }
                if (Properties.Settings.Default.DisableSingleFileLogging == true)
                {
                    if (File.Exists(LogDirectory + "/PS4-KLOG-RPC-log.txt"))
                    {
                        File.Delete(LogDirectory + "/PS4-KLOG-RPC-log.txt");
                    }
                }
                else
                {
                    if (File.Exists(LogDirectory + "/PS4-KLOG-RPC-log.txt"))
                    {
                        File.Move(LogDirectory + "/PS4-KLOG-RPC-log.txt", LogDirectory + "/PS4-KLOG-RPC-log-backup-" + DateTime.Now.ToString("hmmsstt") + ".txt");
                    }
                }

                File.Create(LogDirectory + "/PS4-KLOG-RPC-log.txt");
            }
            catch(Exception ex)
            {

            }
            if (Properties.Settings.Default.ConsolePrefCom == true && Properties.Settings.Default.ConsolePrefPort != "")
            {
                this.Monitor.NewSerialDataRecieved += this.OnNewSerialDataReceived;
                this.Monitor.SetSpeed(false);
                this.OpenMonitor();
            }
            else
            {
                threadcom = new System.Threading.Thread(() => RefreshProcesses());
                threadcom.Start();
            }

            //var temp = clients.GetListing();

            //using (Stream stream = clients.OpenRead(@"/dev/klog",FtpDataType.ASCII,true))
            //{
            //    rtbAll.Selection.Load(stream, DataFormats.Text);
            //    rtbAll.ScrollToEnd();
            //}

        }
        bool openconnection = false;
        public async void RefreshProcesses()
        {
            try
            {
                if (tc == null)
                {
                    tc = new MainWindow.TelnetConnection(SKU.Address, 777);//new connection
                }
                // while connected
                while (tc != null && tc.IsConnected)
                {

                    string hi = tc.Read();

                    // send client input to server
                    tc.WriteNothing();

                    // display server output

                    if (!String.IsNullOrEmpty(hi) && !String.IsNullOrWhiteSpace(hi) && hi.Length > 3)
                    {
                        //Console.Write(hi);



                        this.Dispatcher.Invoke(() => rtbAll.AppendText(hi));

                        using (StreamWriter sw = File.AppendText(LogDirectory + "/PS4-KLOG-RPC-log.txt"))
                        {
                            sw.Write(hi);
                        }

                        if (AutoScroll == true)
                            this.Dispatcher.Invoke(() => rtbAll.ScrollToEnd());

                    }


                    // send client input to server
                    // Console.WriteLine(tc.Read());
                    //  rtbAll.Selection.Load(rtbAll.Selection.Text, DataFormats.Text);
                    // tc.WriteLine(prompt);

                }

            }
            catch(Exception ex)
            {

            }
        }
        
        private void btnClearAll_Click(object sender, RoutedEventArgs e)
        {
            //clean the all screen
            rtbAll.SelectAll();
            rtbAll.Selection.Text = "";

            //clients.DeleteFile(@"/data/Infamous_LOG.txt");/*we no longer need this file*/
            //RefreshProcesses();
        }

        private void btnRefreshProcesses_Click(object sender, RoutedEventArgs e)
        {
            RefreshProcesses();
        }

        private void btnPreferences_Click(object sender, RoutedEventArgs e)
        {
            My_Neighborhood_WPF_.PS4ConsolePages.PrefSettings prefsettings = new My_Neighborhood_WPF_.PS4ConsolePages.PrefSettings();
            prefsettings.ShowDialog();

        }

        private void RibbonWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if(threadcom != null)
            {
                if(threadcom.IsAlive == true)
                {
                    threadcom.Abort();
                }
            }
            isOpen = false;
        }

        private void btnSelectAll_Click(object sender, RoutedEventArgs e)
        {
            rtbAll.SelectAll();
        }

        private void btnCopy_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Clipboard.SetText(rtbAll.Selection.Text);
        }

        private void cbxScrollimput_Checked(object sender, RoutedEventArgs e)
        {
            if (cbxScrollimput.IsChecked == false)
                AutoScroll = false;
            else
                AutoScroll = true;
        }

        private void btnOpenLogDirectory_Click(object sender, RoutedEventArgs e)
        {

            if (Directory.Exists(LogDirectory))
            {
                var urlPart = LogDirectory;
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(urlPart));
                e.Handled = true;

            }
        }
    }
}

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

namespace My_Neighborhood_WPF_
{
    /// <summary>
    /// Interaction logic for PS4Console.xaml
    /// </summary>
    public partial class PS4Console : RibbonWindow
    {
        public MainWindow.SKUS SKU = new MainWindow.SKUS();
        public MainWindow.TelnetConnection tc = null;
        public FtpClient clients = new FtpClient();
        public PS4Console()
        {
            InitializeComponent();
        }
        #region << Serial Integration >>

        private SerialMonitor Monitor = new SerialMonitor();

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
                this.Dispatcher.Invoke(() => rtbAll.ScrollToEnd());
            }
        }

        #endregion << Serial Integration >>

        private void RibbonWindow_Loaded(object sender, RoutedEventArgs e)
        {
            rtbAll.Selection.Text = "";
            this.Title = SKU.Target + " [" + SKU.Power + "] - Console Output for PlayStation®4";
            Style noSpaceStyle = new Style(typeof(Paragraph));
            noSpaceStyle.Setters.Add(new Setter(Paragraph.MarginProperty, new Thickness(0)));
            rtbAll.Resources.Add(typeof(Paragraph), noSpaceStyle);
            if (Properties.Settings.Default.ConsolePrefCom == true && Properties.Settings.Default.ConsolePrefPort != "")
            {
                this.Monitor.NewSerialDataRecieved += this.OnNewSerialDataReceived;
                this.Monitor.SetSpeed(false);
                this.OpenMonitor();
            }
            else
            {
                System.Threading.Thread threadcom = new System.Threading.Thread(() => RefreshProcesses());
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
            clients.DeleteFile(@"/data/Infamous_LOG.txt");
            RefreshProcesses();
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
    }
}

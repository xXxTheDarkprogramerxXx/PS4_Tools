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


        private void RibbonWindow_Loaded(object sender, RoutedEventArgs e)
        {
            rtbAll.Selection.Text = "";
            this.Title = SKU.Target + " [" + SKU.Power + "] - Console Output for PlayStation®4";
            RefreshProcesses();


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
                rtbAll.SelectAll();

                rtbAll.Selection.Text = "";
               
                    using (Client client = new Client(SKU.Address.ToString(), 777, new System.Threading.CancellationToken()))
                    {
                        //client.IsConnected;
                        //(await client.TryLoginAsync("username", "password", 12)).Should().Be(true);
                        client.TryLoginAsync()
                        //client.WriteLine("show statistic wan2");
                        string s = await client.TerminatedReadAsync("END:");
                        //s.Should().Contain(">");
                        //s.Should().Contain("WAN2");
                        //Regex regEx = new Regex("(?!WAN2 total TX: )([0-9.]*)(?! GB ,RX: )([0-9.]*)(?= GB)");
                        //regEx.IsMatch(s).Should().Be(true);
                        //MatchCollection matches = regEx.Matches(s);
                        //decimal tx = decimal.Parse(matches[0].Value);
                        //decimal rx = decimal.Parse(matches[1].Value);
                        //(tx + rx).Should().BeLessThan(50);
                    }
                

                //telnetClient.Close();
                //using (Stream stream = await clients.OpenReadAsync(@"/data/Infamous_LOG.txt", FtpDataType.ASCII))
                //{

                //    rtbAll.Selection.Load(stream, DataFormats.Text);
                //    rtbAll.ScrollToEnd();
                //}
            }
            catch (Exception ex)
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
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace My_Neighborhood_WPF_
{
    /// <summary>
    /// Interaction logic for frmLoadIPS.xaml
    /// </summary>
    public partial class frmLoadIPS : Window
    {

        public string IPSelected { get; set; }

        public frmLoadIPS()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Get host name
            String strHostName = Dns.GetHostName();

            // Find host by name
            IPHostEntry iphostentry = Dns.GetHostByName(strHostName);

            // Enumerate IP addresses
            int nIP = 0;
            foreach (IPAddress ipaddress in iphostentry.AddressList)
            {
                listBox.Items.Add(ipaddress.ToString());
            }
        }
        

        private void listBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            // on double click return item
            IPSelected = listBox.SelectedItem.ToString();
            this.Close();
        }
    }
}

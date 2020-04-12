using My_Neighborhood_WPF_.Properties;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
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

namespace My_Neighborhood_WPF_.PS4ConsolePages
{
    /// <summary>
    /// Interaction logic for SettingsCom.xaml
    /// </summary>
    public partial class SettingsCom : Window
    {
        public SettingsCom()
        {
            InitializeComponent();
        }
        private string[] COMDevices;

        private int DeviceCount;

        private void RefreshCOMDevices()
        {
            this.lstvcomdevices.Items.Clear();
            this.COMDevices = SerialPort.GetPortNames();
            this.DeviceCount = 0;
            foreach (string text in this.COMDevices)
            {
                this.lstvcomdevices.Items.Add(text);
                this.DeviceCount++;
            }
            //this.lblNumberDevices.Text = "Connected COM Devices: " + this.DeviceCount.ToString();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            RefreshCOMDevices();
        }

        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            RefreshCOMDevices();
        }

        private void lstvcomdevices_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            //get selected device
            try
            {
                Settings.Default.ConsolePrefPort = this.lstvcomdevices.SelectedItems[0].ToString();
                if (!Settings.Default.ConsolePrefCom)
                {
                    Settings.Default.ConsolePrefCom = true;
                }
                Settings.Default.Save();
                this.Close();      
            }
            catch(Exception ex)
            {
                if (!Settings.Default.ConsolePrefCom)
                {
                    MessageBox.Show(ex.Message, "Error on com", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    //base.DialogResult = DialogResult;
                }
            }
        }
    }
}

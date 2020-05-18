using System;
using System.Collections.Generic;
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
using Microsoft.AppCenter.Crashes;
using MahApps.Metro.Controls;
using MahApps.Metro;

namespace My_Neighborhood_WPF_.PS4ConsolePages
{
    /// <summary>
    /// Interaction logic for PrefSettings.xaml
    /// </summary>
    public partial class PrefSettings : MetroWindow
    {
        bool Loading = true;
        public PrefSettings()
        {
            InitializeComponent();
            if (Properties.Settings.Default.ConsolePrefCom == true && Properties.Settings.Default.ConsolePrefPort != "")
            {
                chb.IsChecked = true;
            }
        }

        private void chb_Checked(object sender, RoutedEventArgs e)
        {
            if (chb.IsChecked == true && Loading == false)
            {
                var result = MessageBox.Show("This will enable and prefer UART debugging for the PS4 are you sure you want to continue?", "Enable UART", MessageBoxButton.YesNo, MessageBoxImage.Exclamation);
                if (result == MessageBoxResult.Yes)
                {
                    //open ps4 pref settings 
                    SettingsCom comsettings = new SettingsCom();
                    comsettings.ShowDialog();
                    if(Properties.Settings.Default.ConsolePrefCom != true && Properties.Settings.Default.ConsolePrefPort == "")
                    {
                        chb.IsChecked = false;
                    }
                }
            }
           
        }

        private void chbLogFile_Checked(object sender, RoutedEventArgs e)
        {
            if (chbCopy.IsChecked == true && Loading == false)
            {
                var result = MessageBox.Show("This will make a bunch of log files you will have to clean it yourself?", "Disable single file logging", MessageBoxButton.YesNo, MessageBoxImage.Exclamation);
                if (result == MessageBoxResult.Yes)
                {

                    Properties.Settings.Default.DisableSingleFileLogging = true;
                    Properties.Settings.Default.Save();

                    chbCopy.IsChecked = false;
                }
            }

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Loading = false;
        }

        private void chb_Unchecked(object sender, RoutedEventArgs e)
        {
            if (chb.IsChecked == false && Loading == false)
            {
                var result = MessageBox.Show("This will disable UART debugging for the PS4 are you sure you want to continue?", "Disable UART", MessageBoxButton.YesNo, MessageBoxImage.Exclamation);
                if (result == MessageBoxResult.Yes)
                {
                    Properties.Settings.Default.ConsolePrefCom = false;
                    Properties.Settings.Default.Save();
                }
            }
        }

        private void chbCopy_Unchecked(object sender, RoutedEventArgs e)
        {
            if (chbCopy.IsChecked == true && Loading == false)
            {
                var result = MessageBox.Show("This will replace the log file each time ?", "Enable single file logging", MessageBoxButton.YesNo, MessageBoxImage.Exclamation);
                if (result == MessageBoxResult.Yes)
                {

                    Properties.Settings.Default.DisableSingleFileLogging = false;
                    Properties.Settings.Default.Save();

                    chbCopy.IsChecked = false;
                }
            }
        }
    }
}

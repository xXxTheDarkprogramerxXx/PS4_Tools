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

namespace My_Neighborhood_WPF_.PS4ConsolePages
{
    /// <summary>
    /// Interaction logic for PrefSettings.xaml
    /// </summary>
    public partial class PrefSettings : Window
    {
        public PrefSettings()
        {
            InitializeComponent();
        }

        private void chb_Checked(object sender, RoutedEventArgs e)
        {
            if (chb.IsChecked == true)
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

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if(Properties.Settings.Default.ConsolePrefCom == true && Properties.Settings.Default.ConsolePrefPort != "")
            {
                chb.IsChecked = true;
            }
        }
    }
}

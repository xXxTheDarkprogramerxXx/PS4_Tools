using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;

namespace My_Neighborhood_WPF_
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            try
            {
                AppCenter.Start("62dcc3d0-bd2f-48a7-89ef-4a17600432e1",
                    typeof(Analytics), typeof(Crashes));

                //MessageBox mess = new MessageBox("S")
                string versionnum = e.Args[0].ToString();
                //MainWindow.VersionNum = versionnum;
            }
            catch (Exception ex)
            {

            }

        }
      
    }
}

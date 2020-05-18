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
using MahApps.Metro;

namespace My_Neighborhood_WPF_
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public ResourceDictionary ThemeDictionary
        {
            // You could probably get it via its name with some query logic as well.
            get { return Resources.MergedDictionaries[0]; }
        }

        public void ChangeTheme(Uri uri)
        {
            ResourceDictionary resourceDict = Application.LoadComponent(uri) as ResourceDictionary;
            Application.Current.Resources.MergedDictionaries.Clear();
            Application.Current.Resources.MergedDictionaries.Add(resourceDict);
        }

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

                Tuple<AppTheme, Accent> appStyle = ThemeManager.DetectAppStyle(Application.Current);

                // now set the Green accent and dark theme
                ThemeManager.ChangeAppStyle(Application.Current,
                                            ThemeManager.GetAccent("White"),
                                            ThemeManager.GetAppTheme("BaseLight")); // or appStyle.Item1
                ThemeManager.ChangeAppStyle(Application.Current,
                                               ThemeManager.GetAccent("Cobalt"),
                                               ThemeManager.GetAppTheme("BaseDark")); // or appStyle.Item1

            }
            catch (Exception ex)
            {

            }

        }
      
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Preferences;

namespace DesignLibrary_Tutorial.Fragments
{
    class SettingsFragment : PreferenceFragment, ISharedPreferencesOnSharedPreferenceChangeListener
    {

        public void OnSharedPreferenceChanged(ISharedPreferences sharedPreferences, string key)
        {
            ISharedPreferences d = PreferenceManager.GetDefaultSharedPreferences(MainActivity.context);


            new System.Threading.Thread(new System.Threading.ThreadStart(delegate
            {             
                if (key == "WMSUrl")
                {
                    string val = d.GetString(key, "");
                    if (val != "")
                    {
                        if(val=="WMS")
                        {
                            val = "http://10.0.2.2/WMS/WMSService.svc";
                        }
                        Constants._service.Url = val;
                    }
                }
            })).Start();

        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
            AddPreferencesFromResource(Resource.Xml.system_settings);
            try
            {
                ISharedPreferences d = PreferenceManager.GetDefaultSharedPreferences(MainActivity.context); // getting access from Preference manager
                PreferenceManager.GetDefaultSharedPreferences(MainActivity.context).RegisterOnSharedPreferenceChangeListener(this); // registerer the preference listner

                EditTextPreference checkbox = (EditTextPreference)FindPreference("WMSUrl");

                checkbox.Text = d.GetString("WMSUrl", "");
            }
            catch (Exception ex)
            {
                //ErrorLoging.WriteErrorLog(Constants.StudentObject.Idx, ex.Message, "Student_Settings_Fragment");
            }

        }


    }
}
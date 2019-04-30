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
using Android.Media;
using Android.Content.Res;
using Android.Util;

using Android.Support.V4.Widget;
using SupportFragment = Android.Support.V4.App.Fragment;
using SupportFragmentManager = Android.Support.V4.App.FragmentManager;
using SupportToolbar = Android.Support.V7.Widget.Toolbar;
using SupportActionBar = Android.Support.V7.App.ActionBar;
using Android.Support.V7.App;
using Android.Support.Design.Widget;
using Android.Support.V4.View;
using Android.Support.V4.App;

namespace DesignLibrary_Tutorial
{
    public class Settings : PreferenceFragment, ISharedPreferencesOnSharedPreferenceChangeListener
    {

        public void OnSharedPreferenceChanged(ISharedPreferences sharedPreferences, string key)
        {
            ISharedPreferences d = PreferenceManager.GetDefaultSharedPreferences(MainActivity.context);


            new System.Threading.Thread(new System.Threading.ThreadStart(delegate
            {
                


                if (key == "Default Range")
                {
                    ApplicationSettings.Defualt_Distance_Range = d.GetInt(key, ApplicationSettings.Defualt_Distance_Range);
                }
                //if (key == "Vibrate")
                //{
                //    NotificationSettings.Vibrate = d.GetBoolean("Vibrate", NotificationSettings.Vibrate);
                //}
                //if (key == "Sound")
                //{
                //    NotificationSettings.Sound = d.GetBoolean("Sound", NotificationSettings.Sound);
                //}
                //if (key == "NotificationRigntone")
                //{
                //    Android.Net.Uri uri = Android.Net.Uri.Parse(d.GetString("NotificationRigntone", RingtoneManager.GetDefaultUri(RingtoneType.Alarm).ToString()));

                //    NotificationSettings.NotificationTone = uri;

                //    Ringtone ringtone = RingtoneManager.GetRingtone(MainActivity.context, uri);
                //    String name = ringtone.GetTitle(MainActivity.context);
                //    RingtonePreference rigntone = (RingtonePreference)FindPreference("NotificationRigntone");
                //    this.Activity.RunOnUiThread(() =>
                //    {
                //        rigntone.Summary = name.ToString();
                //    });
                //}
            })).Start();

        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
            AddPreferencesFromResource(Resource.Xml.System_Settings);
            try
            {
                ISharedPreferences d = PreferenceManager.GetDefaultSharedPreferences(MainActivity.context); // getting access from Preference manager
                PreferenceManager.GetDefaultSharedPreferences(MainActivity.context).RegisterOnSharedPreferenceChangeListener(this); // registerer the preference listner

                EditTextPreference edittextrange = (EditTextPreference)FindPreference("Default Range");

                edittextrange.Text = d.GetString("Default Range", ApplicationSettings.Defualt_Distance_Range.ToString());
                edittextrange.Summary = "The Defualt Range for all product searches";

                //CheckBoxPreference checkboxvibrate = (CheckBoxPreference)FindPreference("Vibrate");

                //checkboxvibrate.Checked = d.GetBoolean("Vibrate", NotificationSettings.Vibrate);


                //CheckBoxPreference checkboxSound = (CheckBoxPreference)FindPreference("Sound");

                //checkboxSound.Checked = d.GetBoolean("Sound", NotificationSettings.Sound);



                //RingtonePreference rigntone = (RingtonePreference)FindPreference("NotificationRigntone");

                //Android.Net.Uri uri = Android.Net.Uri.Parse(d.GetString("NotificationRigntone", RingtoneManager.GetDefaultUri(RingtoneType.Alarm).ToString()));

                //Ringtone ringtone = RingtoneManager.GetRingtone(MainActivity.context, uri);
                //String name = ringtone.GetTitle(MainActivity.context);
                //rigntone.Summary = name;
            }
            catch (Exception ex)
            {
                ErrorLoging.WriteErrorLog(Constants.StudentObject.Idx, ex.Message, "Student_Settings_Fragment");
            }

        }


    }
}
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
using Android.Content.PM;
using System.IO;
using Android;
using Android.Support.V4.App;
using System.Threading;
using Android.Util;
using Android.Preferences;
using System.Threading.Tasks;

namespace DesignLibrary_Tutorial.Startup
{
    [Activity(Label = "PS4 Tools", Icon = "@drawable/logo", Theme = "@style/SplashTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class SplashScreen : Activity, ISharedPreferencesOnSharedPreferenceChangeListener
    {
        #region << Permisions>>
        /*We need the following permissions camera and read write*/
        static readonly int REQUEST_CAMERA = 2;

        static readonly int REQUEST_ReadExternalStorage = 0;

        static readonly int REQUEST_WriteExternalStorage = 1;

        static string[] PERMISSIONS = {
            Manifest.Permission.ReadExternalStorage,
            Manifest.Permission.WriteExternalStorage,
            Manifest.Permission.Camera
        };


        #endregion << Permisions>>

        #region << Methods >>

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            MainActivity.context = this;

            #region << Check For Permisions >>
            if (ActivityCompat.CheckSelfPermission(this, Manifest.Permission.ReadExternalStorage) != (int)Permission.Granted)
            {

                // Camera permission has not been granted
                RequestReadWirtePermission();
                while (ActivityCompat.CheckSelfPermission(this, Manifest.Permission.ReadExternalStorage) != (int)Permission.Granted)
                {
                    Thread.Sleep(100);
                }
            }
            #endregion << Check For Permisions  >>

            #region << Check Saved Files >>


            //check for URL
            retrieveset();


            #endregion << Check Saved Files >>

            StartActivity(typeof(MainActivity));

            Finish();

            // Disable activity slide-in animation
            OverridePendingTransition(0, Resource.Animator.fadeout);
        }

        /// <summary>
        /// We retrieve all the settings out of the Local System Here
        /// </summary>
        protected void retrieveset()
        {
            //retreive 
            ISharedPreferences d = PreferenceManager.GetDefaultSharedPreferences(this); // getting access from Preference manager
            PreferenceManager.GetDefaultSharedPreferences(this).RegisterOnSharedPreferenceChangeListener(this); // registerer the preference listner

            //string uri = d.GetString("SettingStr", null);
            //if (uri != null && uri != string.Empty)
            //{
            //    Constants._service.Url = uri;
            //}
            //else
            //{
            //    //Constants._service.Url = "";
            //}
        }

        private void RequestReadWirtePermission()
        {
            Log.Info(this.Title.ToString(), "READWRITE permission has NOT been granted. Requesting permission.");

            if (ActivityCompat.ShouldShowRequestPermissionRationale(this, Manifest.Permission.ReadExternalStorage))
            {
                // Provide an additional rationale to the user if the permission was not granted
                // and the user would benefit from additional context for the use of the permission.
                // For example if the user has previously denied the permission.
                Log.Info(this.Title.ToString(), "Displaying READWRITE permission rationale to provide additional context.");

                // Snackbar.Make(layout, "Contacts permissions are needed to demonstrate access",
                //  Snackbar.LengthIndefinite).SetAction("OK", new Action<View>(delegate (View obj) {
                //ActivityCompat.RequestPermissions(this, new String[] { Manifest.Permission.ReadExternalStorage }, REQUEST_ReadExternalStorage);
                ActivityCompat.RequestPermissions(this, new String[] { Manifest.Permission.WriteExternalStorage }, REQUEST_WriteExternalStorage);
                //  })).Show();
            }
            else
            {
                // Camera permission has not been granted yet. Request it directly.
                //ActivityCompat.RequestPermissions(this, new String[] { Manifest.Permission.ReadExternalStorage }, REQUEST_ReadExternalStorage);
                ActivityCompat.RequestPermissions(this, new String[] { Manifest.Permission.WriteExternalStorage }, REQUEST_WriteExternalStorage);
            }

            if (ActivityCompat.ShouldShowRequestPermissionRationale(this, Manifest.Permission.Camera))
            {
                // Provide an additional rationale to the user if the permission was not granted
                // and the user would benefit from additional context for the use of the permission.
                // For example if the user has previously denied the permission.
                Log.Info(this.Title.ToString(), "Displaying Camera permission rationale to provide additional context.");

                // Snackbar.Make(layout, "Contacts permissions are needed to demonstrate access",
                //  Snackbar.LengthIndefinite).SetAction("OK", new Action<View>(delegate (View obj) {
                //ActivityCompat.RequestPermissions(this, new String[] { Manifest.Permission.ReadExternalStorage }, REQUEST_ReadExternalStorage);
                ActivityCompat.RequestPermissions(this, new String[] { Manifest.Permission.Camera }, REQUEST_CAMERA);
                //  })).Show();
            }
            else
            {
                // Camera permission has not been granted yet. Request it directly.
                //ActivityCompat.RequestPermissions(this, new String[] { Manifest.Permission.ReadExternalStorage }, REQUEST_ReadExternalStorage);
                ActivityCompat.RequestPermissions(this, new String[] { Manifest.Permission.Camera }, REQUEST_CAMERA);
            }

        }

        private string AppCommonPath(string ApplicationName)
        {

            string returnstring = "";

            if (Android.OS.Environment.IsExternalStorageEmulated == true)
            {

                returnstring = System.IO.Path.Combine(Android.OS.Environment.ExternalStorageDirectory.AbsolutePath, "PS4 Tools", ApplicationName);

                if (!System.IO.Directory.Exists(returnstring))
                {
                    System.IO.Directory.CreateDirectory(returnstring);
                }
            }
            else
            {
                returnstring = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments), "PS4 Tools", ApplicationName);
                if (!System.IO.Directory.Exists(returnstring))
                {
                    System.IO.Directory.CreateDirectory(returnstring);
                }
            }
            return returnstring;
        }


        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
        {
            if (requestCode == REQUEST_CAMERA)
            {
                // Received permission result for camera permission.
                Log.Info(this.Title.ToString(), "Received response for Camera permission request.");

                // Check if the only required permission has been granted
                if (grantResults.Length == 1 && grantResults[0] == Permission.Granted)
                {
                    // Camera permission has been granted, preview can be displayed
                    Log.Info(this.Title.ToString(), "CAMERA permission has now been granted. Showing preview.");
                    // Snackbar.Make(layout, Resource.String.permision_available_camera, Snackbar.LengthShort).Show();
                }
                else
                {
                    Log.Info(this.Title.ToString(), "CAMERA permission was NOT granted.");
                    //Snackbar.Make(layout, Resource.String.permissions_not_granted, Snackbar.LengthShort).Show();
                }
            }
            else if (requestCode == REQUEST_WriteExternalStorage)
            {
                Log.Info(this.Title.ToString(), "Received response for contact permissions request.");

                // We have requested multiple permissions for contacts, so all of them need to be
                // checked.
                if (PermissionUtil.VerifyPermissions(grantResults))
                {
                    // All required permissions have been granted, display contacts fragment.
                    //Snackbar.Make(layout, Resource.String.permision_available_contacts, Snackbar.LengthShort).Show();

                }
                else
                {
                    Log.Info(this.Title.ToString(), "Contacts permissions were NOT granted.");
                    // Snackbar.Make(layout, Resource.String.permissions_not_granted, Snackbar.LengthShort).Show();
                }

            }
            else
            {
                base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            }
        }

        public void OnSharedPreferenceChanged(ISharedPreferences sharedPreferences, string key)
        {
            // we dont change preferences
        }

        #endregion << Methods >>

    }
}
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using Android;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Media;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V4.View;
using Android.Support.V4.Widget;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using Xamarin.Essentials;

namespace PS4_PKG_Viewer_Android
{
    [Activity(Label = "PS4 PKG Viewer", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true)]
    public class MainActivity : AppCompatActivity, NavigationView.IOnNavigationItemSelectedListener
    {
        TextView txtTitle = null;
        TextView txtContentID = null;
        TextView txtPKGType = null;
        TextView txtSystemVersion = null;
        ImageView imgPkg = null;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_main);
            Android.Support.V7.Widget.Toolbar toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);

            FloatingActionButton fab = FindViewById<FloatingActionButton>(Resource.Id.fab);
            fab.Click += FabOnClick;

            DrawerLayout drawer = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
            ActionBarDrawerToggle toggle = new ActionBarDrawerToggle(this, drawer, toolbar, Resource.String.navigation_drawer_open, Resource.String.navigation_drawer_close);
            drawer.AddDrawerListener(toggle);
            toggle.SyncState();

            NavigationView navigationView = FindViewById<NavigationView>(Resource.Id.nav_view);
            navigationView.SetNavigationItemSelectedListener(this);

            txtTitle = FindViewById<TextView>(Resource.Id.txtTitle);

            txtContentID = FindViewById<TextView>(Resource.Id.txtContentID);

            txtPKGType = FindViewById<TextView>(Resource.Id.txtType);
            txtSystemVersion = FindViewById<TextView>(Resource.Id.txtVer);

            imgPkg = FindViewById<ImageView>(Resource.Id.imgpkg);
            ListView listView = (ListView)FindViewById(Resource.Id.lstSFO);
            listView.Visibility = ViewStates.Gone;
            View usermenu = navigationView.GetHeaderView(0);
            TextView txtVersion = usermenu.FindViewById<TextView>(Resource.Id.version);

            try
            {
                // get the version number of the application
                txtVersion.Text = "VERSION " + Application.Context.ApplicationContext.PackageManager.GetPackageInfo(Application.Context.ApplicationContext.PackageName, 0).VersionName;
            }
            catch (Exception ex)
            {
                txtVersion.Text = "VERSION 1.00";
                //Crashes.TrackError(ex);
            }
        }

        public override void OnBackPressed()
        {
            DrawerLayout drawer = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
            if (drawer.IsDrawerOpen(GravityCompat.Start))
            {
                drawer.CloseDrawer(GravityCompat.Start);
            }
            else
            {
                base.OnBackPressed();
            }
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.menu_main, menu);
            return true;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            int id = item.ItemId;
            if (id == Resource.Id.action_settings)
            {
                return true;
            }

            return base.OnOptionsItemSelected(item);
        }
        MediaPlayer currentPlayer;
        public void Play(byte[] AudioFile)
        {
            Stop();
            currentPlayer = new MediaPlayer();
            currentPlayer.Prepared += (sender, e) =>
            {
                currentPlayer.Start();
            };
            currentPlayer.SetDataSource(new StreamMediaDataSource(new System.IO.MemoryStream(AudioFile)));
            currentPlayer.Prepare();
        }

        void Stop()
        {
            if (currentPlayer == null)
                return;

            currentPlayer.Stop();
            currentPlayer.Dispose();
            currentPlayer = null;
        }

        public class StreamMediaDataSource : MediaDataSource
        {
            System.IO.Stream data;

            public StreamMediaDataSource(System.IO.Stream Data)
            {
                data = Data;
            }

            public override long Size
            {
                get
                {
                    return data.Length;
                }
            }

            public override int ReadAt(long position, byte[] buffer, int offset, int size)
            {
                data.Seek(position, System.IO.SeekOrigin.Begin);
                return data.Read(buffer, offset, size);
            }

            public override void Close()
            {
                if (data != null)
                {
                    data.Dispose();
                    data = null;
                }
            }

            protected override void Dispose(bool disposing)
            {
                base.Dispose(disposing);

                if (data != null)
                {
                    data.Dispose();
                    data = null;
                }
            }
        }
        PS4_Tools.PKG.SceneRelated.Unprotected_PKG PKG = new PS4_Tools.PKG.SceneRelated.Unprotected_PKG();
        async Task<FileResult> PickAndShow(PickOptions options, View view)
        {
            try
            {
                var result = await FilePicker.PickAsync();
                if (result != null)
                {
                    string Text = $"File Name: {result.FileName}";
                    if (result.FileName.EndsWith("pkg", StringComparison.OrdinalIgnoreCase) ||
                        result.FileName.EndsWith("pkg", StringComparison.OrdinalIgnoreCase))
                    {
                        var stream = await result.OpenReadAsync();
                        PKG = PS4_Tools.PKG.SceneRelated.Read_PKG(stream);
                        if (PKG == null)
                        {
                            throw new Exception("File might be corrupt ?");
                        }
                        try
                        {
                            if (PKG.PS4_Title != null)
                            {
                                RunOnUiThread(() => txtTitle.Text = "Title : " + PKG.PS4_Title);
                            }
                        }
                        catch
                        {

                        }

                        try
                        {
                            RunOnUiThread(() => txtContentID.Text = "Content ID : " + PKG.Content_ID);
                        }
                        catch
                        {

                        }
                        try
                        {
                            RunOnUiThread(() => txtSystemVersion.Text = "System Version : " + PKG.Firmware_Version);
                        }
                        catch
                        {

                        }
                        try
                        {
                            RunOnUiThread(() => txtPKGType.Text = "Type : " + PKG.PKGState.ToString());
                        }
                        catch
                        {

                        }
                        try
                        {
                            var imageBitmap = BitmapFactory.DecodeByteArray(PKG.Icon, 0, PKG.Icon.Length);
                            RunOnUiThread(() => imgPkg.SetImageBitmap(imageBitmap));
                        }
                        catch (Exception ex)
                        {
                            try
                            {
                                var imageBitmap = BitmapFactory.DecodeByteArray(PKG.Image, 0, PKG.Image.Length);
                                RunOnUiThread(() => imgPkg.SetImageBitmap(imageBitmap));
                            }
                            catch
                            {

                            }
                        }
                       

                        return result;
                    }
                }
            }
            catch (Exception ex)
            {
                // The user canceled or something went wrong
                Snackbar.Make(view, ex.Message, Snackbar.LengthLong)
                .SetAction("Action", (Android.Views.View.IOnClickListener)null).Show();
                return null;
            }
            return null;
        }
        private void FabOnClick(object sender, EventArgs eventArgs)
        {
            View view = (View)sender;


            var options = new PickOptions
            {
                PickerTitle = "Please select a PKG file",
                //FileTypes = customFileType,
            };
            var item = PickAndShow(options, view);


        }

        public bool OnNavigationItemSelected(IMenuItem item)
        {
            int id = item.ItemId;

            if (id == Resource.Id.nav_mediaplayer)
            {
                try
                {
                    if (PKG.Sound != null)
                    {
                        var bytes = PS4_Tools.Media.Atrac9.LoadAt9(PKG.Sound);
                        Play(bytes);
                    }
                }
                catch (Exception ex)
                {

                }
            }
            else if (id == Resource.Id.nav_sfo)
            {
                TextView txtSFO = FindViewById<TextView>(Resource.Id.txtSFO);
                txtSFO.Visibility = ViewStates.Visible;
                txtSFO.Text = "SFO INFO";
                if (PKG.Param != null)
                {
                    List<string> ListItems = new List<string>();
                    for (int i = 0; i < PKG.Param.Tables.Count; i++)
                    {
                        ListItems.Add(PKG.Param.Tables[i].Name + ":" + PKG.Param.Tables[i].Value);
                    }

                    ArrayAdapter adapter = new ArrayAdapter<String>(this, Android.Resource.Layout.SimpleListItem1, ListItems);

                    ListView listView = (ListView)FindViewById(Resource.Id.lstSFO);
                    listView.Adapter = (adapter);
                    listView.Visibility = ViewStates.Visible;
                }
               
      
            }
            else if (id == Resource.Id.nav_home)
            {
                
            }
            else if (id == Resource.Id.nav_manage)
            {
                TextView txtSFO = FindViewById<TextView>(Resource.Id.txtSFO);
                txtSFO.Visibility = ViewStates.Visible;
                txtSFO.Text = "INFO";
               // if (PKG.Param != null)
                {
                    var items = PKG.Header.DisplayInfo();

                    ArrayAdapter adapter = new ArrayAdapter<String>(this, Android.Resource.Layout.SimpleListItem1, items);

                    ListView listView = (ListView)FindViewById(Resource.Id.lstSFO);
                    listView.Visibility = ViewStates.Visible;
                    listView.Adapter = (adapter);
                }
            }
            else if (id == Resource.Id.nav_share)
            {
                Intent i = new Intent(Intent.ActionSend);
                i.SetType("text/plain");
                i.PutExtra(Intent.ExtraSubject, "Share the app");
                i.PutExtra(Intent.ExtraText, "Check out PS4 Tools on Android \n" + @"https://github.com/xXxTheDarkprogramerxXx/PS4_Tools/releases");
                StartActivity(Intent.CreateChooser(i, "Share URL"));
            }
            //else if (id == Resource.Id.nav_send)
            //{

            //}

            DrawerLayout drawer = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
            drawer.CloseDrawer(GravityCompat.Start);
            return true;
        }
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}


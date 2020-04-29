using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Support.V4.Widget;
using SupportFragment = Android.Support.V4.App.Fragment;
using SupportFragmentManager = Android.Support.V4.App.FragmentManager;
using SupportToolbar = Android.Support.V7.Widget.Toolbar;
using SupportActionBar = Android.Support.V7.App.ActionBar;
using Android.Support.V7.App;
using Android.Support.Design.Widget;
using Android.Support.V4.View;
using Android.Support.V4.App;
using System.Collections.Generic;
using Java.Lang;
using DesignLibrary_Tutorial.Fragments;
using Android.Locations;
using System.Linq;
using System.Threading.Tasks;
using Square.Picasso;
using Android.Graphics.Drawables;
using Android.Graphics;
using Android.Media;
using Plugin.FilePicker.Abstractions;
using Plugin.FilePicker;

namespace DesignLibrary_Tutorial
{
    [Activity(Label = "PS4 Tools", Theme = "@style/Theme.DesignDemo",ScreenOrientation =Android.Content.PM.ScreenOrientation.Landscape)]
    public class MainActivity : AppCompatActivity
    {
        private DrawerLayout mDrawerLayout;

        public static Activity context;

        public static double myLocationLatitude;
        public static double myLocationLongitude;

       

        MediaPlayer _player;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            this.Title = "PS4 Tools";

            SupportToolbar toolBar = FindViewById<SupportToolbar>(Resource.Id.toolBar);
            SetSupportActionBar(toolBar);
            toolBar.SetTitleTextColor(Color.White);

            SupportActionBar ab = SupportActionBar;
            ab.SetHomeAsUpIndicator(Resource.Drawable.ic_menu);
            ab.SetDisplayHomeAsUpEnabled(true);
            mDrawerLayout = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);

            _player = MediaPlayer.Create(this, Resource.Raw.scan);
            //var progressDialog = ProgressDialog.Show(this, "Please wait..", "Loading user account...", true);
            new System.Threading.Thread(new System.Threading.ThreadStart(delegate
            {

                RunOnUiThread(() =>
                {

                    try
                    {
                        NavigationView navigationView = FindViewById<NavigationView>(Resource.Id.nav_view);
                        if (navigationView != null)
                        {
                            SetUpDrawerContent(navigationView);
                        }

                        TabLayout tabs = FindViewById<TabLayout>(Resource.Id.tabs);

                        ViewPager viewPager = FindViewById<ViewPager>(Resource.Id.viewpager);

                        SetUpViewPager(viewPager);

                        tabs.SetupWithViewPager(viewPager);
                        FloatingActionButton fab = FindViewById<FloatingActionButton>(Resource.Id.fab);
                        fab.Visibility = ViewStates.Visible;
                        fab.Click += async (o, e) =>
                    {
                        View anchor = o as View;

                        string PKGLocation;
                        System.IO.Stream stream;
                        try
                        {
                            FileData fileData = await CrossFilePicker.Current.PickFile();
                            if (fileData == null)
                            {
                                return;
                            }

                            string fileName = fileData.FileName;
                            //string contents = System.Text.Encoding.UTF8.GetString(fileData.DataArray);
                            PKGLocation = fileData.FilePath;

                            //test to make sure file is not URI file 

                            if (PKGLocation.ToUpper().Contains("CONTENT://"))
                            {
                                stream = new System.IO.MemoryStream(fileData.DataArray);
                                //path needs to be made from a stream
                                var pkgfile = PS4_Tools.PKG.SceneRelated.Read_PKG(stream);
                                //ImageView pbPkg = FindViewById<ImageView>(Resource.Id.PKGIcon);
                                //pbPkg.SetImageBitmap(BytesToBitmap(pkgfile.Image));
                                //TextView lblPackageInfo = FindViewById<TextView>(Resource.Id.txtPKGInfo);
                                //lblPackageInfo.Text = pkgfile.PS4_Title + "\n" + pkgfile.PKG_Type.ToString() + "\n" +
                                //                      pkgfile.Param.TitleID; //display whatever info youd like here
                            }
                            else
                            {

                                var pkgfile = PS4_Tools.PKG.SceneRelated.Read_PKG(PKGLocation);
                                //ImageView pbPkg = FindViewById<ImageView>(Resource.Id.PKGIcon);
                                //pbPkg.SetImageBitmap(BytesToBitmap(pkgfile.Image));
                                //TextView lblPackageInfo = FindViewById<TextView>(Resource.Id.txtPKGInfo);
                                //lblPackageInfo.Text = pkgfile.PS4_Title + "\n" + pkgfile.PKG_Type.ToString() + "\n" +
                                //                      pkgfile.Param.TitleID; //display whatever info youd like here
                            }

                            System.Console.WriteLine("File name chosen: " + fileName);
                            //System.Console.WriteLine("File data: " + contents);
                        }
                        catch (System.Exception ex)
                        {
                            System.Console.WriteLine("Exception choosing file: " + ex.ToString());
                        }

                    };


                        View usermenu = navigationView.GetHeaderView(0);

                        ImageView imguser = usermenu.FindViewById<ImageView>(Resource.Id.imgViewHeader);

                        //replace image from cheese to cranswick logo
                        imguser.SetImageResource(Resource.Drawable.ps4_tools);

                        //do the image click event 

                        imguser.Click += delegate
                        {
                            //Intent intent = new Intent(this, typeof(Account));
                            //StartActivity(intent);
                        };

                        //set the user pic
                        //Picasso.With(this)
                        //      .Load(Constants.user_details.Image)
                        //      .Into(imguser);

                        TextView txtusersname = usermenu.FindViewById<TextView>(Resource.Id.txtUserName);

                        txtusersname.Text = "Hi " + Constants.Operator;
                        context = this;


                        System.Timers.Timer timer = new System.Timers.Timer(TimeSpan.FromMinutes(10).TotalMilliseconds);//refresh every 1 min
                        timer.Elapsed += Timer_Elapsed;
                        timer.Enabled = true;
                    }
                    catch (System.Exception ex)
                    {
                        string errormessage = ex.Message;
                    }
                });


               

            })).Start();
        }

        private void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            ReleadData();
        }

        public static void ReleadData()
        {
            Fragment1.ReleadData();
        }

        public override void OnBackPressed()
        {
            ////set alert for executing the task
            //Android.Support.V7.App.AlertDialog.Builder alert = new Android.Support.V7.App.AlertDialog.Builder(this);
            //alert.SetTitle("Log Out");
            //alert.SetMessage("Are you sure you want to log out ?");
            //alert.SetPositiveButton("Log Out", (senderAlert, args) => {
                this.Finish();
                base.OnBackPressed();
            //});

            //alert.SetNegativeButton("Cancel", (senderAlert, args) => {
              
            //});

            //Dialog dialog = alert.Create();
            //dialog.Show();
            
        }

        private void SetUpViewPager(ViewPager viewPager)
        {

            TabAdapter adapter = new TabAdapter(SupportFragmentManager);
            adapter.AddFragment(new Fragment1(), "PUP");
            adapter.AddFragment(new Fragment2(), "Imaging");
            adapter.AddFragment(new Fragment1(), "RCO");
            adapter.AddFragment(new Fragment1(), "PKG");
            adapter.AddFragment(new Fragment1(), "Media");
            adapter.AddFragment(new Fragment1(), "Licensing");
            adapter.AddFragment(new Fragment1(), "Sava Data");
            adapter.AddFragment(new Fragment1(), "Remastered");
            //adapter.AddFragment(new Fragment2(), "Fragment 2");
            //adapter.AddFragment(new Fragment3(), "Fragment 3");

            viewPager.Adapter = adapter;

        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Android.Resource.Id.Home:
                    mDrawerLayout.OpenDrawer((int)GravityFlags.Left);
                    return true;
                //case Resource.Id.Account:
                //    Intent intent = new Intent(this, typeof(Account));
                //    StartActivity(intent);
                //    return (true);
                case Resource.Id.nav_home_camera:
                    try
                    {

                        //var progressDialog = ProgressDialog.Show(context, "", "Scanning", true);

                        FloatingActionButton fab = FindViewById<FloatingActionButton>(Resource.Id.fab);
                        fab.PerformClick();

                    }
                    catch (System.Exception ex)
                    {
                        string brokenforshit = ex.Message;
                    }
                    return true;
                default:
                    return base.OnOptionsItemSelected(item);                    
            }
        }

        private void SetUpDrawerContent(NavigationView navigationView)
        {
            navigationView.NavigationItemSelected += (object sender, NavigationView.NavigationItemSelectedEventArgs e) =>
            {
                switch (e.MenuItem.ItemId)
                {
                    case Resource.Id.nav_home:
                        e.MenuItem.SetChecked(true);
                        mDrawerLayout.CloseDrawers();
                        return;
                    case Resource.Id.nav_messages:

                        //open the notifications intent
                        Intent notificationintent = new Intent(this, typeof(UserNotification));
                        StartActivity(notificationintent);
                        e.MenuItem.SetChecked(false);
                        mDrawerLayout.CloseDrawers();
                        return;

                    case Resource.Id.action_settings:
                        Intent settings_intent = new Intent(this, typeof(Startup.SettingsActivity));
                        StartActivity(settings_intent);
                        e.MenuItem.SetChecked(false);
                        mDrawerLayout.CloseDrawers();
                        return;
                    default:
                        e.MenuItem.SetChecked(true);
                        mDrawerLayout.CloseDrawers();
                        return;
                }
            };
        }


        public override bool OnCreateOptionsMenu(IMenu menu)
        {
           // MenuInflater.Inflate(Resource.Menu.actionbar, menu);

           // var item = menu.FindItem(Resource.Id.action_search);

            try
            {

                //var searchItem = MenuItemCompat.GetActionView(item);
                //_searchView = searchItem.JavaCast<SearchView>();
                //_searchView.PerformClick();
                //_searchView.QueryTextChange += (s, e) =>
                //{
                //    if(e.ToString().Length >= 5)
                //    {
                //        //e.NewText;
                //    }
                //   // e.NewText;
                //};

                //_searchView.QueryTextSubmit += (s, e) =>
                //{
                ////TODO: Do something fancy when search button on keyboard is pressed
                ////Toast.MakeText(this, "Searched for: " + e.Query, ToastLength.Short).Show();
                ////open search window here 
                //    e.Handled = true;
                //};

                //item = menu.FindItem(Resource.Id.nav_messages);

                //View actionView = MenuItemCompat.GetActionView(item);
                //textNotificationItemCount = (TextView)actionView.FindViewById(Resource.Id.cart_badge);
                //textNotificationItemCount.Click += delegate
                //{
                //    //Intent intent = new Intent(this, typeof(Account));
                //    //StartActivity(intent);
                //};
                //actionView.Click += delegate
                //{
                //    //Intent intent = new Intent(this, typeof(Account));
                //    //StartActivity(intent);
                //};
            }
            catch (System.Exception ex)
            {

            }

            return base.OnCreateOptionsMenu(menu);
        }
        public class TabAdapter : FragmentPagerAdapter
        {
            public List<SupportFragment> Fragments { get; set; }
            public List<string> FragmentNames { get; set; }

            public TabAdapter (SupportFragmentManager sfm) : base (sfm)
            {
                Fragments = new List<SupportFragment>();
                FragmentNames = new List<string>();
            }

            public void AddFragment(SupportFragment fragment, string name)
            {
                Fragments.Add(fragment);
                FragmentNames.Add(name);
            }

            public override int Count
            {
                get
                {
                    return Fragments.Count;
                }
            }

            public override SupportFragment GetItem(int position)
            {
                return Fragments[position];
            }

            public override ICharSequence GetPageTitleFormatted(int position)
            {
                return new Java.Lang.String(FragmentNames[position]);
            }
        }

        private string CransTechPath(string ApplicationName)
        {

            string returnstring = "";

            if (Android.OS.Environment.IsExternalStorageEmulated == true)
            {

                returnstring = System.IO.Path.Combine(Android.OS.Environment.ExternalStorageDirectory.AbsolutePath, "Cranswick", ApplicationName);

                if (!System.IO.Directory.Exists(returnstring))
                {
                    System.IO.Directory.CreateDirectory(returnstring);
                }
            }
            else
            {
                returnstring = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments), "Cranswick", ApplicationName);
                if (!System.IO.Directory.Exists(returnstring))
                {
                    System.IO.Directory.CreateDirectory(returnstring);
                }
            }
            return returnstring;
        }

    }
}


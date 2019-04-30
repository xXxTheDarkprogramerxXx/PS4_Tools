using System.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

using Android.App;
using Android.OS;
using SupportToolbar = Android.Support.V7.Widget.Toolbar;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Support.V4.Widget;
using Android.Support.Design.Widget;
using System;
using Android.Widget;
using DesignLibrary_Tutorial.Helpers;
using Android.Views;
using Square.Picasso;
using Android.Graphics;
using Android.Util;
using Android.Content;
using DesignLibrary.Helpers;
using System.Threading;
using Android.Graphics.Drawables;
using Android.Provider;
using System.IO;
using System.Net;
using System.Xml.Linq;
using System.Collections.Specialized;
using System.Net.Sockets;

namespace DesignLibrary_Tutorial
{
    [Activity(Label = "Login", Theme = "@style/Theme.DesignDemo")]
    public class Login : AppCompatActivity
    {
       

        CollapsingToolbarLayout collapsingToolBar;
        public static Context context;

        string Operator = string.Empty;
        string OperatorPass = string.Empty;
        string Company = string.Empty;
        string CompanyPass = string.Empty;

        static ProgressDialog progressDialog;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);


            SetContentView(Resource.Layout.Account_layout);
            
           

            SupportToolbar toolBar = FindViewById<SupportToolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolBar);
            SupportActionBar.SetDisplayHomeAsUpEnabled(false);

            toolBar.SetTitleTextColor(Color.Black);

            string cheeseName = Intent.GetStringExtra("Login");
            collapsingToolBar = FindViewById<CollapsingToolbarLayout>(Resource.Id.collapsing_toolbar);
            collapsingToolBar.Title = cheeseName;
            // collapsingToolBar.SetCollapsedTitleTextColor(Color.Black);
            ImageView imageView1 = FindViewById<ImageView>(Resource.Id.imageView1);
            imageView1.Visibility = ViewStates.Invisible;

            LoadBackDrop();

            FloatingActionButton fab = FindViewById<FloatingActionButton>(Resource.Id.Img);
            fab.Click += delegate
            {
                //floating action button to open settings
                StartActivity(typeof(Startup.SettingsActivity));
            };

            //get username and password ext ext 

            TextView txtUsername = FindViewById<TextView>(Resource.Id.txtUsername);
            TextView txtUserPassword = FindViewById<TextView>(Resource.Id.txtUserPass);
            TextView txtCompany = FindViewById<TextView>(Resource.Id.txtCompany);
            TextView txtCompPass = FindViewById<TextView>(Resource.Id.txtCompanyPass);

            try
            {
                /*Operator*/
                txtUsername.FocusChange += (sender, args) =>
                {
                    if (args.HasFocus == false)
                    {
                    /*Only way i could get event leave event to fire*/
                        new System.Threading.Thread(new System.Threading.ThreadStart(delegate
                        {

                        //NotificationSystem.ShowNotification(this);

                                try
                                {
                                    string defaultCompany = Constants._service.Login_GetDefaultCompany(txtUsername.Text.TrimEnd());

                                RunOnUiThread(() =>
                                {
                                    txtCompany.Text = defaultCompany;
                                });
                                string defaultCompanyPasword = Constants._service.Login_GetDefaultCompanyPassword(defaultCompany);
                                RunOnUiThread(() =>
                                {
                                    txtCompPass.Text = defaultCompanyPasword;
                                });
                                }
                                catch(Exception ex)
                                {
                                    ViewDialog alert = new ViewDialog();


                                    RunOnUiThread(() => alert.showDialog(this, "Error Connecting to service\n" +ex.Message ));
                                }
                           
                        })).Start();

                        Operator = txtUsername.Text.TrimEnd();
                    }
                };

                /*Operator Password*/
                txtUserPassword.FocusChange += (sender, args) =>
                {
                    if (args.HasFocus == false)
                    {
                    /*Only way i could get event leave event to fire*/

                        OperatorPass = txtUserPassword.Text.TrimEnd();
                    }
                };

                /*Company*/
                txtCompany.FocusChange += (sender, args) =>
                {
                    if (args.HasFocus == false)
                    {
                    /*Only way i could get event leave event to fire*/

                        Company = txtCompany.Text.TrimEnd();
                    }
                };

                /*Company Pass*/
                txtCompPass.FocusChange += (sender, args) =>
                {
                    if (args.HasFocus == false)
                    {
                    /*Only way i could get event leave event to fire*/

                        CompanyPass = txtCompPass.Text.TrimEnd();
                    }
                };

                txtCompPass.KeyPress += TxtCompPass_KeyPress;

                context = this;

                try
                {
                    //_service = new ServiceWrapper();
                    if (Constants._service == new WMSScanner.WMSService())
                    {
                        OpenSettings();
                    }
                }
                catch (Exception ex)
                {
                    //we need to check if the URL is set if not open up the screen for preferences
                    //open url development 
                    OpenSettings();


                }
            }
            catch(Exception ex)
            {
                ViewDialog alert = new ViewDialog();


                RunOnUiThread(() => alert.showDialog(this, "Error Connecting to service\n" + ex.Message));
            }
        }


        public void OpenSettings()  
        {
            StartActivity(typeof(Startup.SettingsActivity));

          
           // _service = new ServiceWrapper();
        }

        public void DoLogin()
        {
            progressDialog = ProgressDialog.Show(this, "Please wait...", "Checking For Updates...", true);
            new Thread(new ThreadStart(delegate
            {
            //NotificationSystem.ShowNotification(this);


            string IpAdress = GetIp();
            IPAddress[] addresses = Dns.GetHostAddresses(Dns.GetHostName());
            string ipAddress = string.Empty;
            if (addresses != null && addresses[0] != null)
            {
                ipAddress = addresses[0].ToString();
            }
            else
            {
                ipAddress = null;
            }

            var android_id = Android.Provider.Settings.Secure.GetString(ContentResolver, Android.Provider.Settings.Secure.AndroidId);
                try
                {
                    WMSScanner.LoginResult lr = Constants._service.Login_Login(Operator, OperatorPass, Company, CompanyPass, "D", ipAddress, android_id, "Android Scanner", "");
                    if (lr.LoggedInCorrectly == true)
                    {
                        try
                        {
                            WMSScanner.DataTableResult dtr = Constants._service.CachScannerVariables(lr.WMSGuid);
                            if (dtr.Successful == true)
                            {
                                Update_Variables.NotFoundCode = dtr.ResultDT.Rows[0]["SettingValue"].ToString().Trim();
                                Update_Variables.Trolly = dtr.ResultDT.Rows[0]["Trolley"].ToString().Trim();
                                Update_Variables.MustScanPassword = dtr.ResultDT.Rows[0]["MustScanPass"].ToString().Trim();
                                Update_Variables.WMSWarehouse = dtr.ResultDT.Rows[0]["WMSWarehouse"].ToString().Trim();
                                Update_Variables.AutoReplace = dtr.ResultDT.Rows[0]["AutoReplace"].ToString().Trim();
                                Update_Variables.Company = Company;
                                Update_Variables.MustCheckoutInsertPalletNumbering = dtr.ResultDT.Rows[0]["UsePalletNumberingOnCheckout"].ToString().Trim();


                                Update_Variables.MustFillLeadingZerosSalesOrder = bool.Parse(dtr.ResultDT.Rows[0]["MustFillLeadingZerosSalesOrder"].ToString().Trim());
                                Update_Variables.MustFillLeadingZerosInvoice = bool.Parse(dtr.ResultDT.Rows[0]["MustFillLeadingZerosInvoice"].ToString().Trim());
                                Update_Variables.MustFillLeadingZerosPO = bool.Parse(dtr.ResultDT.Rows[0]["MustFillLeadingZerosPO"].ToString().Trim());

                                Update_Variables.MustFillLeadingZerosSalesOrderAmount = int.Parse(dtr.ResultDT.Rows[0]["MustFillLeadingZerosSalesOrderAmount"].ToString());
                                Update_Variables.MustFillLeadingZerosInvoiceAmount = int.Parse(dtr.ResultDT.Rows[0]["MustFillLeadingZerosInvoiceAmount"].ToString());
                                Update_Variables.MustFillLeadingZerosPOAmount = int.Parse(dtr.ResultDT.Rows[0]["MustFillLeadingZerosPOAmount"].ToString());

                            }

                            WMSScanner.DataTableResult mustScanResult = Constants._service.CachScannerMustScan(lr.WMSGuid);
                            if (mustScanResult.Successful == true)
                            {
                                Update_Variables.MustScanItems = mustScanResult.ResultDT;
                            }

                            /*Will Exclude this for now testing still needs to be done*/
                            //WriteFiles(username, "", "");

                            Constants.CompanyPassword = "";
                            Constants.Password = "";
                            Constants.Operator = Operator.Trim();
                            Update_Variables.WMSGuid = new Guid(lr.WMSGuid);
                            Update_Variables.SysproGuid = lr.SysproGuid;

                            RunOnUiThread(() => Toast.MakeText(this, "Logged in to WMS Mobile Scanner.", ToastLength.Long).Show());

                            //we need to set the operator as active for today
                            WMSScanner.BoolResult activeflaged = Constants._service.Android_Update_Operator_Active_Flag(Update_Variables.WMSGuid.ToString());
                            if(activeflaged.Successful == false)
                            {
                                RunOnUiThread(() => progressDialog.Cancel());
                                ViewDialog alert = new ViewDialog();


                                RunOnUiThread(() => alert.showDialog(this, activeflaged.Message));
                                return;
                            }

                            //StartService(new Intent(this, typeof(CheckPickActivity)));
                            /*Start the WMS Check For New Assignment Notification*/
                            RunOnUiThread(() => progressDialog.Cancel());
                            var ScannerMenu = new Intent(this, typeof(MainActivity));/*New Main Activity*/
                            StartActivity(ScannerMenu);
                           
                        }
                        catch (Exception ex)
                        {
                            RunOnUiThread(() => progressDialog.Cancel());
                            RunOnUiThread(() => Toast.MakeText(this, ex.Message.ToString(), ToastLength.Long));
                        }
                    }
                    else
                    {
                        RunOnUiThread(() => progressDialog.Hide());
                        ViewDialog alert = new ViewDialog();


                        RunOnUiThread(() => alert.showDialog(this, lr.Message + "Could not log in"));

                    }
                }
                catch (Exception ex)
                {
                    RunOnUiThread(() => progressDialog.Hide());
                    ViewDialog alert = new ViewDialog();


                    RunOnUiThread(() => alert.showDialog(this, ex.Message));
                }
               
            })).Start();
        }


        private void TxtCompPass_KeyPress(object sender, View.KeyEventArgs e)
        {
            if (e.KeyCode == Keycode.Enter && e.Event.Action == KeyEventActions.Down)
            {
                //Enter Key Pressed Do Login
                DoLogin();
            }
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Android.Resource.Id.Home:
                    Finish();
                    return true;
            }

            return base.OnOptionsItemSelected(item);
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.sample_actions, menu);
            return true;
        }

        private void LoadBackDrop()
        {
            ImageView imageView = FindViewById<ImageView>(Resource.Id.backdrop);

            imageView.SetImageResource(Resource.Drawable.cransblack);
        }

        public static string GetIp()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            //throw new Exception("Local IP Address Not Found!");
            //don't throw an exception we just need to figure out the right way
            return "";
        }
    }

    public class ViewDialog
    {

        public void showDialog(Activity activity, string msg)
        {
            Dialog dialog = new Dialog(activity);
            dialog.RequestWindowFeature((int)WindowFeatures.NoTitle);
            dialog.SetCancelable(false);
            dialog.SetContentView(Resource.Layout.MessageBox_Layout);

            TextView text = (TextView)dialog.FindViewById(Resource.Id.text_dialog);
            text.Text = msg;

            Button dialogButton = (Button)dialog.FindViewById(Resource.Id.btn_dialog);
            dialogButton.Click += delegate
            {
                dialog.Dismiss();
            };

            dialog.Show();

        }
    }
}
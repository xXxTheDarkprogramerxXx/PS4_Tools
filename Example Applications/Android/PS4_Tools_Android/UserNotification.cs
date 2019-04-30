using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

using Android.App;
using Android.OS;
using SupportToolbar = Android.Support.V7.Widget.Toolbar;
using Android.Support.V7.App;
using Android.Support.Design.Widget;
using Android.Widget;
using Android.Content.PM;
using Android.Content;
using Android.Views;
using Android.Support.V7.Widget;
using SupportFragment = Android.Support.V4.App.Fragment;
using Android.Graphics;
using Android.Util;
using Android.Content;
using Android.Content.Res;
using DesignLibrary.Helpers;
using SupportActionBar = Android.Support.V7.App.ActionBar;
using Android.Support.V4.View;
using Android.Support.V4.App;
using Java.Lang;
using Android.Graphics.Drawables;

namespace DesignLibrary_Tutorial
{
    [Activity(Label = "UserNotification", Theme = "@style/Base.Theme.DesignDemo")]
    public class UserNotification : AppCompatActivity
    {
        public static Context context;

        public static NotificationAdapter _adapter;

        List<Notifications> allnotice = new List<Notifications>();

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);


            SetContentView(Resource.Layout.NotificationsLayout);

            SupportToolbar toolBar = FindViewById<SupportToolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolBar);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            Title = "Notifications";
            context = this;

            RecyclerView recyclerView = FindViewById<RecyclerView>(Resource.Id.recyclerview);
            SetUpRecyclerView(recyclerView);
        }

        private void SetUpRecyclerView(RecyclerView recyclerView)
        {
            var progressDialog = ProgressDialog.Show(context, "", "", true);
            new System.Threading.Thread(new System.Threading.ThreadStart(delegate
            {
                this.RunOnUiThread(() =>
                {
                    /*Notifications will be stored on local db ?*/
                    System.Data.DataTable dtnotifications = new System.Data.DataTable();

                    if (dtnotifications.Rows.Count > 0)
                    {
                        for (int i = 0; i < dtnotifications.Rows.Count; i++)
                        {
                            /* create a noticiation obj*/
                            Notifications notification_obj = new Notifications();
                            notification_obj.ID = Convert.ToInt32(dtnotifications.Rows[i]["ID"]);
                            notification_obj.Title = dtnotifications.Rows[i]["Title"].ToString();
                            notification_obj.Description = dtnotifications.Rows[i]["Description"].ToString();
                            DateTime datetemp = new DateTime();
                            DateTime.TryParse(dtnotifications.Rows[i]["Date"].ToString(), out datetemp);
                            notification_obj.Date = datetemp;
                            notification_obj.IsSeen = Convert.ToBoolean(dtnotifications.Rows[i]["IsSeen"]);
                            notification_obj.ProductRef = (dtnotifications.Rows[i]["ProductRef"] != DBNull.Value) ? Convert.ToInt32(dtnotifications.Rows[i]["ProductRef"]) : 0;
                            allnotice.Add(notification_obj);
                        }
                    }
                    else
                    {
                        Toast.MakeText(this, "No notifications at this time", ToastLength.Long).Show();
                    }


                    var values = allnotice;

                    recyclerView.SetLayoutManager(new LinearLayoutManager(recyclerView.Context));

                    _adapter = new NotificationAdapter(recyclerView.Context, values, context.Resources);
                    recyclerView.SetAdapter(_adapter);

                    recyclerView.SetItemClickListener((rv, position, view) =>
                    {
                        if (values[position].ProductRef != 0)
                        {
                            //An item has been clicked
                            //Context context = view.Context;
                            //Intent intent = new Intent(context, typeof(CatelogDetailActivity));
                            //intent.PutExtra(CatelogDetailActivity.IDX, values[position].ProductRef.ToString());

                            //context.StartActivity(intent);
                        }
                    });

                    //only handel long clicks
                    progressDialog.Hide();
                });
            })).Start();
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

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
        }  
    }
}
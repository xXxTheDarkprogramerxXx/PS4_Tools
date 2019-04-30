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


namespace DesignLibrary_Tutorial.Startup
{
    [Activity(Label = "Settings", Theme = "@style/Theme.DesignDemo")]
    class SettingsActivity : AppCompatActivity
    {

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Xml.pref_with_action);

            SupportToolbar toolBar = FindViewById<SupportToolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolBar);
            this.Title = "PS4 Tools Settings";
            SupportActionBar ab = SupportActionBar;
            //ab.SetHomeAsUpIndicator(Android.Resource.Drawable.back);
            ab.SetDisplayHomeAsUpEnabled(true);

            FragmentManager.BeginTransaction().Replace(Resource.Id.content_frame, new Fragments.SettingsFragment()).Commit();
        }
    }
}
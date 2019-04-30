using System;
using System.Collections.Generic;

using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Widget;
using Android.Views;
using Square.Picasso;
using Android.Graphics;
using Android.Util;
using Android.Content;
using DesignLibrary.Helpers;
using Android.Graphics.Drawables;
using System.IO;
using Android.Content.Res;
using SupportToolbar = Android.Support.V7.Widget.Toolbar;
using Android.Support.Design.Widget;
using ZXing.Mobile;
using Android.Media;

namespace DesignLibrary_Tutorial
{
    [Activity(Label = "Sales Order", Theme = "@style/Theme.DesignDemo")]
    public class TripScanner : AppCompatActivity
    {
        public static SimpleStringRecyclerViewAdapter _adapter;
        public static SimpleWMSRecyclerViewAdapter _adapterWMS;

        public static List<string> list;

        public static Context context;
        public static Activity act;

        public const string EXTRA_NAME = "cheese_name";//this is how we can pass paramters if need be
        public static string JobNo = "jobno";
        CollapsingToolbarLayout collapsingToolBar;
        //we now have to add this screens information
        public static WMSScanner.TripMaster SelectedSingleTrip = new WMSScanner.TripMaster();


        public static List<WMSScanner.TripDetail> tripdetails = new List<WMSScanner.TripDetail>();

        List<WMSControlIem> ItemsForList = new List<WMSControlIem>();

        public static bool FirstItem = true;


        public ZXing.Mobile.MobileBarcodeScanner scanner;

        MediaPlayer _player;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);


            SetContentView(Resource.Layout.Activity_SubCategory);
            act = this;

            //this needs to be initialized somewhere 
            MobileBarcodeScanner.Initialize(Application);
            //add this somewhere something keeps breaking
            scanner = new ZXing.Mobile.MobileBarcodeScanner();

            string cheeseName = Intent.GetStringExtra(EXTRA_NAME);
            JobNo = cheeseName;
            SupportToolbar toolBar = FindViewById<SupportToolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolBar);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            Title = "Invoice - " + cheeseName;
            toolBar.SetTitleTextColor(Color.White);

            context = this;
            var progressDialog = ProgressDialog.Show(this, "", "", true);
            new System.Threading.Thread(new System.Threading.ThreadStart(delegate
            {
                try
                {
                    RecyclerView recyclerView = FindViewById<RecyclerView>(Resource.Id.recyclerview);
                    //recyclerView.NestedScrollingEnabled = false;
                    recyclerView.HasFixedSize = true;
                    SetUpRecyclerView(recyclerView);


                    RecyclerView recyclerView1 = FindViewById<RecyclerView>(Resource.Id.recyclerview1);
                    //recyclerView.NestedScrollingEnabled = false;
                    recyclerView1.HasFixedSize = true;
                    SetUpRecyclerViewWMSItem(recyclerView1);

                    FloatingActionButton fab = FindViewById<FloatingActionButton>(Resource.Id.Filter);
                    fab.Visibility = ViewStates.Invisible;
                }
                catch (Exception ex)
                {

                }
                act.RunOnUiThread(() =>
                {
                        progressDialog.Cancel();
                });
            })).Start();
            _player = MediaPlayer.Create(this, Resource.Raw.scan);

            

            // Create your fragment here
        }

        private void SetUpRecyclerViewWMSItem(RecyclerView recyclerView)
        {

            //var progressDialog = ProgressDialog.Show(context, "", "", true);
            new System.Threading.Thread(new System.Threading.ThreadStart(delegate
            {
                act.RunOnUiThread(() =>
                {
                    ItemsForList.Clear();

                    //we need to fetch the info for this as well

                    int index = 0;
                    //still working on how to do this but for now this will have to do
                    WMSControlIem newitem = new WMSControlIem();
                    newitem.Idx = ++index;
                    newitem.Name = "Scan Sales Order";
                    newitem.Description = "Scan Sales Order";
                    newitem.ItemType = typeof(string);
                    newitem.IsReadOnly = false;
                    newitem.SuggestedItemName = "";
                    newitem.SuggestedItemValue = "";
                    newitem.IsPassword = false;


                    ItemsForList.Add(newitem);
                    var values = ItemsForList;

                    recyclerView.SetLayoutManager(new LinearLayoutManager(recyclerView.Context));

                    _adapterWMS = new SimpleWMSRecyclerViewAdapter(recyclerView.Context, values, Resources);
                    recyclerView.SetAdapter(_adapterWMS);

                    recyclerView.SetItemClickListener((rv, position, view) =>
                    {

                    });
                });
            }))
            {
                Name = "TripWMSControl"
            }.Start();
        }

        private void SetUpRecyclerView(RecyclerView recyclerView)
        {
            try
            {
                new System.Threading.Thread(new System.Threading.ThreadStart(delegate
                {
                    act.RunOnUiThread(() =>
                    {
                        //we need to fetch the info for this as well

                        WMSScanner.TripsDetails tripswithso = Constants._service.Android_Routing_Get_SalesOrders_On_Trips(Update_Variables.WMSGuid.ToString(), JobNo);
                        if (tripswithso.Successful == false)
                        {
                            //show error
                        }

                        //check if items has user on them
                        List<WMSScanner.TripDetail> itemsfordisplay = new List<WMSScanner.TripDetail>();
                        for (int i = 0; i < tripswithso.trips.Length; i++)
                        {
                            if (tripswithso.trips[i].UserLastScannedk__BackingField == null)
                            {
                                itemsfordisplay.Add(tripswithso.trips[i]);
                            }
                            else
                            {
                                //item has already been set 
                                FirstItem = false;
                            }
                        }

                        tripdetails.Clear();
                        tripdetails.AddRange(itemsfordisplay);


                        var values = tripdetails;

                        recyclerView.SetLayoutManager(new LinearLayoutManager(recyclerView.Context));

                        _adapter = new SimpleStringRecyclerViewAdapter(recyclerView.Context, values, Resources);
                        recyclerView.SetAdapter(_adapter);

                        recyclerView.SetItemClickListener((rv, position, view) =>
                    {

                    });
                    });
                }
                )
                )
                {
                    Name = "TripInvoice"
                }.Start();
            }
            catch(Exception ex)
            {
                string error = ex.Message;
            }
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            try
            {
                MenuInflater.Inflate(Resource.Menu.subactiobar, menu);

                
            }
            catch(Exception ex)
            {

            }
            return base.OnCreateOptionsMenu(menu);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            try
            {
                switch (item.ItemId)
                {
                    case Android.Resource.Id.Home:

                        return true;
                    case Resource.Id.nav_camera:
                        try
                        {

                            new System.Threading.Thread(new System.Threading.ThreadStart(async delegate
                            {
                                

                                var result = await scanner.Scan();

                                if (result != null)
                                {
                                    var founditem = tripdetails.Find(prod => prod._invoice == result.Text);
                                    if (founditem == null)
                                    {
                                        for (int i = 0; i < tripdetails.Count; i++)
                                        {
                                            if(tripdetails[i]._invoice == result.Text)
                                            {
                                                founditem = tripdetails[i];
                                            }
                                        }
                                    }
                                    if (founditem != null)
                                    {

                                        _player.Start();
                                    //add the text to the item
                                    EditText mActionText = FindViewById<EditText>(Resource.Id.ActionText);
                                        act.RunOnUiThread(() =>
                                        {
                                            mActionText.Text = result.Text;

                                        //now we do our other validations here
                                        for (int i = 0; i < tripdetails.Count; i++)
                                            {
                                                if (tripdetails[i]._invoice.Contains(result.Text))
                                                {

                                                    if (FirstItem == true)
                                                    {
                                                        WMSScanner.BoolResult addfirstitemtowms = Constants._service.Android_Update_FirstScanDate(Update_Variables.WMSGuid.ToString(), JobNo, founditem._salesorder);
                                                        if (addfirstitemtowms.Successful == false)
                                                        {
                                                            Android.Support.V7.App.AlertDialog.Builder alert = new Android.Support.V7.App.AlertDialog.Builder(act);
                                                            alert.SetTitle("Error");
                                                            alert.SetMessage(addfirstitemtowms.Message);
                                                            alert.SetPositiveButton("OK", (senderAlert, args) =>
                                                            {

                                                            });
                                                            Dialog dialog = alert.Create();
                                                            dialog.Show();
                                                            FirstItem = true;
                                                        }

                                                    //post to wms that use can be locked to an order
                                                    FirstItem = false;
                                                    }
                                                    tripdetails.RemoveAt(i);

                                                    WMSScanner.BoolResult SalesOrderTrip = Constants._service.Android_Update_Operator_SalesOrder_On_Trip(Update_Variables.WMSGuid.ToString(), JobNo, founditem._salesorder);
                                                    if (SalesOrderTrip.Successful == false)
                                                    {
                                                        Android.Support.V7.App.AlertDialog.Builder alert = new Android.Support.V7.App.AlertDialog.Builder(act);
                                                        alert.SetTitle("Error");
                                                        alert.SetMessage(SalesOrderTrip.Message);
                                                        alert.SetPositiveButton("OK", (senderAlert, args) =>
                                                        {

                                                        });
                                                        Dialog dialog = alert.Create();
                                                        dialog.Show();
                                                        FirstItem = false;
                                                        return;
                                                    }

                                                    break;
                                                }
                                            }
                                            if (tripdetails.Count == 0)
                                            {
                                            //this was the last item
                                            //scan it out and post it to WMS

                                            WMSScanner.BoolResult LastScan = Constants._service.Android_Update_EndScanDate(Update_Variables.WMSGuid.ToString(), JobNo, founditem._salesorder);
                                                if (LastScan.Successful == false)
                                                {
                                                    Android.Support.V7.App.AlertDialog.Builder alert = new Android.Support.V7.App.AlertDialog.Builder(act);
                                                    alert.SetTitle("Error");
                                                    alert.SetMessage(LastScan.Message);
                                                    alert.SetPositiveButton("OK", (senderAlert, args) =>
                                                    {

                                                    });
                                                    Dialog dialog = alert.Create();
                                                    dialog.Show();
                                                    FirstItem = false;
                                                    return;
                                                }

                                                Android.Support.V7.App.AlertDialog.Builder completed = new Android.Support.V7.App.AlertDialog.Builder(act);
                                                completed.SetTitle("WMS POD");
                                                completed.SetMessage("Sales Order Completed");
                                                completed.SetPositiveButton("OK", (senderAlert, args) =>
                                                {

                                                });
                                                Dialog completeddialog = completed.Create();
                                                completeddialog.Show();
                                            /*Android Seems To Cache These Values*/
                                                FirstItem = true;
                                                act.Finish();
                                                MainActivity.ReleadData();
                                            }
                                            _adapter.NotifyDataSetChanged();
                                            mActionText.Text = "";
                                        });
                                    }
                                    else
                                    {
                                        act.RunOnUiThread(() =>
                                        {
                                            Android.Support.V7.App.AlertDialog.Builder alert = new Android.Support.V7.App.AlertDialog.Builder(act);
                                            alert.SetTitle("WMS Trips");
                                            alert.SetMessage("Invoice not on Sales Order");
                                            alert.SetPositiveButton("OK", (senderAlert, args) =>
                                            {

                                            });
                                            try
                                            {
                                                Dialog dialog = alert.Create();
                                                dialog.Show();
                                            }
                                            catch (Exception ex)
                                            {
                                                //somehow the elert crashes now
                                            }
                                            //_adapter.NotifyDataSetChanged();
                                        });
                                    }
                                }

                            /*open camera screen*/
                            }))
                            {
                                Name = "TripOptions"
                            }.Start();
                        }
                        catch (Exception ex)
                        {
                            string brokenforshit = ex.Message;
                        }

                        return (true);
                    default:
                        return base.OnOptionsItemSelected(item);
                }
            }
            catch(Exception ex)
            {
                string error = ex.Message;
            }
            return (true);
        }
        public class SimpleStringRecyclerViewAdapter : RecyclerView.Adapter, IFilterable
        {
            private readonly TypedValue mTypedValue = new TypedValue();
            private int mBackground;
            private List<WMSScanner.TripDetail> mValues;
            Resources mResource;
            private Dictionary<int, int> mCalculatedSizes;
            private Filter mFilter;

            public SimpleStringRecyclerViewAdapter(Context context, List<WMSScanner.TripDetail> items, Resources res)
            {
                context.Theme.ResolveAttribute(Resource.Attribute.selectableItemBackground, mTypedValue, true);
                mBackground = mTypedValue.ResourceId;
                mValues = items;
                mResource = res;
                mCalculatedSizes = new Dictionary<int, int>();
            }

            public override int ItemCount
            {
                get
                {
                    return mValues.Count;
                }
            }

            public Filter Filter
            {
                get
                {
                    return mFilter;
                }
            }




            public override async void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
            {
                var simpleHolder = holder as SimpleViewHolder;

                simpleHolder.mBoundString = mValues[position]._invoice;
                simpleHolder.mTxtView.Text = mValues[position]._invoice + " - " + TripScanner.SelectedSingleTrip._comment;

                int drawableID = position;
                BitmapFactory.Options options = new BitmapFactory.Options();


                simpleHolder.mImageView.SetBackgroundDrawable(context.GetDrawable(Resource.Drawable.if_list_118647));
                //simpleHolder.mImageView.SetBackgroundDrawable(mValues[position].Image);
            }
            public static Bitmap drawableToBitmap(Android.Graphics.Drawables.Drawable drawable)
            {
                Bitmap bitmap = null;

                if (drawable is BitmapDrawable)
                {
                    BitmapDrawable bitmapDrawable = (BitmapDrawable)drawable;
                    if (bitmapDrawable.Bitmap != null)
                    {
                        return bitmapDrawable.Bitmap;
                    }
                }

                if (drawable.IntrinsicWidth <= 0 || drawable.IntrinsicHeight <= 0)
                {
                    bitmap = Bitmap.CreateBitmap(1, 1, Bitmap.Config.Argb8888); // Single color bitmap will be created of 1x1 pixel
                }
                else
                {
                    bitmap = Bitmap.CreateBitmap(drawable.IntrinsicWidth, drawable.IntrinsicHeight, Bitmap.Config.Argb8888);
                }

                Canvas canvas = new Canvas(bitmap);
                drawable.SetBounds(0, 0, canvas.Width, canvas.Height);
                drawable.Draw(canvas);
                return bitmap;
            }

            public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
            {
                View view = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.List_Item, parent, false);
                view.SetBackgroundResource(mBackground);

                return new SimpleViewHolder(view);
            }

        }
        public class SimpleViewHolder : RecyclerView.ViewHolder
        {
            public string mBoundString;
            public readonly View mView;
            public readonly ImageView mImageView;
            public readonly TextView mTxtView;

            public SimpleViewHolder(View view) : base(view)
            {
                mView = view;
                mTxtView = view.FindViewById<TextView>(Resource.Id.text1);
                mImageView = view.FindViewById<ImageView>(Resource.Id.avatar);
            }

            public override string ToString()
            {
                return base.ToString() + " '" + mTxtView.Text;
            }
        }



        public class SimpleWMSRecyclerViewAdapter : RecyclerView.Adapter, IFilterable
        {
            private readonly TypedValue mTypedValue = new TypedValue();
            private int mBackground;
            private List<WMSControlIem> mValues;
            Resources mResource;
            private Dictionary<int, int> mCalculatedSizes;
            private Filter mFilter;

            public SimpleWMSRecyclerViewAdapter(Context context, List<WMSControlIem> items, Resources res)
            {
                context.Theme.ResolveAttribute(Resource.Attribute.selectableItemBackground, mTypedValue, true);
                mBackground = mTypedValue.ResourceId;
                mValues = items;
                mResource = res;
                mCalculatedSizes = new Dictionary<int, int>();
            }

            public override int ItemCount
            {
                get
                {
                    return mValues.Count;
                }
            }

            public Filter Filter
            {
                get
                {
                    return mFilter;
                }
            }




            public override async void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
            {
                var simpleHolder = holder as WMSViewHolder;

                simpleHolder.mBoundString = mValues[position].Name;
                simpleHolder.mActionName.Text = mValues[position].Name;
                simpleHolder.mActionText.Text = mValues[position].Value;
                simpleHolder.mActionDescription.Text = mValues[position].SuggestedItemValue;
                simpleHolder.mbtnUp.Click += delegate { /*nothing right now*/};

                simpleHolder.mActionText.KeyPress += (object sender, View.KeyEventArgs e) =>
                {
                    e.Handled = false;
                    if (e.Event.Action == KeyEventActions.Down && e.KeyCode == Keycode.Enter)
                    {
                        simpleHolder.mbtnCheck.PerformClick();

                        //your logic here
                        e.Handled = true;
                    }
                };

                simpleHolder.mbtnCheck.Click += delegate (object sender, EventArgs e)
                {
                    var progressDialog = ProgressDialog.Show(context, "", "Processing", true);
                    new System.Threading.Thread(new System.Threading.ThreadStart(delegate
                    {
                        //add the text to the item
                        EditText mActionText = simpleHolder.mActionText;
                        act.RunOnUiThread(() =>
                        {
                            string currentinvoice = mActionText.Text;
                            var founditem = tripdetails.Find(prod => prod._invoice == currentinvoice);
                            if (founditem != null)
                            {


                                //now we do our other validations here
                                for (int i = 0; i < tripdetails.Count; i++)
                                {
                                    if (tripdetails[i]._invoice.Contains(currentinvoice))
                                    {

                                        if (FirstItem == true)
                                        {
                                            WMSScanner.BoolResult addfirstitemtowms = Constants._service.Android_Update_FirstScanDate(Update_Variables.WMSGuid.ToString(), JobNo, founditem._salesorder);
                                            if (addfirstitemtowms.Successful == false)
                                            {
                                                Android.Support.V7.App.AlertDialog.Builder alert = new Android.Support.V7.App.AlertDialog.Builder(act);
                                                alert.SetTitle("Error");
                                                alert.SetMessage(addfirstitemtowms.Message);
                                                alert.SetPositiveButton("OK", (senderAlert, args) =>
                                                {

                                                });
                                                Dialog dialog = alert.Create();
                                                dialog.Show();
                                                FirstItem = true;
                                                progressDialog.Hide();
                                                return;
                                            }

                                            //post to wms that use can be locked to an order
                                            FirstItem = false;
                                        }
                                        tripdetails.RemoveAt(i);

                                        WMSScanner.BoolResult SalesOrderTrip = Constants._service.Android_Update_Operator_SalesOrder_On_Trip(Update_Variables.WMSGuid.ToString(), JobNo, founditem._salesorder);
                                        if (SalesOrderTrip.Successful == false)
                                        {
                                            Android.Support.V7.App.AlertDialog.Builder alert = new Android.Support.V7.App.AlertDialog.Builder(act);
                                            alert.SetTitle("Error");
                                            alert.SetMessage(SalesOrderTrip.Message);
                                            alert.SetPositiveButton("OK", (senderAlert, args) =>
                                            {

                                            });
                                            Dialog dialog = alert.Create();
                                            dialog.Show();
                                            FirstItem = false;
                                            progressDialog.Hide();
                                            return;
                                        }

                                        break;
                                    }
                                }
                                if (tripdetails.Count == 0)
                                {
                                    //this was the last item
                                    //scan it out and post it to WMS

                                    WMSScanner.BoolResult LastScan = Constants._service.Android_Update_EndScanDate(Update_Variables.WMSGuid.ToString(), JobNo, founditem._salesorder);
                                    if (LastScan.Successful == false)
                                    {
                                        Android.Support.V7.App.AlertDialog.Builder alert = new Android.Support.V7.App.AlertDialog.Builder(act);
                                        alert.SetTitle("Error");
                                        alert.SetMessage(LastScan.Message);
                                        alert.SetPositiveButton("OK", (senderAlert, args) =>
                                        {

                                        });
                                        Dialog dialog = alert.Create();
                                        dialog.Show();
                                        FirstItem = false;
                                        progressDialog.Hide();
                                        return;
                                    }

                                    Android.Support.V7.App.AlertDialog.Builder completed = new Android.Support.V7.App.AlertDialog.Builder(act);
                                    completed.SetTitle("WMS POD");
                                    completed.SetMessage("Sales Order Completed");
                                    completed.SetPositiveButton("OK", (senderAlert, args) =>
                                    {

                                    });
                                    Dialog completeddialog = completed.Create();
                                    completeddialog.Show();
                                    /*Android Seems To Cache These Values*/
                                    FirstItem = true;
                                    progressDialog.Hide();
                                    act.Finish();
                                    MainActivity.ReleadData();
                                }
                                _adapter.NotifyDataSetChanged();
                                mActionText.Text = "";
                                progressDialog.Hide();
                            }
                            else
                            {
                                Android.Support.V7.App.AlertDialog.Builder completed = new Android.Support.V7.App.AlertDialog.Builder(act);
                                completed.SetTitle("WMS POD");
                                completed.SetMessage("Invoice not on Sales Order");
                                completed.SetPositiveButton("OK", (senderAlert, args) =>
                                {

                                });
                                Dialog completeddialog = completed.Create();
                                completeddialog.Show();

                                _adapter.NotifyDataSetChanged();
                                mActionText.Text = "";
                                progressDialog.Hide();
                            }
                        });


                        /*open camera screen*/
                    })).Start();
                };

            }
            public static Bitmap drawableToBitmap(Android.Graphics.Drawables.Drawable drawable)
            {
                Bitmap bitmap = null;

                if (drawable is BitmapDrawable)
                {
                    BitmapDrawable bitmapDrawable = (BitmapDrawable)drawable;
                    if (bitmapDrawable.Bitmap != null)
                    {
                        return bitmapDrawable.Bitmap;
                    }
                }

                if (drawable.IntrinsicWidth <= 0 || drawable.IntrinsicHeight <= 0)
                {
                    bitmap = Bitmap.CreateBitmap(1, 1, Bitmap.Config.Argb8888); // Single color bitmap will be created of 1x1 pixel
                }
                else
                {
                    bitmap = Bitmap.CreateBitmap(drawable.IntrinsicWidth, drawable.IntrinsicHeight, Bitmap.Config.Argb8888);
                }

                Canvas canvas = new Canvas(bitmap);
                drawable.SetBounds(0, 0, canvas.Width, canvas.Height);
                drawable.Draw(canvas);
                return bitmap;
            }

            public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
            {
                View view = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.wmsrolingitemlayout, parent, false);
                view.SetBackgroundResource(mBackground);

                return new WMSViewHolder(view);
            }

        }

        public class WMSViewHolder : RecyclerView.ViewHolder
        {
            public string mBoundString;
            public readonly View mView;
            public readonly TextView mActionName;
            public readonly EditText mActionText;
            public readonly TextView mActionDescription;
            public readonly ImageButton mbtnUp;
            //public readonly ImageButton btnCamera;
            public readonly ImageButton mbtnCheck;
            public readonly ImageButton mbtnDown;

            public WMSViewHolder(View view) : base(view)
            {
                mView = view;
                mActionName = view.FindViewById<TextView>(Resource.Id.ActionName);
                mActionText = view.FindViewById<EditText>(Resource.Id.ActionText);
                mActionDescription = view.FindViewById<TextView>(Resource.Id.ActionDescription);
                mbtnUp = view.FindViewById<ImageButton>(Resource.Id.btnUp);
               // btnCamera = view.FindViewById<ImageButton>(Resource.Id.btnCamera);
                mbtnCheck = view.FindViewById<ImageButton>(Resource.Id.btnCheck);
                mbtnDown = view.FindViewById<ImageButton>(Resource.Id.btnDown);

            }

            public override string ToString()
            {
                return base.ToString() + " '" + mActionName.Text;
            }
        }
    }


    //tring name, string description, Type itemType, bool readOnly, string suggestedItemName, string suggestedItemValue, bool isPassword
    public class WMSControlIem
    {
        public int Idx { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Type ItemType { get; set; }
        public string SuggestedItemName { get; set; }
        public string SuggestedItemValue { get; set; }
        public bool IsReadOnly { get; set; }
        public bool IsPassword { get; set; }

        public string Value { get; set; }
    }
}
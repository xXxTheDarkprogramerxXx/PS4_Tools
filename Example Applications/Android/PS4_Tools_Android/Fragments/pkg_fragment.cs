using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Android.Support.V7.Widget;
using Android.Graphics.Drawables;
using DesignLibrary.Helpers;
using Android.Graphics;

namespace DesignLibrary_Tutorial.Fragments
{
    public class pkg_fragment : Fragment
    {
        #region << Vars >>

        public static SimpleStringRecyclerViewAdapter _adapter;//our adapter for the recycle view

        public static List<string> list;//custom list of objects we mainly only want this to load when the program starts 

        public static Context context;//Context if needed
        public static Activity act;// Activity if needed

        public static List<WMSScanner.TripDetail> tripobject = new List<WMSScanner.TripDetail>();//our list of objects

        #endregion << Vars >>

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            RecyclerView recyclerView = inflater.Inflate(Resource.Layout.Fragment1, container, false) as RecyclerView;
            context = this.Context;
            act = this.Activity;
            SetUpRecyclerView(recyclerView);

            return recyclerView;
        }

        /// <summary>
        /// Setup our custom recycler view
        /// </summary>
        /// <param name="recyclerView"></param>
        private void SetUpRecyclerView(RecyclerView recyclerView)
        {
            new System.Threading.Thread(new System.Threading.ThreadStart(delegate
            {
                act.RunOnUiThread(() =>
                {
                    WMSScanner.TripsDetails tripresult = new WMSScanner.TripsDetails();
                    if (tripresult.Successful == false)
                    {
                        //error out 
                    }
                    tripobject.Clear();
                    var temp = new WMSScanner.TripDetail();
                    temp._invoice = "35088 | 2019-02-13 | Cranswick Supplier";
                    tripobject.Add(temp);

                    temp = new WMSScanner.TripDetail();
                    temp._invoice = "35089 | 2019-02-13 | Cranswick Supplier";
                    tripobject.Add(temp);

                    temp = new WMSScanner.TripDetail();
                    temp._invoice = "35090 | 2019-02-13 | Cranswick Supplier";
                    tripobject.Add(temp);
                    //tripobject.AddRange(tripresult.trips);
                    if (tripobject.Count == 0)
                    {
                        Toast.MakeText(context, "No Trips Loaded For Operator", ToastLength.Long).Show();
                    }
                    else
                    {
                        var values = tripobject;
                        recyclerView.SetLayoutManager(new LinearLayoutManager(recyclerView.Context));
                        _adapter = new SimpleStringRecyclerViewAdapter(recyclerView.Context, values, Activity.Resources);
                        recyclerView.SetAdapter(_adapter);
                        recyclerView.SetItemClickListener((rv, position, view) =>
                        {
                            try
                            {
                                //start the signing screen
                                Context context = view.Context;
                                Intent intent = new Intent(context, typeof(TripScanner));
                                intent.PutExtra(TripScanner.EXTRA_NAME, values[position]._tripno);//pass information to the next screen
                                context.StartActivity(intent);
                            }
                            catch (System.Exception ex)
                            {

                            }
                        });
                    }
                });
            })).Start();
        }

        /// <summary>
        /// Custom Recyler View Adapter
        /// </summary>
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
                mFilter = new myFilter(this);
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
                simpleHolder.mTxtView.Text = mValues[position]._invoice;

                int drawableID = position;
                BitmapFactory.Options options = new BitmapFactory.Options();

                simpleHolder.mImageView.SetBackgroundDrawable(context.GetDrawable(Resource.Drawable.if_list_118647));

                //Picasso.With(Fragment1.context)
                //     .Load(mValues[position].Image)
                //     .Into(simpleHolder.mImageView);

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
    }
}
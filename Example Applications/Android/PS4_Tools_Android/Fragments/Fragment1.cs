using System;
using Android.App;
using Android.OS;
using Android.Support.V7.Widget;
using Android.Views;
using SupportFragment = Android.Support.V4.App.Fragment;
using System.Collections.Generic;
using DesignLibrary_Tutorial.Helpers;
using Android.Graphics;
using Android.Util;
using Android.Content;
using Android.Content.Res;
using Android.Widget;
using DesignLibrary.Helpers;
using Java.Lang;
using Android.Graphics.Drawables;
using Square.Picasso;
using Java.Net;
using System.IO;
using Xamarin.Controls;
using Plugin.FilePicker.Abstractions;
using Plugin.FilePicker;

namespace DesignLibrary_Tutorial.Fragments
{
    public class Fragment1 : SupportFragment
    {
        #region << Vars >>

        public static SimpleStringRecyclerViewAdapter _adapter;//our adapter for the recycle view

        public static List<string> list;//custom list of objects we mainly only want this to load when the program starts 

        public static Context context;//Context if needed
        public static Activity act;// Activity if needed

        public static List<WMSScanner.TripDetail> tripobject = new List<WMSScanner.TripDetail>();//our list of objects

        #endregion << Vars >>

        #region << Methods >>

        public static void ReleadData()
        {
            //WMSScanner.TripsDetails tripresult = Constants._service.GetRoutringDriver(Update_Variables.WMSGuid.ToString());
            //if (tripresult.Successful == false)
            //{
            //    //error out 
            //}
            //tripobject.Clear();
            //tripobject.AddRange(tripresult.trips);

            //_adapter.NotifyDataSetChanged();
        }

        /// <summary>
        /// Standard On Create Method
        /// </summary>
        /// <param name="savedInstanceState"></param>
        public override void OnCreate(Bundle savedInstanceState)
        {
            /*return base on create*/
            base.OnCreate(savedInstanceState);
            //create a separate fragment here if needed

        }

        /// <summary>
        /// On View Created Method
        /// </summary>
        /// <param name="inflater"></param>
        /// <param name="container"></param>
        /// <param name="savedInstanceState"></param>
        /// <returns></returns>
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            RecyclerView recyclerView = inflater.Inflate(Resource.Layout.Fragment1, container, false) as RecyclerView;
            context = this.Context;
            act = this.Activity;
            SetUpRecyclerView(recyclerView);
           
            return recyclerView;
        }

        #region << Recycle View >>

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
                    if(tripresult.Successful == false)
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

        /// <summary>
        /// Recycle View Holder
        /// </summary>
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

        #endregion << Recycle View >>

        //This is used for endless scroll not needed in this screen but keeping it here as a reference
        private List<string> GetRandomSubList(List<string> items, int amount)
        {
            list = new List<string>();
            Random random = new Random();
            while (list.Count < amount)
            {
                list.Add(items[random.Next(items.Count)]);
            }

            return list;
        }

        #region << Search Functionality >>
        class myFilter : Filter
        {
            RecyclerView.Adapter a;
            public myFilter(RecyclerView.Adapter adapter) : base()
            {
                a = adapter;
            }
            protected override Filter.FilterResults PerformFiltering(Java.Lang.ICharSequence constraint)
            {
                FilterResults results = new FilterResults();
                if (constraint != null)
                {
                    var searchFor = constraint.ToString();
                    Console.WriteLine("searchFor:" + searchFor);
                    var matchList = new List<string>();

                    if (list.Contains(searchFor))
                    {

                    }
                    Console.WriteLine("resultCount:" + matchList.Count);

                    Java.Lang.Object[] matchObjects;
                    matchObjects = new Java.Lang.Object[matchList.Count];
                    for (int i = 0; i < matchList.Count; i++)
                    {
                        matchObjects[i] = new Java.Lang.String(matchList[i]);
                    }

                    results.Values = matchObjects;
                    results.Count = matchList.Count;
                }
                return results;
            }
            protected override void PublishResults(Java.Lang.ICharSequence constraint, Filter.FilterResults results)
            {
                a.NotifyDataSetChanged();
            }
        }

        #endregion << Search Functionality >>

        #endregion << Methods >>
    }
}
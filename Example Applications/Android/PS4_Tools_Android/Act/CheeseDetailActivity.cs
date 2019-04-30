//using System.Linq;
//using System;
//using System.Collections.Generic;
//using System.ComponentModel;
//using System.Runtime.CompilerServices;

//using Android.App;
//using Android.OS;
//using SupportToolbar = Android.Support.V7.Widget.Toolbar;
//using Android.Support.V7.App;
//using Android.Support.V7.Widget;
//using Android.Support.V4.Widget;
//using Android.Support.Design.Widget;
//using System;
//using Android.Widget;
//using DesignLibrary_Tutorial.Helpers;
//using Android.Views;
//using Square.Picasso;
//using Android.Graphics;
//using Android.Util;
//using Android.Content;
//using DesignLibrary.Helpers;
//using System.Threading;

//namespace DesignLibrary_Tutorial
//{
//    [Activity(Label = "CheeseDetailActivity", Theme = "@style/Theme.DesignDemo")]
//    public class CheeseDetailActivity : AppCompatActivity
//    {
//        public const string EXTRA_NAME = "cheese_name";
//        public const string imgurl = "imgurl";
//        public const string IDX = "idx";
//        public const string SubCat_Idx = "idx";

//        CollapsingToolbarLayout collapsingToolBar;
//        SimpleStringRecyclerViewAdapter adapter;

//        public static int loadint = 0;
//        bool firtstload = true;

//        public static Context context;
//        public static int _IDX = 0;
//        public static int SubIDX = 0;


//        #region << Filter Settings >>

//        //this is the Distance in KM
//        public static int DistanceFilter = 25;

//        //Product Filter
//        public static string ProductFilter = "";

//        public static string RegionFilter = "";

//        #endregion << Filter Settings >>

//        protected override void OnCreate(Bundle savedInstanceState)
//        {
//            base.OnCreate(savedInstanceState);

//            SetContentView(Resource.Layout.Activity_Detail);

//            SupportToolbar toolBar = FindViewById<SupportToolbar>(Resource.Id.toolbar);
//            SetSupportActionBar(toolBar);
//            SupportActionBar.SetDisplayHomeAsUpEnabled(true);

//            string cheeseName = Intent.GetStringExtra(EXTRA_NAME);
//            collapsingToolBar = FindViewById<CollapsingToolbarLayout>(Resource.Id.collapsing_toolbar);
//            collapsingToolBar.Title = cheeseName;

//            LoadBackDrop();

//            FloatingActionButton fab = FindViewById<FloatingActionButton>(Resource.Id.Filter);
//            fab.Click += delegate
//            {
//                Filter_popup();
//            };
//            context = this;
//            string idx = Intent.GetStringExtra(IDX);
//            _IDX = Convert.ToInt32(idx);
//            SubIDX = Convert.ToInt32(Intent.GetStringExtra(SubCat_Idx));
//            RecyclerView recyclerView = (RecyclerView)FindViewById(Resource.Id.recyclerview);
//            SetUpRecyclerView(recyclerView);

//        }


//        #region << Popup's >>
//        private PopupWindow Filter_popup()
//        {
//            View lastview = null;

//            // Initialize a new instance of LayoutInflater service
//            LayoutInflater inflater = (LayoutInflater)this.GetSystemService(LayoutInflaterService);

//            // Inflate the custom layout/view
//            View customView = inflater.Inflate(Resource.Layout.Filter_Detail_Layout, null);


//            /*
//                   public PopupWindow (View contentView, int width, int height)
//                       Create a new non focusable popup window which can display the contentView.
//                       The dimension of the window must be passed to this constructor.

//                       The popup does not provide any background. This should be handled by
//                       the content view.

//                   Parameters
//                       contentView : the popup's content
//                       width : the popup's width
//                       height : the popup's height
//                       focusable : the popup's focability (use the keyboard)
//               */
//            // Initialize a new instance of popup window
//            PopupWindow mPopupWindow = new PopupWindow(
//                        customView,
//                        ViewGroup.LayoutParams.MatchParent,
//                        ViewGroup.LayoutParams.WrapContent,
//                        true
//                );

//            // Set an elevation value for popup window
//            // Call requires API level 21
//            if (Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
//            {
//                mPopupWindow.Elevation = 5.0f;
//            }
//            // Get a reference for the custom view close button
//            ImageButton closeButton = (ImageButton)customView.FindViewById(Resource.Id.ib_close);
//            closeButton.Click += (o, e) =>
//            {
//                mPopupWindow.Dismiss();
//            };

//            //reference the filter distance 
//            TextView txtDistance = customView.FindViewById<TextView>(Resource.Id.txtRadius);
//            txtDistance.Text = DistanceFilter.ToString();

//            // Get a reference for the custom view add button
//            FloatingActionButton AddButton = (FloatingActionButton)customView.FindViewById(Resource.Id.fab);

//            AddButton.Click += (o, e) =>
//            {
//                // here we filter
//                if (txtDistance.Text == "")
//                {
//                    DistanceFilter = 50;
//                }
//                else
//                {
//                    int.TryParse(txtDistance.Text, out DistanceFilter);
//                }

//                mPopupWindow.Dismiss();
//            };

//            // Closes the popup window when touch outside.
//            mPopupWindow.OutsideTouchable = true;
//            mPopupWindow.Focusable = true;

//            mPopupWindow.SetBackgroundDrawable(new Android.Graphics.Drawables.BitmapDrawable());
//            /*
//                    public void showAtLocation (View parent, int gravity, int x, int y)
//                        Display the content view in a popup window at the specified location. If the
//                        popup window cannot fit on screen, it will be clipped.
//                        Learn WindowManager.LayoutParams for more information on how gravity and the x
//                        and y parameters are related. Specifying a gravity of NO_GRAVITY is similar
//                        to specifying Gravity.LEFT | Gravity.TOP.

//                    Parameters
//                        parent : a parent view to get the getWindowToken() token from
//                        gravity : the gravity which controls the placement of the popup window
//                        x : the popup's x location offset
//                        y : the popup's y location offset
//                */
//            // Finally, show the popup window at the center location of root relative layout
//            mPopupWindow.ShowAtLocation(collapsingToolBar, GravityFlags.Center, 0, 0);

//            return mPopupWindow;

//        }


//        #endregion << Popup's >>

//        public override bool OnOptionsItemSelected(IMenuItem item)
//        {
//            switch (item.ItemId)
//            {
//                case Android.Resource.Id.Home:
//                    Finish();
//                    return true;
//            }

//            return base.OnOptionsItemSelected(item);
//        }

//        public override bool OnCreateOptionsMenu(IMenu menu)
//        {
//            MenuInflater.Inflate(Resource.Menu.sample_actions, menu);
//            return true;
//        }

//        private void LoadBackDrop()
//        {
//            ImageView imageView = FindViewById<ImageView>(Resource.Id.backdrop);

//            Picasso.With(this)
//                    .Load(Intent.GetStringExtra(imgurl))
//                    .Into(imageView);
//        }



//        private void SetUpRecyclerView(RecyclerView recyclerView)
//        {
//            /* generate list with app info */
            
//            recyclerView.HasFixedSize = true;

//            LinearLayoutManager layoutmanager = new LinearLayoutManager(recyclerView.Context);
//            recyclerView.SetLayoutManager(layoutmanager);

//            /*retrieve list from the service*/
//            SpecialService.ListProductResult result = Constants._service.RetrieveList(loadint, true, _IDX, true, SubIDX, true, DistanceFilter, true);

//            IList<SpecialService.Product_Object> products = result.ResultList; // SQL_Product.RetrieveList(loadint, _IDX, SubIDX, DistanceFilter);


//            adapter = new SimpleStringRecyclerViewAdapter(recyclerView, products);
//            recyclerView.SetAdapter(adapter);



//            recyclerView.SetItemClickListener((rv, position, view) =>
//            {
//                string test = position.ToString();

//                /* now we view an indiviual item */

//                //view.LongClick += View_LongClick;
//                //view.Click += View_Click;
//                Context context = view.Context;
//                Intent intent = new Intent(context, typeof(ProductDetailActivity));
//                intent.PutExtra(ProductDetailActivity.EXTRA_NAME, products[position].ProductName);
//                intent.PutExtra(ProductDetailActivity.IDX, products[position].Idx);
//                intent.PutExtra(ProductDetailActivity.imgurl, products[position].ProductImage);
//                intent.PutExtra(ProductDetailActivity.CatIdx, products[position].Category);
//                intent.PutExtra(ProductDetailActivity.SubCat_Idx, products[position].SubCategory);
//                intent.AddFlags(ActivityFlags.NewTask);

//                context.StartActivity(intent);

//            });

           

//        }

//        //public class SimpleStringRecyclerViewAdapter : InfiniteScrollRecyclerViewAdapter<SpecialService.Product_Object>
//        //{
//        //    private readonly TypedValue mTypedValue = new TypedValue();
//        //    private int mBackground;
//        //    private IList<SpecialService.Product_Object> mValues;

//        //    //public SimpleStringRecyclerViewAdapter(Context context, List<Product_Object> items)
//        //    //{
//        //    //    context.Theme.ResolveAttribute(Resource.Attribute.selectableItemBackground, mTypedValue, true);
//        //    //    mBackground = mTypedValue.ResourceId;
//        //    //    mValues = items;
//        //    //}

//        //    public SimpleStringRecyclerViewAdapter(RecyclerView recyclerView,IList<SpecialService.Product_Object> items) : base(recyclerView, items)
//        //    {
//        //        context.Theme.ResolveAttribute(Resource.Attribute.selectableItemBackground, mTypedValue, true);
//        //        mBackground = mTypedValue.ResourceId;
//        //        mValues = items;
//        //    }

//        //    public override void DoBindViewHolder(RecyclerView.ViewHolder holder, int position)
//        //    {
//        //        var simpleHolder = holder as SimpleViewHolder;
//        //        if (mValues != null)
//        //        {

//        //            simpleHolder.mBoundString = mValues[position].ProductName;
//        //            simpleHolder.mTxttitle.Text = mValues[position].ProductName;
//        //            simpleHolder.mTxtDescription.Text = mValues[position].Description;
//        //            simpleHolder.mtxtPrice.Text = "R" + mValues[position].Price.ToString();
//        //            simpleHolder.mTxtExpire.Text = mValues[position].ExpiryDate.ToShortDateString();

//        //            //calc discount
//        //            decimal discount = 100 -((mValues[position].Price * 100) / mValues[position].Discount);
//        //            simpleHolder.mTxtDiscount.Text = Math.Round(discount, 1).ToString() + "%";
//        //            if (mValues[position].Distance != 0)
//        //            {
//        //                simpleHolder.mTxtDistance.Text = mValues[position].Distance.ToString() + "KM";
//        //            }

//        //           int drawableID = position;
//        //            BitmapFactory.Options options = new BitmapFactory.Options();


//        //            Picasso.With(CheeseDetailActivity.context)
//        //                .Load(mValues[position].ProductImage)
//        //                .Into(simpleHolder.mImageView);


//        //        }
//        //    }

//        //    public override RecyclerView.ViewHolder GetViewHolder(ViewGroup parent, int viewType)
//        //    {
//        //        View view = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.List_CarView, parent, false);
//        //        view.SetBackgroundResource(mBackground);

//        //        return new SimpleViewHolder(view);
//        //    }

//        //    protected override int ItemViewType(int position)
//        //    {
//        //        return 0;
//        //    }

//        //    protected override IEnumerable<SpecialService.Product_Object> LoadMoreItems(int numberOfExistingItems)
//        //    {
//        //        //Thread.Sleep(3000); // simulate a long running process for loading new data

//        //        loadint = loadint + 10;

//        //        SpecialService.ListProductResult result = Constants._service.RetrieveList(loadint, true, _IDX, true, SubIDX, true, DistanceFilter, true);

//        //        IList<SpecialService.Product_Object> mlist = result.ResultList;

//        //        foreach (var item in mlist)
//        //        {
//        //            mValues.Add(item);
//        //        }
//        //        return mlist; // add 10 numbers
//        //    }
//        //}

//        public class SimpleViewHolder : RecyclerView.ViewHolder
//        {
//            public string mBoundString;
//            public readonly View mView;
//            public readonly ImageView mImageView;
//            public readonly TextView mTxttitle;
//            public readonly TextView mTxtDescription;
//            public readonly TextView mtxtPrice;
//            public readonly TextView mTxtExpire;
//            public readonly TextView mTxtDistance;
//            public readonly TextView mTxtDiscount;

//            public SimpleViewHolder(View view) : base(view)
//            {
//                mView = view;
//                mImageView = view.FindViewById<ImageView>(Resource.Id.imgvPro);
//                mTxttitle = view.FindViewById<TextView>(Resource.Id.txtTitle);
//                mTxtDescription = view.FindViewById<TextView>(Resource.Id.txtDescription);
//                mtxtPrice = view.FindViewById<TextView>(Resource.Id.txtPrice);
//                mTxtExpire = view.FindViewById<TextView>(Resource.Id.txtExpire);
//                mTxtDistance = view.FindViewById<TextView>(Resource.Id.txtDistance);
//                mTxtDiscount = view.FindViewById<TextView>(Resource.Id.txtDiscount);
//            }

//            public override string ToString()
//            {
//                return base.ToString() + " '" + mTxttitle.Text;
//            }
//        }


//    }
//}
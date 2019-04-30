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

namespace DesignLibrary_Tutorial
{
    [Activity(Label = "CategoryTypeView", Theme = "@style/Theme.DesignDemo")]
    public class CategoryTypeView : AppCompatActivity
    {
        public const string EXTRA_NAME = "cheese_name";
        public const string imgurl = "imgurl";
        public const string IDX = "idx";

        CollapsingToolbarLayout collapsingToolBar;
        SimpleStringRecyclerViewAdapter adapter;

        public static int loadint = 0;
        bool firtstload = true;

        public static Context context;
        public static int _IDX = 0;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.Activity_Detail);

            SupportToolbar toolBar = FindViewById<SupportToolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolBar);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);

            string cheeseName = Intent.GetStringExtra(EXTRA_NAME);
            collapsingToolBar = FindViewById<CollapsingToolbarLayout>(Resource.Id.collapsing_toolbar);
            collapsingToolBar.Title = cheeseName;

            LoadBackDrop();

            FloatingActionButton fab = FindViewById<FloatingActionButton>(Resource.Id.Filter);
            fab.Click += delegate
            {
                Filter_popup();
            };
            context = this;
            _IDX = Convert.ToInt32(Intent.GetStringExtra(IDX));
            RecyclerView recyclerView = (RecyclerView)FindViewById(Resource.Id.recyclerview);
            SetUpRecyclerView(recyclerView);

        }


        #region << Popup's >>
        private PopupWindow Filter_popup()
        {
            View lastview = null;

            // Initialize a new instance of LayoutInflater service
            LayoutInflater inflater = (LayoutInflater)this.GetSystemService(LayoutInflaterService);

            // Inflate the custom layout/view
            View customView = inflater.Inflate(Resource.Layout.Filter_Detail_Layout, null);


            /*
                   public PopupWindow (View contentView, int width, int height)
                       Create a new non focusable popup window which can display the contentView.
                       The dimension of the window must be passed to this constructor.

                       The popup does not provide any background. This should be handled by
                       the content view.

                   Parameters
                       contentView : the popup's content
                       width : the popup's width
                       height : the popup's height
                       focusable : the popup's focability (use the keyboard)
               */
            // Initialize a new instance of popup window
            PopupWindow mPopupWindow = new PopupWindow(
                        customView,
                        ViewGroup.LayoutParams.MatchParent,
                        ViewGroup.LayoutParams.WrapContent,
                        true
                );

            // Set an elevation value for popup window
            // Call requires API level 21
            if (Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
            {
                mPopupWindow.Elevation = 5.0f;
            }
            // Get a reference for the custom view close button
            ImageButton closeButton = (ImageButton)customView.FindViewById(Resource.Id.ib_close);
            closeButton.Click += (o, e) =>
            {
                mPopupWindow.Dismiss();
            };

            // Get a reference for the custom view add button
            FloatingActionButton AddButton = (FloatingActionButton)customView.FindViewById(Resource.Id.fab);

            AddButton.Click += (o, e) =>
            {
                // here we filter

                mPopupWindow.Dismiss();
            };

            // Closes the popup window when touch outside.
            mPopupWindow.OutsideTouchable = true;
            mPopupWindow.Focusable = true;

            mPopupWindow.SetBackgroundDrawable(new Android.Graphics.Drawables.BitmapDrawable());
            /*
                    public void showAtLocation (View parent, int gravity, int x, int y)
                        Display the content view in a popup window at the specified location. If the
                        popup window cannot fit on screen, it will be clipped.
                        Learn WindowManager.LayoutParams for more information on how gravity and the x
                        and y parameters are related. Specifying a gravity of NO_GRAVITY is similar
                        to specifying Gravity.LEFT | Gravity.TOP.

                    Parameters
                        parent : a parent view to get the getWindowToken() token from
                        gravity : the gravity which controls the placement of the popup window
                        x : the popup's x location offset
                        y : the popup's y location offset
                */
            // Finally, show the popup window at the center location of root relative layout
            mPopupWindow.ShowAtLocation(collapsingToolBar, GravityFlags.Center, 0, 0);

            return mPopupWindow;

        }


        #endregion << Popup's >>

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

            Picasso.With(this)
                    .Load(Intent.GetStringExtra(imgurl))
                    .Into(imageView);
        }



        private void SetUpRecyclerView(RecyclerView recyclerView)
        {
            /* generate list with app info */

            recyclerView.HasFixedSize = true;

            LinearLayoutManager layoutmanager = new LinearLayoutManager(recyclerView.Context);
            recyclerView.SetLayoutManager(layoutmanager);

            
            adapter = new SimpleStringRecyclerViewAdapter(recyclerView, SQL_Product.RetrieveList(loadint, _IDX));
            recyclerView.SetAdapter(adapter);



            recyclerView.SetItemClickListener((rv, position, view) =>
            {
                string test = position.ToString();

                //view.LongClick += View_LongClick;
                //view.Click += View_Click;
                //Context context = view.Context;
                //Intent intent = new Intent(context, typeof(CheeseDetailActivity));
                //intent.PutExtra(CheeseDetailActivity.EXTRA_NAME, values[position].ApplicationName);
                //intent.PutExtra("data_used", data[position].ApplicationData);
                //intent.PutExtra("user_id", position);
                //intent.PutExtra("pid", pid[position].ApplicationPID);

                //intent.AddFlags(ActivityFlags.NewTask);

                //context.StartActivity(intent);

            });

           

        }

        public class SimpleStringRecyclerViewAdapter : RecyclerView.Adapter
        {
            private readonly TypedValue mTypedValue = new TypedValue();
            private int mBackground;
            private List<string> mValues;
            private List<string> mDistance;
            List<Android.Graphics.Drawables.Drawable> mResource;
            private Dictionary<int, int> mCalculatedSizes;

            public SimpleStringRecyclerViewAdapter(Context context, List<string> items, List<Android.Graphics.Drawables.Drawable> res, List<string> Distance)
            {
                context.Theme.ResolveAttribute(Resource.Attribute.selectableItemBackground, mTypedValue, true);
                mBackground = mTypedValue.ResourceId;
                mValues = items;
                mResource = res;
                mDistance = Distance;
                mCalculatedSizes = new Dictionary<int, int>();
            }

            public override int ItemCount
            {
                get
                {
                    return mValues.Count;
                }
            }

            public override async void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
            {
                var simpleHolder = holder as SimpleViewHolder;

                simpleHolder.mBoundString = mValues[position];
                simpleHolder.mTxtView.Text = mValues[position];
                simpleHolder.mDistance.Text = mDistance[position];

                int drawableID = position;
                BitmapFactory.Options options = new BitmapFactory.Options();

                Picasso.With(CategoryTypeView.context)
                         .Load(mValues[position].ProductImage)
                         .Into(simpleHolder.mImageView);
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
            public Bitmap convertToBitmap(Android.Graphics.Drawables.Drawable drawable, int widthPixels, int heightPixels)
            {
                Bitmap mutableBitmap = Bitmap.CreateBitmap(widthPixels, heightPixels, Bitmap.Config.Argb8888);
                Canvas canvas = new Canvas(mutableBitmap);
                drawable.SetBounds(0, 0, widthPixels, heightPixels);
                drawable.Draw(canvas);

                return mutableBitmap;
            }
        }

        public class SimpleViewHolder : RecyclerView.ViewHolder
        {
            public string mBoundString;
            public readonly View mView;
            public readonly ImageView mImageView;
            public readonly TextView mTxtView;
            public readonly TextView mDistance;

            public SimpleViewHolder(View view) : base(view)
            {
                mView = view;
                mImageView = view.FindViewById<ImageView>(Resource.Id.avatar);
                mTxtView = view.FindViewById<TextView>(Resource.Id.text1);
            }

            public override string ToString()
            {
                return base.ToString() + " '" + mTxtView.Text;
            }
        }
    }
}
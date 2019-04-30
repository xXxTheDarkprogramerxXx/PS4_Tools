using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Support.V7.Widget;
using Android.Content.Res;
using Android.Util;
using Android.Graphics;

namespace DesignLibrary_Tutorial
{
    public class NotificationAdapter : RecyclerView.Adapter, IFilterable
    {

        private readonly TypedValue mTypedValue = new TypedValue();
        private int mBackground;
        private List<Notifications> mValues;
        Resources mResource;
        private Dictionary<int, int> mCalculatedSizes;
        private Filter mFilter;

        public NotificationAdapter(Context context, List<Notifications> items, Resources res)
        {
            context.Theme.ResolveAttribute(Resource.Attribute.selectableItemBackground, mTypedValue, true);
            mBackground = mTypedValue.ResourceId;
            mValues = items;
            mResource = res;
            //mFilter = new myFilter(this);
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
            var simpleHolder = holder as NotificationAdapterViewHolder;

            simpleHolder.mBoundString = mValues[position].Title;
            simpleHolder.mTitel.Text = mValues[position].Title;
            if (mValues[position].Description == null)
            {
                simpleHolder.mDescription.Text = "";
            }
            else
            {
                simpleHolder.mDescription.Text = mValues[position].Description;
            }
            if (mValues[position].Date != new DateTime())
            {
                simpleHolder.mDateTime.Text = mValues[position].Date.ToShortDateString();
            }
            else
            {
                simpleHolder.mDateTime.Visibility = ViewStates.Gone;
            }

            int drawableID = position;
            BitmapFactory.Options options = new BitmapFactory.Options();

            //simpleHolder.mImageView.SetBackgroundDrawable(mValues[position].Image);
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View view = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.Notification_Item, parent, false);
            view.SetBackgroundResource(mBackground);

            return new NotificationAdapterViewHolder(view);
        }

    }

    public class NotificationAdapterViewHolder : RecyclerView.ViewHolder
    {
        public string mBoundString;
        public readonly View mView;
        public readonly TextView mTitel;
        public readonly TextView mDescription;
        public readonly TextView mDateTime;

        public NotificationAdapterViewHolder(View view) : base(view)
        {
            mView = view;
            mTitel = view.FindViewById<TextView>(Resource.Id.txtTitle);
            mDescription = view.FindViewById<TextView>(Resource.Id.txtDescription);
            mDateTime = view.FindViewById<TextView>(Resource.Id.txtDate);
        }

        public override string ToString()
        {
            return base.ToString() + " '" + mTitel.Text;
        }
    }


    //class NotificationAdapterViewHolder : Java.Lang.Object
    //{
    //    //Your adapter views to re-use
    //    //public TextView Title { get; set; }
    //}
}
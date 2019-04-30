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

namespace DesignLibrary_Tutorial.adapter
{
    public class GoogleCardsShopAdapter : IFilterable
    {

        //    private LayoutInflater mInflater;

        //    List<string> lst = new List<string>();

        //    public GoogleCardsShopAdapter(Context context, List<DummyModel> items)
        //    {
        //        mInflater = (LayoutInflater)context
        //                .GetSystemService(Context.LayoutInflaterService);
        //    }


        //public override long GetItemId(int position)
        //    {
        //        return GetItem(position).getId();
        //    }


        //public override View GetView(int position, View convertView, ViewGroup parent)
        //    {
        //        ViewHolder holder;
        //        if (convertView == null)
        //        {
        //            convertView = mInflater.Inflate(
        //                    R.layout.list_item_google_cards_shop, parent, false);
        //            holder = new ViewHolder();
        //            holder.image = (ImageView)convertView
        //                    .findViewById(R.id.list_item_google_cards_shop_image);
        //            holder.promo = (TextView)convertView
        //                    .findViewById(R.id.list_item_google_cards_shop_promo);
        //            holder.discount = (TextView)convertView
        //                    .findViewById(R.id.list_item_google_cards_shop_discount);
        //            holder.price = (TextView)convertView
        //                    .findViewById(R.id.list_item_google_cards_shop_price);
        //            holder.description = (TextView)convertView
        //                    .findViewById(R.id.list_item_google_cards_shop_description);
        //            holder.day = (TextView)convertView
        //                    .findViewById(R.id.list_item_google_cards_shop_day);
        //            holder.buy = (TextView)convertView
        //                    .findViewById(R.id.list_item_google_cards_shop_buy);
        //            holder.buy.setOnClickListener(this);
        //            convertView.setTag(holder);
        //        }
        //        else
        //        {
        //            holder = (ViewHolder)convertView.GetTag();
        //        }

        //        holder.buy.SetTag(position);
        //        DummyModel item = GetItem(position);
        //        ImageUtil.displayImage(holder.image, item.getImageURL(), null);

        //        convertView.Click += delegate
        //        {
        //            // TODO Auto-generated method stub
        //            int possition = (int)convertView.GetTag();
        //            switch (convertView.getId())
        //            {
        //                case R.id.list_item_google_cards_shop_buy:
        //                    // click on explore button
        //                    Toast.MakeText(getContext(), "Buy: ", Toast.LENGTH_SHORT).show();
        //                    break;
        //            }
        //        };


        //        return convertView;
        //    }

        //    public class ViewHolder
        //    {
        //        public ImageView image;
        //        public TextView promo;
        //        public TextView discount;
        //        public TextView price;
        //        public TextView description;
        //        public TextView day;
        //        public TextView buy;
        //    }
        public Filter Filter
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public IntPtr Handle
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
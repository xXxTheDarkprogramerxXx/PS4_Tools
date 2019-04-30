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

namespace DesignLibrary_Tutorial
{
    class MessageBox
    {
        public PopupWindow OK(View parent, Context context, string Message, string Title, MessageBoxButton button)
        {
            View lastview = null;

            // Initialize a new instance of LayoutInflater service
            LayoutInflater inflater = (LayoutInflater)context.GetSystemService("layout_inflater");

            // Inflate the custom layout/view
            View customView = inflater.Inflate(Resource.Layout.MessageBox_Layout, null);

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
            mPopupWindow.ShowAtLocation(parent, GravityFlags.Center, 0, 0);

            return mPopupWindow;
        }
    }

    public enum MessageBoxResult
    {
        /// <summary>The message box returns no result.</summary>
        None,
        /// <summary>The result value of the message box is OK.</summary>
        OK,
        /// <summary>The result value of the message box is Cancel.</summary>
        Cancel,
        /// <summary>The result value of the message box is Yes.</summary>
        Yes = 6,
        /// <summary>The result value of the message box is No.</summary>
        No
    }
    public enum MessageBoxButton
    {
        /// <summary>The message box displays an OK button.</summary>
        OK,
        /// <summary>The message box displays OK and Cancel buttons.</summary>
        OKCancel,
        /// <summary>The message box displays Yes, No, and Cancel buttons.</summary>
        YesNoCancel = 3,
        /// <summary>The message box displays Yes and No buttons.</summary>
        YesNo
    }
    
}
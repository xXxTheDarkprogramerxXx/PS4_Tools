package md5f770f24a9c39f29cf9dfe9ce0599cae5;


public class XamarinRecyclerViewOnScrollListener
	extends android.support.v7.widget.RecyclerView.OnScrollListener
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_onScrolled:(Landroid/support/v7/widget/RecyclerView;II)V:GetOnScrolled_Landroid_support_v7_widget_RecyclerView_IIHandler\n" +
			"";
		mono.android.Runtime.register ("DesignLibrary.Helpers.XamarinRecyclerViewOnScrollListener, com.jem.onspecial, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", XamarinRecyclerViewOnScrollListener.class, __md_methods);
	}


	public XamarinRecyclerViewOnScrollListener () throws java.lang.Throwable
	{
		super ();
		if (getClass () == XamarinRecyclerViewOnScrollListener.class)
			mono.android.TypeManager.Activate ("DesignLibrary.Helpers.XamarinRecyclerViewOnScrollListener, com.jem.onspecial, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "", this, new java.lang.Object[] {  });
	}

	public XamarinRecyclerViewOnScrollListener (android.support.v7.widget.LinearLayoutManager p0) throws java.lang.Throwable
	{
		super ();
		if (getClass () == XamarinRecyclerViewOnScrollListener.class)
			mono.android.TypeManager.Activate ("DesignLibrary.Helpers.XamarinRecyclerViewOnScrollListener, com.jem.onspecial, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "Android.Support.V7.Widget.LinearLayoutManager, Xamarin.Android.Support.v7.RecyclerView, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", this, new java.lang.Object[] { p0 });
	}


	public void onScrolled (android.support.v7.widget.RecyclerView p0, int p1, int p2)
	{
		n_onScrolled (p0, p1, p2);
	}

	private native void n_onScrolled (android.support.v7.widget.RecyclerView p0, int p1, int p2);

	private java.util.ArrayList refList;
	public void monodroidAddReference (java.lang.Object obj)
	{
		if (refList == null)
			refList = new java.util.ArrayList ();
		refList.add (obj);
	}

	public void monodroidClearReferences ()
	{
		if (refList != null)
			refList.clear ();
	}
}

package md573e1bf0a11ae2162e0a84e8c85028a5b;


public class GoogleCardsShopAdapter
	extends java.lang.Object
	implements
		mono.android.IGCUserPeer,
		android.widget.Filterable
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_getFilter:()Landroid/widget/Filter;:GetGetFilterHandler:Android.Widget.IFilterableInvoker, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\n" +
			"";
		mono.android.Runtime.register ("DesignLibrary_Tutorial.adapter.GoogleCardsShopAdapter, com.jem.onspecial, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", GoogleCardsShopAdapter.class, __md_methods);
	}


	public GoogleCardsShopAdapter ()
	{
		super ();
		if (getClass () == GoogleCardsShopAdapter.class)
			mono.android.TypeManager.Activate ("DesignLibrary_Tutorial.adapter.GoogleCardsShopAdapter, com.jem.onspecial, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "", this, new java.lang.Object[] {  });
	}


	public android.widget.Filter getFilter ()
	{
		return n_getFilter ();
	}

	private native android.widget.Filter n_getFilter ();

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

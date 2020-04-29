package md5d69364805abe5678acfb684a204c090f;


public class DefaultViewHolder
	extends android.support.v7.widget.RecyclerView.ViewHolder
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"";
		mono.android.Runtime.register ("DesignLibrary_Tutorial.DefaultViewHolder, com.jem.onspecial, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", DefaultViewHolder.class, __md_methods);
	}


	public DefaultViewHolder (android.view.View p0)
	{
		super (p0);
		if (getClass () == DefaultViewHolder.class)
			mono.android.TypeManager.Activate ("DesignLibrary_Tutorial.DefaultViewHolder, com.jem.onspecial, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "Android.Views.View, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=84e04ff9cfb79065", this, new java.lang.Object[] { p0 });
	}

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

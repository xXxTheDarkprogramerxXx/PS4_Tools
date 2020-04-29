package md52302a138a62f6e923224a052e83b1dc5;


public class PupFragment_myFilter
	extends android.widget.Filter
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_performFiltering:(Ljava/lang/CharSequence;)Landroid/widget/Filter$FilterResults;:GetPerformFiltering_Ljava_lang_CharSequence_Handler\n" +
			"n_publishResults:(Ljava/lang/CharSequence;Landroid/widget/Filter$FilterResults;)V:GetPublishResults_Ljava_lang_CharSequence_Landroid_widget_Filter_FilterResults_Handler\n" +
			"";
		mono.android.Runtime.register ("DesignLibrary_Tutorial.Fragments.PupFragment+myFilter, com.jem.onspecial, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", PupFragment_myFilter.class, __md_methods);
	}


	public PupFragment_myFilter ()
	{
		super ();
		if (getClass () == PupFragment_myFilter.class)
			mono.android.TypeManager.Activate ("DesignLibrary_Tutorial.Fragments.PupFragment+myFilter, com.jem.onspecial, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "", this, new java.lang.Object[] {  });
	}

	public PupFragment_myFilter (android.support.v7.widget.RecyclerView.Adapter p0)
	{
		super ();
		if (getClass () == PupFragment_myFilter.class)
			mono.android.TypeManager.Activate ("DesignLibrary_Tutorial.Fragments.PupFragment+myFilter, com.jem.onspecial, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "Android.Support.V7.Widget.RecyclerView+Adapter, Xamarin.Android.Support.v7.RecyclerView, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", this, new java.lang.Object[] { p0 });
	}


	public android.widget.Filter.FilterResults performFiltering (java.lang.CharSequence p0)
	{
		return n_performFiltering (p0);
	}

	private native android.widget.Filter.FilterResults n_performFiltering (java.lang.CharSequence p0);


	public void publishResults (java.lang.CharSequence p0, android.widget.Filter.FilterResults p1)
	{
		n_publishResults (p0, p1);
	}

	private native void n_publishResults (java.lang.CharSequence p0, android.widget.Filter.FilterResults p1);

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

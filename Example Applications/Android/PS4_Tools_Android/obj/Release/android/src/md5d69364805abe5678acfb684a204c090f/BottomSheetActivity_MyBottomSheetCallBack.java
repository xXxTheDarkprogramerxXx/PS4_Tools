package md5d69364805abe5678acfb684a204c090f;


public class BottomSheetActivity_MyBottomSheetCallBack
	extends android.support.design.widget.BottomSheetBehavior.BottomSheetCallback
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_onSlide:(Landroid/view/View;F)V:GetOnSlide_Landroid_view_View_FHandler\n" +
			"n_onStateChanged:(Landroid/view/View;I)V:GetOnStateChanged_Landroid_view_View_IHandler\n" +
			"";
		mono.android.Runtime.register ("DesignLibrary_Tutorial.BottomSheetActivity+MyBottomSheetCallBack, com.jem.onspecial, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", BottomSheetActivity_MyBottomSheetCallBack.class, __md_methods);
	}


	public BottomSheetActivity_MyBottomSheetCallBack () throws java.lang.Throwable
	{
		super ();
		if (getClass () == BottomSheetActivity_MyBottomSheetCallBack.class)
			mono.android.TypeManager.Activate ("DesignLibrary_Tutorial.BottomSheetActivity+MyBottomSheetCallBack, com.jem.onspecial, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "", this, new java.lang.Object[] {  });
	}


	public void onSlide (android.view.View p0, float p1)
	{
		n_onSlide (p0, p1);
	}

	private native void n_onSlide (android.view.View p0, float p1);


	public void onStateChanged (android.view.View p0, int p1)
	{
		n_onStateChanged (p0, p1);
	}

	private native void n_onStateChanged (android.view.View p0, int p1);

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

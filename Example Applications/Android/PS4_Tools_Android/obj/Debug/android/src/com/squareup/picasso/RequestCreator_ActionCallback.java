package com.squareup.picasso;


public class RequestCreator_ActionCallback
	extends java.lang.Object
	implements
		mono.android.IGCUserPeer,
		com.squareup.picasso.Callback
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_onError:()V:GetOnErrorHandler:Square.Picasso.ICallbackInvoker, Square.Picasso\n" +
			"n_onSuccess:()V:GetOnSuccessHandler:Square.Picasso.ICallbackInvoker, Square.Picasso\n" +
			"";
		mono.android.Runtime.register ("Square.Picasso.RequestCreator+ActionCallback, Square.Picasso, Version=2.5.2.0, Culture=neutral, PublicKeyToken=null", RequestCreator_ActionCallback.class, __md_methods);
	}


	public RequestCreator_ActionCallback () throws java.lang.Throwable
	{
		super ();
		if (getClass () == RequestCreator_ActionCallback.class)
			mono.android.TypeManager.Activate ("Square.Picasso.RequestCreator+ActionCallback, Square.Picasso, Version=2.5.2.0, Culture=neutral, PublicKeyToken=null", "", this, new java.lang.Object[] {  });
	}


	public void onError ()
	{
		n_onError ();
	}

	private native void n_onError ();


	public void onSuccess ()
	{
		n_onSuccess ();
	}

	private native void n_onSuccess ();

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

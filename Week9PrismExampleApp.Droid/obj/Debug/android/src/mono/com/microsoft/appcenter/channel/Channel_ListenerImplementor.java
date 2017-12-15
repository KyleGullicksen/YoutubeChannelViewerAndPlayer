package mono.com.microsoft.appcenter.channel;


public class Channel_ListenerImplementor
	extends java.lang.Object
	implements
		mono.android.IGCUserPeer,
		com.microsoft.appcenter.channel.Channel.Listener
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_onEnqueuingLog:(Lcom/microsoft/appcenter/ingestion/models/Log;Ljava/lang/String;)V:GetOnEnqueuingLog_Lcom_microsoft_appcenter_ingestion_models_Log_Ljava_lang_String_Handler:Com.Microsoft.Appcenter.Channel.IChannelListenerInvoker, Microsoft.AppCenter.Android.Bindings\n" +
			"";
		mono.android.Runtime.register ("Com.Microsoft.Appcenter.Channel.IChannelListenerImplementor, Microsoft.AppCenter.Android.Bindings, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null", Channel_ListenerImplementor.class, __md_methods);
	}


	public Channel_ListenerImplementor () throws java.lang.Throwable
	{
		super ();
		if (getClass () == Channel_ListenerImplementor.class)
			mono.android.TypeManager.Activate ("Com.Microsoft.Appcenter.Channel.IChannelListenerImplementor, Microsoft.AppCenter.Android.Bindings, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null", "", this, new java.lang.Object[] {  });
	}


	public void onEnqueuingLog (com.microsoft.appcenter.ingestion.models.Log p0, java.lang.String p1)
	{
		n_onEnqueuingLog (p0, p1);
	}

	private native void n_onEnqueuingLog (com.microsoft.appcenter.ingestion.models.Log p0, java.lang.String p1);

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

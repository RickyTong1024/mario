using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;

public class mario_tool : MonoBehaviour {

	[DllImport("__Internal")]
	private static extern void startMTA ();
	[DllImport("__Internal")]
	private static extern void onJewelGet(string guid, string scene, string num);
	[DllImport("__Internal")]
	private static extern void onJewelConsume(string guid, string scene, string num);
	[DllImport("__Internal")]
	private static extern void onUserDo(string guid, string scene, string num);
	[DllImport("__Internal")]
	private static extern void onRaid(string guid, string scene, string num);
	[DllImport("__Internal")]
	private static extern void createNotify(string text, int secondsFromNow);
	[DllImport("__Internal")]
	private static extern void cancelNotify();
	[DllImport("__Internal")]
	private static extern void init_tool();
	[DllImport("__Internal")]
	private static extern string _getCountryCode();
	[DllImport("__Internal")]
	private static extern void _hfad();
	[DllImport("__Internal")]
	private static extern void _close_hfad();
	[DllImport("__Internal")]
	private static extern void _cyad();

	public static mario_tool _instance;

	private string[] m_titles = {"mario_tool_xdgq", "mario_tool_tlhf"};
	private string[] m_texts = {"mario_tool_ywjz", "mario_tool_ndtl"};
	private int m_ad_num = 0;
	private int m_ad_tnum = 5;
	
	void Awake()
	{
		_instance = this;
	}

	void Start () {

		if(Application.isEditor)
		{
			return;
		}
		#if UNITY_ANDROID
		#elif UNITY_IPHONE
		startMTA ();
		init_tool();
		#endif
		
		cancel_notify ();
		int _second = timer.dtnow().Second;
		int _minute = timer.dtnow().Minute;
		int _hour = timer.dtnow().Hour;
		if (_hour < 12)
		{
			int time = (12 - _hour) * 60 - _minute;
			time = time * 60 - _second;
			int index = Random.Range(0, 2);
			create_notify(game_data._instance.get_language_string(m_titles[index]), game_data._instance.get_language_string(m_texts[index]), game_data._instance.get_language_string(m_texts[index]), time);
		}
		if (_hour < 18)
		{
			int time = (18 - _hour) * 60 - _minute;
			time = time * 60 - _second;
			int index = Random.Range(0, 2);
			create_notify(game_data._instance.get_language_string(m_titles[index]), game_data._instance.get_language_string(m_texts[index]), game_data._instance.get_language_string(m_texts[index]), time);
		}
		{
			int time = (12 + 24 - _hour) * 60 - _minute;
			time = time * 60 - _second;
			int index = Random.Range(0, 2);
			create_notify(game_data._instance.get_language_string(m_titles[index]), game_data._instance.get_language_string(m_texts[index]), game_data._instance.get_language_string(m_texts[index]), time);
		}
		{
			int time = (18 + 24 - _hour) * 60 - _minute;
			time = time * 60 - _second;
			int index = Random.Range(0, 2);
			create_notify(game_data._instance.get_language_string(m_titles[index]), game_data._instance.get_language_string(m_texts[index]), game_data._instance.get_language_string(m_texts[index]), time);
		}
		{
			int time = (12 + 48 - _hour) * 60 - _minute;
			time = time * 60 - _second;
			int index = Random.Range(0, 2);
			create_notify(game_data._instance.get_language_string(m_titles[index]), game_data._instance.get_language_string(m_texts[index]), game_data._instance.get_language_string(m_texts[index]), time);
		}
		{
			int time = (18 + 48 - _hour) * 60 - _minute;
			time = time * 60 - _second;
			int index = Random.Range(0, 2);
			create_notify(game_data._instance.get_language_string(m_titles[index]), game_data._instance.get_language_string(m_texts[index]), game_data._instance.get_language_string(m_texts[index]), time);
		}
	}
	
	public void onJewelGet(string scene, int num)
	{
		if(Application.isEditor)
		{
			return;
		}
		
		#if UNITY_ANDROID
		AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
		AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>("currentActivity");
		jo.Call("onJewelGet", mario._instance.m_self.userid.ToString(), scene, num.ToString());
		#elif UNITY_IPHONE
		onJewelGet(mario._instance.m_self.userid.ToString(), scene, num.ToString());
		#endif
	}
	
	public void onJewelConsume(string scene, int num)
	{
		if(Application.isEditor)
		{
			return;
		}
		
		#if UNITY_ANDROID
		AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
		AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>("currentActivity");
		jo.Call("onJewelConsume", mario._instance.m_self.userid.ToString(), scene, num.ToString());
		#elif UNITY_IPHONE
		onJewelConsume(mario._instance.m_self.userid.ToString(), scene, num.ToString());
		#endif
	}
	
	public void onUserDo(string scene, int num)
	{
		if(Application.isEditor)
		{
			return;
		}
		
		#if UNITY_ANDROID
		AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
		AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>("currentActivity");
		jo.Call("onUserDo", mario._instance.m_self.userid.ToString(), scene, num.ToString());
		#elif UNITY_IPHONE
		onUserDo(mario._instance.m_self.userid.ToString(), scene, num.ToString());
		#endif
	}
	
	public void onRaid(string scene, int num)
	{
		if(Application.isEditor)
		{
			return;
		}
		
		#if UNITY_ANDROID
		AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
		AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>("currentActivity");
		jo.Call("onRaid", mario._instance.m_self.userid.ToString(), scene, num.ToString());
		#elif UNITY_IPHONE
		onRaid(mario._instance.m_self.userid.ToString(), scene, num.ToString());
		#endif
	}
	
	public void create_notify(string title, string text, string ticker, int secondsFromNow)
	{
		if(Application.isEditor)
		{
			return;
		}
		
		#if UNITY_ANDROID
		AndroidJavaClass jc = new AndroidJavaClass("com.moon.boxworld.AlarmReceiver");
		jc.CallStatic("startAlarm", new object[4]{title, text, ticker, secondsFromNow});
		#elif UNITY_IPHONE
		createNotify(text, secondsFromNow);
		#endif
	}
	
	public void cancel_notify()
	{
		if(Application.isEditor)
		{
			return;
		}
		
		#if UNITY_ANDROID
		AndroidJavaClass javaClass = new AndroidJavaClass("com.moon.boxworld.AlarmReceiver");
		javaClass.CallStatic("clearNotification");
		#elif UNITY_IPHONE
		cancelNotify();
		#endif
	}

	public void hfad() {
		if (Application.isEditor)
		{
			return;
		}
		#if UNITY_ANDROID
		if (game_data._instance.m_channel == "google")
		{
			AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
			AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>("currentActivity");
			jo.Call("hfad");
		}
		#elif UNITY_IPHONE
		_hfad();
		#endif
	}
	
	public void close_hfad() {
		if (Application.isEditor)
		{
			return;
		}
		#if UNITY_ANDROID
		if (game_data._instance.m_channel == "google")
		{
			AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
			AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>("currentActivity");
			jo.Call("close_hfad");
		}
		#elif UNITY_IPHONE
		_close_hfad();
		#endif
	}
	
	public void cyad() {
		if (Application.isEditor)
		{
			return;
		}
		m_ad_num++;
		if (m_ad_num < m_ad_tnum)
		{
			return;
		}
		m_ad_tnum = m_ad_tnum + (int)(m_ad_tnum * 1.5);
		#if UNITY_ANDROID
		if (game_data._instance.m_channel == "google")
		{
			AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
			AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>("currentActivity");
			jo.Call("cyad");
		}
		#elif UNITY_IPHONE
		_cyad();
		#endif
	}
}

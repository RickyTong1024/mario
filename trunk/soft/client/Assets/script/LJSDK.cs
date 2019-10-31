using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

public class LJSDK : MonoBehaviour {

	public static LJSDK _instance;
	public string m_channel;
	private int m_req_num = 0;
	private s_t_shop m_t_shop;

	void Awake()
	{
		_instance = this;
	}

	/*****************************************基础接口***********************************************/

	//初始化接口
	public void init() {
		if (Application.isEditor)
		{
			init_callback_success("");
			return;
		}
		#if UNITY_ANDROID
		androidContext().Call("init");
		#else
		init_callback_success("");
		#endif
	}

	void init_callback_success(string res) {
		s_message mes = new s_message ();
		mes.m_type = "init_success";
		cmessage_center._instance.add_message (mes);
	}

	void init_callback_failed(string res) {
		mario._instance.wait(false);
		s_message mes = new s_message ();
		mes.m_type = "init_failed";
		mario._instance.show_single_dialog_box (res, mes);
	}
	
	//登录接口 
	public void login(){
		#if UNITY_ANDROID
		androidContext().Call("login");
		#elif UNITY_WEBPLAYER
		Application.ExternalCall("login");
		#elif UNITY_STANDALONE_WIN
		#if STEAM
		login_callback_success("");
		#endif
		#endif
	}

	void login_callback_success(string res) {
		#if UNITY_ANDROID
		JsonData data = JsonMapper.ToObject(res);

		protocol.game.cmsg_login_android msg = new protocol.game.cmsg_login_android();
		msg.userid = (String)data["username"];
		msg.token = (String)data["token"];
		msg.channel = (String)data["channellabel"];
		msg.nationality = "";
		msg.ver = game_data._instance.m_pt_ver;
		net_http._instance.send_msg<protocol.game.cmsg_login_android>(opclient_t.OPCODE_LOGIN_ANDROID, msg, true, game_data._instance.get_language_string("login_gui_zzlj"));
		#elif UNITY_WEBPLAYER
		JsonData data = JsonMapper.ToObject(res);
		string locale = (String)data["locale"];
		locale = locale.Split('_')[1];

		protocol.game.cmsg_login_android msg = new protocol.game.cmsg_login_android();
		msg.userid = (String)data["id"];
		msg.token = "";
		msg.channel = game_data._instance.m_channel;
		msg.nationality = locale;
		msg.ver = game_data._instance.m_pt_ver;
		net_http._instance.send_msg<protocol.game.cmsg_login_android>(opclient_t.OPCODE_LOGIN_ANDROID, msg, true, game_data._instance.get_language_string("login_gui_zzlj"));
		#elif UNITY_STANDALONE_WIN
		#if STEAM
		protocol.game.cmsg_login_android msg = new protocol.game.cmsg_login_android();
		msg.userid = Steamworks.SteamUser.GetSteamID ().ToString ();
		msg.token = "";
		msg.channel = game_data._instance.m_channel;
		msg.nationality = "";
		msg.ver = game_data._instance.m_pt_ver;
		net_http._instance.send_msg<protocol.game.cmsg_login_android>(opclient_t.OPCODE_LOGIN_ANDROID, msg, true, game_data._instance.get_language_string("login_gui_zzlj"));
		#endif
		#endif
	}
	
	void login_callback_failed(string res) {
		s_message mes = new s_message ();
		mes.m_type = "login_fail";
		cmessage_center._instance.add_message (mes);
	}

	public void req_info() {
		#if UNITY_ANDROID
		foreach (KeyValuePair<int, s_t_shop> kv in game_data._instance.m_t_shop)
		{
			if (kv.Value.type == 1)
			{
				androidContext().Call("req_info", kv.Value.code);
				m_req_num++;
			}
		}
		mario._instance.wait (true, game_data._instance.get_language_string("recharge_iphone_splb"));
		#elif UNITY_WEBPLAYER
		foreach (KeyValuePair<int, s_t_shop> kv in game_data._instance.m_t_shop)
		{
			if (kv.Value.type == 1)
			{
				Application.ExternalCall("req_info", kv.Value.code);
				m_req_num++;
			}
		}
		mario._instance.wait (true, game_data._instance.get_language_string("recharge_iphone_splb"));
		#endif
	}
	
	void recharge_android_product(string s)
	{
		if (s != "")
		{
			string[] ss = s.Split (' ');
			string code = ss[1];
			string desc = ss[0];
			foreach (KeyValuePair<int, s_t_shop> kv in game_data._instance.m_t_shop)
			{
				if (kv.Value.code == code)
				{
					kv.Value.ios_desc = desc;
					break;
				}
			}
		}
		m_req_num--;
		if (m_req_num == 0)
		{
			mario._instance.wait (false);
			
			s_message mes = new s_message();
			mes.m_type = "recharge_load_end";
			cmessage_center._instance.add_message(mes);
		}
	}

	void recharge_web_product(string s)
	{
		if (s != "")
		{
			int index = s.IndexOf(' ');
			string code = s.Substring(0, index);
			string desc = s.Substring(index + 1);
			foreach (KeyValuePair<int, s_t_shop> kv in game_data._instance.m_t_shop)
			{
				if (kv.Value.code == code)
				{
					JsonData data = JsonMapper.ToObject(desc);
					float hl = (float)data["currency"]["usd_exchange_inverse"];
					string dw = (string)data["currency"]["user_currency"];
					desc = (kv.Value.price_my * hl).ToString("f2") + " " + dw;
					kv.Value.ios_desc = desc;
					break;
				}
			}
		}
		m_req_num--;
		if (m_req_num == 0)
		{
			mario._instance.wait (false);
			
			s_message mes = new s_message();
			mes.m_type = "recharge_load_end";
			cmessage_center._instance.add_message(mes);
		}
	}
	
	//定额支付接口
	public void pay(s_t_shop t_shop) {
		#if UNITY_ANDROID
		mario._instance.wait (true);
		m_t_shop = t_shop;
		androidContext().Call("pay", mario._instance.m_self.openid, t_shop.price, t_shop.id, t_shop.desc, t_shop.code, mario._instance.m_self.notify_uri);
		#endif
	}
	
	void pay_callback_success(string res) {
		if (m_channel == "google")
		{
			protocol.game.cmsg_google_pay msg = new protocol.game.cmsg_google_pay();
			msg.id = m_t_shop.id;
			msg.package_name = "com.moon.boxworld.google";
			msg.product_id = m_t_shop.code;
			msg.purchase_token = res;
			net_http._instance.send_msg<protocol.game.cmsg_google_pay>(opclient_t.OPCODE_GOOGLE_PAY, msg);
		}
		else
		{
			s_message mes = new s_message ();
			mes.m_type = "recharge_android_success";
			cmessage_center._instance.add_message (mes);
		}
	}
	
	void pay_callback_failed(string res) {
		mario._instance.wait (false);
	}

	//登出接口 
	public void logout(){
		#if UNITY_ANDROID
		androidContext().Call("logout");
		#endif 
	}

	void logout_callback(string res) {
		s_message mes = new s_message ();
		mes.m_type = "lj_logout";
		cmessage_center._instance.add_message (mes);
	}

	public void exit(){
		#if UNITY_ANDROID
		androidContext().Call("exit", (int)game_data._instance.m_lang);
		#endif 
	}

	//退出接口 
	public void kill(){
		#if UNITY_ANDROID
		androidContext().Call("kill");
		#endif
	}

	public void doSetExtData(){
		#if UNITY_ANDROID
		androidContext().Call("doSetExtData", mario._instance.m_self.userid.ToString()
		                      , mario._instance.m_self.name.ToString()
		                      , mario._instance.m_self.level.ToString()
		                      , mario._instance.m_self.jewel.ToString());
		#endif
	}

	public void init_channel() {
		#if UNITY_ANDROID
		m_channel = androidContext().Call<string>("getChanelLabel");
		#elif UNITY_WEBPLAYER 
		Application.ExternalCall("getChannelLabel");
		#endif
	}

	void init_channel_web_callback(string channel)
	{
		m_channel = channel;
		game_data._instance.m_channel = m_channel;
		game_data._instance.init_pt_ver();
	}

	#if UNITY_ANDROID
	private AndroidJavaObject androidContext() {
		return new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity");
	}
	#endif

	#if UNITY_ANDROID
	void Update()
	{
		if(Input.GetKey(KeyCode.Escape))
		{
			exit();
		}
	}
	#endif

	void web_callback(string type)
	{
		mario._instance.wait (false);
		if (type == "1")
		{
			s_message mes = new s_message();
			mes.m_type = "recharge_web";
			mario._instance.show_double_dialog_box(game_data._instance.get_language_string("user_gui_zfdd"), mes, null);
		}
	}

	public bool need_pt()
	{
		if (game_data._instance.m_channel == "" 
		    || game_data._instance.m_channel == "yymoon" 
		    || game_data._instance.m_channel == "google"
		    || game_data._instance.m_channel == "IOS_yymoon"
		    || game_data._instance.m_channel == "web_yymoon"
		    || game_data._instance.m_channel == "win_yymoon")
		{
			return false;
		}
		return true;
	}

	public bool need_gg()
	{
		if (game_data._instance.m_channel == "yymoon" 
		    || game_data._instance.m_channel == "web_yymoon"
		    || game_data._instance.m_channel == "win_yymoon")
		{
			return true;
		}
		return false;
	}
}

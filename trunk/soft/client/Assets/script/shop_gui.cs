using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class shop_gui : MonoBehaviour, IMessage {

	public GameObject m_sub_shop;
	public List<GameObject> m_ss;
	public GameObject m_shop_view;
	private int m_index = 0;
	private int m_buy_id;
	private List<GameObject> m_shop_items = new List<GameObject>();
	private bool m_shop_init = false;
	private float m_pay_time = 0;
	public GameObject m_zs;

	void Start () {
		cmessage_center._instance.add_handle (this);
	}
	
	void OnDestroy()
	{
		cmessage_center._instance.remove_handle (this);
	}

	void OnEnable () {
		InvokeRepeating ("time", 0, 1.0f);
	}
	
	// Update is called once per frame
	void OnDisable () {
		CancelInvoke("time");
	}

	public void reset(int index)
	{
		m_index = index;
		this.gameObject.SetActive (true);
		m_ss [m_index].GetComponent<UIToggle> ().value = true;
		mario._instance.remove_child (m_shop_view);
#if UNITY_IPHONE
		recharge_iphone._instance.init();
		if (!m_shop_init && index == 0)
		{
			recharge_iphone._instance.req_info();
			return;
		}
#elif UNITY_ANDROID
		if (!Application.isEditor && !m_shop_init && index == 0)
		{
			LJSDK._instance.req_info();
			return;
		}
#elif UNITY_WEBPLAYER
		if (!Application.isEditor && !m_shop_init && index == 0)
		{
			if (game_data._instance.m_channel == "web_facebook")
			{
				LJSDK._instance.req_info();
				return;
			}
		}
#endif
		m_shop_items.Clear ();
		int num = 0;
		foreach (KeyValuePair<int, s_t_shop> kv in game_data._instance.m_t_shop)
		{
			s_t_shop t_shop = kv.Value;
			if (t_shop.slot == index)
			{
				GameObject obj = (GameObject)Instantiate(m_sub_shop);
				obj.transform.parent = m_shop_view.transform;
				obj.transform.localPosition = new Vector3(-310 + num * 200,0,0);
				obj.transform.localScale = new Vector3(1,1,1);
				obj.GetComponent<UIButtonMessage>().target = this.gameObject;
				obj.GetComponent<shop_sub>().reset(t_shop);
				obj.SetActive(true);
				m_shop_items.Add(obj);
				num++;
			}
		}
		m_shop_view.GetComponent<UIScrollView> ().ResetPosition ();
	}

	public void message (s_message message)
	{
		if (message.m_type == "recharge_load_end")
		{
			m_shop_init = true;
			reset(m_index);
		}
		if (message.m_type == "recharge_android_success")
		{
			m_pay_time = 3;
		}
		if (message.m_type == "recharge_web")
		{
			protocol.game.cmsg_pay msg = new protocol.game.cmsg_pay();
			msg.channel = game_data._instance.m_channel;
			net_http._instance.send_msg<protocol.game.cmsg_pay>(opclient_t.OPCODE_PAY, msg);
		}
	}
	
	public void net_message (s_net_message message)
	{
		if (message.m_opcode == opclient_t.OPCODE_SHOP_BUY || message.m_opcode == opclient_t.OPCODE_GOOGLE_PAY)
		{
			s_t_shop t_shop = game_data._instance.get_t_shop(m_buy_id);
			if (t_shop.type != 1)
			{
				mario._instance.m_self.jewel -= t_shop.price;
				mario_tool._instance.onJewelConsume(t_shop.desc, t_shop.price);
			}
			mario._instance.show_tip(game_data._instance.get_language_string("user_gui_hd") + t_shop.name);
			if (t_shop.type == 1)
			{
				mario._instance.m_self.jewel += t_shop.def;
				mario_tool._instance.onJewelGet(t_shop.desc, t_shop.def);
			}
			else if (t_shop.type == 2)
			{

			}
			else if (t_shop.type == 3)
			{
				mario._instance.m_self.testify = t_shop.def;
			}
			else if (t_shop.type == 4)
			{
				if (mario._instance.m_self.exp_time > timer.now())
				{
					mario._instance.m_self.exp_time += (ulong)t_shop.def * 86400000;
				}
				else
				{
					mario._instance.m_self.exp_time = timer.now() + (ulong)t_shop.def * 86400000;
				}
			}
			for (int i = 0; i < m_shop_items.Count; ++i)
			{
				m_shop_items[i].GetComponent<shop_sub>().reset();
			}
		}
		if (message.m_opcode == opclient_t.OPCODE_PAY)
		{
			if (message.m_res == -1)
			{
				#if UNITY_ANDROID
				m_pay_time = 5;
				mario._instance.wait (true);
				#elif UNITY_WEBPLAYER || UNITY_STANDALONE_WIN
				s_message mes = new s_message();
				mes.m_type = "recharge_web";
				mario._instance.show_double_dialog_box(game_data._instance.get_language_string("user_gui_wzfdd"), mes, null);
				#endif
			}
			else
			{
				protocol.game.smsg_pay _msg = net_http._instance.parse_packet<protocol.game.smsg_pay> (message.m_byte);
				mario._instance.show_tip(string.Format(game_data._instance.get_language_string("user_gui_hdzs"), _msg.jewel));
				mario._instance.m_self.jewel += _msg.jewel;
				mario_tool._instance.onJewelGet("安卓支付", _msg.jewel);
			}
		}
	}

	void click(GameObject obj)
	{
		if (obj.name == "close")
		{
			this.GetComponent<ui_show_anim>().hide_ui();
		}
	}

	void select_shop(GameObject obj)
	{
		int index = int.Parse(obj.name.Substring(1, obj.name.Length - 1));
		reset (index);
	}
	
	void click_item(GameObject obj)
	{
		s_t_shop t_shop = obj.GetComponent<shop_sub>().m_t_shop;
		m_buy_id = t_shop.id;
		if (t_shop.type == 1)
		{
			if (Application.isEditor)
			{
				return;
			}
			#if UNITY_IPHONE
			recharge_iphone._instance.buy(t_shop);
			#elif UNITY_ANDROID
			LJSDK._instance.pay(t_shop);
			#elif UNITY_WEBPLAYER
			string order = mario._instance.m_self.userid.ToString() + timer.now().ToString();
			Application.ExternalCall("show_recharge", order, t_shop.desc, t_shop.price, mario._instance.m_self.notify_uri, mario._instance.m_self.openid);
			mario._instance.wait(true);
			#elif UNITY_STANDALONE_WIN
			string order = mario._instance.m_self.userid.ToString() + timer.now().ToString();
			string url = string.Format("http://mario.web.yymoon.com/recharge/pay.php?WIDout_trade_no={0}&WIDsubject={1}&WIDtotal_fee={2}&WINotify_url={3}&WIextra_common_param={4}",
			                           order, t_shop.desc, t_shop.price, mario._instance.m_self.notify_uri, mario._instance.m_self.openid);
			Application.OpenURL(url);
			s_message mes = new s_message();
			mes.m_type = "recharge_web";
			mario._instance.show_double_dialog_box(game_data._instance.get_language_string("user_gui_zfdd"), mes, null);
			#endif
			return;
		}
		if (t_shop.type != 1 && mario._instance.m_self.jewel < t_shop.price)
		{
			mario._instance.show_tip(game_data._instance.get_language_string("user_gui_zsbz"));
			return;
		}
		protocol.game.cmsg_shop_buy msg = new protocol.game.cmsg_shop_buy();
		msg.id = t_shop.id;
		net_http._instance.send_msg<protocol.game.cmsg_shop_buy>(opclient_t.OPCODE_SHOP_BUY, msg);
	}

	void time()
	{
		m_ss [m_index].GetComponent<UIToggle> ().value = true;
		m_zs.GetComponent<UILabel>().text = mario._instance.m_self.jewel.ToString();
	}

	void Update()
	{
		if (m_pay_time > 0)
		{
			m_pay_time -= Time.deltaTime;
			if (m_pay_time <= 0)
			{
				m_pay_time = 0;
				protocol.game.cmsg_pay msg = new protocol.game.cmsg_pay();
				msg.channel = game_data._instance.m_channel;
				net_http._instance.send_msg<protocol.game.cmsg_pay>(opclient_t.OPCODE_PAY, msg);
			}
		}
	}
}

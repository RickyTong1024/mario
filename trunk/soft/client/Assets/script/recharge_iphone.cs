using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

public class recharge_iphone : MonoBehaviour {
	
#if UNITY_IPHONE

	[DllImport("__Internal")]
	private static extern void InitIAPManager();//初始化
	
	[DllImport("__Internal")]
	private static extern bool IsProductAvailable();//判断是否可以购买
	
	[DllImport("__Internal")]
	private static extern void RequstProductInfo(string s);//获取商品信息
	
	[DllImport("__Internal")]
	private static extern void BuyProduct(string s);//购买商品

	public static recharge_iphone _instance;
	private s_t_shop m_t_shop;
	private int m_req_num = 0;

	void Awake()
	{
		_instance = this;
	}

	public void init () {
		InitIAPManager();
	}

	public void req_info()
	{
		foreach (KeyValuePair<int, s_t_shop> kv in game_data._instance.m_t_shop)
		{
			if (kv.Value.type == 1)
			{
				RequstProductInfo(kv.Value.code);
				m_req_num++;
			}
		}
		mario._instance.wait (true, game_data._instance.get_language_string("recharge_iphone_splb"));
	}
		
	void recharge_iphone_product(string s)
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
		m_req_num--;
		if (m_req_num == 0)
		{
			mario._instance.wait (false);

			s_message mes = new s_message();
			mes.m_type = "recharge_load_end";
			cmessage_center._instance.add_message(mes);
		}
	}

	public void buy(s_t_shop t_shop)
	{
		m_t_shop = t_shop;
		BuyProduct(m_t_shop.code);
		mario._instance.wait (true);
	}

	void recharge_iphone_done(string s)
	{
		mario._instance.wait (false);

		protocol.game.cmsg_shop_buy msg = new protocol.game.cmsg_shop_buy();
		msg.id = m_t_shop.id;
		msg.receipt = System.Text.Encoding.Default.GetBytes (s);
		net_http._instance.send_msg<protocol.game.cmsg_shop_buy>(opclient_t.OPCODE_SHOP_BUY, msg);
	}

	void recharge_iphone_cancel(string s)
	{
		mario._instance.wait (false);
	}
#endif
}

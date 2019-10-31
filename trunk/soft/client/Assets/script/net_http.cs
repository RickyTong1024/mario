using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System;

public class net_pck
{
	public opclient_t opcode;
	public object obj;
	public float wait;
	public bool restart;
	public string text;
};

public class net_http : MonoBehaviour,IMessage {

	/*private string m_ip = "http://192.168.2.128:8080/";
	private string m_ips = "https://192.168.2.128:8080/";
	private string m_en_ip = "http://192.168.2.128:8080/";
	private string m_en_ips = "https://192.168.2.128:8080/";*/

	private string m_ip = "http://mario.yymoon.com:8080/";
	private string m_ips = "https://mario.yymoon.com:8080/";
	private string m_en_ip = "http://mario.en.yymoon.com:8080/";
	private string m_en_ips = "https://mario.en.yymoon.com:8080/";
	private List<net_pck> m_pcks = new List<net_pck>();
	private float m_wait = 0;
	private WWW m_www;
	public static net_http _instance;
	
	void Awake()
	{
		_instance = this;
	}
	
	void Start ()
	{
		cmessage_center._instance.add_handle (this);
	}

	void OnDestroy()
	{
		cmessage_center._instance.remove_handle (this);
	}

	public static int bytesToInt(byte[] bytes)
	{
		int addr = bytes[0];
		addr |= ((bytes[1] << 8));
		addr |= ((bytes[2] << 16));
		addr |= ((bytes[3] << 24));
		return addr;
	}
	
	private void onRecMsg(opclient_t opcode, int res, byte[] msg)
	{
		s_net_message _message = new s_net_message ();
		_message.m_byte = msg;
		_message.m_opcode = opcode;
		_message.m_res = res;
		cmessage_center._instance.add_net_message (_message);
	}
	
	private bool do_www(WWW _www, opclient_t opcode)
	{
		if (!string.IsNullOrEmpty(_www.error))
		{
			mario._instance.wait(false);
			m_wait = 0;
				
			do_fail();

			Debug.Log("http error  :" + _www.error);
			return false;
		}  
		else
		{
			mario._instance.wait(false);
			m_wait = 0;

			protocol.game.msg_response response = net_http._instance.parse_packet<protocol.game.msg_response> (_www.bytes);
			if (response.res == 0)
			{
				if (mario._instance.m_self != null)
				{
					mario._instance.m_self.m_pck_id++;
				}
				onRecMsg(opcode, response.res, response.msg);
			}
			else if (response.res == -1)
			{
				if (mario._instance.m_self != null)
				{
					mario._instance.m_self.m_pck_id++;
				}
				onRecMsg(opcode, response.res, response.error);
			}
			else
			{
				mario._instance.show_tip(game_data._instance.get_t_error(response.res));
			}

			return true;
		}
	}

	public void send_msg<T>(opclient_t opcode, T obj, bool restart = true, string text = "", float wait = 10)
	{
		net_pck np = new net_pck();
		np.opcode = opcode;
		np.obj = obj;
		np.wait = wait;
		np.text = text;
		np.restart = restart;
		m_pcks.Add(np);
		
		if (m_pcks.Count > 1)
		{
			return;
		}
		
		net_start();
	}

	public T parse_packet<T>(byte[] bytes)
	{
		System.IO.MemoryStream _ms = new System.IO.MemoryStream(bytes);
		object _msg = new object();
		_msg = ProtoBuf.Serializer.Deserialize<T> (_ms);
		return (T)_msg;
	}
	
	void net_start()
	{
		m_wait = m_pcks[0].wait;
		mario._instance.wait(true, m_pcks[0].text);
		
		Type t = m_pcks [0].obj.GetType ();
		if (t.GetProperty ("common") != null)
		{
			protocol.game.msg_common comm = new protocol.game.msg_common();
			comm.userid = mario._instance.m_self.userid;
			comm.sig = mario._instance.m_self.m_sig;
			comm.pck_id = mario._instance.m_self.m_pck_id;
			t.GetProperty ("common").SetValue(m_pcks [0].obj, comm, null);
		}
		
		System.IO.MemoryStream _memStream = new System.IO.MemoryStream ();  
		ProtoBuf.Serializer.Serialize(_memStream,m_pcks[0].obj);
		byte[] b = encrypt_des.encode(_memStream.ToArray ());
		StartCoroutine(http(m_pcks[0].opcode, b));
	}

	void do_fail()
	{
		if (m_pcks[0].restart)
		{
			s_message _message = new s_message();
			_message.m_type = "net_restart";
			mario._instance.show_single_dialog_box(game_data._instance.get_language_string("net_http_wlsb"), _message);
		}
		else
		{
			onRecMsg(m_pcks[0].opcode, 1, null);
			m_pcks.RemoveAt (0);
			if (m_pcks.Count > 0)
			{
				net_start();
			}
		}
	}
	
	void IMessage.net_message(s_net_message message)
	{

	}
	
	void IMessage.message(s_message message)
	{
		if(message.m_type == "net_restart")
		{
			net_start();
		}
	}

	void Update ()
	{
		if(m_wait > 0)
		{
			m_wait -= Time.deltaTime;
			if(m_wait <= 0)
			{
				this.StopAllCoroutines ();
				mario._instance.wait(false);

				do_fail();
			}
		}
	}
	
	IEnumerator http(opclient_t opcode, byte[] msg)
	{
		string ip = m_ip;
		if (game_data._instance.m_channel == "web_facebook")
		{
			ip = m_ips;
		}
		if (game_data._instance.m_lang == e_language.el_english)
		{
			ip = m_en_ip;
			if (game_data._instance.m_channel == "web_facebook")
			{
				ip = m_en_ips;
			}
		}
		m_www = new WWW (ip + ((int)opcode).ToString(), msg);
		while(!m_www.isDone)
		{
			yield return new WaitForSeconds(0.1f);
		}
		
		bool res = do_www (m_www, opcode);
		if (res)
		{
			m_pcks.RemoveAt (0);
			if (m_pcks.Count > 0)
			{
				net_start();
			}
		}
	}
}

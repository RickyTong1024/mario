using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class s_message
{
	public long m_guid = 0;
	public bool m_remove = false;
	public string m_type;
	public float time = 0.0f;
	public s_message m_next;
	public ArrayList m_floats = new ArrayList();
	public ArrayList m_ints = new ArrayList();	
	public ArrayList m_string = new ArrayList();
	public ArrayList m_object = new ArrayList ();
	public ArrayList m_long = new ArrayList ();
	public List<bool> m_bools = new List<bool> ();
}

public class s_net_message
{
	public string m_des;
	public opclient_t m_opcode;
	public byte[] m_byte;
	public int m_res;
}

public interface IMessage
{
	void message (s_message message);
	void net_message (s_net_message message);
}

public class cmessage_center : MonoBehaviour {

	private ArrayList m_messages = new ArrayList();
	private ArrayList m_net_messages = new ArrayList();
	private ArrayList m_handles = new ArrayList();

	private ArrayList m_temp_messages = new ArrayList();
	public static cmessage_center _instance;

	void Awake()
	{
		_instance = this;
	}
	
	public int get_message_num(string type)
	{
		int _num = 0;

		for(int i = 0;i < m_messages.Count;i ++)
		{
			s_message _message = m_messages[i] as s_message;

			if(_message.m_type == type)
			{
				_num ++;
			}
		}

		return _num;
	}

	public void add_message(s_message message)
	{
		if(message == null)
		{
			return ;
		}

		m_temp_messages.Add (message);
	}

	public void add_net_message(s_net_message message)
	{
		m_net_messages.Add (message);
	}

	public void add_handle(IMessage message)
	{
		for(int i = 0;i < m_handles.Count;i ++)
		{
			if(m_handles[i] == message)
			{ 
				return;
			}
		}

		m_handles.Add (message);
	}
	public void remove_handle(IMessage message)
	{
		for(int i = 0;i < m_handles.Count;)
		{
			if(m_handles[i] == message)
			{
				m_handles.RemoveAt(i);
				continue;
			}
			i ++;
		}
	}

	void message()
	{
		for(int c = 0;c < m_messages.Count;)
		{
			s_message _message = m_messages[c] as s_message;
			if(_message.time <= 0.0f)
			{
				m_messages.RemoveAt(c);
				for(int i = 0;i < m_handles.Count;)
				{
					IMessage _handles = (IMessage)m_handles[i];

					if(_handles != null)
					{
						_handles.message(_message);
						i ++;
					}
					else
					{
						m_handles.RemoveAt(c);
					}
				}
			}
			else
			{
				_message.time -= Time.deltaTime;
				c ++;
			}
		}
	}

	void net_message()
	{
		while(m_net_messages.Count > 0)
		{
			s_net_message _message = m_net_messages[0] as s_net_message;
			m_net_messages.RemoveAt(0);
			for(int i = 0;i < m_handles.Count;i ++)
			{
				IMessage _handles = (IMessage)m_handles[i];

				if(_handles != null)
				{
					_handles.net_message(_message);
				}
			}
		}
	}

	void handle_message()
	{
		while(m_temp_messages.Count > 0)
		{
			m_messages.Add(m_temp_messages[0]);
			m_temp_messages.RemoveAt(0);
		}

		message ();
		net_message ();
	}

	void Update () {
		handle_message ();
	}
}

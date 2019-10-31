using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class edit_cy
{
	public int fx;
	public int world;
	public mario_point p;
	public Sprite sp;
	public int num;
}

public class render : MonoBehaviour, IMessage {

	public GameObject m_rd;
	public GameObject m_rd1;
	public GameObject m_loading;
	public GameObject m_fuzhu;
	public GameObject m_play_mode;
	public GameObject m_edit_mode;
	private List<edit_cy> m_edit_cys = new List<edit_cy>();

	void Start()
	{
		cmessage_center._instance.add_handle (this);
	}

	public void message (s_message message)
	{
		if (message.m_type == "play_mode")
		{
			m_edit_mode.SetActive(false);
			m_play_mode.SetActive(true);
			m_rd.SetActive(true);
			m_rd1.SetActive(true);
			m_fuzhu.SetActive(false);
			mario_point p = (mario_point)message.m_object[0];
			int world = 0;
			if (message.m_ints.Count > 0)
			{
				world = (int)message.m_ints[0];
			}
			int mode = 0;
			if (message.m_ints.Count > 1)
			{
				mode = (int)message.m_ints[1];
			}
			m_play_mode.GetComponent<play_mode>().reload(p, world, mode);
		}
		if (message.m_type == "close_play_mode")
		{
			m_play_mode.SetActive(false);
			if (mario._instance.m_game_state != e_game_state.egs_edit)
			{
				m_rd.SetActive(false);
				m_rd1.SetActive(false);
			}
		}
		if (message.m_type == "edit_mode")
		{
			m_play_mode.SetActive(false);
			m_edit_mode.SetActive(true);
			m_rd.SetActive(true);
			m_rd1.SetActive(true);
			m_fuzhu.SetActive(true);
			mario_point p = null;
			if (message.m_object.Count > 0)
			{
				p = (mario_point)message.m_object[0];
			}
			int world = 0;
			if (message.m_ints.Count > 0)
			{
				world = (int)message.m_ints[0];
			}
			m_edit_mode.GetComponent<edit_mode>().reload(p, world, m_edit_cys);
			m_edit_cys = new List<edit_cy>();
		}
		if (message.m_type == "close_edit_mode")
		{
			m_edit_mode.SetActive(false);
			m_rd.SetActive(false);
			m_rd1.SetActive(false);
		}
		if (message.m_type == "first_load")
		{
			foreach (KeyValuePair<int, s_t_unit> kv in game_data._instance.m_t_unit)	
			{
				s_t_unit t_unit = kv.Value;
				string name = t_unit.res;
				string cname = "unit/" + name + "/" + name;
				if (t_unit != null && t_unit.kfg == 1)
				{
					cname = "unit/" + name + "/1/" + name;
				}
				GameObject res = (GameObject)Resources.Load (cname);
				GameObject obj = (GameObject)Instantiate(res);
				obj.transform.parent = m_loading.transform;
				obj.transform.localPosition = new Vector3 (0, 0, 0);
				obj.transform.localScale = new Vector3 (1, 1, 1);
			}
			s_message mes = new s_message();
			mes.m_type = "first_load_end";
			mes.time = 0.3f;
			cmessage_center._instance.add_message(mes);
		}
		if (message.m_type == "first_load_end")
		{
			mario._instance.remove_child(m_loading);
		}
		if (message.m_type == "edit_canying")
		{
			edit_cy ec = (edit_cy)message.m_object[0];
			if (m_edit_cys.Count > 0)
			{
				edit_cy cy1 = m_edit_cys[m_edit_cys.Count - 1];
				float d = Mathf.Sqrt((cy1.p.x - ec.p.x) * (cy1.p.x - ec.p.x) + (cy1.p.y - ec.p.y) * (cy1.p.y - ec.p.y));
				if (d < 640)
				{
					return;
				}
			}
			m_edit_cys.Add(ec);
			if (m_edit_cys.Count > 50)
			{
				m_edit_cys.RemoveAt(0);
			}
		}
	}

	public void net_message (s_net_message message)
	{
	}
}

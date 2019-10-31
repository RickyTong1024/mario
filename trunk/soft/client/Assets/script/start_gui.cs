using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class start_gui : MonoBehaviour {

	public GameObject m_panel;
	public GameObject m_start_gui_sub;
	public GameObject m_num;
	public GameObject m_touxiang;
	public GameObject m_gq;
	public GameObject m_name;
	public GameObject m_map_name;
	public GameObject m_jd;
	private List<GameObject> m_subs = new List<GameObject>();
	private GameObject m_big;
	private int m_wait_time;
	private int m_state;

	void OnEnable()
	{
		m_num.GetComponent<UILabel> ().text = "x " + mario._instance.m_self.br_life.ToString ();
		m_touxiang.GetComponent<UISprite> ().spriteName = game_data._instance.get_t_touxiang (mario._instance.m_self.br_user_head);
		m_name.GetComponent<UILabel> ().text = mario._instance.m_self.br_user_name;
		m_gq.GetComponent<UISprite> ().spriteName = game_data._instance.get_t_guojia (mario._instance.m_self.br_user_country);
		m_map_name.GetComponent<UILabel> ().text = mario._instance.m_self.br_map_name;
		s_t_br t_br = game_data._instance.get_t_br(mario._instance.m_self.br_hard);
		m_jd.GetComponent<UILabel>().text = (mario._instance.m_self.br_index + 1).ToString() + "/" + t_br.num;
		this.gameObject.SetActive (true);
		if (mario._instance.m_start_type == 0)
		{
			start ();
		}
		else if (mario._instance.m_start_type == 1)
		{
			die ();
		}
		else
		{
			next ();
		}
	}

	public void start()
	{
		m_num.SetActive (false);
		mario._instance.remove_child (m_panel);
		m_subs.Clear ();
		m_wait_time = 100;
		m_state = 0;
		for (int i = 0; i < 4; ++i)
		{
			List<GameObject> subs = new List<GameObject>();
			int w = 0;
			for (int j = 24; j >= 0; --j)
			{
				GameObject obj = (GameObject)Instantiate(m_start_gui_sub);
				obj.transform.parent = m_panel.transform;
				obj.transform.localPosition = new Vector3(-640, -125 - 50 * i, 0);
				obj.transform.localScale = new Vector3(1,1,1);
				w = obj.GetComponent<start_gui_sub> ().run_to (-432 + j * 36, -125 - 50 * i, true, 1.0f, true, w);
				if (j % 4 == 0)
				{
					obj.GetComponent<start_gui_sub> ().m_sound = true;
				}
				obj.SetActive (true);
				if (i % 2 == 0)
				{
					subs.Add (obj);
				}
				else
				{
					m_subs.Add(obj);
				}
			}
			for (int j = subs.Count - 1; j >= 0; --j)
			{
				m_subs.Add(subs[j]);
			}
			if (w > m_wait_time)
			{
				m_wait_time = w;
			}
		}
		m_wait_time += 80;
	}

	public void die()
	{
		int num = mario._instance.m_self.br_life;
		m_num.SetActive (false);
		mario._instance.remove_child (m_panel);
		m_subs.Clear ();
		m_wait_time = 100;
		m_state = 0;
		int step = num / 25;
		if (step < 1)
		{
			step = 0;
		}
		for (int i = 0; i < 4; ++i)
		{
			for (int j = 0; j < 25; ++j)
			{
				if (num <= i * 25 + j)
				{
					continue;
				}
				GameObject obj = (GameObject)Instantiate(m_start_gui_sub);
				obj.transform.parent = m_panel.transform;
				int r = -1;
				if (i % 2 == 1)
				{
					r = 1;
					obj.transform.localPosition = new Vector3(432 - j * 36, -125 - 50 * i, 0);
				}
				else
				{
					obj.transform.localPosition = new Vector3(-432 + j * 36, -125 - 50 * i, 0);
				}
				obj.transform.localScale = new Vector3(r,1,1);
				if (i * 25 + j == 0)
				{
					obj.GetComponent<start_gui_sub> ().m_sound = true;
				}
				obj.SetActive (true);
				m_subs.Add (obj);
			}
		}
	}

	public void next()
	{
		int num = mario._instance.m_self.br_life;
		m_num.SetActive (true);
		mario._instance.remove_child (m_panel);
		m_subs.Clear ();
		m_wait_time = 150;
		m_state = 3;
		for (int i = 0; i < 4; ++i)
		{
			for (int j = 0; j < 25; ++j)
			{
				if (num - 1 <= i * 25 + j)
				{
					continue;
				}
				GameObject obj = (GameObject)Instantiate(m_start_gui_sub);
				obj.transform.parent = m_panel.transform;
				int r = -1;
				if (i % 2 == 1)
				{
					r = 1;
					obj.transform.localPosition = new Vector3(432 - j * 36, -125 - 50 * i, 0);
				}
				else
				{
					obj.transform.localPosition = new Vector3(-432 + j * 36, -125 - 50 * i, 0);
				}
				obj.transform.localScale = new Vector3(r,1,1);
				obj.SetActive (true);
			}
		}
		{
			GameObject obj = (GameObject)Instantiate(m_start_gui_sub);
			obj.transform.parent = m_panel.transform;
			obj.transform.localPosition = new Vector3(0, 2, 0);
			obj.transform.localScale = new Vector3(2.5f,2.5f,1);
			obj.SetActive (true);
		}
	}

	void jump()
	{
		m_wait_time = 30;
		m_state = 1;
		m_big = m_subs [0];
		m_big.GetComponent<start_gui_sub> ().jump ();
	}

	void run()
	{
		m_wait_time = 40;
		m_state = 2;
		m_big.GetComponent<start_gui_sub> ().run_to (0, 2, true, 2.5f);
		for (int i = 0; i < 4; ++i)
		{
			for (int j = 0; j < 25; ++j)
			{
				int index = i * 25 + j;
				if (index == 0)
				{
					continue;
				}
				if (m_subs.Count <= index)
				{
					continue;
				}
				if (i % 2 == 0)
				{
					if (j == 0)
					{
						m_subs[index].GetComponent<start_gui_sub> ().run_to (-432 + j * 36, -125 - 50 * i + 50, false);
					}
					else
					{
						m_subs[index].GetComponent<start_gui_sub> ().run_to (-432 + j * 36 - 36, -125 - 50 * i, false);
					}
				}
				else
				{
					if (j == 0)
					{
						m_subs[index].GetComponent<start_gui_sub> ().run_to (432 - j * 36, -125 - 50 * i + 50, true);
					}
					else
					{
						m_subs[index].GetComponent<start_gui_sub> ().run_to (432 - j * 36 + 36, -125 - 50 * i, true);
					}
				}
			}
		}
	}

	void FixedUpdate()
	{
		if (m_wait_time > 0)
		{
			m_wait_time--;
			if (m_wait_time == 0)
			{
				if (m_state == 0)
				{
					jump();
				}
				else if (m_state == 1)
				{
					run ();
				}
				else if (m_state == 2)
				{
					m_num.SetActive(true);
					m_num.transform.localScale = new Vector3(0.1f, 0.1f ,0.1f);
					utils.add_scale_anim(m_num, 0.3f, new Vector3(1, 1, 1), 0);
					m_state = 3;
					m_wait_time = 50;
				}
				else if (m_state == 3)
				{
					mario._instance.change_state(e_game_state.egs_br_play, 1, delegate() { this.gameObject.SetActive(false); });
				}
			}
		}
	}
}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

[Serializable]
public class road_gui_road
{
	public Vector3 m_c2;
	public List<Vector3> m_points = new List<Vector3>();
	public List<bool> m_fxs = new List<bool>();
	public List<GameObject> m_lubiao = new List<GameObject>();
}

public class road_gui : MonoBehaviour {

	public List<road_gui_road> m_roads = new List<road_gui_road>();
	public GameObject m_zhu1;
	public GameObject m_zhu2;
	public GameObject m_zhu3;
	public GameObject m_zhu4;
	public GameObject m_road1;
	public GameObject m_road2;
	public GameObject m_c2;
	public GameObject m_main;
	public GameObject m_life;
	public GameObject m_jd;
	public Vector3 m_qd;
	private int m_type = 0;
	private int m_wait_time = 0;
	private int m_state = 0;
	private int m_index = 0;

	void OnEnable()
	{
		reset ();
		if (mario._instance.m_start_type == 0)
		{
			start ();
		}
		else
		{
			next ();
		}
	}

	void OnDisable()
	{
		this.GetComponent<Animator> ().enabled = false;
	}

	void reset()
	{
		m_type = 0;
		if (mario._instance.m_self.br_hard > 2)
		{
			m_type = 1;
		}
		m_index = mario._instance.m_self.br_index;
		if (m_type == 0)
		{
			m_road1.SetActive(true);
			m_road2.SetActive(false);
		}
		else
		{
			m_road1.SetActive(false);
			m_road2.SetActive(true);
		}
		m_c2.transform.localPosition = m_roads[m_type].m_c2;
		m_life.GetComponent<UILabel>().text = "x " + mario._instance.m_self.br_life.ToString();
		s_t_br t_br = game_data._instance.get_t_br(mario._instance.m_self.br_hard);
		m_jd.GetComponent<UILabel>().text = (mario._instance.m_self.br_index + 1).ToString() + "/" + t_br.num;
	}

	public void start()
	{
		m_wait_time = 75;
		m_state = 0;
		m_index = 0;
		m_main.SetActive (false);
		m_main.transform.localScale = new Vector3(1, 1, 1);
		for (int i = 0; i < m_roads [m_type].m_lubiao.Count; ++i)
		{
			m_roads [m_type].m_lubiao[i].SetActive(false);
		}
	}

	public void next()
	{
		m_wait_time = 75;
		m_state = 2;
		m_main.SetActive (true);
		if (m_index == 0)
		{
			m_main.transform.localPosition = m_qd;
		}
		else
		{
			m_main.transform.localPosition = m_roads[m_type].m_points[m_index - 1];
			if (m_roads[m_type].m_fxs[m_index - 1])
			{
				m_main.transform.localScale = new Vector3(-1, 1, 1);
			}
			else
			{
				m_main.transform.localScale = new Vector3(1, 1, 1);
			}
		}
		for (int i = 0; i <= m_index; ++i)
		{
			m_roads [m_type].m_lubiao[i].SetActive(true);
		}
		for (int i = m_index + 1; i < m_roads [m_type].m_lubiao.Count; ++i)
		{
			m_roads [m_type].m_lubiao[i].SetActive(false);
		}
	}

	void zhu1()
	{
		m_zhu1.GetComponent<sprite_anim> ().play ("stand");
		m_zhu2.GetComponent<sprite_anim> ().play ("stand");
	}
	
	void zhu2()
	{
		m_zhu3.GetComponent<sprite_anim> ().play ("stand");
		m_zhu4.GetComponent<sprite_anim> ().play ("stand");
	}
	
	void end()
	{
		this.GetComponent<Animator> ().Play("road_main");
		m_state = 1;
	}

	void end1()
	{
		this.GetComponent<Animator> ().enabled = false;
		m_state = 2;
		m_wait_time = 30;
		m_roads [m_type].m_lubiao [0].SetActive (true);
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
					this.GetComponent<Animator> ().enabled = true;
					if (m_type == 0)
					{
						m_road1.SetActive(true);
						m_road2.SetActive(false);
						this.GetComponent<Animator> ().Play("road_gui1");
					}
					else
					{
						m_road1.SetActive(false);
						m_road2.SetActive(true);
						this.GetComponent<Animator> ().Play("road_gui2");
					}
				}
				else if (m_state == 2)
				{
					m_main.GetComponent<road_gui_sub>().run_to(m_roads[m_type].m_points[m_index].x, m_roads[m_type].m_points[m_index].y);
					m_wait_time = 50;
					m_state = 3;
				}
				else if (m_state == 3)
				{
					mario._instance.play_sound("sound/quan");
					List<int> p = new List<int>();
					p.Add((int)m_roads[m_type].m_points[m_index].x);
					p.Add((int)m_roads[m_type].m_points[m_index].y);
					mario._instance.change_state(e_game_state.egs_br_start, 3, delegate() { this.gameObject.SetActive(false); }, p);
				}
			}
		}
	}
}

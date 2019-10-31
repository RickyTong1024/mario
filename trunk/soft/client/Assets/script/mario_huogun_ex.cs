using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class mario_huogun_ex : mario_attack_ex {

	public List<GameObject> m_s;
	private int m_d;

	public override void reset ()
	{
		for (int i = 6; i < m_s.Count; ++i)
		{
			if (i - 6 < m_param [0] / 2)
			{
				m_s[i].SetActive(true);
				if (m_shadow != null)
				{
					m_shadow.GetComponent<mario_huogun_ex>().m_s[i].SetActive(true);
				}
			}
			else
			{
				m_s[i].SetActive(false);
				if (m_shadow != null)
				{
					m_shadow.GetComponent<mario_huogun_ex>().m_s[i].SetActive(false);
				}
			}
		}
		m_d = -m_param [1] * 45;
		refresh (m_d);
	}

	public override void change ()
	{
		if (m_param[0] < 7)
		{
			m_param[0] += 1;
		}
		else
		{
			m_param[0] = 0;
		}
		if (m_unit != null)
		{
			game_data._instance.m_arrays[m_world][m_init_pos.y][m_init_pos.x].param[0] = m_param[0];
		}
		reset ();
	}

	public override void change1 ()
	{
		if (m_param[1] < 7)
		{
			m_param[1] += 1;
		}
		else
		{
			m_param[1] = 0;
		}
		if (m_unit != null)
		{
			game_data._instance.m_arrays[m_world][m_init_pos.y][m_init_pos.x].param[1] = m_param[1];
		}
		reset ();
	}

	void refresh(int d)
	{
		mario_point p = utils.get_rxy(d);
		for (int i = 0; i < m_s.Count; ++i)
		{
			m_s[i].transform.localPosition = new Vector3(p.x * i * 32 / 1000, p.y * i * 32 / 1000);
			m_s[i].transform.localEulerAngles = new Vector3(0, 0, -d * 20);
		}
		if (m_shadow != null)
		{
			for (int i = 0; i < m_s.Count; ++i)
			{
				m_shadow.GetComponent<mario_huogun_ex>().m_s[i].transform.localPosition = new Vector3(p.x * i * 32 / 1000, p.y * i * 32 / 1000);
				m_shadow.GetComponent<mario_huogun_ex>().m_s[i].transform.localEulerAngles = new Vector3(0, 0, -d * 20);
			}
		}
	}

	public override void be_hit(mario_obj obj)
	{
		if (obj.m_main)
		{
			bool flag = false;
			mario_point p = utils.get_rxy(m_d);
			for (int i = 0; i < m_s.Count; ++i)
			{
				int left = p.x * i * 320 / 1000 - 100 + m_pos.x;
				int right = p.x * i * 320 / 1000 + 100 + m_pos.x;
				int top = p.y * i * 320 / 1000 + 100 + m_pos.y;
				int bottom = p.y * i * 320 / 1000 - 100 + m_pos.y;
				if (right >= obj.m_bound.left && left <= obj.m_bound.right && top >= obj.m_bound.bottom && bottom <= obj.m_bound.top)
				{
					if (i - 6 >= m_param [0] / 2)
					{
						break;
					}
					flag = true;
					break;
				}
			}
			if (flag)
			{
				obj.set_bl(0, 4);
			}
		}
	}

	public override void tupdate()
	{
		m_d = (-m_param [1] * 45 + play_mode._instance.m_time) % 360;
		if (m_param[0] % 2 == 0)
		{
			m_d = (-m_param [1] * 45 - play_mode._instance.m_time) % 360;
		}
		refresh (m_d);
	}
}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class mario_luoci : mario_attack_ex1 {

	private bool m_hit = false;
	private int m_die_time = 0;
	private int m_sz_time = 0;
	private int m_db = 0;

	public override void reset ()
	{
		if (m_param[0] == 0)
		{
			m_collider.m_rect.h = 640;
			m_collider.m_rect.w = 590;
			m_db = 960;
			set_scale(0.5f, 0.5f, 1);
		}
		else if (m_param[0] == 1)
		{
			m_collider.m_rect.h = 1280;
			m_collider.m_rect.w = 1180;
			m_db = 1280;
			set_scale(1, 1, 1);
		}
		else
		{
			m_collider.m_rect.h = 1920;
			m_collider.m_rect.w = 1770;
			m_db = 1600;
			set_scale(1.5f, 1.5f, 1);
		}
	}

	public override void set_bl(int index, int num)
	{
		if (index == 0 && num == 4)
		{
			m_is_die = true;
			m_bkcf = true;
		}
		if (index == 0 && num == 5)
		{
			if (m_param[0] == 0)
			{
				m_is_die = true;
				m_bkcf = true;
			}
			else if (m_param[0] == 1)
			{
				m_sz_time = 30;
				m_velocity.y = 0;
			}
			else if (m_param[0] == 1)
			{
				m_sz_time = 5;
				m_velocity.y = 0;
			}
		}
	}

	public override void change()
	{
		if (m_param[0] < 2)
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

	public override void tupdate()
	{
		if (m_is_die)
		{
			if (m_die_time == 0)
			{
				play_anim("die");
				m_velocity.x = 0;
				m_velocity.y = 0;
			}
			else if (m_die_time > 20)
			{
				m_is_destory = 1;
			}
			m_die_time++;
		}
		else
		{
			int dx = play_mode._instance.m_main_char.m_pos.x - m_pos.x;
			int dy = play_mode._instance.m_main_char.m_pos.y - m_pos.y;
			if (play_mode._instance.m_main_char != null && dx > -m_db && dx < m_db && dy < 0)
			{
				m_hit = true;
				m_bkcf = true;
				play_anim("stand1");
			}
			if (m_hit)
			{
				if (m_sz_time > 0)
				{
					m_sz_time--;
					m_velocity.y -= 5;
					if (m_velocity.y < -250)
					{
						m_velocity.y = -250;
					}
				}
				else
				{
					if (m_velocity.y > -100)
					{
						m_velocity.y -= 10;
					}
					else
					{
						m_velocity.y -= 20;
					}
					if (m_velocity.y < -250)
					{
						m_velocity.y = -250;
					}
				}
			}
		}
	}
}

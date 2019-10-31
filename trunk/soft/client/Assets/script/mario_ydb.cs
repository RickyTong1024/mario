using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class mario_ydb : mario_block1 {

	public List<GameObject> m_s;
	private int m_time = 0;
	private int m_state = 0;
	private mario_point m_p = new mario_point();

	public override void init (string name, List<int> param, int world, int x, int y, int xx, int yy)
	{
		base.init (name, param, world, x, y, xx, yy);
		m_is_dzd = 0;
	}

	public override void reset()
	{
		for (int i = 0; i < m_s.Count; ++i)
		{
			m_s[i].SetActive(false);
		}
		m_s [m_param [0]].SetActive (true);
		m_time = 100;
		m_p.set (m_pos.x * 2, m_pos.y * 2);
	}

	public override bool be_left_hit (mario_obj obj, ref int px)
	{
		return false;
	}
	
	public override bool be_right_hit (mario_obj obj, ref int px)
	{
		return false;
	}
	
	public override bool be_top_hit (mario_obj obj, ref int py)
	{
		return false;
	}
	
	public override bool be_bottom_hit (mario_obj obj, ref int py)
	{
		if (m_param[0] == 2 && obj.m_main)
		{
			m_state = 1;
		}
		return base.be_bottom_hit (obj, ref py);
	}
	
	public override bool be_left_top_hit (mario_obj obj, ref int px, ref int py)
	{
		return false;
	}
	
	public override bool be_right_top_hit (mario_obj obj, ref int px, ref int py)
	{
		return false;
	}
	
	public override bool be_left_bottom_hit (mario_obj obj, ref int px, ref int py)
	{
		if (m_param[0] == 2 && obj.m_main)
		{
			m_state = 1;
		}
		return base.be_left_bottom_hit (obj, ref px, ref py);
	}
	
	public override bool be_right_bottom_hit (mario_obj obj, ref int px, ref int py)
	{
		if (m_param[0] == 2 && obj.m_main)
		{
			m_state = 1;
		}
		return base.be_right_bottom_hit (obj, ref px, ref py);
	}

	public override mario_point move()
	{
		mario_point p = new mario_point ();
		if (m_param[0] == 2)
		{
			if (m_state == 1)
			{
				p.y = -50;
				m_pos.y -= 50;
			}
		}
		else
		{
			int yx = 60;
			if (m_time < 60)
			{
				yx = m_time;
			}
			else if (m_time > 140)
			{
				yx = 200 - m_time;
			}
			if (m_state == 0)
			{
				yx = -yx;
			}
			m_time++;
			if (m_time >= 200)
			{
				m_time = 0;
				m_state = 1 - m_state;
			}

			if (m_param[0] == 0)
			{
				m_p.x += yx;
				int dx = m_p.x / 2 - m_pos.x;
				p.x = dx;
				m_pos.x += dx;
			}
			else
			{
				m_p.y += yx;
				int dy = m_p.y / 2 - m_pos.y;
				p.y = dy;
				m_pos.y += dy;
			}
		}
		return p;
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
}

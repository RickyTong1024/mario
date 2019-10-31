﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class mario_wugui_big : mario_charater {
	
	public override void init (string name, List<int> param, int world, int x, int y, int xx, int yy)
	{
		base.init (name, param, world, x, y, xx, yy);
		m_life = 1;
		m_min_speed = 20;
		m_can_be_on_char = true;
	}
	
	public override void reset ()
	{
		if (m_param[0] % 2 == 0)
		{
			set_fx(mario_fx.mf_left);
		}
		else
		{
			set_fx(mario_fx.mf_right);
		}
		if (m_param[0] / 2 == 0)
		{
			m_hr.SetActive(false);
		}
		else
		{
			m_hr.SetActive(true);
		}
	}
	
	public override bool be_bottom_hit(mario_obj obj, ref int py)
	{
		if (m_bl[0] == 1)
		{
			return false;
		}
		if (obj.m_main)
		{
			obj.m_pvelocity.y = 150;
			if (obj.m_velocity.y < 0)
			{
				obj.m_velocity.y = 0;
			}
			play_mode._instance.caisi(0, true, obj.m_pos.x, obj.m_bound.bottom);
			this.m_is_destory = 1;
			play_mode._instance.add_score(m_pos.x, m_pos.y, 200);
			mario._instance.play_sound ("sound/caisi");
			List<int> l = new List<int>();
			l.Add(1);
			l.Add(0);
			l.Add(0);
			l.Add(0);
			play_mode._instance.create_mario_obj_ex("mario_wuguike_big", null, l, -1, -1, m_pos.x, m_pos.y);
			py = obj.get_bottom_hit_pos(this);
			return true;
		}
		return base.be_bottom_hit (obj, ref py);
	}
	
	public override void be_hit(mario_obj obj)
	{
		if (m_bl[0] == 1)
		{
			return;
		}
		base.be_hit (obj);
	}
	
	public override void change()
	{
		if (m_param[0] < 3)
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
	
	public override void set_bl(int index, int num)
	{
		base.set_bl (index, num);
		if (index == 0 && num == 4)
		{
			m_is_die = true;
			m_velocity.x = Random.Range(-50, 50);
			m_velocity.y = 150;
			mario._instance.play_sound ("sound/zhuang");
			play_mode._instance.add_score(m_pos.x, m_pos.y, 200);
		}
	}
	
	public override void tupdate()
	{
		base.tupdate ();
		if (m_bl[0] == 4)
		{
			m_velocity.y -= utils.g_g;
			play_anim("die1");
		}
		else if (m_bl[0] == 0)
		{
			play_anim("stand");
			if (m_param[0] / 2 == 1)
			{
				check_zhineng();
			}
		}
	}
}

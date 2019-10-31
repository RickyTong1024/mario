﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class mario_ciwei : mario_charater {

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
			play_mode._instance.add_score(m_pos.x, m_pos.y, 500);
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

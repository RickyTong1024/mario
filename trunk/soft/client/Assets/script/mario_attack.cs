﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class mario_attack : mario_obj {

	public override void init (string name, List<int> param, int world, int x, int y, int xx, int yy)
	{
		base.init (name, param, world, x, y, xx, yy);
		m_type = mario_type.mt_attack;
	}

	public override bool be_left_hit (mario_obj obj, ref int px)
	{
		if (obj.m_type != mario_type.mt_charater)
		{
			return false;
		}
		if (obj.m_main)
		{
			obj.set_bl(0, 4);
		}
		if (obj.m_wgk)
		{
			mario._instance.play_sound("sound/doff");
		}
		if (obj.m_velocity.x < 0)
		{
			obj.m_velocity.x = 0;
			obj.change_way(mario_fx.mf_right);
		}
		px = obj.get_left_hit_pos(this);
		return true;
	}
	
	public override bool be_right_hit (mario_obj obj, ref int px)
	{
		if (obj.m_type != mario_type.mt_charater)
		{
			return false;
		}
		if (obj.m_main)
		{
			obj.set_bl(0, 4);
		}
		if (obj.m_wgk)
		{
			mario._instance.play_sound("sound/doff");
		}
		if (obj.m_velocity.x > 0)
		{
			obj.m_velocity.x = 0;
			obj.change_way(mario_fx.mf_left);
		}
		px = obj.get_right_hit_pos(this);
		return true;
	}
	
	public override bool be_top_hit (mario_obj obj, ref int py)
	{
		if (obj.m_type != mario_type.mt_charater)
		{
			return false;
		}
		if (obj.m_main)
		{
			obj.set_bl(0, 4);
		}
		if (obj.m_velocity.y > 0)
		{
			obj.m_velocity.y = 0; 
			if (obj.m_main)
			{
				play_mode._instance.ding();
			}
		}
		py = obj.get_top_hit_pos(this);
		return true;
	}
	
	public override bool be_bottom_hit (mario_obj obj, ref int py)
	{
		if (obj.m_type != mario_type.mt_charater)
		{
			return false;
		}
		if (obj.m_main)
		{
			obj.set_bl(0, 4);
		}
		if (obj.m_velocity.y < 0)
		{
			obj.m_velocity.y = 0;
		}
		py = obj.get_bottom_hit_pos(this);
		return true;
	}
}

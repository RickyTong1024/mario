using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class mario_fx_pao_big : mario_charater {

	public override void init (string name, List<int> param, int world, int x, int y, int xx, int yy)
	{
		base.init (name, param, world, x, y, xx, yy);
		m_life = 1;
		m_can_be_on_char = true;
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
			m_velocity.y -= 15;
			play_anim("die1");
		}
		else if (m_bl[0] == 0)
		{
			if (play_mode._instance.m_main_char == null)
			{
				return;
			}
			else if (play_mode._instance.m_time % 200 == 90)
			{
				play_anim("attack");
			}
			else if (play_mode._instance.m_time % 200 == 100)
			{
				List<int> l = new List<int>();
				l.Add(0);
				l.Add(0);
				l.Add(0);
				l.Add(0);
				mario_obj obj = play_mode._instance.create_mario_obj_ex("mario_huoqiu_big", null, l, -1, -1, m_pos.x, m_pos.y + 200);
				obj.m_velocity.set (play_mode._instance.m_main_char.m_pos.x, play_mode._instance.m_main_char.m_pos.y);
				obj.reset();
			}
			else if (play_mode._instance.m_time % 200 == 120)
			{
				List<int> l = new List<int>();
				l.Add(0);
				l.Add(0);
				l.Add(0);
				l.Add(0);
				mario_obj obj = play_mode._instance.create_mario_obj_ex("mario_huoqiu_big", null, l, -1, -1, m_pos.x, m_pos.y + 200);
				obj.m_velocity.set (play_mode._instance.m_main_char.m_pos.x, play_mode._instance.m_main_char.m_pos.y);
				obj.reset();
			}
			else if (play_mode._instance.m_time % 200 == 140)
			{
				List<int> l = new List<int>();
				l.Add(0);
				l.Add(0);
				l.Add(0);
				l.Add(0);
				mario_obj obj = play_mode._instance.create_mario_obj_ex("mario_huoqiu_big", null, l, -1, -1, m_pos.x, m_pos.y + 200);
				obj.m_velocity.set (play_mode._instance.m_main_char.m_pos.x, play_mode._instance.m_main_char.m_pos.y);
				obj.reset();
			}

			if (play_mode._instance.m_main_char.m_pos.x < m_pos.x)
			{
				set_fx(mario_fx.mf_left);
			}
			else
			{
				set_fx(mario_fx.mf_right);
			}
		}
	}
}

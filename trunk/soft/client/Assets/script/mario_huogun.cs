using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class mario_huogun : mario_block {

	private mario_obj m_hg_ex;

	public override void reset ()
	{
		if (m_hg_ex == null)
		{
			List<int> p = new List<int>();
			p.Add(m_param[0]);
			p.Add(m_param[1]);
			p.Add(m_param[2]);
			p.Add(m_param[3]);
			if (m_edit_mode)
			{
				m_hg_ex = edit_mode._instance.create_mario_obj("mario_huogun_ex", null, p, m_init_pos.x, m_init_pos.y);
			}
			else
			{
				m_hg_ex = play_mode._instance.create_mario_obj("mario_huogun_ex", null, p, m_init_pos.x, m_init_pos.y);
			}
		}
		if (m_param [0] % 2 == 0)
		{
			set_fx(mario_fx.mf_left);
		}
		else
		{
			set_fx(mario_fx.mf_right);
		}
	}

	public override void change ()
	{
		m_hg_ex.GetComponent<mario_obj> ().change ();
		for (int i = 0; i < 3; ++i)
		{
			m_param[i] = m_hg_ex.GetComponent<mario_obj>().m_param[i];
			game_data._instance.m_arrays[m_world][m_init_pos.y][m_init_pos.x].param[i] = m_hg_ex.GetComponent<mario_obj>().m_param[i];
		}
		reset ();
	}

	public override void change1 ()
	{
		m_hg_ex.GetComponent<mario_obj> ().change1 ();
		for (int i = 0; i < 3; ++i)
		{
			m_param[i] = m_hg_ex.GetComponent<mario_obj>().m_param[i];
			game_data._instance.m_arrays[m_world][m_init_pos.y][m_init_pos.x].param[i] = m_hg_ex.GetComponent<mario_obj>().m_param[i];
		}
	}
}

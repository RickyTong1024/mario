using UnityEngine;
using System.Collections;

public class mario_hua_block : mario_block {

	public override void reset ()
	{
		if (m_param[0] == 0)
		{
			set_fx(mario_fx.mf_right);
		}
		else
		{
			set_fx(mario_fx.mf_left);
		}
	}

	public override mario_point move()
	{
		mario_point p = new mario_point ();
		if (m_param[0] == 0)
		{
			p.x = 50;
		}
		else
		{
			p.x = -50;
		}
		return p;
	}

	public override void change()
	{
		if (m_param[0] == 0)
		{
			m_param[0] = 1;
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

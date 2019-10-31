using UnityEngine;
using System.Collections;

public class mario_chilun : mario_attack_ex {

	private int m_db = 0;
	public override void reset ()
	{
		if (m_param[0] == 0)
		{
			m_collider.m_rect.h = 960;
			m_collider.m_rect.w = 960;
			m_db = 650;
			set_scale(0.5f, 0.5f, 1);
		}
		else if (m_param[0] == 1)
		{
			m_collider.m_rect.h = 1920;
			m_collider.m_rect.w = 1920;
			m_db = 1100;
			set_scale(1, 1, 1);
		}
		else
		{
			m_collider.m_rect.h = 3840;
			m_collider.m_rect.w = 3840;
			m_db = 2000;
			set_scale(2, 2, 1);
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

	public override void be_hit(mario_obj obj)
	{
		if (obj.m_main)
		{
			int d = (int)Mathf.Sqrt((m_pos.x - obj.m_pos.x) * (m_pos.x - obj.m_pos.x) + (m_pos.y - obj.m_pos.y) * (m_pos.y - obj.m_pos.y));
			if (d < m_db)
			{
				obj.set_bl(0, 4);
			}
		}
	}
}

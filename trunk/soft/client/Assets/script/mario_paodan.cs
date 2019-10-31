using UnityEngine;
using System.Collections;

public class mario_paodan : mario_attack_ex {

	private mario_point m_dir = new mario_point();
	private mario_point m_yz = new mario_point();

	public override void reset ()
	{
		m_dir.x = m_velocity.x;
		m_dir.y = m_velocity.y;
		m_yz.x = m_pos.x * 100;
		m_yz.y = m_pos.y * 100;

		set_vr ();
	}

	public override bool be_bottom_hit(mario_obj obj, ref int py)
	{
		if (obj.m_main)
		{
			obj.m_pvelocity.y = 150;
			obj.m_velocity.y = 0;
			play_mode._instance.caisi(0, true, obj.m_pos.x, obj.m_bound.bottom);
			this.m_is_die = true;
			play_mode._instance.add_score(m_pos.x, m_pos.y, 100);
			mario._instance.play_sound ("sound/caisi");
		}
		return false;
	}

	void set_vr()
	{
		int yz = (int)Mathf.Sqrt ((float)(m_dir.x * m_dir.x * 100 + m_dir.y * m_dir.y * 100));
		if (yz == 0)
		{
			return;
		}
		m_yz.x += m_dir.x * 4000 / yz * 10;
		m_yz.y += m_dir.y * 4000 / yz * 10;
		m_velocity.x = m_yz.x / 100 - m_pos.x;
		m_velocity.y = m_yz.y / 100 - m_pos.y;

		float d = Mathf.Atan2 (m_dir.y, m_dir.x) * 180 / Mathf.PI;
		set_angles (0, 0, d);
	}

	public override void tupdate()
	{
		if (m_is_die)
		{
			m_velocity.x = 0;
			m_velocity.y -= utils.g_g;
		}
		else
		{
			if (play_mode._instance.m_main_char != null)
			{
				int x = play_mode._instance.m_main_char.m_pos.x;
				int y = play_mode._instance.m_main_char.m_pos.y;
				x = x - m_pos.x;
				y = y - m_pos.y;

				int r1 = utils.atan(x, y);
				int r2 = utils.atan(m_dir.x, m_dir.y);

				if (r1 < r2)
				{
					int r = r2 - r1;
					if (r < 180)
					{
						r2 = r2 - 1;
					}
					else
					{
						r2 = r2 + 1;
					}
				}
				else
				{
					int r = r1 - r2;
					if (r < 180)
					{
						r2 = r2 + 1;
					}
					else
					{
						r2 = r2 - 1;
					}
				}
				r2 = (r2 + 360) % 360;
				m_dir = utils.tan(r2);
			}
			set_vr ();
		}
	}
}

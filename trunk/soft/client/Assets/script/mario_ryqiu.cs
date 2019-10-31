using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class mario_ryqiu : mario_attack_ex {

	private int m_y;
	private	int m_up = 0;
	private int m_d_time = 0;

	public override void init (string name, List<int> param, int world, int x, int y, int xx, int yy)
	{
		base.init (name, param, world, x, y, xx, yy);
		m_y = m_pos.y;
		if (!m_edit_mode)
		{
			m_pos.y = -2000;
			m_velocity.y = 200;
		}
		m_bkcf = true;
	}

	public override void tupdate()
	{
		m_d_time++;
		if (m_up == 0)
		{
			int dy = m_y - m_pos.y;
			if (dy <= 0)
			{
				m_up = 1;
				m_d_time = 0;
				m_velocity.y = 0;
				this.transform.FindChild("sprite").localEulerAngles = new Vector3(0, 0, 180);
				if (m_shadow != null)
				{
					m_shadow.transform.FindChild("sprite").localEulerAngles = new Vector3(0, 0, 180);
				}
			}
			else if (dy > 2000)
			{
				m_velocity.y = 200;
			}
			else
			{
				m_velocity.y -= 10;
				if (m_velocity.y <= 10)
				{
					m_velocity.y = 10;
				}
			}
		}
		else if (m_up == 1)
		{
			int my = -2000 - m_pos.y;
			if (my >= 0)
			{
				m_up = 20;
				m_d_time = 0;
				m_velocity.y = 0;
				this.transform.FindChild("sprite").localEulerAngles = new Vector3(0, 0, 0);
				if (m_shadow != null)
				{
					m_shadow.transform.FindChild("sprite").localEulerAngles = new Vector3(0, 0, 0);
				}
			}
			else
			{
				m_velocity.y -= 10;
				if (m_velocity.y <= -200)
				{
					m_velocity.y = -200;
				}
			}
		}
		else if (m_up == 20)
		{
			if (m_d_time >= 100)
			{
				m_up = 0;
				m_d_time = 0;
				m_velocity.y = 200;
			}
		}
	}
}

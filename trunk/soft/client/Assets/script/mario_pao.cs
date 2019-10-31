using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class mario_pao : mario_block {

	private int m_z = 0;
	private mario_obj m_pd = null;

	public override void init (string name, List<int> param, int world, int x, int y, int xx, int yy)
	{
		base.init (name, param, world, x, y, xx, yy);
		m_bkcf = true;
	}

	public override void tupdate()
	{
		if (play_mode._instance.m_main_char == null)
		{
			return;
		}
		if (play_mode._instance.m_time % 250 == 100 && m_pd == null)
		{
			mario._instance.play_sound("sound/pao");
			play_anim("pao");
			List<int> l = new List<int>();
			l.Add(0);
			l.Add(0);
			l.Add(0);
			l.Add(0);
			if (m_z == 0)
			{
				m_pd = play_mode._instance.create_mario_obj_ex("mario_paodan", null, l, -1, -1, m_pos.x + 640, m_pos.y);
				m_pd.m_velocity.set (200, 0);
			}
			else if (m_z == 45)
			{
				m_pd = play_mode._instance.create_mario_obj_ex("mario_paodan", null, l, -1, -1, m_pos.x + 450, m_pos.y + 450);
				m_pd.m_velocity.set (100, 100);
			}
			else if (m_z == 90)
			{
				m_pd = play_mode._instance.create_mario_obj_ex("mario_paodan", null, l, -1, -1, m_pos.x, m_pos.y + 640);
				m_pd.m_velocity.set (0, 200);
			}
			else if (m_z == 135)
			{
				m_pd = play_mode._instance.create_mario_obj_ex("mario_paodan", null, l, -1, -1, m_pos.x - 450, m_pos.y + 450);
				m_pd.m_velocity.set (-100, 100);
			}
			else if (m_z == 180)
			{
				m_pd = play_mode._instance.create_mario_obj_ex("mario_paodan", null, l, -1, -1, m_pos.x - 640, m_pos.y);
				m_pd.m_velocity.set (-200, 0);
			}
			else if (m_z == 225)
			{
				m_pd = play_mode._instance.create_mario_obj_ex("mario_paodan", null, l, -1, -1, m_pos.x - 450, m_pos.y - 450);
				m_pd.m_velocity.set (-100, -100);
			}
			else if (m_z == 270)
			{
				m_pd = play_mode._instance.create_mario_obj_ex("mario_paodan", null, l, -1, -1, m_pos.x, m_pos.y - 640);
				m_pd.m_velocity.set (0, -200);
			}
			else
			{
				m_pd = play_mode._instance.create_mario_obj_ex("mario_paodan", null, l, -1, -1, m_pos.x + 450, m_pos.y - 450);
				m_pd.m_velocity.set (100, -100);
			}
			m_pd.reset();
		}

		int dx = play_mode._instance.m_main_char.m_pos.x - m_pos.x;
		int dy = play_mode._instance.m_main_char.m_pos.y - m_pos.y;

		if (dx < 0)
		{
			if (dy > 0)
			{
				if (dy * 100 < -dx * 41)
				{
					m_z = 180;
				}
				else if (dy * 100 < -dx * 241)
				{
					m_z = 135;
				}
				else
				{
					m_z = 90;
				}
			}
			else
			{
				if (-dy * 100 < -dx * 41)
				{
					m_z = 180;
				}
				else if (-dy * 100 < -dx * 241)
				{
					m_z = 225;
				}
				else
				{
					m_z = 270;
				}
			}
		}
		else
		{
			if (dy > 0)
			{
				if (dy * 100 < dx * 41)
				{
					m_z = 0;
				}
				else if (dy * 100 < dx * 241)
				{
					m_z = 45;
				}
				else
				{
					m_z = 90;
				}
			}
			else
			{
				if (-dy * 100 < dx * 41)
				{
					m_z = 0;
				}
				else if (-dy * 100 < dx * 241)
				{
					m_z = 315;
				}
				else
				{
					m_z = 270;
				}
			}
		}
		
		this.transform.FindChild("pao").localEulerAngles = new Vector3 (0, 0, m_z);
		if (m_shadow != null)
		{
			m_shadow.transform.FindChild("pao").localEulerAngles = new Vector3 (0, 0, m_z);
		}
	}
}

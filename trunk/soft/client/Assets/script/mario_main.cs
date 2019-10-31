using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class mario_main : mario_charater {

	private int m_die_time = 0;
	private int m_win_state = 0;
	public int m_big = 0;
	public int m_yin_time = 0;
	public GameObject m_bhz;

	public override void init (string name, List<int> param, int world, int x, int y, int xx, int yy)
	{
		base.init (name, param, world, x, y, xx, yy);
		m_main = true;
		m_is_on_floor = true;
		m_per_is_on_floor = true;
		m_life = 1;
		m_bl.Add (0);
	}

	public void end ()
	{
		m_win_state = 2;
		m_is_destory = 1;
		play_mode._instance.m_state = 100;
	}

	public void set_cs(int big, int ytime)
	{
		m_big = big;
		m_yin_time = ytime;
		if (big == 1)
		{
			m_bhz.SetActive (true);
			m_bhz.GetComponent<Animator>().Play("chixu");
		}
	}

	public override void set_bl(int index, int num)
	{
		base.set_bl (index, num);
		if (index == 0 && num == 4)
		{
			if (m_yin_time > 0)
			{
				m_bl[0] = 0;
			}
			else if (m_big == 0)
			{
				m_is_die = true;
			}
			else
			{
				m_bl[0] = 0;
				m_bl[1] = 50;
				mario._instance.play_sound ("sound/dmg");
				pause_anim();
				m_bhz.SetActive(true);
				m_bhz.GetComponent<Animator>().Play("chuxian");
			}
		}
		if (index == 0 && num == 3)
		{
			if (m_big == 1)
			{
				m_bl[0] = 0;
				m_bl[1] = 0;
			}
			else
			{
				pause_anim();
				m_bhz.SetActive(true);
				m_bhz.GetComponent<Animator>().Play("chuxian");
			}
		}
	}
	
	public override void tupdate()
	{
		if (m_bl[1] > 0)
		{
			m_bl[1]--;
			if (m_bl[1] == 0)
			{
				if (m_bl[0] == 3)
				{
					m_bl[0] = 0;
					m_big = 1;
					m_bhz.GetComponent<Animator>().Play("chixu");
				}
				else
				{
					m_big = 0;
					m_yin_time = 150;
					m_bhz.SetActive(false);
				}
				continue_anim();
			}
		}

		if (m_yin_time > 0)
		{
			m_yin_time--;
			if (m_yin_time % 6 == 3)
			{
				this.transform.FindChild("sprite").GetComponent<SpriteRenderer>().enabled = false;
			}
			else if (m_yin_time % 4 == 0)
			{
				this.transform.FindChild("sprite").GetComponent<SpriteRenderer>().enabled = true;
			}
		}

		if (play_mode._instance.m_state == 1)
		{
			if (m_win_state == 0)
			{
				m_win_state = 1;
				m_velocity.set(0, 0);
				play_anim("win");
				play_mode._instance.m_die_pos = new mario_point(m_pos.x + m_world * 10000000, m_pos.y);
			}
			mario_obj obj = m_bl_objs[0];
			int dx = obj.m_pos.x - m_pos.x;
			int dy = obj.m_pos.y + 100 - m_pos.y;
			if (dx < 0)
			{
				dx = dx / 10;
				if (dx == 0)
				{
					dx = -1;
				}
			}
			else if (dx > 0)
			{
				dx = dx / 10;
				if (dx == 0)
				{
					dx = 1;
				}
			}
			if (dy < 0)
			{
				dy = dy / 10;
				if (dy == 0)
				{
					dy = -1;
				}
			}
			else if (dy > 0)
			{
				dy = dy / 10;
				if (dy == 0)
				{
					dy = 1;
				}
			}
			
			m_pos.x += dx;
			m_pos.y += dy;
		}
		else if (m_is_die)
		{
			m_die_time++;
			if (m_die_time == 1)
			{
				play_mode._instance.m_die_pos = new mario_point(m_pos.x + m_world * 10000000, m_pos.y);
				mario._instance.play_mus_ex1 ("music/lose", false);
			}
			if (m_die_time < 50)
			{
				play_anim("die");
				m_velocity.x = 0;
				m_velocity.y = 0;
			}
			else if (m_die_time == 50)
			{
				play_anim("die2");
				m_velocity.y = 150;
			}
			else if (m_die_time < 100)
			{
				m_velocity.y -= utils.g_g;
				if (m_velocity.y < -160)
				{
					m_velocity.y = -160;
				}
			}
			else
			{
				m_is_destory = 1;
			}

			if (m_die_time == 20)
			{
				play_mode._instance.start_cha();
			}
		}
		else if (!m_is_on_floor)
		{
			if (!m_edit_mode)
			{
				if (play_mode._instance.m_paqiang > 0)
				{
					play_anim("paqiang");
					if (play_mode._instance.m_time % 6 == 0)
					{
						if (m_fx == mario_fx.mf_left)
						{
							play_mode._instance.effect("scyw", m_pos.x - 310, m_pos.y + 300);
						}
						else
						{
							play_mode._instance.effect("scyw", m_pos.x + 310, m_pos.y + 300);
						}
						mario._instance.play_sound_ex ("sound/moca");
					}
				}
				else if (play_mode._instance.m_jump_num <= 1)
				{
					if (m_velocity.y >= 0)
					{
						play_anim("jump11");
					}
					else
					{
						play_anim("jump12");
					}
				}
				else if (play_mode._instance.m_jump_num == 2)
				{
					play_anim("jump21");
				}
				else
				{
					play_anim("jump3");
				}
			}
		}
		else if (m_is_move)
		{
			if (!m_edit_mode)
			{
				bool flag = false;
				if (play_mode._instance.m_shache_time > 0)
				{
					flag = true;
					if (get_anim_name() != "shache")
					{
						mario._instance.play_sound_ex ("sound/moca");
					}
					play_anim("shache");
					if (play_mode._instance.m_time % 6 == 0 && play_mode._instance.m_shache_time > 0)
					{
						play_mode._instance.effect("scyw", m_pos.x, m_pos.y - utils.g_grid_size / 2);
					}
				}
				if (!flag)
				{
					int speed = (int)(m_velocity.x * 0.7);
					if (speed < 0)
					{
						speed = -speed;
					}
					if (speed < 30)
					{
						speed = 30;
					}
					else if (speed > 100)
					{
						speed = 100;
					}
					if (speed <= 56)
					{
						play_anim("run", speed);
					}
					else if (speed <= 84)
					{
						play_anim("run1", speed);
						if (play_mode._instance.m_time % 10 == 0)
						{
							mario._instance.play_sound_ex ("sound/step", 0.5f);
						}
					}
					else
					{
						play_anim("run2", speed);
						if (play_mode._instance.m_time % 8 == 0)
						{
							mario._instance.play_sound_ex ("sound/step");
						}
					}
				}
			}
		}
		else
		{
			if (!m_per_is_on_floor)
			{
				play_anim("dun");
			}
			else if (get_anim_name() != "dun")
			{
				play_anim("stand");
			}
		}

		if (!m_is_die && mario._instance.m_game_state == e_game_state.egs_edit && mario._instance.m_self.guide >= 100)
		{
			edit_cy ec = new edit_cy();
			ec.fx = (int)m_fx;
			ec.world = m_world;
			ec.p = new mario_point(m_pos.x, m_pos.y);
			ec.sp = this.GetComponent<mario_anim>().get_sprite();

			s_message mes = new s_message();
			mes.m_type = "edit_canying";
			mes.m_object.Add(ec);
			cmessage_center._instance.add_message(mes);
		}
	}
}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class mario_po_block : mario_block {

	private bool m_hit = false;
	private int m_hit_time = 0;
	private int m_die_time;
	public GameObject m_s;
	private GameObject m_edit_obj;

	public override void init (string name, List<int> param, int world, int x, int y, int xx, int yy)
	{
		base.init (name, param, world, x, y, xx, yy);
		m_is_dzd = 2;
	}

	public override void reset ()
	{
		mario._instance.remove_child (m_s);
		if (m_shadow != null)
		{
			mario._instance.remove_child (m_shadow.GetComponent<mario_po_block>().m_s);
		}
		m_edit_obj = null;
		if (m_param[0] != 0 && m_edit_mode)
		{
			s_t_unit t_unit = game_data._instance.get_t_unit(m_param[0]);
			string cname = "unit/" + t_unit.res + "/" + t_unit.res;
			GameObject res = (GameObject)Resources.Load (cname);
			m_edit_obj = (GameObject)Instantiate(res);
			m_edit_obj.transform.parent = m_s.transform;
			m_edit_obj.transform.localScale = new Vector3 (1, 1, 1);
			mario_obj mobj = m_edit_obj.GetComponent<mario_obj> ();
			mobj.m_edit_mode = true;
			List<int> p = new List<int>();
			p.Add(m_param[1]);
			p.Add(m_param[2]);
			p.Add(m_param[3]);
			p.Add (0);
			mobj.init (t_unit.res, p, m_world, -1, -1, 0, 0);
		}
	}

	public override bool be_left_hit (mario_obj obj, ref int px)
	{
		if (obj.m_type != mario_type.mt_charater)
		{
			return false;
		}
		if (obj.m_wgk)
		{
			if (obj.m_name == "mario_wuguike_big" || obj.m_name == "mario_huangguike_big")
			{
				hit();
				return false;
			}
			else if (m_param[0] > 0)
			{
				hit1 ();
			}
			else
			{
				hit ();
			}
		}
		return base.be_left_hit (obj, ref px);
	}
	
	public override bool be_right_hit (mario_obj obj, ref int px)
	{
		if (obj.m_type != mario_type.mt_charater)
		{
			return false;
		}
		if (obj.m_wgk)
		{
			if (obj.m_name == "mario_wuguike_big" || obj.m_name == "mario_huangguike_big")
			{
				hit();
				return false;
			}
			else if (m_param[0] > 0)
			{
				hit1 ();
			}
			else
			{
				hit ();
			}
		}
		return base.be_right_hit (obj, ref px);
	}

	public override bool be_top_hit (mario_obj obj, ref int px)
	{
		if (obj.m_type != mario_type.mt_charater)
		{
			return false;
		}
		if (obj.m_main)
		{
			if (!m_hit)
			{
				for (int i = 0; i < m_nl_objs.Count; ++i)
				{
					mario_obj obj1 = m_nl_objs[i];
					if (obj1.m_life == 1 || obj1.m_wgk)
					{
						obj1.set_bl(0, 4);
					}
					else
					{
						obj1.m_pvelocity.y = 250;
					}
				}
			}
			if (m_param[0] > 0)
			{
				hit1 ();
			}
			else
			{
				hit ();
			}
		}
		return base.be_top_hit (obj, ref px);
	}

	public override void be_hit(mario_obj obj)
	{
		base.be_hit (obj);
		if (obj.m_name == "mario_huoqiu_big" || (obj.m_name == "mario_luoci" && obj.m_param[0] != 0))
		{
			hit();
		}
	}

	void hit()
	{
		m_is_die = true;
		m_bkcf = true;
		if (!m_hit)
		{
			this.play_anim("die");
		}
		else
		{
			this.play_anim("die1");
		}
		play_mode._instance.add_score(10);
		mario._instance.play_sound ("sound/ding");
	}

	void hit1()
	{
		if (m_hit)
		{
			return;
		}
		m_hit = true;
		m_bkcf = true;
		{
			s_t_unit t_unit = game_data._instance.get_t_unit(m_param[0]);
			play_anim ("hit");
			mario._instance.play_sound ("sound/dinfo");
			List<int> p = new List<int>();
			p.Add(m_param[1]);
			p.Add(m_param[2]);
			p.Add(m_param[3]);
			p.Add (0);
			mario_obj obj1 = play_mode._instance.create_mario_obj(t_unit.res, null, p, m_init_pos.x, m_init_pos.y);
			obj1.set_bl(0, 1);
		}
		this.transform.FindChild("sprite").GetComponent<SpriteRenderer>().sortingOrder = 1;
	}

	public override void change()
	{
		if (m_edit_obj == null)
		{
			return;
		}
		m_edit_obj.GetComponent<mario_obj> ().change ();
		for (int i = 0; i < 3; ++i)
		{
			m_param[i + 1] = m_edit_obj.GetComponent<mario_obj>().m_param[i];
			game_data._instance.m_arrays[m_world][m_init_pos.y][m_init_pos.x].param[i + 1] = m_edit_obj.GetComponent<mario_obj>().m_param[i];
		}
	}

	public override void tupdate()
	{
		if (m_is_die)
		{
			m_die_time++;
			if (m_die_time >= 50)
			{
				m_is_destory = 1;
			}
		}
		else if (m_hit)
		{
			m_hit_time++;
			if (m_hit_time == 20)
			{
				if (m_param[0] == 0)
				{
					play_mode._instance.add_score(m_pos.x, m_pos.y + 1000, 100);
				}
				this.transform.FindChild("sprite").GetComponent<SpriteRenderer>().sortingOrder = 0;
			}
			else if (m_hit_time >= 30)
			{
				play_anim("stand1");
			}
		}
		else
		{
			play_anim("stand");
		}
	}
}

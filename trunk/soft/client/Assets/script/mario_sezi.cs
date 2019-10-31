using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class mario_sezi : mario_block {

	private bool m_hit;
	private int m_hit_time = 0;
	private int m_num = 1;
	public GameObject m_s;
	private GameObject m_edit_obj;

	public override void reset ()
	{
		mario._instance.remove_child (m_s);
		if (m_shadow != null)
		{
			mario._instance.remove_child (m_shadow.GetComponent<mario_sezi>().m_s);
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

	public override bool be_top_hit (mario_obj obj, ref int py)
	{
		if (obj.m_main && !m_hit)
		{
			m_hit = true;
			dingchu();
		}
		return base.be_top_hit (obj, ref py);
	}

	void dingchu()
	{
		m_bkcf = true;
		
		if (m_param[0] == 0)
		{
			play_anim("hit");
			mario._instance.play_sound ("sound/coins");
		}
		else
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
	
	public override void tupdate()
	{
		if (!m_hit)
		{
			m_num++;
			if (m_num > 6)
			{
				m_num = 1;
			}
			play_anim("stand");
		}
		else
		{
			m_hit_time++;
			if (m_hit_time == 20)
			{
				play_mode._instance.add_score(m_pos.x, m_pos.y, 500 * m_num);
				this.transform.FindChild("sprite").GetComponent<SpriteRenderer>().sortingOrder = 0;
				play_anim(m_num.ToString());
			}
		}
	}
}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class mario_cym : mario_obj {

	public GameObject m_s;
	private GameObject m_edit_obj;
	private List<GameObject> m_objs = new List<GameObject>();

	public override void init (string name, List<int> param, int world, int x, int y, int xx, int yy)
	{
		base.init (name, param, world, x, y, xx, yy);
		m_bkcf = true;
	}

	public override void reset ()
	{
		mario._instance.remove_child (m_s);
		if (m_shadow != null)
		{
			mario._instance.remove_child (m_shadow.GetComponent<mario_cym>().m_s);
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

	void dingchu()
	{
		if (m_param[0] == 0)
		{

		}
		else
		{
			while (m_objs.Count < 3)
			{
				m_objs.Add(null);
			}
			int index= -1;
			for (int i = 0; i < m_objs.Count; ++i)
			{
				if (m_objs[i] == null)
				{
					index = i;
					break;
				}
			}
			if (index == -1)
			{
				return;
			}
			s_t_unit t_unit = game_data._instance.get_t_unit(m_param[0]);
			List<int> p = new List<int>();
			p.Add(m_param[1]);
			p.Add(m_param[2]);
			p.Add(m_param[3]);
			p.Add (0);
			mario_obj obj1 = play_mode._instance.create_mario_obj(t_unit.res, null, p, m_init_pos.x, m_init_pos.y);
			m_objs[index] = obj1.gameObject;
		}
	}

	public override void tupdate()
	{
		if (play_mode._instance.m_time % 250 == 50)
		{
			dingchu();
		}
	}
}

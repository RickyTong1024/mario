using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class play_mode : MonoBehaviour {

	public Camera m_cam;
	public GameObject m_bj;
	public GameObject m_other;
	public GameObject m_back;
	public GameObject m_main;
	public GameObject m_shadow;
	public List<bool> m_now_inputs = new List<bool>();
	private List<bool> m_per_inputs = new List<bool>();
	private List<List<mario_obj>> m_aobjs = new List<List<mario_obj>> ();
	private List<mario_obj> m_mobjs = new List<mario_obj> ();
	private List<List<mario_obj>> m_xyobjs = new List<List<mario_obj>>();
	public mario_qtree m_qt = new mario_qtree();
	public mario_point m_roll = new mario_point();
	private mario_point m_grid = new mario_point();
	private int m_y_roll_target = 0;
	public mario_obj m_main_char;
	private mario_scene m_scene;
	private int m_left = 0;
	private int m_left_time = 0;
	private int m_right = 0;
	private int m_right_time = 0;
	private int m_shache_type = 0;
	public int m_shache_time = 0;
	private bool m_jump = false;
	private bool m_sjump = false;
	private List<int> m_inputs = new List<int> ();
	private List<int> m_good_inputs = new List<int> ();
	private bool m_zr;
	// 是否踩死怪
	private bool m_caisi = false;
	private int m_caisi_y = 0;
	public int m_paqiang = 0;
	private bool m_paqiang_jump = false;
	private int m_jump_state = 0;
	public int m_jump_num = 0;
	private int m_jump_num_time = 0;
	private int m_jump_x = 0;
	private bool m_chuan = false;
	public int m_time = 0;
	public int m_total_time = 0;
	private int m_mode = 0;
	private int m_rindex = 0;
	public int m_state = 0;
	public GameObject m_render;
	private mario_point m_qpos;
	private List< HashSet<int> > m_has_create = new List< HashSet<int> >();
	public bool m_pause;
	public mario_point m_die_pos = new mario_point();
	public int m_score = 0;
	public int m_ys = 0;
	private int m_show_cha = 0;
	private int m_show_cha_time = 0;
	private bool m_has_time_tx = false;
	private int m_time_tx_time = 0;
	private int m_zhuiji_x = 0;
	private GameObject m_zhuiji;
	private List<mario_obj> bobjs = new List<mario_obj> ();
	private List<mario_obj> tobjs = new List<mario_obj> ();
	private HashSet<int> m_need_calc = new HashSet<int> ();
	private List< Dictionary<int, List<mario_obj> > > m_delete_objs = new List< Dictionary<int, List<mario_obj> > >();
	public int m_world = 0;
	private mario_obj m_csm;

	public static play_mode _instance;
	void Awake()
	{
		_instance = this;
	}

	public void reload()
	{
		m_qpos = null;
		reset (0);
	}

	public void reload(mario_point qpos, int world, int mode)
	{
		m_qpos = qpos;
		m_mode = mode;
		reset (world);
	}

	public void reload_self(mario_point qpos, int world)
	{
		m_qpos = qpos;
		reset (world, false);
	}

	public void reset(int world, bool all = true)
	{
		m_world = world;
		if (all)
		{
			if (m_mode == 0)
			{
				m_inputs = new List<int> ();
				if (m_good_inputs.Count > 0)
				{
					m_inputs.AddRange(m_good_inputs);
				}
			}
			else
			{
				m_inputs = game_data._instance.m_map_inputs;
			}
			m_has_create.Clear ();
			m_delete_objs.Clear ();
			for (int i = 0; i < 3; ++i)
			{
				m_has_create.Add(new HashSet<int>());
				m_delete_objs.Add(new Dictionary<int, List<mario_obj>>());
			}
			m_now_inputs.Clear();
			m_per_inputs.Clear();
			for (int i = 0; i < 4; ++i)
			{
				m_now_inputs.Add(false);
				m_per_inputs.Add(false);
			}
			m_rindex = 0;
			m_time = 0;
			m_total_time = 0;
			m_score = 0;
			m_ys = 0;
			m_need_calc.Clear ();
			mario._instance.remove_child (m_main);
			mario._instance.remove_child (m_shadow);
			mario._instance.remove_child (m_other);
			m_left = 0;
			m_right = 0;
			m_has_time_tx = false;
			m_time_tx_time = 0;
		}

		m_mobjs.Clear ();
		m_aobjs.Clear ();
		for (int i = (int)mario_obj.mario_type.mt_null; i < (int)mario_obj.mario_type.mt_end; ++i)
		{
			m_aobjs.Add(new List<mario_obj>());
		}
		m_xyobjs.Clear ();
		for (int j = 0; j < game_data._instance.m_map_data.maps[m_world].y_num; ++j)
		{
			List<mario_obj> xyobjs = new List<mario_obj>();
			for (int i = 0; i < game_data._instance.m_map_data.maps[m_world].x_num; ++i)
			{
				xyobjs.Add(null);
			}
			m_xyobjs.Add(xyobjs);
		}
		m_qt = new mario_qtree ();
		if (m_zhuiji)
		{
			UnityEngine.Object.Destroy(m_zhuiji);
			m_zhuiji = null;
		}
		m_roll = new mario_point();
		m_y_roll_target = 0;
		m_main_char = null;
		m_left_time = 0;
		m_right_time = 0;
		m_shache_type = 0;
		m_shache_time = 0;
		m_jump = false;
		m_sjump = false;
		m_caisi = false;
		m_paqiang = 0;
		m_paqiang_jump = false;
		m_jump_state = 0;
		m_jump_num = 0;
		m_jump_num_time = 0;
		m_chuan = false;
		m_show_cha = 0;
		m_show_cha_time = 0;
		m_state = 0;
		m_pause = false;
		m_grid = new mario_point();
		m_csm = null;
		m_zr = false;

		int cx = 1;
		int cy = game_data._instance.m_map_data.maps[m_world].qd_y;
		if (m_qpos != null)
		{
			cx = m_qpos.x / utils.g_grid_size;
			cy = m_qpos.y / utils.g_grid_size;
		}
		if (cy < 0)
		{
			cy = 0;
		}
		else if (cy >= game_data._instance.m_map_data.maps[m_world].y_num)
		{
			cy = game_data._instance.m_map_data.maps[m_world].y_num - 1;
		}
		m_main_char = create_mario_obj("mario_main", null, new List<int>(), cx, cy);

		m_roll.x = m_main_char.m_pos.x;
		m_roll.y = m_main_char.m_pos.y - utils.g_roll_y;
		m_y_roll_target = m_roll.y;
		if (m_roll.y + utils.g_height > game_data._instance.m_map_data.maps[m_world].y_num * utils.g_grid_size)
		{
			m_roll.y = game_data._instance.m_map_data.maps[m_world].y_num * utils.g_grid_size - utils.g_height;
		}
		if (m_roll.y < 0)
		{
			m_roll.y = 0;
		}

		m_grid.x = m_roll.x / utils.g_grid_size;
		m_grid.y = (m_roll.y + utils.g_roll_y) / utils.g_grid_size;
		for (int i = m_grid.x - utils.g_start_x; i <= m_grid.x + utils.g_start_x; ++i)
		{
			if (i < 0 || i >= game_data._instance.m_map_data.maps[m_world].x_num)
			{
				continue;
			}
			for (int j = m_grid.y - utils.g_start_y; j <= m_grid.y + utils.g_start_y; ++j)
			{
				if (j < 0 || j >= game_data._instance.m_map_data.maps[m_world].y_num)
				{
					continue;
				}
				do_create (i, j);
			}
		}

		create_scene ();

		if (game_data._instance.m_map_data.mode != 0)
		{
			m_zhuiji_x = m_main_char.m_pos.x - utils.g_grid_size * 5;
			GameObject res = (GameObject)Resources.Load ("unit/other/zhuiji");
			m_zhuiji = (GameObject)Instantiate(res);
			m_zhuiji.transform.parent = m_other.transform;
			m_zhuiji.transform.localPosition = new Vector3 (m_zhuiji_x / 10, 0, 0);
			m_zhuiji.transform.localScale = new Vector3 (1, 1, 1);
		}
		
		update_ex ();
		check_create ();
	}

	public void create_scene()
	{
		mario._instance.remove_child (m_back);
		string cname = "scene/" + game_data._instance.m_map_data.maps[m_world].map_theme.ToString() + "/scene";
		GameObject res = (GameObject)Resources.Load (cname);
		GameObject obj = (GameObject)Instantiate(res);
		obj.transform.parent = m_back.transform;
		obj.transform.localPosition = new Vector3 (0, 0, 0);
		obj.transform.localScale = new Vector3 (1, 1, 1);
		m_scene = obj.GetComponent<mario_scene> ();
	}

	public mario_obj create_mario_obj(string name, s_t_unit unit, List<int> param, int x, int y)
	{
		return create_mario_obj_ex(name, unit, param, x, y, utils.g_grid_size * x + utils.g_grid_size / 2, utils.g_grid_size * y + utils.g_grid_size / 2);
	}

	public mario_obj create_mario_obj_ex(string name, s_t_unit unit, List<int> param, int x, int y, int xx, int yy)
	{
		string cname = "unit/" + name + "/" + name;
		if (unit != null && unit.kfg == 1)
		{
			cname = "unit/" + name + "/" + game_data._instance.m_map_data.maps[m_world].map_theme.ToString() + "/" + name;
		}
		GameObject res = (GameObject)Resources.Load (cname);
		GameObject obj = (GameObject)Instantiate(res);
		obj.transform.parent = m_main.transform;
		obj.transform.localScale = new Vector3 (1, 1, 1);
		mario_obj mobj = obj.GetComponent<mario_obj> ();
		mobj.m_unit = unit;
		Transform fx = obj.transform.FindChild("fx");
		if (fx != null)
		{
			fx.gameObject.SetActive (false);
		}
		mobj.init (name, param, m_world, x, y, xx, yy);
		add_obj(mobj);
		mobj.create_shadow (m_shadow);
		mobj.m_is_new = true;
		mobj.check_is_start(m_grid);
		
		return mobj;
	}

	void add_obj(mario_obj obj)
	{
		m_mobjs.Add (obj);
		if (obj.m_init_pos.x != -1)
		{
			m_xyobjs[obj.m_init_pos.y][obj.m_init_pos.x] = obj;
		}
		if (obj.m_type == mario_obj.mario_type.mt_null)
		{
			return;
		}
		m_aobjs [(int)obj.m_type].Add (obj);
		m_qt.insert (obj);
	}

	void remove_obj(mario_obj obj)
	{
		m_mobjs.Remove (obj);
		if (obj.m_init_pos.x != -1)
		{
			m_xyobjs[obj.m_init_pos.y][obj.m_init_pos.x] = null;
		}
		if (obj.m_type == mario_obj.mario_type.mt_null)
		{
			return;
		}
		m_aobjs [(int)obj.m_type].Remove (obj);
		if (obj.m_qtree != null)
		{
			obj.m_qtree.remove(obj);
			obj.m_qtree = null;
		}
	}

	mario_obj get_obj(int x, int y)
	{
		if (x < 0 || x >= game_data._instance.m_map_data.maps[m_world].x_num || y < 0 || y >= game_data._instance.m_map_data.maps[m_world].y_num)
		{
			return null;
		}
		return m_xyobjs[y][x];
	}

	public void refresh_obj(mario_obj obj)
	{
		if (obj.m_type == mario_obj.mario_type.mt_null)
		{
			return;
		}
		obj.reset_bound ();
		if (obj.m_pos.x != obj.m_per_pos.x || obj.m_pos.y != obj.m_per_pos.y)
		{
			if (obj.m_qtree != null)
			{
				obj.m_qtree.remove(obj);
				obj.m_qtree = null;
			}
			m_qt.insert (obj);
		}
	}

	void check_input()
	{
		bool lx = false;
		if (m_good_inputs.Count > 0 && !m_zr)
		{
			if (m_good_inputs[m_good_inputs.Count - 2] >= m_time)
			{
				lx = true;
			}
		}
		if (m_mode == 0 && !lx)
		{
			if (m_now_inputs[0] && !m_per_inputs[0])
			{
				m_inputs.Add(m_time);
				m_inputs.Add(0);
				if (m_left_time > 0)
				{
					m_left = 2;
				}
				else
				{
					m_left = 1;
				}
				m_left_time = 15;
			}
			if (!m_now_inputs[0] && m_per_inputs[0])
			{
				m_inputs.Add(m_time);
				m_inputs.Add(1);
				m_left = 0;
				m_shache_time = 0;
			}
			if (m_now_inputs[1] && !m_per_inputs[1])
			{
				m_inputs.Add(m_time);
				m_inputs.Add(2);
				if (m_right_time > 0)
				{
					m_right = 2;
				}
				else
				{
					m_right = 1;
				}
				m_right_time = 15;
			} 
			if (!m_now_inputs[1] && m_per_inputs[1])
			{
				m_inputs.Add(m_time);
				m_inputs.Add(3);
				m_right = 0;
				m_shache_time = 0;
			}
			if (m_now_inputs[2] && !m_per_inputs[2])
			{
				m_inputs.Add(m_time);
				m_inputs.Add(4);
				m_jump = true;
			}
			if (!m_now_inputs[2] && m_per_inputs[2])
			{
				m_inputs.Add(m_time);
				m_inputs.Add(5);
				m_jump = false;
			}
			if (m_now_inputs[3] && !m_per_inputs[3])
			{
				m_inputs.Add(m_time);
				m_inputs.Add(6);
				m_chuan = true;
			}
			if (!m_now_inputs[3] && m_per_inputs[3])
			{
				m_inputs.Add(m_time);
				m_inputs.Add(7);
				m_chuan = false;
			}
		}
		else
		{
			while (m_rindex < m_inputs.Count && m_inputs[m_rindex] == m_time)
			{
				m_rindex++;
				if (m_inputs[m_rindex] == 0)
				{
					if (m_left_time > 0)
					{
						m_left = 2;
					}
					else
					{
						m_left = 1;
					}
					m_left_time = 15;
				}
				if (m_inputs[m_rindex] == 1)
				{
					m_left = 0;
					m_shache_time = 0;
				}
				if (m_inputs[m_rindex] == 2)
				{
					if (m_right_time > 0)
					{
						m_right = 2;
					}
					else
					{
						m_right = 1;
					}
					m_right_time = 15;
				} 
				if (m_inputs[m_rindex] == 3)
				{
					m_right = 0;
					m_shache_time = 0;
				}
				if (m_inputs[m_rindex] == 4)
				{
					m_jump = true;
				}
				if (m_inputs[m_rindex] == 5)
				{
					m_jump = false;
				}
				if (m_inputs[m_rindex] == 6)
				{
					m_chuan = true;
				}
				if (m_inputs[m_rindex] == 7)
				{
					m_chuan = false;
				}
				m_rindex++;
			}
		}
		for (int i = 0; i < m_now_inputs.Count; ++i)
		{
			m_per_inputs[i] = m_now_inputs[i];
		}

		if (m_main_char != null && m_state == 0 && !m_main_char.is_die() && m_main_char.m_no_mc_time == 0)
		{
			if (m_left > 0)
			{
				m_main_char.set_fx(mario_obj.mario_fx.mf_left);
			}
			else if (m_right > 0)
			{
				m_main_char.set_fx(mario_obj.mario_fx.mf_right);
			}
		}

		if (m_chuan && m_csm != null)
		{
			int w = 0;
			int x = 0;
			int y = 0;
			if (game_data._instance.get_csm(m_world, m_csm.m_init_pos.x, m_csm.m_init_pos.y, ref w, ref x, ref y))
			{
				m_pause = true;
				m_csm.GetComponent<mario_obj>().play_anim("chuan");
				mario._instance.play_sound ("sound/men");
				mario._instance.show_play_mask(delegate() { 
					int yin_time = m_main_char.GetComponent<mario_main>().m_yin_time;
					int big = m_main_char.GetComponent<mario_main>().m_big;
					for (int i = 0; i < m_mobjs.Count; ++i)
					{
						m_mobjs[i].m_is_destory = 2;
					}
					check_delete();
					if (w != m_world)
					{
						if (m_has_time_tx)
						{
							mario._instance.play_mus(game_data._instance.get_map_music(w), true, 1 - game_data._instance.m_map_data.no_music, 1.5f);
						}
						else
						{
							mario._instance.play_mus(game_data._instance.get_map_music(w), true, 1 - game_data._instance.m_map_data.no_music, 1.0f);
						}
					}
					reload_self(new mario_point(x * utils.g_grid_size, y * utils.g_grid_size), w);
					m_main_char.GetComponent<mario_main>().set_cs(big, yin_time);
				});
			}
		}
		m_chuan = false;

		m_paqiang_jump = false;
		if (m_paqiang > 0 && m_jump)
		{
			m_paqiang_jump = true;
			m_main_char.m_velocity.y = 0;
			m_main_char.m_pvelocity.y = 0;
			m_jump_state = 100;
			m_jump_num = 0;
			if (m_paqiang == 1)
			{
				m_main_char.m_velocity.x = 120;
				m_main_char.set_fx(mario_obj.mario_fx.mf_right);
				play_mode._instance.effect("hit", m_main_char.m_pos.x - 310, m_main_char.m_pos.y - utils.g_grid_size / 2);
			}
			else
			{
				m_main_char.m_velocity.x = -120;
				m_main_char.set_fx(mario_obj.mario_fx.mf_left);
				play_mode._instance.effect("hit", m_main_char.m_pos.x + 310, m_main_char.m_pos.y - utils.g_grid_size / 2);
			}
			m_main_char.m_no_mc_time = 20;
			mario._instance.play_sound_ex ("sound/yo");
			mario._instance.play_sound ("sound/jump", 0.5f);
			m_sjump = false;
		}
		else if (m_caisi && m_jump)
		{
			m_main_char.m_velocity.y = 0;
			m_main_char.m_pvelocity.y = m_caisi_y;
			m_jump_state = 100;
			m_jump_num = 0;
			m_jump_x = m_main_char.m_velocity.x > 0 ? m_main_char.m_velocity.x : -m_main_char.m_velocity.x;
			float vol = 1;
			if (m_main_char.m_velocity.x < -80 || m_main_char.m_velocity.x > 80)
			{
				m_jump_num++;
				m_jump_num_time = 10;
				if (m_jump_num == 2)
				{
					mario._instance.play_sound_ex ("sound/ya");
					vol = 0.5f;
				}
				else if (m_jump_num == 3)
				{
					mario._instance.play_sound_ex ("sound/yaho");
					vol = 0.5f;
				}
				if (m_jump_num >= 4)
				{
					m_jump_num = 1;
				}
			}
			else
			{
				m_jump_num = 0;
				m_jump_num_time = 0;
			}
			mario._instance.play_sound ("sound/jump", vol);
			m_sjump = false;
		}
		else if (m_jump_state == 0 && m_jump)
		{
			m_jump_state = 100;
			m_jump_x = m_main_char.m_velocity.x > 0 ? m_main_char.m_velocity.x : -m_main_char.m_velocity.x;
			float vol = 1;
			if (m_main_char.m_velocity.x < -80 || m_main_char.m_velocity.x > 80)
			{
				m_jump_num++;
				m_jump_num_time = 10;
				if (m_jump_num == 2)
				{
					mario._instance.play_sound_ex ("sound/ya");
					vol = 0.5f;
				}
				else if (m_jump_num == 3)
				{
					mario._instance.play_sound_ex ("sound/yaho");
					vol = 0.5f;
				}
				if (m_jump_num >= 4)
				{
					m_jump_num = 1;
				}
			}
			else
			{
				m_jump_num = 0;
				m_jump_num_time = 0;
			}
			mario._instance.play_sound ("sound/jump", vol);
			m_sjump = false;
		}
		if (m_main_char.m_pvelocity.y < 0)
		{
			m_jump_state = 1;
			m_jump = false;
		}
		if (m_jump_state >= 100 && !m_jump)
		{
			m_sjump = true;
		}
		int n = m_jump_num - 1;
		if (n < 0)
		{
			n = 0;
		}
		if (m_jump_state >= 102)
		{
			m_jump_state += 1;

			m_main_char.m_velocity.y += 10 + n;
		}
		else if (m_jump_state >= 100)
		{
			m_jump_state += 1;
			m_main_char.m_velocity.y += 90 + m_jump_x / 20;
		}
		if (m_jump_state > 120 + n * 2)
		{
			m_jump_state = 1;
			m_jump = false;
		}
		if (m_jump_state >= 102 && m_sjump)
		{
			m_jump_state = 1;
		}
	}

	void check_zhli()
	{
		/// 站立状态
		for (int k = 0; k < m_aobjs[(int)mario_obj.mario_type.mt_charater].Count; ++k)
		{
			mario_obj mo1 = m_aobjs[(int)mario_obj.mario_type.mt_charater][k];
			if (mo1.is_die() || !mo1.m_is_start)
			{
				continue;
			}
			bool bflag = false;
			int mc = 0;
			if (mo1 == m_main_char && m_jump_state >= 100)
			{
				bflag = true;
			}
			if (!bflag && mo1.m_velocity.y != 0)
			{
				bflag = true;
			}
			if (!bflag)
			{
				bool flag = false;
				bobjs.Clear();
				mo1.m_is_on_char = false;
				m_qt.retrive_floor(mo1, ref bobjs);
				for (int i = 0; i < bobjs.Count; ++i)
				{
					mario_obj mo2 = bobjs[i];
					if (mo1 == mo2 || mo2.is_die() || mo2.m_main)
					{
						continue;
					}
					if (mo1.m_main && !mo2.m_has_main_floor)
					{
						continue;
					}
					if (mo1.check_on_floor(mo2))
					{
						flag = true;
						mo1.m_is_on_floor = true;
						mo2.m_nl_objs.Add(mo1);
						mo1.m_bnl_objs.Add(mo2);
						if (mo2.m_can_be_on_char)
						{
							mo1.m_is_on_char = true;
						}
						if (mo1 == m_main_char)
						{
							m_jump_state = 0;
						}
						if (mo2.m_mocali > mc)
						{
							mc = mo2.m_mocali;
						}
					}
				}
				if (!flag)
				{
					bflag = true;
				}
			}
			if (bflag)
			{
				if (mo1.m_is_on_floor)
				{
					mo1.m_velocity.x += mo1.m_per_nl_point.x;
					mo1.m_velocity.y += mo1.m_per_nl_point.y;
					mo1.m_is_on_floor = false;
				}
			}
			if (mo1.m_main)
			{
				if (bflag)
				{
					m_shache_time = 0;
					if (m_main_char.m_no_mc_time > 0)
					{
					}
					else if (m_left == 1)
					{
						m_main_char.m_is_move = true;
						if (m_main_char.m_velocity.x <= -84)
						{
							m_main_char.m_velocity.x += 4;
						}
						else if (m_main_char.m_velocity.x <= -80)
						{
							m_main_char.m_velocity.x = -80;
						}
						else if (m_main_char.m_velocity.x <= 0)
						{
							m_main_char.m_velocity.x -= 2;
						}
						else
						{
							m_main_char.m_velocity.x -= 10;
						}
					}
					else if (m_left == 2)
					{
						m_main_char.m_is_move = true;
						if (m_main_char.m_velocity.x <= -164)
						{
							m_main_char.m_velocity.x += 4;
						}
						else if (m_main_char.m_velocity.x <= -160)
						{
							m_main_char.m_velocity.x = -160;
						}
						else if (m_main_char.m_velocity.x <= 0)
						{
							if (m_main_char.m_velocity.x <= -80)
							{
								m_main_char.m_velocity.x -= 1;
							}
							else
							{
								m_main_char.m_velocity.x -= 3;
							}
						}
						else
						{
							m_main_char.m_velocity.x -= 12;
						}
					}
					else if (m_right == 1)
					{
						m_main_char.m_is_move = true;
						if (m_main_char.m_velocity.x >= 84)
						{
							m_main_char.m_velocity.x -= 4;
						}
						else if (m_main_char.m_velocity.x >= 80)
						{
							m_main_char.m_velocity.x = 80;
						}
						else if (m_main_char.m_velocity.x >= 0)
						{
							m_main_char.m_velocity.x += 2;
						}
						else
						{
							m_main_char.m_velocity.x += 10;
						}
					}
					else if (m_right == 2)
					{
						m_main_char.m_is_move = true;
						if (m_main_char.m_velocity.x >= 164)
						{
							m_main_char.m_velocity.x -= 164;
						}
						else if (m_main_char.m_velocity.x >= 160)
						{
							m_main_char.m_velocity.x = 160;
						}
						else if (m_main_char.m_velocity.x >= 0)
						{
							if (m_main_char.m_velocity.x > 80)
							{
								m_main_char.m_velocity.x += 1;
							}
							else
							{
								m_main_char.m_velocity.x += 3;
							}
						}
						else
						{
							m_main_char.m_velocity.x += 12;
						}
					}
				}
				else
				{
					if (m_main_char.m_no_mc_time > 0)
					{
					}
					else if (m_left == 1)
					{
						m_main_char.m_is_move = true;
						if (m_shache_type == 0 && m_shache_time > 0)
						{
							m_shache_time--;
							if (m_main_char.m_velocity.x - mc * 3 / 2 < 0)
							{
								m_main_char.m_velocity.x = 0;
							}
							else
							{
								m_main_char.m_velocity.x -= mc * 3 / 2;
								m_shache_time = 10;
							}
						}
						else if (m_main_char.m_velocity.x < -80 - mc / 2)
						{
							m_main_char.m_velocity.x += mc / 2;
						}
						else if (m_main_char.m_velocity.x <= -80)
						{
							m_main_char.m_velocity.x = -80;
						}
						else if (m_main_char.m_velocity.x <= 0)
						{
							m_main_char.m_velocity.x -= 3;
						}
						else if (m_main_char.m_velocity.x >= 100)
						{
							m_shache_type = 0;
							m_shache_time = 5;
						}
						else if (m_main_char.m_velocity.x - mc * 2 < 0)
						{
							m_main_char.m_velocity.x = 0;
						}
						else
						{
							m_main_char.m_velocity.x -= mc * 2;
						}
					}
					else if (m_left == 2)
					{
						m_main_char.m_is_move = true;
						if (m_shache_type == 0 && m_shache_time > 0)
						{
							m_shache_time--;
							if (m_main_char.m_velocity.x - mc * 3 / 2 < 0)
							{
								m_main_char.m_velocity.x = 0;
							}
							else
							{
								m_main_char.m_velocity.x -= mc * 3 / 2;
								m_shache_time = 10;
							}
						}
						else if (m_main_char.m_velocity.x < -160 - mc / 2)
						{
							m_main_char.m_velocity.x += mc / 2;
						}
						else if (m_main_char.m_velocity.x <= -160)
						{
							m_main_char.m_velocity.x = -160;
						}
						else if (m_main_char.m_velocity.x <= 0)
						{
							if (m_main_char.m_velocity.x < -80)
							{
								m_main_char.m_velocity.x -= 1;
							}
							else
							{
								m_main_char.m_velocity.x -= 3;
							}
						}
						else if (m_main_char.m_velocity.x >= 100)
						{
							m_shache_type = 0;
							m_shache_time = 5;
						}
						else if (m_main_char.m_velocity.x - mc * 3 < 0)
						{
							m_main_char.m_velocity.x = 0;
						}
						else
						{
							m_main_char.m_velocity.x -= mc * 3;
						}
					}
					else if (m_right == 1)
					{
						m_main_char.m_is_move = true;
						if (m_shache_type == 1 && m_shache_time > 0)
						{
							m_shache_time--;
							if (m_main_char.m_velocity.x + mc * 3 / 2 > 0)
							{
								m_main_char.m_velocity.x = 0;
							}
							else
							{
								m_main_char.m_velocity.x += mc * 3 / 2;
								m_shache_time = 10;
							}
						}
						else if (m_main_char.m_velocity.x > 80 + mc / 2)
						{
							m_main_char.m_velocity.x -= mc / 2;
						}
						else if (m_main_char.m_velocity.x >= 80)
						{
							m_main_char.m_velocity.x = 80;
						}
						else if (m_main_char.m_velocity.x >= 0)
						{
							m_main_char.m_velocity.x += 3;
						}
						else if (m_main_char.m_velocity.x <= -100)
						{
							m_shache_type = 1;
							m_shache_time = 5;
						}
						else if (m_main_char.m_velocity.x + mc * 2 > 0)
						{
							m_main_char.m_velocity.x = 0;
						}
						else
						{
							m_main_char.m_velocity.x += mc * 2;
						}
					}
					else if (m_right == 2)
					{
						m_main_char.m_is_move = true;
						if (m_shache_type == 1 && m_shache_time > 0)
						{
							m_shache_time--;
							if (m_main_char.m_velocity.x + mc * 3 / 2 > 0)
							{
								m_main_char.m_velocity.x = 0;
							}
							else
							{
								m_main_char.m_velocity.x += mc * 3 / 2;
								m_shache_time = 10;
							}
						}
						else if (m_main_char.m_velocity.x > 160 + mc / 2)
						{
							m_main_char.m_velocity.x -= mc / 2;
						}
						else if (m_main_char.m_velocity.x >= 160)
						{
							m_main_char.m_velocity.x = 160;
						}
						else if (m_main_char.m_velocity.x >= 0)
						{
							if (m_main_char.m_velocity.x > 80)
							{
								m_main_char.m_velocity.x += 1;
							}
							else
							{
								m_main_char.m_velocity.x += 3;
							}
						}
						else if (m_main_char.m_velocity.x <= -100)
						{
							m_shache_type = 1;
							m_shache_time = 5;
						}
						else if (m_main_char.m_velocity.x + mc * 3 > 0)
						{
							m_main_char.m_velocity.x = 0;
						}
						else
						{
							m_main_char.m_velocity.x += mc * 3;
						}
					}
					else if (m_main_char.m_velocity.x < 0)
					{
						if (m_main_char.m_velocity.x >= -15)
						{
							m_main_char.m_velocity.x += 1;
						}
						else
						{
							m_main_char.m_velocity.x += mc / 2;
						}
					}
					else if (m_main_char.m_velocity.x > 0)
					{
						if (m_main_char.m_velocity.x <= 15)
						{
							m_main_char.m_velocity.x -= 1;
						}
						else
						{
							m_main_char.m_velocity.x -= mc / 2;
						}
					}
					if (m_main_char.m_velocity.x == 0 && m_shache_time == 0)
					{
						m_main_char.m_is_move = false;
					}
				}

				if (m_main_char.m_is_on_floor && !m_main_char.m_per_is_on_floor)
				{
					if (m_right == 0 && m_main_char.m_velocity.x < 0)
					{
						m_main_char.set_fx(mario_obj.mario_fx.mf_left);
					}
					else if (m_left == 0 && m_main_char.m_velocity.x > 0)
					{
						m_main_char.set_fx(mario_obj.mario_fx.mf_right);
					}
				}
			}
			else
			{
				if (!mo1.m_per_is_on_char && mo1.m_is_on_char && !mo1.m_wgk)
				{
					mo1.m_velocity.set (0, 0);
				}
				if (!mo1.m_is_on_char || mo1.m_wgk)
				{
					if (mo1.m_no_mc_time > 0)
					{
					}
					else if (mo1.m_fx == mario_obj.mario_fx.mf_right)
					{
						if (mo1.m_velocity.x > mo1.m_min_speed + mc)
						{
							mo1.m_velocity.x -= mc;
						}
						else if (mo1.m_velocity.x < mo1.m_min_speed - mc)
						{
							mo1.m_velocity.x += mc;
						}
						else
						{
							mo1.m_velocity.x = mo1.m_min_speed;
						}
					}
					else
					{
						if (mo1.m_velocity.x < -mo1.m_min_speed - mc)
						{
							mo1.m_velocity.x += mc;
						}
						else if (mo1.m_velocity.x > -mo1.m_min_speed + mc)
						{
							mo1.m_velocity.x -= mc;
						}
						else
						{
							mo1.m_velocity.x = -mo1.m_min_speed;
						}
					}
				}
			}
		}
	}

	void check_paqiang()
	{
		m_paqiang = 0;
		if (!m_main_char.m_is_on_floor && m_main_char.m_velocity.y < 0)
		{
			if (m_left > 0)
			{
				bobjs.Clear();
				m_qt.retrive_left(m_main_char, ref bobjs);
				for (int i = 0; i < bobjs.Count; ++i)
				{
					mario_obj mo2 = bobjs[i];
					if (m_main_char == mo2 || mo2.is_die() || mo2.m_main)
					{
						continue;
					}
					if (mo2.m_unit != null)
					{
						mario_obj tobj = get_obj(mo2.m_init_pos.x + 1, mo2.m_init_pos.y - 1);
						if (tobj != null && tobj.m_type == mario_obj.mario_type.mt_block)
						{
							continue;
						}
						tobj = get_obj(mo2.m_init_pos.x + 1, mo2.m_init_pos.y - 2);
						if (tobj != null && tobj.m_type == mario_obj.mario_type.mt_block)
						{
							continue;
						}
					}
					if (m_main_char.check_left_floor(mo2))
					{
						m_paqiang = 1;
						break;
					}
				}
			}
			else if (m_right > 0)
			{
				bobjs.Clear();
				m_qt.retrive_right(m_main_char, ref bobjs);
				for (int i = 0; i < bobjs.Count; ++i)
				{
					mario_obj mo2 = bobjs[i];
					if (m_main_char == mo2 || mo2.is_die() || mo2.m_main)
					{
						continue;
					}
					if (mo2.m_unit != null)
					{
						mario_obj tobj = get_obj(mo2.m_init_pos.x - 1, mo2.m_init_pos.y - 1);
						if (tobj != null && tobj.m_type == mario_obj.mario_type.mt_block)
						{
							continue;
						}
						tobj = get_obj(mo2.m_init_pos.x - 1, mo2.m_init_pos.y - 2);
						if (tobj != null && tobj.m_type == mario_obj.mario_type.mt_block)
						{
							continue;
						}
					}
					if (m_main_char.check_right_floor(mo2))
					{
						m_paqiang = 2;
						break;
					}
				}
			}
		}
	}

	void check_state()
	{
		m_caisi = false;

		/// 预速度
		for (int k = 0; k < m_aobjs[(int)mario_obj.mario_type.mt_charater].Count; ++k)
		{
			mario_obj mo1 = m_aobjs[(int)mario_obj.mario_type.mt_charater][k];
			mo1.m_velocity.x += mo1.m_pvelocity.x;
			mo1.m_velocity.y += mo1.m_pvelocity.y;
			mo1.m_pvelocity = new mario_point();
		}

		check_zhli ();
		check_paqiang ();

		/// 自由落体
		for (int k = 0; k < m_aobjs[(int)mario_obj.mario_type.mt_charater].Count; ++k)
		{
			mario_obj mo1 = m_aobjs[(int)mario_obj.mario_type.mt_charater][k];
			if (!mo1.m_is_on_floor && !mo1.is_die() && mo1.m_is_start)
			{
				if (mo1.m_main && m_paqiang > 0)
				{
					if (mo1.m_velocity.y > -70)
					{
						mo1.m_velocity.y -= 10;
					}
					else if (mo1.m_velocity.y < -90)
					{
						mo1.m_velocity.y += 10;
					}
					else
					{
						mo1.m_velocity.y = -80;
					}
				}
				else
				{
					mo1.m_velocity.y -= 15;
					if (mo1.m_velocity.y < -160)
					{
						mo1.m_velocity.y = -160;
					}
				}
			}
		}

		/// 静态处理移动
		for (int k = 0; k < m_aobjs[(int)mario_obj.mario_type.mt_block].Count; ++k)
		{
			mario_obj mo1 = m_aobjs[(int)mario_obj.mario_type.mt_block][k];
			mo1.do_move();
		}
		for (int k = 0; k < m_aobjs[(int)mario_obj.mario_type.mt_block1].Count; ++k)
		{
			mario_obj mo1 = m_aobjs[(int)mario_obj.mario_type.mt_block1][k];
			mo1.do_move();
		}

		/// 处理粘连移动
		for (int k = 0; k < m_aobjs[(int)mario_obj.mario_type.mt_charater].Count; ++k)
		{
			mario_obj mo1 = m_aobjs[(int)mario_obj.mario_type.mt_charater][k];
			mo1.m_nl_point = new mario_point();
			if (mo1.m_bnl_objs.Count > 0)
			{
				int xx1 = 0;
				int xx2 = 0;
				int yy = -999999;
				for (int i = 0; i < mo1.m_bnl_objs.Count; ++i)
				{
					mario_point p = mo1.m_bnl_objs[i].get_nl_calc_point();
					if (p.x > 0 && p.x > xx1)
					{
						xx1 = p.x;
					}
					if (p.x < 0 && p.x < xx2)
					{
						xx2 = p.x;
					}
					if (p.y > yy)
					{
						yy = p.y;
					}
				}
				int xx = xx1 + xx2;
				if (mo1.m_no_mc_time > 0)
				{
					xx = 0;
				}
				mo1.m_nl_point.set (xx, yy);
				if (!mo1.m_per_is_on_floor && (mo1.m_main || !mo1.m_is_on_char))
				{
					mo1.m_velocity.x -= xx;
				}
			}
			mo1.m_per_nl_point.set(mo1.m_nl_point.x, mo1.m_nl_point.y);
			if (mo1.m_per_nl_point.y < 0)
			{
				mo1.m_per_nl_point.y = 0;
			}
		}

		/// 位置
		for (int k = 0; k < m_mobjs.Count; ++k)
		{
			mario_obj mo1 = m_mobjs[k];
			if (!mo1.is_die() && !mo1.m_is_start)
			{
				continue;
			}
			int vx = mo1.m_velocity.x + mo1.m_nl_point.x;
			int vy = mo1.m_velocity.y + mo1.m_nl_point.y;
			if (vx != 0 || vy != 0)
			{
				mo1.m_pos.x += vx;
				mo1.m_pos.y += vy;
			}
			refresh_obj(mo1);
		}

		// 碰撞
		for (int k = 0; k < m_aobjs[(int)mario_obj.mario_type.mt_charater].Count; ++k)
		{
			mario_obj mo1 = m_aobjs[(int)mario_obj.mario_type.mt_charater][k];
			if (!mo1.m_main && !mo1.m_wgk)
			{
				check_hit (mo1, mario_obj.mario_type.mt_charater);
				check_hit (mo1, mario_obj.mario_type.mt_block1);
				check_hit (mo1, mario_obj.mario_type.mt_block);
				check_hit (mo1, mario_obj.mario_type.mt_attack);
			}
		}
		for (int k = 0; k < m_aobjs[(int)mario_obj.mario_type.mt_charater].Count; ++k)
		{
			mario_obj mo1 = m_aobjs[(int)mario_obj.mario_type.mt_charater][k];
			if (mo1.m_wgk)
			{
				check_hit (mo1, mario_obj.mario_type.mt_charater);
				check_hit (mo1, mario_obj.mario_type.mt_block1);
				check_hit (mo1, mario_obj.mario_type.mt_block);
				check_hit (mo1, mario_obj.mario_type.mt_attack);
			}
		}
		for (int k = 0; k < m_aobjs[(int)mario_obj.mario_type.mt_charater].Count; ++k)
		{
			mario_obj mo1 = m_aobjs[(int)mario_obj.mario_type.mt_charater][k];
			if (mo1.m_wgk)
			{
				check_hit (mo1, mario_obj.mario_type.mt_charater, false ,true);
			}
		}
		if (m_main_char != null)
		{
			check_hit (m_main_char, mario_obj.mario_type.mt_charater);
			check_hit (m_main_char, mario_obj.mario_type.mt_block1);
			check_hit (m_main_char, mario_obj.mario_type.mt_block);
			check_hit (m_main_char, mario_obj.mario_type.mt_attack);
			check_hit (m_main_char, mario_obj.mario_type.mt_attack_ex);
			check_hit (m_main_char, mario_obj.mario_type.mt_attack_ex1);
		}

		for (int k = 0; k < m_aobjs[(int)mario_obj.mario_type.mt_attack_ex1].Count; ++k)
		{
			mario_obj mo1 = m_aobjs[(int)mario_obj.mario_type.mt_attack_ex1][k];
			check_hit (mo1, mario_obj.mario_type.mt_block, false);
		}

		/// 主角
		{
			int vx = m_main_char.m_velocity.x + m_main_char.m_nl_point.x;
			if (vx < 0 && m_main_char.m_pos.x + vx < utils.g_grid_size / 2)
			{
				m_main_char.m_velocity.x = 0;
				vx = 0;
				m_main_char.m_pos.x = utils.g_grid_size / 2;
			}
			else if (vx > 0 && m_main_char.m_pos.x + vx >= game_data._instance.m_map_data.maps[m_world].x_num * utils.g_grid_size - utils.g_grid_size / 2)
			{
				m_main_char.m_velocity.x = 0;
				vx = 0;
				m_main_char.m_pos.x = game_data._instance.m_map_data.maps[m_world].x_num * utils.g_grid_size - utils.g_grid_size / 2 - 1;
			}
			if (m_main_char.m_velocity.y != 0 && m_jump_state == 0)
			{
				m_jump_state = 1;
				m_jump_num = 0;
				m_jump_num_time = 0;
			}
			if (m_jump_num_time > 0 && m_jump_state == 0)
			{
				m_jump_num_time--;
				if (m_jump_num_time <= 0)
				{
					m_jump_num = 0;
				}
			}
		}
	}

	void check_hit_make_list_wgk(mario_obj mo1, bool wgk)
	{
		for (int i = 0; i < bobjs.Count; ++i)
		{
			mario_obj mo2 = bobjs[i];
			if (mo1 == mo2)
			{
				continue;
			}
			/// 乌龟对撞
			if (wgk != mo2.m_wgk)
			{
				continue;
			}
			if (mo1.hit(mo2))
			{
				tobjs.Add(bobjs[i]);
			}
		}
	}

	void check_hit_make_list_normal(mario_obj mo1)
	{
		for (int i = 0; i < bobjs.Count; ++i)
		{
			mario_obj mo2 = bobjs[i];
			if (mo1 == mo2)
			{
				continue;
			}
			if (mo1.hit(mo2))
			{
				tobjs.Add(mo2);
			}
		}
	}

	void check_hit_make_list(mario_obj mo1, mario_obj.mario_type type, bool wgk)
	{
		bobjs.Clear ();
		tobjs.Clear ();
		if (m_aobjs[(int)type].Count <= 5)
		{
			bobjs.AddRange(m_aobjs[(int)type]);
		}
		else
		{
			mo1.m_qtree.retrive(mo1, ref bobjs, (int)type);
		}
		if (mo1.m_wgk)
		{
			check_hit_make_list_wgk (mo1, wgk);
		}
		else
		{
			check_hit_make_list_normal(mo1);
		}
	}

	void check_hit(mario_obj mo1, mario_obj.mario_type type, bool all = true, bool wgk = false)
	{
		if (mo1.is_die() || !mo1.m_is_start)
		{
			return;
		}
		check_hit_make_list (mo1, type, wgk);
		if (all)
		{
			if (type != mario_obj.mario_type.mt_charater)
			{
				check_left(mo1);
				check_right(mo1);
				check_top(mo1);
				check_bottom(mo1, type);
			}
			else
			{
				check_top(mo1);
				check_bottom(mo1, type);
				check_left(mo1);
				check_right(mo1);
			}
			if (mo1.m_main)
			{
				check_left_top(mo1);
				check_right_top(mo1);
				check_left_bottom(mo1);
				check_right_bottom(mo1);
			}
		}
		
		for (int i = 0; i < tobjs.Count; ++i)
		{
			mario_obj mo2 = tobjs[i];
			if (!mo2.is_die() && mo1.hit(mo2))
			{
				mo2.be_hit(mo1);
			}
		}
	}

	void check_left(mario_obj mo1)
	{
		// 左
		bool flag = false;
		int px = 0;
		for (int i = 0; i < tobjs.Count; ++i)
		{
			mario_obj mo2 = tobjs[i];
			if (mo2.m_nright && mo2.m_unit != null && mo2.m_unit.is_static == 1)
			{
				mario_obj o = get_obj(mo2.m_init_pos.x + 1, mo2.m_init_pos.y);
				if (o && !o.is_die() && o.m_is_static)
				{
					continue;
				}
			}
			if (!mo2.is_die() && mo1.left_hit(mo2))
			{
				int ppx = 0;
				bool flag1 = mo2.be_left_hit(mo1, ref ppx);
				if (flag1)
				{
					if (!flag)
					{
						px = ppx;
						flag = true;
					}
					else if (ppx > px)
					{
						px = ppx;
					}
				}
			}
		}
		if (flag)
		{
			mo1.m_pos.x = px;
			refresh_obj(mo1);
		}
	}

	void check_right(mario_obj mo1)
	{
		// 右
		bool flag = false;
		int px = 0;
		for (int i = 0; i < tobjs.Count; ++i)
		{
			mario_obj mo2 = tobjs[i];
			if (mo2.m_nleft && mo2.m_unit != null && mo2.m_unit.is_static == 1)
			{
				mario_obj o = get_obj(mo2.m_init_pos.x - 1, mo2.m_init_pos.y);
				if (o && !o.is_die() && o.m_is_static)
				{
					continue;
				}
			}
			if (!mo2.is_die() && mo1.right_hit(mo2))
			{
				int ppx = 0;
				bool flag1 = mo2.be_right_hit(mo1, ref ppx);
				if (flag1)
				{
					if (!flag)
					{
						px = ppx;
						flag = true;
					}
					else if (ppx < px)
					{
						px = ppx;
					}
				}
			}
		}
		if (flag)
		{
			mo1.m_pos.x = px;
			refresh_obj(mo1);
		}
	}

	void check_top(mario_obj mo1)
	{
		// 上
		bool flag = false;
		int py = 0;
		for (int i = 0; i < tobjs.Count; ++i)
		{
			mario_obj mo2 = tobjs[i];
			if (!mo2.is_die() && mo1.top_hit(mo2))
			{
				int ppy = 0;
				bool flag1 = mo2.be_top_hit(mo1, ref ppy);
				if (flag1)
				{
					if (!flag)
					{
						py = ppy;
						flag = true;
					}
					else if (ppy < py)
					{
						py = ppy;
					}
				}
			}
		}
		if (flag)
		{
			mo1.m_pos.y = py;
			refresh_obj(mo1);
		}
	}

	void check_bottom(mario_obj mo1, mario_obj.mario_type type)
	{
		// 下
		bool flag = false;
		int py = 0;
		for (int i = 0; i < tobjs.Count; ++i)
		{
			mario_obj mo2 = tobjs[i];
			if (!mo2.is_die() && mo1.bottom_hit(mo2))
			{
				int ppy = 0;
				bool flag1 = mo2.be_bottom_hit(mo1, ref ppy);
				if (flag1)
				{
					if (!flag)
					{
						py = ppy;
						flag = true;
					}
					else if (ppy > py)
					{
						py = ppy;
					}
				}
			}
		}
		if (flag)
		{
			mo1.m_pos.y = py;
			refresh_obj(mo1);
		}

		// 地板
		for (int i = 0; i < mo1.m_bnl_objs.Count; ++i)
		{
			mario_obj mo2 = mo1.m_bnl_objs[i];
			if (!mo2.is_die() && type == mo2.m_type)
			{
				int ppy = 0;
				mo2.be_bottom_hit(mo1, ref ppy);
			}
		}

	}

	void check_left_top(mario_obj mo1)
	{
		// 左上
		bool flag = false;
		int px = 0;
		int py = 0;
		for (int i = 0; i < tobjs.Count; ++i)
		{
			mario_obj mo2 = tobjs[i];
			if (!mo2.is_die() && mo1.left_top_hit(mo2))
			{
				int ppx = 0;
				int ppy = 0;
				bool flag1 = mo2.be_left_top_hit(mo1, ref ppx, ref ppy);
				if (flag1)
				{
					if (!flag)
					{
						px = ppx;
						py = ppy;
						flag = true;
					}
					else
					{
						if (ppx > px)
						{
							px = ppx;
						}
						if (ppy < py)
						{
							py = ppy;
						}
					}
				}
			}
		}
		if (flag)
		{
			mo1.m_pos.x = px;
			mo1.m_pos.y = py;
			refresh_obj(mo1);
		}
	}

	void check_right_top(mario_obj mo1)
	{
		// 右上
		bool flag = false;
		int px = 0;
		int py = 0;
		for (int i = 0; i < tobjs.Count; ++i)
		{
			mario_obj mo2 = tobjs[i];
			if (!mo2.is_die() && mo1.right_top_hit(mo2))
			{
				int ppx = 0;
				int ppy = 0;
				bool flag1 = mo2.be_right_top_hit(mo1, ref ppx, ref ppy);
				if (flag1)
				{
					if (!flag)
					{
						px = ppx;
						py = ppy;
						flag = true;
					}
					else
					{
						if (ppx < px)
						{
							px = ppx;
						}
						if (ppy < py)
						{
							py = ppy;
						}
					}
				}
			}
		}
		if (flag)
		{
			mo1.m_pos.x = px;
			mo1.m_pos.y = py;
			refresh_obj(mo1);
		}
	}

	void check_left_bottom(mario_obj mo1)
	{
		// 左下
		bool flag = false;
		int px = 0;
		int py = 0;
		for (int i = 0; i < tobjs.Count; ++i)
		{
			mario_obj mo2 = tobjs[i];
			if (!mo2.is_die() && mo1.left_bottom_hit(mo2))
			{
				int ppx = 0;
				int ppy = 0;
				bool flag1 = mo2.be_left_bottom_hit(mo1, ref ppx, ref ppy);
				if (flag1)
				{
					if (!flag)
					{
						px = ppx;
						py = ppy;
						flag = true;
					}
					else
					{
						if (ppx > px)
						{
							px = ppx;
						}
						if (ppy > py)
						{
							py = ppy;
						}
					}
				}
			}
		}
		if (flag)
		{
			mo1.m_pos.x = px;
			mo1.m_pos.y = py;
			refresh_obj(mo1);
		}

	}
	
	void check_right_bottom(mario_obj mo1)
	{
		// 右下
		bool flag = false;
		int px = 0;
		int py = 0;
		for (int i = 0; i < tobjs.Count; ++i)
		{
			mario_obj mo2 = tobjs[i];
			if (!mo2.is_die() && mo1.right_bottom_hit(mo2))
			{
				int ppx = 0;
				int ppy = 0;
				bool flag1 = mo2.be_right_bottom_hit(mo1, ref ppx, ref ppy);
				if (flag1)
				{
					if (!flag)
					{
						px = ppx;
						py = ppy;
						flag = true;
					}
					else
					{
						if (ppx < px)
						{
							px = ppx;
						}
						if (ppy > py)
						{
							py = ppy;
						}
					}
				}
			}
		}
		if (flag)
		{
			mo1.m_pos.x = px;
			mo1.m_pos.y = py;
			refresh_obj(mo1);
		}
	}

	void check_create()
	{
		if (m_main_char == null)
		{
			return;
		}

		int delta = m_main_char.m_pos.x - m_roll.x;
		if (delta < 2 && delta > -2)
		{
			m_roll.x = m_main_char.m_pos.x;
		}
		else if (delta < 5 && delta > 0)
		{
			m_roll.x++;
		}
		else if (delta > -5 && delta < 0)
		{
			m_roll.x--;
		}
		else
		{
			m_roll.x += delta / 5;
		}

		if (m_main_char.m_is_on_floor || m_paqiang_jump)
		{
			m_y_roll_target = m_main_char.m_pos.y - utils.g_roll_y;
		}
		delta = m_main_char.m_pos.y - utils.g_roll_y - m_roll.y;
		if (delta < 0 && delta > -40)
		{
			m_roll.y = m_main_char.m_pos.y - utils.g_roll_y;
		}
		else if (delta > -100 && delta < 0)
		{
			m_roll.y -= 10;
		}
		else if (delta < 0)
		{
			m_roll.y += delta / 10;
		}
		else
		{
			delta = m_y_roll_target - m_roll.y;
			if (delta > 0 && delta < 40)
			{
				m_roll.y = m_y_roll_target;
			}
			else if (delta < 250 && delta > 0)
			{
				m_roll.y += 10;
			}
			else if (delta > 0)
			{
				m_roll.y += delta / 25;
			}
		}
		if (m_roll.y + utils.g_height > game_data._instance.m_map_data.maps[m_world].y_num * utils.g_grid_size)
		{
			m_roll.y = game_data._instance.m_map_data.maps[m_world].y_num * utils.g_grid_size - utils.g_height;
		}
		if (m_roll.y < 0)
		{
			m_roll.y = 0;
		}
		
		HashSet<int> has_create = new HashSet<int>();
		int px = m_roll.x / utils.g_grid_size;
		if (px != m_grid.x)
		{
			int i = px + utils.g_start_x;
			if (m_grid.x > px)
			{
				i = px - utils.g_start_x;
			}

			if (i >= 0 && i < game_data._instance.m_map_data.maps[m_world].x_num)
			{
				for (int j = m_grid.y - utils.g_start_y; j <= m_grid.y + utils.g_start_y; ++j)
				{
					if (j < 0 || j >= game_data._instance.m_map_data.maps[m_world].y_num)
					{
						continue;
					}
					int key = j * game_data._instance.m_map_data.maps[m_world].x_num + i;
					has_create.Add(key);
					do_create(i, j);
				}
			}
			m_grid.x = px;
		}

		int py = (m_roll.y + utils.g_roll_y) / utils.g_grid_size;
		if (py != m_grid.y)
		{
			int j = py + utils.g_start_y;
			if (m_grid.y > py)
			{
				j = py - utils.g_start_y;
			}
			if (j >= 0 && j < game_data._instance.m_map_data.maps[m_world].y_num)
			{
				for (int i = m_grid.x - utils.g_start_x; i <= m_grid.x + utils.g_start_x; ++i)
				{
					if (i < 0 || i >= game_data._instance.m_map_data.maps[m_world].x_num)
					{
						continue;
					}
					int key = j * game_data._instance.m_map_data.maps[m_world].x_num + i;
					if (!has_create.Contains(key))
					{
						has_create.Add(key);
						do_create(i, j);
					}
				}
			}
			m_grid.y = py;
		}

		m_need_calc.Clear ();
		for (int k = 0; k < m_aobjs[(int)mario_obj.mario_type.mt_charater].Count; ++k)
		{
			mario_obj tobj = m_aobjs[(int)mario_obj.mario_type.mt_charater][k];
			int x = tobj.m_grid.x;
			int y = tobj.m_grid.y;
			for (int j = y - 1; j <= y + 1; ++j)
			{
				if (j < 0 || j >= game_data._instance.m_map_data.maps[m_world].y_num)
				{
					continue;
				}
				for (int i = x - 1; i <= x + 1; ++i)
				{
					if (i < 0 || i >= game_data._instance.m_map_data.maps[m_world].x_num)
					{
						continue;
					}
					if (tobj.m_is_start)
					{
						int key = j * game_data._instance.m_map_data.maps[m_world].x_num + i;
						m_need_calc.Add(key);
						if (!has_create.Contains(key))
						{
							has_create.Add(key);
							do_create(i, j);
						}
					}
				}
			}
		}
	}

	void do_create(int i, int j)
	{
		int key = j * game_data._instance.m_map_data.maps[m_world].x_num + i;
		if (m_delete_objs[m_world].ContainsKey(key))
		{
			List<mario_obj> l = m_delete_objs[m_world][key];
			for (int k = 0; k < l.Count; ++k)
			{
				mario_obj tos = l[k];
				add_obj(tos);
				tos.gameObject.SetActive(true);
				if (tos.m_shadow != null)
				{
					tos.m_shadow.SetActive(true);
				}
				tos.m_is_destory = 0;
				tos.m_is_new = true;
				tos.check_is_start(m_grid);
			}
			m_delete_objs[m_world].Remove(key);
		}

		s_t_unit t_unit = game_data._instance.get_t_unit(game_data._instance.m_arrays[m_world][j][i].type);
		if (t_unit == null)
		{
			return;
		}
		if (m_has_create[m_world].Contains(key))
		{
			return;
		}
		create_mario_obj(t_unit.res, t_unit, game_data._instance.m_arrays[m_world][j][i].param, i, j);
		m_has_create[m_world].Add(key);
	}

	public bool need_calc(int i, int j)
	{
		int key = j * game_data._instance.m_map_data.maps[m_world].x_num + i;
		return m_need_calc.Contains (key); 
	}

	void check_delete()
	{
		List<mario_obj> tos = new List<mario_obj>();
		for (int i = 0; i < m_mobjs.Count; ++i)
		{
			if (m_mobjs[i].m_is_destory > 0)
			{
				tos.Add(m_mobjs[i]);
			}
		}
		for (int i = 0; i < tos.Count; ++i)
		{
			if (tos[i] == m_main_char)
			{
				m_main_char = null;
			}
			if (tos[i].m_is_destory == 1 || tos[i].m_unit == null || !tos[i].m_bkcf)
			{
				if (tos[i].m_unit != null && !tos[i].m_bkcf)
				{
					m_has_create[m_world].Remove(tos[i].m_init_pos.y * game_data._instance.m_map_data.maps[m_world].x_num + tos[i].m_init_pos.x);
				}
				remove_obj(tos[i]);
				tos[i].destroy_shadow();
				UnityEngine.Object.Destroy(tos[i].gameObject);
			}
			else
			{
				remove_obj(tos[i]);
				tos[i].check_state(m_grid);
				tos[i].gameObject.SetActive(false);
				if (tos[i].m_shadow != null)
				{
					tos[i].m_shadow.SetActive(false);
				}
				int key = tos[i].m_grid.y * game_data._instance.m_map_data.maps[m_world].x_num + tos[i].m_grid.x;
				if (!m_delete_objs[m_world].ContainsKey(key))
				{
					m_delete_objs[m_world].Add(key, new List<mario_obj>());
				}
				m_delete_objs[m_world][key].Add(tos[i]);
			}
		}
	}

	void update_ex()
	{
		for (int i = 0; i < m_mobjs.Count; ++i)
		{
			m_mobjs[i].update_ex(m_grid);
		}
		m_scene.update_ex (m_roll);

		if (game_data._instance.m_map_data.mode != 0)
		{
			if (game_data._instance.m_map_data.mode == 1)
			{
				m_zhuiji_x += 20;
			}
			else if (game_data._instance.m_map_data.mode == 2)
			{
				m_zhuiji_x += 40;
			}
			m_zhuiji.transform.localPosition = new Vector3(m_zhuiji_x / 10, (m_roll.y + utils.g_roll_y) / 10);
			if (m_zhuiji_x >= m_main_char.m_pos.x)
			{
				m_main_char.m_is_die = true;
			}
		}
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if (m_pause)
		{
			return;
		}
		if (mario._instance.m_game_state == e_game_state.egs_edit)
		{
			if (mario._instance.m_self.guide == 0 && m_main_char != null && m_main_char.m_pos.x > 22400)
			{
				s_message mes = new s_message();
				mes.m_type = "jx_1";
				mario._instance.show_xsjx_dialog_box(game_data._instance.get_language_string("play_mode_zkql"), mes);
				m_pause = true;
				return;
			}
		}
		if (m_state == 0 && m_main_char != null && !m_main_char.is_die() && m_main_char.m_bl[1] == 0)
		{
			m_time += 1;
			m_total_time = m_time;
			if (m_left_time > 0)
			{
				m_left_time--;
			}
			if (m_right_time > 0)
			{
				m_right_time--;
			}
			check_input ();
			m_csm = null;
			if (m_pause)
			{
				return;
			}
			if (mario._instance.m_game_state != e_game_state.egs_edit)
			{
				if (m_time / 50 >= game_data._instance.m_map_data.time)
				{
					m_main_char.m_is_die = true;
					s_message mes = new s_message();
					mes.m_type = "time_up";
					cmessage_center._instance.add_message(mes);
				}
				if (!m_has_time_tx && m_time / 50 >= game_data._instance.m_map_data.time - 100)
				{
					m_has_time_tx = true;
					mario._instance.pause_mus();
					mario._instance.play_sound_ex1("sound/hurry");
				}
				if (m_has_time_tx)
				{
					m_time_tx_time++;
					if (m_time_tx_time == 150)
					{
						mario._instance.continue_mus(1.5f);
					}
				}
			}
		}
		if (m_main_char)
		{
			if (!m_main_char.is_die() && m_state == 0 && m_main_char.m_bl[1] == 0)
			{
				check_state ();
				update_ex ();
				check_delete ();
				check_create ();
			}
			else if (m_main_char.m_bl[1] != 0)
			{
				m_main_char.update_ex(m_grid);
				check_delete ();
			}
			else if (m_main_char.is_die())
			{
				m_main_char.m_pos.x += m_main_char.m_velocity.x;
				m_main_char.m_pos.y += m_main_char.m_velocity.y;
				m_main_char.update_ex(m_grid);
				check_delete ();
			}
			else
			{
				m_main_char.update_ex(m_grid);
				check_delete ();
			}
		}
		else if (m_state == 0)
		{
			m_state = 101;
		}

		if (m_render.transform.localScale.x < 1.0f)
		{
			float s = m_render.transform.localScale.x + 0.01f;
			m_render.transform.localScale = new Vector3(s, s, 1.0f);
		}
		else if (m_render.transform.localScale.x > 1.0f)
		{
			m_render.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
		}

		if (m_state == 100)
		{
			if (mario._instance.m_game_state != e_game_state.egs_edit)
			{
				m_state = 150;
				m_time_tx_time = 0;
				mario._instance.play_mus_ex1 ("music/win", false);
			}
			else
			{
				win ();
				m_state = 999;
			}
		}
		else if (m_state == 150)
		{
			m_time_tx_time++;
			if (game_data._instance.m_map_data.time > m_time / 50)
			{
				m_time += 50;
				add_score(50);
				if (game_data._instance.m_map_data.time > m_time / 50)
				{
					m_time += 50;
					add_score(50);
				}
				if (game_data._instance.m_map_data.time > m_time / 50)
				{
					m_time += 50;
					add_score(50);
				}
				mario._instance.play_sound("sound/js");
			}
			else if (m_time_tx_time >= 200)
			{
				win ();
				m_state = 999;
			}
		}
		else if (m_state == 101)
		{
			lose ();
			m_state = 999;
		}

		if (m_show_cha > 0)
		{
			show_cha();
		}
	}

	public void win()
	{
		if (mario._instance.m_game_state == e_game_state.egs_edit)
		{
			if (mario._instance.m_self.guide == 3)
			{
				mario._instance.m_self.guide = 4;
			}
			s_message mes = new s_message();
			mes.m_type = "edit_mode";
			mario_point p = new mario_point(m_die_pos.x % 10000000, m_die_pos.y);
			mes.m_object.Add(p);
			mes.m_ints.Add(m_world);
			cmessage_center._instance.add_message(mes);
		}
		else
		{
			s_message mes = new s_message();
			mes.m_type = "play_win";
			mes.m_object.Add(m_inputs);
			mes.m_ints.Add(m_score);
			mes.m_ints.Add(m_total_time);
			cmessage_center._instance.add_message(mes);
		}
	}

	public void lose()
	{
		if (mario._instance.m_game_state == e_game_state.egs_edit)
		{
			if (mario._instance.m_self.guide < 100)
			{
				s_message mes = new s_message();
				mes.m_type = "edit_mode";
				cmessage_center._instance.add_message(mes);
			}
			else
			{
				s_message mes = new s_message();
				mes.m_type = "edit_mode";
				mario_point p = new mario_point(m_die_pos.x % 10000000, m_die_pos.y);
				mes.m_object.Add(p);
				mes.m_ints.Add(m_world);
				cmessage_center._instance.add_message(mes);
			}
		}
		else
		{
			s_message mes = new s_message();
			mes.m_type = "play_lose";
			mes.m_object.Add(m_die_pos);
			cmessage_center._instance.add_message(mes);

			game_data._instance.m_die_poses[m_world].Add(new mario_point(m_die_pos.x, m_die_pos.y));
		}
	}

	public void start_cha()
	{
		if (m_show_cha == 0)
		{
			m_show_cha = 1;
		}
	}

	void show_cha()
	{
		m_show_cha_time++;
		if (m_show_cha_time % 3 != 0)
		{
			return;
		}
		if (mario._instance.m_game_state == e_game_state.egs_edit)
		{
			return;
		}
		mario_point p = new mario_point (m_die_pos.x % 10000000, m_die_pos.y);
		if (m_show_cha > 1)
		{
			bool flag = true;
			while (flag)
			{
				if (m_show_cha > game_data._instance.m_die_poses[m_world].Count)
				{
					m_show_cha = 0;
					return;
				}
				p = new mario_point (game_data._instance.m_die_poses[m_world][m_show_cha - 2].x % 10000000, game_data._instance.m_die_poses[m_world][m_show_cha - 2].y);
				if (p.y < 0)
				{
					p.y = 0;
				}
				int x = p.x / utils.g_grid_size - m_grid.x;
				int y = p.y / utils.g_grid_size - m_grid.y;
				if (x <= -utils.g_active_x || x >= utils.g_active_x || y < -utils.g_active_y || y > utils.g_active_y)
				{
					flag = true;
				}
				else
				{
					flag = false;
				}
				m_show_cha++;
			}
		}
		else
		{
			m_show_cha++;
		}

		if (m_show_cha == 2)
		{
			GameObject res = (GameObject)Resources.Load ("unit/other/cha_big");
			GameObject obj = (GameObject)Instantiate(res);
			obj.transform.parent = m_other.transform;
			obj.transform.localPosition = new Vector3(p.x / 10, p.y / 10);
			obj.transform.localScale = new Vector3 (1, 1, 1);
		}
		else
		{
			GameObject res = (GameObject)Resources.Load ("unit/other/cha_small");
			GameObject obj = (GameObject)Instantiate(res);
			obj.transform.parent = m_other.transform;
			obj.transform.localPosition = new Vector3(p.x / 10, p.y / 10);
			obj.transform.localScale = new Vector3 (1, 1, 1);
		}
	}

	public void add_score(int score)
	{
		m_score += score;
	}

	public void add_score(int x, int y, int score)
	{
		m_score += score;
		s_message mes = new s_message ();
		mes.m_type = "add_score";
		mes.m_ints.Add (x / 10);
		mes.m_ints.Add (y / 10);
		mes.m_ints.Add (score);
		cmessage_center._instance.add_message (mes);
	}

	public void caisi(int y, bool tx = false, int xx = 0, int yy = 0)
	{
		m_caisi = true;
		m_caisi_y = y;
		m_main_char.m_is_on_floor = true;
		if (tx)
		{
			effect ("hit", xx, yy);
		}
	}

	public void effect(string name, int xx, int yy)
	{
		GameObject res = (GameObject)Resources.Load ("unit/other/" + name);
		GameObject obj = (GameObject)Instantiate(res);
		obj.transform.parent = m_main.transform;
		obj.transform.localPosition = new Vector3(xx / 10, yy / 10);
		obj.transform.localScale = new Vector3 (1, 1, 1);
	}

	public void ding()
	{
		m_jump_state = 1;
		m_sjump = true;
		m_jump = false;
	}

	public void show_chuan(mario_obj obj)
	{
		m_csm = obj;
	}

	public bool can_show_chuan()
	{
		return m_csm != null;
	}

	void Update()
	{
		if (mario._instance.key_down(KeyCode.LeftArrow))
		{
			m_now_inputs[0] = true;
		}
		if (mario._instance.key_up(KeyCode.LeftArrow))
		{
			m_now_inputs[0] = false;
		}
		if (mario._instance.key_down(KeyCode.RightArrow))
		{
			m_now_inputs[1] = true;
		} 
		if (mario._instance.key_up(KeyCode.RightArrow))
		{
			m_now_inputs[1] = false;
		}
		if (mario._instance.key_down(KeyCode.Z))
		{
			m_now_inputs[2] = true;
		}
		if (mario._instance.key_up(KeyCode.Z))
		{
			m_now_inputs[2] = false;
		}
		if (mario._instance.key_down(KeyCode.X))
		{
			m_now_inputs[3] = true;
		}
		if (mario._instance.key_up(KeyCode.X))
		{
			m_now_inputs[3] = false;
		}
		if (Application.isEditor && Input.GetKeyUp(KeyCode.E))
		{
			m_good_inputs = new List<int>();
			m_good_inputs.AddRange(m_inputs);
		}
		if (Application.isEditor && Input.GetKeyUp(KeyCode.R))
		{
			m_zr = true;
			for (int i = 0; i < m_inputs.Count / 2; ++i)
			{
				if (m_inputs[i * 2] > m_time)
				{
					m_inputs.RemoveRange(i * 2, m_inputs.Count - i * 2);
				}
			}
		}
		m_cam.transform.localPosition = new Vector3 (m_roll.x / 10, (m_roll.y + utils.g_roll_y) / 10);
	}
}

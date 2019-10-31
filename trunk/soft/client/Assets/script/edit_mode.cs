using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class edit_mode : MonoBehaviour {

	private HashSet<int> m_has_create = new HashSet<int>();
	private List<mario_obj> m_objs = new List<mario_obj>();
	public mario_obj m_main_char;
	private mario_scene m_scene;
	private bool m_left = false;
	private bool m_right = false;
	private bool m_up = false;
	private bool m_down = false;
	private int m_time;
	public mario_point m_roll = new mario_point();
	private mario_point m_grid = new mario_point();
	public Camera m_cam;
	public GameObject m_fuzhu;
	public GameObject m_other;
	public GameObject m_render;
	public GameObject m_back;
	public GameObject m_main;
	public GameObject m_shadow;
	public mario_point m_qpos;
	private int m_add_qz_type = 0;
	private int m_add_qz_x = 0;
	private int m_add_qz_y = 0;
	private mario_point m_pre_csg;
	private List<GameObject> m_hongs = new List<GameObject>();
	private GameObject m_lv;
	private GameObject m_ban;
	private GameObject m_ban1;
	private GameObject m_zhuiji;
	private int m_yf;
	private GameObject m_dlg;
	public int m_world = 0;
	private bool m_first_csm = false;
	private int m_csm_id = 0;
	private int m_csm_w = 0;
	private int m_csm_x = 0;
	private int m_csm_y = 0;
	private int m_edit_cys_time = 0;
	private int m_edit_cys_num = 0;
	private int m_edit_cys_tnum = 0;
	private List<edit_cy> m_ecs;

	public static edit_mode _instance;
	void Awake()
	{
		_instance = this;
		Joystick.On_JoystickHolding += joy_hold;
		Joystick.On_JoystickMoveEnd += joy_move_end;
	}

	public void reload(mario_point qpos, int world, List<edit_cy> ecs)
	{
		m_edit_cys_time = 0;
		m_edit_cys_num = 0;
		m_edit_cys_tnum = ecs.Count;
		m_ecs = new List<edit_cy> ();
		m_ecs.AddRange (ecs);
		for (int i = 0; i < m_ecs.Count; ++i)
		{
			m_ecs[i].num = m_ecs.Count - 1 - i;
		}
		m_qpos = qpos;
		reset (world);
	}

	public void reload(int world)
	{
		m_qpos = new mario_point(m_main_char.m_pos.x, m_main_char.m_pos.y);
		reset (world);
	}

	public IEnumerator reload_fg(int world)
	{
		yield return new WaitForEndOfFrame();
		int w = (int)(mario._instance.m_width / 0.8f + 0.01f);
		int h = (int)(mario._instance.m_height / 0.8f + 0.01f);
		RenderTexture rd = new RenderTexture(w, h, 0);
		m_cam.targetTexture = rd;
		m_cam.Render();
		RenderTexture.active = rd;
		Texture2D tex = new Texture2D(w, h, TextureFormat.RGB24, false);
		tex.ReadPixels(new Rect(0, 0, w, h), 0, 0);
		tex.Apply();
		yield return tex;
		m_cam.targetTexture = null;   
		RenderTexture.active = null; 
		GameObject.Destroy(rd);

		int rx = m_roll.x;
		int ry = m_roll.y;

		m_qpos = new mario_point(m_main_char.m_pos.x, m_main_char.m_pos.y);
		bool b = m_first_csm;
		reset (world);
		m_first_csm = b;

		for (int i = 0; i < 24; ++i)
		{
			for (int j = 0; j < 12; ++j)
			{
				int x = w / 2 + (i - 12) * 64 - rx % utils.g_grid_size / 10;
				int y = h / 2 + (j - 6) * 64 - ry % utils.g_grid_size / 10;
				if (x < 0 || y < 0 || x + 63 >= w || y + 63 >= h)
				{
					continue;
				}
				GameObject res = (GameObject)Resources.Load ("unit/other/fg");
				GameObject obj = (GameObject)Instantiate(res);
				obj.transform.parent = m_fuzhu.transform;
				obj.transform.localPosition = new Vector3((i - 12) * utils.g_grid_size / 10 + 32, (j - 6) * utils.g_grid_size / 10 + 32, 0);
				obj.transform.localScale = new Vector3 (1, 1, 1);
				obj.GetComponent<edit_gui_fg>().reset(i, j);
				
				Texture2D tex1 = new Texture2D(64, 64, TextureFormat.RGB24, false);
				tex1.SetPixels(tex.GetPixels(x, y, 64, 64));
				tex1.Apply (false);
				Sprite sp = Sprite.Create(tex1, new Rect(0, 0, 64, 64), new Vector2(0.5f, 0.5f), 1);
				obj.GetComponent<SpriteRenderer>().sprite = sp;
			}
		}
		m_main_char.m_pos = new mario_point(m_qpos.x, m_qpos.y);
	}

	public void reload_jump(mario_point qpos, int world)
	{
		m_qpos = qpos;
		bool b = m_first_csm;
		reset (world);
		m_first_csm = b;
	}

	public void reload_self(int world)
	{
		bool b = m_first_csm;
		m_qpos = null;
		reset (world);
		m_first_csm = b;
	}

	void reset(int world)
	{
		m_world = world;
		m_dlg = null;
		mario._instance.remove_child (m_main);
		mario._instance.remove_child (m_fuzhu);
		m_hongs.Clear();
		mario._instance.remove_child (m_other);
		mario._instance.remove_child (m_shadow);
		m_has_create.Clear ();
		m_objs.Clear ();
		m_zhuiji = null;
		m_roll = new mario_point();
		m_grid = new mario_point ();
		m_left = false;
		m_right = false;
		m_up = false;
		m_down = false;
		m_time = 0;
		m_add_qz_type = 0;
		m_add_qz_x = 0;
		m_add_qz_y = 0;
		m_yf = 0;
		m_first_csm = false;

		reset_db();

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
		del_obj (m_main_char);

		m_roll.x = m_main_char.m_pos.x;
		m_roll.y = m_main_char.m_pos.y - utils.g_roll_y;
		if (m_roll.y + utils.g_height > 30 * utils.g_grid_size)
		{
			m_roll.y = 30 * utils.g_grid_size - utils.g_height;
		}
		if (m_roll.y < 0)
		{
			m_roll.y = 0;
		}

		m_grid.x = m_roll.x / utils.g_grid_size;
		m_grid.y = (m_roll.y + utils.g_roll_y) / utils.g_grid_size;		
		for (int i = m_grid.x - utils.g_active_x; i <= m_grid.x + utils.g_active_x; ++i)
		{
			if (i < 0 || i >= game_data._instance.m_map_data.maps[m_world].x_num)
			{
				continue;
			}
			for (int j = m_grid.y - utils.g_active_y; j <= m_grid.y + utils.g_active_y; ++j)
			{
				if (j < 0 || j >= game_data._instance.m_map_data.maps[m_world].y_num)
				{
					continue;
				}
				do_create(i, j);
			}
		}
		reset_db ();

		create_scene ();

		for (int i = 0; i < 24; ++i)
		{
			for (int j = 0; j < 11; ++j)
			{
				GameObject res = (GameObject)Resources.Load ("unit/other/fuzhu");
				GameObject obj = (GameObject)Instantiate(res);
				obj.transform.parent = m_fuzhu.transform;
				obj.transform.localPosition = new Vector3((i - 12) * utils.g_grid_size / 10, (j * utils.g_grid_size - utils.g_height / 2) / 10, 0);
				obj.transform.localScale = new Vector3 (1, 1, 1);
			}
		}

		{
			GameObject res = (GameObject)Resources.Load ("unit/other/box");
			m_lv = (GameObject)Instantiate(res);
			m_lv.transform.parent = m_other.transform;
			m_lv.transform.localPosition = new Vector3(0, 0, 0);
			m_lv.transform.localScale = new Vector3 (1, 0.2f, 1);
			m_lv.GetComponent<SpriteRenderer>().color = new Color(0, 1, 0, 1);
			m_lv.SetActive(false);
			for (int i = 0; i < 8; ++i)
			{
				res = (GameObject)Resources.Load ("unit/other/box");
				GameObject hong = (GameObject)Instantiate(res);
				hong.transform.parent = m_other.transform;
				hong.transform.localPosition = new Vector3(0, 0, 0);
				hong.transform.localScale = new Vector3 (4, 4, 1);
				hong.GetComponent<SpriteRenderer>().color = new Color(0, 1, 0, 0.25f);
				hong.SetActive(false);
				m_hongs.Add(hong);
			}
		}

		{
			GameObject res = (GameObject)Resources.Load ("unit/other/box");
			m_ban = (GameObject)Instantiate(res);
			m_ban.transform.parent = m_other.transform;
			m_ban.transform.localPosition = new Vector3(0, 0, 0);
			m_ban.transform.localScale = new Vector3 (4, 120, 1);
			m_ban.GetComponent<SpriteRenderer>().color = new Color(1, 0, 0, 0.25f);

			m_ban1 = (GameObject)Instantiate(res);
			m_ban1.transform.parent = m_other.transform;
			m_ban1.transform.localPosition = new Vector3(0, 0, 0);
			m_ban1.transform.localScale = new Vector3 (4, 120, 1);
			m_ban1.GetComponent<SpriteRenderer>().color = new Color(1, 0, 0, 0.25f);
		}

		reset_zhuiji ();

		if (m_edit_cys_num == m_edit_cys_tnum)
		{
			for (int i = 0; i < m_ecs.Count; ++i)
			{
				create_cy(i);
			}
		}

		if (mario._instance.m_self.guide > 0 && mario._instance.m_self.guide <= 2)
		{
			create_xy();
		}
	}

	public void reset_ending()
	{
		for (int i = 0; i < m_objs.Count; ++i)
		{
			if (m_objs[i].m_name == "mario_end")
			{
				m_objs[i].reset();
			}
		}
	}

	void add_obj(mario_obj obj)
	{
		m_objs.Add (obj);
		m_has_create.Add (obj.m_init_pos.y * utils.g_max_x + obj.m_init_pos.x);
	}

	void del_obj(mario_obj obj)
	{
		m_objs.Remove (obj);
		m_has_create.Remove (obj.m_init_pos.y * utils.g_max_x + obj.m_init_pos.x);
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

	public mario_obj create_mario_obj(string name, s_t_unit unit, List<int> param, int x, int y, bool bb = false)
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
		mobj.m_edit_mode = true;
		mobj.init (name, param, m_world, x, y, utils.g_grid_size * x + utils.g_grid_size / 2, utils.g_grid_size * y + utils.g_grid_size / 2);
		mobj.create_shadow (m_shadow);
		add_obj (mobj);

		if (bb && unit != null && unit.id != 1)
		{
			Vector3 v = new Vector3(obj.transform.localScale.x, obj.transform.localScale.y, obj.transform.localScale.z);
			obj.transform.localScale = new Vector3 (v.x * 0.7f, v.y * 0.7f, v.z * 0.7f);
			TweenScale.Begin (obj, 0.1f, new Vector3(v.x, v.y, v.z));
		}

		return mobj;
	}

	public void create_xy()
	{
		int num = utils.jx_block.Length / 3;
		for (int i = 0; i < num; ++i)
		{
			create_mario_obj_xy(utils.jx_block[i * 3 + 2], utils.jx_block[i * 3], utils.jx_block[i * 3 + 1]);
		}
		string s = "unit/other/dlg";
		if (game_data._instance.m_lang != e_language.el_chinese)
		{
			s = "unit/other/dlg1";
		}
		GameObject res = (GameObject)Resources.Load (s);
		m_dlg = (GameObject)Instantiate(res);
		m_dlg.transform.parent = m_other.transform;
		m_dlg.transform.localPosition = new Vector3(0, 0, 0);
		m_dlg.transform.localScale = new Vector3 (1, 1, 1);
	}

	public void set_dlg(int x, int y)
	{
		m_dlg.transform.localPosition = new Vector3(x * utils.g_grid_size / 10 + 20, (y + 1) * utils.g_grid_size / 10 - 20, 0);
	}

	void create_mario_obj_xy(int id, int x, int y)
	{
		s_t_unit t_unit = game_data._instance.get_t_unit (id);
		string cname = "unit/" + t_unit.res + "/" + t_unit.res;
		if (t_unit.kfg == 1)
		{
			cname = "unit/" + t_unit.res + "/" + game_data._instance.m_map_data.maps[m_world].map_theme.ToString() + "/" + t_unit.res;
		}
		GameObject res = (GameObject)Resources.Load (cname);
		GameObject obj = (GameObject)Instantiate(res);
		obj.transform.parent = m_main.transform;
		obj.transform.localScale = new Vector3 (1, 1, 1);
		obj.transform.localPosition = new Vector3((utils.g_grid_size * x + utils.g_grid_size / 2) / 10, (utils.g_grid_size * y + utils.g_grid_size / 2) / 10);
		Transform fx = obj.transform.FindChild("fx");
		if (fx != null)
		{
			fx.gameObject.SetActive (false);
		}
		obj.GetComponent<mario_obj> ().format_shadow(obj.transform, "shader/gray", -20);
	}

	public void add_obj(int x, int y, int type)
	{
		if (!can_add_ex(x, y))
		{
			mario._instance.show_tip(game_data._instance.get_language_string("edit_mode_qdzd"));
			return;
		}
		if (type > 0)
		{
			s_t_unit t_unit = game_data._instance.get_t_unit(type);
			if (t_unit.is_sw == 1 && !check_sw(x, y))
			{
				mario._instance.show_tip(game_data._instance.get_language_string("edit_mode_tqsw"));
				return;
			}
			if (type == 1)
			{
				if (y < game_data._instance.m_map_data.maps[m_world].y_num && game_data._instance.m_arrays[m_world][y][x].type != 0)
				{
					mario._instance.show_tip(game_data._instance.get_language_string("edit_mode_wffz"));
					return;
				}
				for (int i = 0; i < y; ++i)
				{
					if (i >= game_data._instance.m_map_data.maps[m_world].y_num)
					{
						continue;
					}
					if (game_data._instance.m_arrays[m_world][i][x].type != 0 && game_data._instance.m_arrays[m_world][i][x].type != 1)
					{
						mario._instance.show_tip(game_data._instance.get_language_string("edit_mode_xfcz"));
						return;
					}
				}
				add_y (y);
				for (int i = 0; i <= y; ++i)
				{
					if (game_data._instance.m_arrays[m_world][i][x].type != 0)
					{
						continue;
					}
					game_data._instance.m_arrays[m_world][i][x].type = type;
					add_init(x, i, type);
				}
				reset_db();
			}
			else if (type == utils.g_csg)
			{
				if (m_pre_csg == null)
				{
					if (y < game_data._instance.m_map_data.maps[m_world].y_num && game_data._instance.m_arrays[m_world][y][x].type == utils.g_csg)
					{
						if (game_data._instance.m_arrays[m_world][y][x].param[2] != 0)
						{
							return;
						}
						m_pre_csg = new mario_point(x, y);
					}
					else
					{
						if (y < game_data._instance.m_map_data.maps[m_world].y_num && game_data._instance.m_arrays[m_world][y][x].type != 0)
						{
							mario._instance.show_tip(game_data._instance.get_language_string("edit_mode_wffz"));
							return;
						}
						if (has_csg(x, y, -1, -1))
						{
							mario._instance.show_tip(game_data._instance.get_language_string("edit_mode_wffz"));
							return;
						}
						add_y (y);
						game_data._instance.m_arrays[m_world][y][x].type = type;
						add_init(x, y, type);
						m_pre_csg = new mario_point(x, y);
					}
				}
				else
				{
					if (y < game_data._instance.m_map_data.maps[m_world].y_num && game_data._instance.m_arrays[m_world][y][x].type != 0)
					{
						return;
					}
					int dx = m_pre_csg.x - x;
					int dy = m_pre_csg.y - y;
					bool flag = false;
					if (dx == 2 && dy == 0 || dx == -2 && dy == 0 || dx == 0 && dy == 2 || dx == 0 && dy == -2
					    || dx == 2 && dy == 2 || dx == -2 && dy == 2 || dx == 2 && dy == -2 || dx == -2 && dy == -2)
					{
						flag = true;
					}
					if (!flag)
					{
						return;
					}
					if (!can_csg(m_pre_csg.x, m_pre_csg.y, x, y))
					{
						return;
					}
					add_y (y);
					game_data._instance.m_arrays[m_world][y][x].type = type;
					game_data._instance.m_arrays[m_world][m_pre_csg.y][m_pre_csg.x].param[2] = x;
					game_data._instance.m_arrays[m_world][m_pre_csg.y][m_pre_csg.x].param[3] = y;
					game_data._instance.m_arrays[m_world][y][x].param[0] = m_pre_csg.x;
					game_data._instance.m_arrays[m_world][y][x].param[1] = m_pre_csg.y;
					add_init(x, y, type);
					m_pre_csg = new mario_point(x, y);
				}
			}
			else if (type == utils.g_cspt)
			{
				if (y < game_data._instance.m_map_data.maps[m_world].y_num && game_data._instance.m_arrays[m_world][y][x].type != 0)
				{
					mario._instance.show_tip(game_data._instance.get_language_string("edit_mode_wffz"));
					return;
				}
				if (!has_csg(x, y, -1, -1))
				{
					mario._instance.show_tip(game_data._instance.get_language_string("edit_mode_xcsd"));
					return;
				}
				add_y (y);
				game_data._instance.m_arrays[m_world][y][x].type = type;
				add_init(x, y, type);
			}
			else if (type == utils.g_hg)
			{
				if (y < game_data._instance.m_map_data.maps[m_world].y_num && game_data._instance.m_arrays[m_world][y][x].type == utils.g_hg)
				{
					change1_init(x, y);
				}
				else if (y < game_data._instance.m_map_data.maps[m_world].y_num && game_data._instance.m_arrays[m_world][y][x].type != 0)
				{
					mario._instance.show_tip(game_data._instance.get_language_string("edit_mode_wffz"));
					return;
				}
				else
				{
					add_y (y);
					game_data._instance.m_arrays[m_world][y][x].type = type;
					add_init(x, y, type);
				}
			}
			else if (type == utils.g_csm)
			{
				if (y < game_data._instance.m_map_data.maps[m_world].y_num && game_data._instance.m_arrays[m_world][y][x].type != 0)
				{
					mario._instance.show_tip(game_data._instance.get_language_string("edit_mode_wffz"));
					return;
				}
				add_y (y);
				if (!m_first_csm)
				{
					m_csm_id = game_data._instance.get_new_csm();
					if (m_csm_id == -1)
					{
						mario._instance.show_tip(game_data._instance.get_language_string("edit_mode_ccsm"));
						return;
					}
					m_csm_w = m_world;
					m_csm_x = x;
					m_csm_y = y;
					game_data._instance.m_arrays[m_world][y][x].type = type;
					game_data._instance.m_arrays[m_world][y][x].param[0] = m_csm_id;
					m_first_csm = true;
				}
				else
				{
					game_data._instance.m_arrays[m_world][y][x].type = type;
					game_data._instance.m_arrays[m_world][y][x].param[0] = m_csm_id;
					game_data._instance.m_arrays[m_world][y][x].param[1] = 1;
					m_first_csm = false;
				}
				add_init(x, y, type);
			}
			else
			{
				if (y < game_data._instance.m_map_data.maps[m_world].y_num && game_data._instance.m_arrays[m_world][y][x].type != 0)
				{
					s_t_unit t_unit1 = game_data._instance.get_t_unit(game_data._instance.m_arrays[m_world][y][x].type);
					bool flag = false;
					if (game_data._instance.m_arrays[m_world][y][x].param[0] == 0)
					{
						if (t_unit1.fwt == 1 && t_unit.fwt >= 10)
						{
							flag = true;
						}
						else if (t_unit1.fwt == 2 && t_unit.fwt == 11)
						{
							flag = true;
						}
					}
					if (flag)
					{
						game_data._instance.m_arrays[m_world][y][x].param[0] = type;
						set_init(x, y, type);
					}
					else
					{
						mario._instance.show_tip(game_data._instance.get_language_string("edit_mode_wffz"));
					}
				}
				else
				{
					if (t_unit.max_num > 0 && game_data._instance.get_unit_num(m_world, type) >= t_unit.max_num)
					{
						mario._instance.show_tip(game_data._instance.get_language_string("edit_mode_zdsl"));
						return;
					}
					add_y (y);
					game_data._instance.m_arrays[m_world][y][x].type = type;
					add_init(x, y, type);
				}
			}
		}
		else if (type == 0 && y < game_data._instance.m_map_data.maps[m_world].y_num)
		{
			del_obj(x, y);
		}
		else if (type == -1)
		{
			if (y < game_data._instance.m_map_data.maps[m_world].y_num && game_data._instance.m_arrays[m_world][y][x].type != 0)
			{
				change_init(x, y);
			}
		}
	}

	void del_obj(int x, int y, bool flag = false)
	{
		if (game_data._instance.m_arrays[m_world][y][x].type == 0 || game_data._instance.m_arrays[m_world][y][x].type >= 1000)
		{
			return;
		}
		s_t_unit t_unit = game_data._instance.get_t_unit(game_data._instance.m_arrays[m_world][y][x].type);
		if (!flag && t_unit.fwt == 1 && game_data._instance.m_arrays[m_world][y][x].param[0] != 0)
		{
			for (int i = 0; i < game_data._instance.m_arrays[m_world][y][x].param.Count; ++i)
			{
				game_data._instance.m_arrays[m_world][y][x].param[i] = 0;
			}
			set_init(x, y, 0);
		}
		else if (game_data._instance.m_arrays[m_world][y][x].type == 1)
		{
			for (int i = game_data._instance.m_map_data.maps[m_world].y_num - 1; i >= y; --i)
			{
				if (game_data._instance.m_arrays[m_world][i][x].type == 1)
				{
					game_data._instance.m_arrays[m_world][i][x].type = 0;
					del_init(x, i);
					del_y(i);
				}
			}
			reset_db();
		}
		else if (game_data._instance.m_arrays[m_world][y][x].type == utils.g_csg)
		{
			int xx = x;
			int yy = y;
			int px = game_data._instance.m_arrays[m_world][y][x].param[0];
			int py = game_data._instance.m_arrays[m_world][y][x].param[1];
			if (px != 0 || py != 0)
			{
				int ix = px + (xx - px) / 2;
				int jy = py + (yy - py) / 2;
				if (game_data._instance.m_arrays[m_world][jy][ix].type == utils.g_cspt)
				{
					game_data._instance.m_arrays[m_world][jy][ix].type = 0;
					del_init(ix, jy);
					del_y(jy);
				}
				game_data._instance.m_arrays[m_world][py][px].param[2] = 0;
				game_data._instance.m_arrays[m_world][py][px].param[3] = 0;
			}
			while (xx != 0 || yy != 0)
			{
				int i = xx;
				int j = yy;
				xx = game_data._instance.m_arrays[m_world][j][i].param[2];
				yy = game_data._instance.m_arrays[m_world][j][i].param[3];
				if (xx != 0 || yy != 0)
				{
					int ix = i + (xx - i) / 2;
					int jy = j + (yy - j) / 2;
					if (game_data._instance.m_arrays[m_world][jy][ix].type == utils.g_cspt)
					{
						game_data._instance.m_arrays[m_world][jy][ix].type = 0;
						del_init(ix, jy);
						del_y(jy);
					}
				}
				game_data._instance.m_arrays[m_world][j][i].type = 0;
				del_init(i, j);
				del_y(j);
			}
		}
		else if (game_data._instance.m_arrays[m_world][y][x].type == utils.g_csm)
		{
			if (m_first_csm && game_data._instance.m_arrays[m_world][y][x].param[0] == m_csm_id)
			{
				m_first_csm = false;
			}
			else
			{
				int iw = 0;
				int ix = 0;
				int iy = 0;
				game_data._instance.get_csm(m_world, x, y, ref iw, ref ix, ref iy);
				game_data._instance.m_arrays[iw][iy][ix].type = 0;
				if (iw == m_world)
				{
					del_init(ix, iy);
					del_y(iy);
				}
			}
			game_data._instance.m_arrays[m_world][y][x].type = 0;
			del_init(x, y);
			del_y(y);
		}
		else
		{
			game_data._instance.m_arrays[m_world][y][x].type = 0;
			del_init(x, y);
			del_y(y);
		}
	}

	bool can_add(int x, int y)
	{
		if (x < 0 || x >= game_data._instance.m_map_data.maps[m_world].x_num || y < 0 || y >= game_data._instance.m_map_data.maps[m_world].y_num)
		{
			return false;
		}
		if (x < 3 && y < game_data._instance.m_map_data.maps[m_world].qd_y)
		{
			return false;
		}
		if (x >= game_data._instance.m_map_data.maps[m_world].x_num - 3 && y < game_data._instance.m_map_data.maps[m_world].zd_y)
		{
			return false;
		}
		return true;
	}

	bool can_add_ex(int x, int y)
	{
		if (x < 0 || x >= game_data._instance.m_map_data.maps[m_world].x_num || y < 0 || y >= utils.g_max_y)
		{
			return false;
		}
		if (x < 3 && y < game_data._instance.m_map_data.maps[m_world].qd_y)
		{
			return false;
		}
		if (x >= game_data._instance.m_map_data.maps[m_world].x_num - 3 && y < game_data._instance.m_map_data.maps[m_world].zd_y)
		{
			return false;
		}
		return true;
	}

	public void not_add_obj()
	{
		m_pre_csg = null;
		m_lv.SetActive (false);
		for (int i = 0; i < m_hongs.Count; ++i)
		{
			m_hongs[i].SetActive(false);
		}
	}

	public void set_mpos(int x, int y)
	{
		if (m_pre_csg != null)
		{
			int px = m_pre_csg.x * utils.g_grid_size + utils.g_grid_size / 2;
			int py = m_pre_csg.y * utils.g_grid_size + utils.g_grid_size / 2;
			int xx = (px + x) / 2;
			int yy = (py + y) / 2;
			float dx = (float)(x - px) / 10;
			float dy = (float)(y - py) / 10;
			float l = Mathf.Sqrt(dx * dx + dy * dy) / utils.g_grid_size * 10;
			float a = Mathf.Atan2(dy, dx);
			a = a * 180.0f / Mathf.PI;
			m_lv.transform.localPosition = new Vector3(xx / 10, yy / 10, 0);
			m_lv.transform.localScale = new Vector3(l * 4, 0.2f, 0);
			m_lv.transform.localEulerAngles = new Vector3(0, 0, a);
			m_lv.SetActive(true);
			for (int i = 0; i < m_hongs.Count; ++i)
			{
				int hx = m_pre_csg.x + utils.csg_points[i, 0];
				int hy = m_pre_csg.y + utils.csg_points[i, 1];
				if (can_csg(m_pre_csg.x, m_pre_csg.y, hx, hy))
				{
					m_hongs[i].SetActive(true);
					hx = hx * utils.g_grid_size + utils.g_grid_size / 2;
					hy = hy * utils.g_grid_size + utils.g_grid_size / 2;
					m_hongs[i].transform.localPosition = new Vector3(hx / 10, hy / 10, 0);
				}
				else
				{
					m_hongs[i].SetActive(false);
				}
			}
		}
	}

	bool can_csg(int x1, int y1, int x2, int y2)
	{
		if (!can_add_ex(x2, y2))
		{
			return false;
		}
		if (y2 < game_data._instance.m_map_data.maps[m_world].y_num && game_data._instance.m_arrays[m_world][y2][x2].type != 0)
		{
			return false;
		}
		if (has_csg(x2, y2, x1, y1))
		{
			return false;
		}
		int dx = x1 + (x2 - x1) / 2;
		int dy = y1 + (y2 - y1) / 2;
		if (!can_add_ex(dx, dy))
		{
			return false;
		}
		if (dy < game_data._instance.m_map_data.maps[m_world].y_num && game_data._instance.m_arrays[m_world][dy][dx].type == utils.g_csg)
		{
			return false;
		}
		if (has_csg(dx, dy, x1, y1))
		{
			return false;
		}

		return true;
	}

	bool has_csg(int x, int y, int px, int py)
	{
		for (int i = 0; i < 4; ++i)
		{
			int x1 = x + utils.csg_points[i * 2, 0] / 2;
			int y1 = y + utils.csg_points[i * 2, 1] / 2;
			int x2 = x + utils.csg_points[i * 2 + 1, 0] / 2;
			int y2 = y + utils.csg_points[i * 2 + 1, 1] / 2;
			if (x1 == px && y1 == py)
			{
				continue;
			}
			if (x2 == px && y2 == py)
			{
				continue;
			}
			if (!can_add(x1, y1))
			{
				continue;
			}
			if (!can_add(x2, y2))
			{
				continue;
			}
			if (game_data._instance.m_arrays[m_world][y1][x1].type != utils.g_csg)
			{
				continue;
			}
			if (game_data._instance.m_arrays[m_world][y2][x2].type != utils.g_csg)
			{
				continue;
			}
			if (game_data._instance.m_arrays[m_world][y1][x1].param[0] == x2 && game_data._instance.m_arrays[m_world][y1][x1].param[1] == y2)
			{
				return true;
			}
			if (game_data._instance.m_arrays[m_world][y1][x1].param[2] == x2 && game_data._instance.m_arrays[m_world][y1][x1].param[3] == y2)
			{
				return true;
			}
		}
		return false;
	}

	bool check_sw(int x, int y)
	{
		int num = 0;
		for (int i = x - utils.g_active_x; i <= x + utils.g_active_x; ++i)
		{
			if (i < 0 || i >= game_data._instance.m_map_data.maps[m_world].x_num)
			{
				continue;
			}
			for (int j = y - utils.g_active_y; j <= y + utils.g_active_y; ++j)
			{
				if (j < 0 || j >= game_data._instance.m_map_data.maps[m_world].y_num)
				{
					continue;
				}
				s_t_unit t_unit = game_data._instance.get_t_unit(game_data._instance.m_arrays[m_world][j][i].type);
				if (t_unit == null)
				{
					continue;
				}
				if (t_unit.is_sw == 1)
				{
					num++;
				}
			}
		}
		if (num >= 30)
		{
			return false;
		}
		return true;
	}

	void reset_db()
	{
		for (int i = 0; i < m_objs.Count; ++i)
		{
			mario_obj mobj = m_objs[i].GetComponent<mario_obj>();
			if (mobj.m_unit != null && mobj.m_unit.id == 1)
			{
				mobj.change();
			}
		}
	}

	public void reset_zhuiji()
	{
		if (game_data._instance.m_map_data.mode != 0 && m_zhuiji == null)
		{
			GameObject res = (GameObject)Resources.Load ("unit/other/zhuiji");
			m_zhuiji = (GameObject)Instantiate(res);
			m_zhuiji.transform.parent = m_other.transform;
			m_zhuiji.transform.localPosition = new Vector3 ((m_main_char.m_pos.x - utils.g_grid_size * 5) / 10, utils.g_height / 2 / 10, 0);
			m_zhuiji.transform.localScale = new Vector3 (1, 1, 1);
		}
		else if (game_data._instance.m_map_data.mode == 0 && m_zhuiji != null)
		{
			Object.Destroy(m_zhuiji);
			m_zhuiji = null;
		}
	}

	public void check_mission()
	{
		if (m_first_csm)
		{
			game_data._instance.m_arrays[m_csm_w][m_csm_y][m_csm_x].type = 0;
			if (m_world == m_csm_w)
			{
				del_init(m_csm_x, m_csm_y);
				del_y(m_csm_y);
			}
			m_first_csm = false;
			mario._instance.show_tip(game_data._instance.get_language_string("edit_mode_qcsm"));
		}
	}

	void add_y(int y)
	{
		if (y >= game_data._instance.m_map_data.maps[m_world].y_num)
		{
			for (int i = game_data._instance.m_arrays[m_world].Count; i < y + 1; ++i)
			{
				List<s_t_mission_sub> arr = new List<s_t_mission_sub>();
				for (int j = 0; j < game_data._instance.m_map_data.maps[m_world].x_num; ++j)
				{
					s_t_mission_sub sub = new s_t_mission_sub();
					sub.type = 0;
					arr.Add(sub);
				}
				game_data._instance.m_arrays[m_world].Add(arr);
			}
			game_data._instance.m_map_data.maps[m_world].y_num = y + 1;
		}
	}
	
	void del_y(int y)
	{
		while (y == game_data._instance.m_map_data.maps[m_world].y_num - 1 && y > utils.g_min_y && y > game_data._instance.m_map_data.maps[m_world].qd_y + 4 && y > game_data._instance.m_map_data.maps[m_world].zd_y + 4)
		{
			bool flag = false;
			for (int j = 0; j < game_data._instance.m_map_data.maps[m_world].x_num; ++j)
			{
				if (game_data._instance.m_arrays[m_world][y][j].type != 0)
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				game_data._instance.m_map_data.maps[m_world].y_num--;
				game_data._instance.m_arrays[m_world].RemoveAt(y);
			}
			y--;
		}
	}

	void add_init(int x, int y, int type)
	{
		s_t_unit t_unit = game_data._instance.get_t_unit(type);
		if (t_unit == null)
		{
			return;
		}
		create_mario_obj(t_unit.res, t_unit, game_data._instance.m_arrays[m_world][y][x].param, x, y, true);
		if (t_unit != null && t_unit.id == utils.g_yinfu)
		{
			mario._instance.play_sound ("sound/yf/0-" + (y + 1).ToString());
		}
		else if (t_unit == null || t_unit.id != 1)
		{
			utils.do_yfu(utils.yuepu[m_yf * 2], utils.yuepu[m_yf * 2 + 1]);
			m_yf++;
			if (m_yf * 2 >= utils.yuepu.Length)
			{
				m_yf = 0;
			}
		}
	}
	
	void del_init(int x, int y)
	{
		for (int i = m_objs.Count - 1; i >= 0; --i)
		{
			mario_obj obj = m_objs[i];
			if (obj.m_init_pos.x == x && obj.m_init_pos.y == y)
			{
				if (obj.m_unit != null && obj.m_unit.id != 1)
				{
					utils.do_yfu(utils.yuepu[m_yf * 2], utils.yuepu[m_yf * 2 + 1]);
					m_yf++;
					if (m_yf * 2 >= utils.yuepu.Length)
					{
						m_yf = 0;
					}
				}
				del_obj(obj);
				obj.destroy_shadow();
				UnityEngine.Object.Destroy(obj.gameObject);
			}
		}
	}

	void change_init(int x, int y)
	{
		for (int i = 0; i < m_objs.Count; ++i)
		{
			if (m_objs[i].m_unit != null && m_objs[i].m_init_pos.x == x && m_objs[i].m_init_pos.y == y)
			{
				m_objs[i].GetComponent<mario_obj>().change();
				break;
			}
		}
	}

	void change1_init(int x, int y)
	{
		for (int i = 0; i < m_objs.Count; ++i)
		{
			if (m_objs[i].m_unit != null && m_objs[i].m_init_pos.x == x && m_objs[i].m_init_pos.y == y)
			{
				m_objs[i].GetComponent<mario_obj>().change1();
				break;
			}
		}
	}

	void set_init(int x, int y, int type)
	{
		for (int i = 0; i < m_objs.Count; ++i)
		{
			if (m_objs[i].m_unit != null && m_objs[i].m_init_pos.x == x && m_objs[i].m_init_pos.y == y)
			{
				for (int k = 0; k < m_objs[i].GetComponent<mario_obj>().m_param.Count; ++k)
				{
					m_objs[i].GetComponent<mario_obj>().m_param[k] = 0;
				}
				m_objs[i].GetComponent<mario_obj>().m_param[0] = type;
				m_objs[i].GetComponent<mario_obj>().reset ();
				break;
			}
		}
	}

	public void add_qz(int type, int x, int y)
	{
		m_add_qz_type = type;
		m_add_qz_x = x;
		m_add_qz_y = y;
	}

	void update_ex()
	{
		for (int i = 0; i < m_objs.Count; ++i)
		{
			m_objs[i].update_ex(m_grid);
		}
		m_main_char.update_ex (m_grid);
		m_scene.update_ex (m_roll);
		if (game_data._instance.m_map_data.mode != 0)
		{
			m_zhuiji.transform.localPosition = new Vector3 ((m_roll.x - utils.g_grid_size * 5) / 10, (m_roll.y + utils.g_roll_y) / 10, 0);
		}
		m_fuzhu.transform.localPosition = new Vector3(-m_roll.x % utils.g_grid_size / 10, -m_roll.y % utils.g_grid_size / 10, 0);
		m_ban.transform.localPosition = new Vector3(-utils.g_grid_size / 2 / 10, utils.g_grid_size * 15 / 10, 0);
		m_ban1.transform.localPosition = new Vector3((game_data._instance.m_map_data.maps[m_world].x_num * utils.g_grid_size + utils.g_grid_size / 2) / 10, utils.g_grid_size * 15 / 10, 0);
	}

	bool update_edit_cy()
	{
		if (m_edit_cys_num < m_edit_cys_tnum)
		{
			m_edit_cys_time++;
			bool nflag = false;
			if (m_edit_cys_num == 0)
			{
				if (m_edit_cys_time == 20)
				{
					nflag = true;
				}
			}
			else
			{
				if (m_edit_cys_time == 3)
				{
					nflag = true;
				}
			}
			while (nflag)
			{
				m_edit_cys_time = 0;
				nflag = !create_cy(m_edit_cys_num);
				m_edit_cys_num++;
				if (m_edit_cys_num >= m_edit_cys_tnum)
				{
					break;
				}
			}
			return false;
		}
		return true;
	}

	void pingyi()
	{
		if (m_add_qz_type > 0)
		{
			bool flag = false;
			if (m_add_qz_type == 1)
			{
				if (m_add_qz_y < 0 && game_data._instance.m_map_data.maps[m_world].qd_y > 2)
				{
					flag = true;
					int y = m_main_char.m_pos.y;
					m_main_char.m_pos.y -= 40;
					if (m_time > 30)
					{
						m_main_char.m_pos.y -= 80;
					}
					if (y / utils.g_grid_size != m_main_char.m_pos.y / utils.g_grid_size)
					{
						int yy = game_data._instance.m_map_data.maps[m_world].qd_y;
						game_data._instance.m_map_data.maps[m_world].qd_y--;
						game_data._instance.m_arrays[m_world][yy][1].type = 0;
						del_init(1, yy);
						del_y(yy + 4);
						for (int i = 0; i < 3; ++i)
						{
							game_data._instance.m_arrays[m_world][game_data._instance.m_map_data.maps[m_world].qd_y][i].type = 0;
							del_init(i, game_data._instance.m_map_data.maps[m_world].qd_y);
						}
						game_data._instance.m_arrays[m_world][game_data._instance.m_map_data.maps[m_world].qd_y][1].type = 1000;
						add_init(1, game_data._instance.m_map_data.maps[m_world].qd_y, 1000);
						reset_db();
					}
				}
				if (m_add_qz_y > 0 && game_data._instance.m_map_data.maps[m_world].qd_y < utils.g_max_y - 5)
				{
					flag = true;
					int y = m_main_char.m_pos.y;
					m_main_char.m_pos.y += 40;
					if (m_time > 30)
					{
						m_main_char.m_pos.y += 80;
					}
					if (y / utils.g_grid_size != m_main_char.m_pos.y / utils.g_grid_size)
					{
						int yy = game_data._instance.m_map_data.maps[m_world].qd_y;
						game_data._instance.m_map_data.maps[m_world].qd_y++;
						del_obj(1, yy + 1, true);
						for (int i = 0; i < 3; ++i)
						{
							del_obj(i, yy, true);
						}
						add_y (game_data._instance.m_map_data.maps[m_world].qd_y + 4);
						del_init(1, yy);
						for (int i = 0; i < 3; ++i)
						{
							game_data._instance.m_arrays[m_world][yy][i].type = 1;
							add_init(i, yy, 1);
						}
						game_data._instance.m_arrays[m_world][game_data._instance.m_map_data.maps[m_world].qd_y][1].type = 1000;
						add_init(1, game_data._instance.m_map_data.maps[m_world].qd_y, 1000);
						reset_db();
					}
				}
			}
			else
			{
				if (m_add_qz_y < 0 && game_data._instance.m_map_data.maps[m_world].zd_y > 2)
				{
					flag = true;
					int y = m_main_char.m_pos.y;
					m_main_char.m_pos.y -= 40;
					if (m_time > 30)
					{
						m_main_char.m_pos.y -= 80;
					}
					if (y / utils.g_grid_size != m_main_char.m_pos.y / utils.g_grid_size)
					{
						int yy = game_data._instance.m_map_data.maps[m_world].zd_y;
						game_data._instance.m_map_data.maps[m_world].zd_y--;
						game_data._instance.m_arrays[m_world][yy][game_data._instance.m_map_data.maps[m_world].x_num - 2].type = 0;
						del_init(game_data._instance.m_map_data.maps[m_world].x_num - 2, yy);
						del_y(yy + 4);
						for (int i = 0; i < 3; ++i)
						{
							game_data._instance.m_arrays[m_world][game_data._instance.m_map_data.maps[m_world].zd_y][game_data._instance.m_map_data.maps[m_world].x_num - i - 1].type = 0;
							del_init(game_data._instance.m_map_data.maps[m_world].x_num - i - 1, game_data._instance.m_map_data.maps[m_world].zd_y);
						}
						game_data._instance.m_arrays[m_world][game_data._instance.m_map_data.maps[m_world].zd_y][game_data._instance.m_map_data.maps[m_world].x_num - 2].type = 1001;
						add_init(game_data._instance.m_map_data.maps[m_world].x_num - 2, game_data._instance.m_map_data.maps[m_world].zd_y, 1001);
						reset_db();
					}
				}
				if (m_add_qz_y > 0 && game_data._instance.m_map_data.maps[m_world].zd_y < utils.g_max_y - 5)
				{
					flag = true;
					int y = m_main_char.m_pos.y;
					m_main_char.m_pos.y += 40;
					if (m_time > 30)
					{
						m_main_char.m_pos.y += 80;
					}
					if (y / utils.g_grid_size != m_main_char.m_pos.y / utils.g_grid_size)
					{
						int yy = game_data._instance.m_map_data.maps[m_world].zd_y;
						game_data._instance.m_map_data.maps[m_world].zd_y++;
						del_obj(game_data._instance.m_map_data.maps[m_world].x_num - 2, yy + 1, true);
						for (int i = 0; i < 3; ++i)
						{
							del_obj(game_data._instance.m_map_data.maps[m_world].x_num - i - 1, yy, true);
						}
						add_y (game_data._instance.m_map_data.maps[m_world].zd_y + 4);
						del_init(game_data._instance.m_map_data.maps[m_world].x_num - 2, yy);
						for (int i = 0; i < 3; ++i)
						{
							game_data._instance.m_arrays[m_world][yy][game_data._instance.m_map_data.maps[m_world].x_num - i - 1].type = 1;
							add_init(game_data._instance.m_map_data.maps[m_world].x_num - i - 1, yy, 1);
						}
						game_data._instance.m_arrays[m_world][game_data._instance.m_map_data.maps[m_world].zd_y][game_data._instance.m_map_data.maps[m_world].x_num - 2].type = 1001;
						add_init(game_data._instance.m_map_data.maps[m_world].x_num - 2, game_data._instance.m_map_data.maps[m_world].zd_y, 1001);
						reset_db();
					}
				}
				if (m_add_qz_x < 0 && game_data._instance.m_map_data.maps[m_world].x_num > utils.g_min_x)
				{
					flag = true;
					int x = m_main_char.m_pos.x;
					m_main_char.m_pos.x -= 40;
					if (m_time > 30)
					{
						m_main_char.m_pos.x -= 80;
					}
					if (x / utils.g_grid_size != m_main_char.m_pos.x / utils.g_grid_size)
					{
						for (int j = 0; j < game_data._instance.m_map_data.maps[m_world].zd_y; ++j)
						{
							for (int i = 1; i < 4; ++i)
							{
								del_obj(game_data._instance.m_map_data.maps[m_world].x_num - i - 1, j, true);
							}
						}
						for (int j = 0; j < game_data._instance.m_map_data.maps[m_world].y_num; ++j)
						{
							del_obj(game_data._instance.m_map_data.maps[m_world].x_num - 1, j, true);
						}
						del_obj(game_data._instance.m_map_data.maps[m_world].x_num - 3, game_data._instance.m_map_data.maps[m_world].zd_y, true);
						del_init(game_data._instance.m_map_data.maps[m_world].x_num - 2, game_data._instance.m_map_data.maps[m_world].zd_y);
						game_data._instance.m_arrays[m_world][game_data._instance.m_map_data.maps[m_world].zd_y][game_data._instance.m_map_data.maps[m_world].x_num - 2].type = 0;
						game_data._instance.m_map_data.maps[m_world].x_num--;
						for (int j = 0; j < game_data._instance.m_map_data.maps[m_world].y_num; ++j)
						{
							game_data._instance.m_arrays[m_world][j].RemoveAt(game_data._instance.m_map_data.maps[m_world].x_num);
						}
						for (int j = 0; j < game_data._instance.m_map_data.maps[m_world].zd_y; ++j)
						{
							for (int i = 2; i >= 0; --i)
							{
								game_data._instance.m_arrays[m_world][j][game_data._instance.m_map_data.maps[m_world].x_num - i - 1].type = 1;
								add_init(game_data._instance.m_map_data.maps[m_world].x_num - i - 1, j, 1);
							}
						}
						game_data._instance.m_arrays[m_world][game_data._instance.m_map_data.maps[m_world].zd_y][game_data._instance.m_map_data.maps[m_world].x_num - 2].type = 1001;
						add_init(game_data._instance.m_map_data.maps[m_world].x_num - 2, game_data._instance.m_map_data.maps[m_world].zd_y, 1001);
						reset_db();
					}
				}
				if (m_add_qz_x > 0 && game_data._instance.m_map_data.maps[m_world].x_num < utils.g_max_x)
				{
					flag = true;
					int x = m_main_char.m_pos.x;
					m_main_char.m_pos.x += 40;
					if (m_time > 30)
					{
						m_main_char.m_pos.x += 80;
					}
					if (x / utils.g_grid_size != m_main_char.m_pos.x / utils.g_grid_size)
					{
						for (int j = 0; j < game_data._instance.m_map_data.maps[m_world].zd_y; ++j)
						{
							for (int i = 0; i < 3; ++i)
							{
								del_obj(game_data._instance.m_map_data.maps[m_world].x_num - i - 1, j, true);
							}
						}
						del_obj(game_data._instance.m_map_data.maps[m_world].x_num - 1, game_data._instance.m_map_data.maps[m_world].zd_y, true);
						del_init(game_data._instance.m_map_data.maps[m_world].x_num - 2, game_data._instance.m_map_data.maps[m_world].zd_y);
						game_data._instance.m_arrays[m_world][game_data._instance.m_map_data.maps[m_world].zd_y][game_data._instance.m_map_data.maps[m_world].x_num - 2].type = 0;
						game_data._instance.m_map_data.maps[m_world].x_num++;
						for (int j = 0; j < game_data._instance.m_map_data.maps[m_world].y_num; ++j)
						{
							s_t_mission_sub sub = new s_t_mission_sub();
							sub.type = 0;
							game_data._instance.m_arrays[m_world][j].Add(sub);
						}
						for (int j = 0; j < game_data._instance.m_map_data.maps[m_world].zd_y; ++j)
						{
							for (int i = 2; i >= 0; --i)
							{
								game_data._instance.m_arrays[m_world][j][game_data._instance.m_map_data.maps[m_world].x_num - i - 1].type = 1;
								add_init(game_data._instance.m_map_data.maps[m_world].x_num - i - 1, j, 1);
							}
						}
						game_data._instance.m_arrays[m_world][game_data._instance.m_map_data.maps[m_world].zd_y][game_data._instance.m_map_data.maps[m_world].x_num - 2].type = 1001;
						add_init(game_data._instance.m_map_data.maps[m_world].x_num - 2, game_data._instance.m_map_data.maps[m_world].zd_y, 1001);
						reset_db();
					}
				}
			}
			if (flag)
			{
				m_time++;
			}
			else
			{
				m_time = 0;
			}
		}
		else
		{
			bool flag = false;
			if (m_left)
			{
				flag = true;
				m_main_char.m_pos.x -= 40;
				if (m_time > 30)
				{
					m_main_char.m_pos.x -= 80;
				}
			}
			if (m_right)
			{
				flag = true;
				m_main_char.m_pos.x += 40;
				if (m_time > 30)
				{
					m_main_char.m_pos.x += 80;
				}
			}
			if (m_up)
			{
				flag = true;
				m_main_char.m_pos.y += 40;
				if (m_time > 30)
				{
					m_main_char.m_pos.y += 80;
				}
			}
			if (m_down)
			{
				flag = true;
				m_main_char.m_pos.y -= 40;
				if (m_time > 30)
				{
					m_main_char.m_pos.y -= 80;
				}
			}
			if (flag)
			{
				m_time++;
			}
			else
			{
				m_time = 0;
			}
		}
	}

	void check_roll()
	{
		if (m_main_char.m_pos.x < 0)
		{
			m_main_char.m_pos.x = 0;
		}
		if (m_main_char.m_pos.x >= game_data._instance.m_map_data.maps[m_world].x_num * utils.g_grid_size)
		{
			m_main_char.m_pos.x = game_data._instance.m_map_data.maps[m_world].x_num * utils.g_grid_size - 1;
		}
		if (m_main_char.m_pos.y < 0)
		{
			m_main_char.m_pos.y = 0;
		}
		if (m_main_char.m_pos.y >= 26 * utils.g_grid_size)
		{
			m_main_char.m_pos.y = 26 * utils.g_grid_size - 1;
		}
		
		m_roll.x = m_main_char.m_pos.x;
		m_roll.y = m_main_char.m_pos.y - utils.g_roll_y;
		if (m_roll.y + utils.g_height > 30 * utils.g_grid_size)
		{
			m_roll.y = 30 * utils.g_grid_size - utils.g_height;
		}
		if (m_roll.y < 0)
		{
			m_roll.y = 0;
		}
		
		bool bflag = false;
		int px = m_roll.x / utils.g_grid_size;
		if (px != m_grid.x)
		{
			bflag = true;
			int ix = m_grid.x + utils.g_active_x;
			if (m_grid.x > px)
			{
				ix = m_grid.x - utils.g_active_x;
			}
			if (ix >= 0 && ix < game_data._instance.m_map_data.maps[m_world].x_num)
			{
				for (int j = m_grid.y - utils.g_active_y; j <= m_grid.y + utils.g_active_y; ++j)
				{
					if (j < 0 || j >= game_data._instance.m_map_data.maps[m_world].y_num)
					{
						continue;
					}
					do_create(ix, j);
				}
			}
			m_grid.x = px;
		}
		
		int py = (m_roll.y + utils.g_roll_y) / utils.g_grid_size;
		if (py != m_grid.y)
		{
			bflag = true;
			int jy = m_grid.y + utils.g_active_y;
			if (m_grid.y > py)
			{
				jy = m_grid.y - utils.g_active_y;
			}
			if (jy >= 0 && jy < game_data._instance.m_map_data.maps[m_world].y_num)
			{
				for (int i = m_grid.x - utils.g_active_x; i <= m_grid.x + utils.g_active_x; ++i)
				{
					if (i < 0 || i >= game_data._instance.m_map_data.maps[m_world].x_num)
					{
						continue;
					}
					do_create(i, jy);
				}
			}
			m_grid.y = py;
		}
		
		if (bflag)
		{
			do_delete();
			reset_db();
		}
	}

	void FixedUpdate ()
	{
		if (m_main_char == null)
		{
			return;
		}

		if (update_edit_cy())
		{
			pingyi ();
		}
		check_roll ();
		update_ex ();

		if (m_render.transform.localScale.x > 0.8f)
		{
			float s = m_render.transform.localScale.x - 0.01f;
			m_render.transform.localScale = new Vector3(s, s, 1.0f);
		}
		else if (m_render.transform.localScale.x < 0.8f)
		{
			m_render.transform.localScale = new Vector3(0.8f, 0.8f, 1.0f);
		}
	}

	bool create_cy(int index)
	{
		edit_cy ec = m_ecs[index];
		if (ec.world != m_world)
		{
			return false;
		}
		GameObject res = (GameObject)Resources.Load ("unit/other/cy");
		GameObject obj = (GameObject)Instantiate(res);
		obj.transform.parent = m_other.transform;
		obj.transform.localPosition = new Vector3 (ec.p.x / 10, ec.p.y / 10, 0);
		if (ec.fx == 0)
		{
			obj.transform.localScale = new Vector3 (-1, 1, 1);
		}
		else
		{
			obj.transform.localScale = new Vector3 (1, 1, 1);
		}
		obj.GetComponent<mario_edit_cy> ().reset (ec);
		int x = ec.p.x / utils.g_grid_size - m_grid.x;
		int y = ec.p.y / utils.g_grid_size - m_grid.y;
		if (x > utils.g_active_x || x < -utils.g_active_x || y > utils.g_active_y || y < -utils.g_active_y)
		{
			return false;
		}
		return true;
	}

	void do_create(int x, int y)
	{
		int key = y * utils.g_max_x + x;
		s_t_unit t_unit = game_data._instance.get_t_unit(game_data._instance.m_arrays[m_world][y][x].type);
		if (t_unit == null)
		{
			return;
		}
		if (m_has_create.Contains(key))
		{
			return;
		}
		create_mario_obj(t_unit.res, t_unit, game_data._instance.m_arrays[m_world][y][x].param, x, y);
	}

	void do_delete()
	{
		List<mario_obj> tos = new List<mario_obj>();
		for (int i = 0; i < m_objs.Count; ++i)
		{
			mario_obj obj = m_objs[i];
			int x = obj.m_grid.x - m_grid.x;
			int y = obj.m_grid.y - m_grid.y;
			if (x > utils.g_active_x || x < -utils.g_active_x || y > utils.g_active_y || y < -utils.g_active_y)
			{
				tos.Add(obj);
			}
		}
		for (int i = 0; i < tos.Count; ++i)
		{
			if (tos[i] == m_main_char)
			{
				m_main_char = null;
			}
			del_obj(tos[i]);
			tos[i].destroy_shadow();
			UnityEngine.Object.Destroy(tos[i].gameObject);
		}
	}

	void Update()
	{
		if (mario._instance.key_down(KeyCode.LeftArrow))
		{
			m_left = true;
		}
		if (mario._instance.key_up(KeyCode.LeftArrow))
		{
			m_left = false;
		}
		if (mario._instance.key_down(KeyCode.RightArrow))
		{
			m_right = true;
		} 
		if (mario._instance.key_up(KeyCode.RightArrow))
		{
			m_right = false;
		}
		if (mario._instance.key_down(KeyCode.UpArrow))
		{
			m_up = true;
		} 
		if (mario._instance.key_up(KeyCode.UpArrow))
		{
			m_up = false;
		}
		if (mario._instance.key_down(KeyCode.DownArrow))
		{
			m_down = true;
		} 
		if (mario._instance.key_up(KeyCode.DownArrow))
		{
			m_down = false;
		}
		m_cam.transform.localPosition = new Vector3 (m_roll.x / 10, (m_roll.y + utils.g_roll_y) / 10);
	}

	void joy_hold(Joystick joy)
	{
		float d = joy.Axis2Angle();
		if (d > -157.5 && d <= -112.5)
		{
			m_up = false;
			m_down = true;
			m_left = true;
			m_right = false;
		}
		else if (d > -112.5 && d <= -67.5)
		{
			m_up = false;
			m_down = false;
			m_left = true;
			m_right = false;
		}
		else if (d > -67.5 && d <= -22.5)
		{
			m_up = true;
			m_down = false;
			m_left = true;
			m_right = false;
		}
		else if (d > -22.5 && d <= 22.5)
		{
			m_up = true;
			m_down = false;
			m_left = false;
			m_right = false;
		}
		else if (d > 22.5 && d <= 67.5)
		{
			m_up = true;
			m_down = false;
			m_left = false;
			m_right = true;
		}
		else if (d > 67.5 && d <= 112.5)
		{
			m_up = false;
			m_down = false;
			m_left = false;
			m_right = true;
		}
		else if (d > 112.5 && d <= 157.5)
		{
			m_up = false;
			m_down = true;
			m_left = false;
			m_right = true;
		}
		else
		{
			m_up = false;
			m_down = true;
			m_left = false;
			m_right = false;
		}
	}

	void joy_move_end(Joystick joy)
	{
		m_up = false;
		m_down = false;
		m_left = false;
		m_right = false;
	}
}

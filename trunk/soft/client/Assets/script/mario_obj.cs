using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class mario_obj : MonoBehaviour {

	public enum mario_type
	{
		mt_null = 0,

		mt_block,
		mt_block1,
		mt_charater,
		mt_attack,
		mt_attack_ex,
		mt_attack_ex1,

		mt_end,
	}

	[HideInInspector]
	public string m_name = "";

	[HideInInspector]
	public s_t_unit m_unit = null;

	[HideInInspector]
	public List<int> m_param;

	[HideInInspector]
	public mario_type m_type = mario_type.mt_null;

	[HideInInspector]
	public mario_point m_pos = new mario_point();

	[HideInInspector]
	public mario_point m_grid = new mario_point();

	[HideInInspector]
	public mario_point m_per_pos = new mario_point();

	[HideInInspector]
	public mario_point m_per_grid = new mario_point();

	[HideInInspector]
	public Animator m_animator = null;

	[HideInInspector]
	public mario_anim m_mario_anim = null;

	public enum mario_fx
	{
		mf_left = 0,
		mf_right,
	}

	[HideInInspector]
	public mario_fx m_fx = mario_fx.mf_right;

	public void set_fx(mario_fx fx)
	{
		if (fx == m_fx)
		{
			return;
		}
		m_fx = fx;
		if (m_fx == mario_fx.mf_right)
		{
			set_scale(1, 1, 1);
		}
		else
		{
			set_scale(-1, 1, 1);
		}
	}

	[HideInInspector]
	public mario_point m_velocity = new mario_point();

	[HideInInspector]
	public mario_point m_pvelocity = new mario_point();

	[HideInInspector]
	public mario_collider m_collider = null;

	[HideInInspector]
	public mario_bound m_bound = new mario_bound();

	public mario_bound get_floor_bound()
	{
		return new mario_bound(m_bound.left, m_bound.right, m_bound.top, m_bound.bottom - 1);
	}

	public mario_bound get_left_bound()
	{
		return new mario_bound(m_bound.left - 1, m_bound.right, m_bound.top, m_bound.bottom);
	}

	public mario_bound get_right_bound()
	{
		return new mario_bound(m_bound.left, m_bound.right + 1, m_bound.top, m_bound.bottom);
	}

	[HideInInspector]
	public mario_bound m_per_bound = new mario_bound();

	[HideInInspector]
	public bool m_is_die = false;

	public bool is_die ()
	{
		return m_is_die || m_is_destory > 0;
	}

	[HideInInspector]
	public bool m_is_static = true;

	// 是否挡子弹
	[HideInInspector]
	public int m_is_dzd = 1;
	
	[HideInInspector]
	public bool m_main = false;

	[HideInInspector]
	public bool m_wgk = false;

	[HideInInspector]
	public int m_life = 0;

	[HideInInspector]
	public bool m_is_on_floor = false;

	[HideInInspector]
	public bool m_per_is_on_floor = false;

	[HideInInspector]
	public bool m_can_be_on_char = false;

	[HideInInspector]
	public bool m_is_on_char = false;

	[HideInInspector]
	public bool m_per_is_on_char = false;

	[HideInInspector]
	public int m_is_destory = 0;

	[HideInInspector]
	public bool m_bkcf = false;

	[HideInInspector]
	public List<mario_obj> m_nl_objs = new List<mario_obj>();

	[HideInInspector]
	public List<mario_obj> m_bnl_objs = new List<mario_obj>();

	[HideInInspector]
	public bool m_is_calc_nl = false;

	[HideInInspector]
	public mario_point m_nl_calc_point = new mario_point();

	[HideInInspector]
	public mario_point m_nl_point = new mario_point();

	[HideInInspector]
	public mario_point m_per_nl_point = new mario_point();

	[HideInInspector]
	public int m_fxdiv = 1;

	[HideInInspector]
	public int m_mocali = 4;

	[HideInInspector]
	public int m_min_speed = 0;

	[HideInInspector]
	public bool m_nleft = false;

	[HideInInspector]
	public bool m_nright = false;

	[HideInInspector]
	public bool m_has_floor = true;

	[HideInInspector]
	public bool m_has_main_floor = true;

	[HideInInspector]
	public mario_point m_init_pos = new mario_point();

	[HideInInspector]
	public int m_world = 0;

	[HideInInspector]
	public bool m_edit_mode = false;

	[HideInInspector]
	public bool m_has_edit = false;

	[HideInInspector]
	public bool m_is_move = false;

	[HideInInspector]
	public GameObject m_shadow = null;
	[HideInInspector]
	public mario_obj m_shadow_obj = null;

	[HideInInspector]
	public List<int> m_bl = new List<int>();

	[HideInInspector]
	public List<mario_obj> m_bl_objs = new List<mario_obj>();

	[HideInInspector]
	public mario_qtree m_qtree = null;

	[HideInInspector]
	public int m_no_mc_time = 0;

	[HideInInspector]
	public bool m_is_start;

	[HideInInspector]
	public bool m_is_new;

	public virtual void init(string name, List<int> param, int world, int x, int y, int xx, int yy)
	{
		m_name = name;
		m_param = param;
		m_collider = this.GetComponent<mario_collider> ();
		m_animator = this.GetComponent<Animator>();
		m_mario_anim = this.GetComponent<mario_anim>();
		if (m_mario_anim != null)
		{
			if (m_mario_anim.do_defualt())
			{
				if (m_animator != null)
				{
					m_animator.enabled = false;
				}
			}
			else
			{
				m_mario_anim.enabled = false;
			}
		}
		m_pos.set(xx, yy);
		reset_bound ();
		record_per();
		set_pos ();
		m_world = world;
		m_init_pos.set (x, y);
		reset ();

		if (x != -1)
		{
			if (x > 0)
			{
				int type = game_data._instance.m_arrays[m_world][y][x - 1].type;
				if (type != 0)
				{
					s_t_unit t_unit = game_data._instance.get_t_unit(type);
					if (t_unit.is_static == 1)
					{
						m_nleft = true;
					}
				}
			}
			if (x < game_data._instance.m_map_data.maps[m_world].x_num - 1)
			{
				int type = game_data._instance.m_arrays[m_world][y][x + 1].type;
				if (type != 0)
				{
					s_t_unit t_unit = game_data._instance.get_t_unit(type);
					if (t_unit.is_static == 1)
					{
						m_nright = true;
					}
				}
			}
		}
	}

	public virtual void reset()
	{
	}

	public void create_shadow(GameObject back)
	{
		if (m_shadow != null)
		{
			Object.Destroy(m_shadow);
		}
		GameObject obj = (GameObject)Instantiate(this.gameObject);
		obj.transform.parent = back.transform;
		obj.transform.localPosition = this.transform.localPosition;
		obj.transform.localScale = this.transform.localScale;
		obj.transform.localEulerAngles = this.transform.localEulerAngles;
		m_shadow = obj;
		m_shadow_obj = m_shadow.GetComponent<mario_obj> ();
		if (m_shadow_obj.m_mario_anim != null)
		{
			if (m_shadow_obj.m_mario_anim.do_defualt())
			{
				if (m_shadow_obj.m_animator != null)
				{
					m_shadow_obj.m_animator.enabled = false;
				}
			}
			else
			{
				m_shadow_obj.m_mario_anim.enabled = false;
			}
		}

		format_shadow (m_shadow.transform, "shader/shadow", -10);
	}

	public void format_shadow(Transform ts, string name, int layer)
	{
		SpriteRenderer sr = ts.GetComponent<SpriteRenderer> ();
		if (sr != null)
		{
			Shader s = Resources.Load(name, typeof(Shader)) as Shader;
			sr.sharedMaterial = new Material(s);
			sr.sortingOrder = layer;
		}
		for (int i = 0; i < ts.childCount; ++i)
		{
			format_shadow(ts.GetChild(i), name, layer);
		}
	}

	public void destroy_shadow()
	{
		if (m_shadow != null)
		{
			Object.Destroy(m_shadow);
		}
	}

	public void reset_bound()
	{
		m_grid.x = m_pos.x / utils.g_grid_size;
		m_grid.y = m_pos.y / utils.g_grid_size;

		if (m_collider == null)
		{
			return;
		}
		m_bound.left = m_pos.x + m_collider.m_rect.x - m_collider.m_rect.w / 2;
		m_bound.right = m_pos.x + m_collider.m_rect.x + m_collider.m_rect.w / 2 - 1;
		m_bound.top = m_pos.y + m_collider.m_rect.y + m_collider.m_rect.h / 2 - 1;
		m_bound.bottom = m_pos.y + m_collider.m_rect.y - m_collider.m_rect.h / 2;

		int div1 = m_fxdiv == 1 ? 1 : m_fxdiv - 1;
		m_bound.left_div = m_pos.x + m_collider.m_rect.x - m_collider.m_rect.w / 2 * div1 / m_fxdiv;
		m_bound.right_div = m_pos.x + m_collider.m_rect.x + m_collider.m_rect.w / 2 * div1 / m_fxdiv - 1;
		m_bound.top_div = m_pos.y + m_collider.m_rect.y + m_collider.m_rect.h / 2 * div1 / m_fxdiv - 1;
		m_bound.bottom_div = m_pos.y + m_collider.m_rect.y - m_collider.m_rect.h / 2 * div1 / m_fxdiv;
	}

	void record_per()
	{
		m_per_pos.set (m_pos.x, m_pos.y);
		m_per_grid.set (m_grid.x, m_grid.y);
		m_per_is_on_floor = m_is_on_floor;
		m_per_is_on_char = m_is_on_char;
		m_per_bound.left = m_bound.left;
		m_per_bound.right = m_bound.right;
		m_per_bound.top = m_bound.top;
		m_per_bound.bottom = m_bound.bottom;
		m_per_bound.left_div = m_bound.left_div;
		m_per_bound.right_div = m_bound.right_div;
		m_per_bound.top_div = m_bound.top_div;
		m_per_bound.bottom_div = m_bound.bottom_div;
	}

	public bool check_on_floor(mario_obj obj)
	{
		if (!obj.m_has_floor)
		{
			return false;
		}
		if (m_collider == null || obj.m_collider == null)
		{
			return false;
		}
		return m_bound.bottom - 1 == obj.m_bound.top && m_bound.right >= obj.m_bound.left && m_bound.left <= obj.m_bound.right;
	}
	
	public bool check_left_floor(mario_obj obj)
	{
		if (!obj.m_has_floor)
		{
			return false;
		}
		if (m_collider == null || obj.m_collider == null)
		{
			return false;
		}
		return m_bound.left - 1 == obj.m_bound.right && m_bound.top >= obj.m_bound.bottom && m_bound.top <= obj.m_bound.top;
	}

	public bool check_right_floor(mario_obj obj)
	{
		if (!obj.m_has_floor)
		{
			return false;
		}
		if (m_collider == null || obj.m_collider == null)
		{
			return false;
		}
		return m_bound.right + 1 == obj.m_bound.left && m_bound.top >= obj.m_bound.bottom && m_bound.top <= obj.m_bound.top;
	}

	public bool left_hit(mario_obj obj)
	{
		if (m_collider == null || obj.m_collider == null)
		{
			return false;
		}
		if (m_main)
		{
			return m_bound.top >= obj.m_bound.bottom_div && m_bound.bottom <= obj.m_bound.top_div && m_per_bound.left > obj.m_per_bound.right && m_bound.left <= obj.m_bound.right;
		}
		return m_bound.top >= obj.m_bound.bottom && m_bound.bottom <= obj.m_bound.top && m_per_bound.left > obj.m_per_bound.right && m_bound.left <= obj.m_bound.right;
	}

	public bool right_hit(mario_obj obj)
	{
		if (m_collider == null || obj.m_collider == null)
		{
			return false;
		}
		if (m_main)
		{
			return m_bound.top >= obj.m_bound.bottom_div && m_bound.bottom <= obj.m_bound.top_div && m_per_bound.right < obj.m_per_bound.left && m_bound.right >= obj.m_bound.left;
		}
		return m_bound.top >= obj.m_bound.bottom && m_bound.bottom <= obj.m_bound.top && m_per_bound.right < obj.m_per_bound.left && m_bound.right >= obj.m_bound.left;
	}

	public bool top_hit(mario_obj obj)
	{
		if (m_collider == null || obj.m_collider == null)
		{
			return false;
		}
		if (m_main)
		{
			return m_bound.right >= obj.m_bound.left_div && m_bound.left <= obj.m_bound.right_div && m_per_bound.top < obj.m_per_bound.bottom && m_bound.top >= obj.m_bound.bottom;
		}
		return m_bound.right >= obj.m_bound.left && m_bound.left <= obj.m_bound.right && m_per_bound.top < obj.m_per_bound.bottom && m_bound.top >= obj.m_bound.bottom;
	}

	public bool bottom_hit(mario_obj obj)
	{
		if (m_collider == null || obj.m_collider == null)
		{
			return false;
		}
		if (m_main)
		{
			return m_bound.right >= obj.m_bound.left_div && m_bound.left <= obj.m_bound.right_div && m_per_bound.bottom > obj.m_per_bound.top && m_bound.bottom <= obj.m_bound.top;
		}
		return m_bound.right >= obj.m_bound.left && m_bound.left <= obj.m_bound.right && m_per_bound.bottom > obj.m_per_bound.top && m_bound.bottom <= obj.m_bound.top;
	}

	public bool left_top_hit(mario_obj obj)
	{
		if (m_collider == null || obj.m_collider == null)
		{
			return false;
		}
		bool flag1 = m_bound.top < obj.m_bound.bottom_div && m_bound.top >= obj.m_bound.bottom && m_per_bound.left > obj.m_per_bound.right && m_bound.left <= obj.m_bound.right;
		bool flag2 = m_bound.left > obj.m_bound.right_div && m_bound.left <= obj.m_bound.right && m_per_bound.top < obj.m_per_bound.bottom && m_bound.top >= obj.m_bound.bottom;
		bool flag3 = m_per_bound.left > obj.m_per_bound.right && m_bound.left <= obj.m_bound.right && m_per_bound.top < obj.m_per_bound.bottom && m_bound.top >= obj.m_bound.bottom;

		return flag1 || flag2 || flag3;
	}

	public bool right_top_hit(mario_obj obj)
	{
		if (m_collider == null || obj.m_collider == null)
		{
			return false;
		}
		bool flag1 = m_bound.top < obj.m_bound.bottom_div && m_bound.top >= obj.m_bound.bottom && m_per_bound.right < obj.m_per_bound.left && m_bound.right >= obj.m_bound.left;
		bool flag2 = m_bound.right < obj.m_bound.left_div && m_bound.right >= obj.m_bound.left && m_per_bound.top < obj.m_per_bound.bottom && m_bound.top >= obj.m_bound.bottom;
		bool flag3 = m_per_bound.right < obj.m_per_bound.left && m_bound.right >= obj.m_bound.left && m_per_bound.top < obj.m_per_bound.bottom && m_bound.top >= obj.m_bound.bottom;

		return flag1 || flag2 || flag3;
	}

	public bool left_bottom_hit(mario_obj obj)
	{
		if (m_collider == null || obj.m_collider == null)
		{
			return false;
		}
		bool flag1 = m_bound.bottom > obj.m_bound.top_div && m_bound.bottom <= obj.m_bound.top && m_per_bound.left > obj.m_per_bound.right && m_bound.left <= obj.m_bound.right;
		bool flag2 = m_bound.left > obj.m_bound.right_div && m_bound.left <= obj.m_bound.right && m_per_bound.bottom > obj.m_per_bound.top && m_bound.bottom <= obj.m_bound.top;
		bool flag3 = m_per_bound.left > obj.m_per_bound.right && m_bound.left <= obj.m_bound.right && m_per_bound.bottom > obj.m_per_bound.top && m_bound.bottom <= obj.m_bound.top;
		
		return flag1 || flag2 || flag3;
	}
	
	public bool right_bottom_hit(mario_obj obj)
	{
		if (m_collider == null || obj.m_collider == null)
		{
			return false;
		}
		bool flag1 = m_bound.bottom > obj.m_bound.top_div && m_bound.bottom <= obj.m_bound.top && m_per_bound.right < obj.m_per_bound.left && m_bound.right >= obj.m_bound.left;
		bool flag2 = m_bound.right < obj.m_bound.left_div && m_bound.right >= obj.m_bound.left && m_per_bound.bottom > obj.m_per_bound.top && m_bound.bottom <= obj.m_bound.top;
		bool flag3 = m_per_bound.right < obj.m_per_bound.left && m_bound.right >= obj.m_bound.left && m_per_bound.bottom > obj.m_per_bound.top && m_bound.bottom <= obj.m_bound.top;

		return flag1 || flag2 || flag3;
	}

	public bool hit(mario_obj obj)
	{
		if (m_collider == null || obj.m_collider == null)
		{
			return false;
		}
		return m_bound.right >= obj.m_bound.left && m_bound.left <= obj.m_bound.right && m_bound.top >= obj.m_bound.bottom && m_bound.bottom <= obj.m_bound.top;
	}
	
	public int get_left_hit_pos(mario_obj obj)
	{
		if (m_collider == null || obj.m_collider == null)
		{
			return 0;
		}
		return obj.m_pos.x + obj.m_collider.m_rect.x + obj.m_collider.m_rect.w / 2 + m_collider.m_rect.w / 2 - m_collider.m_rect.x;
	}
	
	public int get_right_hit_pos(mario_obj obj)
	{
		if (m_collider == null || obj.m_collider == null)
		{
			return 0;
		}
		return obj.m_pos.x + obj.m_collider.m_rect.x - obj.m_collider.m_rect.w / 2 - m_collider.m_rect.w / 2 - m_collider.m_rect.x;
	}
	
	public int get_top_hit_pos(mario_obj obj)
	{
		if (m_collider == null || obj.m_collider == null)
		{
			return 0;
		}
		return obj.m_pos.y + obj.m_collider.m_rect.y - obj.m_collider.m_rect.h / 2 - m_collider.m_rect.h / 2 - m_collider.m_rect.y;
	}
	
	public int get_bottom_hit_pos(mario_obj obj)
	{
		if (m_collider == null || obj.m_collider == null)
		{
			return 0;
		}
		return obj.m_pos.y + obj.m_collider.m_rect.y + obj.m_collider.m_rect.h / 2 + m_collider.m_rect.h / 2 - m_collider.m_rect.y;
	}

	protected string get_anim_name()
	{
		if (m_mario_anim != null)
		{
			return m_mario_anim.get_name();
		}
		return "";
	}

	public void play_anim(string name, int speed = -1)
	{
		if (m_mario_anim != null)
		{
			if (m_mario_anim.has_anim(name))
			{
				if (m_animator != null && m_animator.enabled)
				{
					m_animator.enabled = false;
					if (m_shadow_obj != null)
					{
						m_shadow_obj.m_animator.enabled = false;
					}
				}
				if (!m_mario_anim.enabled)
				{
					m_mario_anim.enabled = true;
					if (m_shadow_obj != null)
					{
						m_shadow_obj.m_mario_anim.enabled = true;
					}
				}
				m_mario_anim.play(name, speed);
				if (m_shadow_obj != null)
				{
					m_shadow_obj.m_mario_anim.play(name, speed);
				}
				return;
			}
		}
		if (m_animator != null)
		{
			if (m_mario_anim != null && m_mario_anim.enabled)
			{
				m_mario_anim.enabled = false;
				if (m_shadow_obj != null)
				{
					m_shadow_obj.m_mario_anim.enabled = false;
				}
			}
			if (!m_animator.enabled)
			{
				m_animator.enabled = true;
				if (m_shadow_obj != null)
				{
					m_shadow_obj.m_animator.enabled = true;
				}
			}
			if (!m_animator.GetCurrentAnimatorStateInfo(0).IsName(name))
			{
				m_animator.Play(name);
				if (m_shadow_obj != null)
				{
					m_shadow_obj.m_animator.Play(name);
				}
			}
		}
	}

	protected void stop_anim()
	{
		if (m_mario_anim != null && m_mario_anim.enabled)
		{
			m_mario_anim.enabled = false;
			if (m_shadow_obj != null)
			{
				m_shadow_obj.m_mario_anim.enabled = false;
			}
		}
		if (m_animator != null && m_animator.enabled)
		{
			m_animator.enabled = false;
			if (m_shadow_obj != null)
			{
				m_shadow_obj.m_animator.enabled = false;
			}
		}
	}

	protected void pause_anim()
	{
		if (m_mario_anim != null)
		{
			m_mario_anim.pause();
			if (m_shadow_obj != null)
			{
				m_shadow_obj.m_mario_anim.pause();
			}
		}
		if (m_animator != null)
		{
			m_animator.speed = 0;
			if (m_shadow_obj != null)
			{
				m_shadow_obj.m_animator.speed = 0;
			}
		}
	}

	protected void continue_anim()
	{
		if (m_mario_anim != null)
		{
			m_mario_anim.conti();
			if (m_shadow_obj != null)
			{
				m_shadow_obj.m_mario_anim.conti();
			}
		}
		if (m_animator != null)
		{
			m_animator.speed = 1;
			if (m_shadow_obj != null)
			{
				m_shadow_obj.m_animator.speed = 1;
			}
		}
	}

	
	public void check_is_start(mario_point grid)
	{
		int x = m_grid.x - grid.x;
		int y = m_grid.y - grid.y;
		if (m_unit == null)
		{
			m_is_start = true;
		}
		if (m_main)
		{
			m_is_start = true;
		}
		else if (x >= -utils.g_start_x && x <= utils.g_start_x && y >= -utils.g_start_y && y <= utils.g_start_y)
		{
			m_is_start = true;
		}
		else if (x < -utils.g_del_x || x > utils.g_del_x || y < -utils.g_del_y || y > utils.g_del_y)
		{
			m_is_start = false;
		}
	}

	public void check_state(mario_point grid)
	{
		m_is_new = false;
		int x = m_grid.x - grid.x;
		int y = m_grid.y - grid.y;
		if (!Application.isEditor && !m_main)
		{
			if (x >= -utils.g_active_x && x <= utils.g_active_x && y >= -utils.g_active_y && y <= utils.g_active_y)
			{
				if (!this.gameObject.activeSelf)
				{
					this.gameObject.SetActive(true);
					if (m_shadow != null)
					{
						m_shadow.gameObject.SetActive(true);
					}
				}
			}
			else
			{
				if (this.gameObject.activeSelf)
				{
					this.gameObject.SetActive(false);
					if (m_shadow != null)
					{
						m_shadow.gameObject.SetActive(false);
					}
				}
			}
		}
		check_is_start (grid);
		if (m_unit != null && m_unit.id == utils.g_ryqiu)
		{
			if (x < -utils.g_del_x || x > utils.g_del_x)
			{
				m_is_destory = 2;
			}
		}
		else if (!m_main)
		{
			if (x >= -utils.g_start_x && x <= utils.g_start_x && y >= -utils.g_start_y && y <= utils.g_start_y)
			{

			}
			else if (!play_mode._instance.need_calc(m_grid.x, m_grid.y))
			{
				m_is_destory = 2;
			}
		}
		if (m_main)
		{
			if (m_pos.y < -100)
			{
				m_is_die = true;
			}
		}
		else if (m_unit != null && m_unit.id == utils.g_ryqiu)
		{

		}
		else if (m_pos.y < -utils.g_grid_size)
		{
			m_is_destory = 1;
		}
	}

	public virtual void tupdate()
	{
	}

	public void update_ex(mario_point grid)
	{
		if (!m_edit_mode)
		{
			check_state(grid);
			tupdate ();
		}
		if (m_edit_mode && m_has_edit)
		{
			play_anim("edit");
		}
		if (m_per_pos.x != m_pos.x || m_per_pos.y != m_pos.y)
		{
			set_pos();
		}
		if (m_no_mc_time > 0)
		{
			m_no_mc_time--;
		}
		record_per();
		m_nl_objs.Clear ();
		m_bnl_objs.Clear ();
		m_is_calc_nl = false;
	}

	private void set_pos()
	{
		this.transform.localPosition = new Vector3(m_pos.x / 10.0f, m_pos.y / 10.0f, 0);
		if (m_shadow != null)
		{
			m_shadow.transform.localPosition = new Vector3(m_pos.x / 10.0f, m_pos.y / 10.0f, 0);
		}
	}

	protected void set_angles(float x, float y, float z)
	{
		this.transform.localEulerAngles = new Vector3 (x, y, z);
		if (m_shadow != null)
		{
			m_shadow.transform.localEulerAngles = new Vector3 (x, y, z);
		}
	}

	protected void set_scale(float x, float y, float z)
	{
		this.transform.localScale = new Vector3 (x, y, z);
		if (m_shadow != null)
		{
			m_shadow.transform.localScale = new Vector3 (x, y, z);
		}
	}

	public virtual bool be_left_hit (mario_obj obj, ref int px)
	{
		return false;
	}
	
	public virtual bool be_right_hit (mario_obj obj, ref int px)
	{
		return false;
	}
	
	public virtual bool be_top_hit (mario_obj obj, ref int py)
	{
		return false;
	}
	
	public virtual bool be_bottom_hit (mario_obj obj, ref int py)
	{
		return false;
	}

	public virtual bool be_left_top_hit (mario_obj obj, ref int px, ref int py)
	{
		return false;
	}
	
	public virtual bool be_right_top_hit (mario_obj obj, ref int px, ref int py)
	{
		return false;
	}
	
	public virtual bool be_left_bottom_hit (mario_obj obj, ref int px, ref int py)
	{
		return false;
	}
	
	public virtual bool be_right_bottom_hit (mario_obj obj, ref int px, ref int py)
	{
		return false;
	}
	
	public virtual void be_hit(mario_obj obj)
	{

	}
	
	public virtual mario_point move()
	{
		return new mario_point();
	}
	
	public void do_move()
	{
		m_is_calc_nl = true;
		m_nl_calc_point = move ();
	}

	protected void change_nl_fx(mario_fx fx)
	{
		if (m_type == mario_type.mt_charater)
		{
			for (int i = 0; i < m_nl_objs.Count; ++i)
			{
				if (m_nl_objs[i].m_min_speed != 0)
				{
					m_nl_objs[i].set_fx (fx);
				}
				m_nl_objs[i].change_nl_fx(fx);
			}
		}
	}

	public mario_point get_nl_calc_point()
	{
		if (m_is_calc_nl)
		{
			return m_nl_calc_point;
		}
		if (m_bnl_objs.Count > 0)
		{
			int xx1 = 0;
			int xx2 = 0;
			int yy = -999999;
			for (int i = 0; i < m_bnl_objs.Count; ++i)
			{
				mario_point p = m_bnl_objs[i].get_nl_calc_point();
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
			m_nl_calc_point = new mario_point(m_velocity.x + xx, m_velocity.y + yy);
		}
		else
		{
			m_nl_calc_point = new mario_point(m_velocity.x, m_velocity.y);
		}
		m_is_calc_nl = true;
		return m_nl_calc_point;
	}

	public virtual void change()
	{
		
	}

	public virtual void change1()
	{
		
	}

	public virtual void set_bl(int index, int num)
	{
		m_bl [index] = num;
		// 0 1 顶出来
		// 0 2 向下顶出
		// 0 3 吃到萝卜 1 ? 持续时间
		// 0 4 伤害
		// 0 5 阻挡
	}

	public virtual void change_way(mario_fx fx)
	{
		
	}
}

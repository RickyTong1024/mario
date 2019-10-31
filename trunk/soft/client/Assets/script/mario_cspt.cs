using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class mario_cspt : mario_block1 {

	private bool m_hit = false;
	private mario_point m_next = new mario_point ();
	public GameObject m_fx1;
	public GameObject m_fx2;
	private int m_hit_time = 0;

	public override void init (string name, List<int> param, int world, int x, int y, int xx, int yy)
	{
		base.init (name, param, world, x, y, xx, yy);
		m_is_dzd = 0;
	}
	
	public override void reset()
	{
		if (m_param[0] % 2 == 0)
		{
			set_fx(mario_fx.mf_right);
		}
		else
		{
			set_fx(mario_fx.mf_left);
		}

		if (m_param[0] < 2)
		{
			m_fx1.SetActive(true);
			m_fx2.SetActive(false);
		}
		else
		{
			m_fx1.SetActive(false);
			m_fx2.SetActive(true);
		}

		for (int i = 0; i < 4; ++i)
		{
			int x1 = m_init_pos.x + utils.csg_points[i * 2, 0] / 2;
			int y1 = m_init_pos.y + utils.csg_points[i * 2, 1] / 2;
			int x2 = m_init_pos.x + utils.csg_points[i * 2 + 1, 0] / 2;
			int y2 = m_init_pos.y + utils.csg_points[i * 2 + 1, 1] / 2;
			if (x1 < 0 || x1 >= game_data._instance.m_map_data.maps[m_world].x_num || y1 < 0 || y1 >= game_data._instance.m_map_data.maps[m_world].y_num)
			{
				continue;
			}
			if (x2 < 0 || x2 >= game_data._instance.m_map_data.maps[m_world].x_num || y2 < 0 || y2 >= game_data._instance.m_map_data.maps[m_world].y_num)
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
			if (game_data._instance.m_arrays[m_world][y1][x1].param[2] == x2 && game_data._instance.m_arrays[m_world][y1][x1].param[3] == y2)
			{
				if (m_param[0] % 2 == 0)
				{
					m_next = new mario_point(x2, y2);
				}
				else
				{
					m_next = new mario_point(x1, y1);
				}
				break;
			}
			if (game_data._instance.m_arrays[m_world][y2][x2].param[2] == x1 && game_data._instance.m_arrays[m_world][y2][x2].param[3] == y1)
			{
				if (m_param[0] % 2 == 1)
				{
					m_next = new mario_point(x2, y2);
				}
				else
				{
					m_next = new mario_point(x1, y1);
				}
				break;
			}
		}
	}
	
	public override bool be_left_hit (mario_obj obj, ref int px)
	{
		return false;
	}
	
	public override bool be_right_hit (mario_obj obj, ref int px)
	{
		return false;
	}
	
	public override bool be_top_hit (mario_obj obj, ref int py)
	{
		return false;
	}
	
	public override bool be_bottom_hit (mario_obj obj, ref int py)
	{
		if (obj.m_type != mario_type.mt_charater)
		{
			return false;
		}
		m_hit = true;
		return base.be_bottom_hit (obj, ref py);
	}
	
	public override bool be_left_top_hit (mario_obj obj, ref int px, ref int py)
	{
		return false;
	}
	
	public override bool be_right_top_hit (mario_obj obj, ref int px, ref int py)
	{
		return false;
	}
	
	public override bool be_left_bottom_hit (mario_obj obj, ref int px, ref int py)
	{
		if (obj.m_type != mario_type.mt_charater)
		{
			return false;
		}
		m_hit = true;
		return base.be_left_bottom_hit (obj, ref px, ref py);
	}
	
	public override bool be_right_bottom_hit (mario_obj obj, ref int px, ref int py)
	{
		if (obj.m_type != mario_type.mt_charater)
		{
			return false;
		}
		m_hit = true;
		return base.be_right_bottom_hit (obj, ref px, ref py);
	}
	
	public override mario_point move()
	{
		mario_point p = new mario_point ();
		if (m_hit)
		{
			m_hit_time++;
			if (m_hit_time < 5)
			{
				return p;
			}
			int px = utils.g_grid_size * m_next.x + utils.g_grid_size / 2;
			int py = utils.g_grid_size * m_next.y + utils.g_grid_size / 2;
			if (px > m_pos.x)
			{
				m_pos.x += 40 * (m_param[0] / 2 + 1);
				p.x += 40 * (m_param[0] / 2 + 1);
			}
			else if (px < m_pos.x)
			{
				m_pos.x -= 40 * (m_param[0] / 2 + 1);
				p.x -= 40 * (m_param[0] / 2 + 1);
			}
			if (py > m_pos.y)
			{
				m_pos.y += 40 * (m_param[0] / 2 + 1);
				p.y += 40 * (m_param[0] / 2 + 1);
			}
			else if (py < m_pos.y)
			{
				m_pos.y -= 40 * (m_param[0] / 2 + 1);
				p.y -= 40 * (m_param[0] / 2 + 1);
			}
			if (px == m_pos.x && py == m_pos.y)
			{
				if (m_fx == mario_fx.mf_left)
				{
					int nx = game_data._instance.m_arrays[m_world][m_next.y][m_next.x].param[0];
					int ny = game_data._instance.m_arrays[m_world][m_next.y][m_next.x].param[1];
					if (nx == 0 && ny == 0)
					{
						m_fx = mario_fx.mf_right;
						nx = game_data._instance.m_arrays[m_world][m_next.y][m_next.x].param[2];
						ny = game_data._instance.m_arrays[m_world][m_next.y][m_next.x].param[3];
					}
					m_next = new mario_point(nx, ny);
				}
				else
				{
					int nx = game_data._instance.m_arrays[m_world][m_next.y][m_next.x].param[2];
					int ny = game_data._instance.m_arrays[m_world][m_next.y][m_next.x].param[3];
					if (nx == 0 && ny == 0)
					{
						m_fx = mario_fx.mf_left;
						nx = game_data._instance.m_arrays[m_world][m_next.y][m_next.x].param[0];
						ny = game_data._instance.m_arrays[m_world][m_next.y][m_next.x].param[1];
					}
					m_next = new mario_point(nx, ny);
				}
			}
		}
		return p;
	}

	public override void change()
	{
		if (m_param[0] < 3)
		{
			m_param[0] = m_param[0] + 1;
		}
		else
		{
			m_param[0] = 0;
		}
		if (m_unit != null)
		{
			game_data._instance.m_arrays[m_world][m_init_pos.y][m_init_pos.x].param[0] = m_param[0];
		}
		reset ();
	}
}

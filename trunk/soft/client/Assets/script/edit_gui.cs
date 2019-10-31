using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class edit_gui : MonoBehaviour, IMessage {

	private bool m_start = false;
	public GameObject m_tools;
	public GameObject m_switch;
	public GameObject m_fg;
	public GameObject m_save;
	public GameObject m_close;
	public GameObject m_time_tip;
	public GameObject m_ts;
	public GameObject m_ts1;
	public GameObject m_ts2;
	public GameObject m_ts3;
	public GameObject m_select;
	public GameObject m_select1;
	public GameObject m_end;
	public GameObject m_tip;
	public GameObject m_zuobiao;
	public GameObject m_left;
	public GameObject m_select2;
	private int m_index = 1;
	private int m_tuo_type = 0;
	private Vector2 m_tuo_pos = new Vector2();
	private int m_row = 0;
	public GameObject m_row_num;
	public List<GameObject> m_selects;
	public List<GameObject> m_row_icons;
	public GameObject m_tool_xuan;
	public List<GameObject> m_mode_selects;
	public GameObject m_mode;
	public GameObject m_select_time;
	public GameObject m_time;
	public GameObject m_time_xuan;
	public GameObject m_fg_xuan;
	public mario_point m_change_point = new mario_point();
	private bool m_return = false;
	private bool m_need_save = false;
	private bool m_flag = false;
	public GameObject m_fg_view;
	public GameObject m_fg_sub;
	public List<GameObject> m_fg_subs;
	public GameObject m_joy_panel;
	public GameObject m_sm_panel;
	public GameObject m_sound;
	public GameObject m_tool_sub;
	public GameObject m_tool_view;
	public int m_world = 0;
	public GameObject m_ending;
	public Joystick m_joy;
	private bool m_time_up = false;
	private int m_time_up_time = 0;
	private bool m_time_down = false;
	private int m_time_down_time = 0;

	void Start()
	{
		cmessage_center._instance.add_handle (this);

		if (mario._instance.m_self.guide < 100)
		{
			mario._instance.m_self.guide = 0;
			jx1();
			s_message mes = new s_message();
			mes.m_type = "play_mode";
			mes.m_object.Add (null);
			cmessage_center._instance.add_message(mes);
		}
		else
		{
			s_message mes = new s_message();
			mes.m_type = "edit_mode";
			cmessage_center._instance.add_message(mes);
		}

		m_row = 0;
		reset_icon (true);
		change_time (game_data._instance.m_map_data.time);
		change_mode (game_data._instance.m_map_data.mode);
		reset_sound (game_data._instance.m_map_data.no_music);

#if !UNITY_IPHONE && !UNITY_ANDROID
		m_joy_panel.SetActive(false);
		m_sm_panel.SetActive(true);
#else
		m_joy_panel.SetActive(true);
		m_sm_panel.SetActive(false);
#endif
	}

	void OnDestroy()
	{
		s_message mes = new s_message();
		mes.m_type = "close_edit_mode";
		cmessage_center._instance.add_message(mes);

		cmessage_center._instance.remove_handle (this);
	}

	void reset_icon(bool is_change = false)
	{
		if (is_change)
		{
			change_row (0);
		}
		else
		{
			change_row(m_row);
		}
		int num = (game_data._instance.m_unit_num + 7) / 8;
		mario._instance.remove_child (m_tool_view);
		for (int i = 0; i < num; ++i)
		{
			GameObject obj = (GameObject)Instantiate(m_tool_sub);
			obj.name = (i + 1).ToString();
			obj.transform.parent = m_tool_view.transform;
			obj.transform.localPosition = new Vector3(0, 170 - 80 * i, 0);
			obj.transform.localScale = new Vector3(1, 1, 1);
			obj.SetActive(true);
			obj.transform.FindChild("row").FindChild("Label").GetComponent<UILabel>().text = (i + 1).ToString();
			for (int j = 1; j <= 8; ++j)
			{
				int index = i * 8 + j;
				s_t_unit t_unit = game_data._instance.get_t_unit_by_site(index - 1);
				if (t_unit == null)
				{
					obj.transform.FindChild(j.ToString()).FindChild("icon").GetComponent<UISprite>().spriteName = "";
				}
				else if (t_unit.kfg == 1)
				{
					obj.transform.FindChild(j.ToString()).FindChild("icon").GetComponent<UISprite>().spriteName = t_unit.icon + "_" + game_data._instance.m_map_data.maps[m_world].map_theme.ToString();
				}
				else
				{
					obj.transform.FindChild(j.ToString()).FindChild("icon").GetComponent<UISprite>().spriteName = t_unit.icon;
				}
			}
		}
		if (m_world != game_data._instance.m_map_data.end_area)
		{
			m_ending.SetActive(true);
		}
		else
		{
			m_ending.SetActive(false);
		}
	}

	void jx1()
	{
		m_switch.SetActive(false);
		m_fg.SetActive (false);
		m_save.SetActive (false);
		m_close.SetActive (false);
		m_time_tip.SetActive (false);
		m_left.SetActive (false);
	}

	void show_fg()
	{
		m_fg_xuan.SetActive(true);
		m_fg_subs.Clear ();
		mario._instance.remove_child (m_fg_view);
		for (int i = 0; i < game_data._instance.m_fg_num; ++i)
		{
			int id = i + 1;
			GameObject obj = (GameObject)Instantiate(m_fg_sub);
			obj.transform.parent = m_fg_view.transform;
			obj.transform.localPosition = new Vector3(-94 + 380 * i, 0, 0);
			obj.transform.localScale = new Vector3(1,1,1);
			obj.GetComponent<fg_sub>().reset (id);
			if (id == game_data._instance.m_map_data.maps[m_world].map_theme)
			{
				obj.GetComponent<fg_sub>().set_gou(true);
			}
			obj.SetActive(true);
			m_fg_subs.Add(obj);
		}
		int x = game_data._instance.m_map_data.maps[m_world].map_theme * 380 - 476;
		if (x < 0)
		{
			x = 0;
		}
		else if (game_data._instance.m_fg_num == game_data._instance.m_map_data.maps[m_world].map_theme)
		{
			x = game_data._instance.m_map_data.maps[m_world].map_theme * 380 - 564;
		}
		m_fg_view.transform.localPosition = new Vector3 (-x, 0, 0);
		m_fg_view.GetComponent<UIPanel>().clipOffset = new Vector2(x, 0);
	}

	void select_area(GameObject obj)
	{
		int index = int.Parse(obj.name.Substring (5, 1));
		s_message mes = new s_message ();
		mes.m_type = "edit_qiehuan_area";
		mes.m_ints.Add (index - 1);
		mario._instance.show_double_dialog_box (string.Format(game_data._instance.get_language_string("edit_gui_qhqy"), index), mes, null);
	}

	void select_fg(GameObject obj)
	{
		if (obj.GetComponent<fg_sub>().m_lk)
		{
			if (!Application.isEditor)
			{
				return;
			}
		}
		int id = obj.GetComponent<fg_sub>().m_id;
		if (id == game_data._instance.m_map_data.maps[m_world].map_theme)
		{
			m_fg_xuan.SetActive(false);
			return;
		}
		game_data._instance.m_map_data.maps[m_world].map_theme = id;
		for (int i = 0; i < m_fg_subs.Count; ++i)
		{
			if (i + 1 != id)
			{
				m_fg_subs[i].GetComponent<fg_sub>().set_gou(false);
			}
			else
			{
				m_fg_subs[i].GetComponent<fg_sub>().set_gou(true);
			}
		}
		reset_icon ();
		StartCoroutine(edit_mode._instance.reload_fg (m_world));
		m_need_save = true;
		m_fg_xuan.SetActive(false);
		m_flag = true;
	}

	public void click(GameObject obj)
	{
		if (obj.name == "switch" && mario._instance.m_self.guide != 4)
		{
			m_ts2.SetActive(false);
			if (!m_start)
			{
				edit_mode._instance.check_mission();
				s_message mes = new s_message();
				mes.m_type = "play_mode";
				if (mario._instance.m_self.guide == 2)
				{
					mario._instance.m_self.guide = 3;
				}
				if (mario._instance.m_self.guide < 100)
				{
					mes.m_object.Add(null);
				}
				else
				{
					mes.m_object.Add(edit_mode._instance.m_main_char.m_pos);
					mes.m_ints.Add(m_world);
				}
				cmessage_center._instance.add_message(mes);
			}
			else
			{
				play_mode._instance.m_die_pos = play_mode._instance.m_main_char.m_pos;
				play_mode._instance.lose();
			}
		}
		else if (mario._instance.m_self.guide == 2)
		{
			mario._instance.show_tip(game_data._instance.get_language_string("edit_gui_djsw"));
			return;
		}
		if (obj.name == "save")
		{
			m_ts3.SetActive(false);
			if (mario._instance.m_self.guide == 4)
			{
				edit_mode._instance.check_mission();
				game_data._instance.save_mission_ex();
			}
			else if (m_need_save)
			{
				edit_mode._instance.check_mission();
				game_data._instance.save_mission();
				m_need_save = false;
			}
		}
		else if (mario._instance.m_self.guide == 4)
		{
			mario._instance.show_tip(game_data._instance.get_language_string("edit_gui_djbc"));
			return;
		}
		if (obj.name == "fg")
		{
			show_fg();
		}
		if (obj.name == "close_fg")
		{
			m_fg_xuan.SetActive(false);
			m_flag = true;
		}
		if (obj.name == "erase")
		{
			m_index = 0;
			m_select1.transform.localPosition = obj.transform.localPosition;
			m_select.SetActive(false);
			m_select1.SetActive(true);
		}
		if (obj.name == "change")
		{
			m_index = -1;
			m_select1.transform.localPosition = obj.transform.localPosition;
			m_select.SetActive(false);
			m_select1.SetActive(true);
		}
		if (obj.name == "down")
		{
			m_ts.SetActive(false);
			m_tool_xuan.SetActive(true);
		}
		if (obj.name == "close_xuan")
		{
			m_tool_xuan.SetActive(false);
			m_flag = true;
		}
		if (obj.name == "time_tip")
		{
			m_time_xuan.SetActive(true);
		}
		if (obj.name == "close_time")
		{
			m_time_xuan.SetActive(false);
			m_flag = true;
		}
		if (obj.name == "return")
		{
			s_message mes = new s_message();
			mes.m_type = "edit_return_ok";
			mario._instance.show_double_dialog_box(game_data._instance.get_language_string("edit_gui_sfqd"), mes);
		}
		if (obj.name == "sound")
		{
			reset_sound(1 - game_data._instance.m_map_data.no_music);
			m_need_save = true;
		}
		if (obj.name == "ending")
		{
			game_data._instance.m_map_data.end_area = m_world;
			edit_mode._instance.reset_ending();
			m_ending.SetActive(false);
			mario._instance.show_tip (game_data._instance.get_language_string("edit_gui_swzd"));
			m_need_save = true;
		}
	}

	void press(GameObject obj)
	{
		if (obj.name == "up_time")
		{
			m_time_up = true;
			m_time_up_time = 0;
			change_time(game_data._instance.m_map_data.time + 10);
			m_need_save = true;
		}
		if (obj.name == "down_time")
		{
			m_time_down = true;
			m_time_down_time = 0;
			change_time(game_data._instance.m_map_data.time - 10);
			m_need_save = true;
		}
	}

	void release(GameObject obj)
	{
		if (obj.name == "up_time")
		{
			m_time_up = false;
			m_time_up_time = 0;
		}
		if (obj.name == "down_time")
		{
			m_time_down = false;
			m_time_down_time = 0;
		}
	}

	public void line_press(GameObject obj)
	{
		Vector2 v = mario._instance.get_mouse_position();
		int x = (int)((v.x - Screen.width * utils.g_height / 10 / Screen.height / 2 + 200) / 2f);
		if (x < 0)
		{
			x = 0;
		}
		else if (x >= game_data._instance.m_map_data.maps[m_world].x_num)
		{
			x = game_data._instance.m_map_data.maps[m_world].x_num - 1;
		}
		edit_mode._instance.reload_jump (new mario_point(x * utils.g_grid_size + utils.g_grid_size / 2, edit_mode._instance.m_main_char.m_pos.y), m_world);
	}

	public void select(GameObject obj)
	{
		int index1 = int.Parse (obj.name);
		int index = m_row * 8 + index1;
		s_t_unit t_unit = game_data._instance.get_t_unit_by_site(index - 1);
		if (t_unit == null)
		{
			return;
		}
		m_index = index1;
		m_select.transform.localPosition = obj.transform.localPosition;
		m_select.SetActive(true);
		m_select1.SetActive(false);
	}

	public void select_xuan(GameObject obj)
	{
		int index1 = int.Parse (obj.name);
		int index2 = int.Parse (obj.transform.parent.name);
		int index = (index2 - 1) * 8 + index1;
		s_t_unit t_unit = game_data._instance.get_t_unit_by_site(index - 1);
		if (t_unit == null)
		{
			return;
		}
		m_index = index1;
		m_select.transform.localPosition = m_selects[m_index - 1].transform.localPosition;
		m_select.SetActive(true);
		m_select1.SetActive(false);

		m_tool_xuan.SetActive(false);
		change_row (index2 - 1);
		m_flag = true;
	}

	public void change_row(int row)
	{
		if ((row - 1) * 8 >= game_data._instance.m_unit_num)
		{
			return;
		}
		m_row = row;
		m_row_num.GetComponent<UILabel>().text = (row + 1).ToString();
		for (int i = 0; i < 8; ++i)
		{
			s_t_unit t_unit = game_data._instance.get_t_unit_by_site(m_row * 8 + i);
			if (t_unit == null)
			{
				m_row_icons[i].GetComponent<UISprite>().spriteName = "";
			}
			else if (t_unit.kfg == 1)
			{
				m_row_icons[i].GetComponent<UISprite>().spriteName = t_unit.icon + "_" + game_data._instance.m_map_data.maps[m_world].map_theme.ToString();
			}
			else
			{
				m_row_icons[i].GetComponent<UISprite>().spriteName = t_unit.icon;
			}
		}
	}

	void select_mode(GameObject obj)
	{
		int index = int.Parse (obj.name) - 1;
		change_mode (index);
		m_need_save = true;
	}

	void change_mode(int mode)
	{
		for (int i = 0; i < 3; ++i)
		{
			m_mode_selects[i].SetActive(false);
		}
		m_mode_selects[mode].SetActive (true);
		if (mode == 0)
		{
			m_mode.GetComponent<UISprite>().spriteName = "mode01";
		}
		else if (mode == 1)
		{
			m_mode.GetComponent<UISprite>().spriteName = "mode02";
		}
		else
		{
			m_mode.GetComponent<UISprite>().spriteName = "mode03";
		}
		game_data._instance.m_map_data.mode = mode;
		if (edit_mode._instance != null)
		{
			edit_mode._instance.reset_zhuiji();
		}
	}

	void reset_sound(int no_sound)
	{
		game_data._instance.m_map_data.no_music = no_sound;
		if (no_sound == 0)
		{
			m_sound.GetComponent<UISprite>().spriteName = "xlb_001";
		}
		else
		{
			m_sound.GetComponent<UISprite>().spriteName = "xlb_002";
		}
	}

	void change_time(int time)
	{
		if (time < 10 || time > 990)
		{
			return;
		}
		m_time.GetComponent<UILabel> ().text = time.ToString ();
		m_select_time.GetComponent<UILabel> ().text = time.ToString ();
		game_data._instance.m_map_data.time = time;
	}

	public void message (s_message message)
	{
		if (message.m_type == "play_mode")
		{
			m_start = true;
			m_switch.GetComponent<UISprite>().spriteName = "edit-1";
			m_tools.SetActive(false);
			mario._instance.show_play_gui();
			mario._instance.play_mus(game_data._instance.get_map_music(m_world), true, 1 - game_data._instance.m_map_data.no_music);
		}
		if (message.m_type == "edit_mode")
		{
			m_start = false;
			m_switch.GetComponent<UISprite>().spriteName = "play-1";
			m_tools.SetActive(true);
			mario._instance.hide_play_gui();
			mario._instance.play_mus("music/select", true, 0.5f);

			if (message.m_ints.Count > 0)
			{
				m_world = (int)message.m_ints[0];
				m_select2.transform.localPosition = new Vector3(60, 160 - 70 * m_world);
			}

			if (mario._instance.m_self.guide == 4)
			{
				s_message mes = new s_message();
				mes.m_type = "jx_4";
				mario._instance.show_xsjx_dialog_box(game_data._instance.get_language_string("edit_gui_hlbc"), mes);
			}
		}
		if (message.m_type == "edit_return_ok")
		{
			if (m_need_save)
			{
				s_message mes = new s_message();
				mes.m_type = "edit_return_save_ok";
				s_message mes1 = new s_message();
				mes1.m_type = "edit_return_save_cancel";
				mario._instance.show_double_dialog_box(game_data._instance.get_language_string("edit_gui_sfbc"), mes, mes1);
			}
			else
			{
				mario._instance.change_state(e_game_state.egs_edit_select, 1, delegate() { Object.Destroy(this.gameObject); });
			}
			m_return = true;
		}
		if (message.m_type == "edit_return_save_ok")
		{
			edit_mode._instance.check_mission();
			game_data._instance.save_mission();
			m_need_save = false;
		}
		if (message.m_type == "edit_return_save_cancel")
		{
			mario._instance.change_state(e_game_state.egs_edit_select, 1, delegate() { Object.Destroy(this.gameObject); });
		}

		if (message.m_type == "jx_1")
		{
			play_mode._instance.lose();
			s_message mes = new s_message();
			mes.m_type = "jx_2";
			mario._instance.show_xsjx_dialog_box(game_data._instance.get_language_string("edit_gui_xyts"), mes);
		}
		if (message.m_type == "jx_2")
		{
			mario._instance.m_self.guide = 1;
			edit_mode._instance.create_xy ();
			m_ts.SetActive (true);
			m_ts1.SetActive(true);
		}
		if (message.m_type == "jx_3")
		{
			m_switch.SetActive(true);
			m_ts2.SetActive(true);
		}
		if (message.m_type == "jx_4")
		{
			m_save.SetActive(true);
			m_ts3.SetActive(true);
		}
		if (message.m_type == "jx_5")
		{
			mario._instance.change_state(e_game_state.egs_edit_select, 1, delegate() { Object.Destroy(this.gameObject); });
		}
		if (message.m_type == "edit_qiehuan_area")
		{
			m_world = (int)message.m_ints[0];
			m_select2.transform.localPosition = new Vector3(60, 160 - 70 * m_world);
			reset_icon ();
			edit_mode._instance.reload_self (m_world);
		}
	}
	
	public void net_message (s_net_message message)
	{
		if (message.m_opcode == opclient_t.OPCODE_SAVE_MAP)
		{
			if (m_return)
			{
				mario._instance.change_state(e_game_state.egs_edit_select, 1, delegate() { Object.Destroy(this.gameObject); });
			}
		}
		if (message.m_opcode == opclient_t.OPCODE_COMPLETE_GUIDE)
		{
			mario._instance.m_self.guide = 200;
			s_message mes = new s_message();
			mes.m_type = "jx_5";
			mario._instance.show_xsjx_dialog_box(game_data._instance.get_language_string("edit_gui_wcjx"), mes);
		}
	}

	void FixedUpdate()
	{
		if (mario._instance.m_self.guide == 1)
		{
			int num = utils.jx_block.Length / 3;
			for (int i = 0; i < num; ++i)
			{
				int x = utils.jx_block[i * 3];
				int y = utils.jx_block[i * 3 + 1];
				int id = utils.jx_block[i * 3 + 2];
				if (x >= game_data._instance.m_map_data.maps[m_world].x_num)
				{
					return;
				}
				if (game_data._instance.m_arrays[m_world][y][x].type != id)
				{
					edit_mode._instance.set_dlg(x, y);
					return;
				}
			}
			mario._instance.m_self.guide = 2;
			s_message mes = new s_message();
			mes.m_type = "jx_3";
			mario._instance.show_xsjx_dialog_box(game_data._instance.get_language_string("edit_gui_swdt"), mes);
		}
		if (m_time_up)
		{
			m_time_up_time++;
			if (m_time_up_time >= 30 && m_time_up_time % 5 == 0)
			{
				change_time(game_data._instance.m_map_data.time + 10);
				m_need_save = true;
			}
		}
		if (m_time_down)
		{
			m_time_down_time++;
			if (m_time_down_time >= 30 && m_time_down_time % 5 == 0)
			{
				change_time(game_data._instance.m_map_data.time - 10);
				m_need_save = true;
			}
		}
	}

	void Update()
	{
		if (m_start)
		{
			return;
		}
		if (edit_mode._instance == null || edit_mode._instance.m_main_char == null)
		{
			return;
		}
		if (m_joy.IsHolding)
		{
			m_ts1.SetActive(false);
			return;
		}
#if !UNITY_IPHONE && !UNITY_ANDROID
		if (mario._instance.key_down(KeyCode.LeftArrow) || mario._instance.key_down(KeyCode.RightArrow) || mario._instance.key_down(KeyCode.Z))
		{
			m_sm_panel.SetActive(false);
		}
#endif
		if (mario._instance.get_mouse_button())
		{
			if (mario._instance.m_self.guide == 2)
			{
				mario._instance.show_tip(game_data._instance.get_language_string("edit_gui_djsw"));
				return;
			}
			if (mario._instance.m_self.guide == 4)
			{
				mario._instance.show_tip(game_data._instance.get_language_string("edit_gui_djbc"));
				return;
			}
			Vector2 v = mario._instance.get_mouse_position();
			bool flag = true;
			if (v.x <= 96 || v.x >= Screen.width * utils.g_height / Screen.height - 96 || v.y <= 48 || v.y >= 560)
			{
				flag = false;
			}
			RaycastHit m_rayhit;
			Ray _ui_ray = mario._instance.m_ui_camera.ScreenPointToRay(Input.mousePosition);
			if (Physics.Raycast(_ui_ray, out m_rayhit, 20.0f))
			{
				m_flag = true;
			}
			if (m_flag)
			{
				flag = false;
			}
			float x = v.x - Screen.width * utils.g_height / 10 / Screen.height / 2;
			float y = v.y - 64;
			int mxx = (int)(x / 0.8 * 10 + edit_mode._instance.m_roll.x);
			int myy = (int)(y / 0.8 * 10 + edit_mode._instance.m_roll.y);
			int xx = mxx / utils.g_grid_size;
			int yy = myy / utils.g_grid_size;
			if (xx == 1 && yy == game_data._instance.m_map_data.maps[m_world].qd_y && m_tuo_type == 0)
			{
				m_tuo_type = 1;
				m_tuo_pos = v;
			}
			else if (xx == game_data._instance.m_map_data.maps[m_world].x_num - 2 && yy == game_data._instance.m_map_data.maps[m_world].zd_y && m_tuo_type == 0)
			{
				m_tuo_type = 2;
				m_tuo_pos = v;
			}
			else if (m_tuo_type == 0 && flag && !(m_change_point.x == xx && m_change_point.y == yy))
			{
				m_change_point = new mario_point(xx, yy);
				int index = m_index;
				if (m_index > 0)
				{
					index = m_row * 8 + m_index;
					s_t_unit t_unit = game_data._instance.get_t_unit_by_site(index - 1);
					index = t_unit.id;
				}
				if (index != 0 && mario._instance.m_self.guide == 1)
				{
					bool fflag = false;
					int dnum = utils.jx_block.Length / 3;
					for (int i = 0; i < dnum; ++i)
					{
						int dx = utils.jx_block[i * 3];
						int dy = utils.jx_block[i * 3 + 1];
						int did = utils.jx_block[i * 3 + 2];
						if (dy == yy && dx == xx)
						{
							fflag = true;
							if (index != did)
							{
								mario._instance.show_tip(game_data._instance.get_language_string("edit_gui_zlbd"));
								return;
							}
						}
					}
					if (!fflag)
					{
						mario._instance.show_tip(game_data._instance.get_language_string("edit_gui_qaxy"));
						return;
					}
				}
				edit_mode._instance.add_obj(xx, yy, index);
				m_need_save = true;
			}

			if (m_tuo_type > 0)
			{
				int vx = 0;
				if (v.x - m_tuo_pos.x > 50)
				{
					vx = 1;
				}
				else if (v.x - m_tuo_pos.x < -50)
				{
					vx = -1;
				}
				int vy = 0;
				if (v.y - m_tuo_pos.y > 50)
				{
					vy = 1;
				}
				else if (v.y - m_tuo_pos.y < -50)
				{
					vy = -1;
				}
				if (vx != 0 || vy != 0)
				{
					edit_mode._instance.add_qz(m_tuo_type, vx, vy);
					m_need_save = true;
				}
			}
			else
			{
				edit_mode._instance.add_qz(0, 0, 0);
			}
			edit_mode._instance.set_mpos(mxx, myy);
		}
		else
		{
			m_change_point = new mario_point();
			m_tuo_type = 0;
			edit_mode._instance.not_add_obj();
			edit_mode._instance.add_qz(0, 0, 0);
			m_flag = false;
		}
		m_end.transform.localPosition = new Vector3 (game_data._instance.m_map_data.maps[m_world].x_num * 2 - 200 + 32, 42, 0);
		m_tip.transform.localPosition = new Vector3 (edit_mode._instance.m_main_char.m_pos.x / utils.g_grid_size * 2 - 200, 42, 0);
		m_zuobiao.GetComponent<UILabel>().text = "x:" + (edit_mode._instance.m_main_char.m_pos.x / utils.g_grid_size).ToString() + " y:" + (edit_mode._instance.m_main_char.m_pos.y/ utils.g_grid_size).ToString();
	}
}

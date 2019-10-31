using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class player_select_gui : MonoBehaviour, IMessage {

	public GameObject m_back;
	public GameObject m_back1;
	public GameObject m_back2;
	public GameObject m_left_view_big;
	public GameObject m_left_view;
	public GameObject m_left_view1;
	public GameObject m_left_panel_big;
	public GameObject m_left_panel;
	public GameObject m_left_panel1;
	public GameObject m_right_view;
	public GameObject m_play_select_big;
	public GameObject m_play_select_sub;
	public GameObject m_play_select_sub1;
	private int m_lan = 0;
	private List<GameObject> m_second = new List<GameObject>();
	public GameObject m_map_level;
	public GameObject m_map_exp;
	public GameObject m_rq;
	public GameObject m_tgl;
	public GameObject m_texture;
	public GameObject m_map_win;
	public GameObject m_map_name;
	public GameObject m_map_id;
	public GameObject m_map_zuo_touxiang;
	public GameObject m_map_zuo_name;
	public GameObject m_map_zuo_guojia;
	public GameObject m_map_dz;
	public GameObject m_map_tg;
	public GameObject m_map_cy;
	public GameObject m_map_sc_text;
	private protocol.game.map_info m_mi;
	private int m_id;
	private List<protocol.game.comment> m_comments;
	public GameObject m_search;
	public GameObject m_search_text;
	public GameObject m_pinglun;
	public GameObject m_pinglun_text;
	private int m_page_up;
	private int m_page_down;
	private int m_max_page;
	private int m_page_type = -1;
	private int m_page_fx;
	public GameObject m_pl_panel;
	public GameObject m_pl_sub;
	public GameObject m_npl;
	public GameObject m_up_tuo;
	public GameObject m_down_tuo;
	private GameObject m_tp_up_tuo;
	private GameObject m_tp_down_tuo;
	public GameObject m_br_start;
	public GameObject m_br_continue;
	public GameObject m_br_jd;
	public GameObject m_br_life;
	public GameObject m_br_index;
	public GameObject m_br_select;
	public GameObject m_br_text;
	public GameObject m_br_bk;
	public List<Texture> m_br_texs;
	public GameObject m_br_select_panel;
	public List<GameObject> m_br_locks;
	private int m_bhard;

	void Start () {
		cmessage_center._instance.add_handle (this);
		mario._instance.show_user (this.gameObject);
		m_left_view1.GetComponent<UIScrollView>().onDragFinished += OnDragFinished;
	}
	
	void OnDestroy()
	{
		cmessage_center._instance.remove_handle (this);
	}

	void OnEnable()
	{
		if (m_lan == 0)
		{
			m_back.SetActive (true);
			m_back1.SetActive (false);
			m_back2.SetActive (false);
			reset_big();
		}
		else if (m_mi != null)
		{
			protocol.game.cmsg_view_comment msg = new protocol.game.cmsg_view_comment();
			msg.id = m_mi.id;
			net_http._instance.send_msg<protocol.game.cmsg_view_comment>(opclient_t.OPCODE_VIEW_COMMENT, msg);
		}
	}
	
	void OnDragFinished()
	{
		if (m_page_type == -1)
		{
			return;
		}
		UIScrollView uv = m_left_view1.GetComponent<UIScrollView> ();
		Vector3 constraint = uv.panel.CalculateConstrainOffset(uv.bounds.min, uv.bounds.max);
		if (constraint.y > 10)
		{
			if (m_page_up <= 0)
			{
				return;
			}
			m_page_up--;
			m_page_fx = -1;
			protocol.game.cmsg_view_map msg = new protocol.game.cmsg_view_map();
			msg.index = m_page_up;
			msg.type = m_page_type;
			msg.ver = game_data.m_self_map_ver;
			net_http._instance.send_msg<protocol.game.cmsg_view_map>(opclient_t.OPCODE_VIEW_MAP, msg);
		}
		else if (constraint.y < -10)
		{
			if (m_page_down >= m_max_page)
			{
				return;
			}
			m_page_down++;
			m_page_fx = 1;
			protocol.game.cmsg_view_map msg = new protocol.game.cmsg_view_map();
			msg.index = m_page_down;
			msg.type = m_page_type;
			msg.ver = game_data.m_self_map_ver;
			net_http._instance.send_msg<protocol.game.cmsg_view_map>(opclient_t.OPCODE_VIEW_MAP, msg);
		}
	}

	void change_big()
	{
		if (m_lan == 0)
		{
			return;
		}
		reset_big ();
		m_lan = 0;
		m_left_panel_big.transform.localPosition = new Vector3 (-300, m_left_panel_big.transform.localPosition.y, m_left_panel_big.transform.localPosition.z);
		utils.add_pos_anim(m_left_panel_big, 0.5f, new Vector3(300, 0, 0), 0);
		m_left_panel.transform.localPosition = new Vector3 (0, m_left_panel.transform.localPosition.y, m_left_panel.transform.localPosition.z);
		utils.add_pos_anim(m_left_panel, 0.5f, new Vector3(300, 0, 0), 0);
	}

	void change_first(bool big)
	{
		if (m_lan == 1)
		{
			return;
		}
		reset_first ();
		m_lan = 1;
		if (big)
		{
			m_left_panel_big.transform.localPosition = new Vector3 (0, m_left_panel_big.transform.localPosition.y, m_left_panel_big.transform.localPosition.z);
			utils.add_pos_anim(m_left_panel_big, 0.5f, new Vector3(-300, 0, 0), 0);
			m_left_panel.transform.localPosition = new Vector3 (300, m_left_panel.transform.localPosition.y, m_left_panel.transform.localPosition.z);
			utils.add_pos_anim(m_left_panel, 0.5f, new Vector3(-300, 0, 0), 0);
		}
		else
		{
			m_left_panel.transform.localPosition = new Vector3 (-300, m_left_panel.transform.localPosition.y, m_left_panel.transform.localPosition.z);
			utils.add_pos_anim(m_left_panel, 0.5f, new Vector3(300, 0, 0), 0);
			m_left_panel1.transform.localPosition = new Vector3 (0, m_left_panel1.transform.localPosition.y, m_left_panel1.transform.localPosition.z);
			utils.add_pos_anim(m_left_panel1, 0.5f, new Vector3(300, 0, 0), 0);
		}
	}

	void change_second()
	{
		if (m_lan == 2)
		{
			return;
		}
		m_lan = 2;
		m_left_panel.transform.localPosition = new Vector3 (0, m_left_panel.transform.localPosition.y, m_left_panel.transform.localPosition.z);
		utils.add_pos_anim(m_left_panel, 0.5f, new Vector3(-300, 0, 0), 0);
		m_left_panel1.transform.localPosition = new Vector3 (300, m_left_panel1.transform.localPosition.y, m_left_panel1.transform.localPosition.z);
		utils.add_pos_anim(m_left_panel1, 0.5f, new Vector3(-300, 0, 0), 0);
	}

	void reset_big()
	{
		mario._instance.remove_child (m_left_view_big);
		m_left_view_big.GetComponent<UIScrollView> ().ResetPosition ();
		int num = 0;
		foreach (KeyValuePair<int, s_t_view_title> kv in game_data._instance.m_t_view_title)	
		{
			s_t_view_title vt = kv.Value;
			bool flag = false;
			if (vt.id == 200)
			{
				if (mario._instance.m_self.testify != 0)
				{
					continue;
				}
			}
			else if (vt.id == 201)
			{
				if (mario._instance.m_self.testify != 1)
				{
					continue;
				}
			}
			else if (vt.id == 202)
			{
				if (mario._instance.m_self.testify != 2)
				{
					continue;
				}
			}
			else if (vt.id == 203)
			{
				if (mario._instance.m_self.testify != 3)
				{
					continue;
				}
			}
			GameObject obj = (GameObject)Instantiate(m_play_select_big);
			obj.name = "big_sub";
			obj.transform.parent = m_left_view_big.transform;
			obj.transform.localPosition = new Vector3(-300,182 - 85 * num,0);
			obj.transform.localScale = new Vector3(1,1,1);
			obj.transform.FindChild("icon").GetComponent<UISprite>().spriteName = vt.icon;
			obj.GetComponent<UIButtonMessage>().target = this.gameObject;
			if (vt.id >= 200)
			{
				obj.GetComponent<play_select_big>().m_clevel = game_data._instance.get_zm(vt.id - 199);
			}
			utils.add_pos_anim(obj, 0.5f, new Vector3(300, 0, 0), num * 0.05f);
			obj.GetComponent<play_select_big>().reset(vt);
			obj.SetActive(true);
			num++;
		}
	}

	void reset_first()
	{
		mario._instance.remove_child (m_left_view);
		m_left_view.GetComponent<UIScrollView> ().ResetPosition ();
		int num = 0;
		foreach (KeyValuePair<int, s_t_view_map> kv in game_data._instance.m_t_view_map)	
		{
			s_t_view_map vm = kv.Value;
			if (vm.id == 4 && mario._instance.m_self.testify < 1)
			{
				continue;
			}
			else if (vm.id == 5 && mario._instance.m_self.testify < 2)
			{
				continue;
			}
			else if (vm.id == 6 && mario._instance.m_self.testify < 3)
			{
				continue;
			}
			else if (vm.id == 7 && mario._instance.m_self.testify < 4)
			{
				continue;
			}
			GameObject obj = (GameObject)Instantiate(m_play_select_sub);
			obj.name = "first_sub";
			obj.transform.parent = m_left_view.transform;
			obj.transform.localPosition = new Vector3(0,182 - 85 * num,0);
			obj.transform.localScale = new Vector3(1,1,1);
			obj.transform.FindChild("icon").GetComponent<UISprite>().spriteName = vm.icon;
			obj.GetComponent<UIButtonMessage>().target = this.gameObject;
			obj.GetComponent<play_select_sub>().reset(vm);
			obj.SetActive(true);
			num++;
		}
	}

	void clear_second()
	{
		mario._instance.remove_child (m_left_view1);
		m_left_view1.GetComponent<UIScrollView> ().ResetPosition ();
		m_second.Clear ();
	}

	void add_second(protocol.game.smsg_view_map msg, int fx)
	{
		int cur_page = 0;
		if (fx == -1)
		{
			if (m_tp_up_tuo != null)
			{
				Object.Destroy (m_tp_up_tuo);
			}
			cur_page = m_page_up;
			if (m_page_down - m_page_up >= 3)
			{
				for (int i = m_second.Count - 1; i >= 0; --i)
				{
					if (m_second[i].GetComponent<play_select_sub1>().m_page == m_page_down)
					{
						Object.Destroy(m_second[i]);
						m_second.RemoveAt(i);
					}
				}
				m_page_down--;
				if (m_tp_down_tuo != null)
				{
					m_tp_down_tuo.transform.localPosition = new Vector3(0,182 - 850 * m_page_down - 850,0);
				}
				else
				{
					m_tp_down_tuo = (GameObject)Instantiate(m_down_tuo);
					m_tp_down_tuo.transform.parent = m_left_view1.transform;
					m_tp_down_tuo.transform.localPosition = new Vector3(0,182 - 850 * m_page_down - 850,0);
					m_tp_down_tuo.transform.localScale = new Vector3(1,1,1);
					m_tp_down_tuo.SetActive(true);
				}
			}
		}
		else if (fx == 1)
		{
			if (m_tp_down_tuo != null)
			{
				Object.Destroy (m_tp_down_tuo);
			}
			cur_page = m_page_down;
			if (m_page_down - m_page_up >= 3)
			{
				for (int i = m_second.Count - 1; i >= 0; --i)
				{
					if (m_second[i].GetComponent<play_select_sub1>().m_page == m_page_up)
					{
						Object.Destroy(m_second[i]);
						m_second.RemoveAt(i);
					}
				}
				m_page_up++;
				if (m_tp_up_tuo != null)
				{
					m_tp_up_tuo.transform.localPosition = new Vector3(0,182 - 850 * m_page_up + 85,0);
				}
				else
				{
					m_tp_up_tuo = (GameObject)Instantiate(m_up_tuo);
					m_tp_up_tuo.transform.parent = m_left_view1.transform;
					m_tp_up_tuo.transform.localPosition = new Vector3(0,182 - 850 * m_page_up + 85,0);
					m_tp_up_tuo.transform.localScale = new Vector3(1,1,1);
					m_tp_up_tuo.SetActive(true);
				}
			}
		}
		int num = 0;
		for (int i = 0; i < msg.infos.Count; ++i)	
		{
			protocol.game.map_show ms = msg.infos[i];
			GameObject obj = (GameObject)Instantiate(m_play_select_sub1);
			obj.name = "small_sub";
			obj.transform.parent = m_left_view1.transform;
			obj.transform.localPosition = new Vector3(0,182 - 85 * num - 850 * cur_page,0);
			obj.transform.localScale = new Vector3(1,1,1);
			obj.GetComponent<UIButtonMessage>().target = this.gameObject;
			obj.GetComponent<play_select_sub1>().reset(ms, cur_page);
			obj.SetActive(true);
			m_second.Add(obj);
			num++;
		}

		if (fx == -1)
		{
			if (cur_page > 0)
			{
				m_tp_up_tuo = (GameObject)Instantiate(m_up_tuo);
				m_tp_up_tuo.transform.parent = m_left_view1.transform;
				m_tp_up_tuo.transform.localPosition = new Vector3(0,182 - 850 * cur_page + 85,0);
				m_tp_up_tuo.transform.localScale = new Vector3(1,1,1);
				m_tp_up_tuo.SetActive(true);
			}
		}
		else
		{
			if (cur_page < m_max_page - 1)
			{
				m_tp_down_tuo = (GameObject)Instantiate(m_down_tuo);
				m_tp_down_tuo.transform.parent = m_left_view1.transform;
				m_tp_down_tuo.transform.localPosition = new Vector3(0,182 - 850 * cur_page - 850,0);
				m_tp_down_tuo.transform.localScale = new Vector3(1,1,1);
				m_tp_down_tuo.SetActive(true);
			}
		}
	}

	void show_map(protocol.game.map_info mi)
	{
		m_back.SetActive (false);
		m_back1.SetActive (true);
		m_back2.SetActive (false);
		m_mi = mi;
		if (m_mi.difficulty > 0)
		{
			m_map_level.GetComponent<UISprite> ().spriteName = "jbjb_" + m_mi.difficulty.ToString();
		}
		else
		{
			m_map_level.GetComponent<UISprite> ().spriteName = "jbjb_" + utils.get_map_nd (m_mi.pas, m_mi.amount);
		}
		if (m_mi.finish == 0)
		{
			m_map_win.SetActive(false);
			m_map_exp.GetComponent<UILabel>().text = "EXP+" + utils.get_map_exp(m_mi.pas, m_mi.amount).ToString();
		}
		else
		{
			m_map_win.SetActive(true);
			m_map_exp.GetComponent<UILabel>().text = "EXP+2";
		}
		m_rq.GetComponent<UILabel>().text = m_mi.date;
		float tgl = 0;
		if (m_mi.amount > 0)
		{
			tgl = (float)m_mi.pas / m_mi.amount * 100;
		}
		m_tgl.GetComponent<UILabel>().text = tgl.ToString("f2") + "%";
		m_texture.GetComponent<UITexture>().mainTexture = game_data._instance.mission_to_texture (m_mi.url);
		m_map_name.GetComponent<UILabel>().text = m_mi.name;
		m_map_id.GetComponent<UILabel>().text = m_mi.id.ToString();
		m_map_zuo_touxiang.GetComponent<UISprite> ().spriteName = game_data._instance.get_t_touxiang (m_mi.head);
		m_map_zuo_name.GetComponent<UILabel>().text = "[u]" + m_mi.owner_name;
		m_map_zuo_guojia.GetComponent<UISprite> ().spriteName = game_data._instance.get_t_guojia (m_mi.country);
		m_map_dz.GetComponent<UILabel>().text = m_mi.like.ToString("N0");
		m_map_tg.GetComponent<UILabel>().text = m_mi.pas.ToString("N0");
		m_map_cy.GetComponent<UILabel>().text = m_mi.amount.ToString("N0");
		if (m_mi.collect == 0)
		{
			m_map_sc_text.GetComponent<UILabel>().text = game_data._instance.get_language_string("play_select_gui_sc");
		}
		else
		{
			m_map_sc_text.GetComponent<UILabel>().text = game_data._instance.get_language_string("play_select_gui_qxsc");
		}
	}

	void reset_pinlun(List<protocol.game.comment> comments)
	{
		m_comments = comments;
		mario._instance.remove_child(m_pl_panel);
		GameObject pobj = null;
		for (int i = 0; i < m_comments.Count; ++i)
		{
			GameObject obj = (GameObject)Instantiate(m_pl_sub);
			obj.transform.parent = m_pl_panel.transform;
			obj.name = m_comments[i].userid.ToString();
			obj.transform.localPosition = new Vector3(0,-245,0);
			obj.transform.localScale = new Vector3(1,1,1);
			obj.transform.FindChild("touxiang").FindChild("icon").GetComponent<UISprite>().spriteName = game_data._instance.get_t_touxiang(m_comments[i].head);
			obj.transform.FindChild("touxiang").FindChild("name").GetComponent<UILabel>().text = "[u]" + player.get_name(m_comments[i].userid, m_comments[i].name, m_comments[i].visitor);
			obj.transform.FindChild("touxiang").FindChild("guoqi").GetComponent<UISprite>().spriteName = game_data._instance.get_t_guojia(m_comments[i].country);
			obj.transform.FindChild("text").GetComponent<UILabel>().text = m_comments[i].text;
			obj.transform.FindChild("time").GetComponent<UILabel>().text = m_comments[i].date;
			obj.SetActive(true);
			if (pobj != null)
			{
				obj.GetComponent<UIWidget>().topAnchor.target = pobj.transform;
				obj.GetComponent<UIWidget>().topAnchor.relative = 0;
				obj.GetComponent<UIWidget>().topAnchor.absolute = 5;
			}
			pobj = obj;
		}
		if (m_comments.Count == 0)
		{
			m_npl.SetActive(true);
		}
		else
		{
			m_npl.SetActive(false);
		}
		m_right_view.GetComponent<UIScrollView>().ResetPosition();
	}

	void click(GameObject obj)
	{
		if (mario._instance.m_self.guide == 200)
		{
			if (obj.name != "big_sub")
			{
				mario._instance.show_tip(game_data._instance.get_language_string("play_select_gui_jxbs"));
				return;
			}
			int id = obj.GetComponent<play_select_big>().m_type;
			if (id != 200)
			{
				mario._instance.show_tip(game_data._instance.get_language_string("play_select_gui_jxbs"));
				return;
			}
		}
		if (mario._instance.m_self.guide == 201 && obj.name != "play")
		{
			mario._instance.show_tip(game_data._instance.get_language_string("play_select_gui_jxpl"));
			return;
		}
		if (obj.name == "close")
		{
			mario._instance.change_state(e_game_state.egs_login, 1, delegate() { Object.Destroy(this.gameObject); });
		}
		if (obj.name == "big_sub")
		{
			if (m_lan == 0)
			{
				int id = obj.GetComponent<play_select_big>().m_type;
				if (id >= 200)
				{
					int clevel = obj.GetComponent<play_select_big>().m_clevel;
					if (clevel > mario._instance.m_self.level)
					{
						s_t_view_title t_vt = game_data._instance.get_t_view_title(id);
						string s = string.Format(game_data._instance.get_language_string("play_select_gui_ddjv"), clevel, t_vt.name);
						mario._instance.show_tip(s);
						return;
					}
					if (mario._instance.m_self.guide == 200)
					{
						obj.GetComponent<play_select_big>().hide_shou();
						mario._instance.m_self.guide = 201;
					}
					protocol.game.cmsg_view_comment msg = new protocol.game.cmsg_view_comment();
					msg.id = mario._instance.m_self.mapid;
					net_http._instance.send_msg<protocol.game.cmsg_view_comment>(opclient_t.OPCODE_VIEW_COMMENT, msg);
				}
				else if (id == 1)
				{
					change_first(true);
				}
				else if (id == 2)
				{
					protocol.game.cmsg_mission_view msg = new protocol.game.cmsg_mission_view();
					net_http._instance.send_msg<protocol.game.cmsg_mission_view>(opclient_t.OPCODE_MISSION_VIEW, msg);
				}
			}
		}
		if (obj.name == "first_sub")
		{
			if (m_lan == 1)
			{
				m_page_type = obj.GetComponent<play_select_sub>().m_type;
				m_page_up = 0;
				m_page_down = 0;
				m_page_fx = 0;
				protocol.game.cmsg_view_map msg = new protocol.game.cmsg_view_map();
				msg.index = 0;
				msg.type = m_page_type;
				msg.ver = game_data.m_self_map_ver;
				net_http._instance.send_msg<protocol.game.cmsg_view_map>(opclient_t.OPCODE_VIEW_MAP, msg);
			}
		}
		if (obj.name == "return")
		{
			if (m_lan == 2)
			{
				m_page_type = -1;
				change_first(false);
			}
			else if (m_lan == 1)
			{
				m_page_type = -1;
				change_big();
			}
			else
			{
				mario._instance.change_state(e_game_state.egs_login, 1, delegate() { Object.Destroy(this.gameObject); });
			}
		}
		if (obj.name == "small_sub")
		{
			protocol.game.map_show ms = obj.GetComponent<play_select_sub1>().m_ms;
			protocol.game.cmsg_view_comment msg = new protocol.game.cmsg_view_comment();
			msg.id = ms.id;
			net_http._instance.send_msg<protocol.game.cmsg_view_comment>(opclient_t.OPCODE_VIEW_COMMENT, msg);
		}
		if (obj.name == "play")
		{
			m_id = m_mi.id;
			protocol.game.cmsg_play_map msg = new protocol.game.cmsg_play_map();
			msg.id = m_id;
			net_http._instance.send_msg<protocol.game.cmsg_play_map>(opclient_t.OPCODE_PLAY_MAP, msg);
		}
		if (obj.name == "search")
		{
			if (m_lan == 0)
			{
				mario._instance.show_tip(game_data._instance.get_language_string("play_select_gui_zxsc"));
				return;
			}
			m_search.SetActive(true);
			m_search_text.GetComponent<UIInput>().value = "";
		}
		if (obj.name == "search_ok")
		{
			string name = m_search_text.GetComponent<UIInput>().value;
			if (name == "")
			{
				mario._instance.show_tip(game_data._instance.get_language_string("play_select_gui_gjzk"));
				return;
			}
			m_page_type = -1;
			protocol.game.cmsg_search_map msg = new protocol.game.cmsg_search_map();
			msg.name = name;
			net_http._instance.send_msg<protocol.game.cmsg_search_map>(opclient_t.OPCODE_SEARCH_MAP, msg);
		}
		if (obj.name == "search_close")
		{
			m_search.GetComponent<ui_show_anim>().hide_ui();
		}
		if (obj.name == "shc")
		{
			protocol.game.cmsg_favorite_map msg = new protocol.game.cmsg_favorite_map();
			msg.id = m_mi.id;
			net_http._instance.send_msg<protocol.game.cmsg_favorite_map>(opclient_t.OPCODE_FAVORITE_MAP, msg);
		}
		if (obj.name == "pl")
		{
			m_pinglun.SetActive(true);
			m_pinglun_text.GetComponent<UIInput>().value = "";
		}
		if (obj.name == "pinglun_ok")
		{
			string text = m_pinglun_text.GetComponent<UIInput>().value;
			if (text == "")
			{
				mario._instance.show_tip(game_data._instance.get_language_string("play_select_gui_plwk"));
				return;
			}
			protocol.game.cmsg_comment msg = new protocol.game.cmsg_comment();
			msg.id = m_mi.id;
			msg.text = text;
			net_http._instance.send_msg<protocol.game.cmsg_comment>(opclient_t.OPCODE_COMMENT, msg);
		}
		if (obj.name == "pinglun_close")
		{
			m_pinglun.GetComponent<ui_show_anim>().hide_ui();
		}
		if (obj.name == "ph")
		{
			protocol.game.cmsg_view_map_point_rank msg = new protocol.game.cmsg_view_map_point_rank();
			msg.map_id = m_mi.id;
			net_http._instance.send_msg<protocol.game.cmsg_view_map_point_rank>(opclient_t.OPCODE_VIEW_MAP_POINT_RANK, msg);
		}
		if (obj.name == "touxiang")
		{
			look_player(m_mi.owner_id);
		}
		if (obj.name == "br_start")
		{
			protocol.game.cmsg_mission_start msg = new protocol.game.cmsg_mission_start();
			msg.hard = m_bhard;
			net_http._instance.send_msg<protocol.game.cmsg_mission_start>(opclient_t.OPCODE_MISSION_START, msg);
		}
		if (obj.name == "br_continue")
		{
			protocol.game.cmsg_mission_continue msg = new protocol.game.cmsg_mission_continue();
			net_http._instance.send_msg<protocol.game.cmsg_mission_continue>(opclient_t.OPCODE_MISSION_CONTINUE, msg);
		}
		if (obj.name == "download")
		{
			s_message mes = new s_message();
			mes.m_type = "player_select_gui_download";
			mario._instance.show_double_dialog_box(game_data._instance.get_language_string("play_select_gui_dself"), mes);
		}
		if (obj.name == "br_drop")
		{
			s_message mes = new s_message();
			mes.m_type = "player_select_gui_br_drop";
			mario._instance.show_double_dialog_box(game_data._instance.get_language_string("play_select_gui_br_drop"), mes);
		}
	}

	void select_pl(GameObject obj)
	{
		int uid = int.Parse (obj.transform.parent.name);
		look_player (uid);
	}

	void look_player(int uid)
	{
		protocol.game.cmsg_view_player msg = new protocol.game.cmsg_view_player();
		msg.userid = uid;
		net_http._instance.send_msg<protocol.game.cmsg_view_player>(opclient_t.OPCODE_VIEW_PLAYER, msg);
	}

	void show_br()
	{
		if (mario._instance.m_self.br_start == 1)
		{
			m_br_start.SetActive(false);
			m_br_continue.SetActive(true);
			m_br_jd.SetActive(true);
			m_br_select_panel.SetActive(false);
			m_br_life.GetComponent<UILabel>().text = "x " + mario._instance.m_self.br_life.ToString();
			m_br_index.GetComponent<UILabel>().text = (mario._instance.m_self.br_index + 1).ToString() + "/8";
			m_br_bk.GetComponent<UITexture>().mainTexture = m_br_texs[mario._instance.m_self.br_hard - 1];
			s_t_br t_br = game_data._instance.get_t_br(mario._instance.m_self.br_hard);
			m_br_text.GetComponent<UILabel>().text = t_br.desc;
		}
		else
		{
			m_br_start.SetActive(true);
			m_br_continue.SetActive(false);
			m_br_jd.SetActive(false);
			m_br_select_panel.SetActive(true);
			m_br_bk.GetComponent<UITexture>().mainTexture = m_br_texs[0];
			m_br_select.transform.localPosition = new Vector3(-140, 150, 0);
			m_bhard = 1;
			s_t_br t_br = game_data._instance.get_t_br(1);
			m_br_text.GetComponent<UILabel>().text = t_br.desc;
			for (int i = 0; i < m_br_locks.Count; ++i)
			{
				if (mario._instance.m_self.br_max + 1 >= i + 2)
				{
					m_br_locks[i].SetActive(false);
				}
				else
				{
					m_br_locks[i].SetActive(true);
				}
			}
		}
	}

	void select_br(GameObject obj)
	{
		int hard = int.Parse (obj.name.Substring (1));
		s_t_br t_br = game_data._instance.get_t_br(hard);
		if (hard > mario._instance.m_self.br_max + 1)
		{
			mario._instance.show_tip(t_br.unlock);
			return;
		}
		m_bhard = hard;
		m_br_select.transform.localPosition = obj.transform.localPosition;
		m_br_bk.GetComponent<UITexture>().mainTexture = m_br_texs[m_bhard - 1];
		m_br_text.GetComponent<UILabel>().text = t_br.desc;
	}

	public void message (s_message message)
	{
		if (message.m_type == "commit_play")
		{
			m_id = m_mi.id;
			protocol.game.cmsg_play_map msg = new protocol.game.cmsg_play_map();
			msg.id = m_id;
			net_http._instance.send_msg<protocol.game.cmsg_play_map>(opclient_t.OPCODE_PLAY_MAP, msg);
		}
		if (message.m_type == "player_select_gui_download")
		{
			protocol.game.cmsg_download_map msg = new protocol.game.cmsg_download_map();
			msg.id = m_mi.id;
			net_http._instance.send_msg<protocol.game.cmsg_download_map>(opclient_t.OPCODE_DOWNLOAD_MAP, msg);
		}
		if (message.m_type == "player_select_gui_br_drop")
		{
			protocol.game.cmsg_mission_continue msg = new protocol.game.cmsg_mission_continue();
			net_http._instance.send_msg<protocol.game.cmsg_mission_continue>(opclient_t.OPCODE_MISSION_DROP, msg);
		}
	}
	
	public void net_message (s_net_message message)
	{
		if (message.m_opcode == opclient_t.OPCODE_VIEW_MAP)
		{
			protocol.game.smsg_view_map _msg = net_http._instance.parse_packet<protocol.game.smsg_view_map> (message.m_byte);
			m_max_page = _msg.page;
			if (m_page_fx == 0)
			{
				clear_second();
			}
			add_second(_msg, m_page_fx);
			change_second();
		}
		if (message.m_opcode == opclient_t.OPCODE_VIEW_COMMENT)
		{
			protocol.game.smsg_view_comment _msg = net_http._instance.parse_packet<protocol.game.smsg_view_comment> (message.m_byte);
			show_map(_msg.infos);
			reset_pinlun(_msg.comments);
			m_right_view.GetComponent<UIScrollView>().ResetPosition();
			for (int i = 0; i < m_second.Count; ++i)
			{
				if (m_second[i].GetComponent<play_select_sub1>().m_ms.id == _msg.infos.id)
				{
					m_second[i].GetComponent<play_select_sub1>().reset(_msg.infos.finish, _msg.infos.pas, _msg.infos.amount, _msg.infos.like);
				}
			}
			mario._instance.m_self.set_per(_msg.infos.head, _msg.infos.country, _msg.infos.owner_name, _msg.infos.name);
		}
		if (message.m_opcode == opclient_t.OPCODE_PLAY_MAP)
		{
			if (mario._instance.m_self.guide == 201)
			{
				mario._instance.m_self.guide = 202;
			}
			if (message.m_res == -1)
			{
				mario._instance.show_tip(game_data._instance.get_language_string("play_select_gui_tlbz"));
				protocol.game.msg_life_error _msg1 = net_http._instance.parse_packet<protocol.game.msg_life_error> (message.m_byte);
				mario._instance.m_self.set_reset_time(_msg1.server_time, _msg1.life_time);
				return;
			}
			protocol.game.smsg_play_map _msg = net_http._instance.parse_packet<protocol.game.smsg_play_map> (message.m_byte);
			mario_tool._instance.onRaid(m_id.ToString(), 1);
			bool flag = game_data._instance.load_mission(m_id, _msg.map_data, _msg.x, _msg.y);
			if (!flag)
			{
				return;
			}
			mario._instance.change_state(e_game_state.egs_play, 2, delegate() 
			                             { 
				if (this.gameObject.activeSelf)
				{
					this.gameObject.SetActive(false); 
				}
				else
				{
					mario._instance.hide_clear_gui();
				}
			});
		}
		if (message.m_opcode == opclient_t.OPCODE_FAVORITE_MAP)
		{
			protocol.game.smsg_favorite_map _msg = net_http._instance.parse_packet<protocol.game.smsg_favorite_map> (message.m_byte);
			m_mi.favorite = _msg.num;
			m_mi.collect = 1 - m_mi.collect;
			show_map(m_mi);
		}
		if (message.m_opcode == opclient_t.OPCODE_COMMENT)
		{
			mario._instance.show_tip(game_data._instance.get_language_string("play_select_gui_fbpl"));
			m_pinglun.GetComponent<ui_show_anim>().hide_ui();
			protocol.game.smsg_comment _msg = net_http._instance.parse_packet<protocol.game.smsg_comment> (message.m_byte);
			m_comments.Insert(0, _msg.comment);
			if (m_comments.Count > 10)
			{
				m_comments.RemoveAt(m_comments.Count - 1);
			}
			reset_pinlun(m_comments);
		}
		if (message.m_opcode == opclient_t.OPCODE_SEARCH_MAP)
		{
			m_search.GetComponent<ui_show_anim>().hide_ui();
			protocol.game.smsg_view_map _msg = net_http._instance.parse_packet<protocol.game.smsg_view_map> (message.m_byte);
			if (_msg.infos.Count == 0)
			{
				mario._instance.show_tip(game_data._instance.get_language_string("play_select_gui_mzdt"));
				return;
			}
			clear_second();
			add_second(_msg, 0);
			change_second();
		}
		if (message.m_opcode == opclient_t.OPCODE_VIEW_MAP_POINT_RANK)
		{
			protocol.game.smsg_view_map_point_rank _msg = net_http._instance.parse_packet<protocol.game.smsg_view_map_point_rank> (message.m_byte);
			mario._instance.show_paihang_gui(_msg, m_mi.id);
		}
		if (message.m_opcode == opclient_t.OPCODE_VIEW_VIDEO)
		{
			protocol.game.smsg_view_video _msg = net_http._instance.parse_packet<protocol.game.smsg_view_video> (message.m_byte);
			bool flag = game_data._instance.load_mission(m_mi.id, _msg.map_data, null, null);
			if (!flag)
			{
				return;
			}
			game_data._instance.load_inputs(_msg.video_data);
			mario._instance.change_state(e_game_state.egs_review, 2, delegate() { this.gameObject.SetActive(false); mario._instance.hide_paihang_gui(); });
		}
		if (message.m_opcode == opclient_t.OPCODE_VIEW_PLAYER)
		{
			protocol.game.smsg_view_player _msg = net_http._instance.parse_packet<protocol.game.smsg_view_player> (message.m_byte);
			mario._instance.show_player_gui(_msg);
		}
		if (message.m_opcode == opclient_t.OPCODE_MISSION_VIEW)
		{
			protocol.game.smsg_mission_view _msg = net_http._instance.parse_packet<protocol.game.smsg_mission_view> (message.m_byte);
			mario._instance.m_self.br_life = _msg.life;
			mario._instance.m_self.br_index = _msg.index;
			mario._instance.m_self.br_start = _msg.start;
			mario._instance.m_self.br_hard = _msg.hard;
			mario._instance.m_self.br_max = _msg.br_max;
			
			m_back.SetActive(false);
			m_back1.SetActive(false);
			m_back2.SetActive(true);
			show_br();
		}
		if (message.m_opcode == opclient_t.OPCODE_MISSION_START || message.m_opcode == opclient_t.OPCODE_MISSION_CONTINUE)
		{
			if (message.m_opcode == opclient_t.OPCODE_MISSION_START)
			{
				mario._instance.m_self.br_index = 0;
				mario._instance.m_self.br_life = 100;
				mario._instance.m_self.br_start = 1;
				mario._instance.m_self.br_hard = m_bhard;
				mario._instance.m_start_type = 0;
			}
			else
			{
				mario._instance.m_start_type = 2;
			}
			protocol.game.smsg_mission_play _msg = net_http._instance.parse_packet<protocol.game.smsg_mission_play> (message.m_byte);
			mario._instance.m_self.set_br(_msg.user_head, _msg.user_country, _msg.user_name, _msg.map_name);
			game_data._instance.load_mission(-1, _msg.map_data, _msg.x, _msg.y);
			mario._instance.change_state(e_game_state.egs_br_road, 2, delegate() { this.gameObject.SetActive(false); });
		}
		if (message.m_opcode == opclient_t.OPCODE_DOWNLOAD_MAP)
		{
			mario._instance.m_self.download_num++;
			mario._instance.show_tip (game_data._instance.get_language_string("play_select_gui_dlok"));
		}
		if (message.m_opcode == opclient_t.OPCODE_MISSION_DROP)
		{
			mario._instance.m_self.br_start = 0;
			show_br();
		}
	}
}

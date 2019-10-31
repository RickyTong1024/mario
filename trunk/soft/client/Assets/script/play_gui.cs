using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class play_gui : MonoBehaviour, IMessage {

	public GameObject m_play_pause;
	public GameObject m_play_lose;
	public GameObject m_brplay_pause;
	public GameObject m_brplay_lose;
	private int m_win;
	public GameObject m_br_life;
	public GameObject m_pause;
	public GameObject m_play_input;
	public GameObject m_sm;
	public GameObject m_reply;
	public GameObject m_score_list;
	public GameObject m_score_text;
	public GameObject m_time_sp;
	public GameObject m_time_text;
	public GameObject m_ys_text;
	public GameObject m_br_life_text;
	public List<GameObject> m_buttons;
	private List<GameObject> m_scores = new List<GameObject>();
	public GameObject m_score_panel;
	public GameObject m_score_sub;
	public GameObject m_mask;
	public GameObject m_time_up;
	public GameObject m_chuan;
	public GameObject m_x_buttom;

	void Start () {
		cmessage_center._instance.add_handle (this);
	}
	
	void OnDestroy()
	{
		cmessage_center._instance.remove_handle (this);
	}

	void OnEnable()
	{
		m_scores.Clear ();
		mario._instance.remove_child (m_score_panel);
		m_play_pause.SetActive (false);
		m_play_lose.SetActive (false);
		m_brplay_pause.SetActive (false);
		m_brplay_lose.SetActive (false);
		m_mask.SetActive (false);
		m_sm.SetActive(false);
		m_time_up.SetActive (false);
		m_chuan.SetActive (false);
		m_x_buttom.SetActive (false);

		if (mario._instance.m_game_state == e_game_state.egs_play)
		{
			m_pause.SetActive(true);
			m_play_input.SetActive(true);
			m_reply.SetActive(false);
			m_score_list.SetActive(true);
			m_br_life.SetActive(false);
		}
		else if (mario._instance.m_game_state == e_game_state.egs_review)
		{
			m_pause.SetActive(true);
			m_play_input.SetActive(false);
			m_reply.SetActive(true);
			m_score_list.SetActive(true);
			m_br_life.SetActive(false);
		}
		else if (mario._instance.m_game_state == e_game_state.egs_edit)
		{
			m_pause.SetActive(false);
			m_play_input.SetActive(true);
			m_reply.SetActive(false);
			m_score_list.SetActive(false);
			m_br_life.SetActive(false);
		}
		else if (mario._instance.m_game_state == e_game_state.egs_edit_play)
		{
			m_pause.SetActive(true);
			m_play_input.SetActive(true);
			m_reply.SetActive(false);
			m_score_list.SetActive(true);
			m_br_life.SetActive(false);
		}
		else if (mario._instance.m_game_state == e_game_state.egs_edit_upload)
		{
			m_pause.SetActive(true);
			m_play_input.SetActive(true);
			m_reply.SetActive(false);
			m_score_list.SetActive(true);
			m_br_life.SetActive(false);
		}
		else if (mario._instance.m_game_state == e_game_state.egs_br_play)
		{
			m_pause.SetActive(true);
			m_play_input.SetActive(true);
			m_reply.SetActive(false);
			m_score_list.SetActive(true);
			m_br_life.SetActive(true);
		}
#if !UNITY_IPHONE && !UNITY_ANDROID
		m_play_input.SetActive(false);
		if (mario._instance.m_self.level == 1)
		{
			m_sm.SetActive(true);
		}
#endif
		m_time_sp.GetComponent<UISprite> ().spriteName = "mode0" + (game_data._instance.m_map_data.mode + 1).ToString ();
	}

	void OnDisable()
	{
		s_message mes = new s_message();
		mes.m_type = "close_play_mode";
		cmessage_center._instance.add_message(mes);

		if (mario._instance.m_game_state != e_game_state.egs_br_start && mario._instance.m_game_state == e_game_state.egs_br_road)
		{
			mario._instance.play_mus("music/select");
		}
	}

#if UNITY_WEBPLAYER
	void OnApplicationFocus(bool focus)
	{
		if (!focus)
		{
			m_mask.SetActive(true);
		}
		else
		{
			m_mask.SetActive(false);
		}
	}
#endif

	public void message (s_message message)
	{
		if (message.m_type == "play_win")
		{
			m_win = 0;
			if (mario._instance.m_game_state == e_game_state.egs_play)
			{
				List<int> input = (List<int>)message.m_object[0];
				protocol.game.cmsg_complete_map msg = new protocol.game.cmsg_complete_map();
				msg.suc = 0;
				msg.point = (int)message.m_ints[0];
				msg.time = (int)message.m_ints[1];
				msg.video = game_data._instance.save_inputs(input);
				net_http._instance.send_msg<protocol.game.cmsg_complete_map>(opclient_t.OPCODE_COMPLETE_MAP, msg);
			}
			else if (mario._instance.m_game_state == e_game_state.egs_review)
			{
				mario._instance.change_state(e_game_state.egs_play_select, 1, delegate() { this.gameObject.SetActive(false); });
			}
			else if (mario._instance.m_game_state == e_game_state.egs_edit)
			{

			}
			else if (mario._instance.m_game_state == e_game_state.egs_edit_play)
			{
				mario._instance.change_state(e_game_state.egs_edit_select, 1, delegate() { this.gameObject.SetActive(false); });
			}
			else if (mario._instance.m_game_state == e_game_state.egs_edit_upload)
			{
				s_message mes = new s_message();
				mes.m_type = "play_gui_uplw_ok";
				mes.m_object.Add(message);
				s_message mes1 = new s_message();
				mes1.m_type = "play_gui_uplw_cancel";
				mario._instance.show_double_dialog_box(game_data._instance.get_language_string("play_gui_uplw"), mes, mes1);
			}
			else if (mario._instance.m_game_state == e_game_state.egs_br_play)
			{
				protocol.game.cmsg_mission_success msg = new protocol.game.cmsg_mission_success();
				msg.point = (int)message.m_ints[0];
				msg.time = (int)message.m_ints[1];
				net_http._instance.send_msg<protocol.game.cmsg_mission_success>(opclient_t.OPCODE_MISSION_SUCCESS, msg);
			}
		}
		if (message.m_type == "play_lose")
		{
			m_win = 1;
			if (mario._instance.m_game_state == e_game_state.egs_play)
			{
				mario_point p = (mario_point)message.m_object[0];
				protocol.game.cmsg_complete_map msg = new protocol.game.cmsg_complete_map();
				msg.suc = 1;
				msg.x = p.x;
				msg.y = p.y;
				net_http._instance.send_msg<protocol.game.cmsg_complete_map>(opclient_t.OPCODE_COMPLETE_MAP, msg);
			}
			else if (mario._instance.m_game_state == e_game_state.egs_review)
			{
				mario._instance.change_state(e_game_state.egs_play_select, 1, delegate() { this.gameObject.SetActive(false); });
			}
			else if (mario._instance.m_game_state == e_game_state.egs_edit)
			{
				
			}
			else if (mario._instance.m_game_state == e_game_state.egs_edit_play)
			{
				m_play_lose.SetActive(true);
			}
			else if (mario._instance.m_game_state == e_game_state.egs_edit_upload)
			{
				m_play_lose.SetActive(true);
			}
			else if (mario._instance.m_game_state == e_game_state.egs_br_play)
			{
				mario_point p = (mario_point)message.m_object[0];
				protocol.game.cmsg_mission_fail msg = new protocol.game.cmsg_mission_fail();
				msg.x = p.x;
				msg.y = p.y;
				net_http._instance.send_msg<protocol.game.cmsg_mission_fail>(opclient_t.OPCODE_MISSION_FAIL, msg);
			}
		}
		if (message.m_type == "add_score")
		{
			int x = (int)message.m_ints[0];
			int y = (int)message.m_ints[1];
			int s = (int)message.m_ints[2];
			GameObject obj = (GameObject)Instantiate(m_score_sub);
			obj.transform.parent = m_score_panel.transform;
			obj.transform.localPosition = new Vector3 (x, y, 0);
			obj.transform.localScale = new Vector3 (1, 1, 1);
			obj.GetComponent<score>().reset(s);
			obj.SetActive(true);
			m_scores.Add(obj);
		}
		if (message.m_type == "time_up")
		{
			m_time_up.SetActive (true);
		}
		if (message.m_type == "play_gui_uplw_ok")
		{
			s_message mes = (s_message)message.m_object[0];
			List<int> input = (List<int>)mes.m_object[0];

			protocol.game.cmsg_upload_map msg = new protocol.game.cmsg_upload_map();
			msg.id = game_data._instance.m_map_id;
			msg.ver = game_data.m_self_map_ver;
			msg.time = (int)mes.m_ints[1];
			msg.video = game_data._instance.save_inputs(input);
			net_http._instance.send_msg<protocol.game.cmsg_upload_map>(opclient_t.OPCODE_UPLOAD_MAP, msg);
		}
		if (message.m_type == "play_gui_uplw_cancel")
		{			
			protocol.game.cmsg_upload_map msg = new protocol.game.cmsg_upload_map();
			msg.id = game_data._instance.m_map_id;
			msg.ver = game_data.m_self_map_ver;
			net_http._instance.send_msg<protocol.game.cmsg_upload_map>(opclient_t.OPCODE_UPLOAD_MAP, msg);
		}
	}

	public void net_message (s_net_message message)
	{
		if (message.m_opcode == opclient_t.OPCODE_COMPLETE_MAP)
		{
			protocol.game.smsg_complete_map _msg = net_http._instance.parse_packet<protocol.game.smsg_complete_map> (message.m_byte);
			if (m_win == 0)
			{
				bool next = false;
				if (_msg.mapid != 0)
				{
					mario._instance.m_self.mapid = _msg.mapid;
					mario._instance.m_self.support = _msg.support;
					int level = game_data._instance.get_zm(_msg.support);
					if (level > 0 && level <= mario._instance.m_self.level)
					{
						next = true;
					}
				}
				mario._instance.m_self.add_exp(_msg.exp + _msg.extra_exp);
				if (mario._instance.m_self.m_review == 0)
				{
					this.gameObject.SetActive(false);
					mario._instance.show_clear_gui(_msg.exp, _msg.extra_exp, _msg.rank, _msg.testify, next);
				}
				else
				{
					mario._instance.change_state(e_game_state.egs_play_select, 1, delegate() { this.gameObject.SetActive(false); });
				}
			}
			else
			{
				m_play_lose.SetActive(true);
			}
		}
		if (message.m_opcode == opclient_t.OPCODE_REPLAY_MAP)
		{
			if (message.m_res == -1)
			{
				mario._instance.show_tip(game_data._instance.get_language_string("play_gui_tlbz"));
				protocol.game.msg_life_error _msg1 = net_http._instance.parse_packet<protocol.game.msg_life_error> (message.m_byte);
				mario._instance.m_self.set_reset_time(_msg1.server_time, _msg1.life_time);
				return;
			}
			mario_tool._instance.onRaid(game_data._instance.m_map_id.ToString(), 1);
			m_play_pause.GetComponent<ui_show_anim>().hide_ui();
			m_play_lose.GetComponent<ui_show_anim>().hide_ui();
			mario._instance.show_play_mask(delegate() { play_mode._instance.reload(); m_time_up.SetActive (false); });

			mario._instance.play_mus_ex(game_data._instance.get_map_music(0), true, 1 - game_data._instance.m_map_data.no_music);
		}
		if (message.m_opcode == opclient_t.OPCODE_UPLOAD_MAP)
		{
			mario._instance.show_tip(game_data._instance.get_language_string("play_gui_sccg"));
			mario._instance.m_self.add_job_exp(100);
			mario._instance.m_self.upload++;
			mario._instance.change_state(e_game_state.egs_edit_select, 1, delegate() { this.gameObject.SetActive(false); });
		}
		if (message.m_opcode == opclient_t.OPCODE_MISSION_FAIL)
		{
			if (message.m_res == -1)
			{
				mario._instance.m_self.m_finish = net_http._instance.parse_packet<protocol.game.smsg_mission_finish> (message.m_byte);
				mario._instance.m_self.m_finish_type = 1;
				mario._instance.m_self.br_start = 0;
				mario._instance.m_self.set_br(0, "", "", "");
				mario._instance.change_state(e_game_state.egs_br_end, 1, delegate() { this.gameObject.SetActive(false); });
			}
			else
			{
				m_brplay_lose.SetActive(true);
			}
		}
		if (message.m_opcode == opclient_t.OPCODE_MISSION_REPLAY)
		{
			mario._instance.m_self.br_life--;
			mario._instance.m_start_type = 1;
			mario._instance.change_state(e_game_state.egs_br_start, 1, delegate() { this.gameObject.SetActive(false); });
		}
		if (message.m_opcode == opclient_t.OPCODE_MISSION_SUCCESS)
		{
			if (message.m_res == -1)
			{
				mario._instance.m_self.m_finish = net_http._instance.parse_packet<protocol.game.smsg_mission_finish> (message.m_byte);
				mario._instance.m_self.m_finish_type = 0;
				mario._instance.m_self.br_start = 2;
				mario._instance.m_self.set_br(0, "", "", "");
				mario._instance.show_clear_gui(false);
				this.gameObject.SetActive(false);
			}
			else
			{
				protocol.game.smsg_mission_play _msg = net_http._instance.parse_packet<protocol.game.smsg_mission_play> (message.m_byte);
				mario._instance.m_self.set_br(_msg.user_head, _msg.user_country, _msg.user_name, _msg.map_name);
				game_data._instance.load_mission(-1, _msg.map_data, _msg.x, _msg.y);
				mario._instance.m_self.br_index++;
				mario._instance.m_start_type = 2;
				mario._instance.show_clear_gui(true);
				this.gameObject.SetActive(false);
			}
		}
	}

	void pause()
	{
		if (!play_mode._instance.m_pause)
		{
			play_mode._instance.m_pause = true;
			if (mario._instance.m_game_state == e_game_state.egs_br_play)
			{
				m_brplay_pause.SetActive(true);
			}
			else
			{
				m_play_pause.SetActive(true);
			}
		}
		else
		{
			play_mode._instance.m_pause = false;
			if (mario._instance.m_game_state == e_game_state.egs_br_play)
			{
				m_brplay_pause.GetComponent<ui_show_anim>().hide_ui();
			}
			else
			{
				m_play_pause.GetComponent<ui_show_anim>().hide_ui();
			}
		}
	}

	void restart()
	{
		if (mario._instance.m_game_state == e_game_state.egs_play)
		{
			protocol.game.cmsg_replay_map msg = new protocol.game.cmsg_replay_map();
			net_http._instance.send_msg<protocol.game.cmsg_replay_map>(opclient_t.OPCODE_REPLAY_MAP, msg);
		}
		else if (mario._instance.m_game_state == e_game_state.egs_br_play)
		{
			if (mario._instance.m_self.br_life <= 1)
			{
				mario._instance.show_tip(game_data._instance.get_language_string("play_gui_smbz"));
				return;
			}
			protocol.game.cmsg_mission_replay msg = new protocol.game.cmsg_mission_replay();
			net_http._instance.send_msg<protocol.game.cmsg_mission_replay>(opclient_t.OPCODE_MISSION_REPLAY, msg);
		}
		else
		{
			m_play_lose.GetComponent<ui_show_anim>().hide_ui();
			m_play_pause.GetComponent<ui_show_anim>().hide_ui();
			mario._instance.show_play_mask(delegate() { play_mode._instance.reload(); m_time_up.SetActive (false); });
			
			mario._instance.play_mus_ex(game_data._instance.get_map_music(0), true, 1 - game_data._instance.m_map_data.no_music);
		}
	}

	void click(GameObject obj)
	{
		if (obj.name == "pause")
		{
			pause();
		}
		if (obj.name == "restart")
		{
			restart();
		}
		if (obj.name == "continue")
		{
			pause();
		}
		if (obj.name == "return")
		{
			if (mario._instance.m_game_state == e_game_state.egs_play)
			{
				mario._instance.change_state(e_game_state.egs_play_select, 1, delegate() { this.gameObject.SetActive(false); });
			}
			else if (mario._instance.m_game_state == e_game_state.egs_review)
			{
				mario._instance.change_state(e_game_state.egs_play_select, 1, delegate() { this.gameObject.SetActive(false); });
			}
			else if (mario._instance.m_game_state == e_game_state.egs_edit_play)
			{
				mario._instance.change_state(e_game_state.egs_edit_select, 1, delegate() { this.gameObject.SetActive(false); });
			}
			else if (mario._instance.m_game_state == e_game_state.egs_edit_upload)
			{
				mario._instance.change_state(e_game_state.egs_edit_select, 1, delegate() { this.gameObject.SetActive(false); });
			}
			else if (mario._instance.m_game_state == e_game_state.egs_br_play)
			{
				mario._instance.change_state(e_game_state.egs_play_select, 1, delegate() { this.gameObject.SetActive(false); });
			}
		}
	}

	void br_end()
	{
		mario._instance.m_self.m_finish_type = 1;
		mario._instance.change_state(e_game_state.egs_br_end, 1, delegate() { this.gameObject.SetActive(false); });
	}

	void calc_touch()
	{
		if (Application.isEditor)
		{
			return;
		}
#if UNITY_ANDROID || UNITY_IPHONE
		List<Vector3> button_poses = new List<Vector3>();
		for (int i = 0; i < m_buttons.Count; ++i)
		{
			Vector3 v = Vector3.zero;
			Transform t = m_buttons[i].transform;
			do
			{
				v += t.localPosition;
				t = t.parent;
			} while (t.gameObject != UICamera.currentCamera.gameObject);
			button_poses.Add(v);
		}
		if (button_poses.Count == 0)
		{
			return;
		}
		List<bool> bflags = new List<bool> ();
		for (int j = 0; j < button_poses.Count; ++j)
		{
			bflags.Add(false);
		}
		for (int i = 0; i < Input.touchCount; ++i)
		{
			Vector3 v = Input.GetTouch(i).position;
			Vector3 uiPos = UICamera.currentCamera.ScreenToWorldPoint(new Vector3(v.x, v.y, 0f));
			uiPos = UICamera.currentCamera.transform.InverseTransformPoint(uiPos);
			for (int j = 0; j < button_poses.Count; ++j)
			{
				if ((button_poses[j] - uiPos).magnitude < 100)
				{
					bflags[j] = true;
				}
			}
		}
		for (int j = 0; j < button_poses.Count; ++j)
		{
			if (j == 3)
			{
				if (bflags[j] && !play_mode._instance.m_now_inputs[j])
				{
					m_buttons[j].GetComponent<UISprite>().spriteName = "xtb_jinmen_01";
					play_mode._instance.m_now_inputs[j] = true;
				}
				else if (!bflags[j] && play_mode._instance.m_now_inputs[j])
				{
					m_buttons[j].GetComponent<UISprite>().spriteName = "xtb_jinmen";
					play_mode._instance.m_now_inputs[j] = false;
				}
			}
			else
			{
				if (bflags[j] && !play_mode._instance.m_now_inputs[j])
				{
					m_buttons[j].GetComponent<UISprite>().spriteName = "xtb_fx_01";
					play_mode._instance.m_now_inputs[j] = true;
				}
				else if (!bflags[j] && play_mode._instance.m_now_inputs[j])
				{
					m_buttons[j].GetComponent<UISprite>().spriteName = "xtb_fx";
					play_mode._instance.m_now_inputs[j] = false;
				}
			}
		}
#endif
	}

	void FixedUpdate()
	{
		if (play_mode._instance == null)
		{
			return;
		}
		int t = game_data._instance.m_map_data.time - play_mode._instance.m_time / 50;
		if (t < 0)
		{
			t = 0;
		}
		string s = t.ToString ();
		while (s.Length < 3)
		{
			s = "0" + s;
		}
		m_time_text.GetComponent<UILabel>().text = s;

		s = play_mode._instance.m_score.ToString ();
		while (s.Length < 9)
		{
			s = "0" + s;
		}
		m_score_text.GetComponent<UILabel>().text = s;
		m_ys_text.GetComponent<UILabel> ().text = "x" + play_mode._instance.m_ys.ToString();
		m_br_life_text.GetComponent<UILabel> ().text = "x" + mario._instance.m_self.br_life.ToString ();

		calc_touch();

		List<GameObject> dobjs = new List<GameObject> ();
		for (int i = 0; i < m_scores.Count; ++i)
		{
			m_scores[i].GetComponent<score>().update_ex();
			if (m_scores[i].GetComponent<score>().m_time > 50)
			{
				dobjs.Add(m_scores[i]);
			}
		}
		for (int i = 0; i < dobjs.Count; ++i)
		{
			m_scores.Remove(dobjs[i]);
			Object.Destroy(dobjs[i]);
		}
		m_score_panel.transform.localPosition = new Vector3 (-play_mode._instance.m_roll.x / 10, (-play_mode._instance.m_roll.y - utils.g_roll_y) / 10);

		if (play_mode._instance.can_show_chuan())
		{
#if UNITY_ANDROID || UNITY_IPHONE
			if (!m_chuan.activeSelf)
			{
				m_chuan.SetActive(true);
			}
#else
			if (!m_x_buttom.activeSelf)
			{
				m_x_buttom.SetActive(true);
			}
#endif
		}
		else
		{
#if UNITY_ANDROID || UNITY_IPHONE
			if (m_chuan.activeSelf)
			{
				m_chuan.SetActive(false);
			}
#else
			if (m_x_buttom.activeSelf)
			{
				m_x_buttom.SetActive(false);
			}
#endif
		}
	}

	void Update()
	{
		if (play_mode._instance == null)
		{
			return;
		}
		#if !UNITY_IPHONE && !UNITY_ANDROID
		if (mario._instance.key_down(KeyCode.LeftArrow) || mario._instance.key_down(KeyCode.RightArrow) || mario._instance.key_down(KeyCode.Z))
		{
			m_sm.SetActive(false);
		}
		#endif
		if (mario._instance.key_down(KeyCode.Escape))
		{
			pause();
		}
		if ((m_play_pause.activeSelf || m_play_lose.activeSelf || m_brplay_pause.activeSelf || m_brplay_lose.activeSelf) && mario._instance.key_down(KeyCode.X))
		{
			restart();
		}
	}
}

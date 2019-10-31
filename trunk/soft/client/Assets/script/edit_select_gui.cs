using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class edit_select_gui : MonoBehaviour, IMessage {

	public GameObject m_clip;
	public GameObject m_panel;
	public GameObject m_info;
	public GameObject m_cn;
	public GameObject m_cn_text;
	public GameObject m_name;
	public GameObject m_rq;
	public GameObject m_texture;
	private List<GameObject> m_clips = new List<GameObject>();
	private List<protocol.game.edit_data> m_ed = new List<protocol.game.edit_data>();
	private int m_index = 0;
	private int m_player_type = 0;
	private string m_cname;
	public GameObject m_level;
	public GameObject m_exp_bar;
	public GameObject m_exp_text;
	public GameObject m_tip;

	void Start () {
		cmessage_center._instance.add_handle (this);
		reset ();
	}
	
	void OnDestroy()
	{
		cmessage_center._instance.remove_handle (this);
	}

	public void reset()
	{
		mario._instance.show_user (this.gameObject);

		for (int i = 0; i < 3; ++i)
		{
			for (int j = 0; j < 4; ++j)
			{
				GameObject obj = (GameObject)Instantiate(m_clip);
				obj.name = "clip" + (i * 4 + j).ToString();
				obj.transform.parent = m_panel.transform;
				obj.transform.localPosition = new Vector3(-345 + 230 * j, 100 - 160 * i,0);
				obj.transform.localScale = new Vector3(1,1,1);
				obj.GetComponent<UIButtonMessage>().target = this.gameObject;
				obj.SetActive(true);
				m_clips.Add(obj);
			}
		}
		protocol.game.cmsg_view_edit msg = new protocol.game.cmsg_view_edit();
		net_http._instance.send_msg<protocol.game.cmsg_view_edit>(opclient_t.OPCODE_VIEW_EDIT, msg);
	}

	void OnEnable()
	{
		if (mario._instance.m_self.guide < 100)
		{
			mario._instance.m_self.job_level = 1;
			mario._instance.m_self.job_exp = 0;
			reset_exp();
		}
		else if (m_info.gameObject.activeSelf)
		{
			protocol.game.cmsg_view_edit_single msg = new protocol.game.cmsg_view_edit_single();
			msg.map_id = m_ed[m_index].id;
			net_http._instance.send_msg<protocol.game.cmsg_view_edit_single>(opclient_t.OPCODE_VIEW_EDIT_SINGLE, msg);

			reset_exp();
		}
		else
		{
			protocol.game.cmsg_view_edit msg = new protocol.game.cmsg_view_edit();
			net_http._instance.send_msg<protocol.game.cmsg_view_edit>(opclient_t.OPCODE_VIEW_EDIT, msg);
		}
	}

	public void guide()
	{
		TextAsset data = Resources.Load("mission/jx") as TextAsset;
		game_data._instance.load_mission(0, data.bytes, null, null);
		mario._instance.change_state(e_game_state.egs_edit, 0, delegate() { this.gameObject.SetActive(false); });
	}

	void reset_exp()
	{
		m_level.GetComponent<UILabel> ().text = mario._instance.m_self.job_level.ToString();
		s_t_job_exp t_job_exp = game_data._instance.get_t_job_exp (mario._instance.m_self.job_level + 1);
		if (t_job_exp != null)
		{
			float v = (float)mario._instance.m_self.job_exp / t_job_exp.exp;
			m_exp_bar.GetComponent<UIProgressBar>().value = v;
			m_exp_text.GetComponent<UILabel>().text = mario._instance.m_self.job_exp.ToString() + "/" + t_job_exp.exp.ToString();
		}
		else
		{
			m_exp_bar.GetComponent<UIProgressBar>().value = 1;
			m_exp_text.GetComponent<UILabel>().text = mario._instance.m_self.job_exp.ToString() + "/--";
		}
		t_job_exp = game_data._instance.get_t_job_exp (mario._instance.m_self.job_level);
	}

	void reset_info()
	{
		m_rq.GetComponent<UILabel>().text = m_ed[m_index].date;
		m_name.GetComponent<UILabel>().text = m_ed[m_index].name;
		m_texture.GetComponent<UITexture>().mainTexture = game_data._instance.mission_to_texture(m_ed[m_index].url);
	}

	void reset_index()
	{
		if (m_ed[m_index].id == 0)
		{
			m_clips[m_index].transform.FindChild("name").GetComponent<UILabel>().text = "";
			m_clips[m_index].transform.FindChild("Texture").GetComponent<UITexture>().mainTexture = game_data._instance.mission_to_texture(new byte[0]);
			m_clips[m_index].transform.FindChild("state").gameObject.SetActive(false);
			m_info.GetComponent<ui_show_anim>().hide_ui();
		}
		else
		{
			m_clips[m_index].transform.FindChild("name").GetComponent<UILabel>().text = m_ed[m_index].name;
			m_clips[m_index].transform.FindChild("Texture").GetComponent<UITexture>().mainTexture = game_data._instance.mission_to_texture(m_ed[m_index].url);
			if (m_ed[m_index].upload == 1)
			{
				m_clips[m_index].transform.FindChild("state").gameObject.SetActive(true);
			}
			reset_info ();
		}
	}

	public void message (s_message message)
	{
		if (message.m_type == "edit_delete_map")
		{
			protocol.game.cmsg_delete_map msg = new protocol.game.cmsg_delete_map();
			msg.id = m_ed[m_index].id;
			net_http._instance.send_msg<protocol.game.cmsg_delete_map>(opclient_t.OPCODE_DELETE_MAP, msg);
		}
		if (message.m_type == "edit_upload_map")
		{
			m_player_type = 2;
			protocol.game.cmsg_play_edit_map msg = new protocol.game.cmsg_play_edit_map();
			msg.id = m_ed[m_index].id;
			net_http._instance.send_msg<protocol.game.cmsg_play_edit_map>(opclient_t.OPCODE_PLAY_EDIT_MAP, msg);
		}
		if (message.m_type == "jx_6")
		{
			mario._instance.change_state(e_game_state.egs_play_select, 1, delegate() { Object.Destroy(this.gameObject); });
		}
	}

	public void net_message (s_net_message message)
	{
		if (message.m_opcode == opclient_t.OPCODE_VIEW_EDIT)
		{
			protocol.game.smsg_view_edit _msg = net_http._instance.parse_packet<protocol.game.smsg_view_edit> (message.m_byte);
			m_ed = _msg.infos;
			for (int i = 0; i < _msg.infos.Count; ++i)
			{
				protocol.game.edit_data ed = _msg.infos[i];
				if (ed.id != 0)
				{
					m_clips[i].transform.FindChild("name").GetComponent<UILabel>().text = ed.name;
					m_clips[i].transform.FindChild("Texture").GetComponent<UITexture>().mainTexture = game_data._instance.mission_to_texture(ed.url);
					if (ed.upload == 1)
					{
						m_clips[i].transform.FindChild("state").gameObject.SetActive(true);
					}
				}
			}
			mario._instance.m_self.job_level = _msg.level;
			mario._instance.m_self.job_exp = _msg.exp;
			reset_exp();
			if (mario._instance.m_self.guide == 200)
			{
				s_message mes = new s_message();
				mes.m_type = "jx_6";
				mario._instance.show_xsjx_dialog_box(game_data._instance.get_language_string("edit_gui_pljx"), mes);
			}
		}
		if (message.m_opcode == opclient_t.OPCODE_CREATE_MAP)
		{
			protocol.game.smsg_create_map _msg = net_http._instance.parse_packet<protocol.game.smsg_create_map> (message.m_byte);
			m_ed[m_index] = _msg.map;
			reset_index();
			m_info.SetActive (true);
		}
		if (message.m_opcode == opclient_t.OPCODE_PLAY_EDIT_MAP)
		{
			protocol.game.smsg_play_edit_map _msg = net_http._instance.parse_packet<protocol.game.smsg_play_edit_map> (message.m_byte);
			bool flag = game_data._instance.load_mission(m_ed[m_index].id, _msg.mapdata, null, null);
			if (!flag)
			{
				return;
			}
			if (m_player_type == 0)
			{
				mario._instance.change_state(e_game_state.egs_edit, 1, delegate() { this.gameObject.SetActive(false); });
			}
			else if (m_player_type == 1)
			{
				mario._instance.change_state(e_game_state.egs_edit_play, 2, delegate() { this.gameObject.SetActive(false); });
			}
			else if (m_player_type == 2)
			{
				mario._instance.change_state(e_game_state.egs_edit_upload, 2, delegate() { this.gameObject.SetActive(false); });
			}
		}
		if (message.m_opcode == opclient_t.OPCODE_DELETE_MAP)
		{
			m_ed[m_index].id = 0;
			reset_index();
		}
		if (message.m_opcode == opclient_t.OPCODE_CHANGE_MAP_NAME)
		{
			m_cn.GetComponent<ui_show_anim>().hide_ui();
			m_ed[m_index].name = m_cname;
			reset_index();
		}
		if (message.m_opcode == opclient_t.OPCODE_VIEW_EDIT_SINGLE)
		{
			protocol.game.smsg_view_edit_single _msg = net_http._instance.parse_packet<protocol.game.smsg_view_edit_single> (message.m_byte);
			m_ed[m_index] = _msg.info;
			reset_index();
		}
	}

	void click(GameObject obj)
	{
		if (obj.name == "close")
		{
			mario._instance.change_state(e_game_state.egs_login, 1, delegate() { Object.Destroy(this.gameObject); });
		}
		if (obj.name == "close_info")
		{
			m_info.GetComponent<ui_show_anim>().hide_ui();
		}
		if (obj.name == "edit")
		{
			if (m_ed[m_index].upload == 1)
			{
				mario._instance.show_tip(game_data._instance.get_language_string("edit_select_gui_wfxg"));
				return;
			}
			m_player_type = 0;
			protocol.game.cmsg_play_edit_map msg = new protocol.game.cmsg_play_edit_map();
			msg.id = m_ed[m_index].id;
			net_http._instance.send_msg<protocol.game.cmsg_play_edit_map>(opclient_t.OPCODE_PLAY_EDIT_MAP, msg);
		}
		if (obj.name == "play")
		{
			m_player_type = 1;
			protocol.game.cmsg_play_edit_map msg = new protocol.game.cmsg_play_edit_map();
			msg.id = m_ed[m_index].id;
			net_http._instance.send_msg<protocol.game.cmsg_play_edit_map>(opclient_t.OPCODE_PLAY_EDIT_MAP, msg);
		}
		if (obj.name == "upload")
		{
			if (mario._instance.m_self.visitor == 1)
			{
				mario._instance.show_tip(game_data._instance.get_language_string("edit_select_gui_ykwf"));
				return;
			}
			if (m_ed[m_index].upload == 1)
			{
				mario._instance.show_tip(game_data._instance.get_language_string("edit_select_gui_wfsc"));
				return;
			}
			if (m_ed[m_index].url.Length == 0)
			{
				mario._instance.show_tip(game_data._instance.get_language_string("edit_select_gui_wfwbj"));
				return;
			}
			if (m_ed[m_index].name == "empty")
			{
				mario._instance.show_tip(game_data._instance.get_language_string("edit_select_gui_xgmz"));
				return;
			}
			s_message mes = new s_message();
			mes.m_type = "edit_upload_map";
			mario._instance.show_double_dialog_box(game_data._instance.get_language_string("edit_select_gui_tgsc"), mes);
		}
		if (obj.name == "delete")
		{
			s_message mes = new s_message();
			mes.m_type = "edit_delete_map";
			mario._instance.show_double_dialog_box(game_data._instance.get_language_string("edit_select_gui_sfsc"), mes);
		}
		if (obj.name == "cn")
		{
			if (m_ed[m_index].upload == 1)
			{
				mario._instance.show_tip(game_data._instance.get_language_string("edit_select_gui_wfxg"));
				return;
			}
			m_cn_text.GetComponent<UIInput>().value = m_ed[m_index].name;
			m_cn.SetActive(true);
		}
		if (obj.name == "cnok")
		{
			string name = m_cn_text.GetComponent<UIInput>().value;
			if (name == "")
			{
				mario._instance.show_tip(game_data._instance.get_language_string("edit_select_gui_mzbk"));
				return;
			}
			m_cname = name;
			protocol.game.cmsg_change_map_name msg = new protocol.game.cmsg_change_map_name();
			msg.id = m_ed[m_index].id;
			msg.name = name;
			net_http._instance.send_msg<protocol.game.cmsg_change_map_name>(opclient_t.OPCODE_CHANGE_MAP_NAME, msg);
		}
		if (obj.name == "close_cn")
		{
			m_cn.GetComponent<ui_show_anim>().hide_ui();
		}
		if (obj.name == "tip")
		{
			m_tip.SetActive(true);
		}
		if (obj.name == "close_tip")
		{
			m_tip.GetComponent<ui_show_anim>().hide_ui();
		}
	}

	void select(GameObject obj)
	{
		m_index = int.Parse (obj.name.Substring(4, obj.name.Length - 4));
		if (m_ed[m_index].id == 0)
		{
			protocol.game.cmsg_create_map msg = new protocol.game.cmsg_create_map();
			msg.index = m_index;
			net_http._instance.send_msg<protocol.game.cmsg_create_map>(opclient_t.OPCODE_CREATE_MAP, msg);
		}
		else
		{
			m_info.SetActive (true);
			reset_info ();
		}
	}
}

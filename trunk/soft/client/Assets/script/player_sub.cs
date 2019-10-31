﻿using UnityEngine;
using System.Collections;

public class player_sub : MonoBehaviour {

	public GameObject m_texture;
	public GameObject m_name;
	public GameObject m_text;
	private int m_id;
	private GameObject m_player_gui;

	public void reset(int type, int id, string name, byte[] url, int def, string time, GameObject obj)
	{
		m_id = id;
		m_player_gui = obj;
		m_texture.GetComponent<UITexture>().mainTexture = game_data._instance.mission_to_texture(url);
		m_name.GetComponent<UILabel> ().text = name;
		string s = "";
		if (type == 1)
		{
			s = string.Format(game_data._instance.get_language_string("play_sub_type1"), time, def);
		}
		else if (type == 2)
		{
			s = string.Format(game_data._instance.get_language_string("play_sub_type2"), time);
		}
		else if (type == 3)
		{
			s = string.Format(game_data._instance.get_language_string("play_sub_type3"), def.ToString("N0"));
		}
		m_text.GetComponent<UILabel> ().text = s;
	}

	void click(GameObject obj)
	{
		m_player_gui.GetComponent<ui_show_anim>().hide_ui();
		
		protocol.game.cmsg_view_comment msg = new protocol.game.cmsg_view_comment();
		msg.id = m_id;
		net_http._instance.send_msg<protocol.game.cmsg_view_comment>(opclient_t.OPCODE_VIEW_COMMENT, msg);
	}
}

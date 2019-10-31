using UnityEngine;
using System.Collections;

public class clear_gui : MonoBehaviour, IMessage {

	public GameObject m_sub;
	public GameObject m_panel;
	public GameObject m_text1;
	public GameObject m_text2;
	public GameObject m_map_clear;
	public GameObject m_next_panel;
	public GameObject m_next_panel1;
	public GameObject m_next_panel_def;
	public GameObject m_touxiang;
	public GameObject m_gq;
	public GameObject m_name;
	public GameObject m_map_name;
	public GameObject m_touxiang1;
	public GameObject m_gq1;
	public GameObject m_name1;
	public GameObject m_map_name1;
	public GameObject m_l2;
	public GameObject m_l4;
	public GameObject m_like;
	public GameObject m_ydz;
	private int m_index = 0;
	private int m_time = 0;
	private bool m_next = false;
	private int m_type = 0;

	void Start () {
		cmessage_center._instance.add_handle (this);
	}
	
	void OnDestroy()
	{
		cmessage_center._instance.remove_handle (this);
	}

	public void reset(int type, int exp, int eexp, int rank, int testify, bool next)
	{
		m_type = type;
		m_next = next;
		mario._instance.play_mus_ex1 ("music/clear", false);
		this.gameObject.SetActive (true);
		this.GetComponent<Animator> ().enabled = true;
		m_map_clear.SetActive(true);
		m_next_panel.SetActive(false);
		m_next_panel1.SetActive(false);
		m_next_panel_def.SetActive(false);
		m_like.SetActive (true);
		m_ydz.SetActive (false);
		mario._instance.remove_child (m_panel);
		m_index = 0;
		m_time = 0;
		if (m_type == 0)
		{
			string s = "exp +" + exp.ToString();
			if (eexp > 0)
			{
				s = s + "(+" + eexp.ToString() + ")";
			}
			m_text1.GetComponent<UILabel> ().text = s;
			if (testify > 0)
			{
				mario._instance.m_self.testify = testify;
				m_text2.GetComponent<UILabel>().text = string.Format(game_data._instance.get_language_string("clear_gui_hdzm"), testify);
			}
			else if (rank == 0)
			{
				m_text2.GetComponent<UILabel>().text = game_data._instance.get_language_string("clear_gui_whdpm");
			}
			else
			{
				m_text2.GetComponent<UILabel>().text = string.Format(game_data._instance.get_language_string("clear_gui_hdpm"), rank);
			}
			m_touxiang.GetComponent<UISprite> ().spriteName = game_data._instance.get_t_touxiang (mario._instance.m_self.per_user_head);
			m_name.GetComponent<UILabel> ().text = mario._instance.m_self.per_user_name;
			m_gq.GetComponent<UISprite> ().spriteName = game_data._instance.get_t_guojia (mario._instance.m_self.per_user_country);
			m_map_name.GetComponent<UILabel> ().text = mario._instance.m_self.per_map_name;
			m_l2.SetActive(true);
			m_l4.SetActive(false);
		}
		else
		{
			m_touxiang1.GetComponent<UISprite> ().spriteName = game_data._instance.get_t_touxiang (mario._instance.m_self.per_user_head);
			m_name1.GetComponent<UILabel> ().text = mario._instance.m_self.per_user_name;
			m_gq1.GetComponent<UISprite> ().spriteName = game_data._instance.get_t_guojia (mario._instance.m_self.per_user_country);
			m_map_name1.GetComponent<UILabel> ().text = mario._instance.m_self.per_map_name;
			m_l2.SetActive(false);
			m_l4.SetActive(true);
		}
	}

	void FixedUpdate () {
		if (m_index < 40 && m_index % 2 == 0)
		{
			int n = m_index / 2;
			{
				GameObject obj = (GameObject)Instantiate(m_sub);
				obj.transform.parent = m_panel.transform;
				obj.transform.localPosition = new Vector3 ((n - 10) * 60, 445, 1);
				obj.transform.localScale = new Vector3 (1, 1, 1);
				obj.GetComponent<clear_gui_sub>().reset (270);
				obj.SetActive(true);
			}

			{
				GameObject obj = (GameObject)Instantiate(m_sub);
				obj.transform.parent = m_panel.transform;
				obj.transform.localPosition = new Vector3 ((n - 10) * 60, -445, 1);
				obj.transform.localScale = new Vector3 (1, 1, 1);
				obj.GetComponent<clear_gui_sub>().reset (-270);
				obj.SetActive(true);
			}
		}
		m_index++;
		if (m_time == 170)
		{
			if (m_next)
			{
				this.GetComponent<Animator> ().enabled = false;
				m_map_clear.SetActive(false);
				if (m_type == 0)
				{
					m_next_panel.SetActive(true);
				}
				else
				{
					m_next_panel1.SetActive(true);
				}
			}
			else
			{
				if (m_type == 0)
				{
					this.GetComponent<Animator> ().enabled = false;
					m_map_clear.SetActive(false);
					m_next_panel_def.SetActive(true);
				}
				else
				{
					mario._instance.change_state(e_game_state.egs_br_end, 1, delegate() { this.gameObject.SetActive(false); });
				}
			}
		}

		m_time++;
	}

	void click(GameObject obj)
	{
		if (obj.name == "ok")
		{
			if (m_type == 0)
			{
				protocol.game.cmsg_play_map msg = new protocol.game.cmsg_play_map();
				msg.id = mario._instance.m_self.mapid;
				net_http._instance.send_msg<protocol.game.cmsg_play_map>(opclient_t.OPCODE_PLAY_MAP, msg);
			}
			else
			{
				mario._instance.m_start_type = 2;
				mario._instance.change_state(e_game_state.egs_br_road, 1, delegate() { this.gameObject.SetActive(false); });
			}
		}
		if (obj.name == "cancel")
		{
			mario._instance.change_state(e_game_state.egs_play_select, 1, delegate() { this.gameObject.SetActive(false); });
		}
		if (obj.name == "exit")
		{
			mario._instance.change_state(e_game_state.egs_play_select, 1, delegate() { this.gameObject.SetActive(false); });
		}
		if (obj.name == "like")
		{
			protocol.game.cmsg_map_like msg = new protocol.game.cmsg_map_like();
			net_http._instance.send_msg<protocol.game.cmsg_map_like>(opclient_t.OPCODE_MAP_LIKE, msg);
		}
	}

	public void message (s_message message)
	{
	}

	public void net_message (s_net_message message)
	{
		if (message.m_opcode == opclient_t.OPCODE_MAP_LIKE)
		{
			m_like.SetActive(false);
			m_ydz.SetActive(true);
		}
	}
}

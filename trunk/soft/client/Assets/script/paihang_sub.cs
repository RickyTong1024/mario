using UnityEngine;
using System.Collections;

public class paihang_sub : MonoBehaviour {

	public GameObject m_rank_icon;
	public GameObject m_rank;
	public GameObject m_name;
	public GameObject m_level;
	public GameObject m_level_text;
	public GameObject m_time;
	public GameObject m_guojia;
	public protocol.game.map_point_rank m_msg;
	private int m_map_id;

	public void reset(int rank, int map_id, protocol.game.map_point_rank msg)
	{
		m_map_id = map_id;
		m_msg = msg;
		m_rank.GetComponent<UILabel> ().text = rank.ToString ();
		m_name.GetComponent<UILabel> ().text = "[u]" + player.get_name(m_msg.user_id, m_msg.player_name, m_msg.visitor);
		s_t_exp t_exp = game_data._instance.get_t_exp (m_msg.player_level);
		m_level.GetComponent<UISprite> ().spriteName = t_exp.icon;
		m_level_text.GetComponent<UILabel>().text = m_msg.player_level.ToString();
		m_time.GetComponent<UILabel> ().text = timer.get_game_time (m_msg.player_point);
		m_guojia.GetComponent<UISprite> ().spriteName = game_data._instance.get_t_guojia (m_msg.player_country);
		if (rank == 1)
		{
			m_rank_icon.GetComponent<UISprite>().spriteName = "hz_01";
		}
		else if (rank == 2)
		{
			m_rank_icon.GetComponent<UISprite>().spriteName = "hz_02";
		}
		else if (rank == 3)
		{
			m_rank_icon.GetComponent<UISprite>().spriteName = "hz_03";
		}
		else
		{
			m_rank_icon.GetComponent<UISprite>().spriteName = "";
		}
		if (m_msg.user_id == mario._instance.m_self.userid)
		{
			this.GetComponent<UISprite>().spriteName = "phb_list_frame001";
		}
	}

	void click(GameObject obj)
	{
		if (obj.name == "hf")
		{
			protocol.game.cmsg_view_video msg = new protocol.game.cmsg_view_video();
			msg.map_id = m_map_id;
			msg.video_id = m_msg.video_id;
			net_http._instance.send_msg<protocol.game.cmsg_view_video>(opclient_t.OPCODE_VIEW_VIDEO, msg);
		}
		if (obj.name == "name")
		{
			s_message mes = new s_message();
			mes.m_type = "ph_look_player";
			mes.m_ints.Add(m_msg.user_id);
			cmessage_center._instance.add_message(mes);
		}
	}
}

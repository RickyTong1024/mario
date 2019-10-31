using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class user_gui : MonoBehaviour {

	public GameObject m_name;
	public GameObject m_level;
	public GameObject m_level_text;
	public GameObject m_exp;
	public GameObject m_exp_text;
	public GameObject m_zm;
	public GameObject m_touxiang;
	public GameObject m_guoqi;
	public GameObject m_jewel;
	public GameObject m_close;

	void OnEnable () {
		InvokeRepeating ("time", 0, 1.0f);
		reset_touxiang ();
	}

	// Update is called once per frame
	void OnDisable () {
		CancelInvoke("time");
	}

	public void change_event(GameObject obj)
	{
		m_close.GetComponent<UIButtonMessage> ().target = obj;
	}

	void click(GameObject obj)
	{
		if (obj.name == "life_add")
		{
			mario._instance.show_shop_gui(1);
		}
		if (obj.name == "jewel_add")
		{
			mario._instance.show_shop_gui(0);
		}
		if (obj.name == "user" && mario._instance.m_game_state == e_game_state.egs_play_select)
		{
			protocol.game.cmsg_view_player msg = new protocol.game.cmsg_view_player();
			msg.userid = mario._instance.m_self.userid;
			net_http._instance.send_msg<protocol.game.cmsg_view_player>(opclient_t.OPCODE_VIEW_PLAYER, msg);
		}
	}

	void reset_touxiang()
	{
		m_name.GetComponent<UILabel>().text = mario._instance.m_self.get_name();
		m_touxiang.GetComponent<UISprite> ().spriteName = game_data._instance.get_t_touxiang (mario._instance.m_self.head);
		m_guoqi.GetComponent<UISprite> ().spriteName = game_data._instance.get_t_guojia (mario._instance.m_self.nationality);
		s_t_exp t_exp = game_data._instance.get_t_exp (mario._instance.m_self.level);
		m_level.GetComponent<UISprite>().spriteName = t_exp.icon;
		m_level_text.GetComponent<UILabel>().text = mario._instance.m_self.level.ToString();
		t_exp = game_data._instance.get_t_exp (mario._instance.m_self.level + 1);
		if (t_exp != null)
		{
			float v = (float)mario._instance.m_self.exp / t_exp.exp;
			m_exp.GetComponent<UIProgressBar>().value = v;
			m_exp_text.GetComponent<UILabel>().text = mario._instance.m_self.exp.ToString() + "/" + t_exp.exp.ToString();
		}
		else
		{
			m_exp.GetComponent<UIProgressBar>().value = 1;
			m_exp_text.GetComponent<UILabel>().text = mario._instance.m_self.exp.ToString() + "/--";
		}
		m_jewel.GetComponent<UILabel>().text = mario._instance.m_self.jewel.ToString();
		m_zm.GetComponent<UISprite>().spriteName = "wjtx_jb0" + mario._instance.m_self.testify.ToString();
	}

	void time()
	{
		reset_touxiang ();
	}
}

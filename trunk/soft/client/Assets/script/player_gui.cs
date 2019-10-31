using UnityEngine;
using System.Collections;

public class player_gui : MonoBehaviour {

	public GameObject m_view;
	public GameObject m_touxiang;
	public GameObject m_name;
	public GameObject m_guojia;
	public GameObject m_level;
	public GameObject m_level_text;
	public GameObject m_exp_bar;
	public GameObject m_exp_text;
	public GameObject m_job_level;
	public GameObject m_job_exp_bar;
	public GameObject m_job_exp_text;
	public GameObject m_time;
	public GameObject m_ztz;
	public GameObject m_ztg;
	public GameObject m_zfs;
	public GameObject m_zpl;
	public GameObject m_gk;
	public GameObject m_bgk;
	public GameObject m_jqwg_s;
	public GameObject m_jqwg_t;
	public GameObject m_tscd_s;
	public GameObject m_tscd_t;
	public GameObject m_wgzd_s;
	public GameObject m_wgzd_t;
	private protocol.game.smsg_view_player m_msg;
	public GameObject m_player_sub;

	public void reset (protocol.game.smsg_view_player msg) {
		m_msg = msg;
		m_name.GetComponent<UILabel>().text = player.get_name(m_msg.data.userid, m_msg.data.name, m_msg.data.visitor);
		m_touxiang.GetComponent<UISprite> ().spriteName = game_data._instance.get_t_touxiang (m_msg.data.head);
		m_guojia.GetComponent<UISprite> ().spriteName = game_data._instance.get_t_guojia (m_msg.data.country);
		s_t_exp t_exp = game_data._instance.get_t_exp (m_msg.data.level);
		m_level.GetComponent<UISprite>().spriteName = t_exp.icon;
		m_level_text.GetComponent<UILabel>().text = m_msg.data.level.ToString();
		t_exp = game_data._instance.get_t_exp (m_msg.data.level + 1);
		if (t_exp != null)
		{
			float v = (float)m_msg.data.exp / t_exp.exp;
			m_exp_bar.GetComponent<UIProgressBar>().value = v;
			m_exp_text.GetComponent<UILabel>().text = m_msg.data.exp.ToString() + "/" + t_exp.exp.ToString();
		}
		else
		{
			m_exp_bar.GetComponent<UIProgressBar>().value = 1;
			m_exp_text.GetComponent<UILabel>().text = m_msg.data.exp.ToString() + "/--";
		}
		m_job_level.GetComponent<UILabel>().text = "Lv" + m_msg.data.mlevel.ToString();
		s_t_job_exp t_job_exp = game_data._instance.get_t_job_exp (m_msg.data.mlevel + 1);
		if (t_job_exp != null)
		{
			float v = (float)m_msg.data.mexp / t_job_exp.exp;
			m_job_exp_bar.GetComponent<UIProgressBar>().value = v;
			m_job_exp_text.GetComponent<UILabel>().text = m_msg.data.mexp.ToString() + "/" + t_job_exp.exp.ToString();
		}
		else
		{
			m_job_exp_bar.GetComponent<UIProgressBar>().value = 1;
			m_job_exp_text.GetComponent<UILabel>().text = m_msg.data.mexp.ToString() + "/--";
		}
		m_time.GetComponent<UILabel>().text = m_msg.data.register;
		m_ztz.GetComponent<UILabel> ().text = m_msg.data.amount.ToString ("N0");
		m_ztg.GetComponent<UILabel>().text = m_msg.data.pas.ToString("N0");
		m_zfs.GetComponent<UILabel>().text = m_msg.data.point.ToString("N0");
		m_zpl.GetComponent<UILabel>().text = m_msg.data.comment.ToString("N0");
		m_gk.GetComponent<UILabel>().text = m_msg.data.video.ToString("N0");
		m_bgk.GetComponent<UILabel>().text = m_msg.data.watched.ToString("N0");

		mario._instance.remove_child (m_jqwg_s);
		if (msg.recent.Count == 0)
		{
			m_jqwg_t.SetActive(true);
			m_jqwg_s.GetComponent<UISprite>().height = 140;
		}
		else
		{
			m_jqwg_t.SetActive(false);
			for (int i = 0; i < msg.recent.Count; ++i)
			{
				GameObject obj = (GameObject)Instantiate(m_player_sub);
				obj.transform.parent = m_jqwg_s.transform;
				obj.transform.localPosition = new Vector3(0, -53 - 95 * i,0);
				obj.transform.localScale = new Vector3(1,1,1);
				obj.GetComponent<player_sub>().reset(1, msg.recent[i].id, msg.recent[i].name, msg.recent[i].url, msg.recent[i].rank, msg.recent[i].time, this.gameObject);
				obj.SetActive(true);
			}
			m_jqwg_s.GetComponent<UISprite>().height = 95 * msg.recent.Count + 10;
		}

		mario._instance.remove_child (m_tscd_s);
		if (msg.upload.Count == 0)
		{
			m_tscd_t.SetActive(true);
			m_tscd_s.GetComponent<UISprite>().height = 140;
		}
		else
		{
			m_tscd_t.SetActive(false);
			for (int i = 0; i < msg.upload.Count; ++i)
			{
				GameObject obj = (GameObject)Instantiate(m_player_sub);
				obj.transform.parent = m_tscd_s.transform;
				obj.transform.localPosition = new Vector3(0, -53 - 95 * i,0);
				obj.transform.localScale = new Vector3(1,1,1);
				obj.GetComponent<player_sub>().reset(2, msg.upload[i].id, msg.upload[i].name, msg.upload[i].url, 0, msg.upload[i].time, this.gameObject);
				obj.SetActive(true);
			}
			m_tscd_s.GetComponent<UISprite>().height = 95 * msg.upload.Count + 10;
		}

		mario._instance.remove_child (m_wgzd_s);
		if (msg.play.Count == 0)
		{
			m_wgzd_t.SetActive(true);
			m_wgzd_s.GetComponent<UISprite>().height = 140;
		}
		else
		{
			m_wgzd_t.SetActive(false);
			for (int i = 0; i < msg.play.Count; ++i)
			{
				GameObject obj = (GameObject)Instantiate(m_player_sub);
				obj.transform.parent = m_wgzd_s.transform;
				obj.transform.localPosition = new Vector3(0, -53 - 95 * i,0);
				obj.transform.localScale = new Vector3(1,1,1);
				obj.GetComponent<player_sub>().reset(3, msg.play[i].id, msg.play[i].name, msg.play[i].url, msg.play[i].play, "", this.gameObject);
				obj.SetActive(true);
			}
			m_wgzd_s.GetComponent<UISprite>().height = 95 * msg.play.Count + 10;
		}
		m_view.GetComponent<UIScrollView> ().ResetPosition ();
	}

	void click(GameObject obj)
	{
		if (obj.name == "close")
		{
			this.GetComponent<ui_show_anim>().hide_ui();
		}
	}
}

using UnityEngine;
using System.Collections;

public class br_end_gui : MonoBehaviour {

	public GameObject m_win;
	public GameObject m_lose;
	public GameObject m_z1;
	public GameObject m_z2;
	public GameObject m_main;
	public GameObject m_panel;
	public GameObject m_zz_panel;
	public GameObject m_exp;
	public GameObject m_ac_list;
	public GameObject m_thank;
	private float m_y;
	private float m_max_y;
	private int m_wait_time = 0;
	private int m_state = 0;

	void OnEnable()
	{
		if (mario._instance.m_self.m_finish_type == 0)
		{
			m_win.SetActive(true);
			m_lose.SetActive(false);
			m_z1.GetComponent<sprite_anim>().play("die");
			m_z2.GetComponent<sprite_anim>().play("die");
		}
		else
		{
			m_win.SetActive(false);
			m_lose.SetActive(true);
			m_main.GetComponent<sprite_anim>().play("die");
		}

		protocol.game.smsg_mission_finish msg = mario._instance.m_self.m_finish;
		m_exp.GetComponent<UILabel> ().text = "+" + msg.exp.ToString ();
		mario._instance.m_self.add_exp (msg.exp);
		mario._instance.remove_child (m_zz_panel);
		for (int i = 0; i < msg.authors.Count; ++i)
		{
			GameObject obj = (GameObject)Instantiate(m_ac_list);
			obj.transform.parent = m_zz_panel.transform;
			obj.transform.localPosition = new Vector3(0, -320 - 120 * i, 0);
			obj.transform.localScale = new Vector3(1,1,1);
			obj.GetComponent<archor_list>().reset(msg.authors[i].user_head, msg.authors[i].user_country, msg.authors[i].user_name, msg.authors[i].map_name);
			obj.SetActive(true);
		}
		m_panel.transform.localPosition = new Vector3 (0, 0, 0);
		m_y = 0;
		m_max_y = 590 + msg.authors.Count * 120 - 20;
		m_thank.transform.localPosition = new Vector3 (0, -m_max_y, 0);
		m_wait_time = 100;
		m_state = 0;
	}

	void FixedUpdate()
	{
		if (m_wait_time > 0)
		{
			m_wait_time--;
			if (m_wait_time == 0)
			{
				if (m_state == 0)
				{
					m_state = 1;
				}
				else if (m_state == 2)
				{
					mario._instance.change_state(e_game_state.egs_play_select, 1, delegate() { this.gameObject.SetActive(false); });
				}
			}
		}
		if (m_state == 1)
		{
			m_y += 2;
			if (m_y > m_max_y)
			{
				m_y = m_max_y;
				m_state = 2;
				m_wait_time = 100;
			}
			m_panel.transform.localPosition = new Vector3 (0, m_y, 0);
		}
	}
}

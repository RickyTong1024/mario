using UnityEngine;
using System.Collections;

public class fg_sub : MonoBehaviour {

	public GameObject m_text;
	public GameObject m_lock;
	public GameObject m_name;
	public GameObject m_gou;
	public int m_id;
	public bool m_lk;

	// Update is called once per frame
	public void reset (int id)
	{
		m_id = id;
		set_gou (false);
		s_t_fg t_fg = game_data._instance.get_t_fg (id);
		Texture2D bt = Resources.Load("texture/back/back_" + id.ToString()) as Texture2D;
		this.GetComponent<UITexture> ().mainTexture = bt;
		m_name.GetComponent<UILabel>().text = t_fg.name;
		if (t_fg.tj > mario._instance.m_self.job_level)
		{
			m_lk = true;
			m_lock.SetActive(true);
			m_text.GetComponent<UILabel>().text = t_fg.desc;
		}
		else
		{
			m_lk = false;
			m_lock.SetActive(false);
		}
	}

	public void set_gou(bool flag)
	{
		m_gou.SetActive(flag);
	}
}

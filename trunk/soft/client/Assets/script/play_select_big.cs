﻿using UnityEngine;
using System.Collections;

public class play_select_big : MonoBehaviour {

	public GameObject m_icon;
	public GameObject m_title;
	public GameObject m_new;
	public GameObject m_shou;
	public int m_type;
	public int m_clevel;
	
	public void reset(s_t_view_title t_view_title)
	{
		m_type = t_view_title.id;
		m_icon.GetComponent<UISprite> ().spriteName = t_view_title.icon;
		m_title.GetComponent<UILabel> ().text = t_view_title.name;
		if (mario._instance.m_self.guide == 200 && m_type == 200)
		{
			m_shou.SetActive (true);
		}
		else
		{
			m_shou.SetActive (false);
		}
	}

	public void hide_shou()
	{
		m_shou.SetActive (false);
	}

	void Update()
	{
		if (m_type >= 200)
		{
			s_t_exp t_exp = game_data._instance.get_t_exp(mario._instance.m_self.level);
			if (t_exp.zm > mario._instance.m_self.testify)
			{
				if (!m_new.activeSelf)
				{
					m_new.SetActive(true);
				}
			}
			else
			{
				if (m_new.activeSelf)
				{
					m_new.SetActive(false);
				}
			}
		}
		else if (m_type == 2)
		{
			if (mario._instance.m_self.br_start != 2)
			{
				if (!m_new.activeSelf)
				{
					m_new.SetActive(true);
				}
			}
			else
			{
				if (m_new.activeSelf)
				{
					m_new.SetActive(false);
				}
			}
		}
	}
}

﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class login_sys_gui : MonoBehaviour, IMessage {

	public GameObject m_bgm_open;
	public GameObject m_bgm_close;
	public GameObject m_sound_open;
	public GameObject m_sound_close;
	public GameObject m_full_open;
	public GameObject m_full_close;
	public List<GameObject> m_changes;
	private int m_index = -1;
	public GameObject m_fbl;

	// Use this for initialization
	void Start () {
		cmessage_center._instance.add_handle (this);
	}

	void OnDestroy()
	{
		cmessage_center._instance.remove_handle (this);
	}

	void OnEnable()
	{
		if (game_data._instance.m_save_data.is_bgm == 1)
		{
			m_bgm_open.SetActive(true);
			m_bgm_close.SetActive(false);
		}
		else
		{
			m_bgm_open.SetActive(false);
			m_bgm_close.SetActive(true);
		}
		if (game_data._instance.m_save_data.is_sound == 1)
		{
			m_sound_open.SetActive(true);
			m_sound_close.SetActive(false);
		}
		else
		{
			m_sound_open.SetActive(false);
			m_sound_close.SetActive(true);
		}
		if (game_data._instance.m_save_data.is_full == 1)
		{
			m_full_open.SetActive(true);
			m_full_close.SetActive(false);
		}
		else
		{
			m_full_open.SetActive(false);
			m_full_close.SetActive(true);
		}
		if (game_data._instance.m_save_data.fbl == 0)
		{
			m_fbl.GetComponent<UILabel>().text = "3:2";
		}
		else
		{
			m_fbl.GetComponent<UILabel>().text = "16:9";
		}
		m_index = -1;
		for (int i = 0; i < m_changes.Count; ++i)
		{
			m_changes[i].transform.FindChild("kuang").gameObject.SetActive(false);
			s_t_key t_key = game_data._instance.get_t_key(game_data._instance.m_save_data.keys[i]);
			if (t_key != null)
			{
				m_changes[i].GetComponent<UILabel>().text = t_key.name;
			}
			else
			{
				m_changes[i].GetComponent<UILabel>().text = "";
			}
		}
	}

	void click(GameObject obj)
	{
		if (obj.name == "bopen")
		{
			m_bgm_open.SetActive(false);
			m_bgm_close.SetActive(true);
			game_data._instance.m_save_data.is_bgm = 0;
			game_data._instance.save_native();
		}
		else if (obj.name == "bclose")
		{
			m_bgm_open.SetActive(true);
			m_bgm_close.SetActive(false);
			game_data._instance.m_save_data.is_bgm = 1;
			game_data._instance.save_native();
		}
		else if (obj.name == "sopen")
		{
			m_sound_open.SetActive(false);
			m_sound_close.SetActive(true);
			game_data._instance.m_save_data.is_sound = 0;
			game_data._instance.save_native();
		}
		else if (obj.name == "sclose")
		{
			m_sound_open.SetActive(true);
			m_sound_close.SetActive(false);
			game_data._instance.m_save_data.is_sound = 1;
			game_data._instance.save_native();
		}
		else if (obj.name == "fopen")
		{
			m_full_open.SetActive(false);
			m_full_close.SetActive(true);
			game_data._instance.m_save_data.is_full = 0;
			game_data._instance.save_native();
		}
		else if (obj.name == "fclose")
		{
			m_full_open.SetActive(true);
			m_full_close.SetActive(false);
			game_data._instance.m_save_data.is_full = 1;
			game_data._instance.save_native();
		}
		else if (obj.name == "quit")
		{
			Application.Quit();
		}
	}

	public void change_ff()
	{
		if (m_fbl.GetComponent<UILabel>().text == "3:2")
		{
			game_data._instance.m_save_data.fbl = 0;
		}
		else
		{
			game_data._instance.m_save_data.fbl = 1;
		}
		game_data._instance.save_native();
	}

	void click_aj(GameObject obj)
	{
		m_index = int.Parse(obj.transform.parent.name);
		for (int i = 0; i < m_changes.Count; ++i)
		{
			m_changes[i].transform.FindChild("kuang").gameObject.SetActive(false);
		}
		m_changes[m_index].transform.FindChild("kuang").gameObject.SetActive(true);
	}

	public void message (s_message message)
	{
	}

	public void net_message (s_net_message message)
	{

	}

	void Update()
	{
		if (m_index != -1)
		{
			if (Input.anyKeyDown)
			{
				int kc = 0;
				foreach (int k in game_data._instance.m_t_key.Keys)
				{
					if (Input.GetKeyDown((KeyCode)k))
					{
						kc = k;
					}
				}
				if (kc != 0)
				{
					if (kc == (int)KeyCode.Escape)
					{
						m_changes[m_index].GetComponent<UILabel>().text = "";
						game_data._instance.m_save_data.keys[m_index] = 0;
						game_data._instance.save_native();
					}
					else
					{
						m_changes[m_index].GetComponent<UILabel>().text = game_data._instance.m_t_key[kc].name;
						game_data._instance.m_save_data.keys[m_index] = kc;
						game_data._instance.save_native();
					}
				}
				m_changes[m_index].transform.FindChild("kuang").gameObject.SetActive(false);
				m_index = -1;
			}
		}
	}
}

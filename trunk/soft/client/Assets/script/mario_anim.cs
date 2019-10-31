using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

[Serializable]
public class mario_anim_sub
{
	public string name;
	public SpriteRenderer render;
	public List<Sprite> ss;
	public bool defualt;
	public int speed;
	public bool once;
	public string link;
}

public class mario_anim : MonoBehaviour {

	public List<mario_anim_sub> m_subs;
	private int m_play_index = -1;
	private Sprite m_s = null;
	private string m_name = "";
	private int m_time;
	private int m_z;
	private bool m_pause = false;

	public bool has_anim(string name)
	{
		for (int i = 0; i < m_subs.Count; ++i)
		{
			if (m_subs[i].name == name)
			{
				return true;
			}
		}
		return false;
	}

	public string get_name()
	{
		return m_name;
	}

	public bool do_defualt()
	{
		int index = -1;
		for (int i = 0; i < m_subs.Count; ++i)
		{
			if (m_subs[i].defualt)
			{
				index = i;
				break;
			}
		}
		if (index == -1)
		{
			return false;
		}
		play_index(index);
		return true;
	}

	public void play(string name, int speed = -1)
	{
		int index = -1;
		for (int i = 0; i < m_subs.Count; ++i)
		{
			if (m_subs[i].name == name)
			{
				index = i;
				break;
			}
		}
		if (index == -1)
		{
			return;
		}
		play_index (index);
		if (speed != -1)
		{
			m_subs [m_play_index].speed = speed;
		}
	}

	public void play_index(int index)
	{
		if (index < 0 || index >= m_subs.Count)
		{
			return;
		}
		if (index == m_play_index)
		{
			return;
		}
		if (m_play_index != -1)
		{
			m_subs[m_play_index].render.sprite = m_s;
		}

		m_play_index = index;
		m_s = m_subs [m_play_index].render.sprite;
		m_subs[m_play_index].render.sprite = m_subs[m_play_index].ss[0];
		m_time = 0;
		m_z = 0;
		m_name = m_subs [m_play_index].name;
	}

	public void pause()
	{
		m_pause = true;
	}

	public void conti()
	{
		m_pause = false;
	}

	public Sprite get_sprite()
	{
		return m_subs [m_play_index].render.sprite;
	}

	void FixedUpdate ()
	{
		if (m_play_index == -1 || m_pause)
		{
			return;
		}
		int s = m_subs [m_play_index].speed;
		m_time += s;
		int i1 = m_z;
		m_z += m_time / 50;
		bool end = false;
		if (m_subs[m_play_index].once)
		{
			if (m_z > m_subs[m_play_index].ss.Count - 1)
			{
				end = true;
				m_z = m_subs[m_play_index].ss.Count - 1;
			}
		}
		else
		{
			m_z = m_z % m_subs[m_play_index].ss.Count;
		}

		if (i1 != m_z)
		{
			m_time = m_time % 50;
			m_subs[m_play_index].render.sprite = m_subs[m_play_index].ss[m_z];
		}

		if (end && m_subs[m_play_index].link != "")
		{
			play(m_subs[m_play_index].link);
		}
	}
}

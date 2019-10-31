using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

[Serializable]
public class sprite_anim_sub
{
	public string name;
	public UISprite render;
	public List<string> ss;
	public int speed;
	public bool once;
	public string link;
}

public class sprite_anim : MonoBehaviour {

	public List<sprite_anim_sub> m_subs;
	private int m_play_index = -1;
	private string m_s = "";
	private string m_name = "";
	private int m_time;
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
	
	public void play(string name)
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
			m_subs[m_play_index].render.spriteName = m_s;
		}
		
		m_play_index = index;
		m_s = m_subs [m_play_index].render.spriteName;
		m_subs[m_play_index].render.spriteName = m_subs[m_play_index].ss[0];
		m_time = 0;
		m_name = m_subs [m_play_index].render.name;
	}
	
	public void pause()
	{
		m_pause = true;
	}
	
	public void conti()
	{
		m_pause = false;
	}
	
	void FixedUpdate ()
	{
		if (m_play_index == -1 || m_pause)
		{
			return;
		}
		int s = m_subs [m_play_index].speed;
		int i1 = m_time * s / 50;
		m_time += 1;
		int i2 = m_time * s / 50;
		bool end = false;
		if (m_subs[m_play_index].once)
		{
			if (i1 > m_subs[m_play_index].ss.Count - 1)
			{
				i1 = m_subs[m_play_index].ss.Count - 1;
			}
			if (i2 > m_subs[m_play_index].ss.Count - 1)
			{
				end = true;
				i2 = m_subs[m_play_index].ss.Count - 1;
			}
		}
		else
		{
			i1 = i1 % m_subs[m_play_index].ss.Count;
			i2 = i2 % m_subs[m_play_index].ss.Count;
		}
		
		if (i1 != i2)
		{
			m_subs[m_play_index].render.spriteName = m_subs[m_play_index].ss[i2];
		}
		
		if (end && m_subs[m_play_index].link != "")
		{
			play(m_subs[m_play_index].link);
		}
	}
}

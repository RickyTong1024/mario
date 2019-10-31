﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class dbc
{
	private int m_x_num = 0;
	private int	m_y_num = 0;
	private byte[] m_data;
	private List<int> m_records = new List<int>();

	public void load_txt(string name)
	{
		m_x_num = 0;
		m_y_num = 0;
		m_records.Clear ();

		{
			TextAsset _txt_data = Resources.Load("config/" + name) as TextAsset;
			if(_txt_data == null)
			{
				Debug.Log("err config__" + name); 
			}
			m_data = _txt_data.bytes;
		}

		for(int i = 0;i < m_data.Length;i ++)
		{
			if(m_data[i] == '\n')
			{
				m_y_num ++;
			}

			if((m_data[i] == '\n' || m_data[i] == '\t') && m_y_num > 1)
			{
				m_records.Add(i + 1);
			}
		}

		for (int i = 0;i < m_data.Length;i ++)
		{
			if(m_data[i] == '\t')
			{
				m_x_num ++;
			}

			else if(m_data[i] == '\n')
			{
				break;
			}
		}

		m_x_num ++;
		m_y_num -= 2;
	}

	public string get(int x, int y)
	{
		int _num = 0;
		int _id = y * m_x_num + x;

		if(_id >= m_records.Count) return "";
		for (int i = m_records [y * m_x_num + x];m_data[i] != '\t' && m_data[i] != '\n' && m_data[i] != '\r';i ++)
		{
			_num ++;
		}
		_num = 0;
		for (int i = m_records [y * m_x_num + x];m_data[i] != '\t' && m_data[i] != '\n' && m_data[i] != '\r';i ++)
		{
			_num ++;
		}
		string _out = System.Text.Encoding.UTF8.GetString (m_data, m_records [y * m_x_num + x],_num);
		if(_out.Length == 0)
		{
			return "0";
		}
		_out = _out.Replace ("{nn}", "\n");
		return _out;
	}

	public int get_x()
	{
		return m_x_num;
	}

	public int get_y()
	{
		return m_y_num;
	}
}
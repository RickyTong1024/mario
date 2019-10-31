using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class play_mask_gui : MonoBehaviour {

	public GameObject m_sub;
	public GameObject m_panel;
	private List<GameObject> m_objs = new List<GameObject>();
	private float m_time = 0;
	private int m_type = 0;
	private mario.ChangeStateHandle m_handle;

	public void reset (mario.ChangeStateHandle handle)
	{
		m_handle = handle;
		this.gameObject.SetActive (true);
		m_time = 0;
		m_type = 0;
		m_objs.Clear ();
		mario._instance.remove_child (m_panel);
		for (int j = 0; j < 4; ++j)
		{
			for (int i = 0; i < 8; ++i)
			{
				GameObject obj = (GameObject)Instantiate(m_sub);
				obj.transform.parent = m_panel.transform;
				obj.transform.localPosition = new Vector3(-560 + i * 160, -240 + j * 160,0);
				obj.transform.localScale = new Vector3(0,0,0);
				obj.SetActive(true);
				m_objs.Add(obj);
			}
		}
		for (int i = 0; i < m_objs.Count; ++i)
		{
			float t = (i / 8 + i % 8) * 0.05f + 1.0f;
			utils.add_scale_anim(m_objs[i], 0.2f, new Vector3(1, 1, 1), t);
		}
	}

	public void finish()
	{
		m_handle ();
	}
	
	// Update is called once per frame
	void Update () {
		m_time += Time.deltaTime;
		if (m_type == 0)
		{
			if (m_time > 1.8f)
			{
				m_type = 1;
				m_time = 0;
			}
		}
		if (m_type == 1)
		{
			if (m_time > 0.5f)
			{
				m_type = 2;
				m_time = 0;
				finish();
			}
		}
		if (m_type == 2)
		{
			if (m_time > 0.1f)
			{
				m_type = 3;
				m_time = 0;
				for (int i = 0; i < m_objs.Count; ++i)
				{
					float t = (i / 8 + i % 8) * 0.05f;
					utils.add_scale_anim(m_objs[i], 0.2f, new Vector3(0, 0, 0), t);
				}
			}
		}
		if (m_type == 3)
		{
			if (m_time > 0.8f)
			{
				m_type = 4;
				this.gameObject.SetActive(false);
			}
		}
	}
}

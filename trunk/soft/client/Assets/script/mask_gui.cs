using UnityEngine;
using System.Collections;

public class mask_gui : MonoBehaviour {

	private mario.ChangeStateHandle m_handle;
	private float m_next;
	private int m_type;
	private float m_x;
	private float m_y;
	private float m_ck;
	private float m_range;
	public GameObject m_t1; 
	public UITexture m_tex; 
	// Use this for initialization
	public void reset (mario.ChangeStateHandle handle, int type = 0, int x = 0, int y = 0)
	{
		m_type = type;
		m_handle = handle;
		this.gameObject.SetActive (true);

		if (m_type == 1)
		{
			m_t1.SetActive(true);
			m_x = (float)x / mario._instance.m_width + 0.5f;
			m_y = (float)y / mario._instance.m_height + 0.5f;
			m_range = 0.6f;
			m_tex.material.SetFloat("_range", m_range);
			m_tex.material.SetFloat("_x", m_x);
			m_tex.material.SetFloat("_y", m_y);
			m_tex.material.SetFloat("_ck", (float)mario._instance.m_width / mario._instance.m_height);
			this.GetComponent<UIPanel>().RebuildAllDrawCalls();
			this.GetComponent<Animator> ().Play ("mask_gui1");
		}
		else
		{
			m_t1.SetActive(false);
			this.GetComponent<Animator> ().Play ("mask_gui");
		}
	}

	void middle()
	{
		m_handle ();
		m_handle = null;
		if (m_type == 1)
		{
			m_t1.SetActive(false);
		}
	}

	void end()
	{
		if (m_handle == null)
		{
			this.gameObject.SetActive (false);
		}
	}

	void Update()
	{
		if (m_range > 0)
		{
			m_range -= Time.deltaTime* 1.2f;
			if (m_range <= 0)
			{
				m_range = 0;
			}
			m_tex.material.SetFloat("_range", m_range);
			this.GetComponent<UIPanel>().RebuildAllDrawCalls();
		}
	}
}

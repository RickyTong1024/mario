using UnityEngine;
using System.Collections;

public class wait_gui : MonoBehaviour {

	public GameObject m_normal;
	public GameObject m_sp;
	public GameObject m_text;
	public GameObject m_ltext;
	private int m_index;

	void OnEnable () {
		InvokeRepeating ("time", 0.5f, 0.5f);
	}
	
	// Update is called once per frame
	void OnDisable () {
		CancelInvoke("time");
	}

	public void reset(bool flag, string text)
	{
		if (!flag)
		{
			this.gameObject.SetActive(false);
		}
		else
		{
			this.gameObject.SetActive(true);
			if (text == "")
			{
				m_normal.SetActive(true);
				m_sp.SetActive(false);
			}
			else
			{
				m_sp.SetActive(true);
				m_normal.SetActive(false);
				m_text.GetComponent<UILabel>().text = text;
			}
		}
	}

	void time()
	{
		m_index = (m_index + 1) % 3;
		string s = "Loading";
		for (int i = 0; i < m_index; ++i)
		{
			s += ".";
		}
		m_ltext.GetComponent<UILabel>().text = s;
	}
}

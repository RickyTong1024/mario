using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class tip_gui_sub
{
	public GameObject tip;
	public float time;
	public float y;
}

public class tip_gui : MonoBehaviour {

	private List<tip_gui_sub> m_tips = new List<tip_gui_sub>();
	private int m_jd = 0;

	public void add_text(string text)
	{
		if (m_tips.Count >= 3)
		{
			Object.Destroy(m_tips[0].tip);
			m_tips.RemoveAt(0);
		}
		tip_gui_sub tgs = new tip_gui_sub ();
		GameObject res = Resources.Load("ui/tip_gui_sub") as GameObject;
		GameObject obj = (GameObject)Instantiate(res);
		obj.transform.FindChild ("text").GetComponent<UILabel> ().text = text;
		tgs.tip = obj;
		tgs.time = 0;
		obj.transform.parent = this.gameObject.transform;
		if (m_jd == 0)
		{
			obj.transform.localPosition = new Vector3(0,180,0);
			tgs.y = 180;
		}
		else if (m_jd == 1)
		{
			obj.transform.localPosition = new Vector3(0,140,0);
			tgs.y = 140;
		}
		else if (m_jd == 2)
		{
			obj.transform.localPosition = new Vector3(0,100,0);
			tgs.y = 140;
			if (m_tips.Count == 1)
			{
				m_tips[0].tip.transform.localPosition = new Vector3(0,140,0);
				m_tips[0].y = 180;
			}
			else if (m_tips.Count == 2)
			{
				m_tips[0].tip.transform.localPosition = new Vector3(0,180,0);
				m_tips[0].y = 220;
				m_tips[1].tip.transform.localPosition = new Vector3(0,140,0);
				m_tips[1].y = 180;
			}
		}
		obj.transform.localScale = new Vector3(1,1,1);
		m_tips.Add (tgs);

		if (m_jd < 2)
		{
			m_jd++;
		}
	}
	
	// Update is called once per frame
	void Update () {
		List<tip_gui_sub> tt = new List<tip_gui_sub>();
		for (int i = 0; i < m_tips.Count; ++i)
		{
			m_tips[i].time += Time.deltaTime;
			if (m_tips[i].time > 3)
			{
				tt.Add(m_tips[i]);
			}
			else if (m_tips[i].time >= 2)
			{
				float t = 3 - m_tips[i].time;
				m_tips[i].tip.GetComponent<UISprite>().alpha = t;
			}
			if (m_tips[i].tip.transform.localPosition.y < m_tips[i].y)
			{
				m_tips[i].tip.transform.localPosition = new Vector3(0, m_tips[i].tip.transform.localPosition.y + Time.deltaTime * 200, 0);
			}
			if (m_tips[i].tip.transform.localPosition.y >= m_tips[0].y)
			{
				m_tips[i].tip.transform.localPosition = new Vector3(0, m_tips[i].y, 0);
			}
		}
		for (int i = 0; i < tt.Count; ++i)
		{
			Object.Destroy(tt[i].tip);
			m_tips.Remove(tt[i]);
		}
	}
}

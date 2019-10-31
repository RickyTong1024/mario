using UnityEngine;
using System.Collections;

public class clear_gui_sub : MonoBehaviour {

	private float m_x;
	private float m_y;
	private float m_yy;
	private float m_time;

	public void reset(float y)
	{
		m_x = transform.localPosition.x;
		m_y = transform.localPosition.y;
		m_yy = y;
		m_time = 0;
	}
	
	// Update is called once per frame
	void Update () {
		if (m_time >= 0)
		{
			m_time += Time.deltaTime;
		}
		float t = m_time;
		if (m_time < 0)
		{
		}
		else if (m_time < 0.1f)
		{
			float y1 = m_y;
			float y2 = m_yy + (m_yy - m_y) * 0.2f;
			transform.localPosition = new Vector3(m_x, y1 + (y2 - y1) * (t / 0.1f), 0);
		}
		else if (m_time < 0.3f)
		{
			t = t - 0.1f;
			float y1 = m_yy + (m_yy - m_y) * 0.2f;
			float y2 = m_y + (m_yy - m_y) * 0.85f;
			transform.localPosition = new Vector3(m_x, y1 + (y2 - y1) * (t / 0.2f), 0);
		}
		else if (m_time < 0.5f)
		{
			t = t - 0.3f;
			float y1 = m_y + (m_yy - m_y) * 0.85f;
			float y2 = m_yy + (m_yy - m_y) * 0.1f;
			transform.localPosition = new Vector3(m_x, y1 + (y2 - y1) * (t / 0.2f), 0);
		}
		else if (m_time < 0.7f)
		{
			t = t - 0.5f;
			float y1 = m_yy + (m_yy - m_y) * 0.1f;
			float y2 = m_y + (m_yy - m_y) * 0.95f;
			transform.localPosition = new Vector3(m_x, y1 + (y2 - y1) * (t / 0.2f), 0);
		}
		else if (m_time < 0.9f)
		{
			t = t - 0.7f;
			float y1 = m_y + (m_yy - m_y) * 0.95f;
			float y2 = m_yy;
			transform.localPosition = new Vector3(m_x, y1 + (y2 - y1) * (t / 0.2f), 0);
		}
		else if (m_time > 0)
		{
			transform.localPosition = new Vector3(m_x, m_yy, 0);
			m_time = -1f;
		}
	}
}

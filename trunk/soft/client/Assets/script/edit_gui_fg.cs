using UnityEngine;
using System.Collections;

public class edit_gui_fg : MonoBehaviour {

	private int m_x;
	private int m_y;
	private int m_time = 0;

	public void reset(int x, int y)
	{
		m_x = x;
		m_y = y;
	}

	void FixedUpdate()
	{
		m_time++;
		if (m_time == (m_x - m_y) * 2 + 20)
		{
			this.GetComponent<TweenRotation>().enabled = true;
		}
		else if (m_time == (m_x - m_y) * 2 + 40)
		{
			Object.Destroy(this.gameObject);
		}
	}
}

using UnityEngine;
using System.Collections;

public class road_gui_sub : MonoBehaviour {

	private int m_state = 0;
	private float m_x = 0;
	private float m_y = 0;
	private float m_speed_x = 0;
	private float m_speed_y = 0;

	public void run_to(float x, float y)
	{
		m_state = 1;
		m_x = x;
		m_y = y;
		m_speed_x = 0;
		m_speed_y = 0;
		this.GetComponent<sprite_anim> ().play ("run");
	}

	void FixedUpdate()
	{
		if (m_state == 1)
		{
			float x = this.transform.localPosition.x;
			float y = this.transform.localPosition.y;
			float dx = m_x - x;
			float dy = m_y - y;
			if (dx > -0.0001f && dx < 0.0001f && dy > -0.0001f && dy < 0.0001f)
			{
				m_state = 0;
				this.GetComponent<sprite_anim> ().play ("stand");
				return;
			}
			float speed_x = dx / 5;
			float speed_y = dy / 5;
			if (dx < 0)
			{
				if (m_speed_x - 1.0f > speed_x)
				{
					m_speed_x -= 1.0f;
				}
				else
				{
					m_speed_x = speed_x;
				}
				if (m_speed_x > -1.0f)
				{
					m_speed_x = -1.0f;
				}
				else if (m_speed_x < -50.0f)
				{
					m_speed_x = -50.0f;
				}
				if (dx >= m_speed_x)
				{
					m_speed_x = dx;
				}
			}
			else if (dx > 0)
			{
				if (m_speed_x + 1.0f < speed_x)
				{
					m_speed_x += 1.0f;
				}
				else
				{
					m_speed_x = speed_x;
				}
				if (m_speed_x < 1.0f)
				{
					m_speed_x = 1.0f;
				}
				else if (m_speed_x > 50.0f)
				{
					m_speed_x = 50.0f;
				}
				if (dx <= m_speed_x)
				{
					m_speed_x = dx;
				}
			}
			else 
			{
				m_speed_x = 0;
			}
			if (dy < 0)
			{
				if (m_speed_y - 1.0f > speed_y)
				{
					m_speed_y -= 1.0f;
				}
				else
				{
					m_speed_y = speed_y;
				}
				if (m_speed_y > -1.0f)
				{
					m_speed_y = -1.0f;
				}
				else if (m_speed_y < -50.0f)
				{
					m_speed_y = -50.0f;
				}
				if (dy >= m_speed_y)
				{
					m_speed_y = dy;
				}
			}
			else if (dy > 0)
			{
				if (m_speed_y + 1.0f < speed_y)
				{
					m_speed_y += 1.0f;
				}
				else
				{
					m_speed_y = speed_y;
				}
				if (m_speed_y < 1.0f)
				{
					m_speed_y = 1.0f;
				}
				else if (m_speed_x > 50.0f)
				{
					m_speed_x = 50.0f;
				}
				if (dy <= m_speed_y)
				{
					m_speed_y = dy;
				}
			}
			else
			{
				m_speed_y = 0;
			}
			this.transform.localPosition = new Vector3(x + m_speed_x, y + m_speed_y, 0);
		}
	}
}

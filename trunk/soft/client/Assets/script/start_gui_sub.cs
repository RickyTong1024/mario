using UnityEngine;
using System.Collections;

public class start_gui_sub : MonoBehaviour {

	private int m_state = 0;
	private float m_x = 0;
	private float m_y = 0;
	private float m_speed_x = 0;
	private float m_speed_y = 0;
	private int m_wait = 0;
	private int m_time = 0;
	public bool m_sound = false;

	public int run_to(float x, float y, bool right, float scale = 1.0f, bool wait = false, int wait_time = 0)
	{
		m_state = 1;
		m_time = 0;
		m_x = x;
		m_y = y;
		m_speed_x = 0;
		m_speed_y = 0;
		this.GetComponent<sprite_anim> ().play ("run");
		if (right)
		{
			this.transform.localScale = new Vector3(scale, scale, 1);
		}
		else
		{
			this.transform.localScale = new Vector3(-scale, scale, 1);
		}
		if (wait)
		{
			m_wait = wait_time + Random.Range (3, 8);
		}
		return m_wait;
	}

	public void jump()
	{
		m_state = 2;
		m_time = 0;
		m_speed_y = 40;
		this.transform.localScale = new Vector3(1, 1, 1);
		this.GetComponent<sprite_anim> ().play ("jump");
		mario._instance.play_sound ("sound/jump");
	}

	void FixedUpdate()
	{
		if (m_wait > 0)
		{
			m_wait--;
		}
		else if (m_state == 2)
		{
			m_time++;
			m_speed_y -= 4f;
			this.transform.localPosition = new Vector3(this.transform.localPosition.x + 10.0f, this.transform.localPosition.y + m_speed_y, 0);
			this.transform.localScale = new Vector3(1 + m_time / 10.0f, 1 + m_time / 10.0f, 1);
			if (m_time >= 15)
			{
				this.GetComponent<sprite_anim> ().play ("stand");
				m_state = 0;
			}
		}
		else if (m_state == 1)
		{
			if (m_time % 50 == 0 && m_sound)
			{
				mario._instance.play_sound ("sound/step");
			}
			m_time++;
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

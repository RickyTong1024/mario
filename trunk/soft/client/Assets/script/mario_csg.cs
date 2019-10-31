using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class mario_csg : mario_obj {

	public List<Sprite> m_sprites;

	public override void reset ()
	{
		if (m_param[0] == 0 && m_param[1] == 0)
		{
			this.transform.FindChild("sprite").GetComponent<SpriteRenderer>().sprite = m_sprites[0];
			this.transform.FindChild("sprite").GetComponent<SpriteRenderer>().sortingOrder = 1;
		}
		else if (m_param[0] < m_init_pos.x && m_param[1] == m_init_pos.y)
		{
			this.transform.FindChild("sprite").GetComponent<SpriteRenderer>().sprite = m_sprites[1];
			this.transform.FindChild("sprite").GetComponent<SpriteRenderer>().sortingOrder = 0;
		}
		else if (m_param[0] > m_init_pos.x && m_param[1] == m_init_pos.y)
		{
			this.transform.FindChild("sprite").GetComponent<SpriteRenderer>().sprite = m_sprites[2];
			this.transform.FindChild("sprite").GetComponent<SpriteRenderer>().sortingOrder = 0;
		}
		else if (m_param[0] == m_init_pos.x && m_param[1] > m_init_pos.y)
		{
			this.transform.FindChild("sprite").GetComponent<SpriteRenderer>().sprite = m_sprites[3];
			this.transform.FindChild("sprite").GetComponent<SpriteRenderer>().sortingOrder = 0;
		}
		else if (m_param[0] == m_init_pos.x && m_param[1] < m_init_pos.y)
		{
			this.transform.FindChild("sprite").GetComponent<SpriteRenderer>().sprite = m_sprites[4];
			this.transform.FindChild("sprite").GetComponent<SpriteRenderer>().sortingOrder = 0;
		}
		else if (m_param[0] < m_init_pos.x && m_param[1] < m_init_pos.y)
		{
			this.transform.FindChild("sprite").GetComponent<SpriteRenderer>().sprite = m_sprites[5];
			this.transform.FindChild("sprite").GetComponent<SpriteRenderer>().sortingOrder = 0;
		}
		else if (m_param[0] > m_init_pos.x && m_param[1] < m_init_pos.y)
		{
			this.transform.FindChild("sprite").GetComponent<SpriteRenderer>().sprite = m_sprites[6];
			this.transform.FindChild("sprite").GetComponent<SpriteRenderer>().sortingOrder = 0;
		}
		else if (m_param[0] > m_init_pos.x && m_param[1] > m_init_pos.y)
		{
			this.transform.FindChild("sprite").GetComponent<SpriteRenderer>().sprite = m_sprites[7];
			this.transform.FindChild("sprite").GetComponent<SpriteRenderer>().sortingOrder = 0;
		}
		else if (m_param[0] < m_init_pos.x && m_param[1] > m_init_pos.y)
		{
			this.transform.FindChild("sprite").GetComponent<SpriteRenderer>().sprite = m_sprites[8];
			this.transform.FindChild("sprite").GetComponent<SpriteRenderer>().sortingOrder = 0;
		}
	}
}

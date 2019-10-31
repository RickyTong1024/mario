using UnityEngine;
using System.Collections;

public class mario_edit_cy : MonoBehaviour {

	[HideInInspector]
	public mario_point m_grid = new mario_point();

	public void reset(edit_cy ec)
	{
		m_grid.set (ec.p.x / utils.g_grid_size, ec.p.y / utils.g_grid_size);
		float a = 0.9f - ec.num * 0.015f;
		if (a < 0.5f)
		{
			a = 0.5f;
		}
		this.GetComponent<SpriteRenderer> ().color = new Color (1, 1, 1, a);
		this.GetComponent<SpriteRenderer> ().sprite = ec.sp;
	}
}

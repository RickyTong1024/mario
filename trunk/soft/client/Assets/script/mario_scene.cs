using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class mario_scene : MonoBehaviour {
	
	public List<GameObject> m_cengs;
	public List<int> m_moves;
	public List<int> m_lens;

	public void update_ex(mario_point roll)
	{
		for (int i = 0; i < m_cengs.Count; ++i)
		{
			int x = (roll.x / 10 / m_moves[i]) % m_lens[i];
			int y = roll.y / 10 / m_moves[i];
			m_cengs[i].transform.localPosition = new Vector3 (-x, -y);
		}
	}
}

using UnityEngine;
using System.Collections;

public class score : MonoBehaviour {

	public int m_time = 0;
	public GameObject m_text;

	public void reset(int s)
	{
		m_text.GetComponent<UILabel> ().text = s.ToString ();
	}

	public void update_ex()
	{
		m_time++;
	}
}

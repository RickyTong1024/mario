using UnityEngine;
using System.Collections;

public class play_select_gui_shou : MonoBehaviour {

	public GameObject m_shou;

	void OnEnable () {
		if (mario._instance.m_self.guide == 201)
		{
			m_shou.SetActive(true);
		}
		else
		{
			m_shou.SetActive(false);
		}
	}
}

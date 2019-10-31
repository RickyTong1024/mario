
using UnityEngine;
using System.Collections;

public class ui_show_anim : MonoBehaviour {

	public GameObject m_obj;

	void OnEnable()
	{
		show_ui ();
	}
	
	public void show_ui()
	{

	}

	public void hide_ui()
	{
		this.gameObject.SetActive(false);
	}
}

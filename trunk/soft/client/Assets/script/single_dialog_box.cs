using UnityEngine;
using System.Collections;

public class single_dialog_box : MonoBehaviour {

	private s_message m_out_message = null;
	public GameObject m_text;

	public void reset(string text, s_message mes)
	{
		this.gameObject.SetActive (true);
		m_out_message = mes;
		m_text.GetComponent<UILabel> ().text = text;
	}

	void click(GameObject obj)
	{
		if (m_out_message != null)
		{
			cmessage_center._instance.add_message(m_out_message);
		}
		m_out_message = null;
		this.gameObject.GetComponent<ui_show_anim>().hide_ui();
	}
}

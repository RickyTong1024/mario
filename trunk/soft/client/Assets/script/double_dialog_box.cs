using UnityEngine;
using System.Collections;

public class double_dialog_box : MonoBehaviour {
	
	private s_message m_out_message = null;
	private s_message m_out_message1 = null;
	public GameObject m_text;
	
	public void reset(string text, s_message mes, s_message mes1)
	{
		this.gameObject.SetActive (true);
		m_out_message = mes;
		m_out_message1 = mes1;
		m_text.GetComponent<UILabel> ().text = text;
	}
	
	void click(GameObject obj)
	{
		if (obj.name == "ok")
		{
			if (m_out_message != null)
			{
				cmessage_center._instance.add_message(m_out_message);
			}
			m_out_message = null;
			this.gameObject.GetComponent<ui_show_anim>().hide_ui();
		}
		if (obj.name == "cancel")
		{
			if (m_out_message1 != null)
			{
				cmessage_center._instance.add_message(m_out_message1);
			}
			m_out_message1 = null;
			this.gameObject.GetComponent<ui_show_anim>().hide_ui();
		}
	}
}
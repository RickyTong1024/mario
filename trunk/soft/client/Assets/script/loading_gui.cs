using UnityEngine;
using System.Collections;

public class loading_gui : MonoBehaviour, IMessage {

	void Start () {
		cmessage_center._instance.add_handle (this);
	}
	
	void OnDestroy()
	{
		cmessage_center._instance.remove_handle (this);
	}

	void OnEnable()
	{
		mario._instance.wait (true, game_data._instance.get_language_string("loading_gui_zzjz"));
		s_message mes = new s_message();
		mes.m_type = "first_load";
		mes.time = 0.1f;
		cmessage_center._instance.add_message(mes);
	}

	public void message (s_message message)
	{
		if (message.m_type == "first_load_end")
		{	
			LJSDK._instance.init();
		}
		if (message.m_type == "init_success")
		{
			mario._instance.wait(false);
			mario._instance.change_state(e_game_state.egs_login, 0, delegate() { Object.Destroy(this.gameObject); });
		}
		if (message.m_type == "init_failed")
		{
			mario._instance.change_state(e_game_state.egs_login, 0, delegate() { Object.Destroy(this.gameObject); });
		}
	}
	
	public void net_message (s_net_message message)
	{
	}
}

using UnityEngine;
using System.Collections;

public class paihang_gui : MonoBehaviour, IMessage {

	public GameObject m_ph_view;
	public GameObject m_ph_sub;
	public GameObject m_l1;
	private bool m_vv = false;
	// Use this for initialization
	void Start () {
		cmessage_center._instance.add_handle (this);
	}
	
	void OnDestroy()
	{
		cmessage_center._instance.remove_handle (this);
	}

	public void check()
	{
		if (m_vv)
		{
			m_vv = false;
			this.gameObject.SetActive(true);
		}
	}

	public void reset (protocol.game.smsg_view_map_point_rank msg, int id)
	{
		m_l1.SetActive (true);
		mario._instance.remove_child(m_ph_view);
		for (int i = 0; i < msg.ranks.Count; ++i)
		{
			protocol.game.map_point_rank mpr = msg.ranks[i];
			GameObject obj = (GameObject)Instantiate(m_ph_sub);
			obj.transform.parent = m_ph_view.transform;
			obj.transform.localPosition = new Vector3(0,170 - i * 75,0);
			obj.transform.localScale = new Vector3(1,1,1);
			obj.GetComponent<paihang_sub>().reset(i + 1, id, mpr);
			obj.SetActive(true);
		}
		m_ph_view.GetComponent<UIScrollView>().ResetPosition();
	}

	void look_player(int uid)
	{
		protocol.game.cmsg_view_player msg = new protocol.game.cmsg_view_player();
		msg.userid = uid;
		net_http._instance.send_msg<protocol.game.cmsg_view_player>(opclient_t.OPCODE_VIEW_PLAYER, msg);
	}

	void click(GameObject obj)
	{
		if (obj.name == "close")
		{
			this.GetComponent<ui_show_anim>().hide_ui();
		}
	}

	public void message (s_message message)
	{
		if (message.m_type == "ph_look_player")
		{
			int uid = (int)message.m_ints[0];
			this.GetComponent<ui_show_anim>().hide_ui();
			look_player(uid);
		}
	}

	public void net_message (s_net_message message)
	{
		if (message.m_opcode == opclient_t.OPCODE_VIEW_VIDEO)
		{
			m_vv = true;
		}
	}
}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class login_gui : MonoBehaviour, IMessage {

	public GameObject m_user;
	public GameObject m_user_tip;
	public GameObject m_zhuce;
	public GameObject m_qiehuan;
	public GameObject m_cname;
	public GameObject m_main;
	private int m_index = 1;
	public GameObject m_select;
	public GameObject m_select1;
	public GameObject m_qh_zh;
	public GameObject m_qh_mm;
	public GameObject m_zc_zh;
	public GameObject m_zc_mm;
	public GameObject m_zc_nc;
	public GameObject m_zc_cc;
	public GameObject m_zc_ccn;
	public GameObject m_guojia;
	public GameObject m_guojia_panel;
	public GameObject m_guojia_gj;
	private int m_guojia_type = 0;
	public GameObject m_cn_nc;
	public GameObject m_cname_cc;
	public GameObject m_cname_ccn;
	public GameObject m_name;
	public GameObject m_level;
	public GameObject m_level_text;
	public GameObject m_exp;
	public GameObject m_exp_text;
	public GameObject m_touxiang;
	public GameObject m_guoqi;
	public List<GameObject> m_texts;
	public List<GameObject> m_txs;
	public GameObject m_zm;
	public GameObject m_dj;
	public GameObject m_icon_panel;
	public GameObject m_sys;
	public GameObject m_lbm;
	public GameObject m_lbm_text;
	public GameObject m_about;
	public GameObject m_ver;
	public GameObject m_cn_sys_panel;
	public GameObject m_en_sys_panel;
	public GameObject m_gg;
	public GameObject m_gg_texture;
	public login_anim m_login_anim;

	// Use this for initialization
	void Start () {
		cmessage_center._instance.add_handle (this);
		m_ver.GetComponent<UILabel>().text = "Ver " + game_data._instance.m_ver;
		if (game_data._instance.m_lang == e_language.el_chinese)
		{
			m_cn_sys_panel.SetActive(true);
			m_en_sys_panel.SetActive(false);
		}
		else
		{
			m_cn_sys_panel.SetActive(false);
			m_en_sys_panel.SetActive(true);
		}
		int y = 0;
		int x = 0;
		foreach (string gj in game_data._instance.m_t_guojia.Keys)
		{
			GameObject obj = (GameObject)Instantiate(m_guojia_gj);
			obj.transform.parent = m_guojia_panel.transform;
			obj.transform.localPosition = new Vector3(-380 + 140 * x, 230 - 50 * y, 0);
			obj.transform.localScale = new Vector3(1,1,1);
			obj.name = gj;
			obj.transform.FindChild("name").GetComponent<UILabel>().text = gj;
			obj.GetComponent<UISprite>().spriteName = game_data._instance.m_t_guojia[gj];
			obj.SetActive(true);
			x++;
			if (x >= 6)
			{
				x = 0;
				y++;
			}
		}
		if (game_data._instance.m_ad_url != "")
		{
			StartCoroutine(load_gg());
		}
	}
	
	void OnDestroy()
	{
		cmessage_center._instance.remove_handle (this);
	}

	void OnEnable()
	{
		m_user.SetActive(false);
		m_main.SetActive(false);
		m_dj.SetActive(false);
		m_icon_panel.SetActive (false);
	}

	IEnumerator load_gg()
	{
		WWW _www = new WWW (game_data._instance.m_ad_image_url + game_data._instance.get_url_end());
		while(!_www.isDone)
		{
			yield  return new WaitForSeconds(0.1f);
		}
		if(_www.error != null)
		{
			Debug.Log("http error : " + _www.error);  
		} 
		else
		{
			m_gg_texture.GetComponent<UITexture>().mainTexture = _www.texture;
			if (LJSDK._instance.need_gg () && (m_dj.activeSelf || m_login_anim.m_play))
			{
				m_gg.SetActive (true);
			}
		}
	}

	void reset()
	{
		m_dj.SetActive(false);
		if (mario._instance.m_self == null)
		{
			if (LJSDK._instance.need_pt())
			{
				mario._instance.wait(true, game_data._instance.get_language_string("login_gui_dlpt"));
				LJSDK._instance.login();
			}
			else
			{
				protocol.game.cmsg_login msg = new protocol.game.cmsg_login();
				msg.openid = game_data._instance.m_save_data.openid;
				msg.openkey = game_data._instance.m_save_data.openkey;
				msg.nationality = "";
				msg.ver = game_data._instance.m_pt_ver;
				msg.channel = game_data._instance.m_channel;
				net_http._instance.send_msg<protocol.game.cmsg_login>(opclient_t.OPCODE_LOGIN, msg, true, game_data._instance.get_language_string("login_gui_zzlj"));
			}
		}
		else
		{
			m_user.SetActive(true);
			m_main.SetActive(true);
			m_icon_panel.SetActive(true);
			reset_user ();
		}
		m_index = Random.Range (1, 7);
		m_select.transform.localPosition = m_txs [m_index - 1].transform.localPosition;
	}

	void reset_user()
	{
		m_name.GetComponent<UILabel>().text = mario._instance.m_self.get_name();
		s_t_exp t_exp = game_data._instance.get_t_exp (mario._instance.m_self.level);
		m_level.GetComponent<UISprite> ().spriteName = t_exp.icon;
		m_level_text.GetComponent<UILabel>().text = mario._instance.m_self.level.ToString();
		m_touxiang.GetComponent<UISprite> ().spriteName = game_data._instance.get_t_touxiang (mario._instance.m_self.head);
		m_guoqi.GetComponent<UISprite> ().spriteName = game_data._instance.get_t_guojia (mario._instance.m_self.nationality);
		t_exp = game_data._instance.get_t_exp (mario._instance.m_self.level + 1);
		if (t_exp != null)
		{
			float v = (float)mario._instance.m_self.exp / t_exp.exp;
			m_exp.GetComponent<UIProgressBar>().value = v;
			m_exp_text.GetComponent<UILabel>().text = mario._instance.m_self.exp.ToString() + "/" + t_exp.exp.ToString();
		}
		else
		{
			m_exp.GetComponent<UIProgressBar>().value = 1;
			m_exp_text.GetComponent<UILabel>().text = mario._instance.m_self.exp.ToString() + "/--";
		}
		if (mario._instance.m_self.visitor == 1)
		{
			m_user_tip.SetActive(true);
		}
		else
		{
			m_user_tip.SetActive(false);
		}
		m_zm.GetComponent<UISprite>().spriteName = "wjtx_jb0" + mario._instance.m_self.testify.ToString();
	}

	void clear_text()
	{
		for (int i = 0; i < m_texts.Count; ++i)
		{
			m_texts[i].GetComponent<UIInput>().value = "";
		}
	}

	void click(GameObject obj)
	{
		if (obj.name == "user")
		{
			if (!Application.isEditor)
			{
				if (LJSDK._instance.need_pt())
				{
					LJSDK._instance.logout();
					return;
				}
			}
			m_main.SetActive(false);
			if (mario._instance.m_self.visitor == 1)
			{
				clear_text();
				m_zhuce.SetActive(true);
			}
			else
			{
				clear_text();
				m_qiehuan.SetActive(true);
			}
		}
		if (obj.name == "close_zc")
		{
			m_zhuce.GetComponent<ui_show_anim>().hide_ui();
			m_main.SetActive(true);
		}
		if (obj.name == "close_qh")
		{
			m_qiehuan.GetComponent<ui_show_anim>().hide_ui();
			m_main.SetActive(true);
		}
		if (obj.name == "qhzh")
		{
			m_zhuce.GetComponent<ui_show_anim>().hide_ui();
			clear_text();
			m_qiehuan.SetActive(true);
		}
		if (obj.name == "qhok")
		{
			string zh = m_qh_zh.GetComponent<UIInput>().value;
			string mm = m_qh_mm.GetComponent<UIInput>().value;
			if (zh == "")
			{
				mario._instance.show_tip(game_data._instance.get_language_string("login_gui_zhbk"));
				return;
			}
			if (mm == "")
			{
				mario._instance.show_tip(game_data._instance.get_language_string("login_gui_mmbk"));
				return;
			}
			protocol.game.cmsg_change_account msg = new protocol.game.cmsg_change_account();
			msg.openid = zh;
			msg.openkey = mm;
			msg.channel = game_data._instance.m_channel;
			net_http._instance.send_msg<protocol.game.cmsg_change_account>(opclient_t.OPCODE_CHANGE_ACCOUNT, msg);
		}
		if (obj.name == "zcok")
		{
			string nc = m_zc_nc.GetComponent<UIInput>().value;
			string zh = m_zc_zh.GetComponent<UIInput>().value;
			string mm = m_zc_mm.GetComponent<UIInput>().value;
			string gj = m_zc_ccn.GetComponent<UILabel>().text;
			if (nc == "")
			{
				mario._instance.show_tip(game_data._instance.get_language_string("login_gui_ncbk"));
				return;
			}
			if (zh == "")
			{
				mario._instance.show_tip(game_data._instance.get_language_string("login_gui_zhbk"));
				return;
			}
			if (mm == "")
			{
				mario._instance.show_tip(game_data._instance.get_language_string("login_gui_mmbk"));
				return;
			}
			if (gj == "")
			{
				mario._instance.show_tip(game_data._instance.get_language_string("login_gui_gjbk"));
				return;
			}
			protocol.game.cmsg_register msg = new protocol.game.cmsg_register();
			msg.openid = zh;
			msg.openkey = mm;
			msg.nickname = nc;
			msg.head = m_index;
			msg.nationality = gj;
			net_http._instance.send_msg<protocol.game.cmsg_register>(opclient_t.OPCODE_REGISTER, msg);
		}
		if (obj.name == "zccc")
		{
			m_guojia.SetActive(true);
			m_zhuce.SetActive(false);
			m_guojia_type = 0;
		}
		if (obj.name == "close_gj")
		{
			m_guojia.SetActive(false);
			if (m_guojia_type == 0)
			{
				m_zhuce.SetActive(true);
			}
			else
			{
				m_cname.SetActive(true);
			}
		}
		if (obj.name == "cnok")
		{
			string nc = m_cn_nc.GetComponent<UIInput>().value;
			string gj = m_cname_ccn.GetComponent<UILabel>().text;
			if (nc == "")
			{
				mario._instance.show_tip(game_data._instance.get_language_string("login_gui_ncbk"));
				return;
			}
			if (gj == "")
			{
				mario._instance.show_tip(game_data._instance.get_language_string("login_gui_gjbk"));
				return;
			}
			protocol.game.cmsg_change_name msg = new protocol.game.cmsg_change_name();
			msg.nickname = nc;
			msg.head = m_index;
			msg.nationality = gj;
			net_http._instance.send_msg<protocol.game.cmsg_change_name>(opclient_t.OPCODE_CHANGE_NAME, msg);
		}
		if (obj.name == "cncc")
		{
			m_guojia.SetActive(true);
			m_cname.SetActive(false);
			m_guojia_type = 1;
		}
		if (obj.name == "play")
		{
			mario._instance.change_state(e_game_state.egs_play_select, 1, delegate() { Object.Destroy(this.gameObject); });
		}
		if (obj.name == "edit")
		{
			mario._instance.change_state(e_game_state.egs_edit_select, 1, delegate() { Object.Destroy(this.gameObject); });
		}
		if (obj.name == "sys")
		{
			if (mario._instance.m_self.m_review == 1)
			{
				return;
			}
			m_main.SetActive(false);
			m_sys.SetActive(true);
		}
		if (obj.name == "close_sys")
		{
			m_sys.GetComponent<ui_show_anim>().hide_ui();
			m_main.SetActive(true);
		}
		if (obj.name == "lbm")
		{
			m_lbm_text.GetComponent<UIInput>().value = "";
			m_lbm.SetActive(true);
		}
		if (obj.name == "close_lbm")
		{
			m_lbm.GetComponent<ui_show_anim>().hide_ui();
		}
		if (obj.name == "about")
		{
			m_about.SetActive(true);
		}
		if (obj.name == "close_about")
		{
			m_about.GetComponent<ui_show_anim>().hide_ui();
		}
		if (obj.name == "lbmok")
		{
			string lbm = m_lbm_text.GetComponent<UIInput>().value;
			if (lbm == "")
			{
				mario._instance.show_tip(game_data._instance.get_language_string("login_gui_lbmk"));
				return;
			}
			protocol.game.cmsg_libao msg = new protocol.game.cmsg_libao();
			msg.code = lbm;
			net_http._instance.send_msg<protocol.game.cmsg_libao>(opclient_t.OPCODE_LIBAO, msg);
		}
		if (obj.name == "facebook")
		{
			Application.OpenURL("https://www.facebook.com/myboxmaker/");
		}
		if (obj.name == "twitter")
		{
			Application.OpenURL("https://twitter.com/myboxmaker");
		}
		if (obj.name == "go")
		{
			mario._instance.play_sound("sound/quan");
			List<int> p = new List<int>();
			p.Add(0);
			p.Add(-20);
			mario._instance.change_state(e_game_state.egs_edit_select, 3, delegate() { Object.Destroy(this.gameObject); }, p);
		}
		if (obj.name == "close_gg")
		{
			m_gg.SetActive(false);
		}
		if (obj.name == "gg_texture")
		{
			Application.OpenURL(game_data._instance.m_ad_url);
		}
	}

	void select_guojia(GameObject obj)
	{
		string cc = obj.name;
		string ccs = game_data._instance.get_t_guojia (cc);
		m_guojia.SetActive(false);
		if (m_guojia_type == 0)
		{
			m_zhuce.SetActive(true);
			m_zc_cc.GetComponent<UISprite>().spriteName = ccs;
			m_zc_ccn.GetComponent<UILabel>().text = cc;
		}
		else
		{
			m_cname.SetActive(true);
			m_cname_cc.GetComponent<UISprite>().spriteName = ccs;
			m_cname_ccn.GetComponent<UILabel>().text = cc;
		}
	}


	void select_tx(GameObject obj)
	{
		m_index = int.Parse (obj.name);
		m_select.transform.localPosition = obj.transform.localPosition;
	}

	void select_tx1(GameObject obj)
	{
		m_index = int.Parse (obj.name);
		m_select1.transform.localPosition = obj.transform.localPosition;
	}

	public void message (s_message message)
	{
		if (message.m_type == "lj_logout")
		{
			m_user.SetActive(false);
			m_main.SetActive(false);
			m_icon_panel.SetActive(false);
		}
		if (message.m_type == "login_fail")
		{
			mario._instance.wait(false);
			m_dj.SetActive(true);
		}
	}

	public void net_message (s_net_message message)
	{
		if (message.m_opcode == opclient_t.OPCODE_LOGIN
		    || message.m_opcode == opclient_t.OPCODE_LOGIN_ANDROID
		    || message.m_opcode == opclient_t.OPCODE_CHANGE_ACCOUNT
		    || message.m_opcode == opclient_t.OPCODE_REGISTER)
		{
			if (message.m_res == -1)
			{
				mario._instance.show_single_dialog_box(game_data._instance.get_language_string("login_gui_feif"), null);
				return;
			}
			protocol.game.smsg_login _msg = net_http._instance.parse_packet<protocol.game.smsg_login> (message.m_byte);
			
			player pl = new player(_msg);
			if (mario._instance.m_self != null)
			{
				pl.m_review = mario._instance.m_self.m_review;
			}
			mario._instance.m_self = pl;
			game_data._instance.m_save_data.openid = _msg.openid;
			game_data._instance.m_save_data.openkey = _msg.openkey;
			game_data._instance.save_native();

			if (pl.nationality != "")
			{
				string ccs = game_data._instance.get_t_guojia (pl.nationality);
				m_zc_cc.GetComponent<UISprite>().spriteName = ccs;
				m_zc_ccn.GetComponent<UILabel>().text = pl.nationality;
				m_cname_cc.GetComponent<UISprite>().spriteName = ccs;
				m_cname_ccn.GetComponent<UILabel>().text = pl.nationality;
			}

			if (message.m_opcode == opclient_t.OPCODE_LOGIN)
			{
				m_user.SetActive(true);
				m_main.SetActive(true);
				m_icon_panel.SetActive(true);
				mario._instance.show_tip(game_data._instance.get_language_string("login_gui_dlcg"));
				pl.m_review = _msg.review;
			}
			else if (message.m_opcode == opclient_t.OPCODE_LOGIN_ANDROID)
			{
				if (pl.visitor == 1)
				{
					m_cname.SetActive(true);
				}
				else
				{
					m_user.SetActive(true);
					m_main.SetActive(true);
					m_icon_panel.SetActive(true);
					mario._instance.show_tip(game_data._instance.get_language_string("login_gui_dlcg"));
				}
			}
			else if (message.m_opcode == opclient_t.OPCODE_CHANGE_ACCOUNT)
			{
				m_qiehuan.GetComponent<ui_show_anim>().hide_ui();
				m_main.SetActive(true);
				mario._instance.show_tip(game_data._instance.get_language_string("login_gui_qhcg"));
			}
			else if (message.m_opcode == opclient_t.OPCODE_REGISTER)
			{
				m_zhuce.GetComponent<ui_show_anim>().hide_ui();
				m_main.SetActive(true);
				mario._instance.show_tip(game_data._instance.get_language_string("login_gui_wscg"));
			} 
			reset_user();
		}
		if (message.m_opcode == opclient_t.OPCODE_CHANGE_NAME)
		{
			string nc = m_cn_nc.GetComponent<UIInput>().value;
			string gj = m_cname_ccn.GetComponent<UILabel>().text;
			mario._instance.m_self.name = nc;
			mario._instance.m_self.head = m_index;
			mario._instance.m_self.nationality = gj;
			mario._instance.m_self.visitor = 0;
			m_cname.GetComponent<ui_show_anim>().hide_ui();
			m_user.SetActive(true);
			m_main.SetActive(true);
			m_icon_panel.SetActive(true);
			mario._instance.show_tip(game_data._instance.get_language_string("login_gui_wscg"));
			reset_user();
		}
		if (message.m_opcode == opclient_t.OPCODE_LIBAO)
		{
			protocol.game.smsg_libao _msg = net_http._instance.parse_packet<protocol.game.smsg_libao> (message.m_byte);
			string s = string.Format(game_data._instance.get_language_string("login_gui_gtlb"), _msg.life);
			mario._instance.show_tip(s);
			m_lbm.GetComponent<ui_show_anim>().hide_ui();
		}
	}
}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class gameload_gui : MonoBehaviour, IMessage {

	public JsonData m_version_www;
	private string m_file;
	private WWW m_www;
	public GameObject m_string;
	private float m_wait_update_res = 0.0f;
	private string m_boxmaker_apk;
	private string m_common_url = "http://mario.oss.yymoon.com/android/";

	void Start () 
	{
		cmessage_center._instance.add_handle (this);
		DontDestroyOnLoad(this.gameObject);
		update_res ();
	}
	
	void OnDestroy()
	{
		cmessage_center._instance.remove_handle (this);
	}

	IEnumerator download_height_ver(string file)
	{
		m_www = new WWW (file);
		while(!m_www.isDone)
		{
			yield  return new WaitForSeconds(0.1f);
		}
		if(m_www.error != null)
		{
			Debug.Log("http error : " + m_www.error); 
			this.StopAllCoroutines ();
			StartCoroutine(download_height_ver(file));
		}
		else
		{
			string ver = m_version_www["ver"].ToString();
			m_boxmaker_apk = Application.persistentDataPath + "/" + "boxmaker_" + ver + ".apk";
			FileStream output = new FileStream(m_boxmaker_apk, FileMode.Create);
			output.Write (m_www.bytes, 0, m_www.bytes.Length);
			output.Flush();
			output.Close();
			this.Invoke("install", 1.0f);
		}
	}

	public void message (s_message message)
	{
		if (message.m_type == "gameload_button_ok")
		{
			int type = int.Parse(m_version_www["type"].ToString());
			string ourl = m_version_www["open_url"].ToString();

			if (type == 1)
			{
				Application.OpenURL(ourl);
				Application.Quit();
				return;
			}
			else if (type == 2)
			{
				StartCoroutine(download_height_ver(ourl));
			}
		}
		if (message.m_type == "gameload_button_cancel")
		{
			string min_ver = m_version_www["min_ver"].ToString();
			if (get_ver(min_ver) <= get_ver(game_data._instance.m_ver))
			{
				StartCoroutine(gonggao ());
			}
			else
			{
				Application.Quit(); 
			}
		}
	}
	
	public void net_message (s_net_message message)
	{

	}

	void login_game()
	{
		this.StopAllCoroutines ();
		mario._instance.change_state(e_game_state.egs_loading, 0, delegate() { UnityEngine.Object.Destroy(this.gameObject); });
	}

	void update_res()
	{
		m_wait_update_res = 0;
		this.StopAllCoroutines ();
		StartCoroutine(download_res());
	}

	IEnumerator download_res()
	{
		string _ver = m_common_url + game_data._instance.m_channel + "/version.json" + game_data._instance.get_url_end();
		WWW _version_www = new WWW (_ver);
		while(!_version_www.isDone)
		{
			yield  return new WaitForSeconds(0.1f);
		}
		if (_version_www.error != null)
		{
			Debug.Log("http error : " + _version_www.error);  
			update_res();
		} 
		else
		{
			m_version_www = JsonMapper.ToObject(_version_www.text);
			string ver = m_version_www["ver"].ToString();
			string min_ver = m_version_www["min_ver"].ToString();
			game_data._instance.m_ad_url = m_version_www["ad_url"].ToString();
			game_data._instance.m_ad_image_url = m_version_www["ad_image_url"].ToString();

			s_message mes_ok = new s_message();
			mes_ok.m_type = "gameload_button_ok";
			s_message mes_cancel = new s_message();
			mes_cancel.m_type = "gameload_button_cancel";

			if (get_ver(min_ver) > get_ver(game_data._instance.m_ver))
			{
				mario._instance.show_double_dialog_box(string.Format(game_data._instance.get_language_string("game_load_gui_bbgj"), ver), mes_ok, mes_cancel);
			}
			else if (get_ver(ver) > get_ver(game_data._instance.m_ver))
			{
				mario._instance.show_double_dialog_box(string.Format(game_data._instance.get_language_string("game_load_gui_fxxbb"), ver), mes_ok, mes_cancel);
			}
			else
			{
				StartCoroutine(gonggao ());
			}
		}
	}

	int get_ver(string ver)
	{
		try
		{
			string[] vers = ver.Split ('.');
			return int.Parse(vers[0]) * 1000 + int.Parse(vers[1]);
		}
		catch (System.Exception)
		{
			return 0;
		}
	}
	
	IEnumerator gonggao()
	{
		string _gg = m_common_url + game_data._instance.m_channel + "/gonggao.txt" + game_data._instance.get_url_end();
		WWW _www = new WWW (_gg);
		while(!_www.isDone)
		{
			yield  return new WaitForSeconds(0.1f);
		}
		if(_www.error != null)
		{
			Debug.Log("http error : " + _www.error);  
			update_res();
		} 
		else
		{
			game_data._instance.m_gonggao = _www.text;
			login_game();
		}
	}

	void install()
	{
		#if UNITY_ANDROID
		//MtaU3D._instance.install(m_boxmaker_apk);
		#endif
		Application.Quit(); 
	}

	// Update is called once per frame
	void Update ()
	{
		string s = game_data._instance.get_language_string("game_load_gui_gxwj");
		if(m_www != null)
		{
			if(m_www.progress > 0)
			{
				string ss = m_www.url.Substring(m_www.url.LastIndexOf("/") + 1);
				int progress = (int)(m_www.progress * 100);
				s = string.Format(game_data._instance.get_language_string("game_load_gui_gxwj1"), ss, progress);
			}
			m_string.GetComponent<UILabel> ().text = s;
		}
		else if(m_version_www == null)
		{
			m_wait_update_res += Time.deltaTime;
			if(m_wait_update_res > 15)
			{
				update_res();
			}
			m_string.GetComponent<UILabel> ().text = string.Format(game_data._instance.get_language_string("game_load_gui_bbxx"), (int)m_wait_update_res);
		}
	}
}

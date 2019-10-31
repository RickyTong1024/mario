using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class mario : MonoBehaviour {
	public static mario _instance;
	public GameObject m_uiroot;
	public Camera m_ui_camera;
	public Font m_efont;
	public player m_self = null;

	private List<AudioSource> m_play_sounds = new List<AudioSource>();
	private int m_play_sound_index = 0;
	private AudioSource m_play_sound_ex;
	private AudioSource m_play_sound_ex1;
	private bool m_enable_sound = true;
	public AudioSource m_play_mus;
	private string m_mus_name;
	private bool m_loop_mus = true;
	private bool m_stop_mus = false;
	private float m_pitch_mus = 1.0f;
	private float m_vols = 1.0f;
	public int m_width = 960;
	public int m_height = 640;
	public int m_start_type = 0;
	public Texture m_mouse;
	private float m_heng;
	private float m_per_heng;
	private float m_shu;
	private float m_per_shu;
	
	void Awake()
	{
		_instance = this;
	}

	void Start()
	{
		Screen.showCursor = false;
		Application.targetFrameRate = 60;
		#if UNITY_ANDROID || UNITY_IPHONE
		m_width = Screen.width * m_height / Screen.height;
		Screen.SetResolution (m_width, 640, true);
		#endif
		game_data._instance.init ();
	}

	public void change_ff(int id, int full)
	{
		#if UNITY_STANDALONE_WIN
		if (id == 0)
		{
			m_width = 960;
		}
		else
		{
			m_width = 1136;
		}
		if (full == 0)
		{
			Screen.SetResolution (m_width, 640, false);
		}
		else
		{
			Screen.SetResolution (m_width, 640, true);
		}
		#endif
	}

	public void ginit_callbak()
	{
		init_ui ();
		bool flag = false;
		#if UNITY_ANDROID
		if (!Application.isEditor)
		{
			flag = true;
			change_state (e_game_state.egs_gameload, 0, null);
		}
		/*else
		{
			flag = true;
			game_data._instance.m_channel = "yymoon";
			change_state (e_game_state.egs_gameload, 0, null);
		}*/
		#endif
		if (!flag)
		{
			change_state (e_game_state.egs_loading, 0, null);
		}
	}

	public void remove_child(GameObject obj)
	{
		if(obj == null)
		{
			return;
		}
		
		List<GameObject> objs = new List<GameObject>();
		for(int i = 0;i < obj.transform.childCount;i ++)
		{
			objs.Add(obj.transform.GetChild(i).gameObject);
		}
		for(int i = 0;i < objs.Count;i ++)
		{
			Object.Destroy(objs[i]);
		}
		objs.Clear ();
	}

	public bool get_mouse_button()
	{
		if(Input.touchCount == 1)
		{
			return true;
		}
		return Input.GetMouseButton (0);
	}
	
	public Vector2 get_mouse_position()
	{
		Vector2 pos = new Vector3 ();
		if(Input.touchCount > 0)
		{
			pos = Input.GetTouch(0).position;
		}
		else
		{
			pos = Input.mousePosition;
		}
		
		float _height = m_uiroot.GetComponent<UIRoot> ().activeHeight;
		
		pos.x = pos.x * (_height / (float)Screen.height);
		pos.y = pos.y * (_height / (float)Screen.height);
		
		return pos;
	}
	
	/////////////////////////////////////////////////////////////////////	
	
	public e_game_state m_game_state = e_game_state.egs_null;
	public GameObject m_main;
	public GameObject m_other;
	private GameObject m_wait_gui;
	private GameObject m_single_dialog_box;
	private GameObject m_xsjx_dialog_box;
	private GameObject m_double_dialog_box;
	private GameObject m_tip_gui;
	private GameObject m_play_gui;
	private GameObject m_user_gui;
	private GameObject m_mask_gui;
	private GameObject m_play_mask_gui;
	private GameObject m_player_gui;
	private GameObject m_clear_gui;
	private GameObject m_road_gui;
	private GameObject m_start_gui;
	private GameObject m_br_end_gui;
	private GameObject m_paihang_gui;
	private GameObject m_shop_gui;

	private GameObject m_gameload_gui;
	private GameObject m_loading_gui;
	private GameObject m_login_gui;
	private GameObject m_edit_gui;
	private GameObject m_play_select_gui;
	private GameObject m_edit_select_gui;

	void init_ui()
	{
		GameObject res = Resources.Load("ui/wait_gui") as GameObject;
		m_wait_gui = (GameObject)Instantiate(res);
		m_wait_gui.transform.parent = m_other.transform;
		m_wait_gui.transform.localPosition = new Vector3(0,0,0);
		m_wait_gui.transform.localScale = new Vector3(1,1,1);

		res = Resources.Load("ui/single_dialog_box") as GameObject;
		m_single_dialog_box = (GameObject)Instantiate(res);
		m_single_dialog_box.transform.parent = m_other.transform;
		m_single_dialog_box.transform.localPosition = new Vector3(0,0,0);
		m_single_dialog_box.transform.localScale = new Vector3(1,1,1);

		res = Resources.Load("ui/xsjx_dialog_box") as GameObject;
		m_xsjx_dialog_box = (GameObject)Instantiate(res);
		m_xsjx_dialog_box.transform.parent = m_other.transform;
		m_xsjx_dialog_box.transform.localPosition = new Vector3(0,0,0);
		m_xsjx_dialog_box.transform.localScale = new Vector3(1,1,1);

		res = Resources.Load("ui/double_dialog_box") as GameObject;
		m_double_dialog_box = (GameObject)Instantiate(res);
		m_double_dialog_box.transform.parent = m_other.transform;
		m_double_dialog_box.transform.localPosition = new Vector3(0,0,0);
		m_double_dialog_box.transform.localScale = new Vector3(1,1,1);

		res = Resources.Load("ui/tip_gui") as GameObject;
		m_tip_gui = (GameObject)Instantiate(res);
		m_tip_gui.transform.parent = m_other.transform;
		m_tip_gui.transform.localPosition = new Vector3(0,0,0);
		m_tip_gui.transform.localScale = new Vector3(1,1,1);

		res = Resources.Load("ui/play_gui") as GameObject;
		m_play_gui = (GameObject)Instantiate(res);
		m_play_gui.transform.parent = m_main.transform;
		m_play_gui.transform.localPosition = new Vector3(0,0,0);
		m_play_gui.transform.localScale = new Vector3(1,1,1);

		res = Resources.Load("ui/user_gui") as GameObject;
		m_user_gui = (GameObject)Instantiate(res);
		m_user_gui.transform.parent = m_main.transform;
		m_user_gui.transform.localPosition = new Vector3(0,0,0);
		m_user_gui.transform.localScale = new Vector3(1,1,1);

		res = Resources.Load("ui/mask_gui") as GameObject;
		m_mask_gui = (GameObject)Instantiate(res);
		m_mask_gui.transform.parent = m_other.transform;
		m_mask_gui.transform.localPosition = new Vector3(0,0,0);
		m_mask_gui.transform.localScale = new Vector3(1,1,1);

		res = Resources.Load("ui/play_mask_gui") as GameObject;
		m_play_mask_gui = (GameObject)Instantiate(res);
		m_play_mask_gui.transform.parent = m_other.transform;
		m_play_mask_gui.transform.localPosition = new Vector3(0,0,0);
		m_play_mask_gui.transform.localScale = new Vector3(1,1,1);

		res = Resources.Load("ui/player_gui") as GameObject;
		m_player_gui = (GameObject)Instantiate(res);
		m_player_gui.transform.parent = m_main.transform;
		m_player_gui.transform.localPosition = new Vector3(0,0,0);
		m_player_gui.transform.localScale = new Vector3(1,1,1);

		res = Resources.Load("ui/clear_gui") as GameObject;
		m_clear_gui = (GameObject)Instantiate(res);
		m_clear_gui.transform.parent = m_other.transform;
		m_clear_gui.transform.localPosition = new Vector3(0,0,0);
		m_clear_gui.transform.localScale = new Vector3(1,1,1);

		res = Resources.Load("ui/road_gui") as GameObject;
		m_road_gui = (GameObject)Instantiate(res);
		m_road_gui.transform.parent = m_other.transform;
		m_road_gui.transform.localPosition = new Vector3(0,0,0);
		m_road_gui.transform.localScale = new Vector3(1,1,1);

		res = Resources.Load("ui/start_gui") as GameObject;
		m_start_gui = (GameObject)Instantiate(res);
		m_start_gui.transform.parent = m_other.transform;
		m_start_gui.transform.localPosition = new Vector3(0,0,0);
		m_start_gui.transform.localScale = new Vector3(1,1,1);

		res = Resources.Load("ui/br_end_gui") as GameObject;
		m_br_end_gui = (GameObject)Instantiate(res);
		m_br_end_gui.transform.parent = m_other.transform;
		m_br_end_gui.transform.localPosition = new Vector3(0,0,0);
		m_br_end_gui.transform.localScale = new Vector3(1,1,1);

		res = Resources.Load("ui/paihang_gui") as GameObject;
		m_paihang_gui = (GameObject)Instantiate(res);
		m_paihang_gui.transform.parent = m_other.transform;
		m_paihang_gui.transform.localPosition = new Vector3(0,0,0);
		m_paihang_gui.transform.localScale = new Vector3(1,1,1);

		res = Resources.Load("ui/shop_gui") as GameObject;
		m_shop_gui = (GameObject)Instantiate(res);
		m_shop_gui.transform.parent = m_other.transform;
		m_shop_gui.transform.localPosition = new Vector3(0,0,0);
		m_shop_gui.transform.localScale = new Vector3(1,1,1);
	}

	public delegate void ChangeStateHandle();

	public void change_state(e_game_state state, int type, ChangeStateHandle handle, List<int> param = null)
	{
		m_game_state = state;
		if (m_game_state == e_game_state.egs_gameload)
		{

		}
		else if (m_game_state == e_game_state.egs_loading || m_game_state == e_game_state.egs_login)
		{
			play_mus("music/login");
		}
		else if (m_game_state == e_game_state.egs_edit_select || m_game_state == e_game_state.egs_play_select)
		{
			play_mus("music/select");
		}
		else if (m_game_state == e_game_state.egs_edit)
		{
			play_mus("music/select", true, 0.5f);
		}
		else if (m_game_state == e_game_state.egs_br_road || m_game_state == e_game_state.egs_br_end)
		{
			play_mus("music/road");
		}
		else if (m_game_state == e_game_state.egs_br_start)
		{
			stop_mus();
		}
		else
		{
			play_mus(game_data._instance.get_map_music(0), true, 1 - game_data._instance.m_map_data.no_music);
		}
		if (type == 0)
		{
			if (handle != null)
			{
				handle();
			}
			end_change_state();
		}
		else if (type == 1)
		{
			show_mask(delegate() { handle(); end_change_state(); });
		}
		else if (type == 2)
		{
			show_play_mask(delegate() { handle(); end_change_state(); });
		}
		else if (type == 3)
		{
			show_mask(delegate() { handle(); end_change_state(); }, param[0], param[1]);
		}
	}

	void end_change_state()
	{
		m_user_gui.SetActive (false);
		if (m_game_state == e_game_state.egs_gameload)
		{
			if (m_gameload_gui == null)
			{
				GameObject res = Resources.Load("ui/gameload_gui") as GameObject;
				m_gameload_gui = (GameObject)Instantiate(res);
				m_gameload_gui.transform.parent = m_main.transform;
				m_gameload_gui.transform.localPosition = new Vector3(0,0,0);
				m_gameload_gui.transform.localScale = new Vector3(1,1,1);
			}
			else
			{
				m_gameload_gui.SetActive(true);
			}
		}
		else if (m_game_state == e_game_state.egs_loading)
		{
			if (m_loading_gui == null)
			{
				GameObject res = Resources.Load("ui/loading_gui") as GameObject;
				m_loading_gui = (GameObject)Instantiate(res);
				m_loading_gui.transform.parent = m_main.transform;
				m_loading_gui.transform.localPosition = new Vector3(0,0,0);
				m_loading_gui.transform.localScale = new Vector3(1,1,1);
			}
			else
			{
				m_loading_gui.SetActive(true);
			}
		}
		else if (m_game_state == e_game_state.egs_login)
		{
			if (m_login_gui == null)
			{
				GameObject res = Resources.Load("ui/login_gui") as GameObject;
				m_login_gui = (GameObject)Instantiate(res);
				m_login_gui.transform.parent = m_main.transform;
				m_login_gui.transform.localPosition = new Vector3(0,0,0);
				m_login_gui.transform.localScale = new Vector3(1,1,1);
			}
			else
			{
				m_login_gui.SetActive(true);
			}
		}
		else if (m_game_state == e_game_state.egs_edit_select)
		{
			if (m_edit_select_gui == null)
			{
				GameObject res = Resources.Load("ui/edit_select_gui") as GameObject;
				m_edit_select_gui = (GameObject)Instantiate(res);
				m_edit_select_gui.transform.parent = m_main.transform;
				m_edit_select_gui.transform.localPosition = new Vector3(0,0,0);
				m_edit_select_gui.transform.localScale = new Vector3(1,1,1);
			}
			else
			{
				m_edit_select_gui.SetActive(true);
			}
			show_user(m_edit_select_gui);
			if (mario._instance.m_self.guide < 100)
			{
				m_edit_select_gui.GetComponent<edit_select_gui>().guide();
			}
		}
		else if (m_game_state == e_game_state.egs_edit)
		{
			if (m_edit_gui == null)
			{
				GameObject res = Resources.Load("ui/edit_gui") as GameObject;
				m_edit_gui = (GameObject)Instantiate(res);
				m_edit_gui.transform.parent = m_main.transform;
				m_edit_gui.transform.localPosition = new Vector3(0,0,0);
				m_edit_gui.transform.localScale = new Vector3(1,1,1);
			}
			else
			{
				m_edit_gui.SetActive(true);
			}
		}
		else if (m_game_state == e_game_state.egs_play_select)
		{
			if (m_play_select_gui == null)
			{
				GameObject res = Resources.Load("ui/play_select_gui") as GameObject;
				m_play_select_gui = (GameObject)Instantiate(res);
				m_play_select_gui.transform.parent = m_main.transform;
				m_play_select_gui.transform.localPosition = new Vector3(0,0,0);
				m_play_select_gui.transform.localScale = new Vector3(1,1,1);
			}
			else
			{
				m_play_select_gui.SetActive(true);
			}
			m_paihang_gui.GetComponent<paihang_gui>().check();
			show_user(m_play_select_gui);
		}
		else if (m_game_state == e_game_state.egs_play)
		{
			m_play_gui.SetActive(true);
			s_message mes = new s_message();
			mes.m_type = "play_mode";
			mes.m_object.Add (null);
			cmessage_center._instance.add_message(mes);
		}
		else if (m_game_state == e_game_state.egs_edit_play)
		{
			m_play_gui.SetActive(true);
			s_message mes = new s_message();
			mes.m_type = "play_mode";
			mes.m_object.Add (null);
			cmessage_center._instance.add_message(mes);
		}
		else if (m_game_state == e_game_state.egs_edit_upload)
		{
			m_play_gui.SetActive(true);
			s_message mes = new s_message();
			mes.m_type = "play_mode";
			mes.m_object.Add (null);
			cmessage_center._instance.add_message(mes);
		}
		else if (m_game_state == e_game_state.egs_review)
		{
			m_play_gui.SetActive(true);
			s_message mes = new s_message();
			mes.m_type = "play_mode";
			mes.m_object.Add (null);
			mes.m_ints.Add (0);
			mes.m_ints.Add (1);
			cmessage_center._instance.add_message(mes);
		}
		else if (m_game_state == e_game_state.egs_br_play)
		{
			m_play_gui.SetActive(true);
			s_message mes = new s_message();
			mes.m_type = "play_mode";
			mes.m_object.Add (null);
			cmessage_center._instance.add_message(mes);
		}
		else if (m_game_state == e_game_state.egs_br_road)
		{
			show_road_gui();
		}
		else if (m_game_state == e_game_state.egs_br_start)
		{
			show_start_gui();
		}
		else if (m_game_state == e_game_state.egs_br_end)
		{
			show_br_end();
		}
	}

	public void wait(bool flag, string text = "")
	{
		m_wait_gui.GetComponent<wait_gui> ().reset (flag, text);
	}
	
	public void show_single_dialog_box(string text, s_message message)
	{
		m_single_dialog_box.GetComponent<single_dialog_box> ().reset (text, message);
	}

	public void show_double_dialog_box(string text, s_message message, s_message message_cencel = null)
	{
		m_double_dialog_box.GetComponent<double_dialog_box> ().reset (text, message, message_cencel);
	}

	public void show_xsjx_dialog_box(string text, s_message message)
	{
		m_xsjx_dialog_box.GetComponent<single_dialog_box> ().reset (text, message);
	}

	public void show_tip(string text)
	{
		m_tip_gui.GetComponent<tip_gui> ().add_text (text);
	}

	public void show_user(GameObject obj)
	{
		m_user_gui.SetActive (true);
		m_user_gui.GetComponent<user_gui>().change_event(obj);
	}

	public void show_mask(ChangeStateHandle handle)
	{
		m_mask_gui.GetComponent<mask_gui>().reset(handle);
	}

	public void show_mask(ChangeStateHandle handle, int x, int y)
	{
		m_mask_gui.GetComponent<mask_gui>().reset(handle, 1, x, y);
	}

	public void show_play_mask(ChangeStateHandle handle)
	{
		m_play_mask_gui.GetComponent<play_mask_gui>().reset(handle);
	}

	public void show_play_gui()
	{
		m_play_gui.SetActive (true);
	}

	public void hide_play_gui()
	{
		m_play_gui.SetActive (false);
	}

	public void show_paihang_gui(protocol.game.smsg_view_map_point_rank msg, int id)
	{
		m_paihang_gui.SetActive (true);
		m_paihang_gui.GetComponent<paihang_gui> ().reset (msg, id);
	}

	public void hide_paihang_gui()
	{
		m_paihang_gui.GetComponent<ui_show_anim> ().hide_ui ();
	}

	public void show_shop_gui(int type)
	{
		m_shop_gui.GetComponent<shop_gui> ().reset (type);
	}

	public void show_player_gui(protocol.game.smsg_view_player msg)
	{
		m_player_gui.SetActive (true);
		m_player_gui.GetComponent<player_gui> ().reset (msg);
	}

	public void show_clear_gui(bool next)
	{
		m_clear_gui.GetComponent<clear_gui>().reset(1, 0, 0, 0, 0, next);
	}

	public void show_clear_gui(int exp, int eexp, int rank, int testify, bool next)
	{
		m_clear_gui.GetComponent<clear_gui>().reset(0, exp, eexp, rank, testify, next);
	}

	public void hide_clear_gui()
	{
		m_clear_gui.SetActive (false);
	}

	public void show_road_gui()
	{
		m_road_gui.SetActive (true);
	}

	public void show_start_gui()
	{
		m_start_gui.SetActive (true);
	}

	public void show_br_end()
	{
		m_br_end_gui.SetActive (true);
	}

	public void play_mus(string name, bool loop = true, float vols = 1.0f, float pitch = 1.0f)
	{
		if (name != m_mus_name)
		{
			m_stop_mus = true;
		}
		else
		{
			m_play_mus.loop = loop;
			m_play_mus.volume = vols;
		}
		m_mus_name = name;
		m_loop_mus = loop;
		m_vols = vols;
		m_pitch_mus = pitch;
		if (m_play_mus.pitch == 0)
		{
			m_play_mus.volume = 0;
		}
	}

	public void play_mus_ex(string name, bool loop = true, float vols = 1.0f, float pitch = 1.0f)
	{
		m_stop_mus = true;
		m_mus_name = name;
		m_loop_mus = loop;
		m_vols = vols;
		m_pitch_mus = pitch;
		if (m_play_mus.pitch == 0)
		{
			m_play_mus.volume = 0;
		}
	}

	public void play_mus_ex1(string name, bool loop = true, float vols = 1.0f, float pitch = 1.0f)
	{
		AudioClip _clip = Resources.Load(name) as AudioClip;
		if(_clip == null)
		{
			return;
		}

		m_stop_mus = false;
		m_mus_name = name;
		m_loop_mus = loop;
		m_vols = vols;
		m_pitch_mus = pitch;

		m_play_mus.clip =_clip;
		m_play_mus.loop = m_loop_mus;
		m_play_mus.pitch = m_pitch_mus;
		m_play_mus.volume = m_vols;
		m_play_mus.Play();
	}

	public void stop_mus()
	{
		m_stop_mus = true;
		m_mus_name = "";
	}

	public void pause_mus()
	{
		m_play_mus.Pause();
	}

	public void continue_mus(float speed)
	{
		m_pitch_mus = speed;
		m_play_mus.pitch = m_pitch_mus;
		m_play_mus.Play ();
	}

	public void enable_bgm()
	{
		m_play_mus.enabled = true;
	}

	public void disable_bgm()
	{
		m_play_mus.enabled = false;
	}

	public void enable_sound()
	{
		m_enable_sound = true;
	}

	public void disable_sound()
	{
		m_enable_sound = false;
	}

	public void play_sound(string name, float vols = 1.0f)
	{
		if (!m_enable_sound)
		{
			return;
		}
		AudioClip _clip = Resources.Load(name) as AudioClip;
		if(_clip == null)
		{
			return;
		}
		
		if (m_play_sounds.Count < m_play_sound_index + 1)
		{		
			m_play_sounds.Add(transform.gameObject.AddComponent<AudioSource>());
		}
		
		m_play_sounds[m_play_sound_index].clip = _clip;
		m_play_sounds[m_play_sound_index].volume = vols;
		m_play_sounds[m_play_sound_index].Play();
		m_play_sound_index++;
		if (m_play_sound_index >= 5)
		{
			m_play_sound_index = 0;
		}
	}

	public void play_sound_ex(string name, float vols = 1.0f)
	{
		if (!m_enable_sound)
		{
			return;
		}
		AudioClip _clip = Resources.Load(name) as AudioClip;
		if(_clip == null)
		{
			return;
		}
		
		if (m_play_sound_ex == null)
		{		
			m_play_sound_ex = transform.gameObject.AddComponent<AudioSource>();
		}
		
		m_play_sound_ex.clip = _clip;
		m_play_sound_ex.volume = vols;
		m_play_sound_ex.Play();
	}

	public void play_sound_ex1(string name, float vols = 1.0f)
	{
		if (!m_enable_sound)
		{
			return;
		}
		AudioClip _clip = Resources.Load(name) as AudioClip;
		if(_clip == null)
		{
			return;
		}
		
		if (m_play_sound_ex1 == null)
		{		
			m_play_sound_ex1 = transform.gameObject.AddComponent<AudioSource>();
		}
		
		m_play_sound_ex1.clip = _clip;
		m_play_sound_ex1.volume = vols;
		m_play_sound_ex1.Play();
	}

	public KeyCode find_key(KeyCode kc)
	{
		KeyCode key = kc;
		if (kc == KeyCode.UpArrow && game_data._instance.m_save_data.keys[0] != 0)
		{
			key = (KeyCode)game_data._instance.m_save_data.keys[0];
		}
		else if (kc == KeyCode.DownArrow && game_data._instance.m_save_data.keys[1] != 0)
		{
			key = (KeyCode)game_data._instance.m_save_data.keys[1];
		}
		else if (kc == KeyCode.LeftArrow && game_data._instance.m_save_data.keys[2] != 0)
		{
			key = (KeyCode)game_data._instance.m_save_data.keys[2];
		}
		else if (kc == KeyCode.RightArrow && game_data._instance.m_save_data.keys[3] != 0)
		{
			key = (KeyCode)game_data._instance.m_save_data.keys[3];
		}
		else if (kc == KeyCode.Z && game_data._instance.m_save_data.keys[4] != 0)
		{
			key = (KeyCode)game_data._instance.m_save_data.keys[4];
		}
		else if (kc == KeyCode.X && game_data._instance.m_save_data.keys[5] != 0)
		{
			key = (KeyCode)game_data._instance.m_save_data.keys[5];
		}
		return key;
	}

	
	public bool key_down(KeyCode kc)
	{
		if (kc == KeyCode.LeftArrow)
		{
			if (Input.GetKeyDown(mario._instance.find_key(KeyCode.LeftArrow)))
			{
				return true;
			}
			if (m_per_heng > -0.1f && m_heng < -0.1f)
			{
				return true;
			}
		}
		else if (kc == KeyCode.RightArrow)
		{
			if (Input.GetKeyDown(mario._instance.find_key(KeyCode.RightArrow)))
			{
				return true;
			}
			if (m_per_heng < 0.1f && m_heng > 0.1f)
			{
				return true;
			}
		}
		else if (kc == KeyCode.UpArrow)
		{
			if (Input.GetKeyDown(mario._instance.find_key(KeyCode.UpArrow)))
			{
				return true;
			}
			if (m_per_shu > -0.1f && m_shu < -0.1f)
			{
				return true;
			}
		}
		else if (kc == KeyCode.DownArrow)
		{
			if (Input.GetKeyDown(mario._instance.find_key(KeyCode.DownArrow)))
			{
				return true;
			}
			if (m_per_shu < 0.1f && m_shu > 0.1f)
			{
				return true;
			}
		}
		else if (kc == KeyCode.Z)
		{
			if (Input.GetKeyDown(mario._instance.find_key(KeyCode.Z)))
			{
				return true;
			}
			if (Input.GetKeyDown(mario._instance.find_key(KeyCode.Joystick1Button1)))
			{
				return true;
			}
		}
		else if (kc == KeyCode.X)
		{
			if (Input.GetKeyDown(mario._instance.find_key(KeyCode.X)))
			{
				return true;
			}
			if (Input.GetKeyDown(mario._instance.find_key(KeyCode.Joystick1Button2)))
			{
				return true;
			}
		}
		else if (kc == KeyCode.Escape)
		{
			if (Input.GetKeyDown(mario._instance.find_key(KeyCode.Escape)))
			{
				return true;
			}
			if (Input.GetKeyDown(mario._instance.find_key(KeyCode.Joystick1Button7)))
			{
				return true;
			}
		}
		return false;
	}

	public bool key_up(KeyCode kc)
	{
		if (kc == KeyCode.LeftArrow)
		{
			if (Input.GetKeyUp(mario._instance.find_key(KeyCode.LeftArrow)))
			{
				return true;
			}
			if (m_per_heng < -0.1f && m_heng > -0.1f)
			{
				return true;
			}
		}
		else if (kc == KeyCode.RightArrow)
		{
			if (Input.GetKeyUp(mario._instance.find_key(KeyCode.RightArrow)))
			{
				return true;
			}
			if (m_per_heng > 0.1f && m_heng < 0.1f)
			{
				return true;
			}
		}
		else if (kc == KeyCode.UpArrow)
		{
			if (Input.GetKeyUp(mario._instance.find_key(KeyCode.UpArrow)))
			{
				return true;
			}
			if (m_per_shu < -0.1f && m_shu > -0.1f)
			{
				return true;
			}
		}
		else if (kc == KeyCode.DownArrow)
		{
			if (Input.GetKeyUp(mario._instance.find_key(KeyCode.DownArrow)))
			{
				return true;
			}
			if (m_per_shu > 0.1f && m_shu < 0.1f)
			{
				return true;
			}
		}
		else if (kc == KeyCode.Z)
		{
			if (Input.GetKeyUp(mario._instance.find_key(KeyCode.Z)))
			{
				return true;
			}
			if (Input.GetKeyUp(mario._instance.find_key(KeyCode.Joystick1Button1)))
			{
				return true;
			}
		}
		else if (kc == KeyCode.X)
		{
			if (Input.GetKeyUp(mario._instance.find_key(KeyCode.X)))
			{
				return true;
			}
			if (Input.GetKeyUp(mario._instance.find_key(KeyCode.Joystick1Button2)))
			{
				return true;
			}
		}
		else if (kc == KeyCode.Escape)
		{
			if (Input.GetKeyUp(mario._instance.find_key(KeyCode.Escape)))
			{
				return true;
			}
			if (Input.GetKeyUp(mario._instance.find_key(KeyCode.Joystick1Button7)))
			{
				return true;
			}
		}
		return false;
	}

	void Update()
	{
		if (m_self != null)
		{
			m_self.update();
		}

		if(m_stop_mus == true)
		{
			if(m_play_mus.volume > 0)
			{
				m_play_mus.volume -= Time.deltaTime;
			}
			
			if(m_play_mus.volume <= 0)
			{
				m_stop_mus = false;
				m_play_mus.volume = 0.0f;
				m_play_mus.Stop();
				
				if(m_mus_name.Length > 0)
				{
					AudioClip _clip = Resources.Load(m_mus_name) as AudioClip;
					
					if(_clip != null)
					{
						m_play_mus.clip =_clip;
						m_play_mus.loop = m_loop_mus;
						m_play_mus.pitch = m_pitch_mus;
						m_play_mus.Play();
					}
				}
			}
		}
		else if(m_play_mus.isPlaying && m_play_mus.volume < m_vols)
		{
			m_play_mus.volume += Time.deltaTime;
			if (m_play_mus.volume > m_vols)
			{
				m_play_mus.volume = m_vols;
			}
		}
		
		m_per_heng = m_heng;
		m_heng = Input.GetAxis("heng");
		m_per_shu = m_shu;
		m_shu = Input.GetAxis("shu");
	}
	
#if UNITY_STANDALONE_WIN || UNITY_WEBPLAYER
	void OnGUI()
	{
		Vector3 msPos = Input.mousePosition;
		GUI.DrawTexture(new Rect(msPos.x - 10, Screen.height-msPos.y - 5, 48, 48), m_mouse);
	}
#endif
}

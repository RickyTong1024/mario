using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class s_t_save_data
{
	public string openid;
	public string openkey;
	public int is_bgm;
	public int is_sound;
	public int is_full;
	public int fbl;

	public List<int> keys;
}

public enum e_language
{
	el_chinese = 0,
	el_english,
}

public enum e_game_state
{
	egs_null = 0,
	egs_gameload,
	egs_loading,
	egs_login,
	egs_play_select,
	egs_play,
	egs_review,
	egs_edit_select,
	egs_edit,
	egs_edit_play,
	egs_edit_upload,
	egs_br_road,
	egs_br_start,
	egs_br_play,
	egs_br_end,
}

public class s_t_mission_sub
{
	private int _type;
	public int type
	{
		get
		{
			return _type;
		}
		set
		{
			if (_type != value)
			{
				_type = value;
				param.Clear();
				if (_type != 0)
				{
					for (int i = 0; i < 4; ++i)
					{
						param.Add(0);
					}
				}
			}
		}
	}
	
	public List<int> param = new List<int>();
}

public class s_t_language
{
	public string id;
	public string zw;
	public string ew;
}

public class s_t_unit
{
	public int id;
	public string name;
	public string icon;
	public string res;
	public int yc;
	public int kfg;
	public int is_static;
	public int fwt;
	public int review;
	public int is_sw;
	public int max_num;
}

public class s_t_exp
{
	public int level;
	public int exp;
	public int zm;
	public string icon;
	public int max_exp;
}

public class s_t_job_exp
{
	public int level;
	public int exp;
}

public class s_t_view_title
{
	public int id;
	public string name;
	public string icon;
}

public class s_t_view_map
{
	public int id;
	public string name;
	public string icon;
}

public class s_t_shop
{
	public int id;
	public int slot;
	public string name;
	public int type;
	public int price;
	public float price_my;
	public string icon;
	public string db;
	public int def;
	public string code;
	public string ios_desc = "";
	public string desc;
}

public class s_t_fg
{
	public int id;
	public string name;
	public int tj;
	public string desc;
	public string music;
}

public class s_t_br
{
	public int id;
	public string name;
	public int num;
	public string desc;
	public string unlock;
}

public class s_t_key
{
	public int code;
	public string name;
}

public class game_data
{
	public static game_data m_game_data;
	public const int m_self_map_ver = 2;
	public string m_ver = "1.20";
	public string m_pt_ver = "";
	public string m_channel = "";
	public e_language m_lang = e_language.el_chinese;
	public string m_gonggao = "";
	public string m_ad_url = "";
	public string m_ad_image_url = "";

	public static game_data _instance
	{
		get
		{
			if(m_game_data == null)
			{
				m_game_data = new game_data();
			}
			
			return 	m_game_data;
		}
	}

	public s_t_save_data m_save_data = new s_t_save_data();

	public void load_native()
	{
		if (PlayerPrefs.HasKey("openid"))
		{
			m_save_data.openid = PlayerPrefs.GetString("openid");
			m_save_data.openkey = PlayerPrefs.GetString("openkey");
		}
		if (PlayerPrefs.HasKey("is_bgm"))
		{
			m_save_data.is_bgm = PlayerPrefs.GetInt("is_bgm");
			m_save_data.is_sound = PlayerPrefs.GetInt("is_sound");
			if (m_save_data.is_bgm == 0)
			{
				mario._instance.disable_bgm();
			}
			if (m_save_data.is_sound == 0)
			{
				mario._instance.disable_sound();
			}
		}
		else
		{
			m_save_data.is_bgm = 1;
			m_save_data.is_sound = 1;
		}
		if (PlayerPrefs.HasKey("is_full"))
		{
			m_save_data.is_full = PlayerPrefs.GetInt("is_full");
		}
		else
		{
			m_save_data.is_full = 0;
		}
		if (PlayerPrefs.HasKey("fbl"))
		{
			m_save_data.fbl = PlayerPrefs.GetInt("fbl");
		}
		else
		{
			m_save_data.fbl = 0;
		}
		mario._instance.change_ff (m_save_data.fbl, m_save_data.is_full);
		m_save_data.keys = new List<int> ();
		if (PlayerPrefs.HasKey("up"))
		{
			m_save_data.keys.Add(PlayerPrefs.GetInt("up"));
			m_save_data.keys.Add(PlayerPrefs.GetInt("down"));
			m_save_data.keys.Add(PlayerPrefs.GetInt("left"));
			m_save_data.keys.Add(PlayerPrefs.GetInt("right"));
			m_save_data.keys.Add(PlayerPrefs.GetInt("jump"));
			m_save_data.keys.Add(PlayerPrefs.GetInt("action"));
		}
		else
		{
			for (int i = 0; i < 6; ++i)
			{
				m_save_data.keys.Add(0);
			}
		}
	}
	
	public void save_native()
	{
		PlayerPrefs.SetString("openid", m_save_data.openid);
		PlayerPrefs.SetString("openkey", m_save_data.openkey);

		PlayerPrefs.SetInt("is_bgm", m_save_data.is_bgm);
		PlayerPrefs.SetInt("is_sound", m_save_data.is_sound);
		PlayerPrefs.SetInt("is_full", m_save_data.is_full);
		PlayerPrefs.SetInt("fbl", m_save_data.fbl);
		if (m_save_data.is_bgm == 0)
		{
			mario._instance.disable_bgm();
		}
		else
		{
			mario._instance.enable_bgm();
		}
		if (m_save_data.is_sound == 0)
		{
			mario._instance.disable_sound();
		}
		else
		{
			mario._instance.enable_sound();
		}
		if (m_save_data.is_full == 0 && Screen.fullScreen)
		{
			Screen.fullScreen = false;
		}
		else if (m_save_data.is_full == 1 && !Screen.fullScreen)
		{
			Screen.fullScreen = true;
		}
		mario._instance.change_ff (m_save_data.fbl, m_save_data.is_full);

		PlayerPrefs.SetInt("up", m_save_data.keys[0]);
		PlayerPrefs.SetInt("down", m_save_data.keys[1]);
		PlayerPrefs.SetInt("left", m_save_data.keys[2]);
		PlayerPrefs.SetInt("right", m_save_data.keys[3]);
		PlayerPrefs.SetInt("jump", m_save_data.keys[4]);
		PlayerPrefs.SetInt("action", m_save_data.keys[5]);
		PlayerPrefs.Save();
	}

	public void delete_native()
	{
		m_save_data.openid = "";
		m_save_data.openkey = "";
		m_save_data.is_bgm = 1;
		m_save_data.is_sound = 1;
		m_save_data.keys.Clear ();
		PlayerPrefs.DeleteAll();
	}
	
	public int m_map_id;
	public protocol.map.map_data1 m_map_data;
	public List< List< List<s_t_mission_sub> > > m_arrays = new List< List< List<s_t_mission_sub> > >();
	public List< List<mario_point> > m_die_poses = new List< List<mario_point> >();
	public List<int> m_map_inputs = new List<int> ();

	void new_world(int index)
	{
		m_die_poses[index].Clear ();
		m_arrays[index].Clear ();
		protocol.map.map_data_sub mdsub = new protocol.map.map_data_sub ();
		mdsub.x_num = 20;
		mdsub.y_num = 10;
		mdsub.qd_y = 2;
		mdsub.zd_y = 2;
		mdsub.map_theme = Random.Range(1, 5);
		m_map_data.maps.Add (mdsub);
		for (int i = 0; i < mdsub.y_num; ++i)
		{
			List<s_t_mission_sub> arr = new List<s_t_mission_sub>();
			for (int j = 0; j < mdsub.x_num; ++j)
			{
				s_t_mission_sub sub = new s_t_mission_sub();
				if (i < 2)
				{
					sub.type = 1;
				}
				else
				{
					sub.type = 0;
				}
				arr.Add(sub);
			}
			m_arrays[index].Add(arr);
		}
		m_arrays[index][2][1].type = 1000;
		m_arrays[index][2][18].type = 1001;
	}

	void reset_mission()
	{
		m_die_poses.Clear ();
		m_arrays.Clear ();
		m_map_data = new protocol.map.map_data1 ();
		m_map_data.mode = 0;
		m_map_data.time = 300;
		m_map_data.no_music = 0;
		m_map_data.end_area = 0;
		for (int i = 0; i < 3; ++i)
		{
			m_die_poses.Add(new List<mario_point>());
			m_arrays.Add(new List<List<s_t_mission_sub>>());
		}
	}

	void new_mission()
	{
		reset_mission ();
		for (int i = 0; i < 3; ++i)
		{
			new_world(i);
		}
	}

	public bool load_mission(int id, byte[] mapdata, List<int> x, List<int> y)
	{
		m_map_id = id;
		reset_mission ();
		if (x != null)
		{
			for (int i = 0; i < x.Count; ++i)
			{
				int cha = x[i] / 10000000;
				m_die_poses[cha].Add(new mario_point(x[i], y[i]));
			}
		}
		if (mapdata.Length == 0)
		{
			new_mission();
		}
		else
		{
			try
			{
				byte[] arr = utils.Decompress(mapdata);
				protocol.map.map_data tmp = net_http._instance.parse_packet<protocol.map.map_data> (arr);
				protocol.map.map_data_sub sub = new protocol.map.map_data_sub();
				sub.array = tmp.array;
				sub.x_num = tmp.x_num;
				sub.y_num = tmp.y_num;
				sub.qd_y = tmp.qd_y;
				sub.zd_y = tmp.zd_y;
				sub.map_theme = tmp.map_theme;
				m_map_data.map_ver = tmp.map_ver;
				m_map_data.mode = tmp.mode;
				m_map_data.time = tmp.time;
				m_map_data.no_music = tmp.no_music;
				m_map_data.end_area = 0;
				m_map_data.maps.Add(sub);
			}
			catch (System.Exception)
			{
				try
				{
					byte[] arr = utils.Decompress(mapdata);
					m_map_data = net_http._instance.parse_packet<protocol.map.map_data1> (arr);
				}
				catch (System.Exception)
				{
					mario._instance.show_tip(get_language_string("game_data_dtjx"));
					new_mission();
					return false;
				}
			}

			try
			{
				if (m_map_data.map_ver > m_self_map_ver)
				{
					mario._instance.show_tip(get_language_string("game_data_dtbb"));
					new_mission();
					return false;
				}
				for (int m = 0; m < 3; ++m)
				{
					if (m >= m_map_data.maps.Count)
					{
						new_world(m);
						continue;
					}
					MemoryStream ms = new MemoryStream(m_map_data.maps[m].array);
					byte[] b = new byte[4];
					for (int j = 0; j < m_map_data.maps[m].y_num; ++j)
					{
						List<s_t_mission_sub> subs = new List<s_t_mission_sub>();
						for (int i = 0; i < m_map_data.maps[m].x_num; ++i)
						{
							s_t_mission_sub sub = new s_t_mission_sub();
							ms.Read(b, 0, b.Length);
							sub.type = System.BitConverter.ToInt32(b, 0);
							if (sub.type != 0)
							{
								for (int k = 0; k < 4; ++k)
								{
									ms.Read(b, 0, b.Length);
									sub.param[k] = System.BitConverter.ToInt32(b, 0);
								}
							}
							s_t_unit t_unit = get_t_unit(sub.type);
							if (t_unit != null)
							{
								if (mario._instance.m_self.m_review == 1 && t_unit.review == 1)
								{
									sub.type = 0;
								}
							}
							subs.Add(sub);
						}
						m_arrays[m].Add(subs);
					}
				}
			}
			catch (System.Exception)
			{
				mario._instance.show_tip(get_language_string("game_data_dtjx"));
				new_mission();
				return false;
			}
		}
		return true;
	}

	public void get_mission_data(ref byte[] data, ref byte[] url)
	{
		m_map_data.map_ver = m_self_map_ver;
		MemoryStream ms;
		byte[] b = new byte[4];
		for (int m = 0; m < 3; ++m)
		{
			ms = new MemoryStream();
			for (int j = 0; j < m_map_data.maps[m].y_num; ++j)
			{
				for (int i = 0; i < m_map_data.maps[m].x_num; ++i)
				{
					b = System.BitConverter.GetBytes(m_arrays[m][j][i].type);
					ms.Write(b, 0, b.Length);
					for (int k = 0; k < m_arrays[m][j][i].param.Count; ++k)
					{
						b = System.BitConverter.GetBytes(m_arrays[m][j][i].param[k]);
						ms.Write(b, 0, b.Length);
					}
				}
			}
			m_map_data.maps[m].array = ms.ToArray();
		}

		ms = new MemoryStream();
		ProtoBuf.Serializer.Serialize(ms, m_map_data);
		data = utils.Compress (ms.ToArray());

		protocol.map.map_url murl = new protocol.map.map_url();
		murl.map_theme = m_map_data.maps[0].map_theme;
		int cs = m_map_data.maps[0].qd_y - 2;
		int zs = m_map_data.maps[0].qd_y + 8;
		if (zs > m_map_data.maps[0].y_num)
		{
			zs = m_map_data.maps[0].y_num;
			cs = zs - 10;
		}
		
		ms = new MemoryStream();
		for (int j = cs; j < zs; ++j)
		{
			for (int i = 0; i < 15; ++i)
			{
				b = System.BitConverter.GetBytes(m_arrays[0][j][i].type);
				ms.Write(b, 0, b.Length);
			}
		}
		murl.array = ms.ToArray();
		
		ms = new MemoryStream();
		ProtoBuf.Serializer.Serialize(ms, murl);
		url = utils.Compress (ms.ToArray());
	}

	public void save_mission ()
	{
		byte[] data = null;
		byte[] url = null;
		get_mission_data (ref data, ref url);

		protocol.game.cmsg_save_map msg = new protocol.game.cmsg_save_map();
		msg.id = m_map_id;
		msg.mapdata = data;
		msg.url = url;
		net_http._instance.send_msg<protocol.game.cmsg_save_map>(opclient_t.OPCODE_SAVE_MAP, msg);
	}

	public void save_mission_ex ()
	{
		byte[] data = null;
		byte[] url = null;
		get_mission_data (ref data, ref url);
		
		protocol.game.cmsg_complete_guide msg = new protocol.game.cmsg_complete_guide();
		msg.data = data;
		msg.url = url;
		net_http._instance.send_msg<protocol.game.cmsg_complete_guide>(opclient_t.OPCODE_COMPLETE_GUIDE, msg);
	}

	public Texture2D mission_to_texture(byte[] data)
	{
		if (data.Length == 0)
		{
			return Resources.Load("texture/back/back") as Texture2D;
		}
		try
		{
			byte[] arr = utils.Decompress(data);
			protocol.map.map_url mu = net_http._instance.parse_packet<protocol.map.map_url> (arr);
			Texture2D texture = new Texture2D (360, 240, TextureFormat.RGBA32, false);
			Texture2D bt = Resources.Load("texture/back/back_" + mu.map_theme.ToString()) as Texture2D;
			texture.SetPixels(0, 0, 360, 240, bt.GetPixels());

			MemoryStream ms = new MemoryStream(mu.array);
			byte[] b = new byte[4];
			for (int j = 0; j < 10; ++j)
			{
				for (int i = 0; i < 15; ++i)
				{
					ms.Read(b, 0, b.Length);
					int type = System.BitConverter.ToInt32(b, 0);
					if (type > 0)
					{
						s_t_unit unit = game_data._instance.get_t_unit(type);
						if (unit == null)
						{
							continue;
						}
						if (mario._instance.m_self.m_review == 1 && unit.review == 1)
						{
							continue;
						}
						Texture2D tt = null;
						if (unit.kfg == 1)
						{
							tt = Resources.Load("texture/" + unit.icon + "_" + mu.map_theme.ToString()) as Texture2D;
						}
						else
						{
							tt = Resources.Load("texture/" + unit.icon) as Texture2D;
						}
						Color[] c = tt.GetPixels();
						for (int y = 0; y < 24; ++y)
						{
							for (int x = 0; x < 24; ++x)
							{
								Color cc = c[y * 24 + x];
								Color cc1 = texture.GetPixel(i * 24 + x, j * 24 + y);
								cc.r = cc.r * cc.a + cc1.r * (1 - cc.a);
								cc.g = cc.g * cc.a + cc1.g * (1 - cc.a);
								cc.b = cc.b * cc.a + cc1.b * (1 - cc.a);
								cc.a = 1;
								texture.SetPixel(i * 24 + x, j * 24 + y, cc);
							}
						}
					}
				}
			}
			texture.Apply (false);

			return texture;
		}
		catch (System.Exception)
		{
			return Resources.Load("texture/back/back") as Texture2D;
		}
	}

	public void load_inputs(byte[] video_data)
	{
		m_map_inputs = new List<int> ();
		try
		{
			byte[] arr = utils.Decompress (video_data);
			int count = arr.Length / 4 - 10;
			MemoryStream ms = new MemoryStream(arr);
			byte[] b = new byte[4];
			// 预留10个
			for (int i = 0; i < 10; ++i)
			{
				ms.Read(b, 0, b.Length);
			}
			for (int i = 0; i < count; ++i)
			{
				ms.Read(b, 0, b.Length);
				int a = System.BitConverter.ToInt32(b, 0);
				m_map_inputs.Add(a);
			}
		}
		catch (System.Exception)
		{
			m_map_inputs.Clear();
		}
	}

	public byte[] save_inputs(List<int> input)
	{
		MemoryStream ms = new MemoryStream();
		/// 预留10个
		for (int i = 0; i < 10; ++i)
		{
			byte[] b = System.BitConverter.GetBytes(0);
			ms.Write(b, 0, b.Length);
		}
		for (int i = 0; i < input.Count; ++i)
		{
			byte[] b = System.BitConverter.GetBytes(input[i]);
			ms.Write(b, 0, b.Length);
		}
		byte[] data = utils.Compress (ms.ToArray());
		return data;
	}

	public string get_map_music(int index)
	{
		s_t_fg t_fg = get_t_fg (m_map_data.maps[index].map_theme);
		if (t_fg == null)
		{
			return "";
		}
		return t_fg.music;
	}

	public bool get_csm(int iw, int ix, int iy, ref int w, ref int x, ref int y)
	{
		int num = m_arrays[iw][iy][ix].param[0];
		int d = 1 - m_arrays[iw][iy][ix].param[1];

		for (int m = 0; m < m_arrays.Count; ++m)
		{
			for (int j = 0; j < m_map_data.maps[m].y_num; ++j)
			{
				for (int i = 0; i < m_map_data.maps[m].x_num; ++i)
				{
					if (m_arrays[m][j][i].type == utils.g_csm && m_arrays[m][j][i].param[0] == num && m_arrays[m][j][i].param[1] == d)
					{
						w = m;
						x = i;
						y = j;
						return true;
					}
				}
			}
		}
		return false;
	}

	public int get_new_csm ()
	{
		HashSet<int> hs = new HashSet<int> ();
		for (int m = 0; m < m_arrays.Count; ++m)
		{
			for (int j = 0; j < m_map_data.maps[m].y_num; ++j)
			{
				for (int i = 0; i < m_map_data.maps[m].x_num; ++i)
				{
					if (m_arrays[m][j][i].type == utils.g_csm)
					{
						hs.Add(m_arrays[m][j][i].param[0]);
					}
				}
			}
		}
		for (int i = 0; i < 15; ++i)
		{
			if (!hs.Contains(i))
			{
				return i;
			}
		}
		return -1;
	}

	public int get_unit_num(int world, int type)
	{
		int num = 0;
		for (int j = 0; j < m_map_data.maps[world].y_num; ++j)
		{
			for (int i = 0; i < m_map_data.maps[world].x_num; ++i)
			{
				if (m_arrays[world][j][i].type == type)
				{
					num++;
				}
			}
		}
		return num;
	}

	///////////////////////////////////////////////

	public Dictionary<string, s_t_language> m_t_language = new Dictionary<string, s_t_language>();

	public string get_language_string(string id)
	{
		if (!m_t_language.ContainsKey(id))
		{
			return "";
		}
		if (m_lang == e_language.el_chinese)
		{
			return m_t_language [id].zw;
		}
		return m_t_language [id].ew;
	}

	public Dictionary<int, s_t_unit> m_t_unit = new Dictionary<int, s_t_unit>();
	public List<int> m_unit_sites = new List<int> ();
	public int m_unit_num = 0;
	
	public s_t_unit get_t_unit(int id)
	{
		if (!m_t_unit.ContainsKey(id))
		{
			return null;
		}
		return m_t_unit [id];
	}

	public s_t_unit get_t_unit_by_site(int site)
	{
		if (site < 0 || site >= m_unit_sites.Count)
		{
			return null;
		}
		int id = m_unit_sites [site];
		if (!m_t_unit.ContainsKey(id))
		{
			return null;
		}
		return m_t_unit [id];
	}

	public Dictionary<int, string> m_t_error = new Dictionary<int, string>();

	public string get_t_error(int id)
	{
		if (!m_t_error.ContainsKey(id))
		{
			return "";
		}
		return m_t_error [id];
	}
	
	public Dictionary<int, s_t_view_map> m_t_view_map = new Dictionary<int, s_t_view_map>();
	
	public s_t_view_map get_t_view_map(int id)
	{
		if (!m_t_view_map.ContainsKey(id))
		{
			return null;
		}
		return m_t_view_map [id];
	}

	public Dictionary<int, s_t_view_title> m_t_view_title = new Dictionary<int, s_t_view_title>();
	
	public s_t_view_title get_t_view_title(int id)
	{
		if (!m_t_view_title.ContainsKey(id))
		{
			return null;
		}
		return m_t_view_title [id];
	}

	public Dictionary<int, string> m_t_touxiang = new Dictionary<int, string>();
	
	public string get_t_touxiang(int id)
	{
		if (!m_t_touxiang.ContainsKey(id))
		{
			return "";
		}
		return m_t_touxiang [id];
	}
	
	public Dictionary<string, string> m_t_guojia = new Dictionary<string, string>();
	
	public string get_t_guojia(string code)
	{
		if (!m_t_guojia.ContainsKey(code))
		{
			return "gq_000";
		}
		return m_t_guojia [code];
	}

	public Dictionary<int, s_t_exp> m_t_exp = new Dictionary<int, s_t_exp>();
	public Dictionary<int, int> m_t_zm = new Dictionary<int, int>();
	
	public s_t_exp get_t_exp(int level)
	{
		if (!m_t_exp.ContainsKey(level))
		{
			return null;
		}
		return m_t_exp [level];
	}

	public int get_zm(int zm)
	{
		if (!m_t_zm.ContainsKey(zm))
		{
			return 0;
		}
		return m_t_zm [zm];
	}

	public Dictionary<int, s_t_job_exp> m_t_job_exp = new Dictionary<int, s_t_job_exp>();
	
	public s_t_job_exp get_t_job_exp(int level)
	{
		if (!m_t_job_exp.ContainsKey(level))
		{
			return null;
		}
		return m_t_job_exp [level];
	}

	public Dictionary<int, s_t_shop> m_t_shop = new Dictionary<int, s_t_shop>();
	
	public s_t_shop get_t_shop(int id)
	{
		if (!m_t_shop.ContainsKey(id))
		{
			return null;
		}
		return m_t_shop [id];
	}
	
	public Dictionary<int, s_t_fg> m_t_fg = new Dictionary<int, s_t_fg>();
	public int m_fg_num;
	public s_t_fg get_t_fg(int id)
	{
		if (!m_t_fg.ContainsKey(id))
		{
			return null;
		}
		return m_t_fg [id];
	}

	public HashSet<int> m_t_map = new HashSet<int>();
	public bool is_test_map(int id)
	{
		return m_t_map.Contains (id);
	}

	public List<s_t_br> m_t_br = new List<s_t_br>();
	public s_t_br get_t_br(int id)
	{
		if (id <= 0)
		{
			return null;
		}
		if (id > m_t_br.Count)
		{
			return m_t_br[m_t_br.Count - 1];
		}

		return m_t_br[id - 1];
	}

	public Dictionary<int, s_t_key> m_t_key = new Dictionary<int, s_t_key>();
	public s_t_key get_t_key(int kc)
	{
		if (!m_t_key.ContainsKey(kc))
		{
			return null;
		}
		return m_t_key [kc];
	}

	public void init()
	{
		load_native ();
		dbc tdbc = new dbc ();
		tdbc.load_txt ("t_language");
		for (int i = 0; i < tdbc.get_y(); ++i) {
			s_t_language t_l = new s_t_language ();
			t_l.id = tdbc.get (0, i);
			t_l.zw = tdbc.get (1, i);
			t_l.ew = tdbc.get (2, i);
			m_t_language [t_l.id] = t_l;
		}

		tdbc.load_txt ("t_unit");
		for (int i = 0; i < tdbc.get_y(); ++i) {
			s_t_unit t_unit = new s_t_unit ();
			t_unit.id = int.Parse (tdbc.get (0, i));
			t_unit.name = tdbc.get (1, i);
			t_unit.icon = tdbc.get (2, i);
			t_unit.res = tdbc.get (3, i);
			t_unit.yc = int.Parse (tdbc.get (4, i));
			t_unit.kfg = int.Parse (tdbc.get (5, i));
			t_unit.is_static = int.Parse (tdbc.get (6, i));
			t_unit.fwt = int.Parse (tdbc.get (7, i));
			t_unit.review = int.Parse (tdbc.get (8, i));
			t_unit.is_sw = int.Parse (tdbc.get (9, i));
			t_unit.max_num = int.Parse (tdbc.get (10, i));
			m_t_unit [t_unit.id] = t_unit;

			if (t_unit.yc == 0) {
				m_unit_sites.Add (t_unit.id);
				m_unit_num++;
			}
		}

		tdbc.load_txt ("t_error");
		for (int i = 0; i < tdbc.get_y(); ++i) {
			int id = int.Parse (tdbc.get (0, i));
			string des = get_language_string (tdbc.get (1, i));
			m_t_error [id] = des;
		}

		tdbc.load_txt ("t_view_map");
		for (int i = 0; i < tdbc.get_y(); ++i) {
			s_t_view_map t_view = new s_t_view_map ();
			t_view.id = int.Parse (tdbc.get (0, i));
			t_view.name = get_language_string (tdbc.get (1, i));
			t_view.icon = tdbc.get (2, i);
			m_t_view_map [t_view.id] = t_view;
		}

		tdbc.load_txt ("t_view_title");
		for (int i = 0; i < tdbc.get_y(); ++i) {
			s_t_view_title t_view = new s_t_view_title ();
			t_view.id = int.Parse (tdbc.get (0, i));
			t_view.name = get_language_string (tdbc.get (1, i));
			t_view.icon = tdbc.get (2, i);
			m_t_view_title [t_view.id] = t_view;
		}

		tdbc.load_txt ("t_touxiang");
		for (int i = 0; i < tdbc.get_y(); ++i) {
			int id = int.Parse (tdbc.get (0, i));
			string icon = tdbc.get (1, i);
			m_t_touxiang [id] = icon;
		}

		tdbc.load_txt ("t_guojia");
		for (int i = 0; i < tdbc.get_y(); ++i) {
			string code = tdbc.get (0, i);
			string icon = tdbc.get (2, i);
			m_t_guojia [code] = icon;
		}

		tdbc.load_txt ("t_exp");
		int zm = 0;
		for (int i = 0; i < tdbc.get_y(); ++i) {
			s_t_exp t_exp = new s_t_exp ();
			t_exp.level = int.Parse (tdbc.get (0, i));
			t_exp.exp = int.Parse (tdbc.get (1, i));
			t_exp.zm = int.Parse (tdbc.get (2, i));
			t_exp.icon = tdbc.get (3, i);
			t_exp.max_exp = int.Parse (tdbc.get (4, i));
			m_t_exp [t_exp.level] = t_exp;
			if (t_exp.zm != zm) {
				zm = t_exp.zm;
				m_t_zm [zm] = t_exp.level;
			}
		}

		tdbc.load_txt ("t_job_exp");
		for (int i = 0; i < tdbc.get_y(); ++i) {
			s_t_job_exp t_job_exp = new s_t_job_exp ();
			t_job_exp.level = int.Parse (tdbc.get (0, i));
			t_job_exp.exp = int.Parse (tdbc.get (1, i));
			m_t_job_exp [t_job_exp.level] = t_job_exp;
		}

		tdbc.load_txt ("t_shop");
		for (int i = 0; i < tdbc.get_y(); ++i) {
			s_t_shop t_shop = new s_t_shop ();
			t_shop.id = int.Parse (tdbc.get (0, i));
			t_shop.slot = int.Parse (tdbc.get (1, i));
			t_shop.name = get_language_string (tdbc.get (2, i));
			t_shop.type = int.Parse (tdbc.get (3, i));
			t_shop.price = int.Parse (tdbc.get (4, i));
			t_shop.price_my = float.Parse (tdbc.get (5, i));
			t_shop.icon = tdbc.get (6, i);
			t_shop.db = tdbc.get (7, i);
			t_shop.def = int.Parse (tdbc.get (8, i));
			t_shop.code = tdbc.get (9, i);
			t_shop.desc = get_language_string (tdbc.get (10, i));
			m_t_shop [t_shop.id] = t_shop;
		}

		tdbc.load_txt ("t_fg");
		for (int i = 0; i < tdbc.get_y(); ++i) {
			s_t_fg t_fg = new s_t_fg ();
			t_fg.id = int.Parse (tdbc.get (0, i));
			t_fg.name = get_language_string (tdbc.get (1, i));
			t_fg.tj = int.Parse (tdbc.get (2, i));
			t_fg.desc = get_language_string (tdbc.get (3, i));
			t_fg.music = tdbc.get (4, i);
			m_t_fg [t_fg.id] = t_fg;
			m_fg_num++;
		}

		tdbc.load_txt ("t_map");
		for (int i = 0; i < tdbc.get_y(); ++i) {
			int id = int.Parse (tdbc.get (0, i));
			m_t_map.Add (id);
		}

		tdbc.load_txt ("t_br");
		for (int i = 0; i < tdbc.get_y(); ++i) {
			s_t_br t_br = new s_t_br ();
			t_br.id = int.Parse (tdbc.get (0, i));
			t_br.name = get_language_string (tdbc.get (2, i));
			t_br.num = int.Parse (tdbc.get (3, i));
			t_br.desc = get_language_string (tdbc.get (4, i));
			t_br.unlock = get_language_string (tdbc.get (6, i));
			m_t_br.Add (t_br);
		}

		tdbc.load_txt ("t_key");
		for (int i = 0; i < tdbc.get_y(); ++i) {
			s_t_key t_key = new s_t_key ();
			t_key.code = int.Parse (tdbc.get (0, i));
			t_key.name = tdbc.get (1, i);
			m_t_key.Add (t_key.code, t_key);
		}

		new_mission ();

		if (Application.isEditor)
		{
			#if STEAM
			m_channel = "win_steam";
			#else
			m_channel = "";
			#endif
		}
		else
		{
			#if UNITY_ANDROID
			LJSDK._instance.init_channel ();
			m_channel = LJSDK._instance.m_channel;
			#elif UNITY_IPHONE
			m_channel = "IOS_yymoon";
			#elif UNITY_WEBPLAYER 
			LJSDK._instance.init_channel ();
			return;
			#elif UNITY_STANDALONE_WIN
			#if STEAM
			m_channel = "win_steam";
			#else
			m_channel = "win_yymoon";
			#endif
			#else
			m_channel = "";
			#endif
		}
		init_pt_ver ();
	}

	public void init_pt_ver()
	{
		m_pt_ver = m_channel + "_" + m_ver;
		mario._instance.ginit_callbak ();
	}

	public string get_url_end()
	{
		return "?t=" + UnityEngine.Random.Range (0, 100000);
	}
}

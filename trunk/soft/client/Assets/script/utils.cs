using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;

[Serializable]
public class mario_point
{
	public int x = 0;
	public int y = 0;
	
	public mario_point() {}
	
	public mario_point(int x, int y)
	{
		this.x = x;
		this.y = y;
	}
	
	public void set(int x, int y)
	{
		this.x = x;
		this.y = y;
	}
}

[Serializable]
public class mario_rect
{
	public int x = 0;
	public int y = 0;
	public int w = 0;
	public int h = 0;
	
	public mario_rect() {}
	
	public mario_rect(int x, int y, int w, int h)
	{
		this.x = x;
		this.y = y;
		this.w = w;
		this.h = h;
	}
	
	public void set(int x, int y, int w, int h)
	{
		this.x = x;
		this.y = y;
		this.w = w;
		this.h = h;
	}
}

[Serializable]
public class mario_bound
{
	public int left = 0;
	public int right = 0;
	public int top = 0;
	public int bottom = 0;
	public int left_div = 0;
	public int right_div = 0;
	public int top_div = 0;
	public int bottom_div = 0;
	public bool bs_yx = true;
	[NonSerialized]
	private List<mario_bound> bs;
	
	public mario_bound()
	{
	}

	public mario_bound(int l, int r, int t, int b)
	{
		set_bound (l, r, t, b, true);
	}

	public mario_bound(int l, int r, int t, int b, bool yx)
	{
		set_bound (l, r, t, b, yx);
	}

	public void set_bound(int l, int r, int t, int b, bool yx)
	{
		left = l;
		right = r;
		top = t;
		bottom = b;
		bs_yx = yx;
	}

	public List<mario_bound> carve(mario_bound b)
	{
		int mid_x = (left + right) / 2;
		int mid_y = (top + bottom) / 2;
		if (bs == null)
		{
			bs = new List<mario_bound>();
			bs.Add(new mario_bound(0, 0, 0, 0, false));
			bs.Add(new mario_bound(0, 0, 0, 0, false));
			bs.Add(new mario_bound(0, 0, 0, 0, false));
			bs.Add(new mario_bound(0, 0, 0, 0, false));
		}
		if (mid_y <= b.top && mid_y > b.bottom)
		{
			if (mid_x > b.right)
			{
				bs[0].set_bound(b.left, b.right, b.top, mid_y, true);
				bs[1].set_bound(b.left, b.right, mid_y - 1, b.bottom, true);
				bs[2].bs_yx = false;
				bs[3].bs_yx = false;
			}
			else if (mid_x <= b.left)
			{
				bs[0].set_bound(b.left, b.right, b.top, mid_y, true);
				bs[1].set_bound(b.left, b.right, mid_y - 1, b.bottom, true);
				bs[2].bs_yx = false;
				bs[3].bs_yx = false;
			}
			else
			{
				bs[0].set_bound(b.left, mid_x - 1, b.top, mid_y, true);
				bs[1].set_bound(mid_x, b.right, b.top, mid_y, true);
				bs[2].set_bound(b.left, mid_x - 1, mid_y - 1, b.bottom, true);
				bs[3].set_bound(mid_x, b.right, mid_y - 1, b.bottom, true);
			}
		}
		else if (mid_y > b.top)
		{
			if (mid_x > b.right)
			{
			}
			else if (mid_x <= b.left)
			{
			}
			else
			{
				bs[0].set_bound(b.left, mid_x - 1, b.top, b.bottom, true);
				bs[1].set_bound(mid_x, b.right, b.top, b.bottom, true);
				bs[2].bs_yx = false;
				bs[3].bs_yx = false;
			}
		}
		else if (mid_y <= b.bottom)
		{
			if (mid_x > b.right)
			{
			}
			else if (mid_x <= b.left)
			{
			}
			else
			{
				bs[0].set_bound(b.left, mid_x - 1, b.top, b.bottom, true);
				bs[1].set_bound(mid_x, b.right, b.top, b.bottom, true);
				bs[2].bs_yx = false;
				bs[3].bs_yx = false;
			}
		}
		return bs;
	}
}

public class mario_qtree
{
	public const int max_object = 1;
	public const int max_level = 9;
	public int level;
	public List<List<mario_obj>> objects = new List<List<mario_obj>>();
	public mario_bound bound = new mario_bound();
	public mario_qtree parent = null;
	public List<mario_qtree> nodes = new List<mario_qtree>();

	public mario_qtree()
	{
		parent = null;
		level = 1;
		bound = new mario_bound (-20480, 143360, 143360, -20480);
		for (int i = 0; i < 4; ++i)
		{
			nodes.Add(null);
		}
		for (int i = (int)mario_obj.mario_type.mt_null; i < (int)mario_obj.mario_type.mt_end; ++i)
		{
			objects.Add(new List<mario_obj>());
		}
	}

	public mario_qtree(mario_qtree p, int l, mario_bound b)
	{
		parent = p;
		level = l;
		bound = b;
		for (int i = 0; i < 4; ++i)
		{
			nodes.Add(null);
		}
		for (int i = (int)mario_obj.mario_type.mt_null; i < (int)mario_obj.mario_type.mt_end; ++i)
		{
			objects.Add(new List<mario_obj>());
		}
	}

	public void split()
	{
		int mid_x = (bound.left + bound.right) / 2;
		int mid_y = (bound.top + bound.bottom) / 2;
		nodes [0] = new mario_qtree (this, level + 1, new mario_bound (mid_x, bound.right, bound.top, mid_y));
		nodes [1] = new mario_qtree (this, level + 1, new mario_bound (bound.left, mid_x, bound.top, mid_y));
		nodes [2] = new mario_qtree (this, level + 1, new mario_bound (bound.left, mid_x, mid_y, bound.bottom));
		nodes [3] = new mario_qtree (this, level + 1, new mario_bound (mid_x, bound.right, mid_y, bound.bottom));
	}

	public int get_index(mario_bound b)
	{
		int index = -1;
		int mid_x = (bound.left + bound.right) / 2;
		int mid_y = (bound.top + bound.bottom) / 2;
		bool istop = b.bottom >= mid_y && b.top < bound.top;
		bool isbottom = b.top < mid_y && b.bottom >= bound.bottom;
		bool isleft = b.left >= bound.left && b.right < mid_x;
		bool isright = b.right < bound.right && b.left >= mid_x;
		if (istop)
		{
			if (isleft)
			{
				index = 1;
			}
			else if (isright)
			{
				index = 0;
			}
		}
		else if (isbottom)
		{
			if (isleft)
			{
				index = 2;
			}
			else if (isright)
			{
				index = 3;
			}
		}
		return index;
	}

	public void insert(mario_obj obj)
	{
		if (nodes[0] != null)
		{
			int index = get_index(obj.m_bound);
			if (index != -1)
			{
				nodes[index].insert(obj);
				return;
			}
		}

		List<mario_obj> sub_objs = objects [(int)obj.m_type];
		sub_objs.Add (obj);
		obj.m_qtree = this;

		if (nodes[0] == null && sub_objs.Count > max_object && level < max_level)
		{
			split();
			for (int i = sub_objs.Count - 1; i >= 0; i--)
			{
				mario_obj tobj = sub_objs[i];
				int index = get_index(tobj.m_bound);
				if (index != -1)
				{
					sub_objs.RemoveAt(i);
					nodes[index].insert(tobj);
				}
			}
		}
	}

	public bool empty()
	{
		for (int i = 0; i < objects.Count; ++i)
		{
			if (objects[i].Count > 0)
			{
				return false;
			}
		}
		if (nodes[0] != null)
		{
			return false;
		}
		return true;
	}

	public void remove(mario_obj obj)
	{
		bool flag = false;
		List<mario_obj> sub_objs = objects [(int)obj.m_type];
		for (int i = 0 ; i < sub_objs.Count; ++i)
		{
			if (sub_objs[i] == obj)
			{
				flag = true;
				break;
			}
		}
		if (!flag)
		{
			return;
		}
		sub_objs.Remove(obj);
		if (empty() && parent != null)
		{
			parent.check_combine();
		}
	}

	public void check_combine()
	{
		if (nodes[0] == null)
		{
			return;
		}
		for (int i = 0; i < nodes.Count; ++i)
		{
			if (!nodes[i].empty())
			{
				return;
			}
		}
		for (int i = 0; i < nodes.Count; ++i)
		{
			nodes[i] = null;
		}
		if (empty() && parent != null)
		{
			parent.check_combine();
		}
	}

	public void retrive(mario_obj obj, ref List<mario_obj> objs, int type)
	{
		retrive_ex (obj.m_bound, ref objs, type);
		mario_qtree t = obj.m_qtree.parent;
		while (t != null)
		{
			objs.AddRange(t.objects[type]);
			t = t.parent;
		}
	}

	public void retrive_floor(mario_obj obj, ref List<mario_obj> objs)
	{
		retrive_ex_floor (obj.get_floor_bound(), ref objs);
		mario_qtree t = obj.m_qtree.parent;
		while (t != null)
		{
			objs.AddRange(t.objects[(int)mario_obj.mario_type.mt_block]);
			objs.AddRange(t.objects[(int)mario_obj.mario_type.mt_block1]);
			objs.AddRange(t.objects[(int)mario_obj.mario_type.mt_attack]);
			objs.AddRange(t.objects[(int)mario_obj.mario_type.mt_charater]);
			t = t.parent;
		}
	}

	public void retrive_left(mario_obj obj, ref List<mario_obj> objs)
	{
		retrive_ex_fx (obj.get_left_bound(), ref objs);
		mario_qtree t = obj.m_qtree.parent;
		while (t != null)
		{
			objs.AddRange(t.objects[(int)mario_obj.mario_type.mt_block]);
			objs.AddRange(t.objects[(int)mario_obj.mario_type.mt_block1]);
			t = t.parent;
		}
	}

	public void retrive_right(mario_obj obj, ref List<mario_obj> objs)
	{
		retrive_ex_fx (obj.get_right_bound(), ref objs);
		mario_qtree t = obj.m_qtree.parent;
		while (t != null)
		{
			objs.AddRange(t.objects[(int)mario_obj.mario_type.mt_block]);
			objs.AddRange(t.objects[(int)mario_obj.mario_type.mt_block1]);
			t = t.parent;
		}
	}

	public void retrive_ex(mario_bound b, ref List<mario_obj> objs, int type)
	{
		if (nodes[0] != null)
		{
			int index = get_index (b);
			if (index != -1)
			{
				nodes[index].retrive_ex(b, ref objs, type);
			}
			else
			{
				List<mario_bound> arr = bound.carve(b);
				for (int i = 0; i < arr.Count; ++i)
				{
					if (!arr[i].bs_yx)
					{
						continue;
					}
					index = get_index (arr[i]);
					if (index != -1)
					{
						nodes[index].retrive_ex(arr[i], ref objs, type);
					}
				}
			}
		}
		objs.AddRange (objects[type]);
	}

	public void retrive_ex_floor(mario_bound b, ref List<mario_obj> objs)
	{
		if (nodes[0] != null)
		{
			int index = get_index (b);
			if (index != -1)
			{
				nodes[index].retrive_ex_floor(b, ref objs);
			}
			else
			{
				List<mario_bound> arr = bound.carve(b);
				for (int i = 0; i < arr.Count; ++i)
				{
					index = get_index (arr[i]);
					if (index != -1)
					{
						nodes[index].retrive_ex_floor(arr[i], ref objs);
					}
				}
			}
		}
		objs.AddRange(objects[(int)mario_obj.mario_type.mt_block]);
		objs.AddRange(objects[(int)mario_obj.mario_type.mt_block1]);
		objs.AddRange(objects[(int)mario_obj.mario_type.mt_attack]);
		objs.AddRange(objects[(int)mario_obj.mario_type.mt_charater]);
	}

	public void retrive_ex_fx(mario_bound b, ref List<mario_obj> objs)
	{
		if (nodes[0] != null)
		{
			int index = get_index (b);
			if (index != -1)
			{
				nodes[index].retrive_ex_fx(b, ref objs);
			}
			else
			{
				List<mario_bound> arr = bound.carve(b);
				for (int i = 0; i < arr.Count; ++i)
				{
					index = get_index (arr[i]);
					if (index != -1)
					{
						nodes[index].retrive_ex_fx(arr[i], ref objs);
					}
				}
			}
		}
		objs.AddRange(objects[(int)mario_obj.mario_type.mt_block]);
		objs.AddRange(objects[(int)mario_obj.mario_type.mt_block1]);
	}
}

public class utils
{
	public static int g_grid_size = 640;
	public static int g_active_x = 10;
	public static int g_active_y = 6;
	public static int g_start_x = 12;
	public static int g_start_y
	{
		get 
		{
			if (game_data._instance.m_map_data.no_music == 0)
			{
				return 10; 
			}
			else
			{
				return 30;
			}
		}
	}
	public static int g_del_x = 20;
	public static int g_del_y
	{
		get 
		{
			if (game_data._instance.m_map_data.no_music == 0)
			{
				return 12; 
			}
			else
			{
				return 32;
			}
		}
	}
	public static int g_load_size = 2;
	public static int g_height = 6400;
	public static int g_roll_y = 3200;
	public static int g_max_x = 200;
	public static int g_max_y = 30;
	public static int g_min_x = 20;
	public static int g_min_y = 10;
	public static int g_yinfu = 110;
	public static int g_ryqiu = 111;
	public static int g_csg = 112;
	public static int g_cspt = 113;
	public static int g_hg = 120;
	public static int g_csm = 122;
	public static int g_g = 15;

	public static int get_map_exp(int tr, int rs)
	{
		if (rs < 100)
		{
			return 2;
		}
		float tgl = (float)tr / rs;
		if (tgl < 0.001f)
		{
			return 11;
		}
		else if (tgl < 0.01f)
		{
			return 9;
		}
		else if (tgl < 0.05f)
		{
			return 7;
		}
		else if (tgl < 0.1f)
		{
			return 6;
		}
		else if (tgl < 0.2f)
		{
			return 5;
		}
		else if (tgl < 0.4f)
		{
			return 4;
		}
		else if (tgl < 0.5f)
		{
			return 3;
		}
		return 2;
	}

	public static int get_map_nd(int tr, int rs)
	{
		float tgl = (float)tr / rs;
		if (rs > 10000)
		{
			if (tgl < 0.0005f)
			{
				return 4;
			}
			else if (tgl < 0.005f)
			{
				return 3;
			}
			else if (tgl < 0.05f)
			{
				return 2;
			}
		}
		else if (rs > 1000)
		{
			if (tgl < 0.005f)
			{
				return 3;
			}
			else if (tgl < 0.05f)
			{
				return 2;
			}
		}
		else if (rs > 100)
		{
			if (tgl < 0.05f)
			{
				return 2;
			}
		}
		else
		{
			return 0;
		}
		return 1;
	}

	public static TweenPosition add_pos_anim(GameObject obj, float speed, Vector3 pos, float delay)
	{
		Vector3 _from = obj.transform.localPosition;
		Vector3 _to = obj.transform.localPosition + pos;
		
		TweenPosition _effect = TweenPosition.Begin(obj,speed,obj.transform.localPosition);
		
		_effect.method = UITweener.Method.EaseInOut;
		_effect.from = _from;
		_effect.to = _to;
		_effect.delay = delay;
		
		return _effect;
	}

	public static TweenScale add_scale_anim(GameObject obj, float speed, Vector3 scale, float delay)
	{
		Vector3 _from = obj.transform.localScale;
		Vector3 _to = scale;
		
		TweenScale _effect = TweenScale.Begin(obj,speed,obj.transform.localScale);
		
		_effect.method = UITweener.Method.EaseInOut;
		_effect.from = _from;
		_effect.to = _to;
		_effect.delay = delay;
		
		return _effect;
	}

	public static byte[] Compress(byte[] inbuf)
	{
		SevenZip.CoderPropID[] propIDs = 
		{
			SevenZip.CoderPropID.DictionarySize,
			SevenZip.CoderPropID.PosStateBits,
			SevenZip.CoderPropID.LitContextBits,
			SevenZip.CoderPropID.LitPosBits,
			SevenZip.CoderPropID.Algorithm,
			SevenZip.CoderPropID.NumFastBytes,
			SevenZip.CoderPropID.MatchFinder,
			SevenZip.CoderPropID.EndMarker
		};
		object[] properties = 
		{
			(Int32)(23),
			(Int32)(2),
			(Int32)(3),
			(Int32)(2),
			(Int32)(1),
			(Int32)(128),
			(string)("bt4"),
			(bool)(true)
		};
		SevenZip.Compression.LZMA.Encoder coder = new SevenZip.Compression.LZMA.Encoder();
		coder.SetCoderProperties(propIDs, properties);
		MemoryStream msInp = new MemoryStream(inbuf);
		MemoryStream msOut = new MemoryStream();
		coder.WriteCoderProperties(msOut);
		coder.Code(msInp, msOut, -1, -1, null);
		return msOut.ToArray();
	}


	public static byte[] Decompress(byte[] inbuf)
	{
		SevenZip.Compression.LZMA.Decoder coder = new SevenZip.Compression.LZMA.Decoder();
		byte[] prop = new byte[5];
		Array.Copy(inbuf, prop, 5);
		coder.SetDecoderProperties(prop);
		MemoryStream msInp = new MemoryStream(inbuf);
		msInp.Seek(5, SeekOrigin.Current);
		MemoryStream msOut = new MemoryStream();
		coder.Code(msInp, msOut, -1, -1, null);
		return msOut.ToArray();
	}

	private static int[] m_tan_arr = {0, 174, 349, 524, 699, 874, 1051, 1227, 1405, 1583, 
		1763, 1943, 2125, 2308, 2493, 2679, 2867, 3057, 3249, 3443, 
		3639, 3838, 4040, 4244, 4452, 4663, 4877, 5095, 5317, 5543, 
		5773, 6008, 6248, 6494, 6745, 7002, 7265, 7535, 7812, 8097, 
		8390, 8692, 9004, 9325, 9656, 9999, 10355, 10723, 11106, 11503, 
		11917, 12348, 12799, 13270, 13763, 14281, 14825, 15398, 16003, 16642, 
		17320, 18040, 18807, 19626, 20503, 21445, 22460, 23558, 24750, 26050, 
		27474, 29042, 30776, 32708, 34874, 37320, 40107, 43314, 47046, 51445, 
		56712, 63137, 71153, 81443, 95143, 114300, 143006, 190811, 286362, 572899, 
		999999999, -572899, -286362, -190811, -143006, -114300, -95143, -81443, -71153, -63137, 
		-56712, -51445, -47046, -43314, -40107, -37320, -34874, -32708, -30776, -29042, 
		-27474, -26050, -24750, -23558, -22460, -21445, -20503, -19626, -18807, -18040, 
		-17320, -16642, -16003, -15398, -14825, -14281, -13763, -13270, -12799, -12348, 
		-11917, -11503, -11106, -10723, -10355, -10000, -9656, -9325, -9004, -8692, 
		-8390, -8097, -7812, -7535, -7265, -7002, -6745, -6494, -6248, -6008, 
		-5773, -5543, -5317, -5095, -4877, -4663, -4452, -4244, -4040, -3838, 
		-3639, -3443, -3249, -3057, -2867, -2679, -2493, -2308, -2125, -1943, 
		-1763, -1583, -1405, -1227, -1051, -874, -699, -524, -349, -174};

	public static mario_point tan(int r)
	{
		if (r == 0)
		{
			return new mario_point(100, 0);
		}
		else if (r == 90)
		{
			return new mario_point(0, 100);
		}
		else if (r == 180)
		{
			return new mario_point(-100, 0);
		}
		else if (r == 270)
		{
			return new mario_point(0, -100);
		}
		int y = m_tan_arr [r % 180] / 10;
		int x = 1000;
		if (y >= 1000000 || y <= -1000000)
		{
			y = y / 10000;
			x = 0;
		}
		else if (y >= 100000 || y <= -100000)
		{
			y = y / 1000;
			x = 1;
		}
		else if (y >= 10000 || y <= -10000)
		{
			y = y / 100;
			x = 10;
		}
		else if (y >= 1000 || y <= -1000)
		{
			y = y / 10;
			x = 100;
		}
		if (r >= 90 && r < 270)
		{
			y = -y;
			x = -x;
		}
		return new mario_point (x, y);
	}

	public static int atan(int x, int y)
	{
		if (x == 0 && y == 0)
		{
			return 0;
		}
		else if (x == 0 && y >= 0)
		{
			return 90;
		}
		else if (x == 0 && y < 0)
		{
			return 270;
		}
		else if (y == 0 && x > 0)
		{
			return 0;
		}
		else if (y == 0 && x < 0)
		{
			return 180;
		}
		int d = y * 10000 / x;
		int mc = 999999999;
		int r = 0;
		for (int i = 0; i < m_tan_arr.Length; ++i)
		{
			int cj = m_tan_arr[i] - d;
			if (cj < 0)
			{
				cj = -cj;
			}
			if (cj < mc)
			{
				mc = cj;
				r = i;
			}
		}
		if (y < 0)
		{
			r += 180;
		}
		return r;
	}

	private static int[] m_rxy_arr = {0, 1000, 17, 999, 34, 999, 52, 998, 69, 997, 87, 996, 
		104, 994, 121, 992, 139, 990, 156, 987, 173, 984, 190, 981, 207, 978, 224, 974, 241, 970, 
		258, 965, 275, 961, 292, 956, 309, 951, 325, 945, 342, 939, 358, 933, 374, 927, 390, 920, 
		406, 913, 422, 906, 438, 898, 453, 891, 469, 882, 484, 874, 499, 866, 515, 857, 529, 848, 
		544, 838, 559, 829, 573, 819, 587, 809, 601, 798, 615, 788, 629, 777, 642, 766, 656, 754, 
		669, 743, 681, 731, 694, 719, 707, 707, 719, 694, 731, 681, 743, 669, 754, 656, 766, 642, 
		777, 629, 788, 615, 798, 601, 809, 587, 819, 573, 829, 559, 838, 544, 848, 529, 857, 515, 
		866, 500, 874, 484, 882, 469, 891, 453, 898, 438, 906, 422, 913, 406, 920, 390, 927, 374, 
		933, 358, 939, 342, 945, 325, 951, 309, 956, 292, 961, 275, 965, 258, 970, 241, 974, 224, 
		978, 207, 981, 190, 984, 173, 987, 156, 990, 139, 992, 121, 994, 104, 996, 87, 997, 69, 
		998, 52, 999, 34, 999, 17, 1000, 0, 999, -17, 999, -34, 998, -52, 997, -69, 996, -87, 
		994, -104, 992, -121, 990, -139, 987, -156, 984, -173, 981, -190, 978, -207, 974, -224, 
		970, -241, 965, -258, 961, -275, 956, -292, 951, -309, 945, -325, 939, -342, 933, -358, 
		927, -374, 920, -390, 913, -406, 906, -422, 898, -438, 891, -453, 882, -469, 874, -484, 
		866, -499, 857, -515, 848, -529, 838, -544, 829, -559, 819, -573, 809, -587, 798, -601, 
		788, -615, 777, -629, 766, -642, 754, -656, 743, -669, 731, -681, 719, -694, 707, -707, 
		694, -719, 681, -731, 669, -743, 656, -754, 642, -766, 629, -777, 615, -788, 601, -798, 
		587, -809, 573, -819, 559, -829, 544, -838, 529, -848, 515, -857, 499, -866, 484, -874, 
		469, -882, 453, -891, 438, -898, 422, -906, 406, -913, 390, -920, 374, -927, 358, -933, 
		342, -939, 325, -945, 309, -951, 292, -956, 275, -961, 258, -965, 241, -970, 224, -974, 
		207, -978, 190, -981, 173, -984, 156, -987, 139, -990, 121, -992, 104, -994, 87, -996, 
		69, -997, 52, -998, 34, -999, 17, -999, 0, -1000, -17, -999, -34, -999, -52, -998, 
		-69, -997, -87, -996, -104, -994, -121, -992, -139, -990, -156, -987, -173, -984, -190, -981, 
		-207, -978, -224, -974, -241, -970, -258, -965, -275, -961, -292, -956, -309, -951, -325, -945, 
		-342, -939, -358, -933, -374, -927, -390, -920, -406, -913, -422, -906, -438, -898, -453, -891, 
		-469, -882, -484, -874, -500, -866, -515, -857, -529, -848, -544, -838, -559, -829, -573, -819, 
		-587, -809, -601, -798, -615, -788, -629, -777, -642, -766, -656, -754, -669, -743, -681, -731, 
		-694, -719, -707, -707, -719, -694, -731, -681, -743, -669, -754, -656, -766, -642, -777, -629, 
		-788, -615, -798, -601, -809, -587, -819, -573, -829, -559, -838, -544, -848, -529, -857, -515, 
		-866, -500, -874, -484, -882, -469, -891, -453, -898, -438, -906, -422, -913, -406, -920, -390, 
		-927, -374, -933, -358, -939, -342, -945, -325, -951, -309, -956, -292, -961, -275, -965, -258, 
		-970, -241, -974, -224, -978, -207, -981, -190, -984, -173, -987, -156, -990, -139, -992, -121, 
		-994, -104, -996, -87, -997, -69, -998, -52, -999, -34, -999, -17, -1000, 0, -999, 17, -999, 34, 
		-998, 52, -997, 69, -996, 87, -994, 104, -992, 121, -990, 139, -987, 156, -984, 173, -981, 190, 
		-978, 207, -974, 224, -970, 241, -965, 258, -961, 275, -956, 292, -951, 309, -945, 325, -939, 342, 
		-933, 358, -927, 374, -920, 390, -913, 406, -906, 422, -898, 438, -891, 453, -882, 469, -874, 484, 
		-866, 500, -857, 515, -848, 529, -838, 544, -829, 559, -819, 573, -809, 587, -798, 601, -788, 615, 
		-777, 629, -766, 642, -754, 656, -743, 669, -731, 681, -719, 694, -707, 707, -694, 719, -681, 731, 
		-669, 743, -656, 754, -642, 766, -629, 777, -615, 788, -601, 798, -587, 809, -573, 819, -559, 829, 
		-544, 838, -529, 848, -515, 857, -500, 866, -484, 874, -469, 882, -453, 891, -438, 898, -422, 906, 
		-406, 913, -390, 920, -374, 927, -358, 933, -342, 939, -325, 945, -309, 951, -292, 956, -275, 961, 
		-258, 965, -241, 970, -224, 974, -207, 978, -190, 981, -173, 984, -156, 987, -139, 990, -121, 992, 
		-104, 994, -87, 996, -69, 997, -52, 998, -34, 999, -17, 999};

	public static mario_point get_rxy(int r)
	{
		r = r % 360;
		if (r < 0)
		{
			r = 360 + r;
		}
		return new mario_point (m_rxy_arr[r * 2], m_rxy_arr[r * 2 + 1]);
	}

	public static void do_yfu(int y, int t)
	{
		int p = 0;
		if (y >= -7 && y <= -1)
		{
			p = yf[-y] + t;
		}
		if (y >= 1 && y <= 7)
		{
			p = yf[y] + t + 12;
		}
		if (y >= 11 && y <= 17)
		{
			p = yf[y - 10] + t + 24;
		}
		p -= 5;
		if (p != 0)
		{
			mario._instance.play_sound ("sound/yf/0-" + p.ToString());
		}
	}
	
	public static int[,] csg_points = {{2, 0}, {-2, 0}, {0, 2}, {0, -2}, {2, 2}, {-2, -2}, {-2, 2}, {2, -2}};
	public static int[] jx_block = {9, 4, 2, 10, 4, 116, 11, 7, 116, 19, 2, 105, 26, 2, 201, 27, 2, 201, 28, 2, 201,
		29, 2, 201, 30, 2, 201, 27, 3, 201, 28, 3, 201, 29, 3, 201, 28, 4, 207, 36, 4, 3, 37, 2, 3, 37, 3, 3, 37, 4, 3,
		37, 5, 3, 37, 0, 1, 37, 1, 1, 38, 0, 1, 38, 1, 1, 39, 0, 1, 39, 1, 1, 40, 0, 1, 40, 1, 1, 41, 0, 1, 41, 1, 1, 
		42, 0, 1, 42, 1, 1, 43, 0, 1, 43, 1, 1, 44, 0, 1, 44, 1, 1, 45, 0, 1, 45, 1, 1, 46, 0, 1, 46, 1, 1, 47, 0, 1, 47, 1, 1};
	public static int[] yuepu = {13, 0, 13, 0, 13, 0, 11, 0, 13, 0, 15, 0, 5, 0, 
		11, 0, 5, 0, 3, 0, 6, 0, 7, 0, 7, -1, 6, 0, 
		5, 0, 13, 0, 15, 0, 16, 0, 14, 0, 15, 0, 13, 0, 11, 0, 12, 0, 7, 0};
	public static int[] yf = {0, 1, 3, 5, 6, 8, 10, 12};
}

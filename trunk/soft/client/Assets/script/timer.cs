using UnityEngine;
using System.Collections;
using System;

public class timer {

	static long dtime_;
	static long dtime1_;
	public static ulong start_time_;

	public static void set_server_time(ulong server_time)
	{
		dtime_ = System.DateTime.Now.Ticks / 10000 - (long)server_time;
		dtime1_ = (System.DateTime.Now.Ticks - System.DateTime.Parse ("1/1/1970").Ticks) / 10000 - 28800000 - (long)server_time;
	}

	public static System.DateTime dtnow()
	{
		return System.DateTime.Now.AddTicks(-dtime1_ * 10000);
	}

	public static ulong now()
	{
		return (ulong)(System.DateTime.Now.Ticks / 10000 - dtime_);
	}

	public static System.DateTime time2dt(ulong time)
	{
		long tm = (long)(time + 28800000) * 10000;
		System.DateTime dt = System.DateTime.Parse ("1/1/1970").AddTicks(tm);
		return dt;
	}
	public static DateTime GetTime(string timeStamp)
	{
		DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
		long lTime = long.Parse(timeStamp + "0000000");
		TimeSpan toNow = new TimeSpan(lTime);
		return dtStart.Add(toNow);
	}
	public static int last_time_today()
	{
		System.DateTime dt = System.DateTime.Parse(dtnow().ToShortDateString() + " 23:59:59");
		long tick = (dt.Ticks - dtnow ().Ticks) / 10000;
		tick = tick % 86400000;
		return (int)tick;
	}
	
	public static bool trigger_time(ulong old_time, int hour, int minute)
	{
		System.DateTime old_dt = time2dt (old_time);
		ulong new_time = now();
		System.DateTime new_dt = time2dt (new_time);
		
		if (new_time <= old_time)
		{
			return false;
		}
		
		if (new_time - old_time >= 86400000)
		{
			return true;
		}
		
		bool old_small = is_small(old_dt, hour, minute);
		bool new_small = is_small(new_dt, hour, minute);
		
		if (is_same_day(old_dt, new_dt))
		{
			if (old_small && !new_small)
			{
				return true;
			}
			else
			{
				return false;
			}
		}
		else
		{
			if (!old_small && !new_small)
			{
				return true;
			}
			else if (old_small && new_small)
			{
				return true;
			}
			else
			{
				return false;
			}
		}
	}

	
	public static bool trigger_week_time(ulong old_time)
	{
		System.DateTime old_dt = time2dt (old_time);
		ulong new_time = now();
		System.DateTime new_dt = time2dt (new_time);
		
		if (new_time <= old_time)
		{
			return false;
		}
		
		if (new_time - old_time >= 86400000 * 7)
		{
			return true;
		}
		
		int nw = (int)new_dt.DayOfWeek;
		if (nw == 0)
		{
			nw = 7;
		}
		int ow = (int)old_dt.DayOfWeek;
		if (ow == 0)
		{
			ow = 7;
		}
		
		if (nw < ow)
		{
			return true;
		}
		else if (nw == ow)
		{
			if (new_time - old_time >= 86400000 * 6)
			{
				return true;
			}
		}
		
		return false;
	}
	
	public static bool trigger_month_time(ulong old_time)
	{
		System.DateTime old_dt = time2dt (old_time);
		ulong new_time = now();
		System.DateTime new_dt = time2dt (new_time);
		
		if (new_time <= old_time)
		{
			return false;
		}
		
		if (new_time - old_time >= 86400000L * 31)
		{
			return true;
		}
		
		int nw = new_dt.Month;
		int ow = old_dt.Month;
		
		if (nw != ow)
		{
			return true;
		}
		
		return false;
	}

	private static bool is_same_day(System.DateTime old_dt, System.DateTime new_dt)
	{
		if (old_dt.Year != new_dt.Year)
		{
			return false;
		}
		else if (old_dt.Month != new_dt.Month)
		{
			return false;
		}
		else if (old_dt.Day != new_dt.Day)
		{
			return false;
		}
		return true;
	}
	
	private static bool is_small(System.DateTime dt, int hour, int minute)
	{
		if (dt.Hour < hour)
		{
			return true;
		}
		else if (dt.Hour == hour)
		{
			if (dt.Minute < minute)
			{
				return true;
			}
			else
			{
				return false;
			}
		}
		return false;
	}

	public static int run_day(ulong old_time)
	{
		ulong now_time = now();
		if (old_time >= now_time)
		{
			return 0;
		}
		ulong delta_time = now_time - old_time;
		ulong day_num = delta_time / 86400000;
		ulong ltime = old_time + day_num * 86400000;
		if (trigger_time(ltime, 0, 0))
		{
			day_num++;
		}
		return (int)day_num;
	}

	public static string get_time_show(long t)
	{
		if (t < 0)
		{
			t = 0;
		}
		int tt = (int)(t / 1000);
		//计算小时,用毫秒总数除以(1000*60*24),后去掉小数点
		int hour = tt / 3600;
		//计算分钟,用毫秒总数减去小时乘以(1000*60*24)后,除以(1000*60),再去掉小数点
		int min = tt % 3600 / 60;
		//同上
		int sec = tt % 60;
		//ulong msec = t - hour*(1000*60*24*60) - min*(1000*60) - sec*1000;
		//拼接字符串
		string _hour = hour.ToString ();
		string _min = min.ToString ();
		string _sec = sec.ToString ();
		
		if(_hour.Length < 2)
		{
			_hour = "0" + _hour;
		}
		
		if(_min.Length < 2)
		{
			_min = "0" + _min;
		}
		
		if(_sec.Length < 2)
		{
			_sec = "0" + _sec;
		}

		string timeString = _hour + ":" + _min +":"+ _sec;
		if (hour == 0)
		{
			timeString = _min +":"+ _sec;
		}
		return timeString;
	}

	public static string get_game_time(int time)
	{
		int m = time / 3000;
		int s = (time / 50) % 60;
		int ss = time % 50 * 2;
		string mstr = m.ToString ();
		string sstr = s.ToString();
		string ssstr = ss.ToString ();
		if (m < 10)
		{
			mstr = "0" + mstr;
		}
		if (s < 10)
		{
			sstr = "0" + sstr;
		}
		if (ss < 10)
		{
			ssstr = "0" + ssstr;
		}
		return mstr + ":" + sstr + ":" + ssstr + "\"";
	}
}

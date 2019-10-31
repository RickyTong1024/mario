using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;

public class mario_editor : EditorWindow {

	private static mario_editor window;
	private string m_load_path;
	private string m_save_path;
	private string m_save_name;
	private string m_gm_string;

	[MenuItem("mario/edit")]
	private static void Execute()
	{
		if (window == null)
		{
			window = (mario_editor)GetWindow(typeof(mario_editor));
		}
		window.position = new Rect(Screen.width - 400, Screen.height - 200, 300, 300);
		window.Show();
	}

	void OnGUI()
	{
		{
			GUILayout.Label("读取地图", GUILayout.Width(60));
			EditorGUILayout.BeginHorizontal();
			m_load_path = EditorGUILayout.TextField(m_load_path, GUILayout.Width(220));
			if (GUILayout.Button("选择文件"))
			{
				m_load_path = EditorUtility.OpenFilePanel("请选择文件", @"..\..\..\scheme\mission", "");
			}
			EditorGUILayout.EndHorizontal();
			bool _ok = GUILayout.Button("读取", GUILayout.Width(80));
			if (_ok)
			{
				load(m_load_path);
			}
		}

		EditorGUILayout.LabelField("");

		{
			GUILayout.Label("保存地图", GUILayout.Width(60));
			EditorGUILayout.BeginHorizontal();
			m_save_path = EditorGUILayout.TextField(m_save_path, GUILayout.Width(220));
			if (GUILayout.Button("选择文件"))
			{
				m_save_path = EditorUtility.OpenFolderPanel("请选择文件夹", @"..\..\..\scheme\mission", "");
			}
			EditorGUILayout.EndHorizontal();
			GUILayout.Label("地图名", GUILayout.Width(60));
			m_save_name = EditorGUILayout.TextField(m_save_name, GUILayout.Width(220));
			bool _ok = GUILayout.Button("保存", GUILayout.Width(80));
			if (_ok)
			{
				save(m_save_name, m_save_path);
			}
		}

		{
			GUILayout.Label("GM命令", GUILayout.Width(60));
			m_gm_string = EditorGUILayout.TextField(m_gm_string, GUILayout.Width(220));
			bool _ok = GUILayout.Button("执行", GUILayout.Width(80));
			if (_ok)
			{
				gm(m_gm_string);
			}
		}

		{
			GUILayout.Label("清理账号", GUILayout.Width(60));
			bool _ok = GUILayout.Button("执行", GUILayout.Width(80));
			if (_ok)
			{
				delete_native();
			}
		}
	}

	private void save(string name, string path)
	{
		byte[] data = null;
		byte[] url = null;
		game_data._instance.get_mission_data (ref data, ref url);

		FileStream fs = new FileStream (path + "/" + name + ".data", FileMode.Create, FileAccess.Write);
		fs.Write (data, 0, data.Length);
		fs.Close ();

		fs = new FileStream (path + "/" + name + ".url", FileMode.Create, FileAccess.Write);
		fs.Write (url, 0, url.Length);
		fs.Close ();
	}

	private void load(string path)
	{
		FileStream fs = new FileStream (path, FileMode.Open, FileAccess.Read);
		byte[] data = new byte[1024 * 1024 * 5];
		fs.Read(data, 0, data.Length);
		fs.Close ();

		game_data._instance.load_mission (game_data._instance.m_map_id, data, new List<int>(), new List<int>());
		edit_mode._instance.reload (edit_mode._instance.m_world);
	}

	private void gm(string s)
	{
		//protocol.game.cmsg_play_map msg = new protocol.game.cmsg_play_map();
		//msg.id = mario._instance.m_self.mapid;
		//net_http._instance.send_msg<protocol.game.cmsg_play_map>(opclient_t.OPCODE_PLAY_MAP, msg);
	}

	private void delete_native()
	{
		game_data._instance.delete_native ();
	}
}

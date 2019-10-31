using UnityEngine;
using System.Collections;

public class play_gui_lose : MonoBehaviour {
	
	void OnEnable()
	{
		mario_tool._instance.cyad();
		mario_tool._instance.hfad ();
	}

	void OnDisable()
	{
		mario_tool._instance.close_hfad ();
	}
}

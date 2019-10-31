using UnityEngine;
using System.Collections;

public class other_language : MonoBehaviour {

	public string text;

	void Awake () {
		if (game_data._instance.m_lang == e_language.el_english)
		{
			this.GetComponent<UILabel>().trueTypeFont = mario._instance.m_efont;
		}
		if (text != "")
		{
			this.GetComponent<UILabel> ().text = game_data._instance.get_language_string(text);
		}
	}
}

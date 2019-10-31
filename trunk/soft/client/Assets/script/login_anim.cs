using UnityEngine;
using System.Collections;

public class login_anim : MonoBehaviour {

	public GameObject m_t2;
	public GameObject m_t3;
	public GameObject m_t4;
	public GameObject m_t5;
	public GameObject m_t6;
	public GameObject m_t7;
	public GameObject m_dj;
	public bool m_play = false;

	void OnEnable()
	{
		m_t2.SetActive (false);
		m_t3.SetActive (false);
		m_t4.SetActive (false);
		m_t5.SetActive (false);
		m_t6.SetActive (false);
		m_t7.SetActive (false);
		m_play = true;
	}

	void t2()
	{
		m_t2.SetActive (true);
	}

	void t3()
	{
		m_t3.SetActive (true);
	}

	void t4()
	{
		m_t4.SetActive (true);
	}

	void t5()
	{
		m_t5.SetActive (true);
	}

	void t6()
	{
		m_t6.SetActive (true);
	}

	void t7()
	{
		m_t7.SetActive (true);
	}

	void end()
	{
		m_play = false;
		m_dj.SetActive (true);
	}
}

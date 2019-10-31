using UnityEngine;
using System.Collections;

public class mario_collider : MonoBehaviour {

	public mario_rect m_rect = new mario_rect();

	#if UNITY_EDITOR
	void OnDrawGizmos ()
	{
		Gizmos.matrix = transform.localToWorldMatrix;
		Gizmos.color = new Color(1f, 0f, 0f);
		Gizmos.DrawWireCube(new Vector3(m_rect.x / 10, m_rect.y / 10, 0), new Vector3(m_rect.w / 10, m_rect.h / 10, 0f));
	}
	#endif
}

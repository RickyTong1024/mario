using UnityEngine;  
using System.Collections;  
using System.Collections.Generic;  

[ExecuteInEditMode]  
public class Joystick : MonoBehaviour  
{    
	#region Delegate & Event  
	public delegate void JoystickEventHandler(Joystick joystick);
	/// <summary> 
	/// 开如  
	/// </summary> 
	public static event JoystickEventHandler On_JoystickMoveStart;
	/// <summary> 
	/// thumb偏离中心位置，并牌按住时，每帧的回调  
	/// </summary> 
	public static event JoystickEventHandler On_JoystickHolding;
	/// <summary> 
	/// Occurs when the joystick stops move 
	/// </summary> 
	public static event JoystickEventHandler On_JoystickMoveEnd;
	
	#endregion      
	
	#region   property
	[HideInInspector]private bool isLimitInCircle = true;  
	public bool IsLimitInCircle { get { return this.isLimitInCircle; } } 
	[SerializeField]private int radius = 100;  
	public int Radius { get{ return this.radius; } } 
	
	[SerializeField]  
	private float minAlpha = 0.3f;  
	public float MinAlpha { get { return this.minAlpha; } } 
	
	private Vector2 joystickAxis = Vector2.zero;  
	/// <summary> 
	/// Gets the joystick axis value between -1 & 1...  
	/// </summary> 
	/// <value> 
	/// The joystick axis.  
	/// </value> 
	public Vector2 JoystickAxis { get { return this.joystickAxis; } }
	
	private Vector2 lastJoystickAxis = Vector2.zero;
	public Vector2 LastJoystickAxis { get { return this.lastJoystickAxis; } }
	
	bool isForBid = false;  
	/// <summary> 
	/// 判断joystick是否被禁用  
	/// </summary> 
	public bool IsForBid { get { return this.isForBid; } }
	bool isHolding = false;
	public bool IsHolding { get { return this.isHolding; } }
	#endregion
	
	UIWidget root;
	[SerializeField]UISprite bg;
	[SerializeField]UISprite thumb;
	
	void Awake()
	{
		root = this.GetComponent<UIWidget>();
		Init();
	}
	
	// Update is called once per frame     
	void Update ()
	{
		if (Application.isEditor && !Application.isPlaying)
		{
			SetJoystickSize(radius);
		}
		
		if (!isForBid && isHolding)
		{
			if (On_JoystickHolding != null)
			{
				On_JoystickHolding(this);
			}
		}
	}
	
	void Init()
	{
		bg.transform.localPosition = Vector3.zero;
		thumb.transform.localPosition = Vector3.zero;
		SetJoystickSize(radius);
		Lighting(minAlpha);
	}
	
	#region ngui event  
	///// <summary> 
	///// test  
	///// </summary> 
	void OnPress (bool isPressed)
	{
		if (isForBid)
		{
			Debug.Log("joystick is forbid!");
			return;
		}
		if(isPressed)
		{
			Lighting(1f);
			CalculateJoystickAxis();
			if (On_JoystickMoveStart != null)
			{
				On_JoystickMoveStart(this);
			}
			isHolding = true;
		}
		else
		{
			CalculateJoystickAxis();
			if (On_JoystickMoveEnd != null)
			{
				On_JoystickMoveEnd(this);
			}
			thumb.transform.localPosition = Vector3.zero;
			Lighting(minAlpha);
			isHolding = false;
		}
	}
	
	void OnDrag(Vector2 delta)
	{
		if (isForBid)
		{
			return;
		}
		      
		CalculateJoystickAxis();
		if (On_JoystickMoveStart != null)
		{
			On_JoystickMoveStart(this);  
		}
	}
	#endregion
	
	#region utile
	
	/// <summary> 
	/// 计算JoystickAxis  
	/// </summary> 
	/// <returns></returns> 
	void CalculateJoystickAxis()
	{
		Vector3 offset = ScreenPos_to_NGUIPos(UICamera.currentTouch.pos);
		Transform t = transform;
		do
		{
			offset -= t.localPosition;
			t = t.parent;
		} while (t.gameObject != UICamera.currentCamera.gameObject);
		{
			if (offset.magnitude > radius)
			{
				offset = offset.normalized * radius;
			}
		}
		thumb.transform.localPosition = offset;
		lastJoystickAxis = joystickAxis;
		joystickAxis = new Vector2(offset.x / radius, offset.y / radius);
	}
	
	/// <summary> 
	/// 屏幕坐标-->ui坐标  
	/// </summary> 
	/// <param name="screenPos"></param> 
	/// <returns></returns> 
	Vector3 ScreenPos_to_NGUIPos(Vector3 screenPos)
	{
		Vector3 uiPos = UICamera.currentCamera.ScreenToWorldPoint(screenPos);
		uiPos = UICamera.currentCamera.transform.InverseTransformPoint(uiPos);
		return uiPos;
	}
	
	/// <summary> 
	/// 屏幕坐标-->ngui坐标  
	/// </summary> 
	/// <param name="screenPos"></param> 
	/// <returns></returns> 
	Vector3 ScreenPos_to_NGUIPos(Vector2 screenPos)
	{
		return ScreenPos_to_NGUIPos(new Vector3(screenPos.x, screenPos.y, 0f));
	}
	
	/// <summary> 
	/// 设置摇杆的大小  
	/// </summary> 
	/// <param name="radius"></param> 
	void SetJoystickSize(int radius)
	{
		root.width = 2 * radius;
		root.height = 2 * radius;
		thumb.width = (int)(50f / 100f * root.width);
		thumb.height = (int)(50f / 100f * root.height);  
	}
	
	/// <summary> 
	/// 点亮摇杆  
	/// </summary> 
	void Lighting(float alpha)
	{
		root.alpha = alpha;
	}

	/// <summary>
	/// Axis2s the angle.
	/// </summary>
	/// <returns>
	/// The angle.
	/// </returns>
	public float Axis2Angle(bool inDegree = true)
	{
		float angle = Mathf.Atan2(joystickAxis.x, joystickAxis.y);
		
		if (inDegree)
		{
			return angle * Mathf.Rad2Deg;
		}
		else
		{
			return angle;
		}
	}
	
	#endregion
}

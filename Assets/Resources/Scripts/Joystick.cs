using System;
using Game.Managers;
using UnityEngine;
using UnityEngine.EventSystems;
using UnitySampleAssets.CrossPlatformInput;

public class Joystick : MonoBehaviour, IPointerUpHandler, IPointerDownHandler, IDragHandler, IEndDragHandler, IEventSystemHandler
{
	private void OnEnable()
	{
		this.CreateVirtualAxes();
		this.thumbRectTransform.anchoredPosition = this.thumbStartPosition;
		this.baseRectTransform.anchoredPosition = this.baseStartPosition;
	}

	private void Awake()
	{
		this._thumbTransform = this.Thumb.transform;
		this._baseTransform = this.Base.transform;
		this.thumbRectTransform = this.Thumb.GetComponent<RectTransform>();
		this.thumbStartPosition = this.thumbRectTransform.anchoredPosition;
		this.baseRectTransform = this.Base.GetComponent<RectTransform>();
		this.baseStartPosition = this.baseRectTransform.anchoredPosition;
	}

	private void CreateVirtualAxes()
	{
		this.useX = (this.axesToUse == Joystick.AxisOption.Both || this.axesToUse == Joystick.AxisOption.OnlyHorizontal);
		this.useY = (this.axesToUse == Joystick.AxisOption.Both || this.axesToUse == Joystick.AxisOption.OnlyVertical);
		if (this.useX)
		{
			this.horizontalVirtualAxis = new CrossPlatformInputManager.VirtualAxis(this.horizontalAxisName, false);
		}
		if (this.useY)
		{
			this.verticalVirtualAxis = new CrossPlatformInputManager.VirtualAxis(this.verticalAxisName, false);
		}
	}

	public void OnDrag(PointerEventData data)
	{
		this.UpdateVirtualAxes(this.CalcInput(data));
	}

	public Vector3 CalcInput(PointerEventData data)
	{
		Vector3 position = this._baseTransform.position;
		this._thumbTransform.position = new Vector3((!this.useX) ? position.x : data.position.x, (!this.useY) ? position.y : data.position.y);
		Vector3 vector = this._thumbTransform.localPosition - this._baseTransform.localPosition;
		Vector3 zero = Vector3.zero;
		zero.x = Mathf.Clamp(vector.x, (float)(-(float)this.MovementRange), (float)this.MovementRange);
		zero.y = Mathf.Clamp(vector.y, (float)(-(float)this.MovementRange), (float)this.MovementRange);
		this._thumbTransform.localPosition = Vector3.ClampMagnitude(vector, (float)this.MovementRange) + this._baseTransform.localPosition;
		return zero;
	}

	private void UpdateVirtualAxes(Vector3 value)
	{
		Vector3 vector = value;
		if (this.DebugLog && GameManager.ShowDebugs)
		{
			UnityEngine.Debug.Log(vector);
		}
		vector.x /= (float)this.MovementRange;
		vector.y /= (float)this.MovementRange;
		vector.x = Mathf.Sign(vector.x) * this.InputModefaer.Evaluate(Mathf.Abs(vector.x));
		vector.y = Mathf.Sign(vector.y) * this.InputModefaer.Evaluate(Mathf.Abs(vector.y));
		if (this.useX)
		{
			this.horizontalVirtualAxis.Update(vector.x);
		}
		if (this.useY)
		{
			this.verticalVirtualAxis.Update(vector.y);
		}
		if (this.DebugLog && GameManager.ShowDebugs)
		{
			UnityEngine.Debug.Log((value / (float)this.MovementRange).ToString("0.000") + " -> " + vector.ToString("0.000"));
		}
	}

	public void OnEndDrag(PointerEventData eventData)
	{
		this.ResetJoyStick();
	}

	public void ResetJoyStick()
	{
		this.thumbRectTransform.anchoredPosition = this.thumbStartPosition;
		this.baseRectTransform.anchoredPosition = this.baseStartPosition;
		this.UpdateVirtualAxes(Vector3.zero);
	}

	private void OnApplicationFocus(bool hasFocus)
	{
		this.ResetJoyStick();
	}

	public void OnPointerUp(PointerEventData data)
	{
		this.thumbRectTransform.anchoredPosition = this.thumbStartPosition;
		this.baseRectTransform.anchoredPosition = this.baseStartPosition;
		this.UpdateVirtualAxes(Vector3.zero);
	}

	public void OnPointerDown(PointerEventData data)
	{
		if (!this.StaticPosition)
		{
			this._thumbTransform.position = data.position;
			this._baseTransform.position = data.position;
		}
		else
		{
			this.UpdateVirtualAxes(this.CalcInput(data));
		}
	}

	private void OnDisable()
	{
		this.UpdateVirtualAxes(Vector3.zero);
		this.thumbRectTransform.anchoredPosition = this.thumbStartPosition;
		this.baseRectTransform.anchoredPosition = this.baseStartPosition;
		if (this.useX)
		{
			this.horizontalVirtualAxis.Remove();
		}
		if (this.useY)
		{
			this.verticalVirtualAxis.Remove();
		}
	}

	public bool DebugLog;

	public int MovementRange = 100;

	public GameObject Thumb;

	public GameObject Base;

	public bool StaticPosition;

	public AnimationCurve InputModefaer = AnimationCurve.Linear(0f, 0f, 1f, 1f);

	public Joystick.AxisOption axesToUse;

	public string horizontalAxisName = "Horizontal";

	public string verticalAxisName = "Vertical";

	private bool useX;

	private bool useY;

	private CrossPlatformInputManager.VirtualAxis horizontalVirtualAxis;

	private CrossPlatformInputManager.VirtualAxis verticalVirtualAxis;

	private Transform _thumbTransform;

	private Transform _baseTransform;

	private RectTransform thumbRectTransform;

	private RectTransform baseRectTransform;

	private Vector3 thumbStartPosition;

	private Vector3 baseStartPosition;

	public enum AxisOption
	{
		Both,
		OnlyHorizontal,
		OnlyVertical
	}
}

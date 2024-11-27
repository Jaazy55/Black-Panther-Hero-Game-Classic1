using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnitySampleAssets.CrossPlatformInput;

public class JoyPad : MonoBehaviour, IPointerUpHandler, IPointerDownHandler, IEventSystemHandler
{
	public void ResetJoypadPosition()
	{
		this.InitializeComponents();
		this.thumbRectTransform.anchoredPosition = this.thumbStartPosition;
		this.baseRectTransform.anchoredPosition = this.baseStartPosition;
	}

	private void InitializeComponents()
	{
		if (this.thumbRectTransform && this.baseRectTransform)
		{
			return;
		}
		this.thumbTransform = this.Thumb.transform;
		this.baseTransform = this.Base.transform;
		this.thumbRectTransform = this.Thumb.GetComponent<RectTransform>();
		this.thumbStartPosition = this.thumbRectTransform.anchoredPosition;
		this.baseRectTransform = this.Base.GetComponent<RectTransform>();
		this.baseStartPosition = this.baseRectTransform.anchoredPosition;
	}

	private void OnEnable()
	{
		this.CreateVirtualAxes();
		this.maxScreenSide = (float)Mathf.Max(Screen.width, Screen.height);
	}

	private void OnDisable()
	{
		this.OnPointerUp(null);
		if (this.useX)
		{
			this.horizontalVirtualAxis.Remove();
		}
		if (this.useY)
		{
			this.verticalVirtualAxis.Remove();
		}
	}

	private void CreateVirtualAxes()
	{
		this.useX = (this.AxesToUse == JoyPad.AxisOption.Both || this.AxesToUse == JoyPad.AxisOption.OnlyHorizontal);
		this.useY = (this.AxesToUse == JoyPad.AxisOption.Both || this.AxesToUse == JoyPad.AxisOption.OnlyVertical);
		if (this.useX)
		{
			this.horizontalVirtualAxis = new CrossPlatformInputManager.VirtualAxis(this.HorizontalAxisName, false);
		}
		if (this.useY)
		{
			this.verticalVirtualAxis = new CrossPlatformInputManager.VirtualAxis(this.VerticalAxisName, false);
		}
	}

	public void Awake()
	{
		this.InitializeComponents();
	}

	public void Start()
	{
		if (this.onDownPosition.Equals(Vector3.zero))
		{
			this.onDownPosition = this.thumbTransform.position;
			this.previousTouchPos = this.thumbTransform.position;
		}
	}

	public void OnPointerDown(PointerEventData data)
	{
		this.thumbTransform.position = data.position;
		this.baseTransform.position = data.position;
		this.dragging = true;
		this.id = data.pointerId;
		if (this.PadControlStyle != JoyPad.ControlStyle.Absolute)
		{
			this.onDownPosition = data.position;
			this.previousTouchPos = data.position;
		}
	}

	public void OnPointerUp(PointerEventData data)
	{
		this.ResetJoyPad();
	}

	public void ResetJoyPad()
	{
		this.thumbRectTransform.anchoredPosition = this.thumbStartPosition;
		this.baseRectTransform.anchoredPosition = this.baseStartPosition;
		this.dragging = false;
		this.id = -1;
		this.UpdateVirtualAxes(Vector3.zero);
	}

	private void OnApplicationFocus(bool hasFocus)
	{
		this.ResetJoyPad();
	}

	private void Update()
	{
		if (!this.dragging || (!this.UseJoy && !this.UsePad))
		{
			return;
		}
		Vector3 inputPosition = this.GetInputPosition();
		this.thumbTransform.position = new Vector3((!this.useX) ? this.baseTransform.position.x : inputPosition.x, (!this.useY) ? this.baseTransform.position.y : inputPosition.y);
		Vector3 vector = this.thumbTransform.localPosition - this.baseTransform.localPosition;
		Vector3 localPosition = Vector3.ClampMagnitude(-vector, (float)this.MovementRange) + this.thumbTransform.localPosition;
		localPosition.x = -Mathf.Clamp(-localPosition.x, (float)this.MovementRange, (float)(Screen.width - this.MovementRange));
		localPosition.y = Mathf.Clamp(localPosition.y, (float)this.MovementRange, (float)(Screen.height - this.MovementRange));
		this.baseTransform.localPosition = localPosition;
		if (this.UsePad)
		{
			JoyPad.ControlStyle padControlStyle = this.PadControlStyle;
			if (padControlStyle != JoyPad.ControlStyle.Swipe)
			{
				if (padControlStyle != JoyPad.ControlStyle.Look)
				{
					this.pointerDelta = (inputPosition - this.onDownPosition).normalized;
				}
				else
				{
					this.onDownPosition = this.previousTouchPos;
					this.pointerDelta = (inputPosition - this.onDownPosition) / this.maxScreenSide;
				}
			}
			else
			{
				this.onDownPosition = this.previousTouchPos;
				this.pointerDelta = (inputPosition - this.onDownPosition).normalized;
			}
			this.previousTouchPos = inputPosition;
			if (this.UseJoy && this.pointerDelta.Equals(Vector3.zero))
			{
				this.pointerDelta = this.GetJoyValue(vector) * this.JoySensitivity;
			}
		}
		else
		{
			this.pointerDelta = this.GetJoyValue(vector) * this.JoySensitivity;
		}
		this.UpdateVirtualAxes(this.pointerDelta * this.Multipler);
	}

	private Vector3 GetJoyValue(Vector3 localThumbPos)
	{
		Vector3 zero = Vector3.zero;
		zero.x = this.ClampTwoSided(localThumbPos.x, (float)this.MovementRange, this.JoySensitivityLimit) / (float)this.MovementRange;
		zero.y = this.ClampTwoSided(localThumbPos.y, (float)this.MovementRange, this.JoySensitivityLimit) / (float)this.MovementRange;
		return zero;
	}

	private float ClampTwoSided(float subject, float maxVal, float minLimit)
	{
		if (maxVal <= 0f || minLimit < 0f || minLimit >= 1f)
		{
			return 0f;
		}
		float num = Mathf.Sign(subject);
		float value = num * subject - maxVal * minLimit;
		float num2 = 1f - minLimit;
		return num * Mathf.Clamp(value, 0f, maxVal * num2) / num2;
	}

	private Vector3 GetInputPosition()
	{
		Vector3 result = Vector3.zero;
		if (UnityEngine.Input.touchCount >= this.id && this.id != -1)
		{
			result = Input.touches[this.id].position;
		}
		return result;
	}

	private void UpdateVirtualAxes(Vector3 value)
	{
		if (this.useX)
		{
			this.horizontalVirtualAxis.Update(value.x);
		}
		if (this.useY)
		{
			this.verticalVirtualAxis.Update(value.y);
		}
	}

	public int MovementRange = 100;

	public GameObject Thumb;

	public GameObject Base;

	public JoyPad.AxisOption AxesToUse;

	public string HorizontalAxisName = "Horizontal";

	public string VerticalAxisName = "Vertical";

	private bool useX;

	private bool useY;

	private CrossPlatformInputManager.VirtualAxis horizontalVirtualAxis;

	private CrossPlatformInputManager.VirtualAxis verticalVirtualAxis;

	private Transform thumbTransform;

	private Transform baseTransform;

	private Vector3 onDownPosition;

	public JoyPad.ControlStyle PadControlStyle = JoyPad.ControlStyle.Swipe;

	private Vector3 pointerDelta;

	private Vector3 previousTouchPos;

	private RectTransform thumbRectTransform;

	private RectTransform baseRectTransform;

	private Vector3 thumbStartPosition;

	private Vector3 baseStartPosition;

	public bool UseJoy = true;

	[Range(0f, 1f)]
	public float JoySensitivity = 1f;

	[Tooltip("`Радиус`, внутри которого джойстик не работает\n0\t- везде работает\n0.5\t- работает от половины MovementRange до полного\n1\t- не работает")]
	[Range(0f, 1f)]
	public float JoySensitivityLimit;

	public float Multipler = 1f;

	public bool UsePad = true;

	private bool dragging;

	private int id = -1;

	private float maxScreenSide;

	public enum AxisOption
	{
		Both,
		OnlyHorizontal,
		OnlyVertical
	}

	public enum ControlStyle
	{
		Absolute,
		Relative,
		Swipe,
		Look
	}
}

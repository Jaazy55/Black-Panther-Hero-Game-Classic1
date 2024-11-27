using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnitySampleAssets.CrossPlatformInput;

public class TouchPad : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IEventSystemHandler
{
	private void OnEnable()
	{
		this.CreateVirtualAxes();
		this.maxScreenSide = (float)Mathf.Max(Screen.width, Screen.height);
	}

	private void OnDisable()
	{
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
		this.useX = (this.AxesToUse == TouchPad.AxisOption.Both || this.AxesToUse == TouchPad.AxisOption.OnlyHorizontal);
		this.useY = (this.AxesToUse == TouchPad.AxisOption.Both || this.AxesToUse == TouchPad.AxisOption.OnlyVertical);
		if (this.useX)
		{
			this.horizontalVirtualAxis = new CrossPlatformInputManager.VirtualAxis(this.HorizontalAxisName);
		}
		if (this.useY)
		{
			this.verticalVirtualAxis = new CrossPlatformInputManager.VirtualAxis(this.VerticalAxisName);
		}
	}

	public void OnPointerDown(PointerEventData data)
	{
		this.dragging = true;
		this.pointerData = data;
		if (this.ControlType != TouchPad.ControlStyle.Absolute)
		{
			this.onDownPosition = data.position;
			this.previousTouchPos = data.position;
		}
	}

	public void OnPointerUp(PointerEventData data)
	{
		this.dragging = false;
		this.pointerData = null;
		this.UpdateVirtualAxes(Vector3.zero);
	}

	private void Update()
	{
		if (!this.dragging)
		{
			return;
		}
		Vector3 inputPosition = this.GetInputPosition();
		TouchPad.ControlStyle controlType = this.ControlType;
		if (controlType != TouchPad.ControlStyle.Swipe)
		{
			if (controlType != TouchPad.ControlStyle.Look)
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
		this.UpdateVirtualAxes(this.pointerDelta * this.Sensitivity);
	}

	private Vector3 GetInputPosition()
	{
		if (this.pointerData != null)
		{
			this.lastInput = this.pointerData.position;
		}
		return this.lastInput;
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

	public TouchPad.AxisOption AxesToUse;

	public float Sensitivity = 1f;

	public TouchPad.ControlStyle ControlType = TouchPad.ControlStyle.Look;

	public string HorizontalAxisName = "Horizontal";

	public string VerticalAxisName = "Vertical";

	private bool useX;

	private bool useY;

	private CrossPlatformInputManager.VirtualAxis horizontalVirtualAxis;

	private CrossPlatformInputManager.VirtualAxis verticalVirtualAxis;

	private bool dragging;

	private PointerEventData pointerData;

	private Vector3 onDownPosition;

	private Vector3 previousTouchPos;

	private Vector3 pointerDelta;

	private float maxScreenSide;

	private Vector3 lastInput;

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

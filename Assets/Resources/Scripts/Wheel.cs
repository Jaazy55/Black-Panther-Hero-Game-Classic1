using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnitySampleAssets.CrossPlatformInput;

public class Wheel : MonoBehaviour, IPointerUpHandler, IPointerDownHandler, IDragHandler, IEventSystemHandler
{
	private void OnEnable()
	{
		this.CreateVirtualAxes();
		this._isSteering = false;
	}

	public void Start()
	{
		this.wheelTransform = this.WheelImage.GetComponent<RectTransform>();
		this._startPos = this.wheelTransform.position;
		this._wheelStartPos = this.wheelTransform.position;
		Rect rect = this.wheelTransform.rect;
		this._wheelRadius = Mathf.Min(rect.height, rect.height) * 0.5f;
		this._wheelRadius *= this.ScaleRatio(this.wheelTransform);
	}

	private float ScaleRatio(RectTransform rectTransform)
	{
		Vector3 position = rectTransform.position;
		Vector3 localPosition = rectTransform.localPosition;
		rectTransform.position += Vector3.one;
		float result = Vector3.one.magnitude / (rectTransform.localPosition - localPosition).magnitude;
		rectTransform.position = position;
		return result;
	}

	private void UpdateVirtualAxes(float value)
	{
		float value2 = value / this.MaxMinRotationAngle;
		this.horizontalVirtualAxis.Update(value2);
	}

	private void CreateVirtualAxes()
	{
		this.horizontalVirtualAxis = new CrossPlatformInputManager.VirtualAxis(this.horizontalAxisName);
	}

	public void OnDrag(PointerEventData data)
	{
		Vector3 normalized = (data.position - new Vector2(this._tempWheelPos.x, this._tempWheelPos.y)).normalized;
		float num = Mathf.Atan(normalized.y / normalized.x) * 180f / 3.14159274f;
		if (!this._isSteering)
		{
			this._isSteering = true;
			this._angleNew = num;
			this._angleOld = num;
		}
		this._angleOld = this._angleNew;
		this._angleNew = num;
		float num2 = this._angleNew - this._angleOld;
		if (Mathf.Abs(num2) > 90f)
		{
			num2 = 0f;
		}
		this._currWheelAngle += num2;
		this._currWheelAngle = Mathf.Clamp(this._currWheelAngle, -this.MaxMinRotationAngle, this.MaxMinRotationAngle);
		this.UpdateVirtualAxes(this._currWheelAngle);
	}

	private void TurnWheel()
	{
		if (!this._isSteering && this._currWheelAngle != 0f)
		{
			this._currWheelAngle = Mathf.Lerp(this._currWheelAngle, 0f, Time.deltaTime * this.WheelMoveSmoothness);
			this.UpdateVirtualAxes(this._currWheelAngle);
		}
		this.wheelTransform.rotation = Quaternion.Euler(0f, 0f, this._currWheelAngle);
	}

	public void OnPointerUp(PointerEventData data)
	{
		this._isSteering = false;
		base.transform.position = this._startPos;
		this.wheelTransform.position = this._wheelStartPos;
	}

	public void OnPointerDown(PointerEventData data)
	{
		Vector2 b = new Vector2(this._startPos.x, this._startPos.y);
		Vector2 b2 = -(data.position - b).normalized * this._wheelRadius;
		this._tempWheelPos = data.position + b2;
		this.wheelTransform.position = this._tempWheelPos;
		this.OnDrag(data);
	}

	public void FixedUpdate()
	{
		this.TurnWheel();
	}

	private void OnDisable()
	{
		this.horizontalVirtualAxis.Remove();
	}

	public int MovementRange = 100;

	public GameObject WheelImage;

	public float MaxMinRotationAngle = 540f;

	public float WheelMoveSmoothness = 1f;

	public string horizontalAxisName = "Horizontal";

	private Vector3 _startPos;

	private Vector3 _tempWheelPos;

	private Vector3 _wheelStartPos;

	private CrossPlatformInputManager.VirtualAxis horizontalVirtualAxis;

	private float _wheelRadius;

	private bool _isSteering;

	private float _currWheelAngle;

	private float _angleOld;

	private float _angleNew;

	private RectTransform wheelTransform;
}

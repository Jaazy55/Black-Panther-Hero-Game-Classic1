using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnitySampleAssets.CrossPlatformInput;

public class AxisTouchButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IEventSystemHandler
{
	private void OnEnable()
	{
		this.axis = (CrossPlatformInputManager.VirtualAxisReference(this.axisName) ?? new CrossPlatformInputManager.VirtualAxis(this.axisName));
		this.FindPairedButton();
	}

	private void FindPairedButton()
	{
		AxisTouchButton[] array = UnityEngine.Object.FindObjectsOfType(typeof(AxisTouchButton)) as AxisTouchButton[];
		if (array != null)
		{
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].axisName == this.axisName && array[i] != this)
				{
					this.pairedWith = array[i];
				}
			}
		}
	}

	private void OnDisable()
	{
		this.axis.Remove();
	}

	public void OnPointerDown(PointerEventData data)
	{
		if (this.pairedWith == null)
		{
			this.FindPairedButton();
		}
		this.axis.Update(Mathf.MoveTowards(this.axis.GetValue, this.axisValue, this.responseSpeed * Time.deltaTime));
	}

	public void OnPointerUp(PointerEventData data)
	{
		this.axis.Update(Mathf.MoveTowards(this.axis.GetValue, 0f, this.responseSpeed * Time.deltaTime));
	}

	public string axisName = "Horizontal";

	public float axisValue = 1f;

	public float responseSpeed = 3f;

	public float returnToCentreSpeed = 3f;

	private AxisTouchButton pairedWith;

	private CrossPlatformInputManager.VirtualAxis axis;
}

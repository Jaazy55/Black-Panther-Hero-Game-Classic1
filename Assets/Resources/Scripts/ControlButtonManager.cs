using System;
using UnityEngine;
using UnitySampleAssets.CrossPlatformInput;

public class ControlButtonManager : MonoBehaviour
{
	private void OnEnable()
	{
		this.VirtualAxis = new CrossPlatformInputManager.VirtualAxis(this.AxisName);
	}

	public void SetAxis(float value)
	{
		this.VirtualAxis.Update(value);
	}

	private void OnDisable()
	{
		this.VirtualAxis.Remove();
	}

	public string AxisName = "Vertical";

	private CrossPlatformInputManager.VirtualAxis VirtualAxis;
}

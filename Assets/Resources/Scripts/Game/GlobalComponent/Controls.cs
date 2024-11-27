using System;
using Game.Vehicle;
using UnityEngine;
using UnitySampleAssets.CrossPlatformInput;

namespace Game.GlobalComponent
{
	public class Controls : MonoBehaviour
	{
		public static void SetControlsByVehicle(VehicleType vehicleType)
		{
			switch (vehicleType)
			{
			case VehicleType.Car:
				Controls.SetControlsByType(ControlsType.Car);
				return;
			case VehicleType.Motorbike:
				Controls.SetControlsByType(ControlsType.Moto);
				return;
			case VehicleType.Bicycle:
				Controls.SetControlsByType(ControlsType.Bike);
				return;
			case VehicleType.Tank:
				Controls.SetControlsByType(ControlsType.Tank);
				return;
			case VehicleType.Copter:
				Controls.SetControlsByType(ControlsType.Copter);
				return;
			case VehicleType.Mech:
				Controls.SetControlsByType(ControlsType.Mech);
				return;
			}
		}

		public static void SetControlsByType(ControlsType controlsType)
		{
			ControlsPanelManager.Instance.SwitchPanel(controlsType);
		}

		public static void SetControlsSubPanel(ControlsType controlsType, int index = 0)
		{
			ControlsPanelManager.Instance.SwitchSubPanel(controlsType, index);
		}

		public static float GetAxis(string axeName)
		{
			return CrossPlatformInputManager.GetAxis(axeName);
		}

		public static bool GetButton(string btnName)
		{
			return CrossPlatformInputManager.GetButton(btnName);
		}

		public static bool GetButtonUp(string btnName)
		{
			return CrossPlatformInputManager.GetButtonUp(btnName);
		}

		public static bool GetButtonDown(string btnName)
		{
			return CrossPlatformInputManager.GetButtonDown(btnName);
		}
	}
}

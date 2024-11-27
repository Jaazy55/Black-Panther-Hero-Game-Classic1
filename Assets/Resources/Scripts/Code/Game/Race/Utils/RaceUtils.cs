using System;
using System.Collections.Generic;
using Game.GlobalComponent;
using Game.Vehicle;
using UnityEngine;

namespace Code.Game.Race.Utils
{
	public static class RaceUtils
	{
		public static void SwapElements<T>(IList<T> array, int a, int b)
		{
			T value = array[a];
			array[a] = array[b];
			array[b] = value;
		}

		public static void BlockVehicleControl(VehicleController vehicleController, bool block)
		{
			if (vehicleController == null)
			{
				return;
			}
			vehicleController.SetInitialization(!block);
			MotorcycleController motorcycleController = vehicleController as MotorcycleController;
			if (motorcycleController != null)
			{
				motorcycleController.crashed = block;
			}
		}

		public static void RefreshCheckPointArrow(Transform target, bool activate)
		{
			if (!UIMarkManager.InstanceExist)
			{
				return;
			}
			UIMarkManager.Instance.TargetStaticMark = target;
			UIMarkManager.Instance.ActivateStaticMark(activate);
		}

		public static Vector3[] CalculateGridAdvanced(Vector3[] positions, Quaternion rotation)
		{
			GameObject gameObject = new GameObject();
			GameObject[] array = new GameObject[positions.Length];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = new GameObject();
				array[i].transform.position = positions[i];
				array[i].transform.parent = gameObject.transform;
			}
			gameObject.transform.rotation = rotation;
			for (int j = 0; j < array.Length; j++)
			{
				array[j].transform.parent = null;
				positions[j] = array[j].transform.position;
				UnityEngine.Object.Destroy(array[j]);
			}
			UnityEngine.Object.Destroy(gameObject);
			return positions;
		}

		public const string Untagged = "Untagged";

		public const string Racer = "Racer";

		public const string PlayerRacer = "PlayerRacer";
	}
}

using System;
using UnityEngine;

namespace Game.Vehicle
{
	public class OnCollisionEnterRerouting : MonoBehaviour
	{
		private void OnCollisionEnter(Collision col)
		{
			this.Reciever.OnCollisionEnter(col);
		}

		public DrivableVehicle Reciever;
	}
}

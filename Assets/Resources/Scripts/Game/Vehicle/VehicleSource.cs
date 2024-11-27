using System;
using UnityEngine;

namespace Game.Vehicle
{
	public class VehicleSource : MonoBehaviour
	{
		public BoxCollider SourceCollider
		{
			get
			{
				if (this.sourceCollider == null)
				{
					this.sourceCollider = base.GetComponent<BoxCollider>();
				}
				return this.sourceCollider;
			}
		}

		public DrivableVehicle RootVehicle
		{
			get
			{
				if (this.owner == null)
				{
					this.owner = base.GetComponentInParent<DrivableVehicle>();
				}
				return this.owner;
			}
			set
			{
				this.owner = value;
			}
		}

		private BoxCollider sourceCollider;

		private DrivableVehicle owner;
	}
}

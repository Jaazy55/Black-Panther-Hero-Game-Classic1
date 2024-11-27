using System;
using UnityEngine;

namespace Game.Mech
{
	[Serializable]
	public struct MechInputs
	{
		public Vector3 camMove;

		public Vector2 move;

		public Vector3 lookPos;

		public bool right90;

		public bool left90;

		public bool rotate180;

		public bool fire;
	}
}

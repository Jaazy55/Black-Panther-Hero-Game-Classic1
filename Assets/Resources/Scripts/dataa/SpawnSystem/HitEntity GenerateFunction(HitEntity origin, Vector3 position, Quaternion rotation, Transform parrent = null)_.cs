using System;
using Game.Character.CharacterController;
using UnityEngine;

namespace Naxeex.SpawnSystem
{
	public delegate HitEntity GenerateFunction(HitEntity origin, Vector3 position, Quaternion rotation, Transform parrent = null);
}

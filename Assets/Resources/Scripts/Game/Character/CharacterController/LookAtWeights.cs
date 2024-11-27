using System;
using UnityEngine;

namespace Game.Character.CharacterController
{
	[Serializable]
	public class LookAtWeights
	{
		[Range(0f, 1f)]
		public float weight = 1f;

		[Range(0f, 1f)]
		public float bodyWeight = 0.2f;

		[Range(0f, 1f)]
		public float headWeight = 2.5f;

		[Range(0f, 1f)]
		public float eyesWeight;

		[Range(0f, 1f)]
		public float clampWeight = 0.5f;
	}
}

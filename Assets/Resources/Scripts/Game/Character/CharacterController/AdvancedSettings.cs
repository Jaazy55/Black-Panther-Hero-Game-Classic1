using System;
using UnityEngine;

namespace Game.Character.CharacterController
{
	[Serializable]
	public class AdvancedSettings
	{
		public float stationaryTurnSpeed = 180f;

		public float movingTurnSpeed = 360f;

		public float headLookResponseSpeed = 2f;

		public float crouchHeightFactor = 0.6f;

		public float FlyHeightFactor = 1.2f;

		public float crouchChangeSpeed = 4f;

		public float autoTurnThresholdAngle = 100f;

		public float autoTurnSpeed = 2f;

		public PhysicMaterial zeroFrictionMaterial;

		public PhysicMaterial highFrictionMaterial;

		public float jumpRepeatDelayTime = 0.25f;

		public float runCycleLegOffset = 0.2f;

		public LayerMask GroundLayerMask;

		public float groundStickyEffect = 1f;

		public float climbVelocityLow = 1f;

		public float climbVelocityMedium = 1f;

		public float climbVelocityHigh = 1f;

		public float wallClimbDistanceLow = 0.6f;

		public float wallClimbDistanceMedium = 0.6f;

		public float wallClimbDistanceHigh = 0.6f;

		public LookAtWeights IdlelookAtWeights;

		public LookAtWeights AimlookAtWeights;

		public LookAtWeights FlyAimlookAtWeights;

		public float DefaultSpeedMult = 1f;

		public float UnderWaterSpeedMult = 0.5f;
	}
}

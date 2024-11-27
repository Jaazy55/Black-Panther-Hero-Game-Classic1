using System;
using Game.Shop;
using UnityEngine;

namespace Game.Items
{
	[Serializable]
	public class PreviewVariables
	{
		public GameObject PreviewModel;

		public PreviewCameraPositions CameraPosition = PreviewCameraPositions.Weaponry;

		public float AdditionalCameraDistance;

		public PreviewDummyPositions DummyPosition = PreviewDummyPositions.Weaponry;

		public PreviewItemPositions ItemPosition;

		public Vector3 PositionOffset = Vector3.zero;

		public PrevewAnimationController.PreviewAnimType PreviewAnimation;
	}
}

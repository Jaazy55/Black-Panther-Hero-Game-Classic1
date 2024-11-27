using System;
using UnityEngine;

namespace Game.GlobalComponent
{
	[Serializable]
	public struct MarkDetails
	{
		[SelectiveString("MarkType")]
		public string markType;

		public Sprite icon;

		public Color color;

		public Vector3 scale;

		public Vector2 offset;

		public float hideDistance;
	}
}

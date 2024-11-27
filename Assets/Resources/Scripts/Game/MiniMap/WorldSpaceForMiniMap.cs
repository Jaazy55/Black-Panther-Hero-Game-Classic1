using System;
using UnityEngine;

namespace Game.MiniMap
{
	public class WorldSpaceForMiniMap : MonoBehaviour
	{
		private void OnDrawGizmosSelected()
		{
			RectTransform component = base.GetComponent<RectTransform>();
			Vector3 vector = component.sizeDelta;
			Vector3 localPosition = component.localPosition;
			Gizmos.color = this.GizmoColor;
			Gizmos.DrawCube(localPosition, new Vector3(vector.x, 2f, vector.y));
			Gizmos.DrawWireCube(localPosition, new Vector3(vector.x, 2f, vector.y));
		}

		public Color GizmoColor = new Color(1f, 1f, 1f, 0.75f);
	}
}

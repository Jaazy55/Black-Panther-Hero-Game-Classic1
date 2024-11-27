using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnitySampleAssets.CrossPlatformInput;

namespace Game.MiniMap
{
	public class ZoomPinch : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IEventSystemHandler
	{
		private void OnEnable()
		{
			this.zoomAxis = new CrossPlatformInputManager.VirtualAxis(this.AxisName);
		}

		private void OnDisable()
		{
			this.zoomAxis.Remove();
		}

		private void Update()
		{
			if (this.touching && UnityEngine.Input.touchCount == 2)
			{
				Touch touch = UnityEngine.Input.GetTouch(0);
				Touch touch2 = UnityEngine.Input.GetTouch(1);
				Vector2 a = touch.position - touch.deltaPosition;
				Vector2 b = touch2.position - touch2.deltaPosition;
				float magnitude = (a - b).magnitude;
				float magnitude2 = (touch.position - touch2.position).magnitude;
				this.deltaMagnitudeDiff = magnitude - magnitude2;
			}
			else
			{
				this.deltaMagnitudeDiff = 0f;
			}
			this.zoomAxis.Update(this.deltaMagnitudeDiff);
		}

		public void OnPointerDown(PointerEventData eventData)
		{
			this.touching = true;
		}

		public void OnPointerUp(PointerEventData eventData)
		{
			this.touching = false;
		}

		public string AxisName = "MapZoom";

		private bool touching;

		private CrossPlatformInputManager.VirtualAxis zoomAxis;

		private float deltaMagnitudeDiff;
	}
}

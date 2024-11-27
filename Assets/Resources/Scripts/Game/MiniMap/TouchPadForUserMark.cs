using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Game.MiniMap
{
	public class TouchPadForUserMark : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IEventSystemHandler
	{
		private void Start()
		{
			this.miniMap = MiniMap.Instance;
		}

		private void Update()
		{
			if (!this.touching)
			{
				return;
			}
			this.CheckDragging();
			if (this.touchTimer < this.TouchTime)
			{
				this.touchTimer += 0.01f;
			}
			else
			{
				this.miniMap.LocateUserMark(this.lastMarkPosition);
				this.touchTimer = 0f;
				this.touching = false;
			}
		}

		private void CheckDragging()
		{
			float num = Vector3.Distance(this.lastPointerPosition, UnityEngine.Input.mousePosition);
			if (num > 10f)
			{
				this.touchTimer = 0f;
				this.touching = false;
			}
		}

		private Vector3 PositionAddiction(PointerEventData eventData)
		{
			float num = (float)Screen.width / this.resolutionIdeal.x;
			float num2 = (float)Screen.height / this.resolutionIdeal.y;
			Vector3 a = new Vector3(eventData.position.x, 0f, eventData.position.y);
			Vector3 b = new Vector3(this.MainRect.position.x, 0f, this.MainRect.position.y);
			Vector3 vector = a - b;
			float num3 = this.miniMap.MiniMapCamera.orthographicSize * 2f;
			float num4 = num3 / this.MainRect.rect.width / num;
			float num5 = num3 / this.MainRect.rect.height / num2;
			return new Vector3(vector.x * num4, 0f, vector.z * num5);
		}

		public void OnPointerDown(PointerEventData eventData)
		{
			this.lastPointerPosition = eventData.position;
			this.lastMarkPosition = this.PositionAddiction(eventData);
			this.touching = true;
		}

		public void OnPointerUp(PointerEventData eventData)
		{
			this.miniMap.MarkOnClick(this.PositionAddiction(eventData));
			this.touchTimer = 0f;
			this.touching = false;
		}

		private const float DraggingTolerance = 10f;

		public float TouchTime = 1f;

		public RectTransform MainRect;

		private MiniMap miniMap;

		private bool touching;

		private float touchTimer;

		private Vector3 lastMarkPosition;

		private Vector2 lastPointerPosition;

		private Vector2 resolutionIdeal = new Vector2(1920f, 1080f);
	}
}

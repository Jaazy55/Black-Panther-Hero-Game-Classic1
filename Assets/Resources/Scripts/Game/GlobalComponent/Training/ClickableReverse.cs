using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Game.GlobalComponent.Training
{
	public class ClickableReverse : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler, IPointerExitHandler, IDragHandler, IEndDragHandler, IEventSystemHandler
	{
		public void OnPointerDown(PointerEventData eventData)
		{
			List<RaycastResult> list = new List<RaycastResult>();
			this.RootRaycaster.Raycast(eventData, list);
			foreach (RaycastResult raycastResult in list)
			{
				if (!(raycastResult.gameObject == base.gameObject))
				{
					Component[] components = raycastResult.gameObject.GetComponents<Component>();
					foreach (Component component in components)
					{
						IPointerDownHandler pointerDownHandler = component as IPointerDownHandler;
						if (pointerDownHandler != null)
						{
							pointerDownHandler.OnPointerDown(eventData);
							this.downPressedComponents.Add(component);
							this.downEventData = eventData;
						}
					}
				}
			}
		}

		public void OnPointerUp(PointerEventData eventData)
		{
			List<RaycastResult> list = new List<RaycastResult>();
			this.RootRaycaster.Raycast(eventData, list);
			foreach (RaycastResult raycastResult in list)
			{
				if (!(raycastResult.gameObject == base.gameObject))
				{
					Component[] components = raycastResult.gameObject.GetComponents<Component>();
					foreach (Component component in components)
					{
						IPointerUpHandler pointerUpHandler = component as IPointerUpHandler;
						if (pointerUpHandler != null)
						{
							pointerUpHandler.OnPointerUp(eventData);
						}
					}
				}
			}
			this.downPressedComponents.Clear();
			this.downEventData = null;
		}

		public void OnPointerClick(PointerEventData eventData)
		{
			List<RaycastResult> list = new List<RaycastResult>();
			this.RootRaycaster.Raycast(eventData, list);
			foreach (RaycastResult raycastResult in list)
			{
				if (!(raycastResult.gameObject == base.gameObject))
				{
					Component[] components = raycastResult.gameObject.GetComponents<Component>();
					foreach (Component component in components)
					{
						IPointerClickHandler pointerClickHandler = component as IPointerClickHandler;
						if (pointerClickHandler != null)
						{
							pointerClickHandler.OnPointerClick(eventData);
						}
					}
				}
			}
		}

		public void OnPointerExit(PointerEventData eventData)
		{
			foreach (Component component in this.downPressedComponents)
			{
				IPointerUpHandler pointerUpHandler = component as IPointerUpHandler;
				if (pointerUpHandler != null)
				{
					pointerUpHandler.OnPointerUp(this.downEventData);
				}
			}
			this.downPressedComponents.Clear();
			this.downEventData = null;
		}

		public void OnDrag(PointerEventData eventData)
		{
			List<RaycastResult> list = new List<RaycastResult>();
			this.RootRaycaster.Raycast(eventData, list);
			foreach (RaycastResult raycastResult in list)
			{
				if (!(raycastResult.gameObject == base.gameObject))
				{
					Component[] components = raycastResult.gameObject.GetComponents<Component>();
					foreach (Component component in components)
					{
						IDragHandler dragHandler = component as IDragHandler;
						if (dragHandler != null)
						{
							dragHandler.OnDrag(eventData);
							this.dragedComponents.Add(component);
							this.dragEventData = eventData;
						}
					}
				}
			}
		}

		public void OnEndDrag(PointerEventData eventData)
		{
			foreach (Component component in this.dragedComponents)
			{
				IEndDragHandler endDragHandler = component as IEndDragHandler;
				if (endDragHandler != null)
				{
					endDragHandler.OnEndDrag(this.dragEventData);
				}
			}
			this.dragedComponents.Clear();
			this.dragEventData = null;
		}

		private void OnDisable()
		{
			if (this.downEventData != null)
			{
				this.OnPointerExit(this.downEventData);
			}
			if (this.dragEventData != null)
			{
				this.OnEndDrag(this.dragEventData);
			}
		}

		public GraphicRaycaster RootRaycaster;

		private readonly List<Component> downPressedComponents = new List<Component>();

		private PointerEventData downEventData;

		private readonly List<Component> dragedComponents = new List<Component>();

		private PointerEventData dragEventData;
	}
}

using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonOnDown : MonoBehaviour, IPointerDownHandler, IEventSystemHandler
{
	public void OnPointerDown(PointerEventData eventData)
	{
		this.OnDownEvents.Invoke();
	}

	public Button.ButtonClickedEvent OnDownEvents;
}

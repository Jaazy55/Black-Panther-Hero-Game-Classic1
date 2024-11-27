using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonOnUp : MonoBehaviour, IPointerUpHandler, IEventSystemHandler
{
	public void OnPointerUp(PointerEventData eventData)
	{
		this.OnUpEvents.Invoke();
	}

	public Button.ButtonClickedEvent OnUpEvents;
}

using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnitySampleAssets.CrossPlatformInput;

public class LookControlsSwitcher : MonoBehaviour, IPointerUpHandler, IPointerDownHandler, IEventSystemHandler
{
	private void Awake()
	{
		if (!this.TouchPad)
		{
			this.TouchPad = base.transform.parent.GetComponentInChildren<TouchPad>();
		}
		if (!this.TouchPad)
		{
			UnityEngine.Debug.LogError("Can't find `TouchPad`");
			base.enabled = false;
		}
		if (!this.JoyPad)
		{
			this.JoyPad = base.GetComponent<JoyPad>();
		}
		if (!this.JoyPad)
		{
			UnityEngine.Debug.LogError("Can't find `JoyPad`");
			base.enabled = false;
		}
		if (!this.ButtonHandler)
		{
			this.ButtonHandler = base.GetComponent<ButtonHandler>();
		}
		if (!this.ButtonHandler)
		{
			UnityEngine.Debug.LogError("Can't find `ButtonHandler`");
			base.enabled = false;
		}
	}

	private void Start()
	{
		this.JoyPad.Start();
	}

	public void OnPointerDown(PointerEventData data)
	{
		this.TouchPad.enabled = false;
		this.JoyPad.enabled = true;
		this.JoyPad.OnPointerDown(data);
		foreach (string downState in this.InputButtons)
		{
			this.ButtonHandler.SetDownState(downState);
		}
	}

	public void OnPointerUp(PointerEventData data)
	{
		this.JoyPad.enabled = false;
		this.TouchPad.enabled = true;
		this.JoyPad.OnPointerUp(data);
		foreach (string upState in this.InputButtons)
		{
			this.ButtonHandler.SetUpState(upState);
		}
	}

	private void OnApplicationFocus(bool hasFocus)
	{
		this.ResertLookControlsSwitcher();
	}

	public void ResertLookControlsSwitcher()
	{
		this.JoyPad.enabled = false;
		this.TouchPad.enabled = true;
		this.JoyPad.ResetJoyPad();
		foreach (string upState in this.InputButtons)
		{
			this.ButtonHandler.SetUpState(upState);
		}
	}

	private void OnEnable()
	{
		this.JoyPad.ResetJoypadPosition();
	}

	private void OnDisable()
	{
		this.OnPointerUp(null);
	}

	public TouchPad TouchPad;

	public JoyPad JoyPad;

	public ButtonHandler ButtonHandler;

	public string[] InputButtons = new string[]
	{
		"Aim",
		"Fire"
	};
}

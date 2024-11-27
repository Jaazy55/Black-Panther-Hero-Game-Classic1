using System;
using UnityEngine;
using UnityEngine.UI;

public class BackButton : MonoBehaviour
{
	public static void ChangeBackButtonsStatus(bool active)
	{
		BackButton.BackButtonsActive = active;
	}

	private void Awake()
	{
		this._button = base.GetComponent<Button>();
	}

	private void Update()
	{
		if (!BackButton.BackButtonsActive)
		{
			return;
		}
		if (this._button != null && UnityEngine.Input.GetKeyDown(KeyCode.Escape))
		{
			this._button.onClick.Invoke();
		}
	}

	public static bool BackButtonsActive = true;

	private Button _button;
}

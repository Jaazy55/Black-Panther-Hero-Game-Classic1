using System;
using Game.Managers;
using UnityEngine;
using UnityEngine.UI;

public class SoundButtonToggle : MonoBehaviour
{
	private void Awake()
	{
		SoundManager.instance.AddValueChangeByType(this.Type, delegate(float value)
		{
			if (this != null)
			{
				this.ValueChanged(value);
			}
		});
	}

	private void Start()
	{
		this.ValueChanged(SoundManager.instance.GetValueByType(this.Type));
	}

	private void ValueChanged(float value)
	{
		Toggle component = base.GetComponent<Toggle>();
		if (component != null)
		{
			component.isOn = (value > 0f);
		}
	}

	public void ToggleAction(bool isChecked)
	{
		float value;
		if (isChecked)
		{
			float valueByType = SoundManager.instance.GetValueByType(this.Type);
			value = ((valueByType <= 0f) ? 0.5f : valueByType);
		}
		else
		{
			value = 0f;
		}
		SoundManager.instance.SetValueByType(value, this.Type);
		if (this.ValueSlider != null)
		{
			if (!isChecked && this.ValueSlider.value > 0f)
			{
				this.ValueSlider.value = 0f;
			}
			if (isChecked && this.ValueSlider.value == 0f)
			{
				this.ValueSlider.value = value;
			}
		}
	}

	public Slider ValueSlider;

	public SoundManager.SoundType Type;
}

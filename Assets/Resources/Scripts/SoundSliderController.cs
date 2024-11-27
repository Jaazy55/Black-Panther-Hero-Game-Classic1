using System;
using Game.Managers;
using UnityEngine;
using UnityEngine.UI;

public class SoundSliderController : MonoBehaviour
{
	private void Awake()
	{
		Slider component = base.GetComponent<Slider>();
		if (component != null)
		{
			component.value = SoundManager.instance.GetValueByType(this.SoundType);
		}
	}

	public SoundManager.SoundType SoundType;
}

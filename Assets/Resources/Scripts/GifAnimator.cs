using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GifAnimator : MonoBehaviour
{
	private void OnEnable()
	{
		base.StartCoroutine("SwitchFrame");
	}

	private void OnDisable()
	{
		base.StopCoroutine("SwitchFrame");
	}

	private IEnumerator SwitchFrame()
	{
		for (;;)
		{
			for (int i = 0; i < this.m_Images.Length; i++)
			{
				this.m_TargetImage.sprite = this.m_Images[i].Frame;
				yield return new WaitForSecondsRealtime(this.m_Images[i].Delay);
			}
			yield return null;
		}
		yield break;
	}

	[SerializeField]
	protected Image m_TargetImage;

	[SerializeField]
	protected GifImage[] m_Images;
}

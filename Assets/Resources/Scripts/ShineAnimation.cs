using System;
using UnityEngine;

[RequireComponent(typeof(UIColorShine))]
public class ShineAnimation : MonoBehaviour
{
	private void OnEnable()
	{
		this.t = 0f;
		this.m_UIColorShine = base.GetComponent<UIColorShine>();
	}

	private void Update()
	{
		this.t += this.speed * Time.unscaledDeltaTime;
		this.m_UIColorShine.shinePositon = this.animCurve.Evaluate(this.t);
	}

	public AnimationCurve animCurve;

	[Tooltip("Animation speed multiplier")]
	public float speed = 1f;

	private float t;

	private UIColorShine m_UIColorShine;
}

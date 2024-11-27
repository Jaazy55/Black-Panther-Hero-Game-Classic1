using System;
using Game.GlobalComponent;
using UnityEngine;
using UnityEngine.UI;

public class AnimatePanel : Cutscene
{
	public override void StartScene()
	{
		this.img = CutscenePanel.Instance.gameObject.GetComponent<Image>();
		this.CurrentColor = this.StartColor;
		this.timer = 0f;
		this.IsPlaying = true;
	}

	public void Update()
	{
		if (!this.IsPlaying)
		{
			return;
		}
		if (this.timer > this.TimeOut)
		{
			this.EndScene(true);
		}
		this.timer += Time.deltaTime;
		this.CurrentColor = Color.Lerp(this.CurrentColor, this.EndColor, Time.deltaTime * this.timer * this.Speed);
		this.img.color = this.CurrentColor;
	}

	public override void EndScene(bool isCheck = true)
	{
		base.EndScene(isCheck);
		this.img.color = new Color(0f, 0f, 0f, 0f);
	}

	public Color StartColor;

	public Color EndColor;

	public Color CurrentColor;

	public float TimeOut;

	public float Speed = 1f;

	private float timer;

	private Image img;
}

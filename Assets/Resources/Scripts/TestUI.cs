using System;
using Game.GlobalComponent;
using UnityEngine;

public class TestUI : MonoBehaviour
{
	private void Update()
	{
		if (this.timer <= 0f && this.work)
		{
			InGameLogManager.Instance.RegisterNewMessage(MessageType.Gems, "geeeeems");
			this.timer = 1f;
		}
		else
		{
			this.timer -= Time.deltaTime;
		}
		if (this.rotate)
		{
			this.UIObject.Rotate(new Vector3(0f, 0f, 1f), 15f);
		}
		if (this.rotateCanvas)
		{
			this.UIObjectOnCanvas.Rotate(new Vector3(0f, 0f, 1f), 10f);
		}
	}

	public void Work()
	{
		this.work = !this.work;
	}

	public void Rotate()
	{
		this.rotate = !this.rotate;
	}

	public void RotateCanvas()
	{
		this.rotateCanvas = !this.rotateCanvas;
	}

	public RectTransform UIObject;

	public RectTransform UIObjectOnCanvas;

	private bool work;

	private bool rotate;

	private bool rotateCanvas;

	private float timer;
}

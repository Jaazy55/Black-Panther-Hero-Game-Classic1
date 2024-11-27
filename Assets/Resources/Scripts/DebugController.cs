using System;
using UnityEngine;
using UnitySampleAssets.CrossPlatformInput;

public class DebugController : MonoBehaviour
{
	private void Start()
	{
		//this.DebugMode = UnityEngine.Debug.isDebugBuild;
	}

	private void Update()
	{
		if (!this.DebugMode)
		{
			return;
		}
		this._timeFps += Time.unscaledDeltaTime;
		this._framesCount += 1f;
		if (this._timeFps > 1f)
		{
			this._fps = this._framesCount / this._timeFps;
			this._framesCount = 0f;
			this._timeFps = 0f;
		}
	}

	private void OnGUI()
	{
		if (!this.DebugMode)
		{
			return;
		}
		GUIStyle guistyle = new GUIStyle();
		int num = Screen.height * 2 / 100;
		Rect position = new Rect(0f, 0f, (float)Screen.width, (float)num);
		guistyle.alignment = TextAnchor.UpperLeft;
		guistyle.fontSize = 45;
		guistyle.normal.textColor = new Color(0f, 1f, 0f, 1f);
		if (this.AdvancedMode)
		{
			float axis = CrossPlatformInputManager.GetAxis("Horizontal");
			float axis2 = CrossPlatformInputManager.GetAxis("Vertical");
			string text = string.Format("{0:0.} fps \n XInput: {1:0.00} \n YInput: {2:0.00} \nScreen: {3:0}x{4:0} DPI: {5:0}", new object[]
			{
				this._fps,
				axis,
				axis2,
				Screen.width,
				Screen.height,
				Screen.dpi
			});
			GUI.Label(position, text, guistyle);
		}
		else
		{
			string text2 = Mathf.Round(this._fps).ToString();
			GUI.Label(position, text2, guistyle);
		}
	}

	public bool DebugMode;

	public bool AdvancedMode;

	private float _timeFps;

	private float _framesCount;

	private float _fps;
}

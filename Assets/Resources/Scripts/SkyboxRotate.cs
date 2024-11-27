using System;
using UnityEngine;

public class SkyboxRotate : MonoBehaviour
{
	private void FixedUpdate()
	{
		RenderSettings.skybox.SetFloat("_Rotation", Time.time * 0.35f);
	}
}

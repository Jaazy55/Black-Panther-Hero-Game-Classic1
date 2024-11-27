using System;
using UnityEngine;

[Serializable]
public struct GifImage
{
	[SerializeField]
	public Sprite Frame;

	[SerializeField]
	public float Delay;
}

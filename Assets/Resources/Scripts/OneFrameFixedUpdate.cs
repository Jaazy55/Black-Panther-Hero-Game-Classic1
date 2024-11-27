using System;
using UnityEngine;

public class OneFrameFixedUpdate
{
	public int FixedCounts
	{
		get
		{
			return this._fixedCounts;
		}
	}

	public bool IsSameFrameCountFrame()
	{
		if (this._prevFrame == Time.renderedFrameCount)
		{
			this._fixedCounts++;
			return true;
		}
		this._fixedCounts = 1;
		this._prevFrame = Time.renderedFrameCount;
		return false;
	}

	public bool IsSameFrame()
	{
		if (this._prevFrame == Time.renderedFrameCount)
		{
			return true;
		}
		this._fixedCounts = 1;
		this._prevFrame = Time.renderedFrameCount;
		return false;
	}

	private int _prevFrame;

	private int _fixedCounts = 1;
}

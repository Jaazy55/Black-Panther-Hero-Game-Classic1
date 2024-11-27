using System;

public class LevelInfo
{
	public int StarsCount
	{
		get
		{
			return this._starsCount;
		}
		set
		{
			this._starsCount = value;
		}
	}

	public bool IsAvailable
	{
		get
		{
			return this._isAvailable;
		}
		set
		{
			this._isAvailable = value;
		}
	}

	private int _starsCount;

	private bool _isAvailable;
}

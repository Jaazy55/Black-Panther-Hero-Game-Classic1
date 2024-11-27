using System;
using UnityEngine;

public class SeparatorAttribute : PropertyAttribute
{
	public SeparatorAttribute()
	{
		this.title = string.Empty;
	}

	public SeparatorAttribute(string _title)
	{
		this.title = _title;
	}

	public readonly string title;
}

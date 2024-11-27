using System;
using UnityEngine;

[AttributeUsage(AttributeTargets.Field)]
public class InspectorButtonAttribute : PropertyAttribute
{
	public InspectorButtonAttribute(string MethodName)
	{
		this.MethodName = MethodName;
	}

	public float ButtonWidth
	{
		get
		{
			return this._buttonWidth;
		}
		set
		{
			this._buttonWidth = value;
		}
	}

	public static float kDefaultButtonWidth = 150f;

	public readonly string MethodName;

	private float _buttonWidth = InspectorButtonAttribute.kDefaultButtonWidth;
}

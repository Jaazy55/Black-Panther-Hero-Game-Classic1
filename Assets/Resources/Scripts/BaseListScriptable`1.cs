using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseListScriptable<T> : ScriptableObject
{
	protected virtual void Awake()
	{
	}

	protected virtual void OnEnable()
	{
	}

	protected virtual void OnDisable()
	{
	}

	protected virtual void OnDestroy()
	{
	}

	public T this[int index]
	{
		get
		{
			return this.m_Details[index];
		}
	}

	public int Count
	{
		get
		{
			return this.m_Details.Count;
		}
	}

	[SerializeField]
	protected List<T> m_Details;
}

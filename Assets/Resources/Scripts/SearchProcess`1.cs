using System;
using System.Collections.Generic;
using Game.Character;
using Game.GlobalComponent;
using UnityEngine;

public class SearchProcess<T> : ISeachProcess where T : Component
{
	public SearchProcess()
	{
		this.Condition = new Func<T, bool>(this.DefaultCondition);
	}

	public SearchProcess(Func<T, bool> condition)
	{
		this.Condition = condition;
	}

	public bool DefaultCondition(T obj)
	{
		return true;
	}

	public void Initialize()
	{
		this.comparer = new DistanceComparer<T>();
		this.m_Founded = new T[200];
		this.containersMarks = new List<MarkContainer>();
		for (int i = 0; i < this.countMarks; i++)
		{
			this.containersMarks.Add(UIMarkManager.Instance.AddDinamicMark(null, this.markType));
		}
	}

	public void Processing()
	{
		int itemInUse = PoolManager.Instance.GetItemInUse<T>(this.m_Founded);
		if (itemInUse > 0)
		{
			this.comparer.playerPosition = PlayerInteractionsManager.Instance.GetPlayerPosition();
			Array.Sort<T>(this.m_Founded, 0, itemInUse, this.comparer);
		}
		int num = 0;
		int num2 = 0;
		while (num2 < itemInUse && num < this.countMarks)
		{
			if (this.Condition(this.m_Founded[num2]))
			{
				this.containersMarks[num].Target = this.m_Founded[num2].transform;
				num++;
			}
			num2++;
		}
		for (int i = num; i < this.countMarks; i++)
		{
			this.containersMarks[num].Target = null;
		}
	}

	public void Release()
	{
		if (UIMarkManager.InstanceExist)
		{
			for (int i = 0; i < this.countMarks; i++)
			{
				UIMarkManager.Instance.RemoveDinamicMarks(this.containersMarks[i]);
			}
		}
	}

	private const int C_ARRAY_SIZE = 200;

	public T[] m_Founded;

	private List<MarkContainer> containersMarks;

	public int countMarks = 10;

	public string markType = "Kill";

	public Func<T, bool> Condition;

	private DistanceComparer<T> comparer;
}

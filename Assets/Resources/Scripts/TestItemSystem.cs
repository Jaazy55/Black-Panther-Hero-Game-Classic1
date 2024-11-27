using System;
using System.Collections.Generic;
using Game.Items;
using UnityEngine;

public class TestItemSystem : MonoBehaviour
{
	public void ShowAllItems()
	{
		Dictionary<int, GameItem> items = ItemsManager.Instance.Items;
		MonoBehaviour.print(items.Count);
		foreach (KeyValuePair<int, GameItem> keyValuePair in items)
		{
			UnityEngine.Debug.LogFormat(keyValuePair.Value, "name {0} ID {1}", new object[]
			{
				keyValuePair.Value.name,
				keyValuePair.Value.ID
			});
		}
	}

	public void ClearAllItems()
	{
		ItemsManager.Instance.Items.Clear();
		MonoBehaviour.print(ItemsManager.Instance.Items.Count);
	}

	[InspectorButton("ShowAllItems")]
	public bool ShowItems;

	[InspectorButton("ClearAllItems")]
	public bool ClearItems;
}

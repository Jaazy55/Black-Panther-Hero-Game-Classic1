using System;
using System.Collections.Generic;
using System.Linq;
using Game.Items;
using UnityEngine;

namespace Game.Shop
{
	public class SalesManager : MonoBehaviour
	{
		public static SalesManager Instance
		{
			get
			{
				SalesManager result;
				if ((result = SalesManager.instance) == null)
				{
					result = (SalesManager.instance = UnityEngine.Object.FindObjectOfType<SalesManager>());
				}
				return result;
			}
		}

		public void Init()
		{
			if (this.inited)
			{
				return;
			}
			this.lastSalesUpdateTime = BaseProfile.ResolveValue<long>("SalesUpdateTime", 0L);
			if (this.lastSalesUpdateTime == 0L)
			{
				BaseProfile.StoreValue<long>(DateTime.Today.ToFileTime(), "SalesUpdateTime");
			}
			this.UpdateSales();
			this.inited = true;
		}

		private void UpdateSales()
		{
			if (TimeManager.AnotherDay(this.lastSalesUpdateTime))
			{
				SalesManager.currentSales = new Dictionary<int, int>();
				int count = ItemsManager.Instance.Items.Count;
				int num = this.SalesCount;
				int num2 = 10000;
				while (num > 0 && num2 > 0)
				{
					int index = UnityEngine.Random.Range(1, count);
					int key = ItemsManager.Instance.Items.ElementAt(index).Key;
					GameItem item = ItemsManager.Instance.GetItem(key);
					if (!ShopManager.Instance.BoughtAlredy(item) && !item.ShopVariables.isDivision && !item.ShopVariables.HideInShop && item.ShopVariables.price != 0 && !SalesManager.currentSales.ContainsKey(key))
					{
						int value = this.SalesValues[UnityEngine.Random.Range(0, this.SalesValues.Length)];
						SalesManager.currentSales.Add(key, value);
						num--;
						num2--;
					}
				}
				BaseProfile.StoreValue<Dictionary<int, int>>(SalesManager.currentSales, "CurrentSales");
				this.lastSalesUpdateTime = DateTime.Today.ToFileTime();
				BaseProfile.StoreValue<long>(this.lastSalesUpdateTime, "SalesUpdateTime");
			}
			else
			{
				SalesManager.currentSales = BaseProfile.ResolveValue<Dictionary<int, int>>("CurrentSales", new Dictionary<int, int>());
			}
			if (this.DebugLog)
			{
				foreach (KeyValuePair<int, int> keyValuePair in SalesManager.currentSales)
				{
					UnityEngine.Debug.Log(keyValuePair.Value + " sale for item " + ItemsManager.Instance.GetItem(keyValuePair.Key).ShopVariables.Name);
				}
			}
		}

		public static float GetSale(int itemID)
		{
			float result = 0f;
			if (SalesManager.currentSales.ContainsKey(itemID))
			{
				result = (float)SalesManager.currentSales[itemID];
			}
			return result;
		}

		//public int GetSale(int itemID, out Sprite sprite)
		//{
		//	sprite = new Sprite();
		//	int num = 0;
		//	if (SalesManager.currentSales.ContainsKey(itemID))
		//	{
		//		num = SalesManager.currentSales[itemID];
		//		if (num != 10)
		//		{
		//			if (num != 20)
		//			{
		//				if (num == 30)
		//				{
		//					sprite = this.SalesSprites[2];
		//				}
		//			}
		//			else
		//			{
		//				sprite = this.SalesSprites[1];
		//			}
		//		}
		//		else
		//		{
		//			sprite = this.SalesSprites[0];
		//		}
		//	}
		//	return num;
		//}

		private const string lastSalesUpdateKey = "SalesUpdateTime";

		private const string currentSalesKey = "CurrentSales";

		public bool DebugLog;

		public Sprite[] SalesSprites;

		public int SalesCount = 2;

		private readonly int[] SalesValues = new int[]
		{
			10,
			20,
			30
		};

		private long lastSalesUpdateTime;

		private static Dictionary<int, int> currentSales = new Dictionary<int, int>();

		private static SalesManager instance;

		private bool inited;
	}
}

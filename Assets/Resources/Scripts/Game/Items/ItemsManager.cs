using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Game.Items
{
	public class ItemsManager : MonoBehaviour
	{
		public static ItemsManager Instance
		{
			get
			{
				ItemsManager result;
				if ((result = ItemsManager.instance) == null)
				{
					result = (ItemsManager.instance = UnityEngine.Object.FindObjectOfType<ItemsManager>());
				}
				return result;
			}
		}

		public static bool HasInstance
		{
			get
			{
				return ItemsManager.instance != null;
			}
		}

		public Dictionary<int, GameItem> Items { get; private set; }

		[ContextMenu("FIX")]
		public void FixLinks()
		{
			string oldValue = "PreviewImg";
			foreach (GameItem gameItem in this.items)
			{
				foreach (Sprite sprite in this.ssss)
				{
					string a = new StringBuilder(sprite.name).Replace(oldValue, string.Empty).ToString();
					if (a == gameItem.ShopVariables.Name)
					{
						gameItem.ShopVariables.ItemIcon = sprite;
					}
				}
			}
		}

		private void Awake()
		{
			ItemsManager.instance = this;
		}

		public void Init()
		{
			if (this.inited)
			{
				return;
			}
			ItemsManager.instance = this;
			this.AssembleGameitems();
			this.inited = true;
		}

		public bool AddItem(GameItem item)
		{
			if (this.Items.ContainsKey(item.ID))
			{
				if (this.ShowDebug)
				{
					UnityEngine.Debug.LogWarning(item.ShopVariables.Name + " - предмет с таким ID уже сеществует. Пытаюсь переполучить ID.");
				}
				item.ID = this.GenerateID(item);
			}
			if (!this.Items.ContainsKey(item.ID))
			{
				this.Items.Add(item.ID, item);
				if (this.ShowDebug)
				{
					UnityEngine.Debug.LogFormat(item.gameObject, "{0} Added item name : {1}", new object[]
					{
						item.ID,
						item.gameObject.name
					});
				}
				return true;
			}
			if (this.ShowDebug)
			{
				UnityEngine.Debug.LogError(item.ShopVariables.Name + " не добавлен. Предмет с таким ID уже сеществует.");
			}
			return false;
		}

		public GameItem GetItem(int id)
		{
			GameItem result = null;
			bool flag = this.Items.TryGetValue(id, out result);
			if (this.ShowDebug)
			{
				foreach (KeyValuePair<int, GameItem> keyValuePair in this.Items)
				{
					UnityEngine.Debug.Log(string.Concat(new object[]
					{
						keyValuePair.Key,
						" ",
						keyValuePair.Value.ShopVariables.Name,
						" ",
						keyValuePair.Value == null
					}));
				}
			}
			return result;
		}

		public int GenerateID(GameItem item)
		{
			return item.ID = item.gameObject.GetInstanceID();
		}

		public Dictionary<int, GameItem> AssembleGameitems()
		{
			this.Items = new Dictionary<int, GameItem>();
			GameItem[] componentsInChildren = base.GetComponentsInChildren<GameItem>();
			foreach (GameItem gameItem in componentsInChildren)
			{
				ItemsManager.Instance.AddItem(gameItem);
				gameItem.Init();
			}
			return this.Items;
		}

		private static ItemsManager instance;

		public bool ShowDebug;

		public Sprite[] ssss;

		public GameItem[] items;

		private bool inited;
	}
}

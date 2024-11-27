using System;
using System.Collections;
using System.Collections.Generic;
using Game.Character.CharacterController;
using Game.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game.Character
{
	public class PlayerInfoManager : MonoBehaviour
	{
		public static int Money
		{
			get
			{
				return PlayerInfoManager.Instance.GetInfoValue(PlayerInfoType.Money);
			}
			set
			{
				PlayerInfoManager.Instance.SetInfoValue(PlayerInfoType.Money, value);
			}
		}

		public static int Gems
		{
			get
			{
				return PlayerInfoManager.Instance.GetInfoValue(PlayerInfoType.Gems);
			}
			set
			{
				PlayerInfoManager.Instance.SetInfoValue(PlayerInfoType.Gems, value);
			}
		}

		public static int AllRecievedGems
		{
			get
			{
				return PlayerInfoManager.Instance.GetInfoValue(PlayerInfoType.AllReceivedGems);
			}
			private set
			{
				PlayerInfoManager.Instance.SetInfoValue(PlayerInfoType.AllReceivedGems, value);
			}
		}

		public static int Experience
		{
			get
			{
				return PlayerInfoManager.Instance.GetInfoValue(PlayerInfoType.Experience);
			}
			set
			{
				PlayerInfoManager.Instance.SetInfoValue(PlayerInfoType.Experience, value);
			}
		}

		public static int Level
		{
			get
			{
				return PlayerInfoManager.Instance.GetInfoValue(PlayerInfoType.LvL);
			}
			set
			{
				PlayerInfoManager.Instance.SetInfoValue(PlayerInfoType.LvL, value);
			}
		}

		public static int VipLevel
		{
			get
			{
				return PlayerInfoManager.Instance.GetInfoValue(PlayerInfoType.VipLvL);
			}
			set
			{
				PlayerInfoManager.Instance.SetInfoValue(PlayerInfoType.VipLvL, value);
			}
		}

		public static int UpgradePoints
		{
			get
			{
				return PlayerInfoManager.Instance.GetInfoValue(PlayerInfoType.UpgradePoints);
			}
			set
			{
				PlayerInfoManager.Instance.SetInfoValue(PlayerInfoType.UpgradePoints, value);
			}
		}

		public static int TotalReceivedMoney
		{
			get
			{
				return PlayerInfoManager.Instance.GetInfoValue(PlayerInfoType.TotalReceivedMoney);
			}
			private set
			{
				PlayerInfoManager.Instance.SetInfoValue(PlayerInfoType.TotalReceivedMoney, value);
			}
		}

		public static int TotalSpentMoney
		{
			get
			{
				return PlayerInfoManager.Instance.GetInfoValue(PlayerInfoType.TotalSpentMoney);
			}
			private set
			{
				PlayerInfoManager.Instance.SetInfoValue(PlayerInfoType.TotalSpentMoney, value);
			}
		}

		public static int TotalTimeInGame
		{
			get
			{
				return PlayerInfoManager.Instance.GetInfoValue(PlayerInfoType.TotalTimeInGame);
			}
			set
			{
				PlayerInfoManager.Instance.SetInfoValue(PlayerInfoType.TotalTimeInGame, value);
			}
		}

		public static void ClearPlayerInfo()
		{
			IEnumerator enumerator = Enum.GetValues(typeof(PlayerInfoType)).GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					object obj = enumerator.Current;
					PlayerInfoType playerInfoType = (PlayerInfoType)obj;
					if (!PlayerInfoManager.NotClearedInfo.Contains(playerInfoType))
					{
						BaseProfile.ClearValue(playerInfoType + "PlayerInfo");
					}
				}
			}
			finally
			{
				IDisposable disposable;
				if ((disposable = (enumerator as IDisposable)) != null)
				{
					disposable.Dispose();
				}
			}
			int value = BaseProfile.ResolveValue<int>(PlayerInfoType.AllReceivedGems + "PlayerInfo", 0);
			BaseProfile.StoreValue<int>(value, PlayerInfoType.Gems + "PlayerInfo");
			if (PlayerInfoManager.Instance != null)
			{
				PlayerInfoManager.Instance.InitInfoDictionary();
			}
		}

		public int GetInfoValue(PlayerInfoType infoType)
		{
			return this.InfoItems[infoType].Value;
		}

		public int GetMaxValue(PlayerInfoType infoType)
		{
			return this.InfoItems[infoType].GetMaxValue();
		}

		public float GetVipMult(PlayerInfoType infoType)
		{
			return (float)PlayerInfoManager.VipLevel * this.InfoItems[infoType].GetVipMult();
		}

		public void SetInfoValue(PlayerInfoType infoType, int value)
		{
			this.InfoItems[infoType].Value = value;
			this.CallChangeEvents(infoType);
		}

		public void AddOnValueChangedEvent(PlayerInfoType infoType, PlayerInfoManager.OnInfoValueChanged onValueChanged)
		{
			if (!this.infoEvents.ContainsKey(infoType))
			{
				this.infoEvents.Add(infoType, new List<PlayerInfoManager.OnInfoValueChanged>());
			}
			this.infoEvents[infoType].Add(onValueChanged);
		}

		public void ChangeInfoValue(PlayerInfoType infoType, int amount, bool useVipMultipler = false)
		{
			PlayerInfoManager.InfoItem infoItem = this.InfoItems[infoType];
			infoItem.ChangeCurrentValue(amount, useVipMultipler);
			this.CallChangeEvents(infoType);
		}

		public void AddSpendMoney(int money)
		{
			PlayerInfoManager.TotalSpentMoney += money;
		}

		private void CallChangeEvents(PlayerInfoType infoType)
		{
			this.SaveItemInfoValue(infoType);
			if (!this.infoEvents.ContainsKey(infoType))
			{
				return;
			}
			PlayerInfoManager.InfoItem infoItem = this.InfoItems[infoType];
			foreach (PlayerInfoManager.OnInfoValueChanged onInfoValueChanged in this.infoEvents[infoType])
			{
				if (onInfoValueChanged != null)
				{
					onInfoValueChanged(infoItem.Value);
				}
			}
		}

		public void InitInfoDictionary()
		{
			IEnumerator enumerator = Enum.GetValues(typeof(PlayerInfoType)).GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					object obj = enumerator.Current;
					PlayerInfoType playerInfoType = (PlayerInfoType)obj;
					PlayerInfoManager.InfoItem infoItem = (!PlayerInfoManager.ItemDefaultValues.ContainsKey(playerInfoType)) ? new PlayerInfoManager.InfoItem() : PlayerInfoManager.ItemDefaultValues[playerInfoType];
					if (this.InfoItems.ContainsKey(playerInfoType))
					{
						this.InfoItems[playerInfoType] = new PlayerInfoManager.InfoItem(infoItem);
					}
					else
					{
						this.InfoItems.Add(playerInfoType, new PlayerInfoManager.InfoItem(infoItem));
					}
					this.InfoItems[playerInfoType].Value = BaseProfile.ResolveValue<int>(playerInfoType + "PlayerInfo", infoItem.Value);
				}
			}
			finally
			{
				IDisposable disposable;
				if ((disposable = (enumerator as IDisposable)) != null)
				{
					disposable.Dispose();
				}
			}
		}

		private void Awake()
		{
			if (PlayerInfoManager.Instance != null)
			{
				UnityEngine.Object.Destroy(base.gameObject);
				return;
			}
			PlayerInfoManager.Instance = this;
			SceneManager.sceneUnloaded += this.OnSceneUnload;
			SceneManager.sceneLoaded += this.OnSceneLoad;
			this.InitInfoDictionary();
			UnityEngine.Object.DontDestroyOnLoad(this);
		}

		private void OnSceneLoad(Scene scene, LoadSceneMode mode)
		{
			if (scene.name == "Game" || scene.name == "Menu")
			{
				this.AddOnValueChangedEvent(PlayerInfoType.Gems, new PlayerInfoManager.OnInfoValueChanged(this.SaveRecievedGems));
				this.AddOnValueChangedEvent(PlayerInfoType.Money, new PlayerInfoManager.OnInfoValueChanged(this.AddReceivedMoney));
				if (scene.name == "Game")
				{
					UIGame instance = UIGame.Instance;
					instance.OnPausePanelOpen = (Action)Delegate.Combine(instance.OnPausePanelOpen, new Action(this.OnPausePanelOpen));
				}
			}
		}

		private void OnSceneUnload(Scene scene)
		{
			if (scene.name == "Game" || scene.name == "Menu")
			{
				this.infoEvents.Clear();
				this.SaveAllInfoValues();
			}
		}

		private void SaveItemInfoValue(PlayerInfoType infoType)
		{
			PlayerInfoManager.InfoItem infoItem = this.InfoItems[infoType];
			if (!infoItem.AvaibleToSave())
			{
				return;
			}
			BaseProfile.StoreValue<int>(infoItem.Value, infoType + "PlayerInfo");
			infoItem.OnValueSave();
		}

		private void SaveAllInfoValues()
		{
			if (PlayerInfoManager.Instance != this)
			{
				return;
			}
			foreach (KeyValuePair<PlayerInfoType, PlayerInfoManager.InfoItem> keyValuePair in this.InfoItems)
			{
				BaseProfile.StoreValue<int>(keyValuePair.Value.Value, keyValuePair.Key + "PlayerInfo");
			}
		}

		private void OnApplicationPause(bool pauseStatus)
		{
			if (!pauseStatus)
			{
				return;
			}
			this.SaveAllInfoValues();
		}

		private void OnApplicationQuit()
		{
			if (PlayerManager.Instance)
			{
				PlayerManager.Instance.DefaultWeaponController.Deinitialization();
			}
			this.SaveAllInfoValues();
		}

		private void OnPausePanelOpen()
		{
			this.SaveAllInfoValues();
		}

		private void SaveRecievedGems(int newGemsValue)
		{
			int lastValueChange = this.InfoItems[PlayerInfoType.Gems].GetLastValueChange();
			if (lastValueChange > 0)
			{
				PlayerInfoManager.AllRecievedGems += lastValueChange;
			}
		}

		private void AddReceivedMoney(int money)
		{
			int lastValueChange = this.InfoItems[PlayerInfoType.Money].GetLastValueChange();
			if (lastValueChange >= 0)
			{
				PlayerInfoManager.TotalReceivedMoney += lastValueChange;
			}
		}

		private static readonly Dictionary<PlayerInfoType, PlayerInfoManager.InfoItem> ItemDefaultValues = new Dictionary<PlayerInfoType, PlayerInfoManager.InfoItem>
		{
			{
				PlayerInfoType.VipLvL,
				new PlayerInfoManager.InfoItem(0, 0, 10, 1f, 1)
			},
			{
				PlayerInfoType.LvL,
				new PlayerInfoManager.InfoItem(1, 1, 50, 1f, 1)
			},
			{
				PlayerInfoType.Money,
				new PlayerInfoManager.InfoItem(0, 0, 2147000000, 10f, 1000)
			},
			{
				PlayerInfoType.Experience,
				new PlayerInfoManager.InfoItem(0, 0, 1000000, 20f, 1000)
			},
			{
				PlayerInfoType.TotalSpentMoney,
				new PlayerInfoManager.InfoItem(0, -1000000, 1000000, 0f, 1000)
			}
		};

		private static readonly List<PlayerInfoType> NotClearedInfo = new List<PlayerInfoType>
		{
			PlayerInfoType.AllReceivedGems,
			PlayerInfoType.TotalReceivedMoney,
			PlayerInfoType.TotalSpentMoney,
			PlayerInfoType.TotalTimeInGame
		};

		private const int UniversalMaxValue = 1000000;

		private const int MoneyMaxValue = 2147000000;

		private const int VipLvLmaxValue = 10;

		private const int LevelMaxValue = 50;

		private const string SavedInfoSubstring = "PlayerInfo";

		private const int MoneyVipMultipler = 10;

		private const int ExpVipMultipler = 20;

		public static PlayerInfoManager Instance;

		public readonly Dictionary<PlayerInfoType, PlayerInfoManager.InfoItem> InfoItems = new Dictionary<PlayerInfoType, PlayerInfoManager.InfoItem>();

		private readonly Dictionary<PlayerInfoType, List<PlayerInfoManager.OnInfoValueChanged>> infoEvents = new Dictionary<PlayerInfoType, List<PlayerInfoManager.OnInfoValueChanged>>();

		private PlayerInfoManager.OnInfoValueChanged valueChanged;

		public class InfoItem
		{
			public InfoItem()
			{
				this.currentValue = 0;
				this.minValue = 0;
				this.maxValue = 1000000;
				this.vipMultipler = 1f;
				this.changeToSave = 1;
				this.lastSavedValue = this.currentValue;
			}

			public InfoItem(PlayerInfoManager.InfoItem other)
			{
				this.currentValue = other.currentValue;
				this.minValue = other.minValue;
				this.maxValue = other.maxValue;
				this.vipMultipler = other.vipMultipler;
				this.changeToSave = other.changeToSave;
				this.lastSavedValue = this.currentValue;
			}

			public InfoItem(int startValue, int minValue, int maxValue, float vipMultipler, int changeToSave)
			{
				this.currentValue = startValue;
				this.minValue = minValue;
				this.maxValue = maxValue;
				this.vipMultipler = vipMultipler;
				this.changeToSave = changeToSave;
				this.lastSavedValue = this.currentValue;
			}

			public int Value
			{
				get
				{
					return this.currentValue;
				}
				set
				{
					this.ChangeCurrentValue(value - this.currentValue, false);
				}
			}

			public void ChangeCurrentValue(int amount, bool useVipMultipler = false)
			{
				if (useVipMultipler)
				{
					amount = (int)((float)amount + this.vipMultipler * (float)PlayerInfoManager.VipLevel * (float)amount / 100f);
				}
				this.currentValue += amount;
				this.lastValueChange = amount;
				this.currentValue = Mathf.Clamp(this.Value, this.minValue, this.maxValue);
			}

			public float GetVipMult()
			{
				return this.vipMultipler;
			}

			public int GetMaxValue()
			{
				return this.maxValue;
			}

			public bool AvaibleToSave()
			{
				return this.currentValue - this.lastSavedValue >= this.changeToSave;
			}

			public void OnValueSave()
			{
				this.lastSavedValue = this.currentValue;
			}

			public int GetLastValueChange()
			{
				return this.lastValueChange;
			}

			private readonly int minValue;

			private readonly int maxValue;

			private readonly float vipMultipler;

			private readonly int changeToSave;

			private int currentValue;

			private int lastSavedValue;

			private int lastValueChange;
		}

		public delegate void OnInfoValueChanged(int newValue);
	}
}

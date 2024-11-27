using System;
using System.Collections.Generic;
using Game.Weapons;
using UnityEngine;

namespace Game.Shop
{
	public class PlayerStoreProfile : MonoBehaviour
	{
		public static PlayerStoreProfile Instance
		{
			get
			{
				if (PlayerStoreProfile.instance == null)
				{
					throw new Exception("PlayerStoreProfile is not initialized");
				}
				return PlayerStoreProfile.instance;
			}
		}

		private void Awake()
		{
			PlayerStoreProfile.instance = this;
		}

		public static void SaveLoadout()
		{
			BaseProfile.StoreValue<Loadout>(PlayerStoreProfile.CurrentLoadout, "Loadout");
		}

		public static void LoadLoadout()
		{
			PlayerStoreProfile.CurrentLoadout = BaseProfile.ResolveValueWitoutDefault<Loadout>("Loadout");
			if (PlayerStoreProfile.CurrentLoadout == null)
			{
				UnityEngine.Debug.LogWarning("Cant find saved loadout. Creating new one.");
				PlayerStoreProfile.CurrentLoadout = new Loadout();
				PlayerStoreProfile.CurrentLoadout.Weapons = new Dictionary<string, int>();
				PlayerStoreProfile.CurrentLoadout.Skin = new Dictionary<string, int>
				{
					{
						"HeadID",
						0
					},
					{
						"FaceID",
						0
					},
					{
						"BodyID",
						0
					},
					{
						"ArmsID",
						0
					},
					{
						"ForearmsID",
						0
					},
					{
						"HandsID",
						0
					},
					{
						"UpperLegsID",
						0
					},
					{
						"LowerLegsID",
						0
					},
					{
						"FootsID",
						0
					},
					{
						"HatID",
						0
					},
					{
						"GlassID",
						0
					},
					{
						"MaskID",
						0
					},
					{
						"LeftBraceletID",
						0
					},
					{
						"RightBraceletID",
						0
					},
					{
						"LeftHuckleID",
						0
					},
					{
						"RightHuckleID",
						0
					},
					{
						"LeftPalmID",
						0
					},
					{
						"RightPalmID",
						0
					},
					{
						"LeftToeID",
						0
					},
					{
						"RightToeID",
						0
					},
					{
						"ExternalBodyID",
						0
					},
					{
						"ExternalForearmsID",
						0
					},
					{
						"ExternalFootsID",
						0
					}
				};
			}
		}

		public static void ClearLoadout()
		{
			BaseProfile.ClearValue("Loadout");
		}

		public bool GetOldWeapon(string weaponName)
		{
			return BaseProfile.ResolveValue<bool>(weaponName, false);
		}

		private static PlayerStoreProfile instance;

		private const string loadoutSavingKey = "Loadout";

		private WeaponController weaponController;

		public static Loadout CurrentLoadout;

		[Serializable]
		public class WeaponProfile
		{
			public WeaponNameList WeaponName;

			public Weapon PlayerWeaponLink;
		}
	}
}

using System;
using Game.Items;
using Game.Shop;

namespace Game.Character.Superpowers
{
	public static class PlayerAbilityManager
	{
		public static void EnableEbilities()
		{
			foreach (GameItemAbility gameItemAbility in PlayerAbilityManager.ActiveAbilities)
			{
				if (gameItemAbility != null)
				{
					gameItemAbility.Enable();
				}
			}
			foreach (GameItemAbility gameItemAbility2 in PlayerAbilityManager.PasiveAbilities)
			{
				if (gameItemAbility2 != null)
				{
					gameItemAbility2.Enable();
				}
			}
		}

		public static int GetSlotIndex(GameItemAbility ability)
		{
			int result = -1;
			if (ability.IsActive)
			{
				for (int i = 0; i < PlayerAbilityManager.ActiveAbilities.Length; i++)
				{
					GameItemAbility gameItemAbility = PlayerAbilityManager.ActiveAbilities[i];
					if (!(gameItemAbility == null))
					{
						if (gameItemAbility.ID == ability.ID)
						{
							result = i;
							break;
						}
					}
				}
			}
			else
			{
				for (int j = 0; j < PlayerAbilityManager.PasiveAbilities.Length; j++)
				{
					GameItemAbility gameItemAbility2 = PlayerAbilityManager.PasiveAbilities[j];
					if (!(gameItemAbility2 == null))
					{
						if (gameItemAbility2.ID == ability.ID)
						{
							result = j;
							break;
						}
					}
				}
			}
			return result;
		}

		public static void SaveAbilities()
		{
			for (int i = 0; i < PlayerAbilityManager.ActiveAbilities.Length; i++)
			{
				if (PlayerAbilityManager.ActiveAbilities[i] != null)
				{
					PlayerStoreProfile.CurrentLoadout.ActiveSuperpowers[i] = PlayerAbilityManager.ActiveAbilities[i].ID;
				}
				else
				{
					PlayerStoreProfile.CurrentLoadout.ActiveSuperpowers[i] = 0;
				}
			}
			for (int j = 0; j < PlayerAbilityManager.PasiveAbilities.Length; j++)
			{
				if (PlayerAbilityManager.PasiveAbilities[j] != null)
				{
					PlayerStoreProfile.CurrentLoadout.PasiveSuperpowers[j] = PlayerAbilityManager.PasiveAbilities[j].ID;
				}
				else
				{
					PlayerStoreProfile.CurrentLoadout.PasiveSuperpowers[j] = 0;
				}
			}
			PlayerStoreProfile.SaveLoadout();
		}

		public static void LoadAbilities()
		{
			int[] activeSuperpowers = PlayerStoreProfile.CurrentLoadout.ActiveSuperpowers;
			int[] pasiveSuperpowers = PlayerStoreProfile.CurrentLoadout.PasiveSuperpowers;
			for (int i = 0; i < activeSuperpowers.Length; i++)
			{
				if (activeSuperpowers[i] != 0)
				{
					PlayerAbilityManager.ActiveAbilities[i] = (GameItemAbility)ItemsManager.Instance.GetItem(activeSuperpowers[i]);
				}
			}
			for (int j = 0; j < pasiveSuperpowers.Length; j++)
			{
				if (pasiveSuperpowers[j] != 0)
				{
					PlayerAbilityManager.PasiveAbilities[j] = (GameItemAbility)ItemsManager.Instance.GetItem(pasiveSuperpowers[j]);
				}
			}
		}

		public static bool IsAbilityAdded(GameItemAbility ability)
		{
			if (ability.IsActive)
			{
				foreach (GameItemAbility gameItemAbility in PlayerAbilityManager.ActiveAbilities)
				{
					if (gameItemAbility && gameItemAbility.ID == ability.ID)
					{
						return true;
					}
				}
			}
			if (!ability.IsActive)
			{
				foreach (GameItemAbility gameItemAbility2 in PlayerAbilityManager.PasiveAbilities)
				{
					if (gameItemAbility2 && gameItemAbility2.ID == ability.ID)
					{
						return true;
					}
				}
			}
			return false;
		}

		public static void AddAbility(GameItemAbility ability, int slotNumber)
		{
			GameItemAbility gameItemAbility;
			if (ability.IsActive)
			{
				gameItemAbility = PlayerAbilityManager.ActiveAbilities[slotNumber];
				PlayerAbilityManager.ActiveAbilities[slotNumber] = ability;
			}
			else
			{
				gameItemAbility = PlayerAbilityManager.PasiveAbilities[slotNumber];
				PlayerAbilityManager.PasiveAbilities[slotNumber] = ability;
			}
			if (gameItemAbility != null)
			{
				gameItemAbility.Disable();
			}
			ability.Enable();
			PlayerAbilityManager.SaveAbilities();
		}

		public static void RemoveAbility(GameItemAbility ability)
		{
			if (ability.IsActive)
			{
				for (int i = 0; i < PlayerAbilityManager.ActiveAbilities.Length; i++)
				{
					GameItemAbility gameItemAbility = PlayerAbilityManager.ActiveAbilities[i];
					if (!(gameItemAbility == null))
					{
						if (gameItemAbility.ID == ability.ID)
						{
							PlayerAbilityManager.ActiveAbilities.SetValue(null, i);
							ability.Disable();
							break;
						}
					}
				}
			}
			else
			{
				for (int j = 0; j < PlayerAbilityManager.PasiveAbilities.Length; j++)
				{
					GameItemAbility gameItemAbility2 = PlayerAbilityManager.PasiveAbilities[j];
					if (!(gameItemAbility2 == null))
					{
						if (gameItemAbility2.ID == ability.ID)
						{
							PlayerAbilityManager.PasiveAbilities.SetValue(null, j);
							ability.Disable();
							break;
						}
					}
				}
			}
			PlayerAbilityManager.SaveAbilities();
		}

		public static bool SkinCanBeRemoved(GameItemSkin skin, GameItemAbility ability)
		{
			for (int i = 0; i < PlayerAbilityManager.ActiveAbilities.Length; i++)
			{
				if (!(PlayerAbilityManager.ActiveAbilities[i] == null) && !(PlayerAbilityManager.ActiveAbilities[i] == ability) && PlayerAbilityManager.ActiveAbilities[i].RelatedSkins.Length > 0)
				{
					for (int j = 0; j < PlayerAbilityManager.ActiveAbilities[i].RelatedSkins.Length; j++)
					{
						if (PlayerAbilityManager.ActiveAbilities[i].RelatedSkins[j] == skin)
						{
							return false;
						}
					}
				}
			}
			for (int k = 0; k < PlayerAbilityManager.PasiveAbilities.Length; k++)
			{
				if (!(PlayerAbilityManager.PasiveAbilities[k] == null) && !(PlayerAbilityManager.PasiveAbilities[k] == ability) && PlayerAbilityManager.PasiveAbilities[k].RelatedSkins.Length > 0)
				{
					for (int l = 0; l < PlayerAbilityManager.PasiveAbilities[k].RelatedSkins.Length; l++)
					{
						if (PlayerAbilityManager.PasiveAbilities[k].RelatedSkins[l] == skin)
						{
							return false;
						}
					}
				}
			}
			return true;
		}

		public static GameItemAbility[] ActiveAbilities = new GameItemAbility[3];

		public static GameItemAbility[] PasiveAbilities = new GameItemAbility[5];
	}
}

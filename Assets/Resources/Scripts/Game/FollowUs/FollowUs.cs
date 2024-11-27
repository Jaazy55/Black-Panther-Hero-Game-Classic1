using System;
using Game.Character;
using UnityEngine;
using UnityEngine.UI;

namespace Game.FollowUs
{
	public class FollowUs : MonoBehaviour
	{
		public static bool BonusUsed
		{
			get
			{
				return BaseProfile.ResolveValue<bool>("SocialBonus", false);
			}
			set
			{
				BaseProfile.StoreValue<bool>(value, "SocialBonus");
			}
		}

		private void Start()
		{
			this.ChangeText();
		}

		public void openVkPage()
		{
			Application.OpenURL("");
			if (!FollowUs.BonusUsed)
			{
				FollowUs.BonusUsed = true;
				PlayerInfoManager.Money += this.bonus;
			}
			this.ChangeText();
		}

		public void openFBPage()
		{
			Application.OpenURL("");
			if (!FollowUs.BonusUsed)
			{
				FollowUs.BonusUsed = true;
				PlayerInfoManager.Money += this.bonus;
			}
			this.ChangeText();
		}

		private void ChangeText()
		{
			this.bonusText.text = ((!FollowUs.BonusUsed) ? "follow us to get bonuses 2000" : "follow us");
		}

		public int bonus = 2000;

		public Text bonusText;
	}
}

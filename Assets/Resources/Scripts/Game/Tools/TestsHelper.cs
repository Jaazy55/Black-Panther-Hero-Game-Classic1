using System;
using Game.Character;
using Game.Character.CharacterController;
using Game.GlobalComponent.HelpfulAds;
using UnityEngine;

namespace Game.Tools
{
	public class TestsHelper : MonoBehaviour
	{
		private void Awake()
		{
			if (!this.CurrentHitEntity)
			{
				this.CurrentHitEntity = PlayerInteractionsManager.Instance.Player;
			}
		}

		public void SetDamage()
		{
			if (!this.CurrentHitEntity)
			{
				throw new Exception("Don't have hit entity for set damage");
			}
			this.CurrentHitEntity.OnHit(DamageType.Instant, null, this.dmg, Vector3.zero, Vector3.zero, 0f);
		}

		public void ChangeOnRagdoll()
		{
			if (!this.CurrentHitEntity)
			{
				throw new Exception("Don't have hit entity for change on ragdoll");
			}
			Human human = this.CurrentHitEntity as Human;
			if (!human)
			{
				throw new Exception("Can't change on ragdoll");
			}
			human.ReplaceOnRagdoll(this.CanWakeUp, false);
		}

		public void HelpfulAds()
		{
			HelpfullAdsManager.Instance.HelpTimerLength = 0f;
		}

		public HitEntity CurrentHitEntity;

		public float dmg = 100f;

		[Space(6f)]
		[InspectorButton("SetDamage")]
		public bool damage;

		[Space(6f)]
		public bool CanWakeUp = true;

		[Space(6f)]
		[InspectorButton("ChangeOnRagdoll")]
		public bool ragdoll;

		[Space(6f)]
		[InspectorButton("HelpfulAds")]
		public bool adsReset;
	}
}

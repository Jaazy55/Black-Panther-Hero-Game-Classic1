using System;
using Game.Character.CharacterController;
using Game.Effects;
using Game.GlobalComponent;
using UnityEngine;

namespace Naxeex.Entity
{
	public class EffectDyingHitEntity : HitEntity, IInitable
	{
		public virtual void Init()
		{
			this.Initialization(true);
		}

		public void DeInit()
		{
		}

		protected override void OnDie()
		{
			base.OnDie();
			if (this.m_DeadEffectPrefab != null)
			{
				this.m_DeadEffect = PoolManager.Instance.GetFromPool<Explosion>(this.m_DeadEffectPrefab, base.transform.position + this.m_Offset, base.transform.rotation);
				if (this.m_DeadEffect != null)
				{
					this.m_DeadEffect.Init(this.LastHitOwner, new GameObject[]
					{
						base.gameObject
					});
				}
			}
			PoolManager.Instance.ReturnToPool(base.gameObject);
		}

		[Separator("Effect Dying Options")]
		[SerializeField]
		private Explosion m_DeadEffectPrefab;

		[SerializeField]
		private Vector3 m_Offset;

		private Explosion m_DeadEffect;
	}
}

using System;
using Game.Character;
using Game.Character.CharacterController;
using UnityEngine;

public class ImuneToFallCombo : ComboManager.BaseComboEffect
{
	protected override void Start()
	{
		base.Start();
		this.player = PlayerInteractionsManager.Instance.Player;
		this.originalCollisionInvul = this.player.RDCollInvul;
		this.originalExplosionInvul = this.player.RDExpInvul;
	}

	public override void AddStackEffect()
	{
		if (this.CollisionInvul)
		{
			this.player.RDCollInvul = true;
		}
		if (this.ExplosionInvul)
		{
			this.player.RDExpInvul = true;
		}
	}

	public override void ClearStacksEffects()
	{
		this.player.RDCollInvul = this.originalCollisionInvul;
		this.player.RDExpInvul = this.originalExplosionInvul;
	}

	[Separator("ImuneToFallCombo")]
	public bool CollisionInvul;

	[Space(5f)]
	public bool ExplosionInvul;

	private Player player;

	private bool originalCollisionInvul;

	private bool originalExplosionInvul;
}

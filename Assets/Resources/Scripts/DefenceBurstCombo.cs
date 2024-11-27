using System;
using Game.Character;
using Game.Character.CharacterController;

public class DefenceBurstCombo : ComboManager.BaseComboEffect
{
	protected override void Start()
	{
		base.Start();
		this.player = PlayerInteractionsManager.Instance.Player;
		this.originalDefence = this.player.Defence;
	}

	public override void AddStackEffect()
	{
		this.player.Defence = this.originalDefence + this.DefencePerStack * (float)this.currStacks;
	}

	public override void ClearStacksEffects()
	{
		this.player.Defence = this.originalDefence;
	}

	[Separator("DefenceBurstCombo")]
	public Defence DefencePerStack;

	private Defence originalDefence;

	private Player player;
}

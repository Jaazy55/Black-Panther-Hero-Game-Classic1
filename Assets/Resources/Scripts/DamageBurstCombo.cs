using System;
using Game.Weapons;

public class DamageBurstCombo : ComboManager.BaseComboEffect
{
	protected override void Start()
	{
		base.Start();
		this.linkedWeapon = base.GetComponent<SpecialWeaponComboController>().LinkedWeapon;
		this.originalDamage = this.linkedWeapon.Damage;
	}

	public override void AddStackEffect()
	{
		float damage = this.originalDamage + (float)this.currStacks * this.originalDamage * this.DamagePercentPerStack / 100f;
		if (this.linkedWeapon is MeleeWeapon)
		{
			(this.linkedWeapon as MeleeWeapon).SetDamage(damage);
		}
		else
		{
			this.linkedWeapon.Damage = damage;
		}
	}

	public override void ClearStacksEffects()
	{
		if (this.linkedWeapon is MeleeWeapon)
		{
			(this.linkedWeapon as MeleeWeapon).SetDamage(this.originalDamage);
		}
		else
		{
			this.linkedWeapon.Damage = this.originalDamage;
		}
	}

	[Separator("DamageBurstCombo")]
	public float DamagePercentPerStack = 10f;

	private Weapon linkedWeapon;

	private float originalDamage;
}

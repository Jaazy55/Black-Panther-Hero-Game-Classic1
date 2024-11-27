using System;
using Game.Character;
using Game.Character.CharacterController;
using Game.Weapons;
using UnityEngine;

public class SpecialWeaponComboController : MonoBehaviour
{
	private void OnPlayerDie(float deathTime)
	{
		this.StopAllEffects();
	}

	private void OnPlayerEnterVehicle(bool isIn)
	{
		this.StopAllEffects();
	}

	private void Start()
	{
		if (ComboManager.Instance != null)
		{
			if (this.WeaponDependent)
			{
				ComboManager instance = ComboManager.Instance;
				instance.WeaponComboEvent = (ComboManager.ComboDelegate)Delegate.Combine(instance.WeaponComboEvent, new ComboManager.ComboDelegate(this.OnCombo));
			}
			else
			{
				ComboManager instance2 = ComboManager.Instance;
				instance2.OverallComboEvent = (ComboManager.ComboDelegate)Delegate.Combine(instance2.OverallComboEvent, new ComboManager.ComboDelegate(this.OnCombo));
			}
		}
		if (this.WeaponDependent && !this.LinkedWeapon)
		{
			this.LinkedWeapon = base.GetComponent<Weapon>();
		}
		if (this.WeaponDependent && !this.LinkedWeapon)
		{
			this.comboAllowed = false;
			if (this.DebugLog)
			{
				UnityEngine.Debug.LogError("Не выставленно комбо-оружие! Комбо заблокированно!");
			}
		}
		if (PlayerDieManager.Instance != null)
		{
			PlayerDieManager instance3 = PlayerDieManager.Instance;
			instance3.PlayerDiedEvent = (PlayerDieManager.PlayerDied)Delegate.Combine(instance3.PlayerDiedEvent, new PlayerDieManager.PlayerDied(this.OnPlayerDie));
		}
		if (PlayerInteractionsManager.Instance.Player != null)
		{
			PlayerInteractionsManager.Instance.Player.PlayerGetInOutVehicleEvent = new Player.PlayerGetInOutVehicle(this.OnPlayerEnterVehicle);
		}
	}

	private void Update()
	{
		this.AutoAllowCombo();
	}

	private void OnCombo(ComboManager.ComboInfo comboInfo)
	{
		if (this.comboAllowed)
		{
			if (this.WeaponDependent && comboInfo.WeaponNameInList != this.LinkedWeapon.WeaponNameInList)
			{
				ComboManager.Instance.UpdateComboMeter(0, string.Empty, true);
				return;
			}
			if (this.DebugLog)
			{
				UnityEngine.Debug.Log(string.Concat(new object[]
				{
					"X",
					comboInfo.ComboMultiplier,
					" COMBO with ",
					comboInfo.WeaponNameInList,
					"!"
				}));
			}
			if (this.WeaponDependent)
			{
				ComboManager.Instance.UpdateComboMeter(comboInfo.ComboMultiplier, (!string.Equals(this.LinkedWeapon.Name, string.Empty)) ? this.LinkedWeapon.Name : this.LinkedWeapon.WeaponNameInList.ToString(), false);
			}
			else
			{
				ComboManager.Instance.UpdateComboMeter(comboInfo.ComboMultiplier, comboInfo.WeaponNameInList.ToString(), false);
			}
			foreach (SpecialWeaponComboController.ComboEffectTrigger comboEffectTrigger in this.comboEffectTriggers)
			{
				if (comboInfo.ComboMultiplier >= comboEffectTrigger.ComboTriggerMultiplier)
				{
					foreach (ComboManager.BaseComboEffect baseComboEffect in comboEffectTrigger.ComboEffects)
					{
						if (baseComboEffect.IsFinisher)
						{
							this.StopAllEffects();
							this.DisallowCombo(baseComboEffect.EffectDuration);
						}
						baseComboEffect.StartEffect(comboInfo);
					}
				}
			}
		}
		else
		{
			ComboManager.Instance.UpdateComboMeter(0, string.Empty, true);
		}
	}

	private void StopAllEffects()
	{
		foreach (SpecialWeaponComboController.ComboEffectTrigger comboEffectTrigger in this.comboEffectTriggers)
		{
			foreach (ComboManager.BaseComboEffect baseComboEffect in comboEffectTrigger.ComboEffects)
			{
				baseComboEffect.StopEffect();
			}
		}
	}

	private void DisallowCombo(float cooldown)
	{
		this.stopCDTime = Time.time + cooldown;
		this.comboAllowed = false;
	}

	private void AutoAllowCombo()
	{
		if (this.stopCDTime != 0f && Time.time >= this.stopCDTime)
		{
			this.stopCDTime = 0f;
			this.comboAllowed = true;
			ComboManager.Instance.ResetWeaponCombo();
			ComboManager.Instance.UpdateComboMeter(0, string.Empty, true);
		}
	}

	public bool DebugLog;

	[Space(10f)]
	public bool WeaponDependent = true;

	public Weapon LinkedWeapon;

	[Space(10f)]
	public SpecialWeaponComboController.ComboEffectTrigger[] comboEffectTriggers;

	private bool comboAllowed = true;

	private float stopCDTime;

	[Serializable]
	public class ComboEffectTrigger
	{
		public int ComboTriggerMultiplier;

		public ComboManager.BaseComboEffect[] ComboEffects;
	}
}

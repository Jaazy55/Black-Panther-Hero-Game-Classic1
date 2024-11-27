using System;
using System.Collections.Generic;
using Game.Character.CameraEffects;
using Game.Character.CharacterController;
using Game.Character.Stats;
using Game.GlobalComponent;
using Game.Items;
using Game.Shop;
using Game.UI;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Weapons
{
	public class WeaponController : MonoBehaviour
	{
		public Weapon CurrentWeapon
		{
			get
			{
				return this.currentWeapon;
			}
		}

		public void Initialization()
		{
			this.playerProfile = base.GetComponent<PlayerStoreProfile>();
			this.player = base.GetComponent<Player>();
			this.ActivateFists();
			this.Owner = base.GetComponent<HitEntity>();
			if (this.Owner == null)
			{
				UnityEngine.Debug.LogError("Can't find owner!");
			}
			if (this.Owner is Player && this.AmmoText == null)
			{
				UnityEngine.Debug.LogError("AmmoText is not set");
			}
			BaseSoundController component = base.GetComponent<BaseSoundController>();
			if (component)
			{
				component.IsInGameSound = true;
			}
			this.attackTargetParameters.AttackMethod = AttackMethod.None;
			UIGame instance = UIGame.Instance;
			instance.OnExitInMenu = (Action)Delegate.Combine(instance.OnExitInMenu, new Action(this.OnExitToMenu));
		}

		public void Deinitialization()
		{
			if (this.currentWeapon != null)
			{
				this.CurrentWeapon.DeInit();
			}
		}

		private void OnExitToMenu()
		{
			if (PlayerManager.Instance.DefaultWeaponController == this)
			{
				this.Deinitialization();
			}
		}

		public void UpdatePlayerStats()
		{
			if (this.player && !this.player.IsTransformer)
			{
				this.meleeWeaponDamageMultipler = this.player.stats.GetPlayerStat(StatsList.MeleeWeaponDamage);
				if (this.WeaponSet.Slots[1].WeaponInstance)
				{
					MeleeWeapon meleeWeapon = (MeleeWeapon)this.WeaponSet.Slots[1].WeaponInstance;
					meleeWeapon.UpdateStats(this.player);
				}
			}
		}

		public void SwitchWeapon(bool right)
		{
			if (this.Weapons != null && this.Weapons.Count > 1)
			{
				if (right)
				{
					this.currentWeaponIndex++;
				}
				else
				{
					this.currentWeaponIndex--;
					while (this.currentWeaponIndex < 0)
					{
						this.currentWeaponIndex += this.Weapons.Count;
					}
				}
				this.InitWeapon(this.currentWeaponIndex);
			}
		}

		private void GetWeapon(string name)
		{
			if (this.Weapons == null || this.Weapons.Count <= 0)
			{
				return;
			}
			int i;
			for (i = 0; i < this.Weapons.Count; i++)
			{
				if (this.Weapons[i].gameObject.name.Equals(name))
				{
					this.currentWeaponIndex = i;
					break;
				}
			}
			if (i >= this.Weapons.Count)
			{
				this.currentWeaponIndex = 0;
			}
			this.currentWeapon = this.Weapons[this.currentWeaponIndex];
			if (this.CurrentWeapon == null)
			{
				this.currentWeapon = this.Weapons[0];
			}
		}

		private void GetWeapon(int index)
		{
			if (this.Weapons == null || this.Weapons.Count <= 0)
			{
				return;
			}
			index %= this.Weapons.Count;
			this.currentWeaponIndex = index;
			this.currentWeapon = this.Weapons[this.currentWeaponIndex];
		}

		public void HideWeapon()
		{
			this.currentWeapon.gameObject.SetActive(false);
		}

		public void ShowWeapon()
		{
			this.currentWeapon.gameObject.SetActive(true);
		}

		public bool ActivateWeaponByType(WeaponArchetype weapArchetype)
		{
			if (this.Weapons == null || this.Weapons.Count <= 1)
			{
				return false;
			}
			int num = -1;
			for (int i = 0; i < this.Weapons.Count; i++)
			{
				if (this.Weapons[i].Archetype == weapArchetype)
				{
					num = i;
					if (weapArchetype != WeaponArchetype.Ranged)
					{
						break;
					}
					if (this.Weapons[i].GetComponent<RangedWeapon>().AmmoCount > 0)
					{
						break;
					}
					num = -1;
				}
			}
			if (num == -1)
			{
				return false;
			}
			this.InitWeapon(num);
			return true;
		}

		private void InitWeapon()
		{
			if (this.CurrentWeapon == null)
			{
				return;
			}
			if (this.WeaponImage)
			{
				this.WeaponImage.sprite = this.CurrentWeapon.image;
			}
			this.CurrentWeapon.gameObject.SetActive(true);
			this.CurrentWeapon.Init();
			this.CurrentWeapon.PerformAttackEvent = new Weapon.AttackEvent(this.PerformAttackEvent);
			this.CurrentWeapon.InflictDamageEvent = new Weapon.DamageEvent(this.InflictDamageEvent);
			RangedWeapon rangedWeapon = this.CurrentWeapon as RangedWeapon;
			if (rangedWeapon && rangedWeapon.EnergyCost == 0f)
			{
				rangedWeapon.AmmoChangedEvent = new Weapon.AttackEvent(this.AmmoChanged);
				rangedWeapon.RechargeStartedEvent = new Weapon.AttackEvent(this.RechargeStarted);
				if (this.AmmoText)
				{
					this.AmmoText.enabled = true;
				}
				this.UpdateAmmoText(rangedWeapon);
			}
			else if (this.AmmoText)
			{
				this.AmmoText.enabled = false;
			}
		}

		private void DeInitWeapon(Weapon weapon)
		{
			if (weapon == null)
			{
				return;
			}
			weapon.PerformAttackEvent = null;
			weapon.InflictDamageEvent = null;
			weapon.DeInit();
			if (this.AmmoText)
			{
				this.AmmoText.enabled = false;
			}
			weapon.gameObject.SetActive(false);
		}

		public void InitWeapon(int index)
		{
			this.DeInitWeapon(this.CurrentWeapon);
			this.GetWeapon(index);
			this.InitWeapon();
		}

		private void RechargeStarted(Weapon weapon)
		{
			RangedWeapon rangedWeapon = weapon as RangedWeapon;
			if (rangedWeapon)
			{
				rangedWeapon.PlayRechargeSound(this.AudioSource);
			}
		}

		private void AmmoChanged(Weapon weapon)
		{
			RangedWeapon rangedWeapon = weapon as RangedWeapon;
			if (rangedWeapon)
			{
				this.UpdateAmmoText(rangedWeapon);
			}
		}

		private void InflictDamageEvent(Weapon weapon, HitEntity owner, HitEntity victim, Vector3 hitPos, Vector3 hitVector, float defenceReduction = 0f)
		{
			float num = 1f;
			if (this.player && weapon.Archetype == WeaponArchetype.Melee)
			{
				num = this.meleeWeaponDamageMultipler;
			}
			float damage = weapon.Damage * num;
			victim.OnHit(weapon.WeaponDamageType, owner, damage, hitPos, hitVector, defenceReduction);
		}

		private void PerformAttackEvent(Weapon weapon)
		{
			weapon.PlayAttackSound(this.AudioSource);
			RangedWeapon rangedWeapon = weapon as RangedWeapon;
			if (rangedWeapon)
			{
				this.MakeShotFlash(rangedWeapon.Muzzle, rangedWeapon.ShotSfx);
				this.MakeShotFireKick(rangedWeapon.FireKickPower);
			}
		}

		private void MakeShotFlash(Transform muzzle, ShotSFXType type)
		{
			if (WeaponManager.Instance && muzzle)
			{
				WeaponManager.Instance.StartShootSFX(muzzle, type);
			}
		}

		private void MakeShotFireKick(float kickPower)
		{
			if (!(this.Owner is Player))
			{
				return;
			}
			FireKick fireKick = EffectManager.Instance.Create<FireKick>();
			fireKick.KickAngle = kickPower;
			fireKick.Play();
		}

		public void UpdateAmmoText()
		{
			RangedWeapon rangedWeapon = this.currentWeapon as RangedWeapon;
			if (rangedWeapon != null)
			{
				this.UpdateAmmoText(rangedWeapon);
			}
		}

		public void UpdateAmmoText(RangedWeapon rangedWeapon)
		{
			if (!(this.Owner is Player) || !this.AmmoText)
			{
				return;
			}
			this.AmmoText.text = rangedWeapon.AmmoCountText;
		}

		public void Aim()
		{
		}

		public void Hold()
		{
		}

		public void Attack()
		{
			if (this.CurrentWeapon)
			{
				this.attackTargetParameters.AttackMethod = AttackMethod.Attack;
			}
		}

		public void Attack(HitEntity entity)
		{
			if (this.CurrentWeapon)
			{
				this.attackTargetParameters.AttackMethod = AttackMethod.AttackEntity;
				this.attackTargetParameters.Victim = entity;
			}
		}

		public void Attack(Vector3 direction)
		{
			if (this.CurrentWeapon)
			{
				this.attackTargetParameters.AttackMethod = AttackMethod.AttackByDirection;
				this.attackTargetParameters.Direction = direction;
			}
		}

		public void AttackWithWeapon()
		{
			this.InvokedAttack();
		}

		private void InvokedAttack()
		{
			switch (this.attackTargetParameters.AttackMethod)
			{
			case AttackMethod.Attack:
				this.CurrentWeapon.Attack(this.Owner);
				break;
			case AttackMethod.AttackByDirection:
				this.CurrentWeapon.Attack(this.Owner, this.attackTargetParameters.Direction);
				break;
			case AttackMethod.AttackEntity:
				this.CurrentWeapon.Attack(this.Owner, this.attackTargetParameters.Victim);
				break;
			default:
				UnityEngine.Debug.LogError("Unsupported attack method!");
				break;
			}
		}

		public void MeleeWeaponAttack(int attackState)
		{
			MeleeWeapon meleeWeapon = this.currentWeapon as MeleeWeapon;
			if (meleeWeapon != null)
			{
				meleeWeapon.MeleeAttack(attackState);
			}
		}

		public void AddWeaponInList(Weapon newWeapon)
		{
			if (!this.Weapons.Contains(newWeapon))
			{
				this.Weapons.Add(newWeapon);
			}
		}

		public void RemoveWeaponFromList(Weapon oldWeapon)
		{
			if (this.Weapons.Contains(oldWeapon))
			{
				this.ActivateWeaponByType(WeaponArchetype.Melee);
				this.Weapons.Remove(oldWeapon);
			}
		}

		public bool CheckIsThisWeaponControllerWeapon(Weapon weapon)
		{
			return this.currentWeapon == weapon;
		}

		public void CheckReloadOnWakeUp()
		{
			RangedWeapon rangedWeapon = this.currentWeapon as RangedWeapon;
			if (rangedWeapon != null && rangedWeapon.AmmoInCartridgeCount <= 0)
			{
				rangedWeapon.RechargeFinish();
			}
		}

		public void LostCurrentWeapon()
		{
		}

		public WeaponSlotTypes GetTargetSlot(Weapon incWeapon)
		{
			if (incWeapon.Type == WeaponTypes.None)
			{
				return WeaponSlotTypes.None;
			}
			if (incWeapon.Type == WeaponTypes.Melee)
			{
				return WeaponSlotTypes.Melee;
			}
			if (incWeapon.Type == WeaponTypes.Pistol || incWeapon.Type == WeaponTypes.SMG)
			{
				return WeaponSlotTypes.Additional;
			}
			if (incWeapon.Type == WeaponTypes.Rifle || incWeapon.Type == WeaponTypes.Shotgun)
			{
				return WeaponSlotTypes.Main;
			}
			if (incWeapon.Type == WeaponTypes.Heavy)
			{
				return WeaponSlotTypes.Heavy;
			}
			return WeaponSlotTypes.Universal;
		}

		public bool WeaponSlotIsEmpty(int targetSlotIndex)
		{
			return this.WeaponSet.Slots[targetSlotIndex].WeaponPrefab == null;
		}

		public int EquipWeapon(GameItemWeapon weaponItem, int slotIndex, bool temporary = false)
		{
			if (this.WeaponSet.Locked || slotIndex >= this.WeaponSet.Slots.Length)
			{
				return -1;
			}
			Loadout currentLoadout = PlayerStoreProfile.CurrentLoadout;
			string key = Loadout.KeyFromWeaponSlot(slotIndex);
			if (this.currentSlotIndex == slotIndex)
			{
				this.ActivateFists();
			}
			int result = (!currentLoadout.Weapons.ContainsKey(key)) ? 0 : currentLoadout.Weapons[key];
			if (currentLoadout.Weapons.ContainsKey(key))
			{
				currentLoadout.Weapons[key] = weaponItem.ID;
			}
			else
			{
				currentLoadout.Weapons.Add(key, weaponItem.ID);
			}
			this.WeaponSet.Slots[slotIndex].WeaponPrefab = weaponItem.Weapon;
			this.UpdateWeaponSlotPlaceholder(slotIndex);
			RangedWeapon rangedWeapon = this.WeaponSet.Slots[slotIndex].WeaponInstance as RangedWeapon;
			if (rangedWeapon != null)
			{
				rangedWeapon.IsFiniteAmmo = !temporary;
			}
			MeleeWeapon x = this.WeaponSet.Slots[slotIndex].WeaponInstance as MeleeWeapon;
			if (x != null)
			{
				this.UpdatePlayerStats();
			}
			if (!temporary)
			{
				PlayerStoreProfile.SaveLoadout();
			}
			return result;
		}

		public void UnEquipWeapon(int slotIndex)
		{
			if (this.WeaponSet.Locked || slotIndex >= this.WeaponSet.Slots.Length)
			{
				return;
			}
			if (this.currentSlotIndex == slotIndex)
			{
				this.ActivateFists();
			}
			Loadout currentLoadout = PlayerStoreProfile.CurrentLoadout;
			string key = "WeaponSlot" + slotIndex;
			if (currentLoadout.Weapons.ContainsKey(key))
			{
				currentLoadout.Weapons[key] = 0;
			}
			else
			{
				currentLoadout.Weapons.Add(key, 0);
			}
			this.WeaponSet.Slots[slotIndex].WeaponPrefab = null;
			if (this.WeaponSet.Slots[slotIndex].WeaponInstance)
			{
				PoolManager.Instance.ReturnToPool(this.WeaponSet.Slots[slotIndex].WeaponInstance);
			}
			this.WeaponSet.Slots[slotIndex].WeaponInstance = null;
			PlayerStoreProfile.SaveLoadout();
		}

		public void ChooseSlot(int slotIndex)
		{
			if (this.WeaponSet.Locked)
			{
				return;
			}
			this.DeInitWeapon(this.currentWeapon);
			this.currentWeapon = this.WeaponSet.Slots[slotIndex].WeaponInstance;
			this.InitWeapon();
			this.currentSlotIndex = slotIndex;
		}

		public void ActivateFists()
		{
			if (this.WeaponSet.Locked)
			{
				if (this.WeaponImage.sprite != this.currentWeapon.image)
				{
					this.InitWeapon();
				}
				return;
			}
			this.DeInitWeapon(this.currentWeapon);
			this.currentWeapon = this.WeaponSet.GetFirstSlotOfType(WeaponSlotTypes.None).WeaponInstance;
			this.InitWeapon();
		}

		private void UpdateWeaponSlotPlaceholder(int slotIndex)
		{
			if (this.WeaponSet.Slots[slotIndex].WeaponInstance)
			{
				PoolManager.Instance.ReturnToPool(this.WeaponSet.Slots[slotIndex].WeaponInstance);
			}
			Weapon fromPool = PoolManager.Instance.GetFromPool<Weapon>(this.WeaponSet.Slots[slotIndex].WeaponPrefab);
			fromPool.transform.parent = this.WeaponSet.Slots[slotIndex].Placeholder.transform;
			fromPool.transform.localRotation = Quaternion.identity;
			fromPool.transform.localPosition = Vector3.zero;
			fromPool.transform.localScale = Vector3.one;
			this.WeaponSet.Slots[slotIndex].WeaponInstance = fromPool;
			this.WeaponSet.Slots[slotIndex].WeaponInstance.gameObject.SetActive(false);
		}

		public void LockWeponSet()
		{
			this.WeaponSet.Locked = true;
		}

		public void UnlockWeponSet()
		{
			this.WeaponSet.Locked = false;
		}

		public bool DebugLog;

		[Space(10f)]
		public AudioSource AudioSource;

		public HitEntity Owner;

		public Text AmmoText;

		public Image WeaponImage;

		public List<Weapon> Weapons = new List<Weapon>();

		[Space(10f)]
		public Weapon StartWeapon;

		public WeaponSet WeaponSet;

		[SerializeField]
		private Weapon currentWeapon;

		private int currentSlotIndex;

		[SerializeField]
		private int currentWeaponIndex;

		private AttackTargetParameters attackTargetParameters = new AttackTargetParameters();

		private PlayerStoreProfile playerProfile;

		private Player player;

		private float meleeWeaponDamageMultipler;
	}
}

using System;
using Game.Character.CharacterController;
using Game.UI;
using UnityEngine;

namespace Game.Weapons
{
	public class WeaponManager : MonoBehaviour
	{
		public static WeaponManager Instance
		{
			get
			{
				return WeaponManager.instance;
			}
		}

		private void Awake()
		{
			WeaponManager.instance = this;
			this.FlashLight.intensity = 0f;
		}

		private void Update()
		{
			if (this.DoFlash && this.isFlashing)
			{
				this.flashTimeout -= Time.deltaTime;
				if (this.flashTimeout < 0f)
				{
					this.StopShootSFX();
				}
			}
		}

		public GunSFX GetShotSFX(ShotSFXType type)
		{
			switch (type)
			{
			case ShotSFXType.Gun:
				return this.GunShotSfx;
			case ShotSFXType.Laser:
				return this.LaserShotSfx;
			case ShotSFXType.AlternativeLaser:
				return this.AlternativeLaserShotSfx;
			default:
				return this.GunShotSfx;
			}
		}

		public void StartShootSFX(Transform parent, ShotSFXType type)
		{
			this.FlashLight.transform.parent = parent;
			this.FlashLight.transform.localPosition = Vector3.zero;
			this.FlashLight.intensity = 1f;
			this.flashTimeout = this.FlashDuration;
			this.isFlashing = true;
			GunSFX shotSFX = this.GetShotSFX(type);
			shotSFX.Emit(parent.position, parent.forward);
		}

		public void StartTraceSfx(Transform parent, GameObject traceSfx, Vector3 shootDirection, float traceLength)
		{
			TraceSFX.Instance.Emit(parent.position, shootDirection);
		}

		public void StopShootSFX()
		{
			if (this.isFlashing)
			{
				this.FlashLight.intensity = 0f;
				this.isFlashing = false;
			}
		}

		public void ResetShootSFX()
		{
			this.StopShootSFX();
			this.FlashLight.transform.parent = base.transform;
			this.GunShotSfx.transform.parent = base.transform;
			this.LaserShotSfx.transform.parent = base.transform;
			this.AlternativeLaserShotSfx.transform.parent = base.transform;
		}

		public void CloseWeaponPanel()
		{
			this.chooseWeaponPanel.SetActive(false);
			UIGame.Instance.Resume();
		}

		private const float TracerDestroyTime = 1f;

		public Light FlashLight;

		public GunSFX GunShotSfx;

		public GunSFX LaserShotSfx;

		public GunSFX AlternativeLaserShotSfx;

		public bool DoFlash = true;

		public float FlashDuration = 0.1f;

		[Space(5f)]
		public GameObject chooseWeaponPanel;

		private static WeaponManager instance;

		private float flashTimeout;

		private bool isFlashing;
	}
}

using System;
using System.Collections;
using Game.Character;
using Game.Character.CharacterController;
using Game.Character.Modes;
using Game.Weapons;
using UnityEngine;

namespace Game.Shop
{
	public class AmmoShop : MonoBehaviour
	{
		public void DeInitShop()
		{
			base.StartCoroutine(this.MoveWeapon(this.lastMovableObject, false));
			this.DeInitCamera();
		}

		private IEnumerator MoveWeapon(GameObject weaponGameObject, bool forward)
		{
			float xCord = (float)((!forward) ? 0 : this.PushStrength);
			Vector3 movePoint = new Vector3(xCord, weaponGameObject.transform.localPosition.y, weaponGameObject.transform.localPosition.z);
			for (;;)
			{
				weaponGameObject.transform.localPosition = Vector3.MoveTowards(weaponGameObject.transform.localPosition, movePoint, this.PushSpeed);
				if (Vector3.Distance(weaponGameObject.transform.localPosition, movePoint) < 0.1f)
				{
					break;
				}
				yield return new WaitForEndOfFrame();
			}
			yield break;
		}

		private void MoveWeaponImmediatly(GameObject weaponGameObject, bool forward)
		{
			base.StopAllCoroutines();
			float x = (float)((!forward) ? 0 : this.PushStrength);
			Vector3 localPosition = new Vector3(x, weaponGameObject.transform.localPosition.y, weaponGameObject.transform.localPosition.z);
			weaponGameObject.transform.localPosition = localPosition;
		}

		private void InitCamera()
		{
			this.oldCameraMode = CameraManager.Instance.GetCurrentCameraMode();
			this.oldCameraTarget = CameraManager.Instance.CameraTarget;
			CameraManager.Instance.SetMode(this.cameraMode, false);
			CameraManager.Instance.SetCameraTarget(this.Weapons[this.currentWeapIndex].WeaponObject.transform);
		}

		private void DeInitCamera()
		{
			CameraManager.Instance.SetCameraTarget(this.oldCameraTarget);
			CameraManager.Instance.SetMode(this.oldCameraMode, false);
		}

		public AmmoShop.WeaponInSale[] Weapons;

		public int PushStrength = -1;

		public float PushSpeed = 0.1f;

		private int currentWeapIndex;

		private CameraMode cameraMode;

		private GameObject lastMovableObject;

		private CameraMode oldCameraMode;

		private Transform oldCameraTarget;

		private PlayerStoreProfile playerProfile;

		private Player playerController;

		private Color textColor;

		[Serializable]
		public class WeaponInSale
		{
			public WeaponNameList WeaponName;

			public WeaponArchetype WeaponArchetype;

			public GameObject WeaponObject;

			[Tooltip("Only for ranged weapons")]
			public GameObject BulletsObject;

			[Range(0f, 10f)]
			public float Damage;

			[Range(0f, 10f)]
			public float AttackSpeed;

			public int Price;

			public int BulletPrice;
		}
	}
}

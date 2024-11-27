using System;
using System.Collections.Generic;
using Game.GlobalComponent;
using Game.Items;
using Game.Weapons;
using UnityEngine;

namespace Game.PickUps
{
	public class PickUpManager : MonoBehaviour
	{
		public static PickUpManager Instance
		{
			get
			{
				PickUpManager result;
				if ((result = PickUpManager.instance) == null)
				{
					result = (PickUpManager.instance = new PickUpManager());
				}
				return result;
			}
		}

		private void Awake()
		{
			PickUpManager.instance = this;
			foreach (PickUp pickUp in this.Pickups)
			{
				BulletsPickup bulletsPickup = pickUp as BulletsPickup;
				if (bulletsPickup != null)
				{
					this.bulletsPickupPrefabs.Add(bulletsPickup, pickUp);
				}
				else
				{
					HealthPickup healthPickup = pickUp as HealthPickup;
					if (healthPickup != null)
					{
						this.healthPackPrefabs.Add(healthPickup, pickUp);
					}
					else
					{
						MoneyPickup moneyPickup = pickUp as MoneyPickup;
						if (moneyPickup != null)
						{
							this.moneyPickupPrefabs.Add(moneyPickup, pickUp);
						}
						EnergyPickup energyPickup = pickUp as EnergyPickup;
						if (energyPickup)
						{
							this.energyPackPrefabs.Add(energyPickup, pickUp);
						}
						WeaponPickup weaponPickup = pickUp as WeaponPickup;
						if (weaponPickup)
						{
							this.weaponPackPrefabs.Add(weaponPickup, pickUp);
						}
						CollectionPickup collectionPickup = pickUp as CollectionPickup;
						if (collectionPickup)
						{
							this.CollectionPackPrefabs.Add(collectionPickup, pickUp);
						}
						BodyArmorPickUp bodyArmorPickUp = pickUp as BodyArmorPickUp;
						if (bodyArmorPickUp)
						{
							this.BodyArmorPackPrefabs.Add(bodyArmorPickUp, pickUp);
						}
					}
				}
			}
		}

		public void RegisterPickup(PickUp pickUp)
		{
			this.ControllPickupsCount();
			this.dropedPickup.Add(pickUp);
			this.takedPickup.Remove(pickUp);
		}

		public void OnTakedPickup(PickUp pickUp)
		{
			this.PickupSound(pickUp);
			this.takedPickup.Add(pickUp);
			this.dropedPickup.Remove(pickUp);
		}

		public bool PickupWasTaked(PickUp pickUp)
		{
			return this.takedPickup.Contains(pickUp);
		}

		public void GenerateMoneyOnPoint(Vector3 position)
		{
			GameObject gameObject = null;
			using (IEnumerator<KeyValuePair<MoneyPickup, PickUp>> enumerator = this.moneyPickupPrefabs.GetEnumerator())
			{
				if (enumerator.MoveNext())
				{
					KeyValuePair<MoneyPickup, PickUp> keyValuePair = enumerator.Current;
					gameObject = keyValuePair.Value.gameObject;
				}
			}
			if (gameObject != null)
			{
				this.PlacePickupPrefab(gameObject, position);
				return;
			}
			throw new Exception("Pickup Money not assigned!");
		}

		public void GenerateBulletsOnPoint(Vector3 position, AmmoTypes bulletsType)
		{
			GameObject gameObject = null;
			foreach (KeyValuePair<BulletsPickup, PickUp> keyValuePair in this.bulletsPickupPrefabs)
			{
				if (keyValuePair.Key.BulletType == bulletsType)
				{
					gameObject = keyValuePair.Value.gameObject;
					break;
				}
			}
			if (gameObject != null)
			{
				this.PlacePickupPrefab(gameObject, position);
				return;
			}
			throw new Exception("Pickup bullets for weapon " + bulletsType + " not assigned!");
		}

		public void GenerateEnergyOnPoint(Vector3 position)
		{
			GameObject gameObject = null;
			foreach (KeyValuePair<EnergyPickup, PickUp> keyValuePair in this.energyPackPrefabs)
			{
				gameObject = keyValuePair.Value.gameObject;
			}
			if (gameObject != null)
			{
				this.PlacePickupPrefab(gameObject, position);
				return;
			}
			throw new Exception("Energy prefab not assigned!");
		}

		public void GenerateHealthPackOnPoint(Vector3 position, PickUpManager.HealthPackType healthPackType)
		{
			PickUpManager.HealthPackType healthPackType2 = healthPackType;
			if (healthPackType2 == PickUpManager.HealthPackType.Random)
			{
				healthPackType2 = (PickUpManager.HealthPackType)UnityEngine.Random.Range(0, 3);
			}
			GameObject gameObject = null;
			foreach (KeyValuePair<HealthPickup, PickUp> keyValuePair in this.healthPackPrefabs)
			{
				if (keyValuePair.Key.HealthPackType == healthPackType2)
				{
					gameObject = keyValuePair.Value.gameObject;
					break;
				}
			}
			if (gameObject != null)
			{
				this.PlacePickupPrefab(gameObject, position);
				return;
			}
			throw new Exception("Pickup for Helathpack type of " + healthPackType + " not assigned!");
		}

		public void GenerateBodyArmorOnPoint(Vector3 position)
		{
			GameObject gameObject = null;
			using (IEnumerator<KeyValuePair<BodyArmorPickUp, PickUp>> enumerator = this.BodyArmorPackPrefabs.GetEnumerator())
			{
				if (enumerator.MoveNext())
				{
					KeyValuePair<BodyArmorPickUp, PickUp> keyValuePair = enumerator.Current;
					gameObject = keyValuePair.Value.gameObject;
				}
			}
			if (gameObject != null)
			{
				this.PlacePickupPrefab(gameObject, position);
				return;
			}
			throw new Exception("Pickup BodyArmor not assigned!");
		}

		private void PlacePickupPrefab(GameObject pickupPrefab, Vector3 position)
		{
			GameObject fromPool = PoolManager.Instance.GetFromPool(pickupPrefab);
			fromPool.transform.position = position;
			fromPool.transform.eulerAngles = Vector3.zero;
			this.RegisterPickup(fromPool.GetComponent<PickUp>());
		}

		private void ControllPickupsCount()
		{
			if (this.dropedPickup.Count >= 20)
			{
				PickUp pickUp = this.dropedPickup[0];
				PoolManager.Instance.ReturnToPool(pickUp);
				this.dropedPickup.Remove(pickUp);
				this.takedPickup.Add(pickUp);
			}
		}

		private void PickupSound(PickUp pickup)
		{
			PointSoundManager pointSoundManager = PointSoundManager.Instance;
			Vector3 position = pickup.transform.position;
			if (pickup is MoneyPickup)
			{
				pointSoundManager.PlayCustomClipAtPoint(position, this.MoneyPickupSound, null);
			}
			else if (pickup is BulletsPickup)
			{
				pointSoundManager.PlayCustomClipAtPoint(position, this.BulletsPickupSound, null);
			}
			else if (pickup is HealthPickup)
			{
				pointSoundManager.PlayCustomClipAtPoint(position, this.HealthPackPickupSound, null);
			}
			else if (pickup is QuestPickUp)
			{
				pointSoundManager.PlayCustomClipAtPoint(position, this.QuestItemPickupSound, null);
			}
		}

		private const int MaxPlacedPickups = 20;

		public static int MinMoney = 80;

		public static int MaxMoney = 120;

		private static PickUpManager instance;

		public PickUp[] Pickups;

		public AudioClip MoneyPickupSound;

		public AudioClip BulletsPickupSound;

		public AudioClip HealthPackPickupSound;

		public AudioClip QuestItemPickupSound;

		public AudioClip WeaponItemPickupSound;

		public GameItem HealthKitGameItem;

		public GameItem BodyArmorGameItem;

		private readonly IDictionary<MoneyPickup, PickUp> moneyPickupPrefabs = new Dictionary<MoneyPickup, PickUp>();

		private readonly IDictionary<BulletsPickup, PickUp> bulletsPickupPrefabs = new Dictionary<BulletsPickup, PickUp>();

		private readonly IDictionary<HealthPickup, PickUp> healthPackPrefabs = new Dictionary<HealthPickup, PickUp>();

		private readonly IDictionary<EnergyPickup, PickUp> energyPackPrefabs = new Dictionary<EnergyPickup, PickUp>();

		private readonly IDictionary<WeaponPickup, PickUp> weaponPackPrefabs = new Dictionary<WeaponPickup, PickUp>();

		private readonly IDictionary<CollectionPickup, PickUp> CollectionPackPrefabs = new Dictionary<CollectionPickup, PickUp>();

		private readonly IDictionary<BodyArmorPickUp, PickUp> BodyArmorPackPrefabs = new Dictionary<BodyArmorPickUp, PickUp>();

		private readonly List<PickUp> dropedPickup = new List<PickUp>();

		private readonly HashSet<PickUp> takedPickup = new HashSet<PickUp>();

		public enum PickupType
		{
			Money,
			Bullets,
			HealthPack,
			QuestItem,
			EnegryPack,
			Weapon,
			Collection
		}

		public enum HealthPackType
		{
			Random = -1,
			Small,
			Medium,
			Large
		}
	}
}

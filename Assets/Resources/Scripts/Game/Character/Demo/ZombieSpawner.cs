using System;
using UnityEngine;

namespace Game.Character.Demo
{
	public class ZombieSpawner : MonoBehaviour
	{
		public static ZombieSpawner Instance { get; private set; }

		public void SpawnZombieAt(Vector3 position)
		{
			UnityEngine.Object.Instantiate<GameObject>(this.ZombiePrefab, position, Quaternion.identity);
		}

		public void SpawnWormAt(Vector3 position)
		{
			UnityEngine.Object.Instantiate<GameObject>(this.WormPrefab, position, Quaternion.identity);
		}

		public void SpawnZombie()
		{
			this.SpawnZombieAt(base.gameObject.transform.position);
		}

		public void SpawnWorm()
		{
			this.SpawnWormAt(base.gameObject.transform.position);
		}

		public void SpawnHell()
		{
			for (int i = 0; i < 10; i++)
			{
				this.SpawnZombie();
				this.SpawnWorm();
			}
		}

		public void SpawnHellOnce()
		{
			if (!this.spawnHell)
			{
				this.SpawnHell();
				this.spawnHell = true;
			}
		}

		private void Awake()
		{
			ZombieSpawner.Instance = this;
		}

		public GameObject ZombiePrefab;

		public GameObject WormPrefab;

		private bool spawnHell;
	}
}

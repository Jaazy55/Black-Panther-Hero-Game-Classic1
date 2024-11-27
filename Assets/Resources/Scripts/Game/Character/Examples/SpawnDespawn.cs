using System;
using UnityEngine;

namespace Game.Character.Examples
{
	public class SpawnDespawn : MonoBehaviour
	{
		private void Start()
		{
			this.cameraManager = CameraManager.Instance;
		}

		public void Spawn()
		{
			this.CharacterControllerCurrent = UnityEngine.Object.Instantiate<GameObject>(this.CharacterControllerPrefab, this.position, Quaternion.identity);
			if (this.CharacterControllerCurrent != null)
			{
				this.cameraManager.SetCameraTarget(this.CharacterControllerCurrent.transform);
				this.cameraManager.SetMode(this.cameraManager.ActivateModeOnStart, false);
			}
		}

		public void Despawn()
		{
			this.position = this.CharacterControllerCurrent.transform.position;
			UnityEngine.Object.Destroy(this.CharacterControllerCurrent.gameObject);
			this.cameraManager.SetMode(null, false);
		}

		public GameObject CharacterControllerPrefab;

		public GameObject CharacterControllerCurrent;

		private CameraManager cameraManager;

		public Vector3 position;
	}
}

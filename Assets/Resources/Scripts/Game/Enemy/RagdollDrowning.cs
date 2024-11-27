using System;
using Game.Character.CharacterController;
using Game.GlobalComponent;
using UnityEngine;

namespace Game.Enemy
{
	public class RagdollDrowning : MonoBehaviour
	{
		public void Init(Transform hips, float hight)
		{
			this.isInit = true;
			this.waterHight = hight;
			this.hips = hips.GetComponent<Rigidbody>();
			this.hips.velocity = Vector3.zero;
		}

		private void OnDisable()
		{
			this.isInit = false;
		}

		private void FixedUpdate()
		{
			if (!this.isInit)
			{
				return;
			}
			if (!PlayerManager.Instance.OnPlayerSignline(base.transform))
			{
				PoolManager.Instance.ReturnToPool(this);
				return;
			}
			float d = this.waterHight - this.hips.transform.position.y;
			this.hips.AddForce(Vector3.up * 1100f * d);
			this.hips.velocity = new Vector3(Mathf.Clamp(this.hips.velocity.x, -0.5f, 0.5f), this.hips.velocity.y, Mathf.Clamp(this.hips.velocity.z, -0.5f, 0.5f));
			this.hips.angularVelocity = new Vector3(this.hips.angularVelocity.x, 0f, this.hips.angularVelocity.z);
			this.hips.transform.position = Vector3.Lerp(this.hips.transform.position, new Vector3(this.hips.transform.position.x, this.waterHight, this.hips.transform.position.z), Time.deltaTime);
		}

		private const float FloatingForсe = 1100f;

		private const float ClampVelocity = 0.5f;

		private float waterHight;

		private Rigidbody hips;

		private bool isInit;
	}
}

using System;
using UnityEngine;

namespace Game.Particles
{
	public class DelayedDisableObject : MonoBehaviour
	{
		private void OnEnable()
		{
			this.delayTimer = this.Delay;
		}

		private void Update()
		{
			if (this.delayTimer > 0f)
			{
				this.delayTimer -= Time.deltaTime;
			}
			else
			{
				base.gameObject.SetActive(false);
			}
		}

		public float Delay;

		private float delayTimer;
	}
}

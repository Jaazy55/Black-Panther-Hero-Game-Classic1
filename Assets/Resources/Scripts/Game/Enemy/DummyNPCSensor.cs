using System;
using UnityEngine;

namespace Game.Enemy
{
	public class DummyNPCSensor : MonoBehaviour
	{
		public bool CanMove
		{
			get
			{
				return this.counter <= 0f;
			}
		}

		private void Update()
		{
			if (this.counter > 0f)
			{
				this.counter -= Time.deltaTime;
			}
		}

		private void OnTriggerStay(Collider col)
		{
			this.counter = 1f;
		}

		private float counter;
	}
}

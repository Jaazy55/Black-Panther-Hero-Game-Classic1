using System;
using UnityEngine;

namespace Game.Character.Extras
{
	[RequireComponent(typeof(Projector))]
	public class RTSProjector : MonoBehaviour
	{
		private void Awake()
		{
			RTSProjector.Instance = this;
			this.projector = base.GetComponent<Projector>();
		}

		public void Enable()
		{
			this.projector.enabled = true;
		}

		public void Disable()
		{
			this.projector.enabled = false;
		}

		public void Project(Vector3 pos, Color color)
		{
			this.projector.material.color = color;
			this.Enable();
			this.projector.fieldOfView = this.FovMax;
			this.timeout = this.AnimTimeout;
			base.transform.position = pos + Vector3.up * this.Distance;
		}

		private void Update()
		{
			this.timeout -= Time.deltaTime;
			if (this.timeout > 0f)
			{
				float num = this.timeout / this.AnimTimeout;
				float num2;
				if ((double)num > 0.5)
				{
					num2 = (num - 0.5f) / 0.5f;
				}
				else
				{
					num2 = num / 0.5f;
				}
				this.projector.fieldOfView = this.FovMin + (this.FovMax - this.FovMin) * num2;
			}
			else
			{
				this.projector.fieldOfView = this.FovMax;
			}
		}

		public static RTSProjector Instance;

		public float AnimTimeout;

		public float Distance;

		public float FovMax;

		public float FovMin;

		private Projector projector;

		private float timeout;
	}
}

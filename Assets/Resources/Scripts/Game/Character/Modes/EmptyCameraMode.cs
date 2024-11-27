using System;
using Game.Character.Config;
using UnityEngine;

namespace Game.Character.Modes
{
	[RequireComponent(typeof(EmptyConfig))]
	public class EmptyCameraMode : CameraMode
	{
		public override Type Type
		{
			get
			{
				return Type.None;
			}
		}

		public override void Init()
		{
			base.Init();
			this.UnityCamera.transform.LookAt(this.cameraTarget);
			this.config = base.GetComponent<EmptyConfig>();
		}
	}
}

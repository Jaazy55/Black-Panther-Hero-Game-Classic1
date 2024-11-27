using System;
using UnityEngine;

namespace Game.Character.Modes
{
	public class ShopCameraMode : CameraMode
	{
		public override Type Type
		{
			get
			{
				return Type.Shop;
			}
		}

		public override void Init()
		{
			base.Init();
			this.PositionFinderInit();
		}

		private void PositionFinderInit()
		{
			if (!this.positionFinder)
			{
				this.positionFinder = new GameObject("Position Finder");
			}
			this.positionFinder.transform.parent = this.Target.parent;
			this.positionFinder.transform.localRotation = this.Target.localRotation;
			this.positionFinder.transform.localPosition = new Vector3(-2f, this.Target.transform.localPosition.y, this.Target.transform.localPosition.z);
			this.positionFinder.transform.LookAt(this.Target);
		}

		public override void SetCameraTarget(Transform target)
		{
			base.SetCameraTarget(target);
			this.PositionFinderInit();
		}

		public override void GameUpdate()
		{
			if (!this.positionFinder)
			{
				return;
			}
			this.UnityCamera.transform.position = Vector3.Lerp(this.UnityCamera.transform.position, this.positionFinder.transform.position, this.CameraSpeed);
			this.UnityCamera.transform.rotation = Quaternion.Slerp(this.UnityCamera.transform.rotation, this.positionFinder.transform.rotation, this.CameraSpeed);
		}

		public override void SetCameraConfigMode(string modeName)
		{
		}

		public float Indent = -2f;

		public float CameraSpeed = 0.01f;

		private GameObject positionFinder;
	}
}

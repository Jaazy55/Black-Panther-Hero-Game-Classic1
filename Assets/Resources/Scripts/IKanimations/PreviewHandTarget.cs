using System;
using UnityEngine;

namespace IKanimations
{
	public class PreviewHandTarget : MonoBehaviour
	{
		private void OnEnable()
		{
			this.IKHandler = UnityEngine.Object.FindObjectOfType<PreviewIKHandler>();
			this.IKHandler.DoIKAnimation = true;
			this.LeftHandTargetOnPlayer = this.IKHandler.leftIKHandTarget;
			this.LeftHandHintOnPlayer = this.IKHandler.leftIKHandHint;
		}

		private void OnDisable()
		{
			this.IKHandler.DoIKAnimation = false;
		}

		private void Update()
		{
			if (this.LeftHandTargetOnPlayer)
			{
				this.LeftHandTargetOnPlayer.position = base.transform.position;
			}
			if (this.LeftHandHintOnWeapon && this.LeftHandHintOnPlayer)
			{
				this.LeftHandHintOnPlayer.position = this.LeftHandHintOnWeapon.transform.position;
			}
		}

		private PreviewIKHandler IKHandler;

		private Transform LeftHandTargetOnPlayer;

		public Transform LeftHandHintOnWeapon;

		private Transform LeftHandHintOnPlayer;
	}
}

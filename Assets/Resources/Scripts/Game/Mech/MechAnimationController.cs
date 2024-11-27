using System;
using System.Collections.Generic;
using Game.Character;
using Game.Character.CharacterController;
using Game.GlobalComponent;
using UnityEngine;

namespace Game.Mech
{
	public class MechAnimationController : MonoBehaviour
	{
		private void Awake()
		{
			this.animator = base.GetComponent<Animator>();
			if (!this.animator)
			{
				this.animator = base.GetComponentInChildren<Animator>();
			}
			this.drivableMech = base.GetComponent<DrivableMech>();
			this.rayHitComparer = new RayHitComparer();
			this.GenerateAnimatorHashes();
			this.slowUpdateProc = new SlowUpdateProc(new SlowUpdateProc.SlowUpdateDelegate(this.SlowUpdate), 0.1f);
		}

		private void GenerateAnimatorHashes()
		{
			this.ForwardHash = Animator.StringToHash("Forward");
			this.TurnHash = Animator.StringToHash("Turn");
			this.UpHash = Animator.StringToHash("Up");
			this.DownHash = Animator.StringToHash("Down");
			this.Right90Hash = Animator.StringToHash("Right90");
			this.Left90Hash = Animator.StringToHash("Left90");
			this.Turn180Hash = Animator.StringToHash("Turn180");
			this.makeShootHash = Animator.StringToHash("MakeShoot");
		}

		private void FixedUpdate()
		{
			this.slowUpdateProc.ProceedOnFixedUpdate();
		}

		private void SlowUpdate()
		{
			this.CheckIsOnGround();
		}

		public void Move(MechInputs inputs)
		{
			float y = inputs.move.y;
			float x = inputs.move.x;
			this.animator.SetFloat(this.ForwardHash, y);
			this.animator.SetFloat(this.TurnHash, x);
			if (this.isSpider)
			{
				return;
			}
			this.makeShoot = false;
			if (inputs.right90 && this.buttonsAvailableForUse)
			{
				this.animator.SetTrigger(this.Right90Hash);
				this.buttonsAvailableForUse = false;
			}
			else if (inputs.left90 && this.buttonsAvailableForUse)
			{
				this.animator.SetTrigger(this.Left90Hash);
				this.buttonsAvailableForUse = false;
			}
			else if (y <= -0.9f && this.buttonsAvailableForUse)
			{
				this.animator.SetTrigger(this.Turn180Hash);
				this.buttonsAvailableForUse = false;
			}
			else if (!inputs.right90 && !inputs.left90 && y >= -0.9f && !this.buttonsAvailableForUse)
			{
				this.buttonsAvailableForUse = true;
			}
			if (Mathf.Abs(x) < 0.1f && y < 0.1f && inputs.fire && this.drivableMech.GunController.DirectionPermittedForShooting(CameraManager.Instance.UnityCamera.transform.forward))
			{
				this.makeShoot = true;
			}
			this.animator.SetBool(this.makeShootHash, this.makeShoot);
		}

		public void StandUp()
		{
			this.ResetMechPosition();
			this.animator.enabled = true;
			this.animator.SetTrigger(this.UpHash);
		}

		public void Down()
		{
			this.animator.SetTrigger(this.DownHash);
			base.Invoke("DisableAnimator", 2.8f);
		}

		private void CheckIsOnGround()
		{
			Ray ray = new Ray(base.transform.position + Vector3.up * 0.1f, -Vector3.up);
			RaycastHit[] array = Physics.RaycastAll(ray, 0.5f, this.groundLayerMask);
			Array.Sort<RaycastHit>(array, this.rayHitComparer);
			foreach (RaycastHit raycastHit in array)
			{
				if (!raycastHit.collider.isTrigger)
				{
					this.OnGround = true;
				}
			}
		}

		private void ResetMechPosition()
		{
			base.transform.rotation = Quaternion.identity;
		}

		private void DisableAnimator()
		{
			this.animator.enabled = false;
		}

		private const float turnBackInputValue = -0.9f;

		public bool isSpider;

		public LayerMask groundLayerMask = -1;

		public bool OnGround = true;

		private Animator animator;

		private IComparer<RaycastHit> rayHitComparer;

		private SlowUpdateProc slowUpdateProc;

		private Transform target;

		private DrivableMech drivableMech;

		private int TurnHash;

		private int ForwardHash;

		private int UpHash;

		private int DownHash;

		private int Right90Hash;

		private int Left90Hash;

		private int Turn180Hash;

		private int makeShootHash;

		private float forwardValue = 0.5f;

		private float turnLeftValue = -0.5f;

		private float turnRightValue = 0.5f;

		private bool makeShoot;

		private bool buttonsAvailableForUse = true;
	}
}

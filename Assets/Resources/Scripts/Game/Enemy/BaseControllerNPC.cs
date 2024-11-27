using System;
using UnityEngine;

namespace Game.Enemy
{
	public class BaseControllerNPC : MonoBehaviour
	{
		public bool IsInited { get; protected set; }

		public virtual void Init(BaseNPC controlledNPC)
		{
			this.CurrentControlledNpc = controlledNPC;
			this.IsInited = true;
			if (this.AnimatorWithController)
			{
				controlledNPC.NPCAnimator.runtimeAnimatorController = this.AnimatorWithController.runtimeAnimatorController;
			}
		}

		public virtual void DeInit()
		{
			this.CurrentControlledNpc = null;
			this.IsInited = false;
		}

		protected virtual void Update()
		{
			if (!this.IsInited)
			{
				return;
			}
		}

		public Animator AnimatorWithController;

		protected BaseNPC CurrentControlledNpc;
	}
}

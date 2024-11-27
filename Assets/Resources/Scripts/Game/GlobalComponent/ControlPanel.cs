using System;
using UnityEngine;

namespace Game.GlobalComponent
{
	[RequireComponent(typeof(Animator))]
	public class ControlPanel : MonoBehaviour
	{
		public ControlsType GetPanelType()
		{
			return this.ControlsType;
		}

		public virtual void OnOpen()
		{
		}

		public virtual void OnClose()
		{
		}

		public Animator GetPanelAnimator()
		{
			Animator result;
			if ((result = this.panelAnimator) == null)
			{
				result = (this.panelAnimator = base.GetComponent<Animator>());
			}
			return result;
		}

		public ControlsType ControlsType;

		private Animator panelAnimator;
	}
}

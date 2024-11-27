using System;
using System.Collections;
using Game.DialogSystem;
using UnityEngine;

namespace Naxeex.GameModes.Inheritors
{
	[CreateAssetMenu(fileName = "Tutorial Interpeter", menuName = "Game Modes/Interpeters/Tutorial Interpeter")]
	public class TutorialInterpreter : RuleInterpreter
	{
		public override void RuleBegin()
		{
			base.RuleBegin();
			if (!this.HasCoroutine)
			{
				DialogManager.Instance.StartCoroutine(this.ShowCoroutine());
			}
			ArenaTutorial.State = ArenaTutorial.TutorialState.WinArena;
			this.HasCoroutine = true;
		}

		public override void RuleRestart()
		{
			base.RuleRestart();
			if (!this.HasCoroutine)
			{
				DialogManager.Instance.StartCoroutine(this.ShowCoroutine());
			}
			ArenaTutorial.State = ArenaTutorial.TutorialState.WinArena;
			this.HasCoroutine = true;
		}

		public override void RuleEnd()
		{
			base.RuleEnd();
			this.HasCoroutine = false;
		}

		private IEnumerator ShowCoroutine()
		{
			if (this.m_DeferredCall > 0f)
			{
				yield return new WaitForSeconds(this.m_DeferredCall);
			}
			if (this.HasCoroutine)
			{
				DialogManager.Instance.StartDialog(this.m_TutorialDialog);
			}
			this.HasCoroutine = false;
			yield break;
		}

		private float m_DeferredCall = 2f;

		[SerializeField]
		private Dialog m_TutorialDialog;

		private bool HasCoroutine;
	}
}

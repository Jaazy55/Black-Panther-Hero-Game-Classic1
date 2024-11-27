using System;
using Game.Character;
using Game.GlobalComponent.Quality;
using UnityEngine;

namespace Naxeex.GameModes.Inheritors
{
	[CreateAssetMenu(fileName = "Teleport Load Scene Interpeter", menuName = "Game Modes/Interpeters/Teleport Load Scene Interpeter")]
	public class TeleportLoadSceneInterpreter : LoadSceneInterpreter
	{
		public override void RuleBegin()
		{
			base.RuleBegin();
			if (PlayerInteractionsManager.Instance.Player.IsDead)
			{
				PlayerInteractionsManager.Instance.Player.Resurrect();
			}
		}

		public override void RuleRestart()
		{
			base.RuleRestart();
			if (PlayerInteractionsManager.Instance.Player.IsDead)
			{
				PlayerInteractionsManager.Instance.Player.Resurrect();
			}
		}

		protected override void OnLoaded()
		{
			base.OnLoaded();
			WaitingPanelController.Instance.StartWaiting(new Action(base.ContinueGame), 50);
			PlayerInteractionsManager.Instance.TeleportPlayerToPosition(this.playerPoint.position, this.playerPoint.rotation);
			if (PlayerInteractionsManager.Instance.Player.IsDead)
			{
				PlayerInteractionsManager.Instance.Player.Resurrect();
			}
		}

		private bool PlayerInPosition()
		{
			return Vector3.Distance(PlayerInteractionsManager.Instance.Player.transform.position, this.playerPoint.position) < 1f;
		}

		[SerializeField]
		protected SpatialPoint playerPoint;
	}
}

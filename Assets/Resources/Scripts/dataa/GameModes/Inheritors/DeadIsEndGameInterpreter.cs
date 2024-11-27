using System;
using System.Collections;
using Game.Character.CharacterController;
using UnityEngine;

namespace Naxeex.GameModes.Inheritors
{
	[CreateAssetMenu(fileName = "Dead Is End Game Interpeter", menuName = "Game Modes/Interpeters/Dead Is End Game Interpeter")]
	public class DeadIsEndGameInterpreter : RuleInterpreter
	{
		public override void RuleBegin()
		{
			PlayerDieManager instance = PlayerDieManager.Instance;
			instance.PlayerDiedEvent = (PlayerDieManager.PlayerDied)Delegate.Combine(instance.PlayerDiedEvent, new PlayerDieManager.PlayerDied(this.PlayerDied));
			PlayerDieManager.Instance.PlayerDieCoroutine = new Func<IEnumerator>(this.PlayerDieCoroutine);
		}

		public override void RuleEnd()
		{
			PlayerDieManager instance = PlayerDieManager.Instance;
			instance.PlayerDiedEvent = (PlayerDieManager.PlayerDied)Delegate.Remove(instance.PlayerDiedEvent, new PlayerDieManager.PlayerDied(this.PlayerDied));
			PlayerDieManager.Instance.PlayerDieCoroutine = null;
		}

		private void PlayerDied(float timeOfDeath)
		{
			Manager.Final();
		}

		private IEnumerator PlayerDieCoroutine()
		{
			yield return null;
			PlayerDieManager.Instance.ResetDeadEventTriggered();
			yield break;
		}
	}
}

using System;
using UnityEngine;

namespace Naxeex.AttaskSystem
{
	public class AttackSource : MonoBehaviour
	{
		[SerializeField]
		private AttackModifierProbability m_ModifierProbability;

		[SerializeField]
		private AttackModifierCollection m_Collection;
	}
}

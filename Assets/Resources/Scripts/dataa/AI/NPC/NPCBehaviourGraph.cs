using System;
using Naxeex.AI.StateSystem;
using UnityEngine;

namespace Naxeex.AI.NPC
{
	[CreateAssetMenu(fileName = "NPCBehaviour", menuName = "NPC/Create Behaviour Graph", order = 1)]
	public class NPCBehaviourGraph : BehaviourGraph<NPCBehaviour>
	{
	}
}

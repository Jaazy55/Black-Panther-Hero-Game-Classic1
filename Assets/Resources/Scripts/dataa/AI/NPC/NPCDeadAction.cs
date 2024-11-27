using System;
using Naxeex.AI.StateSystem;
using UnityEngine;

namespace Naxeex.AI.NPC
{
	[CreateAssetMenu(fileName = "NPCDeadAction", menuName = "NPC/Dead State Action", order = 10)]
	public class NPCDeadAction : StateAction<NPCBehaviour>
	{
		internal override void OnStateEnter(NPCBehaviour entity)
		{
			entity.Die();
		}

		internal override void OnStateExit(NPCBehaviour entity)
		{
		}

		internal override void OnStateUpdate(NPCBehaviour entity)
		{
		}
	}
}

using System;
using Naxeex.AI.StateSystem;

namespace Naxeex.AI.NPC
{
	public class NPCBehaviourController : BehaviourController<NPCBehaviour>
	{
		public NPCBehaviourController(NPCBehaviour entity, BehaviourGraph<NPCBehaviour> behaviour) : base(entity, behaviour)
		{
		}
	}
}

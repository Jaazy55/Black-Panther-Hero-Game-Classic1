using System;

namespace Game.Character.CharacterController
{
	[Serializable]
	public class NpcRelations
	{
		public Faction FirstFaction;

		public Faction SecondFaction;

		public Relations TheirRelations = Relations.Neutral;
	}
}

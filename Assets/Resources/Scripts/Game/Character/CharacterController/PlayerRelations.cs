using System;
using UnityEngine;

namespace Game.Character.CharacterController
{
	[Serializable]
	public class PlayerRelations
	{
		public Relations CurrentRelations
		{
			get
			{
				if (this.RelationValue <= -10f)
				{
					return Relations.Hostile;
				}
				if (this.RelationValue >= 10f)
				{
					return Relations.Friendly;
				}
				return Relations.Neutral;
			}
			set
			{
				if (value != Relations.Neutral)
				{
					if (value != Relations.Hostile)
					{
						if (value == Relations.Friendly)
						{
							this.RelationValue = 10f;
						}
					}
					else
					{
						this.RelationValue = -10f;
					}
				}
				else
				{
					this.RelationValue = 0f;
				}
			}
		}

		public void ChangeRelationValue(float value)
		{
			this.RelationValue += value;
		}

		public const float HostileRelationsTreshold = -10f;

		public const float FriendlyRelationsTreshold = 10f;

		public const float OneStarWeight = 2f;

		public Faction NpcFaction;

		[Tooltip("<-10 - Враги; >10 - Друзья; Иначе нейтралы")]
		public float RelationValue;
	}
}

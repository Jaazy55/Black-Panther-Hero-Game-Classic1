using System;

namespace Game.Character.CharacterController.Enums
{
	public enum MeleeAttackState
	{
		None = -2,
		Idle,
		PunchLeft1,
		PunchLeft2,
		PunchLeft3,
		PunchRight1,
		PunchRight2,
		PunchRight3,
		PunchRight4,
		StabRight1 = 20,
		StabRight2,
		StabRight3,
		DoubleArmHit1 = 30,
		DoubleArmHit2,
		DoubleArmHit3,
		BattleAxeHit1 = 40,
		BattleAxeHit2,
		LowKickLeft1 = 100,
		LowKickRight1,
		HighKickLeft1,
		HighKickRight1,
		HighKickRight2,
		HighKick360Right1,
		GroundSmash = 125
	}
}

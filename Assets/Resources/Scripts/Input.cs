using System;
using UnityEngine;

namespace Game.Character.CharacterController
{
	public struct Input
	{
		public Vector3 camMove;

		public Vector3 inputMove;

		public Vector3 lookPos;

		public bool crouch;

		public bool jump;

		public bool die;

		public bool reset;

		public bool smoothAimRotation;

		public bool aimTurn;

		public bool sprint;

		public bool shootRope;

		public bool fly;

		public AttackState AttackState;
	}
}

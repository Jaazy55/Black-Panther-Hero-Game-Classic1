using System;
using System.Collections.Generic;
using Game.Character.CharacterController;
using UnityEngine;

namespace Game.Items
{
	public class GameItemPlayerChanger : GameItemPowerUp
	{
		public override bool CanBeEquiped
		{
			get
			{
				Transform transform = PlayerManager.Instance.Player.transform;
				Vector3 vector = transform.position + Vector3.up * 0.05f;
				LayerMask obstaclesLayerMask = PlayerManager.Instance.DefaulAnimationController.ObstaclesLayerMask;
				return base.CanBeEquiped && (base.isActive || (!Physics.Raycast(vector - transform.right, transform.right, this.ModelSizeVector.x, obstaclesLayerMask) && !Physics.Raycast(vector, transform.up, this.ModelSizeVector.y, obstaclesLayerMask) && !Physics.Raycast(vector - transform.forward, transform.forward, this.ModelSizeVector.z, obstaclesLayerMask) && !PlayerManager.Instance.Player.IsDead));
			}
		}

		public override void Activate()
		{
			base.Activate();
			List<GameItemPlayerChanger> list = new List<GameItemPlayerChanger>();
			foreach (GameItemPowerUp gameItemPowerUp in StuffManager.ActivePowerUps)
			{
				GameItemPlayerChanger gameItemPlayerChanger = gameItemPowerUp as GameItemPlayerChanger;
				if (gameItemPlayerChanger != null && gameItemPlayerChanger != this)
				{
					list.Add(gameItemPlayerChanger);
				}
			}
			foreach (GameItemPowerUp gameItemPowerUp2 in list)
			{
				gameItemPowerUp2.Deactivate();
			}
			PlayerManager.Instance.SwitchPlayer(this.NewPlayerPrefab);
		}

		public override void Deactivate()
		{
			base.Deactivate();
			PlayerManager.Instance.ResetPlayer();
		}

		public override bool SameParametrWithOther(object[] parametrs)
		{
			return this.NewPlayerPrefab == (GameObject)parametrs[0];
		}

		public GameObject NewPlayerPrefab;

		[Tooltip("Ширина, высота, глубина модели")]
		public Vector3 ModelSizeVector = new Vector3(1f, 2f, 1f);
	}
}

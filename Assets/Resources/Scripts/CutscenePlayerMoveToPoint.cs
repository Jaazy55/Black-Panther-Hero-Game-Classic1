using System;
using System.Collections;
using Game.Character;
using Game.Character.CameraEffects;
using Game.Character.CharacterController;
using Game.Character.Input;
using Game.GlobalComponent;
using UnityEngine;
using UnityEngine.AI;

public class CutscenePlayerMoveToPoint : Cutscene
{
	public override void StartScene()
	{
		this.IsPlaying = true;
		this.velocityFilter = new InputFilter(10, 1f);
		this.playerTransform = PlayerInteractionsManager.Instance.Player.transform;
		this.player = PlayerInteractionsManager.Instance.Player;
		EffectManager.Instance.StopAll();
		base.StartCoroutine(this.MoveToPoint());
	}

	private IEnumerator MoveToPoint()
	{
		while (PlayerInteractionsManager.Instance.inVehicle || PlayerManager.Instance.IsGettingInOrOut)
		{
			yield return new WaitForSeconds(0.25f);
		}
		this.player.enabled = false;
		this.agent = this.player.agent;
		this.agent.enabled = true;
		while (Vector3.Distance(this.EndPosition.position, this.playerTransform.position) > this.DistanceToPoint && this.IsPlaying)
		{
			if (this.agent.enabled)
			{
				this.agent.SetDestination(this.EndPosition.position);
				if (MetroManager.Instance.CurrentMetro != null)
				{
					MetroManager.Instance.CurrentMetro.RemoveObstructive();
				}
			}
			yield return new WaitForSeconds(0.25f);
		}
		this.EndScene(true);
		yield break;
	}

	private Vector3 SmoothVelocityVector(Vector3 v)
	{
		this.velocityFilter.AddSample(new Vector2(v.x, v.z));
		Vector2 value = this.velocityFilter.GetValue();
		Vector3 vector = new Vector3(value.x, 0f, value.y);
		return vector.normalized;
	}

	private void Update()
	{
		if (this.IsPlaying)
		{
			if (this.player == null)
			{
				return;
			}
			this.player.GetComponent<AnimationController>().Move(new Game.Character.CharacterController.Input
			{
				camMove = this.SmoothVelocityVector((this.agent.steeringTarget - this.playerTransform.position).normalized) * this.MoveSpeed,
				AttackState = new AttackState(),
				lookPos = this.EndPosition.position + Vector3.up,
				crouch = false,
				inputMove = Vector3.zero,
				jump = false,
				smoothAimRotation = false,
				aimTurn = false
			});
		}
	}

	public override void EndScene(bool isCheck)
	{
		base.EndScene(isCheck);
		this.agent.enabled = false;
		this.player.enabled = true;
		base.StopAllCoroutines();
	}

	public float MoveSpeed = 1f;

	public Transform StartPosition;

	public Transform EndPosition;

	public float DistanceToPoint = 0.5f;

	private Transform playerTransform;

	private Player player;

	private NavMeshAgent agent;

	private InputFilter velocityFilter;
}

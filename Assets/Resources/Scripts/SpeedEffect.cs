using System;
using System.Collections.Generic;
using Game.Character.CharacterController;
using UnityEngine;

public class SpeedEffect : MonoBehaviour
{
	private Rigidbody curRbody
	{
		get
		{
			return PlayerManager.Instance.Player.rigidbody;
		}
	}

	private void Update()
	{
		if (this.curRbody.gameObject.activeInHierarchy && this.curRbody.velocity.magnitude > this.velocityForEnableEffect)
		{
			float t = (Mathf.Clamp(this.curRbody.velocity.magnitude, this.velocityForEnableEffect, this.velocityForMaxEffect) - this.velocityForEnableEffect) / (this.velocityForMaxEffect - this.velocityForEnableEffect);
			base.transform.localPosition = new Vector3(base.transform.localPosition.x, base.transform.localPosition.y, Mathf.Lerp(this.minZPos, this.maxZPos, t));
			base.transform.rotation = Quaternion.Euler(0f, 0f, UnityEngine.Random.Range(-this.deltaRot, this.deltaRot));
			this.RandomizeLinesZPositions();
		}
		else
		{
			base.transform.localPosition = new Vector3(base.transform.localPosition.x, base.transform.localPosition.y, this.minZPos);
		}
	}

	private void RandomizeLinesZPositions()
	{
		for (int i = 0; i < this.lines.Count; i++)
		{
			Transform transform = this.lines[i];
			transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, this.lineLocalZPos + UnityEngine.Random.Range(-this.deltaZ, this.deltaZ));
		}
	}

	public List<Transform> lines;

	private float minZPos = -8f;

	private float maxZPos = -4f;

	private float lineLocalZPos = 10f;

	public float deltaZ = 0.5f;

	public float deltaRot = 0.5f;

	public float velocityForMaxEffect = 20f;

	public float velocityForEnableEffect = 5f;
}

using System;
using Game.Character.CharacterController;
using Game.Effects;
using Game.GlobalComponent;
using Game.Weapons;
using UnityEngine;

public class EffectAreaProjectile : BallisticProjectile
{
	protected override void OnCollisionEnter(Collision col)
	{
		HitEntity component = col.collider.gameObject.GetComponent<HitEntity>();
		if (component != null)
		{
			component.OnHit(this.HitDamageType, this.currentOwner, this.ProjectileDamage, col.contacts[0].point, col.contacts[0].normal, this.DefenceIgnorance);
		}
		this.onHit(col);
	}

	public void onHit(Collision collision)
	{
		GameObject fromPool = PoolManager.Instance.GetFromPool(this.ExplosionPrefab);
		fromPool.transform.position = base.transform.position;
		Explosion component = fromPool.GetComponent<Explosion>();
		component.Init(this.currentOwner, this.ExplosionDamage, this.ExplosionRange, null);
		this.effectArea = PoolManager.Instance.GetFromPool(this.AreaEffectPrefab);
		this.effectArea.transform.position = base.transform.position;
		this.effectArea.transform.LookAt(new Vector3(0f, 0f, 0f), collision.impulse);
		this.effectAreaScript = this.effectArea.GetComponent<EffectArea>();
		this.effectAreaScript.Activate(this.currentOwner);
		this.DeInit();
	}

	public override void DeInit()
	{
		if (this.Trail != null)
		{
			this.Trail.SetActive(false);
		}
		this.rigidBody.velocity = Vector3.zero;
		this.rigidBody.angularVelocity = Vector3.zero;
		this.currentOwner = null;
		PoolManager.Instance.ReturnToPoolWithDelay(this.effectArea, this.effectAreaScript.EffectDuration + this.ReturnToPoolDelay);
		PoolManager.Instance.ReturnToPool(base.gameObject);
	}

	[Separator("EffectAreaProjectile")]
	public GameObject AreaEffectPrefab;

	public float ReturnToPoolDelay = 5f;

	private GameObject effectArea;

	private EffectArea effectAreaScript;
}

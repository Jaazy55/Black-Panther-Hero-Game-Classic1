using System;
using System.Collections;
using Game.Character;
using Game.Character.CameraEffects;
using Game.Character.CharacterController;
using Game.Character.Input;
using Game.Character.Stats;
using Game.Enemy;
using Game.GlobalComponent;
using Game.Vehicle;
using UnityEngine;

public class Transformer : MonoBehaviour
{
	private void Start()
	{
		this.bigDynamicLayer = 1 << LayerMask.NameToLayer("Terrain");
		this.Init(TransformerForm.Robot);
	}

	public void Init(TransformerForm form = TransformerForm.Robot)
	{
		if (this.inited)
		{
			return;
		}
		if (this.DebugLog)
		{
			UnityEngine.Debug.Log("Заинициэйтился");
		}
		this.robotModel.SetActive(true);
		this.currentForm = form;
		this.mainAnimator = base.GetComponent<Animator>();
		this.transformAnimator = this.transformationModel.GetComponent<Animator>();
		this.transformationModel.SetActive(false);
		this.mainController = base.GetComponent<AnimationController>();
		this.baseNpc = base.GetComponent<BaseNPC>();
		this.trColSize = this.transformationCollider.size;
		this.trColCenter = this.transformationCollider.center;
		this.transformationCollider.size = Vector3.zero;
		this.transformationCollider.gameObject.SetActive(false);
		this.owner = base.GetComponent<HitEntity>();
		this.dontGoThroughThings = base.GetComponent<DontGoThroughThings>();
		this.rb = base.GetComponent<Rigidbody>();
		this.inited = true;
	}

	public void DeInit()
	{
		base.StopAllCoroutines();
		this.transformating = false;
		this.transformationCollider.gameObject.SetActive(false);
		this.transformationModel.SetActive(false);
		this.inited = false;
	}

	private void Update()
	{
		if (this.isPlayer && InputManager.Instance.GetInput<bool>(InputType.Action, false) && !this.transformating)
		{
			this.Transform();
		}
		if (this.mainAnimator)
		{
			this.mainAnimator.applyRootMotion = !this.transformating;
		}
	}

	private void FixedUpdate()
	{
		if (this.transformating && this.currentForm == TransformerForm.Robot)
		{
			this.transformationCollider.size = Vector3.Lerp(Vector3.zero, this.trColSize, (Time.time - this.transformationStartTime) / this.timeToTransformation);
			this.transformationCollider.center = Vector3.Lerp(Vector3.zero, this.trColCenter, (Time.time - this.transformationStartTime) / this.timeToTransformation);
		}
	}

	public void Transform()
	{
		this.Transform(null, null);
	}

	public void Transform(GameObject existingCar, HitEntity target)
	{
		if (this.transformationTryTime + this.transformationTryCD > Time.time)
		{
			return;
		}
		if (!this.transformating)
		{
			this.transformationTryTime = Time.time;
			TransformerForm transformerForm = this.currentForm;
			if (transformerForm != TransformerForm.Robot)
			{
				if (transformerForm == TransformerForm.Car)
				{
					if (this.EnoughSpaceForRobot())
					{
						base.StartCoroutine(this.TransformToRobotCorutine(existingCar, target));
					}
				}
			}
			else if (this.EnoughSpaceForCar())
			{
				base.StartCoroutine(this.TransformToCarCorutine());
			}
			if (this.transformating)
			{
				PointSoundManager.Instance.PlaySoundAtPoint(base.transform.position, TypeOfSound.TransformationMain);
			}
			else
			{
				PointSoundManager.Instance.PlaySoundAtPoint(base.transform.position, TypeOfSound.Fail);
			}
		}
	}

	public IEnumerator TransformToCarCorutine()
	{
		this.transformating = true;
		if (this.isPlayer)
		{
			this.mainController.UseIkLook = false;
			CameraManager.Instance.GetCurrentCameraMode().SetCameraConfigMode("Default");
			CameraEffect cameraEffect = EffectManager.Instance.Create(Game.Character.CameraEffects.Type.SprintShake);
			cameraEffect.Stop();
		}
		yield return new WaitForSeconds(this.timeToTPose);
		this.mainAnimator.enabled = false;
		this.transformationStartTime = Time.time;
		this.transformationCollider.gameObject.SetActive(true);
		this.transformationCollider.size = Vector3.zero;
		this.rb.isKinematic = true;
		this.rb.velocity = Vector3.zero;
		this.robotModel.SetActive(false);
		this.transformationModel.SetActive(true);
		this.transformAnimator.SetFloat("SpeedMultipler", -this.animationDirection);
		this.transformAnimator.SetBool("Transformating", true);
		yield return new WaitForSeconds(this.timeToTransformation);
		this.transformAnimator.SetBool("Transformating", false);
		this.transformationModel.SetActive(false);
		this.currentForm = TransformerForm.Car;
		this.transformationCollider.gameObject.SetActive(false);
		if (this.isPlayer)
		{
			this.mainController.ExitAnimEnd();
		}
		this.car = PoolManager.Instance.GetFromPool(this.carPrefab);
		this.car.SetActive(false);
		this.car.transform.position = base.transform.position + base.transform.right * this.carOffset.x + base.transform.up * this.carOffset.y + base.transform.forward * this.carOffset.z;
		this.car.transform.rotation = base.transform.rotation;
		VehicleStatus carVehicleStatus = this.car.GetComponent<DrivableVehicle>().GetVehicleStatus();
		carVehicleStatus.Health = this.owner.Health;
		carVehicleStatus.Defence.Set(this.owner.Defence);
		carVehicleStatus.Faction = this.owner.Faction;
		this.car.SetActive(true);
		this.car.GetComponent<Rigidbody>().velocity = this.rb.velocity;
		base.transform.parent = this.car.transform;
		if (this.isPlayer)
		{
			PlayerInteractionsManager.Instance.InstantEnterVehicle(this.car.GetComponent<DrivableVehicle>(), true);
		}
		this.transformating = false;
		if (this.isPlayer)
		{
			this.mainController.ExitAnimStart();
		}
		else
		{
			this.baseNpc.CurrentController.enabled = false;
		}
		yield break;
	}

	public IEnumerator TransformToRobotCorutine(GameObject existingCar = null, HitEntity target = null)
	{
		this.transformating = true;
		if (existingCar != null)
		{
			this.Init(TransformerForm.Robot);
			this.car = existingCar;
		}
		this.rb.isKinematic = true;
		if (this.isPlayer)
		{
			this.mainController.UseIkLook = false;
		}
		else
		{
			this.baseNpc.CurrentController.enabled = false;
		}
		this.robotModel.SetActive(false);
		this.transformationModel.SetActive(true);
		this.transformAnimator.SetFloat("SpeedMultipler", this.animationDirection);
		this.transformAnimator.SetBool("Transformating", true);
		if (this.isPlayer)
		{
			this.mainController.ExitAnimStart();
			if (this.dontGoThroughThings)
			{
				this.dontGoThroughThings.SetPrevPostion(base.transform.position);
			}
			PlayerInteractionsManager.Instance.InstantExitVehicle();
		}
		else
		{
			base.transform.parent = null;
		}
		base.transform.position = this.car.transform.position - (base.transform.right * this.carOffset.x + base.transform.up * this.carOffset.y + base.transform.forward * this.carOffset.z);
		base.transform.rotation = Quaternion.LookRotation(Vector3.ProjectOnPlane(this.car.transform.forward, Vector3.up), Vector3.up);
		this.car.GetComponentInChildren<VehicleStatus>().Health = new CharacterStat();
		PoolManager.Instance.ReturnToPool(this.car);
		if (this.DebugLog)
		{
			UnityEngine.Debug.Log("Начал проигрывать трансформацию");
		}
		yield return new WaitForSeconds(this.timeToTransformation);
		if (this.DebugLog)
		{
			UnityEngine.Debug.Log("Закончил проигрывать трансформацию");
		}
		this.transformAnimator.SetBool("Transformating", false);
		this.robotModel.SetActive(true);
		this.transformationModel.SetActive(false);
		this.mainAnimator.enabled = true;
		this.currentForm = TransformerForm.Robot;
		this.rb.isKinematic = false;
		if (this.isPlayer)
		{
			this.mainController.UseIkLook = true;
			this.mainController.ExitAnimEnd();
		}
		else
		{
			this.baseNpc.CurrentController.enabled = true;
			if (target != null)
			{
				BaseControllerNPC baseControllerNPC;
				this.baseNpc.ChangeController(BaseNPC.NPCControllerType.Smart, out baseControllerNPC);
				((SmartHumanoidController)this.baseNpc.CurrentController).AddTarget(target, false);
				((SmartHumanoidController)this.baseNpc.CurrentController).InitBackToDummyLogic();
			}
		}
		this.transformating = false;
		yield break;
	}

	public void ResetToRobotForm()
	{
		if (this.isPlayer)
		{
			this.mainController.ExitAnimStart();
			if (this.dontGoThroughThings)
			{
				this.dontGoThroughThings.SetPrevPostion(base.transform.position);
			}
			PlayerInteractionsManager.Instance.InstantExitVehicle();
		}
		else
		{
			base.transform.parent = null;
		}
		base.transform.position = this.car.transform.position - (base.transform.right * this.carOffset.x + base.transform.up * this.carOffset.y + base.transform.forward * this.carOffset.z);
		base.transform.rotation = Quaternion.LookRotation(Vector3.ProjectOnPlane(this.car.transform.forward, Vector3.up), Vector3.up);
		this.car.GetComponentInChildren<VehicleStatus>().Health = new CharacterStat();
		PoolManager.Instance.ReturnToPool(this.car);
		this.mainAnimator.enabled = true;
		this.currentForm = TransformerForm.Robot;
		if (this.isPlayer)
		{
			this.mainController.ExitAnimEnd();
		}
		else
		{
			this.baseNpc.CurrentController.enabled = true;
		}
	}

	public virtual void OnCollisionEnter(Collision c)
	{
		this.Effects(c);
	}

	public void Effects(Collision c)
	{
		if (c.contacts.Length < 1)
		{
			return;
		}
		if (c.gameObject.layer == DrivableVehicle.terrainLayerNumber)
		{
			return;
		}
		float num = Mathf.Abs(Vector3.Dot(c.relativeVelocity, c.contacts[0].normal));
		if (c.gameObject.layer == DrivableVehicle.chatacterLayerNumber && num > 7f)
		{
			PointSoundManager.Instance.PlaySoundAtPoint(c.contacts[0].point, TypeOfSound.CarHitHuman);
		}
		else if (c.gameObject.layer == DrivableVehicle.smallDynamicLayerNumber && num > 7f)
		{
			Rigidbody component = c.gameObject.GetComponent<Rigidbody>();
			if (component)
			{
				float mass = component.mass;
				if (mass < 5f && this.hitTimer < 0f)
				{
					this.hitTimer = 1f;
					PointSoundManager.Instance.PlaySoundAtPoint(c.contacts[0].point, TypeOfSound.CarHitHuman);
				}
			}
			else
			{
				PointSoundManager.Instance.PlaySoundAtPoint(c.contacts[0].point, (num >= 30f) ? TypeOfSound.StrongCarHit : TypeOfSound.CarHit);
			}
		}
	}

	private bool EnoughSpaceForCar()
	{
		if (this.currentForm == TransformerForm.Car)
		{
			return true;
		}
		LayerMask mask = this.SpaceCheckMask;
		mask &= ~this.bigDynamicLayer;
		Vector3 center = this.transformationCollider.transform.position + this.trColCenter;
		Vector3 halfExtents = new Vector3(this.transformationCollider.transform.localScale.x * this.trColSize.x, this.transformationCollider.transform.localScale.y * this.trColSize.y, this.transformationCollider.transform.localScale.z * this.trColSize.z) / 2f;
		return !Physics.CheckBox(center, halfExtents, this.transformationCollider.transform.rotation, mask);
	}

	private bool EnoughSpaceForRobot()
	{
		if (this.currentForm == TransformerForm.Robot)
		{
			return true;
		}
		Transform parent = base.transform.parent;
		if (!parent)
		{
			return true;
		}
		Vector3 a = parent.position - (base.transform.right * this.carOffset.x + base.transform.up * this.carOffset.y + base.transform.forward * this.carOffset.z);
		return !Physics.Raycast(a + Vector3.up * 0.2f, Vector3.up, 4f, this.SpaceCheckMask);
	}

	private const float relativeSpeedForHitSmallDynamic = 7f;

	private const float relativeSpeedForStrongHit = 30f;

	private const float relativeSpeedForHitBigDynamic = 2f;

	public bool DebugLog;

	[Space(10f)]
	public TransformerForm currentForm;

	public GameObject robotModel;

	public GameObject transformationModel;

	public bool transformating;

	public float timeToTPose = 0.3f;

	public float timeToTransformation = 3.54f;

	public GameObject carPrefab;

	public Vector3 carOffset;

	public BoxCollider transformationCollider;

	public bool isPlayer;

	public float animationDirection = 1f;

	public LayerMask SpaceCheckMask;

	private Animator mainAnimator;

	private Animator transformAnimator;

	private AnimationController mainController;

	private BaseNPC baseNpc;

	private Vector3 trColSize;

	private Vector3 trColCenter;

	private HitEntity owner;

	private DontGoThroughThings dontGoThroughThings;

	private GameObject car;

	private float transformationStartTime;

	private Rigidbody rb;

	private bool inited;

	private float hitTimer = 1f;

	private int bigDynamicLayer;

	private float transformationTryTime;

	private float transformationTryCD = 1f;
}

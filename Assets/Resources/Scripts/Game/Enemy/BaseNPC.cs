using System;
using Game.Character;
using Game.Character.CharacterController;
using Game.GlobalComponent;
using UnityEngine;

namespace Game.Enemy
{
	public class BaseNPC : MonoBehaviour, IInitable
	{
		public virtual void OnAlarm(HitEntity disturber)
		{
			if (FactionsManager.Instance.GetRelations(disturber.Faction, this.StatusNpc.Faction) != Relations.Friendly)
			{
				this.ChangeController(BaseNPC.NPCControllerType.Smart);
				SmartHumanoidController smartHumanoidController = this.CurrentController as SmartHumanoidController;
				if (smartHumanoidController)
				{
					smartHumanoidController.AddPersonalTarget(disturber);
					smartHumanoidController.InitBackToDummyLogic();
				}
			}
		}

		public BaseControllerNPC CurrentController
		{
			get
			{
				return this.currentNpcController;
			}
		}

		public BaseNPC.NPCControllerType CurrentControllerType
		{
			get
			{
				return this.currentControllerType;
			}
		}

		public virtual void Init()
		{
			this.ChangeController(this.QuietControllerType);
			this.toCheckSectorTimer = 1f;
		}

		public virtual void DeInit()
		{
			this.DeInitCurrentController();
		}

		public virtual void ChangeController(BaseNPC.NPCControllerType newControllerType)
		{
			BaseControllerNPC baseControllerNPC;
			this.ChangeController(newControllerType, out baseControllerNPC);
		}

		public virtual void ChangeController(BaseNPC.NPCControllerType newControllerType, out BaseControllerNPC controller)
		{
			if (this.currentControllerType == newControllerType)
			{
				controller = this.currentNpcController;
				return;
			}
			this.DeInitCurrentController();
			BaseControllerNPC baseControllerNPC = null;
			foreach (BaseNPC.NPCControllerLink npccontrollerLink in this.Controllers)
			{
				if (npccontrollerLink.ControllerType == newControllerType)
				{
					baseControllerNPC = npccontrollerLink.Controller;
					break;
				}
			}
			if (baseControllerNPC == null)
			{
				throw new Exception(string.Concat(new object[]
				{
					"Controller type '",
					newControllerType,
					"' for NPC '",
					base.gameObject.name,
					"' not assigned!"
				}));
			}
			this.InitController(baseControllerNPC);
			this.currentControllerType = newControllerType;
			controller = this.currentNpcController;
		}

		public void TryTalk(TalkingObject.PhraseType type)
		{
			this.NPCPhrases.TalkPhraseOfType(type);
		}

		private void Awake()
		{
			this.slowUpdateProc = new SlowUpdateProc(new SlowUpdateProc.SlowUpdateDelegate(this.SlowUpdate), 1f);
			if (this.WaterSensor == null)
			{
				this.WaterSensor = base.GetComponentInChildren<WaterSensor>();
			}
		}

		private void FixedUpdate()
		{
			this.slowUpdateProc.ProceedOnFixedUpdate();
		}

		private void SlowUpdate()
		{
			if (this.toCheckSectorTimer > 0f)
			{
				this.toCheckSectorTimer -= this.slowUpdateProc.DeltaTime;
				return;
			}
			if (!SectorManager.Instance.IsInActiveSector(base.transform.position) || PlayerInteractionsManager.Instance.IsPlayerInMetro)
			{
				PoolManager.Instance.ReturnToPool(this);
			}
		}

		private void DeInitCurrentController()
		{
			if (this.currentNpcController == null)
			{
				return;
			}
			this.currentNpcController.DeInit();
			PoolManager.Instance.ReturnToPool(this.currentNpcController);
			this.currentNpcController = null;
			this.currentControllerType = BaseNPC.NPCControllerType.None;
		}

		private void InitController(BaseControllerNPC newController)
		{
			this.currentNpcController = PoolManager.Instance.GetFromPool<BaseControllerNPC>(newController, base.transform.position, base.transform.rotation);
			this.currentNpcController.transform.parent = base.transform;
			this.currentNpcController.Init(this);
		}

		private void OnTriggerStay()
		{
		}

		private const int ToCheckSectorTime = 1;

		public BaseNPC.NPCControllerLink[] Controllers;

		public BaseNPC.NPCControllerType QuietControllerType;

		[Separator("Public Links")]
		public Rigidbody NPCRigidbody;

		public Animator NPCAnimator;

		public GameObject RootModel;

		public SpecificNPCLinks SpecificNpcLinks;

		public BaseStatusNPC StatusNpc;

		public TalkingObject NPCPhrases;

		public WaterSensor WaterSensor;

		private BaseNPC.NPCControllerType currentControllerType = BaseNPC.NPCControllerType.None;

		private BaseControllerNPC currentNpcController;

		private SlowUpdateProc slowUpdateProc;

		private float toCheckSectorTimer;

		public enum NPCControllerType
		{
			None = -1,
			Simple,
			Smart,
			Pedestrian
		}

		[Serializable]
		public class NPCControllerLink
		{
			public BaseNPC.NPCControllerType ControllerType;

			public BaseControllerNPC Controller;
		}
	}
}

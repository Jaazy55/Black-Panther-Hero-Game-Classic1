using System;
using System.Collections.Generic;
using Game.Character;
using Game.Character.CharacterController;
using Game.MiniMap;
using Game.Vehicle;
using UnityEngine;

namespace Game.GlobalComponent
{
	public class Metro : MonoBehaviour
	{
		private void Start()
		{
			MetroManager.Instance.RegisterMetro(this);
			if (!this.MetroMark)
			{
				this.MetroMark = base.GetComponentInChildren<MetroMarkForMiniMap>();
			}
		}

		public void EnterInMetro(CutsceneManager.Callback callback)
		{
			MetroManager.Instance.CurrentMetro = this;
			this.MainEnterCutsceneManager.Init(callback, new CutsceneManager.Callback(this.ExitMetroCallback));
		}

		public void RemoveObstructive()
		{
			this.m_ObstructiveRigids = new List<Rigidbody>();
			Collider[] array = Physics.OverlapBox(base.transform.TransformPoint(this.DeathBox.center), this.DeathBox.extents, base.transform.rotation);
			for (int i = 0; i < array.Length; i++)
			{
				Rigidbody attachedRigidbody = array[i].attachedRigidbody;
				if (attachedRigidbody != null && !this.m_ObstructiveRigids.Contains(attachedRigidbody))
				{
					this.m_ObstructiveRigids.Add(attachedRigidbody);
				}
			}
			for (int j = 0; j < this.m_ObstructiveRigids.Count; j++)
			{
				DrivableVehicle component = this.m_ObstructiveRigids[j].GetComponent<DrivableVehicle>();
				if (component != null)
				{
					component.DestroyVehicle();
				}
			}
		}

		public void ExitFromMetro()
		{
			MetroManager.Instance.CurrentMetro = this;
			MetroManager.Instance.TerminusMetro = null;
			PlayerInteractionsManager.Instance.Player.transform.position = this.EnterPoint.position;
			PlayerInteractionsManager.Instance.Player.transform.rotation = this.EnterPoint.rotation;
			this.MainExitCutsceneManager.Init(delegate
			{
				MetroManager.Instance.InMetro = false;
			}, new CutsceneManager.Callback(this.ExitMetroCallback));
		}

		private void ExitMetroCallback()
		{
			MetroManager.Instance.InMetro = false;
		}

		private void OnTriggerEnter(Collider col)
		{
			Player componentInParent = col.GetComponentInParent<Player>();
			if (!componentInParent || componentInParent.IsFlying)
			{
				return;
			}
			if (PlayerInteractionsManager.Instance.inVehicle)
			{
				return;
			}
			if (componentInParent.transform.position.y - base.transform.position.y < -2f)
			{
				MetroManager.Instance.GetInMetro();
			}
		}

		private const float MetroHight = -2f;

		public Transform EnterPoint;

		public CutsceneManager MainEnterCutsceneManager;

		public CutsceneManager MainExitCutsceneManager;

		public MetroMarkForMiniMap MetroMark;

		public Bounds DeathBox;

		public List<Rigidbody> m_ObstructiveRigids;
	}
}

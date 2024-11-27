using System;
using Game.Shop;
using UnityEngine;

namespace Game.Items
{
	public class GameItemPowerUp : GameItem
	{
		public override bool CanBeEquiped
		{
			get
			{
				return this.RemainingDuration > 0 || !TimeManager.OnCoolDown(this.cdStartTime, this.Cooldawn);
			}
		}

		public bool Paused { get; protected set; }

		public int RemainingDuration { get; protected set; }

		public int RemainingCooldawn { get; protected set; }

		public bool isActive { get; protected set; }

		private void Awake()
		{
			ShopManager.Instance.ShopOpeningEvent = new ShopManager.ShopOpened(this.Pause);
			ShopManager.Instance.ShopCloseningEvent = new ShopManager.ShopClosed(this.Сontinue);
		}

		private void FixedUpdate()
		{
			if (!this.isActive || this.Paused)
			{
				return;
			}
			this.remainingUpdateCD -= Time.deltaTime;
			if (this.remainingUpdateCD <= 0f)
			{
				this.RemainingDuration -= 3;
				this.remainingUpdateCD = 3f;
				BaseProfile.StoreValue<int>(this.RemainingDuration, "duration" + this.ID);
			}
			if (this.RemainingDuration <= 0)
			{
				this.Deactivate();
			}
		}

		public void Pause()
		{
			this.Paused = true;
		}

		public void Сontinue()
		{
			this.Paused = false;
		}

		public override void UpdateItem()
		{
			if (!ShopManager.Instance.BoughtAlredy(this))
			{
				return;
			}
			this.cdStartTime = BaseProfile.GetValue<long>("cdStart" + this.ID);
			this.RemainingDuration = BaseProfile.GetValue<int>("duration" + this.ID);
			this.RemainingCooldawn = TimeManager.RemainingCooldawn(this.cdStartTime, this.Cooldawn);
			if (!this.isActive && !TimeManager.OnCoolDown(this.cdStartTime, this.Cooldawn))
			{
				this.ResetDuration();
			}
		}

		public override void OnBuy()
		{
			this.cdStartTime = DateTime.Now.ToFileTime() - this.Cooldawn;
			this.ResetDuration();
			BaseProfile.StoreValue<long>(this.cdStartTime, "cdStart" + this.ID);
			BaseProfile.StoreValue<int>(this.RemainingDuration, "duration" + this.ID);
		}

		private void ResetDuration()
		{
			this.RemainingDuration = this.Duration * ShopManager.Instance.GetBIValue(this.ID, this.ShopVariables.gemPrice);
			BaseProfile.StoreValue<int>(this.RemainingDuration, "duration" + this.ID);
		}

		public virtual void Activate()
		{
			if (this.isActive)
			{
				return;
			}
			this.isActive = true;
			StuffManager.ActivePowerUps.Add(this);
		}

		public virtual void Deactivate()
		{
			if (!this.isActive)
			{
				return;
			}
			this.cdStartTime = DateTime.Now.ToFileTime();
			BaseProfile.StoreValue<long>(this.cdStartTime, "cdStart" + this.ID);
			BaseProfile.StoreValue<int>(this.RemainingDuration, "duration" + this.ID);
			this.isActive = false;
			StuffManager.ActivePowerUps.Remove(this);
		}

		private const int updateCD = 3;

		private const string durationStorePrefix = "duration";

		private const string cdStartStorePrefix = "cdStart";

		[Tooltip("In seconds")]
		public int Duration = 60;

		[Tooltip("In seconds")]
		public long Cooldawn = 600L;

		private long cdStartTime;

		private float remainingUpdateCD = 3f;
	}
}

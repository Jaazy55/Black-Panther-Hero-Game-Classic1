using System;
using System.Collections.Generic;
using Game.GlobalComponent;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Items
{
	public class PowerUpTimeStatusPanel : MonoBehaviour
	{
		private void Awake()
		{
			this.slowUpdateProc = new SlowUpdateProc(new SlowUpdateProc.SlowUpdateDelegate(this.SlowUpdate), 2f);
			this.image = base.GetComponent<Image>();
			this.lastAspectRatio = this.RatioFitter.aspectRatio;
		}

		private void OnEnable()
		{
			this.CheckActualPowerUp();
		}

		private void FixedUpdate()
		{
			this.slowUpdateProc.ProceedOnFixedUpdate();
		}

		private void SlowUpdate()
		{
			this.CheckActualPowerUp();
		}

		private void FillContent()
		{
			for (int i = 0; i < StuffManager.ActivePowerUps.Count; i++)
			{
				if (this.MaxShowedItemsCount > 0 && this.trackedPowerUps.Count >= this.MaxShowedItemsCount)
				{
					break;
				}
				GameItemPowerUp gameItemPowerUp = StuffManager.ActivePowerUps[i];
				if (!this.trackedPowerUps.Contains(gameItemPowerUp))
				{
					if (this.MinRemainingDurationToShow <= 0 || gameItemPowerUp.RemainingDuration <= this.MinRemainingDurationToShow)
					{
						PowerUpTimeStatusItem fromPool = PoolManager.Instance.GetFromPool<PowerUpTimeStatusItem>(this.StatusItemPrefab);
						fromPool.transform.parent = this.ContentContainer;
						fromPool.transform.localScale = Vector3.one;
						fromPool.Init(gameItemPowerUp);
						this.trackedPowerUps.Add(gameItemPowerUp);
						this.powerUpToStatus.Add(gameItemPowerUp, fromPool);
					}
				}
			}
		}

		private void CheckActualPowerUp()
		{
			for (int i = 0; i < this.trackedPowerUps.Count; i++)
			{
				GameItemPowerUp gameItemPowerUp = this.trackedPowerUps[i];
				if (!StuffManager.ActivePowerUps.Contains(gameItemPowerUp))
				{
					PowerUpTimeStatusItem o = this.powerUpToStatus[gameItemPowerUp];
					PoolManager.Instance.ReturnToPool(o);
					this.trackedPowerUps.Remove(gameItemPowerUp);
					this.powerUpToStatus.Remove(gameItemPowerUp);
				}
			}
			if (StuffManager.ActivePowerUps.Count != this.trackedPowerUps.Count)
			{
				this.FillContent();
			}
			bool flag = this.trackedPowerUps.Count > 0;
			if (this.image)
			{
				this.image.enabled = flag;
			}
			if (this.GridLayoutGroup)
			{
				this.GridLayoutGroup.enabled = flag;
			}
			if (this.RatioFitter)
			{
				this.RatioFitter.enabled = flag;
				if (flag)
				{
					float num = (float)this.trackedPowerUps.Count + this.AdditionalAspectRatio;
					if (Math.Abs(this.lastAspectRatio - num) > 0f)
					{
						this.RatioFitter.aspectRatio = num;
					}
				}
			}
		}

		private const int CheckActualPowerUpPeriod = 2;

		public RectTransform ContentContainer;

		public PowerUpTimeStatusItem StatusItemPrefab;

		public AspectRatioFitter RatioFitter;

		public GridLayoutGroup GridLayoutGroup;

		public float AdditionalAspectRatio = 0.5f;

		[Tooltip("0 - infinity")]
		public int MaxShowedItemsCount;

		[Tooltip("0 - not consider time")]
		public int MinRemainingDurationToShow;

		private readonly List<GameItemPowerUp> trackedPowerUps = new List<GameItemPowerUp>();

		private readonly Dictionary<GameItemPowerUp, PowerUpTimeStatusItem> powerUpToStatus = new Dictionary<GameItemPowerUp, PowerUpTimeStatusItem>();

		private SlowUpdateProc slowUpdateProc;

		private Image image;

		private float lastAspectRatio;
	}
}

using System;
using UnityEngine;

namespace Game.GlobalComponent.Quality
{
	public class QualityButton : MonoBehaviour
	{
		private void Awake()
		{
			QualityManager.updateQuality = (QualityManager.UpdateQuality)Delegate.Combine(QualityManager.updateQuality, new QualityManager.UpdateQuality(this.UpdateButtonState));
		}

		private void OnDestroy()
		{
			QualityManager.updateQuality = (QualityManager.UpdateQuality)Delegate.Remove(QualityManager.updateQuality, new QualityManager.UpdateQuality(this.UpdateButtonState));
		}

		private void OnEnable()
		{
			this.UpdateButtonState();
		}

		public void UpdateButtonState()
		{
			bool flag = QualityManager.QualityLvl == this.QualityLvl;
			this.OnStateObject.SetActive(flag);
			this.OffStateObject.SetActive(!flag);
		}

		public void SetQualityLvl()
		{
			QualityManager.ChangeQuality(this.QualityLvl, this.ApplyNow);
		}

		public bool ApplyNow;

		public QualityLvls QualityLvl;

		public GameObject OnStateObject;

		public GameObject OffStateObject;
	}
}

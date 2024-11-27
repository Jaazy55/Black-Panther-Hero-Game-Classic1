using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Game.GlobalComponent.Qwest
{
	public class QwestPanelManager : MonoBehaviour
	{
		public void OnPanelOpen()
		{
			try
			{
				if (GameEventManager.Instance == null)
				{
					return;
				}
			}
			catch (Exception)
			{
				return;
			}
			this.ReloadRecords();
		}

		public void ReloadRecords()
		{
			foreach (QwestUIRecord qwestUIRecord in this.Records)
			{
				qwestUIRecord.gameObject.SetActive(false);
				qwestUIRecord.Qwest = null;
			}
			int num = 0;
			foreach (Qwest qwest in GameEventManager.Instance.ActiveQwests)
			{
				QwestUIRecord qwestUIRecord2 = this.Records[num + 1];
				qwestUIRecord2.gameObject.SetActive(true);
				qwestUIRecord2.Qwest = qwest;
				Text componentInChildren = qwestUIRecord2.GetComponentInChildren<Text>();
				componentInChildren.text = qwest.GetQwestStatus();
				num++;
			}
			if (GameEventManager.Instance.ActiveQwests.Count == 0)
			{
				QwestUIRecord qwestUIRecord3 = this.Records[0];
				qwestUIRecord3.gameObject.SetActive(true);
			}
			else
			{
				this.Records[0].gameObject.SetActive(false);
			}
			this.ReviewRecords();
		}

		public void ReviewRecords()
		{
			foreach (QwestUIRecord qwestUIRecord in this.Records)
			{
				Image component = qwestUIRecord.GetComponent<Image>();
				if (qwestUIRecord.Qwest != null && qwestUIRecord.Qwest.Equals(GameEventManager.Instance.MarkedQwest))
				{
					component.sprite = this.markSprite;
				}
				else if (qwestUIRecord.Qwest != null)
				{
					component.sprite = this.defaultSprite;
				}
			}
		}

		private void OnEnable()
		{
			this.OnPanelOpen();
		}

		public Sprite markSprite;

		public Sprite defaultSprite;

		public List<QwestUIRecord> Records;
	}
}

using System;
using UnityEngine;

namespace Game.GlobalComponent.Qwest
{
	public class QwestUIRecord : MonoBehaviour
	{
		public void CancelQwest()
		{
			if (this.Qwest != null)
			{
				QwestTimerManager.Instance.QwestCanceled(this.Qwest);
				GameEventManager.Instance.QwestFailed(this.Qwest);
			}
		}

		public void RecordClick()
		{
			if (this.Qwest != null && GameEventManager.Instance.TaskSelectionAvailable)
			{
				GameEventManager.Instance.MarkedQwest = this.Qwest;
				GameEventManager.Instance.RefreshQwestArrow();
			}
		}

		public Qwest Qwest;
	}
}

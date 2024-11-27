using System;
using UnityEngine;

namespace Game.GlobalComponent.Qwest
{
	public class QwestArrowToggle : MonoBehaviour
	{
		public void Toggle(bool isChecked)
		{
			QwestProfile.QwestArrow = isChecked;
			GameEventManager.Instance.RefreshQwestArrow();
		}
	}
}

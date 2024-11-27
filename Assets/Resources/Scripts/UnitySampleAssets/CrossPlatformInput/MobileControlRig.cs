using System;
using System.Collections;
using UnityEngine;

namespace UnitySampleAssets.CrossPlatformInput
{
	public class MobileControlRig : MonoBehaviour
	{
		private void OnEnable()
		{
			this.CheckEnableControlRig();
		}

		private void CheckEnableControlRig()
		{
			this.EnableControlRig(false);
		}

		private void EnableControlRig(bool enabled)
		{
			IEnumerator enumerator = base.transform.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					object obj = enumerator.Current;
					Transform transform = (Transform)obj;
					transform.gameObject.SetActive(enabled);
				}
			}
			finally
			{
				IDisposable disposable;
				if ((disposable = (enumerator as IDisposable)) != null)
				{
					disposable.Dispose();
				}
			}
		}
	}
}

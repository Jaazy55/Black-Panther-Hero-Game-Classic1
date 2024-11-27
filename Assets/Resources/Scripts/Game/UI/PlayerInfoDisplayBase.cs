using System;
using Game.Character;
using UnityEngine;

namespace Game.UI
{
	public abstract class PlayerInfoDisplayBase : MonoBehaviour
	{
		protected abstract PlayerInfoType GetInfoType();

		protected abstract void Display();

		private void Awake()
		{
			PlayerInfoManager.Instance.AddOnValueChangedEvent(this.GetInfoType(), new PlayerInfoManager.OnInfoValueChanged(this.OnChangeValue));
			this.InfoValue = PlayerInfoManager.Instance.GetInfoValue(this.GetInfoType());
			this.Display();
		}

		private void OnChangeValue(int newValue)
		{
			this.InfoValue = newValue;
			this.Display();
		}

		protected int InfoValue;
	}
}

using System;
using UnityEngine;
using UnityEngine.UI;

namespace Prefabs.Managers.Shop.Prefabs
{
	public class StatView : MonoBehaviour
	{
		public void SetIcon(Sprite sp)
		{
			this.icon.sprite = sp;
		}

		public void SetNameStat(string nameStat)
		{
			this.nameStat.text = nameStat.ToUpper();
		}

		public virtual void SetValue(float value)
		{
			if (value > 0f)
			{
				this.value.text = "+" + value.ToString("F2");
			}
			else if (value < 0f)
			{
				this.value.text = "-" + value.ToString("F2");
			}
			else
			{
				this.value.text = value.ToString("F2");
			}
		}

		[SerializeField]
		private Image icon;

		[SerializeField]
		private Text nameStat;

		[SerializeField]
		protected Text value;
	}
}

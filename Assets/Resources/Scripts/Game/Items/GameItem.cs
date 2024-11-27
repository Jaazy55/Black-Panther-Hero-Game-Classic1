using System;
using UnityEngine;

namespace Game.Items
{
	public class GameItem : MonoBehaviour
	{
		public virtual bool CanBeBought
		{
			get
			{
				return true;
			}
		}

		public virtual bool CanBeEquiped
		{
			get
			{
				return true;
			}
		}

		public virtual void Init()
		{
		}

		public virtual void UpdateItem()
		{
		}

		public virtual void OnBuy()
		{
		}

		public virtual void OnEquip()
		{
		}

		public virtual void OnUnequip()
		{
		}

		public virtual bool SameParametrWithOther(object[] parametrs)
		{
			return false;
		}

		[ShowOnly]
		public int ID;

		public ItemsTypes Type;

		public ShopVariables ShopVariables;

		public PreviewVariables PreviewVariables;
	}
}

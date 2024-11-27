using System;
using UnityEngine;

namespace Game.MiniMap
{
	public class MarkButton : MonoBehaviour
	{
		public void Init(MarkForMiniMap m)
		{
			this.mark = m;
		}

		public void OnMarkClick()
		{
			this.mark.MarckOnClick();
		}

		private MarkForMiniMap mark;
	}
}

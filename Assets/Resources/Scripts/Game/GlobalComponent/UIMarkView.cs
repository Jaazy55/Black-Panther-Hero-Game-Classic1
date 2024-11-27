using System;
using UnityEngine;
using UnityEngine.UI;

namespace Game.GlobalComponent
{
	public class UIMarkView : UIMarkViewBase
	{
		private void Update()
		{
			base.IconColor = Color.Lerp(this.m_ColorA, this.m_ColorB, this.m_Curve.Evaluate(Time.unscaledTime));
		}

		public void UpdateDistanceLabel(float dist)
		{
			if (this.lastDistance != (int)dist)
			{
				this.lastDistance = (int)dist;
				this.m_DistanceLabel.text = this.lastDistance + " M";
			}
		}

		[SerializeField]
		private Text m_DistanceLabel;

		[SerializeField]
		private Color m_ColorA;

		[SerializeField]
		private Color m_ColorB;

		[SerializeField]
		private AnimationCurve m_Curve;

		private int lastDistance;
	}
}

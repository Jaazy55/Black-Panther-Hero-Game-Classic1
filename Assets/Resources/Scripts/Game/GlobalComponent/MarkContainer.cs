using System;
using UnityEngine;

namespace Game.GlobalComponent
{
	[Serializable]
	public class MarkContainer
	{
		public MarkContainer()
		{
		}

		public MarkContainer(UIMarkViewBase markView, Transform targetTransform, MarkDetails details)
		{
			this.mark = markView;
			this.target = targetTransform;
			this.SetupMark(details);
		}

		public Transform Target
		{
			get
			{
				return this.target;
			}
			set
			{
				this.target = value;
			}
		}

		public void SetupMark(MarkDetails details)
		{
			if (this.mark == null)
			{
				return;
			}
			this.mark.SetIconSprite(details.icon);
			this.mark.IconColor = details.color;
			this.mark.transform.localScale = details.scale;
			this.MinDistanceView = details.hideDistance;
			this.offset = details.offset;
		}

		public void FreeResources()
		{
			this.target = null;
			if (this.mark != null)
			{
				UnityEngine.Object.Destroy(this.mark.gameObject);
			}
		}

		public UIMarkViewBase mark;

		[SerializeField]
		private Transform target;

		[Range(0f, 100f)]
		public float MinDistanceView = 3f;

		public Vector3 offset;
	}
}

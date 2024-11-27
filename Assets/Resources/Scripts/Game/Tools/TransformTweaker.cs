using System;
using System.Collections;
using UnityEngine;

namespace Game.Tools
{
	public class TransformTweaker : MonoBehaviour
	{
		public void Tweak()
		{
			if (!this.TargetTransform || !this.ChangeableTransform)
			{
				UnityEngine.Debug.LogWarning("Not set target transform or changable transform");
				return;
			}
			if (!this.copyPosition && !this.copyRotation && !this.copyScale)
			{
				UnityEngine.Debug.LogWarning("At least one option should be selected in 'what to copy'");
				return;
			}
			this.counterDepth = 0;
			this.TweakingTransform(this.TargetTransform, this.ChangeableTransform);
		}

		private void TweakingTransform(Transform target, Transform changeable)
		{
			this.CopyValues(target, changeable);
			if (this.recursive && this.counterDepth <= this.howDeep)
			{
				this.counterDepth++;
				IEnumerator enumerator = changeable.GetEnumerator();
				try
				{
					while (enumerator.MoveNext())
					{
						object obj = enumerator.Current;
						Transform transform = (Transform)obj;
						Transform transform2 = target.Find(transform.name);
						if (transform2)
						{
							this.TweakingTransform(transform2, transform);
						}
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

		private void CopyValues(Transform target, Transform changeable)
		{
			if (this.copyPosition)
			{
				if (this.copyLocalValue)
				{
					changeable.localPosition = target.localPosition;
				}
				else
				{
					changeable.position = target.position;
				}
			}
			if (this.copyRotation)
			{
				if (this.copyLocalValue)
				{
					changeable.localRotation = target.localRotation;
				}
				else
				{
					changeable.rotation = target.rotation;
				}
			}
			if (this.copyScale)
			{
				changeable.localScale = target.localScale;
			}
		}

		public Transform TargetTransform;

		public Transform ChangeableTransform;

		public bool recursive;

		public bool copyLocalValue;

		public int howDeep;

		[Separator("What to copy")]
		public bool copyPosition;

		public bool copyRotation;

		public bool copyScale;

		[InspectorButton("Tweak")]
		public bool tweak;

		private int counterDepth;
	}
}

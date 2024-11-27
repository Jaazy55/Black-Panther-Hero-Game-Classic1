using System;
using System.Collections.Generic;
using Game.Character.Utils;
using UnityEngine;

namespace Game.Character.CharacterController
{
	internal class Tweener : MonoBehaviour
	{
		public static Tweener Instance
		{
			get
			{
				if (!Tweener.instance)
				{
					Tweener.instance = CameraInstance.CreateInstance<Tweener>("Tweener");
				}
				return Tweener.instance;
			}
		}

		public void MoveTo(Transform trans, Vector3 targetPos, float time, Tweener.OnFinish onFinish = null)
		{
			this.tweens.Add(new Tweener.TweenPos
			{
				Transform = trans,
				StartPos = trans.position,
				TargetPos = targetPos,
				Time = time,
				Timeout = 0f,
				callback = onFinish
			});
		}

		public void RotateTo(Transform trans, Quaternion rot, float time, Tweener.OnFinish onFinish = null)
		{
			this.tweens.Add(new Tweener.TweenRot
			{
				Transform = trans,
				StartRot = trans.rotation,
				TargetRot = rot,
				Time = time,
				Timeout = 0f,
				callback = onFinish
			});
		}

		private void Awake()
		{
			Tweener.instance = this;
			this.tweens = new List<Tweener.Tween>();
			this.finishedTweens = new List<Tweener.Tween>();
		}

		private void FixedUpdate()
		{
			foreach (Tweener.Tween tween in this.tweens)
			{
				tween.Update();
				if (tween.Timeout >= tween.Time)
				{
					if (tween.callback != null)
					{
						tween.callback();
					}
					this.finishedTweens.Add(tween);
				}
			}
			foreach (Tweener.Tween item in this.finishedTweens)
			{
				this.tweens.Remove(item);
			}
			this.finishedTweens.Clear();
		}

		private static Tweener instance;

		private List<Tweener.Tween> tweens;

		private List<Tweener.Tween> finishedTweens;

		public delegate void OnFinish();

		private abstract class Tween
		{
			public abstract void Update();

			public Transform Transform;

			public float Time;

			public float Timeout;

			public Tweener.OnFinish callback;
		}

		private class TweenPos : Tweener.Tween
		{
			public override void Update()
			{
				this.Timeout += UnityEngine.Time.deltaTime;
				float t = this.Timeout / this.Time;
				this.Transform.position = Vector3.Lerp(this.StartPos, this.TargetPos, t);
			}

			public Vector3 TargetPos;

			public Vector3 StartPos;
		}

		private class TweenRot : Tweener.Tween
		{
			public override void Update()
			{
				this.Timeout += UnityEngine.Time.deltaTime;
				float t = this.Timeout / this.Time;
				this.Transform.rotation = Quaternion.Slerp(this.StartRot, this.TargetRot, t);
			}

			public Quaternion TargetRot;

			public Quaternion StartRot;
		}
	}
}

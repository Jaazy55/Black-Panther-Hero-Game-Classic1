using System;
using System.Diagnostics;
using UnityEngine;

namespace Game.GlobalComponent
{
	public abstract class PerformanceTest : MonoBehaviour
	{
		//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event PerformanceTest.CalcOfResults EndTestingEvent;

		public abstract void Init();

		public virtual float GetResult()
		{
			return this.Result;
		}

		public void CallEndTestingEvent(float result, PerformanceTest test)
		{
			this.IsEnd = true;
			if (this.EndTestingEvent != null)
			{
				this.EndTestingEvent(result, test);
			}
		}

		public abstract void EndTesting();

		[HideInInspector]
		public bool IsEnd;

		public bool IsNotReturnResults;

		public float DetectingTime;

		[Range(0f, 100f)]
		protected float Result;

		public delegate void CalcOfResults(float result, PerformanceTest test);
	}
}

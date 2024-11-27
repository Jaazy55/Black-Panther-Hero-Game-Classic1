using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.GlobalComponent
{
	public class MainThreadExecuter : MonoBehaviour
	{
		public static MainThreadExecuter Instance
		{
			get
			{
				return MainThreadExecuter.instance;
			}
		}

		public void Run(MainThreadExecuter.Runnable runnable)
		{
			this.runnables.Enqueue(runnable);
		}

		private void Awake()
		{
			if (MainThreadExecuter.instance == null)
			{
				UnityEngine.Object.DontDestroyOnLoad(this);
				MainThreadExecuter.instance = this;
				this.runnables = new Queue<MainThreadExecuter.Runnable>();
			}
			else
			{
				UnityEngine.Object.Destroy(base.gameObject);
			}
		}

		private void Update()
		{
			for (int i = 0; i < this.RunsPerFrame; i++)
			{
				if (this.runnables.Count <= 0)
				{
					break;
				}
				this.runnables.Dequeue()();
			}
		}

		public int RunsPerFrame = 5;

		private static MainThreadExecuter instance;

		private Queue<MainThreadExecuter.Runnable> runnables;

		public delegate void Runnable();
	}
}

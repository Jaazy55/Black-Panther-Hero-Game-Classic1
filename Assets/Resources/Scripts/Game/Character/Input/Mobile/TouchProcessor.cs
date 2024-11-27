using System;
using UnityEngine;

namespace Game.Character.Input.Mobile
{
	public class TouchProcessor
	{
		public TouchProcessor(int numberOfTouches)
		{
			this.touches = new SimTouch[numberOfTouches];
			for (int i = 0; i < this.touches.Length; i++)
			{
				this.touches[i] = new SimTouch(i, KeyCode.LeftAlt);
			}
		}

		public SimTouch[] GetTouches()
		{
			return this.touches;
		}

		public int GetTouchCount()
		{
			return this.touches.Length;
		}

		public int GetActiveTouchCount()
		{
			int num = 0;
			foreach (SimTouch simTouch in this.touches)
			{
				if (simTouch.Status != TouchStatus.Invalid)
				{
					num++;
				}
			}
			return num;
		}

		public SimTouch GetTouch(int index)
		{
			return this.touches[index];
		}

		public void ScanInput()
		{
			for (int i = 0; i < this.touches.Length; i++)
			{
				this.touches[i].ScanInput();
			}
		}

		public void ShowDebug(bool status)
		{
		}

		private readonly SimTouch[] touches;
	}
}

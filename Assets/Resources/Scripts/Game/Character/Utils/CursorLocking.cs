using System;
using UnityEngine;

namespace Game.Character.Utils
{
	public class CursorLocking : MonoBehaviour
	{
		private void Awake()
		{
			CursorLocking.instance = this;
		}

		private void Update()
		{
			if (this.LockCursor)
			{
				CursorLocking.Lock();
			}
			else
			{
				CursorLocking.Unlock();
			}
			CursorLocking.IsLocked = Compatibility.IsCursorLocked();
			if (UnityEngine.Input.GetKeyDown(this.LockKey))
			{
				CursorLocking.Lock();
			}
			if (UnityEngine.Input.GetKeyDown(this.UnlockKey))
			{
				CursorLocking.Unlock();
			}
			if (!Compatibility.IsCursorLocked())
			{
				Compatibility.SetCursorVisible(true);
			}
		}

		public static void Lock()
		{
			Compatibility.LockCursor(false);
			Compatibility.SetCursorVisible(false);
			CursorLocking.instance.LockCursor = false;
		}

		public static void Unlock()
		{
			Compatibility.LockCursor(false);
			Compatibility.SetCursorVisible(true);
			CursorLocking.instance.LockCursor = false;
		}

		public bool LockCursor;

		public KeyCode LockKey;

		public KeyCode UnlockKey;

		public static bool IsLocked;

		private static CursorLocking instance;
	}
}

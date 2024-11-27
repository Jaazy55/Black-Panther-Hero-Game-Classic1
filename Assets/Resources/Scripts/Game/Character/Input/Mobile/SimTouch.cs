using System;
using UnityEngine;

namespace Game.Character.Input.Mobile
{
	public class SimTouch
	{
		public SimTouch(int fingerID, KeyCode simKey)
		{
			this.FingerId = fingerID;
			SimTouch.SimulationKey = simKey;
			this.lastMouseStatus = SimTouch.MouseStatus.None;
		}

		public void ScanInput()
		{
			this.UpdateTouchInput();
		}

		private SimTouch.MouseStatus GetMouseStatus()
		{
			bool flag = true;
			if (UnityEngine.Input.GetKey(SimTouch.SimulationKey))
			{
				if (this.FingerId > 0)
				{
					if (this.lastMouseStatus != SimTouch.MouseStatus.None && this.lastMouseStatus != SimTouch.MouseStatus.StartDown)
					{
						this.DeltaPosition = Vector3.zero;
						this.Position = this.lastPosition;
						return this.lastMouseStatus;
					}
					flag = false;
					if (UnityEngine.Input.GetMouseButtonDown(0))
					{
						return SimTouch.MouseStatus.StartDown;
					}
					if (UnityEngine.Input.GetMouseButton(0))
					{
						return SimTouch.MouseStatus.Down;
					}
				}
			}
			else if (this.FingerId > 0)
			{
				return SimTouch.MouseStatus.None;
			}
			if (flag)
			{
				if (UnityEngine.Input.GetMouseButtonDown(0))
				{
					return SimTouch.MouseStatus.StartDown;
				}
				if (UnityEngine.Input.GetMouseButton(0))
				{
					return SimTouch.MouseStatus.Down;
				}
				if (UnityEngine.Input.GetMouseButtonUp(0))
				{
					return SimTouch.MouseStatus.Up;
				}
			}
			return SimTouch.MouseStatus.None;
		}

		private void UpdateTouchSim()
		{
			this.DeltaTime = Time.deltaTime;
			this.Position = UnityEngine.Input.mousePosition;
			this.DeltaPosition = new Vector2(UnityEngine.Input.mousePosition.x, UnityEngine.Input.mousePosition.y) - this.lastPosition;
			SimTouch.MouseStatus mouseStatus = this.GetMouseStatus();
			this.lastMouseStatus = mouseStatus;
			switch (mouseStatus)
			{
			case SimTouch.MouseStatus.StartDown:
				this.Status = TouchStatus.Start;
				this.StartPosition = UnityEngine.Input.mousePosition;
				this.DeltaPosition = Vector2.zero;
				this.lastPosition = UnityEngine.Input.mousePosition;
				this.tapTimeout = this.TapTimeWindow;
				break;
			case SimTouch.MouseStatus.Down:
			{
				this.Status = TouchStatus.Stationary;
				float sqrMagnitude = this.DeltaPosition.sqrMagnitude;
				if (sqrMagnitude > this.MoveThreshold * this.MoveThreshold)
				{
					this.Status = TouchStatus.Moving;
					this.lastPosition = this.Position;
				}
				break;
			}
			case SimTouch.MouseStatus.Up:
				this.Status = TouchStatus.End;
				if (this.tapTimeout > 0f)
				{
					this.TapCount++;
				}
				break;
			case SimTouch.MouseStatus.None:
				if (this.Status != TouchStatus.Invalid && this.Status != TouchStatus.End)
				{
					this.Status = TouchStatus.End;
				}
				else if (this.Status == TouchStatus.End)
				{
					this.Status = TouchStatus.Invalid;
				}
				break;
			}
			this.tapTimeout -= Time.deltaTime;
			if (this.tapTimeout < 0f)
			{
				this.TapCount = 1;
			}
		}

		private bool GetTouchByID(int id, out Touch touch)
		{
			for (int i = 0; i < UnityEngine.Input.touchCount; i++)
			{
				Touch touch2 = UnityEngine.Input.GetTouch(i);
				if (touch2.fingerId == id)
				{
					touch = touch2;
					return true;
				}
			}
			touch = default(Touch);
			return false;
		}

		private void UpdateTouchInput()
		{
			Touch touch;
			if (this.GetTouchByID(this.FingerId, out touch))
			{
				this.DeltaPosition = touch.position - this.lastPosition;
				this.DeltaTime = touch.deltaTime;
				this.Position = touch.position;
				this.TapCount = touch.tapCount;
				switch (touch.phase)
				{
				case TouchPhase.Began:
					this.StartPosition = touch.position;
					this.DeltaPosition = Vector2.zero;
					this.lastPosition = this.Position;
					this.Status = TouchStatus.Start;
					this.PressTime = 0f;
					break;
				case TouchPhase.Moved:
				case TouchPhase.Stationary:
				{
					this.Status = TouchStatus.Stationary;
					float sqrMagnitude = this.DeltaPosition.sqrMagnitude;
					if (sqrMagnitude > this.MoveThreshold * this.MoveThreshold)
					{
						this.Status = TouchStatus.Moving;
						this.lastPosition = this.Position;
					}
					this.PressTime += Time.deltaTime;
					break;
				}
				case TouchPhase.Ended:
					this.Status = TouchStatus.End;
					break;
				default:
					this.Status = TouchStatus.Invalid;
					break;
				}
			}
			else if (this.Status != TouchStatus.Invalid && this.Status != TouchStatus.End)
			{
				this.Status = TouchStatus.End;
			}
			else if (this.Status == TouchStatus.End)
			{
				this.Status = TouchStatus.Invalid;
			}
		}

		public int FingerId;

		public Vector2 Position;

		public Vector2 StartPosition;

		public TouchStatus Status;

		public int TapCount;

		public float DeltaTime;

		public Vector2 DeltaPosition;

		public float TapTimeWindow = 0.3f;

		public float PressTime;

		public float MoveThreshold = 0.5f;

		private Vector2 lastPosition;

		private static KeyCode SimulationKey;

		private float tapTimeout;

		private SimTouch.MouseStatus lastMouseStatus;

		private enum MouseStatus
		{
			StartDown,
			Down,
			Up,
			None
		}
	}
}

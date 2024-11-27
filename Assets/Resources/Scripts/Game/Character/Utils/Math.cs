using System;
using UnityEngine;

namespace Game.Character.Utils
{
	public static class Math
	{
		public static float ExpN(float x, float n)
		{
			return Mathf.Exp(n * Mathf.Log(x));
		}

		public static float NormalizeValueInRange(float a, float min, float max)
		{
			if (max <= min)
			{
				return 1f;
			}
			return (Mathf.Clamp(a, min, max) - min) / (max - min);
		}

		public static float DeNormalizeValueInRange(float a, float min, float max)
		{
			return Mathf.Clamp(a, 0f, 1f) * (max - min) + min;
		}

		public static bool IsInRange(float a, float min, float max)
		{
			return a >= min && a <= max;
		}

		public static void ToPIRange(ref float angle)
		{
			if (angle < -3.14159274f)
			{
				angle += 6.28318548f;
			}
			if (angle > 3.14159274f)
			{
				angle -= 6.28318548f;
			}
		}

		public static float Sqr(float x)
		{
			return x * x;
		}

		public static void ToSpherical(Vector3 dir, out float rotX, out float rotZ)
		{
			float x = Mathf.Sqrt(Math.Sqr(dir.x) + Math.Sqr(dir.z));
			rotX = Mathf.Atan2(dir.x, dir.z);
			rotZ = Mathf.Atan2(dir.y, x);
		}

		public static void ToCartesian(float rotX, float rotZ, out Vector3 dir)
		{
			float y = Mathf.Sin(rotZ);
			float num = Mathf.Cos(rotZ);
			float num2 = Mathf.Sin(rotX);
			float num3 = Mathf.Cos(rotX);
			dir.x = num2 * num;
			dir.y = y;
			dir.z = num3 * num;
		}

		public static float ConvergeToValue(float target, float val, float timeRel, float speedPerSec)
		{
			if (val > target)
			{
				val -= timeRel * speedPerSec;
				if (val < target)
				{
					val = target;
				}
			}
			else
			{
				val += timeRel * speedPerSec;
				if (val > target)
				{
					val = target;
				}
			}
			return val;
		}

		public static void Swap<T>(ref T a, ref T b)
		{
			T t = a;
			a = b;
			b = t;
		}

		public static Vector3 VectorXZ(Vector3 v)
		{
			Vector3 vector = v;
			vector.y = 0f;
			return vector.normalized;
		}

		public static void CorrectRotationUp(ref Quaternion rot)
		{
			Vector3 forward = rot * Vector3.forward;
			rot = Quaternion.LookRotation(forward, Vector3.up);
		}

		public static float SmoothDamp(float current, float target, ref float currentVelocity, float smoothTime, float maxSpeed, float deltaTime)
		{
			smoothTime = Mathf.Max(0.0001f, smoothTime);
			float num = 2f / smoothTime;
			float num2 = num * deltaTime;
			float num3 = 1f / (1f + num2 + 0.48f * num2 * num2 + 0.235f * num2 * num2 * num2);
			float num4 = current - target;
			float num5 = target;
			float num6 = maxSpeed * smoothTime;
			num4 = Mathf.Clamp(num4, -num6, num6);
			target = current - num4;
			float num7 = (currentVelocity + num * num4) * deltaTime;
			currentVelocity = (currentVelocity - num * num7) * num3;
			float num8 = target + (num4 + num7) * num3;
			if (num5 - current > 0f == num8 > num5)
			{
				num8 = num5;
				currentVelocity = (num8 - num5) / deltaTime;
			}
			return num8;
		}

		public static Vector3[] GetNearPlanePoints(Camera camera)
		{
			Plane[] array = GeometryUtility.CalculateFrustumPlanes(camera);
			return new Vector3[]
			{
				Math.Intersection3Planes(array[1], array[2], array[4]),
				Math.Intersection3Planes(array[1], array[3], array[4]),
				Math.Intersection3Planes(array[0], array[3], array[4]),
				Math.Intersection3Planes(array[0], array[2], array[4])
			};
		}

		public static Vector3 Intersection3Planes(Plane p0, Plane p1, Plane p2)
		{
			float d = p0.normal[0] * p1.normal[1] * p2.normal[2] - p0.normal[0] * p1.normal[2] * p2.normal[1] + p1.normal[0] * p2.normal[1] * p0.normal[2] - p1.normal[0] * p0.normal[1] * p2.normal[2] + p2.normal[0] * p0.normal[1] * p1.normal[2] - p2.normal[0] * p1.normal[1] * p0.normal[2];
			return (Vector3.Cross(p1.normal, p2.normal) * -p0.distance + Vector3.Cross(p2.normal, p0.normal) * -p1.distance + Vector3.Cross(p0.normal, p1.normal) * -p2.distance) / d;
		}
	}
}

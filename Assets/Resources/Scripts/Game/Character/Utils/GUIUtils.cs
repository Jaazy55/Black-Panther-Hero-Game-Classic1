using System;
using UnityEngine;

namespace Game.Character.Utils
{
	internal static class GUIUtils
	{
		public static bool SliderEdit(string label, float min, float max, ref float value)
		{
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			float num = value;
			GUILayout.Label(label, new GUILayoutOption[]
			{
				GUILayout.Width(GUIUtils.labelMaxWidth)
			});
			value = GUILayout.HorizontalSlider(value, min, max, new GUILayoutOption[0]);
			GUILayout.EndHorizontal();
			value = Mathf.Clamp(value, min, max);
			return num != value;
		}

		public static bool SliderEdit(string label, int min, int max, ref int value)
		{
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			int num = value;
			GUILayout.Label(label, new GUILayoutOption[0]);
			value = (int)GUILayout.HorizontalSlider((float)value, (float)min, (float)max, new GUILayoutOption[0]);
			GUILayout.EndHorizontal();
			value = Mathf.Clamp(value, min, max);
			return num != value;
		}

		public static bool Toggle(string label, ref bool value)
		{
			bool flag = value;
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			GUILayout.Label(label, new GUILayoutOption[]
			{
				GUILayout.Width(GUIUtils.labelMaxWidth)
			});
			value = GUILayout.Toggle(value, string.Empty, new GUILayoutOption[0]);
			GUILayout.EndHorizontal();
			return flag != value;
		}

		public static bool Selection(string label, string[] labels, ref int index)
		{
			return false;
		}

		public static Enum EnumSelection(string label, Enum selected, ref bool changed)
		{
			return null;
		}

		public static bool String(string label, ref string input)
		{
			string b = input;
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			GUILayout.Label(label, new GUILayoutOption[]
			{
				GUILayout.Width(GUIUtils.labelMaxWidth)
			});
			input = GUILayout.TextField(input, new GUILayoutOption[0]);
			GUILayout.EndHorizontal();
			return input != b;
		}

		public static bool Vector2(string label, ref Vector2 input)
		{
			Vector2 rhs = input;
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			GUILayout.Label(label, new GUILayoutOption[]
			{
				GUILayout.Width(GUIUtils.labelMaxWidth)
			});
			string text = input.x.ToString();
			string value = GUILayout.TextField(text, new GUILayoutOption[0]);
			string text2 = input.y.ToString();
			string value2 = GUILayout.TextField(text2, new GUILayoutOption[0]);
			try
			{
				input.x = Convert.ToSingle(value);
				input.y = Convert.ToSingle(value2);
			}
			catch
			{
			}
			GUILayout.EndHorizontal();
			return input != rhs;
		}

		public static bool Vector3(string label, ref Vector3 input)
		{
			Vector3 rhs = input;
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			GUILayout.Label(label, new GUILayoutOption[]
			{
				GUILayout.Width(GUIUtils.labelMaxWidth)
			});
			string text = input.x.ToString();
			string value = GUILayout.TextField(text, new GUILayoutOption[0]);
			string text2 = input.y.ToString();
			string value2 = GUILayout.TextField(text2, new GUILayoutOption[0]);
			string text3 = input.z.ToString();
			string value3 = GUILayout.TextField(text3, new GUILayoutOption[0]);
			try
			{
				input.x = Convert.ToSingle(value);
				input.y = Convert.ToSingle(value2);
				input.z = Convert.ToSingle(value3);
			}
			catch
			{
			}
			GUILayout.EndHorizontal();
			return input != rhs;
		}

		public static void Separator(string label, float height)
		{
			GUILayout.Box(label, new GUILayoutOption[]
			{
				GUILayout.ExpandWidth(true),
				GUILayout.Height(height)
			});
		}

		private static float labelMaxWidth = 130f;
	}
}

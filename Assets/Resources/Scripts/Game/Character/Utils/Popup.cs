using System;
using UnityEngine;

namespace Game.Character.Utils
{
	public class Popup
	{
		public static bool List(Rect position, ref bool showList, ref int listEntry, GUIContent buttonContent, object[] list, GUIStyle listStyle, Popup.ListCallBack callBack)
		{
			return Popup.List(position, ref showList, ref listEntry, buttonContent, list, "button", "box", listStyle, callBack);
		}

		public static bool List(Rect position, ref bool showList, ref int listEntry, GUIContent buttonContent, object[] list, GUIStyle buttonStyle, GUIStyle boxStyle, GUIStyle listStyle, Popup.ListCallBack callBack)
		{
			int controlID = GUIUtility.GetControlID(Popup.popupListHash, FocusType.Passive);
			bool flag = false;
			EventType typeForControl = Event.current.GetTypeForControl(controlID);
			if (typeForControl != EventType.MouseDown)
			{
				if (typeForControl == EventType.MouseUp)
				{
					if (showList)
					{
						flag = true;
						callBack();
					}
				}
			}
			else if (position.Contains(Event.current.mousePosition))
			{
				GUIUtility.hotControl = controlID;
				showList = true;
			}
			GUI.Label(position, buttonContent, buttonStyle);
			if (showList)
			{
				string[] array = new string[list.Length];
				for (int i = 0; i < list.Length; i++)
				{
					array[i] = list[i].ToString();
				}
				Rect position2 = new Rect(position.x, position.y, position.width, (float)(list.Length * 20));
				GUI.Box(position2, string.Empty, boxStyle);
				listEntry = GUI.SelectionGrid(position2, listEntry, array, 1, listStyle);
			}
			if (flag)
			{
				showList = false;
			}
			return flag;
		}

		private static int popupListHash = "PopupList".GetHashCode();

		public delegate void ListCallBack();
	}
}

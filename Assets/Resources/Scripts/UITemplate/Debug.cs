using System;
using UnityEngine.UI;

namespace UITemplate
{
	public class Debug
	{
		public static void Log(string message)
		{
			//UnityEngine.Debug.InitLog();
		}

		private static void LogMessage(string message)
		{
			if (Debug.logText != null)
			{
			//	UnityEngine.Debug.logText.text = UnityEngine.Debug.logText.text + "\n" + message;
			}
		}

		public static void LogFormat(string format, params object[] args)
		{
			//UnityEngine.Debug.InitLog();
		}

		private static void InitLog()
		{
		}

		private static Text logText;
	}
}

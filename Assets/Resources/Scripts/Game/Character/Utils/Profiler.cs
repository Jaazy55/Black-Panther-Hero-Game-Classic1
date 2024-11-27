using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Game.Character.Utils
{
	public static class Profiler
	{
		public static void Start(string key)
		{
			Stopwatch stopwatch = null;
			if (Profiler.timeSegments.TryGetValue(key, out stopwatch))
			{
				stopwatch.Reset();
				stopwatch.Start();
			}
			else
			{
				stopwatch = new Stopwatch();
				stopwatch.Start();
				Profiler.timeSegments.Add(key, stopwatch);
			}
		}

		public static void Stop(string key)
		{
			Profiler.timeSegments[key].Stop();
		}

		public static string[] GetResults()
		{
			string[] array = new string[Profiler.timeSegments.Count];
			int num = 0;
			foreach (KeyValuePair<string, Stopwatch> keyValuePair in Profiler.timeSegments)
			{
				long elapsedMilliseconds = keyValuePair.Value.ElapsedMilliseconds;
				long num2 = keyValuePair.Value.ElapsedTicks / (Stopwatch.Frequency / 1000000L);
				array[num++] = string.Concat(new object[]
				{
					keyValuePair.Key,
					" ",
					elapsedMilliseconds,
					" [ms] | ",
					num2,
					" [us]"
				});
			}
			return array;
		}

		private static readonly Dictionary<string, Stopwatch> timeSegments = new Dictionary<string, Stopwatch>();
	}
}

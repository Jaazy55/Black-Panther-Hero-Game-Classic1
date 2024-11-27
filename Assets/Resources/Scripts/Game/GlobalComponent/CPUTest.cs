using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UnityEngine;

namespace Game.GlobalComponent
{
	public class CPUTest : PerformanceTest
	{
		public override void Init()
		{
			this.absLeadTime = 0.0;
			this.maxLeadTime = 0.0;
			this.minLeadTime = 10000.0;
			this.t = 0.0;
			this.s = 0f;
			this.timer = this.DetectingTime;
			this.isInit = true;
		}

		public void FixedUpdate()
		{
			/*if (this.IsEnd)
			{
				return;
			}
			if (!this.isInit)
			{
				return;
			}
			this.time = DateTime.Now;
			for (int i = 0; i < (int)(this.count * 0.27f); i++)
			{
				string sequence = Console.In.ReadToEnd();
				int length = sequence.Length;
				sequence = Regex.Replace(sequence, ">.*\n|\n", string.Empty);
				int length2 = sequence.Length;
				Task<int> task = Task.Run<int>(delegate()
				{
					string text = Regex.Replace(sequence, "tHa[Nt]", "<4>");
					text = Regex.Replace(text, "aND|caN|Ha[DS]|WaS", "<3>");
					text = Regex.Replace(text, "a[NSt]|BY", "<2>");
					text = Regex.Replace(text, "<[^>]*>", "|");
					text = Regex.Replace(text, "\\|[^|][^|]*\\|", "-");
					return text.Length;
				});
				string[] array = Variants.variantsCopy();
			}
			this.t = DateTime.Now.Subtract(this.time).TotalMilliseconds;
			this.results.Add(this.t);
			if (this.absLeadTime <= 0.0)
			{
				this.absLeadTime = this.t;
			}
			if (this.t > this.maxLeadTime)
			{
				this.maxLeadTime = this.t;
			}
			if (this.t < this.minLeadTime)
			{
				this.minLeadTime = this.t;
			}
			this.absLeadTime = this.results.Sum() / (double)this.results.Count;
			this.Timer();*/
		}

		private void Timer()
		{
			this.timer -= Time.deltaTime;
			if (this.timer <= 0f)
			{
				this.isInit = false;
				this.EndTesting();
			}
		}

		public override void EndTesting()
		{
			this.Result = (float)(100.0 - 2.0 * this.absLeadTime);
			MonoBehaviour.print(this.Result);
			base.CallEndTestingEvent(this.Result, this);
			this.t = (double)this.s;
		}

		private static IEnumerable<string> Chunks(string sequence)
		{
			int numChunks = Environment.ProcessorCount;
			int start = 0;
			int chunkSize = sequence.Length / numChunks;
			while (--numChunks >= 0)
			{
				if (numChunks > 0)
				{
					yield return sequence.Substring(start, chunkSize);
				}
				else
				{
					yield return sequence.Substring(start);
				}
				start += chunkSize;
			}
			yield break;
		}

		private static int[] CountRegexes(string chunk)
		{
			int[] array = new int[9];
			string[] array2 = Variants.variantsCopy();
			for (int i = 0; i < 9; i++)
			{
				Match match = Regex.Match(chunk, array2[i]);
				while (match.Success)
				{
					array[i]++;
					match = match.NextMatch();
				}
			}
			return array;
		}

		private const float point = 0.27f;

		private const float MaxLeadTime = 50f;

		private float s;

		public float count = 100f;

		private DateTime time;

		private double t;

		private double absLeadTime;

		private double minLeadTime;

		private double maxLeadTime;

		private bool isInit;

		private float timer;

		private List<double> results = new List<double>();
	}
}

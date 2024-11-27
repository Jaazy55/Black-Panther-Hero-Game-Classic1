using System;

namespace Game.GlobalComponent
{
	public class Variants
	{
		public static string[] variantsCopy()
		{
			return new string[]
			{
				"agggtaaa|tttaccct",
				"[cgt]gggtaaa|tttaccc[acg]",
				"a[act]ggtaaa|tttacc[agt]t",
				"ag[act]gtaaa|tttac[agt]ct",
				"agg[act]taaa|ttta[agt]cct",
				"aggg[acg]aaa|ttt[cgt]ccct",
				"agggt[cgt]aa|tt[acg]accct",
				"agggta[cgt]a|t[acg]taccct",
				"agggtaa[cgt]|[acg]ttaccct"
			};
		}
	}
}

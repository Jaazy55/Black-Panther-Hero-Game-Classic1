using System;

namespace Naxeex.GameModes
{
	[Serializable]
	public struct ResultLine
	{
		public ResultLine(string name, float result)
		{
			this.Name = name;
			this.Result = result;
		}

		public string Name;

		public float Result;
	}
}

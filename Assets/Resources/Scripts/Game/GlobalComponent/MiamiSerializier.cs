using System;
using FullSerializer;

namespace Game.GlobalComponent
{
	public class MiamiSerializier
	{
		public static string JSONSerialize(object obj)
		{
			fsSerializer fsSerializer = new fsSerializer();
			fsData fsData;
			fsSerializer.TrySerialize<object>(obj, out fsData);
			return fsData.ToString();
		}

		public static object JSONDeserialize(string str)
		{
			return MiamiSerializier.JSONDeserialize<object>(str);
		}

		public static T JSONDeserialize<T>(string str)
		{
			fsData data = fsJsonParser.Parse(str);
			fsSerializer fsSerializer = new fsSerializer();
			T result = default(T);
			fsSerializer.TryDeserialize<T>(data, ref result);
			return result;
		}
	}
}

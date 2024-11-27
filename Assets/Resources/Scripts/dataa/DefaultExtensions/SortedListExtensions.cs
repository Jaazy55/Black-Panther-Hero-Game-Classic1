using System;
using System.Collections.Generic;

namespace Naxeex.DefaultExtensions
{
	public static class SortedListExtensions
	{
		public static int BinarySearch<TKey, TValue>(this SortedList<TKey, TValue> sortedList, TKey keyToFind, IComparer<TKey> comparer = null)
		{
			IList<TKey> keys = sortedList.Keys;
			TKey[] array = new TKey[keys.Count];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = keys[i];
			}
			if (comparer == null)
			{
				comparer = Comparer<TKey>.Default;
			}
			return Array.BinarySearch<TKey>(array, keyToFind, comparer);
		}

		public static IEnumerable<TKey> GetKeyRangeBetween<TKey, TValue>(this SortedList<TKey, TValue> sortedList, TKey low, TKey high, IComparer<TKey> comparer = null)
		{
			int lowIndex = sortedList.BinarySearch(low, comparer);
			if (lowIndex < 0)
			{
				lowIndex = ~lowIndex;
			}
			int highIndex = sortedList.BinarySearch(high, comparer);
			if (highIndex < 0)
			{
				highIndex = ~highIndex - 1;
			}
			IList<TKey> keys = sortedList.Keys;
			for (int i = lowIndex; i < highIndex; i++)
			{
				yield return keys[i];
			}
			yield break;
		}
	}
}

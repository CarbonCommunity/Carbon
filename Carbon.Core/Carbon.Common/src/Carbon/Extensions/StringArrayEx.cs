/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * All rights reserved.
 *
 */

using Facepunch;

namespace Carbon.Extensions;

public static class StringArrayEx
{
	/// <summary>
	/// Combines a string array into a string with separator contents between them.
	/// </summary>
	/// <param name="array">Targeted string array.</param>
	/// <param name="separator">String array to string with this separation between each word.</param>
	/// <param name="lastSeparator">String array to string with this ending separation between the last word.</param>
	/// <returns></returns>
	public static string ToString(this IEnumerable<string> array, string separator, string lastSeparator = null)
	{
		if (string.IsNullOrEmpty(lastSeparator)) lastSeparator = separator;

		var count = array.Count();
		if (count == 0) { return string.Empty; }
		if (count == 1) { return array.First(); }

		var str = string.Join(separator, array);
		str += string.Format("{0}{1}", lastSeparator, array.ElementAt(count - 1));

		return str;
	}

	/// <summary>
	/// Combines a string array into a string with separator contents between them.
	/// </summary>
	/// <param name="array">Targeted string array.</param>
	/// <param name="startIndex">Gets the array starting from this index.</param>
	/// <param name="separator">String array to string with this separation between each word.</param>
	/// <param name="throwError">Return an error if errored about start index over it's length, if the case.</param>
	/// <returns></returns>
	public static string ToString(this IEnumerable<string> array, int startIndex, string separator = " ", bool throwError = false)
	{
		var count = array.Count();
		if (count == 0) { return string.Empty; }
		if (count == 1) { return array.First(); }

		if (startIndex > count)
		{
			return throwError ? string.Format("ERROR! The start index ({0}) is over the length of the arguments ({1}).", startIndex, count) : null;
		}

		return string.Join(separator, array, startIndex, count - startIndex);
	}

	/// <summary>
	/// Combines a string array into a string with separator contents between them.
	/// </summary>
	/// <param name="array">Targeted string array.</param>
	/// <param name="startIndex">Gets the array starting from this index.</param>
	/// <param name="separator">String array to string with this separation between each word.</param>
	/// <param name="lastSeparator">String array to string with this ending separation between the last word.</param>
	/// <param name="throwError">Return an error if errored about start index over it's length, if the case.</param>
	/// <returns></returns>
	public static string ToString(this IEnumerable<string> array, int startIndex, string separator, string lastSeparator = null, bool throwError = false)
	{
		if (lastSeparator == null) lastSeparator = separator;

		var count = array.Count();
		if (count == 0) { return string.Empty; }
		if (count == 1) { return array.First(); }

		if (startIndex > count)
		{
			return throwError ? string.Format("ERROR! The start index ({0}) is over the length of the arguments ({1}).", startIndex, count) : null;
		}

		var str = string.Join(separator, array, startIndex, count - startIndex) + string.Format("{0}{1}", lastSeparator, array.ElementAt(count - 1));

		return str;
	}

	/// <summary>
	/// Cuts text into pieces at chunkSize size and returns them each.
	/// </summary>
	/// <param name="text">Targeted string.</param>
	/// <param name="chunkSize">Chunk size.</param>
	/// <returns></returns>
	public static string[] Split(this string text, int chunkSize, bool includeLeftovers = true)
	{
		var splits = Enumerable.Range(0, text.Length / chunkSize)
			.Select(i => text.Substring(i * chunkSize, chunkSize));

		var remainingText = text.Replace(splits.ToString(""), "");
		var splitsList = Pool.GetList<string>();
		splitsList.AddRange(splits);

		if (includeLeftovers && !string.IsNullOrEmpty(remainingText))
		{
			splitsList.Add(remainingText);
		}

		var result = splitsList.ToArray();
		Pool.FreeList(ref splitsList);

		return result;
	}
}

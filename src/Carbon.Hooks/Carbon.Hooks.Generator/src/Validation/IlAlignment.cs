using Carbon.Validation.Metadata;

namespace Carbon.Validation;

/// <summary>
///     Aligns two builds of the same method body so instruction indexes recorded against the old
///     build (injection indexes, branch targets) can be re-anchored onto the new build. Uses a
///     patience-diff over position-independent instruction keys: unique keys shared by both sides
///     anchor the alignment, equal runs are matched greedily between anchors.
/// </summary>
internal sealed class IlAlignment
{
	private readonly IlInstruction[] _old;
	private readonly IlInstruction[] _new;
	private readonly int[] _map;

	public IlAlignment(IlInstruction[] oldInstructions, IlInstruction[] newInstructions)
	{
		_old = oldInstructions;
		_new = newInstructions;
		_map = new int[oldInstructions.Length];
		Array.Fill(_map, -1);
		Align(0, oldInstructions.Length, 0, newInstructions.Length);
	}

	/// <summary>Maps an old instruction index to its new index, or -1 when it has no counterpart.</summary>
	public int Map(int oldIndex)
	{
		return oldIndex >= 0 && oldIndex < _map.Length ? _map[oldIndex] : -1;
	}

	/// <summary>
	///     Best-effort proposal for where a deleted instruction's position lands in the new body:
	///     the mapping of the next surviving instruction, so an injection lands ahead of the same
	///     surviving code.
	/// </summary>
	public int ProposeNearby(int oldIndex)
	{
		for (var i = oldIndex + 1; i < _map.Length; i++)
		{
			if (_map[i] >= 0)
			{
				return _map[i];
			}
		}

		for (var i = oldIndex - 1; i >= 0; i--)
		{
			if (_map[i] >= 0)
			{
				return _map[i] + 1;
			}
		}

		return -1;
	}

	/// <summary>
	///     How trustworthy the mapping of a single index is, judged by how well the surrounding
	///     instructions agree.
	/// </summary>
	public AlignmentConfidence GetConfidence(int oldIndex, int newIndex)
	{
		if (newIndex < 0)
		{
			return AlignmentConfidence.None;
		}

		const int window = 3;
		var compared = 0;
		var matched = 0;
		for (var delta = -window; delta <= window; delta++)
		{
			var oldAt = oldIndex + delta;
			var newAt = newIndex + delta;
			var oldInside = oldAt >= 0 && oldAt < _old.Length;
			var newInside = newAt >= 0 && newAt < _new.Length;
			if (!oldInside && !newInside)
			{
				continue;
			}

			compared++;
			if (oldInside && newInside && _old[oldAt].AlignmentKey == _new[newAt].AlignmentKey)
			{
				matched++;
			}
		}

		if (compared == 0 || matched == compared)
		{
			return AlignmentConfidence.High;
		}

		return matched * 10 >= compared * 7 ? AlignmentConfidence.Medium : AlignmentConfidence.Low;
	}

	/// <summary>
	///     Dice similarity over instruction-key bigrams; used to spot renamed methods by their bodies.
	/// </summary>
	public static double BodySimilarity(IlInstruction[] left, IlInstruction[] right)
	{
		if (left.Length == 0 || right.Length == 0)
		{
			return left.Length == right.Length ? 1 : 0;
		}

		if (left.Length == 1 || right.Length == 1)
		{
			return left[0].AlignmentKey == right[0].AlignmentKey && left.Length == right.Length ? 1 : 0;
		}

		var bigrams = new Dictionary<(string, string), int>();
		for (var i = 0; i < left.Length - 1; i++)
		{
			var key = (left[i].AlignmentKey, left[i + 1].AlignmentKey);
			bigrams[key] = bigrams.TryGetValue(key, out var count) ? count + 1 : 1;
		}

		var common = 0;
		for (var i = 0; i < right.Length - 1; i++)
		{
			var key = (right[i].AlignmentKey, right[i + 1].AlignmentKey);
			if (bigrams.TryGetValue(key, out var count) && count > 0)
			{
				bigrams[key] = count - 1;
				common++;
			}
		}

		return 2.0 * common / (left.Length - 1 + right.Length - 1);
	}

	private void Align(int oldStart, int oldEnd, int newStart, int newEnd)
	{
		while (true)
		{
			// Fast path: strip the common prefix and suffix first.
			while (oldStart < oldEnd && newStart < newEnd && _old[oldStart].AlignmentKey == _new[newStart].AlignmentKey)
			{
				_map[oldStart++] = newStart++;
			}

			while (oldEnd > oldStart && newEnd > newStart && _old[oldEnd - 1].AlignmentKey == _new[newEnd - 1].AlignmentKey)
			{
				_map[--oldEnd] = --newEnd;
			}

			if (oldStart >= oldEnd || newStart >= newEnd)
			{
				return;
			}

			// Patience step: anchor on keys unique within both windows.
			var oldUnique = CollectUnique(_old, oldStart, oldEnd);
			var newUnique = CollectUnique(_new, newStart, newEnd);
			var anchors = new List<(int OldIndex, int NewIndex)>();
			foreach (var (key, oldIndex) in oldUnique)
			{
				if (newUnique.TryGetValue(key, out var newIndex))
				{
					anchors.Add((oldIndex, newIndex));
				}
			}

			if (anchors.Count == 0)
			{
				return;
			}

			anchors.Sort((a, b) => a.OldIndex.CompareTo(b.OldIndex));
			var chain = LongestIncreasingChain(anchors);
			if (chain.Count == 0)
			{
				return;
			}

			var previousOld = oldStart;
			var previousNew = newStart;
			foreach (var (oldIndex, newIndex) in chain)
			{
				Align(previousOld, oldIndex, previousNew, newIndex);
				_map[oldIndex] = newIndex;
				previousOld = oldIndex + 1;
				previousNew = newIndex + 1;
			}

			oldStart = previousOld;
			newStart = previousNew;
		}
	}

	private static Dictionary<string, int> CollectUnique(IlInstruction[] instructions, int start, int end)
	{
		var counts = new Dictionary<string, int>(end - start);
		var positions = new Dictionary<string, int>(end - start);
		for (var i = start; i < end; i++)
		{
			var key = instructions[i].AlignmentKey;
			counts[key] = counts.TryGetValue(key, out var count) ? count + 1 : 1;
			positions[key] = i;
		}

		var unique = new Dictionary<string, int>();
		foreach (var (key, count) in counts)
		{
			if (count == 1)
			{
				unique.Add(key, positions[key]);
			}
		}

		return unique;
	}

	private static List<(int OldIndex, int NewIndex)> LongestIncreasingChain(List<(int OldIndex, int NewIndex)> anchors)
	{
		// Anchors are sorted by OldIndex; find the longest chain with strictly increasing NewIndex.
		var tailIndexes = new List<int>();
		var previousAnchor = new int[anchors.Count];
		Array.Fill(previousAnchor, -1);

		for (var i = 0; i < anchors.Count; i++)
		{
			var value = anchors[i].NewIndex;
			var low = 0;
			var high = tailIndexes.Count;
			while (low < high)
			{
				var middle = (low + high) / 2;
				if (anchors[tailIndexes[middle]].NewIndex < value)
				{
					low = middle + 1;
				}
				else
				{
					high = middle;
				}
			}

			if (low > 0)
			{
				previousAnchor[i] = tailIndexes[low - 1];
			}

			if (low == tailIndexes.Count)
			{
				tailIndexes.Add(i);
			}
			else
			{
				tailIndexes[low] = i;
			}
		}

		var chain = new List<(int OldIndex, int NewIndex)>();
		if (tailIndexes.Count == 0)
		{
			return chain;
		}

		for (var at = tailIndexes[^1]; at >= 0; at = previousAnchor[at])
		{
			chain.Add(anchors[at]);
		}

		chain.Reverse();
		return chain;
	}
}

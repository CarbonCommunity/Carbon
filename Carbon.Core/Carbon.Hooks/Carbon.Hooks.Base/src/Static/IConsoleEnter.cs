using System.Collections.Generic;
using API.Hooks;
using Windows;
using Patch = API.Hooks.Patch;

/*
 *
 * Copyright (c) 2023 Carbon Community 
 * Copyright (c) 2023 turner
 * All rights reserved.
 *
 */

namespace Carbon.Hooks;

#if WIN

public partial class Category_Static
{
	public partial class Static_Console
	{
		[HookAttribute.Patch("IConsoleEnter", "IConsoleEnter", typeof(ConsoleInput), "OnEnter", new System.Type[] { })]
		[HookAttribute.Identifier("7aec275cf9ec428baa152b4108fcd390")]
		[HookAttribute.Options(HookFlags.Static | HookFlags.Hidden | HookFlags.IgnoreChecksum)]
		public class Static_Console_Enter_7aec275cf9ec428baa152b4108fcd390 : Patch
		{
			internal static LinkedList<string> history = new();
			internal static int count = 0;
			internal static LinkedListNode<string> lastSelected = null;

			public static void Add(string value)
			{
				if (value.Length == 0) return;

				var first = history.First;

				if ((first?.Value) == value)
				{
					lastSelected = history.First;
					return;
				}

				if (count > 200)
				{
					history.AddFirst(value);
					history.RemoveLast();
				}
				else
				{
					history.AddFirst(value);
					count++;
				}

				lastSelected = history.First;
			}

			public static string GetUp()
			{
				if (count == 0) return string.Empty;

				var value = lastSelected.Value;

				if (lastSelected.Next != null)
				{
					lastSelected = lastSelected.Next;
				}

				return value;
			}

			public static string GetDown()
			{
				if (count == 0) return string.Empty;

				var value = lastSelected.Value;

				if (lastSelected.Previous != null)
				{
					lastSelected = lastSelected.Previous;
				}

				return value == lastSelected.Value ? string.Empty : lastSelected.Value;
			}

			private static void Prefix(ConsoleInput __instance)
			{
				Add(__instance.inputString);
			}
		}
	}
}

#endif

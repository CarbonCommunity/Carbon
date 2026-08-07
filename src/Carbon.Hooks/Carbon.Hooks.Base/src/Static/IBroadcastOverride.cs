using System.Xml.Linq;
using API.Hooks;
using Carbon.Core;
using Facepunch.Math;
using UnityEngine;

namespace Carbon.Hooks;

#pragma warning disable IDE0051

public partial class Category_Static
{
#if !MINIMAL
	public partial class Static_Debug
	{
		[HookAttribute.Patch("IBroadcastOverride", "IBroadcastOverride", typeof(ConVar.Chat), "Broadcast", new System.Type[] { typeof(string), typeof(string), typeof(string), typeof(ulong) })]
		[HookAttribute.Options(HookFlags.Static | HookFlags.Hidden | HookFlags.IgnoreChecksum)]
		public class IBroadcastOverride : Patch
		{
			public static bool Prefix(string message, ref string username, ref string color, ref ulong userid)
			{
				// OnServerMessage
				if(HookCaller.CallStaticHook(3155060134, message, username, color, userid) != null)
				{
					return false;
				}

				const string defaultValue = "-1";

				if (userid == 0ul)
				{
					if (Community.Runtime.Core.DefaultServerChatName != defaultValue)
					{
						username = Community.Runtime.Core.DefaultServerChatName;
					}
					if (Community.Runtime.Core.DefaultServerChatColor != defaultValue)
					{
						color = Community.Runtime.Core.DefaultServerChatColor;
					}
					if (Community.Runtime.Core.DefaultServerChatId != -1)
					{
						userid = (ulong)Community.Runtime.Core.DefaultServerChatId;
					}
				}

				var text = username.EscapeRichText();
				ConsoleNetwork.BroadcastToAllClients("chat.add", 2, userid, $"<color={color}>{text}</color> {message}");
				ConVar.Chat.ChatEntry ce = default;
				ce.Channel = ConVar.Chat.ChatChannel.Server;
				ce.Message = message;
				ce.UserId = userid.ToString();
				ce.Username = username;
				ce.Color = color;
				ce.Time = Epoch.Current;
				ConVar.Chat.Record(ce);
				return false;
			}
		}

		[HookAttribute.Patch("INoGiveNotices", "INoGiveNotices", typeof(ConVar.Chat), "BroadcastPlayerAction", new System.Type[] { typeof(BasePlayer), typeof(string) })]
		[HookAttribute.Options(HookFlags.Static | HookFlags.Hidden | HookFlags.IgnoreChecksum)]
		public class INoGiveNotices : Patch
		{
			public static bool Prefix(BasePlayer subject, string action)
			{
				if (Community.Runtime.Core.NoGiveNoticesCache && (action.Contains("give") || action.Contains("gave")))
				{
					return false;
				}
				return true;
			}
		}

		[HookAttribute.Patch("INoGiveNotices2", "INoGiveNotices2", typeof(ConVar.Chat), "BroadcastPlayerAction", new System.Type[] { typeof(BasePlayer), typeof(string), typeof(BasePlayer), typeof(string) })]
		[HookAttribute.Options(HookFlags.Static | HookFlags.Hidden | HookFlags.IgnoreChecksum)]
		public class INoGiveNotices2 : Patch
		{
			public static bool Prefix(BasePlayer subjectA, string middle, BasePlayer subjectB, string suffix)
			{
				if (Community.Runtime.Core.NoGiveNoticesCache && (middle.Contains("give") || middle.Contains("gave")))
				{
					return false;
				}
				return true;
			}
		}
	}
#endif
}

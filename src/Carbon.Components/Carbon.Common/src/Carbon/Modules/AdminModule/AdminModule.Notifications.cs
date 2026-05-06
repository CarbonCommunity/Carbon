#if !MINIMAL

namespace Carbon.Modules;

public partial class AdminModule
{
	public class Notifications
	{
		public static NotificationQueue Queue = new();

		public static List<Notification> GetOrCreateQueue(BasePlayer player)
		{
			if (!Queue.TryGetValue(player.userID, out var list))
			{
				Queue[player.userID] = list = new();
			}

			return list;
		}

		public static void Redraw(BasePlayer player)
		{
			var queue = GetOrCreateQueue(player);

			using var cui = new CUI(Singleton.Handler);

			var container = cui.CreateContainer("adminmodulenotifs", xMin: 0.95f, xMax: 0.95f, yMin: 0.95f, yMax: 0.95f, destroyUi: "adminmodulenotifs");

			for (int i = 0; i < queue.Count; i++)
			{
				var value = queue[i];
				var panel = cui.CreatePanel(container, "adminmodulenotifs", Cache.CUI.BlankColor, OxMin: -2050, OyMin: -20 - (20 * i), OyMax: -(20 * i));
				cui.CreateText(container, panel, "1 1 1 1", value.Message, 9, align: TextAnchor.MiddleRight);
				cui.CreatePanel(container, panel, "1 0 0 0.2", xMin: 1f, yMax: 0.1f, OxMin: -40);
			}

			cui.Send(container, player);
		}

		public static void Add(BasePlayer player, string message, float duration = 2f)
		{
			var queue = GetOrCreateQueue(player);

			Community.Runtime.Core.timer.In(duration, () =>
			{
				queue.RemoveAt(queue.Count - 1);
				Redraw(player);
			});

			Notification notification = default;
			notification.Message = message;
			notification.Duration = duration;
			queue.Insert(0, notification);

			Redraw(player);
		}

		public class NotificationQueue : Dictionary<ulong, List<Notification>>;

		public struct Notification
		{
			public string Message;
			public float Duration;
		}
	}
}

#endif

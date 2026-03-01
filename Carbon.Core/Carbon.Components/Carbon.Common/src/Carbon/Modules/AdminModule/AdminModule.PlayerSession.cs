using Facepunch;

namespace Carbon.Modules;

public partial class AdminModule
{
	internal Dictionary<BasePlayer, PlayerSession> PlayerSessions = new();

	public PlayerSession GetPlayerSession(BasePlayer player)
	{
		if (PlayerSessions.TryGetValue(player, out PlayerSession adminPlayer)) return adminPlayer;

		adminPlayer = new PlayerSession(player);
		PlayerSessions.Add(player, adminPlayer);
		return adminPlayer;
	}

	public class PlayerSession : IDisposable
	{
		public static PlayerSession Blank { get; } = new PlayerSession(null);

		public BasePlayer Player;
		public bool IsInMenu;
		public Dictionary<int, Page> ColumnPages = new();
		public Dictionary<string, object> LocalStorage = new();

		public Tab SelectedTab;
		public int TabSkip;
		public int LastPressedColumn;
		public int LastPressedRow;

		public Tab.Option Tooltip;
		public Tab.Option Input;
		public Tab.Option PreviousInput;

		internal Tab.OptionDropdown _selectedDropdown;
		internal Page _selectedDropdownPage = new();

		public PlayerSession(BasePlayer player)
		{
			Player = player;
		}

		public void SetPage(int column, int page)
		{
			if (ColumnPages.TryGetValue(column, out var pageInstance))
			{
				pageInstance.CurrentPage = page;
				pageInstance.Check();
			}
		}
		public T GetStorage<T>(Tab tab, string id, T @default = default)
		{
			try
			{
				var mainId = id;
				id = $"{tab?.Id}_{id}";

				if (LocalStorage.TryGetValue(id, out var storage)) return (T)storage;

				return SetStorage(tab, mainId, (T)@default);
			}
			catch (Exception ex) { Logger.Warn($"Failed GetStorage<{typeof(T).Name}>({tab?.Id}, {id}): {ex.Message}"); }

			return (T)default;
		}
		public T SetStorage<T>(Tab tab, string id, T value)
		{
			id = $"{tab?.Id}_{id}";

			LocalStorage[id] = value;
			return value;
		}
		public T SetDefaultStorage<T>(Tab tab, string id, T value)
		{
			if (Player == null) return default;

			return GetStorage(tab, id, value);
		}
		public void ClearStorage(Tab tab, string id)
		{
			id = $"{tab?.Id}_{id}";

			LocalStorage[id] = null;
		}
		public bool HasStorage(Tab tab, string id)
		{
			id = $"{tab?.Id}_{id}";

			return LocalStorage.ContainsKey(id);
		}
		public void Clear()
		{
			foreach (var page in ColumnPages)
			{
				var value = ColumnPages[page.Key];
				Facepunch.Pool.Free(ref value);
			}

			ColumnPages.Clear();
			// LocalStorage.Clear();

			_selectedDropdown = null;
			_selectedDropdownPage.CurrentPage = 0;
		}
		public Page GetOrCreatePage(int column)
		{
			if (ColumnPages.TryGetValue(column, out var page)) return page;
			else
			{
				ColumnPages[column] = page = Pool.Get<Page>();
				return page;
			}
		}

		public void Dispose()
		{
			Clear();
		}

		public class Page : Pool.IPooled
		{
			public int CurrentPage { get; set; }
			public int TotalPages { get; set; }

			public void Check()
			{
				if (CurrentPage < 0)
				{
					CurrentPage = TotalPages;
				}
				else if (CurrentPage > TotalPages)
				{
					CurrentPage = 0;
				}
			}

			public void EnterPool()
			{
			}

			public void LeavePool()
			{
				CurrentPage = 0;
				TotalPages = 0;
			}
		}
	}
}

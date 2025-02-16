using Facepunch;
using Newtonsoft.Json;
using ProtoBuf;
using Formatting = Newtonsoft.Json.Formatting;

namespace Oxide.Game.Rust.Cui;

public static class CuiHelper
{
	public static Dictionary<BasePlayer, HashSet<string>> ActivePanels { get; } = new();

	private static JsonSerializerSettings _cuiSettings = new()
	{
		DefaultValueHandling = DefaultValueHandling.Ignore
	};

	public static HashSet<string> GetActivePanelList(BasePlayer player)
	{
		if (!ActivePanels.TryGetValue(player, out var panels))
		{
			ActivePanels[player] = panels = new();
		}

		return panels;
	}

	public static int DestroyActivePanelList(BasePlayer player, string[] except = null)
	{
		var temp = Pool.Get<List<string>>();
		temp.AddRange(GetActivePanelList(player));

		var count = 0;

		foreach (var element in temp.Where(x => except == null || except.Length == 0 || !except.Any(y => x.StartsWith(y))))
		{
			DestroyUi(player, element);
			count++;
		}

		Pool.FreeUnmanaged(ref temp);
		return count;
	}

	public static string ToJson(List<CuiElement> elements, bool format = false)
	{
		return JsonConvert.SerializeObject(elements, format ? Formatting.Indented : Formatting.None, _cuiSettings).Replace("\\n", "\n");
	}

	public static string ToJson(CuiElement element, bool format = false)
	{
		return JsonConvert.SerializeObject(element, format ? Formatting.Indented : Formatting.None, _cuiSettings).Replace("\\n", "\n");
	}

	public static List<CuiElement> FromJson(string json) => JsonConvert.DeserializeObject<List<CuiElement>>(json);

	public static string GetGuid() => Guid.NewGuid().ToString("N");

	public static bool AddUi(BasePlayer player, string json)
	{
		if (player == null || player.net == null)
		{
			return false;
		}

		// CanUseUI
		if (HookCaller.CallStaticHook(1307002116, player, json) != null) return false;

		CommunityEntity.ServerInstance.ClientRPC(RpcTarget.Player("AddUI", player), json);
		return true;
	}

	public static bool AddUi(BasePlayer player, List<CuiElement> elements)
	{
		if (player == null || player.net == null)
		{
			return false;
		}

		var json = ToJson(elements);

		// CanUseUI
		if (HookCaller.CallStaticHook(1307002116, player, json) != null) return false;

		if (elements != null && elements.Count > 0)
		{
			var element = elements[0].Name;
			var panelList = GetActivePanelList(player);
			if (!panelList.Contains(element)) panelList.Add(element);
		}

		CommunityEntity.ServerInstance.ClientRPC(RpcTarget.Player("AddUI", player), json);
		return true;
	}

	public static bool DestroyUi(BasePlayer player, string name)
	{
		if (player?.net != null)
		{
			var panelList = GetActivePanelList(player);
			if (panelList.Contains(name)) panelList.Remove(name);

			// OnDestroyUI
			HookCaller.CallStaticHook(503981600, player, name);
			CommunityEntity.ServerInstance.ClientRPC(RpcTarget.Player("DestroyUI", player), name);
			return true;
		}

		return false;
	}

	public static void SetColor(this ICuiColor elem, Color color)
	{
		elem.Color = $"{color.r} {color.g} {color.b} {color.a}";
	}

	public static Color GetColor(this ICuiColor elem) => ColorEx.Parse(elem.Color);
}

[JsonConverter(typeof(ComponentConverter))]
public interface ICuiComponent
{
	[JsonProperty("type")]
	string Type { get; }
}

public interface ICuiColor
{
	[JsonProperty("color")]
	string Color { get; set; }
}

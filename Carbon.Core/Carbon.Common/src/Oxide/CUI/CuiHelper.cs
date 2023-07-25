using Facepunch;
using Newtonsoft.Json;
using Formatting = Newtonsoft.Json.Formatting;

/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * All rights reserved.
 *
 */

namespace Oxide.Game.Rust.Cui;

public static class CuiHelper
{
	public static Dictionary<BasePlayer, List<string>> ActivePanels { get; internal set; } = new();

	internal static JsonSerializerSettings _cuiSettings = new()
	{
		DefaultValueHandling = DefaultValueHandling.Ignore
	};

	public static List<string> GetActivePanelList(BasePlayer player)
	{
		if (!ActivePanels.TryGetValue(player, out var panels))
		{
			ActivePanels[player] = panels = new();
		}

		return panels;
	}

	public static void DestroyActivePanelList(BasePlayer player, string[] except = null)
	{
		var list = GetActivePanelList(player);
		var cui = Pool.GetList<string>();
		cui.AddRange(GetActivePanelList(player).Where(x => except == null || except.Length == 0 ? true : !except.Any(y => x.StartsWith(y))));

		foreach (var element in cui)
		{
			DestroyUi(player, element);
		}

		Pool.FreeList(ref cui);
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

	public static string GetGuid() => $"{Guid.NewGuid():N}";

	public static bool AddUi(BasePlayer player, List<CuiElement> elements)
	{
		var json = ToJson(elements);
		if (player?.net == null || Interface.CallHook("CanUseUI", player, json) != null) return false;

		if (elements != null && elements.Count > 0)
		{
			var element = elements[0].Name;
			var panelList = GetActivePanelList(player);
			if (!panelList.Contains(element)) panelList.Add(element);
		}

		return AddUi(player, json, true);
	}

	public static bool AddUi(BasePlayer player, string json, object token = null)
	{
		if (token is bool hookResult1 && !hookResult1) return false;
		else token = Interface.CallHook("CanUseUI", player, json) == null;

		if (player?.net != null && token is bool hookResult2 && hookResult2)
		{
			CommunityEntity.ServerInstance.ClientRPCEx(new Network.SendInfo { connection = player.net.connection }, null, "AddUI", json);
			return true;
		}

		return false;
	}

	public static bool DestroyUi(BasePlayer player, string name)
	{
		if (player?.net != null)
		{
			var panelList = GetActivePanelList(player);
			if (panelList.Contains(name)) panelList.Remove(name);

			Interface.CallHook("OnDestroyUI", player, name);
			CommunityEntity.ServerInstance.ClientRPCEx(new Network.SendInfo { connection = player.net.connection }, null, "DestroyUI", name);
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

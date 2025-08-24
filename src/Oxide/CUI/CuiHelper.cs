using Facepunch;
using Newtonsoft.Json;
using ProtoBuf;
using Formatting = Newtonsoft.Json.Formatting;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace Oxide.Game.Rust.Cui;

public sealed class JsonArrayPool<T> : IArrayPool<T>
{
	public static readonly JsonArrayPool<T> Shared = new JsonArrayPool<T>();
	public T[] Rent(int minimumLength) => System.Buffers.ArrayPool<T>.Shared.Rent(minimumLength);
	public void Return(T[] array) => System.Buffers.ArrayPool<T>.Shared.Return(array);
}

public static class CuiHelper
{
	public static Dictionary<BasePlayer, HashSet<string>> ActivePanels { get; } = new();

	private static readonly StringBuilder sb = new StringBuilder(64 * 1024);

	private static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
	{
		DefaultValueHandling = DefaultValueHandling.Ignore,
		NullValueHandling = NullValueHandling.Ignore,
		DateParseHandling = DateParseHandling.None,
		FloatFormatHandling = FloatFormatHandling.Symbol,
		StringEscapeHandling = StringEscapeHandling.Default
	};

	private static readonly JsonSerializer _serializer = JsonSerializer.Create(Settings);
	private static readonly StringWriter sw = new StringWriter(sb, CultureInfo.InvariantCulture);
	private static readonly JsonTextWriter jw = new JsonTextWriter(sw)
	{
		Formatting = Formatting.None,
		ArrayPool = JsonArrayPool<char>.Shared,
		CloseOutput = false
	};
	private static readonly JsonTextWriter jwFormated = new JsonTextWriter(sw)
	{
		Formatting = Formatting.Indented,
		ArrayPool = JsonArrayPool<char>.Shared,
		CloseOutput = false
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
		sb.Clear();
		var writer = format ? jwFormated : jw;
		_serializer.Serialize(writer, elements);
		var json = sb.ToString().Replace("\\n", "\n");
		return json;
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
		sb.Clear();
		sb.Append(color.r).Append(' ')
			.Append(color.g).Append(' ')
			.Append(color.b).Append(' ')
			.Append(color.a);
		elem.Color = sb.ToString();
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

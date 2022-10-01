///
/// Copyright (c) 2022 Carbon Community 
/// All rights reserved
/// 

using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Oxide.Core;
using UnityEngine;
using Formatting = Newtonsoft.Json.Formatting;

namespace Oxide.Game.Rust.Cui
{
	public static class CuiHelper
	{
		public static string ToJson (List<CuiElement> elements, bool format = false)
		{
			return JsonConvert.SerializeObject(elements, format ? Formatting.Indented : Formatting.None, new JsonSerializerSettings
			{
				DefaultValueHandling = DefaultValueHandling.Ignore
			}).Replace("\\n", "\n");
		}

		public static List<CuiElement> FromJson (string json) => JsonConvert.DeserializeObject<List<CuiElement>>(json);

		public static string GetGuid () => Guid.NewGuid().ToString().Replace("-", string.Empty);

		public static bool AddUi (BasePlayer player, List<CuiElement> elements) => AddUi(player, ToJson(elements));

		public static bool AddUi (BasePlayer player, string json)
		{
			if (player?.net != null && Interface.CallHook("CanUseUI", player, json) == null)
			{
				CommunityEntity.ServerInstance.ClientRPCEx(new Network.SendInfo { connection = player.net.connection }, null, "AddUI", json);
				return true;
			}

			return false;
		}

		public static bool DestroyUi (BasePlayer player, string elem)
		{
			if (player?.net != null)
			{
				Interface.CallHook("OnDestroyUI", player, elem);
				CommunityEntity.ServerInstance.ClientRPCEx(new Network.SendInfo { connection = player.net.connection }, null, "DestroyUI", elem);
				return true;
			}

			return false;
		}

		public static void SetColor (this ICuiColor elem, Color color)
		{
			elem.Color = $"{color.r} {color.g} {color.b} {color.a}";
		}

		public static Color GetColor (this ICuiColor elem) => ColorEx.Parse(elem.Color);
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
}

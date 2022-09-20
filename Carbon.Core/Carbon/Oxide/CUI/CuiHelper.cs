using Network;
using Oxide.Core;
using System.Collections.Generic;
using System;
using UnityEngine;
using Newtonsoft.Json;
using Formatting = Newtonsoft.Json.Formatting;

namespace Oxide.Game.Rust.Cui
{
    public static class CuiHelper
    {
        public static string ToJson ( List<CuiElement> elements, bool format = false )
        {
            return JsonConvert.SerializeObject ( elements, format ? Formatting.Indented : Formatting.None, new JsonSerializerSettings
            {
                DefaultValueHandling = DefaultValueHandling.Ignore
            } ).Replace ( "\\n", "\n" );
        }

        public static List<CuiElement> FromJson ( string json )
        {
            return JsonConvert.DeserializeObject<List<CuiElement>> ( json );
        }

        public static string GetGuid ()
        {
            return Guid.NewGuid ().ToString ().Replace ( "-", string.Empty );
        }

        public static bool AddUi ( BasePlayer player, List<CuiElement> elements )
        {
            return AddUi ( player, ToJson ( elements ) );
        }

        public static bool AddUi ( BasePlayer player, string json )
        {
            if ( player?.net != null && Interface.CallHook ( "CanUseUI", player, json ) == null )
            {
                CommunityEntity.ServerInstance.ClientRPCEx ( new SendInfo
                {
                    connection = player.net.connection
                }, null, "AddUI", json );
                return true;
            }

            return false;
        }

        public static bool DestroyUi ( BasePlayer player, string elem )
        {
            if ( player?.net != null )
            {
                Interface.CallHook ( "OnDestroyUI", player, elem );
                CommunityEntity.ServerInstance.ClientRPCEx ( new SendInfo
                {
                    connection = player.net.connection
                }, null, "DestroyUI", elem );
                return true;
            }

            return false;
        }

        public static void SetColor ( this ICuiColor elem, Color color )
        {
            elem.Color = $"{color.r} {color.g} {color.b} {color.a}";
        }

        public static Color GetColor ( this ICuiColor elem )
        {
            return ColorEx.Parse ( elem.Color );
        }
    }

    [JsonConverter ( typeof ( ComponentConverter ) )]
    public interface ICuiComponent
    {
        [JsonProperty ( "type" )]
        string Type { get; }
    }

    public interface ICuiColor
    {
        [JsonProperty ( "color" )]
        string Color { get; set; }
    }
}
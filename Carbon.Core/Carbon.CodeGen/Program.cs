using Humanlights.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Carbon.CodeGen
{
    internal class Program
    {
        static void Main ( string [] args )
        {
            DoHooks ();

            Console.ReadLine ();
        }

        public static void DoHookDocs ()
        {
            var hooks = "..\\..\\..\\..\\Tools\\hooks.json";
            var jobject = JsonConvert.DeserializeObject<JObject> ( OsEx.File.ReadText ( hooks ) );
            var result = $@"---
description: >-
  This is a solution to your hook problems. Carbon.Extended provides an
  extensive amount of hooks that work with most Oxide plugins, and more!
---

# Carbon.Extended

## Download

Get the latest version of Carbon.Extended [**here**](https://github.com/Carbon-Modding/Carbon.Core/releases/latest/download/Carbon.Extended.dll)!

# Hooks
";

            var categories = new Dictionary<string, List<JObject>> ();

            foreach ( var entry in jobject [ "data" ] )
            {
                var subcategory = entry [ "subcategory" ].ToString ();
                var list = ( List<JObject> )null;

                if ( !categories.TryGetValue ( subcategory, out list ) )
                {
                    categories.Add ( subcategory, list = new List<JObject> () );
                }

                list.Add ( entry.ToObject<JObject> () );

                Console.WriteLine ( $"{entry [ "example" ]}" );
            }

            foreach ( var category in categories )
            {
                result += $"## {category.Key}\n";

                foreach ( var entry in category.Value )
                {
                    result += $@"<details>
<summary>{entry [ "name" ]}</summary>
{entry [ "description" ]}

{entry [ "example" ]}
</details>

";
                }
            }

            OsEx.File.Create ( "test.md", result );
        }

        public static string [] WhitelistedHooks = new string []
        {
            "CanDropActiveItem",
            "OnPlayerDropActiveItem",
            "CanMoveItem"
        };

        public static void DoHooks ()
        {
            var hooks = JsonConvert.DeserializeObject<HookPackage> ( OsEx.File.ReadText ( "..\\..\\..\\..\\Tools\\Rust.opj" ) );
            var hookFolder = "..\\..\\..\\Carbon.Extended\\Hooks";
            var rust = typeof ( Bootstrap ).Assembly;

            OsEx.Folder.Create ( hookFolder, true );

            foreach ( var manifest in hooks.Manifests )
            {
                foreach ( var hook in manifest.Hooks )
                {
                    var output = string.Empty;
                    var hookName = ( string.IsNullOrEmpty ( hook.Hook.BaseHookName ) ? hook.Hook.HookName : hook.Hook.BaseHookName ).Split ( ' ' ) [ 0 ];
                    if ( hookName.Contains ( "/" ) || !WhitelistedHooks.Contains ( hookName ) ) continue;

                    var typeName = hook.Hook.TypeName.Replace ( "/", "." ).Split ( new string [] { ".<" }, StringSplitOptions.RemoveEmptyEntries ) [ 0 ];
                    var methodName = hook.Hook.Signature.Name.Contains ( "<Start" ) ? "Start" : hook.Hook.Signature.Name;
                    var method = rust.GetType ( typeName ).GetMethod ( methodName );
                    var parameters = method.GetParameters ().Select ( x => $"{x.ParameterType.FullName.Replace ( "+", "." )} {x.Name}" ).ToArray ();
                    var arguments = hook.Hook.ArgumentString;

                    if ( hook.Hook.Instructions != null && hook.Hook.Instructions.Length > 0 )
                    {

                    }
                    else if ( hook.Hook.InjectionIndex > 0 && !hookName.StartsWith ( "Can" ) ) output = ( method.ReturnType == typeof ( void ) ?
                            GetTemplate_NoReturn ( "Postfix", parameters.ToString ( ", " ), arguments ) :
                            GetTemplate_Return ( "Postfix", parameters.ToString ( ", " ), method.ReturnType.FullName, arguments ) )
                            .Replace ( "[TYPE]", typeName )
                            .Replace ( "[METHOD]", methodName )
                            .Replace ( "[HOOK]", hookName );
                    else output = ( method.ReturnType == typeof ( void ) ?
                            GetTemplate_NoReturn ( "Prefix", parameters.ToString ( ", " ), arguments ) :
                            GetTemplate_Return ( "Prefix", parameters.ToString ( ", " ), method.ReturnType.FullName, arguments ) )
                            .Replace ( "[TYPE]", typeName )
                            .Replace ( "[METHOD]", methodName )
                            .Replace ( "[HOOK]", hookName );

                    Console.WriteLine ( $"{typeName}.{methodName} -> {hookName} {arguments}" );

                    if ( output.Length > 0 ) OsEx.File.Create ( Path.Combine ( hookFolder, $"{hookName}.cs" ), output );
                }
            }
        }

        public static string GetTemplate_NoReturn ( string method, string parameters = null, string arguments = null )
        {
            return $@"using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{{
    [HarmonyPatch ( typeof ( [TYPE] ), ""[METHOD]"" )]
    public class [HOOK]
    {{
        public static {( method == "Prefix" ? "bool" : "void" )} {method} ( {( string.IsNullOrEmpty ( parameters ) ? "" : $"{parameters} " )}{( arguments != null && arguments.Contains ( "this" ) ? $", ref [TYPE] __instance " : "" )})
        {{
            {(method == "Prefix" ? $"return HookExecutor.CallStaticHook ( \"[HOOK]\"{(string.IsNullOrEmpty( arguments ) ? "" : $", {arguments.Replace ( "this", "__instance" )}")} ) == null;" : "HookExecutor.CallStaticHook ( \"[HOOK]\" );" )}
        }}
    }}
}}";
        }
        public static string GetTemplate_Return ( string method, string parameters = null, string returnType = "object", string arguments = null )
        {
            return $@"using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{{
    [HarmonyPatch ( typeof ( [TYPE] ), ""[METHOD]"" )]
    public class [HOOK]
    {{
        public static bool {method} ( {( string.IsNullOrEmpty ( parameters ) ? "" : $"{parameters}, " )}ref {returnType} __result{( arguments != null && arguments.Contains("this") ? $", ref [TYPE] __instance" : "")} )
        {{
            CarbonCore.Log ( $""{method} [HOOK]"" );

            var result = HookExecutor.CallStaticHook ( ""[HOOK]""{( string.IsNullOrEmpty ( arguments ) ? "" : $", {arguments.Replace("this", "__instance")}" )} );
            
            if ( result != null )
            {{
                __result = ( {returnType} ) result;
                return false;
            }}

            return true;
        }}
    }}
}}";
        }
    }
}
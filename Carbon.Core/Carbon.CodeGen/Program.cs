using Humanlights.Extensions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Carbon.CodeGen
{
    internal class Program
    {
        static void Main ( string [] args )
        {
            var hooks = JsonConvert.DeserializeObject<HookPackage> ( OsEx.File.ReadText ( "..\\..\\..\\..\\Tools\\Rust.opj" ) );
            var hookFolder = "..\\..\\..\\Carbon.Extended\\Hooks";

            foreach ( var manifest in hooks.Manifests )
            {
                foreach ( var hook in manifest.Hooks )
                {
                    Console.WriteLine ( $"{hook.Type} {hook.Hook.Name} {hook.Hook.TypeName}" );

                    var output = $@"using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{{
    [HarmonyPatch ( typeof ( {hook.Hook.TypeName} ), ""{hook.Hook.Signature.Name}"" )]
    public class {hook.Hook.Name}
    {{
        public static void Prefix ()
        {{
            HookExecutor.CallStaticHook ( ""{hook.Hook.Name}"" );
        }}
    }}
}}";
                    OsEx.File.Create ( Path.Combine ( hookFolder, $"{hook.Hook.Name}.cs" ), output );
                }
            }

            Console.WriteLine ( $"{hooks == null}" );
            Console.ReadLine ();
        }
    }
}

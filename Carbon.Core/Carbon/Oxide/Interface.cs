using Carbon.Core;
using Carbon.Core.Harmony;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oxide.Core
{
    public class Interface
    {
        public static OxideMod Oxide { get; set; } = new OxideMod ();

        public static void Initialize ()
        {
            Oxide.Load ();
            CarbonCore.Log ( $"  Instance Directory: {Oxide.InstanceDirectory}" );
            CarbonCore.Log ( $"  Root Directory: {Oxide.RootDirectory}" );
            CarbonCore.Log ( $"  Config Directory: {Oxide.ConfigDirectory}" );
            CarbonCore.Log ( $"  Data Directory: {Oxide.DataDirectory}" );
            CarbonCore.Log ( $"  Lang Directory: {Oxide.LangDirectory}" );
            CarbonCore.Log ( $"  Log Directory: {Oxide.LogDirectory}" );
            CarbonCore.Log ( $"  Plugin Directory: {Oxide.PluginDirectory}" );
        }

        public static object CallHook(string hook, params object [] args )
        {
            return HookExecutor.CallStaticHook ( hook, args );
        }
    }
}

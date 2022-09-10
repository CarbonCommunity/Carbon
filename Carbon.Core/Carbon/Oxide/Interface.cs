using Carbon.Core;

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

        public static object CallHook ( string hookName )
        {
            return HookExecutor.CallStaticHook ( hookName );
        }
        public static object CallHook ( string hookName, object arg1 )
        {
            return HookExecutor.CallStaticHook ( hookName, arg1 );
        }
        public static object CallHook ( string hookName, object arg1, object arg2 )
        {
            return HookExecutor.CallStaticHook ( hookName, arg1, arg2 );
        }
        public static object CallHook ( string hookName, object arg1, object arg2, object arg3 )
        {
            return HookExecutor.CallStaticHook ( hookName, arg1, arg2, arg3 );
        }
        public static object CallHook ( string hookName, object arg1, object arg2, object arg3, object arg4 )
        {
            return HookExecutor.CallStaticHook ( hookName, arg1, arg2, arg3, arg4 );
        }
        public static object CallHook ( string hookName, object arg1, object arg2, object arg3, object arg4, object arg5 )
        {
            return HookExecutor.CallStaticHook ( hookName, arg1, arg2, arg3, arg4, arg5 );
        }
        public static object CallHook ( string hookName, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6 )
        {
            return HookExecutor.CallStaticHook ( hookName, arg1, arg2, arg3, arg4, arg5, arg6 );
        }
        public static object CallHook ( string hookName, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7 )
        {
            return HookExecutor.CallStaticHook ( hookName, arg1, arg2, arg3, arg4, arg5, arg6, arg7 );
        }
        public static object CallHook ( string hookName, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8 )
        {
            return HookExecutor.CallStaticHook ( hookName, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8 );
        }
        public static object CallHook ( string hookName, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9 )
        {
            return HookExecutor.CallStaticHook ( hookName, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9 );
        }
    }
}

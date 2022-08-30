using Oxide.Plugins;
using System.Collections.Generic;
using System.Reflection;

namespace Carbon.Core.Harmony
{
    public static class HookExecutor
    {
        internal static Dictionary<string, MethodInfo> _hookCache { get; } = new Dictionary<string, MethodInfo> ();

        public static object CallHook<T> ( this T plugin, string hookName, BindingFlags flags, params object [] args ) where T : Plugin
        {
            var hook = ( MethodInfo )null;

            if ( !_hookCache.TryGetValue ( hookName, out hook ) )
            {
                hook = plugin.Type.GetMethod ( hookName, flags );

                if ( hook == null )
                {
                    CarbonCore.Error ( $"Couldn't find hook '{hookName}'" );
                    return null;
                }

                _hookCache.Add ( hookName, hook );
            }

            return hook?.Invoke ( plugin, args );
        }
        public static object CallHook<T> ( this T plugin, string hookName, params object [] args ) where T : Plugin
        {
            return CallHook ( plugin, hookName, BindingFlags.NonPublic | BindingFlags.Instance, args );
        }
        public static object CallPublicHook<T> ( this T plugin, string hookName, params object [] args ) where T : Plugin
        {
            return CallHook ( plugin, hookName, BindingFlags.Public | BindingFlags.Instance, args );
        }

        public static object CallStaticHook ( string hookName, BindingFlags flag, params object [] args )
        {
            var objectOverride = ( object )null;
            var pluginOverride = ( Plugin )null;

            foreach ( var mod in CarbonLoader._loadedMods )
            {
                foreach ( var plugin in mod.Plugins )
                {
                    try
                    {
                        var result = plugin.CallHook ( hookName, flag, args ); ;
                        if ( result != null && objectOverride != null )
                        {
                            CarbonCore.WarnFormat ( $"Hook '{hookName}' conflicts with {pluginOverride.Name}" );
                            break;
                        }

                        objectOverride = result;
                        pluginOverride = plugin;
                    }
                    catch { }
                }
            }

            return objectOverride;
        }
        public static object CallStaticHook ( string hookName, params object [] args )
        {
            return CallStaticHook ( hookName, BindingFlags.NonPublic | BindingFlags.Instance, args );
        }
        public static object CallPublicStaticHook ( string hookName, params object [] args )
        {
            return CallStaticHook ( hookName, BindingFlags.Public | BindingFlags.Instance, args );
        }
    }
}
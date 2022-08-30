using Oxide.Plugins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Carbon.Core.Harmony
{
    public static class HookExecutor
    {
        public static object CallHook<T> ( this T plugin, string hookName, BindingFlags flags, params object [] args ) where T : Plugin
        {
            return plugin.Type.GetMethod ( hookName, flags )?.Invoke ( plugin, args );
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
                        if(result != null && objectOverride != null )
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
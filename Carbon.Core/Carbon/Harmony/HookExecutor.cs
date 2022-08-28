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

        public static void CallStaticHook ( string hookName, BindingFlags flag, params object [] args )
        {
            foreach ( var mod in CarbonLoader._loadedMods )
            {
                foreach ( var plugin in mod.Plugins )
                {
                    try
                    {
                        plugin.CallHook ( hookName, flag, args );
                    }
                    catch { }
                }
            }
        }

        public static void CallStaticHook ( string hookName, params object [] args )
        {
            CallStaticHook ( hookName, BindingFlags.NonPublic | BindingFlags.Instance, args );
        }
        public static void CallPublicStaticHook ( string hookName, params object [] args )
        {
            CallStaticHook ( hookName, BindingFlags.Public | BindingFlags.Instance, args );
        }
    }
}
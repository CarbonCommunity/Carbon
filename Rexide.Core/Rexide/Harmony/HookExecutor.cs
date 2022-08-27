using Oxide.Plugins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Rexide.Core.Harmony
{
    public static class HookExecutor
    {
        public static object CallHook ( this Plugin plugin, string hookName, BindingFlags flags, params object [] args )
        {
            return plugin.Type.GetMethod ( hookName, flags )?.Invoke ( plugin, args );
        }
        public static object CallHook ( this Plugin plugin, string hookName, params object [] args )
        {
            return CallHook ( plugin, hookName, BindingFlags.NonPublic | BindingFlags.Instance, args );
        }
        public static object CallPublicHook ( this Plugin plugin, string hookName, params object [] args )
        {
            return CallHook ( plugin, hookName, BindingFlags.Public | BindingFlags.Instance, args );
        }

        public static void CallStaticHook ( string hookName, params object [] args )
        {
            foreach ( var mod in RexideLoader._loadedMods )
            {
                foreach ( var plugin in mod.Plugins )
                {
                    try
                    {
                        plugin.CallHook ( hookName, args );
                    }
                    catch { }
                }
            }
        }
    }
}
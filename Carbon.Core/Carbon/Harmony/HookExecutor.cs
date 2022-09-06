using Oxide.Plugins;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Carbon.Core
{
    public static class HookExecutor
    {
        internal static Dictionary<int, object []> _argumentBuffer { get; } = new Dictionary<int, object []> ();

        internal static object [] _allocateBuffer ( int count )
        {
            if ( !_argumentBuffer.TryGetValue ( count, out var buffer ) )
            {
                _argumentBuffer.Add ( count, buffer = new object [ count ] );
            }

            return buffer;
        }
        internal static void _clearBuffer ( object [] buffer )
        {
            for ( int i = 0; i < buffer.Length; i++ )
            {
                buffer [ i ] = null;
            }
        }

        public static object CallHook<T> ( T plugin, string hookName ) where T : Plugin
        {
            return CallHook ( plugin, hookName, BindingFlags.NonPublic | BindingFlags.Instance, null );
        }
        public static object CallHook<T> ( T plugin, string hookName, object arg1 ) where T : Plugin
        {
            var buffer = _allocateBuffer ( 1 );
            buffer [ 0 ] = arg1;

            var result = CallHook ( plugin, hookName, BindingFlags.NonPublic | BindingFlags.Instance, buffer );

            _clearBuffer ( buffer );
            return result;
        }
        public static object CallHook<T> ( T plugin, string hookName, object arg1, object arg2 ) where T : Plugin
        {
            var buffer = _allocateBuffer ( 2 );
            buffer [ 0 ] = arg1;
            buffer [ 1 ] = arg2;

            var result = CallHook ( plugin, hookName, BindingFlags.NonPublic | BindingFlags.Instance, buffer );

            _clearBuffer ( buffer );
            return result;
        }
        public static object CallHook<T> ( T plugin, string hookName, object arg1, object arg2, object arg3 ) where T : Plugin
        {
            var buffer = _allocateBuffer ( 3 );
            buffer [ 0 ] = arg1;
            buffer [ 1 ] = arg2;
            buffer [ 2 ] = arg3;

            var result = CallHook ( plugin, hookName, BindingFlags.NonPublic | BindingFlags.Instance, buffer );

            _clearBuffer ( buffer );
            return result;
        }
        public static object CallHook<T> ( T plugin, string hookName, object arg1, object arg2, object arg3, object arg4 ) where T : Plugin
        {
            var buffer = _allocateBuffer ( 4 );
            buffer [ 0 ] = arg1;
            buffer [ 1 ] = arg2;
            buffer [ 2 ] = arg3;
            buffer [ 3 ] = arg4;

            var result = CallHook ( plugin, hookName, BindingFlags.NonPublic | BindingFlags.Instance, buffer );

            _clearBuffer ( buffer );
            return result;
        }
        public static object CallHook<T> ( T plugin, string hookName, object arg1, object arg2, object arg3, object arg4, object arg5 ) where T : Plugin
        {
            var buffer = _allocateBuffer ( 5 );
            buffer [ 0 ] = arg1;
            buffer [ 1 ] = arg2;
            buffer [ 2 ] = arg3;
            buffer [ 3 ] = arg4;
            buffer [ 4 ] = arg5;

            var result = CallHook ( plugin, hookName, BindingFlags.NonPublic | BindingFlags.Instance, buffer );

            _clearBuffer ( buffer );
            return result;
        }
        public static object CallHook<T> ( T plugin, string hookName, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6 ) where T : Plugin
        {
            var buffer = _allocateBuffer ( 6 );
            buffer [ 0 ] = arg1;
            buffer [ 1 ] = arg2;
            buffer [ 2 ] = arg3;
            buffer [ 3 ] = arg4;
            buffer [ 4 ] = arg5;
            buffer [ 6 ] = arg6;

            var result = CallHook ( plugin, hookName, BindingFlags.NonPublic | BindingFlags.Instance, buffer );

            _clearBuffer ( buffer );
            return result;
        }
        public static object CallHook<T> ( T plugin, string hookName, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7 ) where T : Plugin
        {
            var buffer = _allocateBuffer ( 7 );
            buffer [ 0 ] = arg1;
            buffer [ 1 ] = arg2;
            buffer [ 2 ] = arg3;
            buffer [ 3 ] = arg4;
            buffer [ 4 ] = arg5;
            buffer [ 6 ] = arg6;
            buffer [ 7 ] = arg7;

            var result = CallHook ( plugin, hookName, BindingFlags.NonPublic | BindingFlags.Instance, buffer );

            _clearBuffer ( buffer );
            return result;
        }
        public static object CallHook<T> ( T plugin, string hookName, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8 ) where T : Plugin
        {
            var buffer = _allocateBuffer ( 8 );
            buffer [ 0 ] = arg1;
            buffer [ 1 ] = arg2;
            buffer [ 2 ] = arg3;
            buffer [ 3 ] = arg4;
            buffer [ 4 ] = arg5;
            buffer [ 6 ] = arg6;
            buffer [ 7 ] = arg7;
            buffer [ 8 ] = arg8;

            var result = CallHook ( plugin, hookName, BindingFlags.NonPublic | BindingFlags.Instance, buffer );

            _clearBuffer ( buffer );
            return result;
        }
        public static object CallHook<T> ( T plugin, string hookName, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9 ) where T : Plugin
        {
            var buffer = _allocateBuffer ( 9 );
            buffer [ 0 ] = arg1;
            buffer [ 1 ] = arg2;
            buffer [ 2 ] = arg3;
            buffer [ 3 ] = arg4;
            buffer [ 4 ] = arg5;
            buffer [ 6 ] = arg6;
            buffer [ 7 ] = arg7;
            buffer [ 8 ] = arg8;
            buffer [ 9 ] = arg9;

            var result = CallHook ( plugin, hookName, BindingFlags.NonPublic | BindingFlags.Instance, buffer );

            _clearBuffer ( buffer );
            return result;
        }

        private static object CallHook<T> ( this T plugin, string hookName, BindingFlags flags, object [] args ) where T : Plugin
        {
            if ( !plugin.HookCache.TryGetValue ( hookName + args?.Length, out var hook ) )
            {
                hook = plugin.Type.GetMethod ( hookName, flags );

                if ( hook == null || !plugin.HookMethodAttributeCache.TryGetValue ( hookName + args?.Length, out hook ) )
                {
                    return null;
                }

                plugin.HookCache.Add ( hookName + args?.Length, hook );
            }

            var beforeTicks = Environment.TickCount;
            var result = hook?.Invoke ( plugin, args );
            var afterTicks = Environment.TickCount;

            if ( afterTicks > beforeTicks + 100 && afterTicks > beforeTicks )
            {
                CarbonCore.WarnFormat ( $" {plugin?.Name} hook took longer than 100ms {hookName}{( args == null ? "" : $"[{args?.Length}]" )} [{( afterTicks - beforeTicks ):0}ms]" );
            }

            return result;
        }

        private static object CallStaticHook ( string hookName, BindingFlags flag = BindingFlags.NonPublic | BindingFlags.Static, object [] args = null )
        {
            var objectOverride = ( object )null;
            var pluginOverride = ( Plugin )null;

            foreach ( var mod in CarbonLoader._loadedMods )
            {
                foreach ( var plugin in mod.Plugins )
                {
                    try
                    {
                        var result = plugin.CallHook ( hookName, flags: flag, args: args );
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

        public static object CallStaticHook ( string hookName )
        {
            return CallStaticHook ( hookName, BindingFlags.NonPublic | BindingFlags.Instance, null );
        }
        public static object CallStaticHook ( string hookName, object arg1 )
        {
            var buffer = _allocateBuffer ( 1 );
            buffer [ 0 ] = arg1;

            var result = CallStaticHook ( hookName, flag: BindingFlags.NonPublic | BindingFlags.Instance, args: buffer );

            _clearBuffer ( buffer );
            return result;
        }
        public static object CallStaticHook ( string hookName, object arg1, object arg2 )
        {
            var buffer = _allocateBuffer ( 2 );
            buffer [ 0 ] = arg1;
            buffer [ 1 ] = arg2;

            var result = CallStaticHook ( hookName, flag: BindingFlags.NonPublic | BindingFlags.Instance, args: buffer );

            _clearBuffer ( buffer );
            return result;
        }
        public static object CallStaticHook ( string hookName, object arg1, object arg2, object arg3 )
        {
            var buffer = _allocateBuffer ( 3 );
            buffer [ 0 ] = arg1;
            buffer [ 1 ] = arg2;
            buffer [ 2 ] = arg3;

            var result = CallStaticHook ( hookName, flag: BindingFlags.NonPublic | BindingFlags.Instance, args: buffer );

            _clearBuffer ( buffer );
            return result;
        }
        public static object CallStaticHook ( string hookName, object arg1, object arg2, object arg3, object arg4 )
        {
            var buffer = _allocateBuffer ( 4 );
            buffer [ 0 ] = arg1;
            buffer [ 1 ] = arg2;
            buffer [ 2 ] = arg3;
            buffer [ 3 ] = arg4;

            var result = CallStaticHook ( hookName, flag: BindingFlags.NonPublic | BindingFlags.Instance, args: buffer );

            _clearBuffer ( buffer );
            return result;
        }
        public static object CallStaticHook ( string hookName, object arg1, object arg2, object arg3, object arg4, object arg5 )
        {
            var buffer = _allocateBuffer ( 5 );
            buffer [ 0 ] = arg1;
            buffer [ 1 ] = arg2;
            buffer [ 2 ] = arg3;
            buffer [ 3 ] = arg4;
            buffer [ 4 ] = arg5;

            var result = CallStaticHook ( hookName, flag: BindingFlags.NonPublic | BindingFlags.Instance, args: buffer );

            _clearBuffer ( buffer );
            return result;
        }
        public static object CallStaticHook ( string hookName, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6 )
        {
            var buffer = _allocateBuffer ( 6 );
            buffer [ 0 ] = arg1;
            buffer [ 1 ] = arg2;
            buffer [ 2 ] = arg3;
            buffer [ 3 ] = arg4;
            buffer [ 4 ] = arg5;
            buffer [ 6 ] = arg6;

            var result = CallStaticHook ( hookName, flag: BindingFlags.NonPublic | BindingFlags.Instance, args: buffer );

            _clearBuffer ( buffer );
            return result;
        }
        public static object CallStaticHook ( string hookName, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7 )
        {
            var buffer = _allocateBuffer ( 7 );
            buffer [ 0 ] = arg1;
            buffer [ 1 ] = arg2;
            buffer [ 2 ] = arg3;
            buffer [ 3 ] = arg4;
            buffer [ 4 ] = arg5;
            buffer [ 6 ] = arg6;
            buffer [ 7 ] = arg7;

            var result = CallStaticHook ( hookName, flag: BindingFlags.NonPublic | BindingFlags.Instance, args: buffer );

            _clearBuffer ( buffer );
            return result;
        }
        public static object CallStaticHook ( string hookName, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8 )
        {
            var buffer = _allocateBuffer ( 8 );
            buffer [ 0 ] = arg1;
            buffer [ 1 ] = arg2;
            buffer [ 2 ] = arg3;
            buffer [ 3 ] = arg4;
            buffer [ 4 ] = arg5;
            buffer [ 6 ] = arg6;
            buffer [ 7 ] = arg7;
            buffer [ 8 ] = arg8;

            var result = CallStaticHook ( hookName, flag: BindingFlags.NonPublic | BindingFlags.Instance, args: buffer );

            _clearBuffer ( buffer );
            return result;
        }
        public static object CallStaticHook ( string hookName, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9 )
        {
            var buffer = _allocateBuffer ( 9 );
            buffer [ 0 ] = arg1;
            buffer [ 1 ] = arg2;
            buffer [ 2 ] = arg3;
            buffer [ 3 ] = arg4;
            buffer [ 4 ] = arg5;
            buffer [ 6 ] = arg6;
            buffer [ 7 ] = arg7;
            buffer [ 8 ] = arg8;
            buffer [ 9 ] = arg9;

            var result = CallStaticHook ( hookName, flag: BindingFlags.NonPublic | BindingFlags.Instance, args: buffer );

            _clearBuffer ( buffer );
            return result;
        }


        public static object CallPublicHook<T> ( T plugin, string hookName ) where T : Plugin
        {
            return CallPublicHook ( plugin, hookName, BindingFlags.Public | BindingFlags.Instance, null );
        }
        public static object CallPublicHook<T> ( T plugin, string hookName, object arg1 ) where T : Plugin
        {
            var buffer = _allocateBuffer ( 1 );
            buffer [ 0 ] = arg1;

            var result = CallPublicHook ( plugin, hookName, buffer );

            _clearBuffer ( buffer );
            return result;
        }
        public static object CallPublicHook<T> ( T plugin, string hookName, object arg1, object arg2 ) where T : Plugin
        {
            var buffer = _allocateBuffer ( 2 );
            buffer [ 0 ] = arg1;
            buffer [ 1 ] = arg2;

            var result = CallPublicHook ( plugin, hookName, buffer );

            _clearBuffer ( buffer );
            return result;
        }
        public static object CallPublicHook<T> ( T plugin, string hookName, object arg1, object arg2, object arg3 ) where T : Plugin
        {
            var buffer = _allocateBuffer ( 3 );
            buffer [ 0 ] = arg1;
            buffer [ 1 ] = arg2;
            buffer [ 2 ] = arg3;

            var result = CallPublicHook ( plugin, hookName, buffer );

            _clearBuffer ( buffer );
            return result;
        }
        public static object CallPublicHook<T> ( T plugin, string hookName, object arg1, object arg2, object arg3, object arg4 ) where T : Plugin
        {
            var buffer = _allocateBuffer ( 4 );
            buffer [ 0 ] = arg1;
            buffer [ 1 ] = arg2;
            buffer [ 2 ] = arg3;
            buffer [ 3 ] = arg4;

            var result = CallPublicHook ( plugin, hookName, buffer );

            _clearBuffer ( buffer );
            return result;
        }
        public static object CallPublicHook<T> ( T plugin, string hookName, object arg1, object arg2, object arg3, object arg4, object arg5 ) where T : Plugin
        {
            var buffer = _allocateBuffer ( 5 );
            buffer [ 0 ] = arg1;
            buffer [ 1 ] = arg2;
            buffer [ 2 ] = arg3;
            buffer [ 3 ] = arg4;
            buffer [ 4 ] = arg5;

            var result = CallPublicHook ( plugin, hookName, buffer );

            _clearBuffer ( buffer );
            return result;
        }
        public static object CallPublicHook<T> ( T plugin, string hookName, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6 ) where T : Plugin
        {
            var buffer = _allocateBuffer ( 6 );
            buffer [ 0 ] = arg1;
            buffer [ 1 ] = arg2;
            buffer [ 2 ] = arg3;
            buffer [ 3 ] = arg4;
            buffer [ 4 ] = arg5;
            buffer [ 6 ] = arg6;

            var result = CallPublicHook ( plugin, hookName, buffer );

            _clearBuffer ( buffer );
            return result;
        }
        public static object CallPublicHook<T> ( T plugin, string hookName, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7 ) where T : Plugin
        {
            var buffer = _allocateBuffer ( 7 );
            buffer [ 0 ] = arg1;
            buffer [ 1 ] = arg2;
            buffer [ 2 ] = arg3;
            buffer [ 3 ] = arg4;
            buffer [ 4 ] = arg5;
            buffer [ 6 ] = arg6;
            buffer [ 7 ] = arg7;

            var result = CallPublicHook ( plugin, hookName, buffer );

            _clearBuffer ( buffer );
            return result;
        }
        public static object CallPublicHook<T> ( T plugin, string hookName, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8 ) where T : Plugin
        {
            var buffer = _allocateBuffer ( 8 );
            buffer [ 0 ] = arg1;
            buffer [ 1 ] = arg2;
            buffer [ 2 ] = arg3;
            buffer [ 3 ] = arg4;
            buffer [ 4 ] = arg5;
            buffer [ 6 ] = arg6;
            buffer [ 7 ] = arg7;
            buffer [ 8 ] = arg8;

            var result = CallPublicHook ( plugin, hookName, buffer );

            _clearBuffer ( buffer );
            return result;
        }
        public static object CallPublicHook<T> ( T plugin, string hookName, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9 ) where T : Plugin
        {
            var buffer = _allocateBuffer ( 9 );
            buffer [ 0 ] = arg1;
            buffer [ 1 ] = arg2;
            buffer [ 2 ] = arg3;
            buffer [ 3 ] = arg4;
            buffer [ 4 ] = arg5;
            buffer [ 6 ] = arg6;
            buffer [ 7 ] = arg7;
            buffer [ 8 ] = arg8;
            buffer [ 9 ] = arg9;

            var result = CallPublicHook ( plugin, hookName, buffer );

            _clearBuffer ( buffer );
            return result;
        }

        private static object CallPublicHook<T> ( this T plugin, string hookName, object [] args ) where T : Plugin
        {
            return CallHook ( plugin, hookName, BindingFlags.Public | BindingFlags.Instance, args );
        }
        private static object CallPublicStaticHook ( string hookName, object [] args )
        {
            return CallStaticHook ( hookName, BindingFlags.Public | BindingFlags.Instance, args );
        }
    }
}
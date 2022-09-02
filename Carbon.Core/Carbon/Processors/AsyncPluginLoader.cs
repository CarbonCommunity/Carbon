using Carbon.Core;
using Humanlights.Unity.Compiler;
using Oxide.Game.Rust.Cui;
using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace Carbon.Core
{
    public class AsyncPluginLoader : ThreadedJob
    {
        public string Source;
        public Assembly Assembly;
        public Exception Exception;

        internal int _retries;

        public override void ThreadFunction ()
        {
            try
            {
                Assembly = CompilerManager.Compile ( Source );
            }
            catch ( Exception exception )
            {
                if ( _retries <= 2 )
                {
                    _retries++;
                    ThreadFunction ();
                }
                else
                {
                    Exception = new Exception ( $"Failed compilation after {_retries} retries.", exception );
                }
            }
        }

        public static void AddCurrentDomainAssemblies ()
        {
            CompilerManager.ReferencedAssemblies.Clear ();

            var assemblies = AppDomain.CurrentDomain.GetAssemblies ();
            var lastCarbon = ( Assembly )null;
            foreach ( var assembly in assemblies )
            {
                if ( CarbonLoader.AssemblyCache.Any ( x => x == assembly ) ) continue;

                // if ( !assembly.FullName.StartsWith ( "Carbon" ) )
                {
                    if ( assembly.ManifestModule is ModuleBuilder builder )
                    {
                        if ( !builder.IsTransient () )
                        {
                            CompilerManager.ReferencedAssemblies.Add ( assembly );
                        }
                    }
                    else
                    {
                        CompilerManager.ReferencedAssemblies.Add ( assembly );
                    }
                }
                //else if ( assembly.FullName.StartsWith ( "Carbon" ) )
                {
                    // lastCarbon = assembly;
                }
            }

            if ( lastCarbon != null )
            {
                CompilerManager.ReferencedAssemblies.Add ( lastCarbon );
                CarbonCore.Log ( $"  Injected {lastCarbon.GetName().Name}" );
            }

            CarbonCore.Log ( $" Added {CompilerManager.ReferencedAssemblies.Count:n0} references." );
        }
    }
}
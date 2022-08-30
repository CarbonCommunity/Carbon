using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Carbon.Core.Harmony;
using Harmony;
using Oxide.Plugins;
using UnityEngine;

namespace Carbon.Core.Processors
{
    public static class CarbonLoader
    {
        public static List<Assembly> AssemblyCache { get; } = new List<Assembly> ();

        public static void LoadCarbonMods ()
        {
            try
            {
                HarmonyInstance.DEBUG = true;
                string path = Path.Combine ( CarbonCore.GetLogsFolder (), ".." );
                FileLog.logPath = Path.Combine ( path, "carbon_log.txt" );
                try
                {
                    File.Delete ( FileLog.logPath );
                }
                catch
                {
                    // ignored
                }

                _modPath = CarbonCore.GetPluginsFolder ();
                if ( !Directory.Exists ( _modPath ) )
                {
                    try
                    {
                        Directory.CreateDirectory ( _modPath );
                        return;
                    }
                    catch
                    {
                        return;
                    }
                }

                AppDomain.CurrentDomain.AssemblyResolve += delegate ( object sender, ResolveEventArgs args )
                {
                    Debug.Log ( "Trying to load assembly: " + args.Name );
                    AssemblyName assemblyName = new AssemblyName ( args.Name );
                    string text2 = Path.Combine ( _modPath, assemblyName.Name + ".dll" );
                    if ( !File.Exists ( text2 ) )
                    {
                        return null;
                    }
                    return LoadAssembly ( text2 );
                };

                foreach ( string text in Directory.EnumerateFiles ( _modPath, "*.dll" ) )
                {
                    if ( !string.IsNullOrEmpty ( text ) && !IsKnownDependency ( Path.GetFileNameWithoutExtension ( text ) ) )
                    {
                        LoadCarbonMod ( text, true );
                    }
                }
            }
            catch ( Exception ex )
            {
                CarbonCore.Error ( "Loading all DLLs failed.", ex );
            }
            finally
            {
                FileLog.FlushBuffer ();
            }
        }
        public static void UnloadCarbonMods ()
        {
            var list = Facepunch.Pool.GetList<CarbonMod> ();
            list.AddRange ( LoadedMods );

            foreach ( var mod in list )
            {
                UnloadCarbonMod ( mod.Name );
            }

            Facepunch.Pool.FreeList ( ref list );
        }

        private static bool LoadCarbonMod ( string name, bool silent = false )
        {
            Log ( name, $"Pre load..." );

            var fileName = Path.GetFileName ( name );

            if ( fileName.EndsWith ( ".dll" ) )
            {
                fileName = fileName.Substring ( 0, fileName.Length - 4 );
            }

            UnloadCarbonMod ( fileName, silent );

            var fullPath = Path.Combine ( _modPath, fileName + ".dll" );
            var domain = "com.rust.carbon." + fileName;

            Log ( domain, "Processing..." );

            try
            {
                var assembly = LoadAssembly ( fullPath );
                if ( assembly == null )
                {
                    LogError ( domain, $"Failed to load harmony mod '{fileName}.dll' from '{_modPath}'" );
                    return false;
                }
                var mod = new CarbonMod
                {
                    Assembly = assembly,
                    AllTypes = assembly.GetTypes (),
                    Name = fileName
                };

                foreach ( var type in mod.AllTypes )
                {
                    if ( typeof ( IHarmonyModHooks ).IsAssignableFrom ( type ) )
                    {
                        try
                        {
                            var harmonyModHooks = Activator.CreateInstance ( type ) as IHarmonyModHooks;

                            if ( harmonyModHooks == null ) LogError ( mod.Name, "Failed to create hook instance: Is null" );
                            else mod.Hooks.Add ( harmonyModHooks );
                        }
                        catch ( Exception arg ) { LogError ( mod.Name, $"Failed to create hook instance {arg}"); }
                    }
                }

                mod.Harmony = HarmonyInstance.Create ( domain );

                try
                {
                    mod.Harmony.PatchAll ( assembly );
                }
                catch ( Exception arg2 )
                {
                    LogError ( mod.Name, $"Failed to patch all hooks: {arg2}");
                    return false;
                }

                foreach ( var hook in mod.Hooks )
                {
                    try
                    {
                        var type = hook.GetType ();
                        if ( type.Name.Equals ( "CarbonInitializer" ) ) continue;

                        hook.OnLoaded ( new OnHarmonyModLoadedArgs () );
                    }
                    catch ( Exception arg3 )
                    {
                        LogError ( mod.Name, $"Failed to call hook 'OnLoaded' {arg3}");
                    }
                }

                Log ( mod.Name, $"Processing plugin..." );
                ProcessPlugin ( mod );
                Log ( mod.Name, $"Processed." );

                AssemblyCache.Add ( assembly );
                LoadedMods.Add ( mod );
            }
            catch ( Exception e )
            {
                LogError ( domain, "Failed to load: " + fullPath );
                ReportException ( domain, e );
                return false;
            }
            return true;
        }
        public static bool UnloadCarbonMod ( string name, bool silent = false )
        {
            CarbonMod mod = GetMod ( name );
            if ( mod == null )
            {
                if ( !silent ) LogError ( "Couldn't unload mod '" + name + "': not loaded" );
                return false;
            }
            foreach ( var hook in mod.Hooks )
            {
                try
                {
                    var type = hook.GetType ();
                    if ( type.Name.Equals ( "CarbonInitializer" ) ) continue;

                    hook.OnUnloaded ( new OnHarmonyModUnloadedArgs () );
                }
                catch ( Exception arg )
                {
                    LogError ( mod.Name, $"Failed to call hook 'OnLoaded' {arg}");
                }
            }

            UnloadMod ( mod );
            return true;
        }

        #region Carbon

        private static void ProcessPlugin ( CarbonMod mod )
        {
            foreach ( var type in mod.AllTypes )
            {
                try
                {
                    if ( type.FullName != null && !type.FullName.StartsWith ( "Oxide.Plugins" ) ) return;

                    if (!type.IsSubclassOf ( typeof ( RustPlugin ) ) ) continue;

                    var instance = Activator.CreateInstance ( type, true );
                    var plugin = instance as RustPlugin;

                    if (plugin == null) continue;
                    plugin.CallPublicHook("SetupMod", mod, type.Name);
                    HookExecutor.CallStaticHook("OnPluginLoaded", plugin);
                    plugin.Init();
                    plugin.LoadConfig();
                    plugin.CallHook("OnServerInitialized");

                    mod.Plugins.Add(plugin);
                    ProcessCommands(type, plugin);

                    Debug.Log($"Loaded: {plugin.Name}");
                }
                catch ( Exception ex ) { CarbonCore.Error ( $"Failed loading '{mod.Name}'", ex ); }
            }
        }
        public static void StalkPluginFolder ()
        {
            if ( CarbonCore.Instance.PluginFolderWatcher != null )
            {
                CarbonCore.Instance.PluginFolderWatcher.Dispose ();
                CarbonCore.Instance.PluginFolderWatcher = null;
            }

            CarbonCore.Instance.PluginFolderWatcher = new FileSystemWatcher ( CarbonCore.GetPluginsFolder ())
            {
                NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.LastAccess | NotifyFilters.FileName,
                Filter = "*.dll"
            };
            CarbonCore.Instance.PluginFolderWatcher.Changed += _onChanged;
            CarbonCore.Instance.PluginFolderWatcher.Created += _onChanged;
            CarbonCore.Instance.PluginFolderWatcher.Renamed += _onRenamed;
            CarbonCore.Instance.PluginFolderWatcher.Deleted += _onRemoved;
            CarbonCore.Instance.PluginFolderWatcher.Error += (sender, err) => { CarbonCore.Error ( $"Shit hit the fan:", err.GetException () ); };

            CarbonCore.Instance.PluginFolderWatcher.EnableRaisingEvents = true;
            CarbonCore.Log ( $"Started stalking '{CarbonCore.Instance.PluginFolderWatcher.Path}'" );
        }

        public static void ProcessCommands ( Type type, RustPlugin plugin = null, BindingFlags flags = BindingFlags.NonPublic | BindingFlags.Instance, string prefix = null )
        {
            var methods = type.GetMethods ( flags );

            foreach ( var method in methods )
            {
                var chatCommand = method.GetCustomAttribute<ChatCommandAttribute> ();
                var consoleCommand = method.GetCustomAttribute<ConsoleCommandAttribute> ();

                if ( chatCommand != null )
                {
                    CarbonCore.Instance.CorePlugin.cmd.AddChatCommand ( string.IsNullOrEmpty ( prefix ) ? chatCommand.Name : $"{prefix}.{chatCommand.Name}", plugin, method.Name );
                }

                if ( consoleCommand != null )
                {
                    CarbonCore.Instance.CorePlugin.cmd.AddConsoleCommand ( string.IsNullOrEmpty ( prefix ) ? consoleCommand.Name : $"{prefix}.{consoleCommand.Name}", plugin, method.Name );
                }
            }

            Facepunch.Pool.Free ( ref methods );
        }

        private static void _onChanged ( object sender, FileSystemEventArgs e )
        {
            CarbonCore.Log ( $"onChanged: '{e.FullPath}" );

            LoadCarbonMod ( e.FullPath, true );
        }

        private static void _onRenamed ( object sender, RenamedEventArgs e )
        {
            CarbonCore.Log ( $"_onRenamed: '{e.OldFullPath}' -> '{e.FullPath}'" );

            UnloadCarbonMod ( e.OldFullPath, true );
            LoadCarbonMod ( e.FullPath );
        }

        private static void _onRemoved ( object sender, FileSystemEventArgs e )
        {
            CarbonCore.Log ( $"_onRemoved: '{e.FullPath}'" );

            UnloadCarbonMod ( e.FullPath );
        }

        #endregion

        private static void UnloadMod ( CarbonMod mod )
        {
            Log ( mod.Name, "UnPatching hooks..." );
            mod.Harmony.UnpatchAll ();
            LoadedMods.Remove ( mod );
            Log ( mod.Name, "Unloaded mod" );
        }

        private static CarbonMod GetMod ( string name )
        {
            return LoadedMods.FirstOrDefault ( x => x.Name.Equals ( name, StringComparison.OrdinalIgnoreCase ) );
        }

        private static Assembly LoadAssembly ( string assemblyPath )
        {
            if ( !File.Exists ( assemblyPath ) )
            {
                return null;
            }
            byte [] rawAssembly = File.ReadAllBytes ( assemblyPath );
            string path = assemblyPath.Substring ( 0, assemblyPath.Length - 4 ) + ".pdb";
            if ( File.Exists ( path ) )
            {
                byte [] rawSymbolStore = File.ReadAllBytes ( path );
                return Assembly.Load ( rawAssembly, rawSymbolStore );
            }
            return Assembly.Load ( rawAssembly );
        }

        private static bool IsKnownDependency ( string assemblyName )
        {
            return assemblyName.StartsWith ( "System.", StringComparison.InvariantCultureIgnoreCase ) || assemblyName.StartsWith ( "Microsoft.", StringComparison.InvariantCultureIgnoreCase ) || assemblyName.StartsWith ( "Newtonsoft.", StringComparison.InvariantCultureIgnoreCase ) || assemblyName.StartsWith ( "UnityEngine.", StringComparison.InvariantCultureIgnoreCase );
        }

        private static void ReportException ( string harmonyId, Exception e )
        {
            LogError ( harmonyId, e );
            ReflectionTypeLoadException ex;
            if ( ( ex = ( e as ReflectionTypeLoadException ) ) != null )
            {
                LogError ( harmonyId, string.Format ( "Has {0} LoaderExceptions:", ex.LoaderExceptions ) );
                foreach ( var e2 in ex.LoaderExceptions )
                {
                    ReportException ( harmonyId, e2 );
                }
            }
            if ( e.InnerException != null )
            {
                LogError ( harmonyId, "Has InnerException:" );
                ReportException ( harmonyId, e.InnerException );
            }
        }

        private static void Log ( string harmonyId, object message )
        {
            CarbonCore.Format ( $"[{harmonyId}] {message}" );
        }

        private static void LogError ( string harmonyId, object message )
        {
            CarbonCore.ErrorFormat ( $"[{harmonyId}] {message}" );
        }

        private static void LogError ( object message )
        {
            CarbonCore.Error ( message );
        }

        private static string _modPath;

        internal static readonly List<CarbonMod> LoadedMods = new List<CarbonMod> ();

        public class CarbonMod
        {
            public string Name { get; set; }
            public HarmonyInstance Harmony { get; set; }
            public Assembly Assembly { get; set; }
            public Type [] AllTypes { get; set; }
            public List<IHarmonyModHooks> Hooks { get; } = new List<IHarmonyModHooks> ();
            public List<RustPlugin> Plugins { get; set; } = new List<RustPlugin> ();
        }
    }
}
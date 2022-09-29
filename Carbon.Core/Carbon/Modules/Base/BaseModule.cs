///
/// Copyright (c) 2022 Carbon Community 
/// All rights reserved
/// 

using Oxide.Core.Configuration;
using System;
using System.IO;

namespace Carbon.Core.Modules
{
    public class BaseModule<C, D> : IModule
    {
        public DynamicConfigFile File { get; private set; }
        public DynamicConfigFile Data { get; private set; }

        public Configuration ConfigInstance { get; set; }
        public D DataInstance { get; private set; }

        public C Config { get; private set; }

        public virtual string Name => "Not set";

        public void Puts ( object message )
        {
            CarbonCore.Log ( $" [{Name}] {message}" );
        }
        public void PutsError ( object message, Exception exception = null )
        {
            CarbonCore.Error ( $" [{Name}] {message}", exception );
        }

        public virtual void Dispose ()
        {
            File = null;
            ConfigInstance = null;
        }

        public virtual void Init ()
        {
            File = new DynamicConfigFile ( Path.Combine ( CarbonCore.GetModulesFolder (),Name, "config.json" ) );
            Data = new DynamicConfigFile ( Path.Combine ( CarbonCore.GetModulesFolder (), Name, "data.json" ) );

            Load ();
            if ( ConfigInstance.Enabled ) OnEnableStatus ();
        }
        public virtual void InitEnd ()
        {
            Puts ( $"Initialized." );
        }
        public virtual void Load ()
        {
            var shouldSave = false;

            if ( !File.Exists () )
            {
                ConfigInstance = new Configuration { Config = Activator.CreateInstance<C> () };
                shouldSave = true;
            }
            else ConfigInstance = File.ReadObject<Configuration> ();

            if ( !Data.Exists () )
            {
                DataInstance = Activator.CreateInstance<D> ();
                shouldSave = true;
            }
            else DataInstance = Data.ReadObject<D> ();

            if ( shouldSave ) Save ();

            Config = ConfigInstance.Config;
        }
        public virtual void Save ()
        {
            if ( ConfigInstance == null )
            {
                ConfigInstance = new Configuration { Config = Activator.CreateInstance<C> () };
                Config = ConfigInstance.Config;
            }

            if ( DataInstance == null )
            {
                DataInstance = Activator.CreateInstance<D> ();
            }

            File.WriteObject ( ConfigInstance );
            Data.WriteObject ( DataInstance );
        }

        public void SetEnabled ( bool enable )
        {
            if ( ConfigInstance != null )
            {
                ConfigInstance.Enabled = enable;
                OnEnableStatus ();
            }
        }
        public bool GetEnabled ()
        {
            return ConfigInstance.Enabled;
        }

        public virtual void OnDisabled ( bool initialized ) { }
        public virtual void OnEnabled ( bool initialized ) { }

        public void OnEnableStatus ()
        {
            try
            {
                if ( ConfigInstance.Enabled ) OnEnabled ( CarbonCore.IsServerFullyInitialized ); else OnDisabled ( CarbonCore.IsServerFullyInitialized );
            }
            catch ( Exception ex ) { PutsError ( $"Failed {( ConfigInstance.Enabled ? "Enable" : "Disable" )} initialization.", ex ); }
        }

        public class Configuration : IModuleConfig
        {
            public bool Enabled { get; set; }
            public C Config { get; set; }
        }
    }
}
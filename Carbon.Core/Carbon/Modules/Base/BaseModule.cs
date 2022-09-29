///
/// Copyright (c) 2022 Carbon Community 
/// All rights reserved
/// 

using Oxide.Core.Configuration;
using System;
using System.IO;

namespace Carbon.Core.Modules
{
    public class BaseModule<T> : IModule
    {
        public DynamicConfigFile File { get; private set; }
        public Configuration ConfigInstance { get; set; }
        public T Config { get; private set; }

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
            File = new DynamicConfigFile ( Path.Combine ( CarbonCore.GetModulesFolder (), $"{Name}.json" ) );
            Load ();
            OnEnableStatus ();
        }
        public virtual void InitEnd ()
        {
            Puts ( $"Initialized." );
        }
        public virtual void Load ()
        {
            ConfigInstance = File.ReadObject<Configuration> ();
            Config = ConfigInstance.Config;
        }
        public virtual void Save ()
        {
            if ( ConfigInstance == null )
            {
                ConfigInstance = new Configuration { Config = Activator.CreateInstance<T> () };
                Config = ConfigInstance.Config;
            }

            File.WriteObject ( ConfigInstance );
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

        public virtual void OnDisabled () { }
        public virtual void OnEnabled () { }

        public virtual void OnEnabledServerInit () { }
        public virtual void OnDisabledServerInit () { }

        public void OnEnableStatus ()
        {
            try
            {
                if ( CarbonCore.IsServerFullyInitialized )
                {
                    if ( ConfigInstance.Enabled ) OnEnabledServerInit (); else OnDisabledServerInit ();
                }
                else
                {
                    if ( ConfigInstance.Enabled ) OnEnabled (); else OnDisabled ();
                }
            }
            catch ( Exception ex ) { PutsError ( $"Failed {( ConfigInstance.Enabled ? "Enable" : "Disable" )} initialization.", ex ); }
        }

        public class Configuration : IModuleConfig
        {
            public bool Enabled { get; set; }
            public T Config { get; set; }
        }
    }
}
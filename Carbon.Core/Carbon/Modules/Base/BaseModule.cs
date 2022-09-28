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
        public Configuration<T> ConfigInstance { get; private set; }
        public T Config { get; private set; }

        public virtual string Name => "Not set";

        public void Puts ( object message )
        {
            CarbonCore.Log ( $" [{Name}] {message}" );
        }

        public virtual void Dispose ()
        {
            File = null;
            ConfigInstance = null;
        }

        public virtual void Init ()
        {
            File = new DynamicConfigFile ( Path.Combine ( CarbonCore.GetModulesFolder (), $"{Name}.json" ) );
        }
        public virtual void Load ()
        {
            ConfigInstance = File.ReadObject<Configuration<T>> ();
            Config = ConfigInstance.Config;

            if ( ConfigInstance.Enabled ) OnEnabled (); else OnDisabled ();
        }
        public virtual void Save ()
        {
            if ( ConfigInstance == null )
            {
                ConfigInstance = new Configuration<T> { Config = Activator.CreateInstance<T> () };
                Config = ConfigInstance.Config;
            }

            File.WriteObject ( ConfigInstance );
        }

        public virtual void OnDisabled ()
        {

        }
        public virtual void OnEnabled ()
        {

        }
    }

    public class Configuration<T>
    {
        public bool Enabled { get; set; }
        public T Config { get; set; }
    }
}
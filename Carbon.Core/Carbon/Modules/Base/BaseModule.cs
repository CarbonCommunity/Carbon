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
        public DynamicConfigFile Config { get; private set; }
        public Configuration<T> ConfigInstance { get; private set; }

        public virtual string Id => "Not set";

        public virtual void Dispose ()
        {
            Config = null;
            ConfigInstance = null;
        }

        public virtual void Init ()
        {
            Config = new DynamicConfigFile ( Path.Combine ( CarbonCore.GetModulesFolder (), $"{Id}.json" ) );
        }
        public virtual void Load ()
        {
            ConfigInstance = Config.ReadObject<Configuration<T>> ();
        }
        public virtual void Save ()
        {
            Config.WriteObject ( ConfigInstance ?? ( ConfigInstance = new Configuration<T> () ) );
        }
    }

    public class Configuration<T>
    {
        public bool Enabled { get; set; }
        public T Config { get; set; }
    }
}
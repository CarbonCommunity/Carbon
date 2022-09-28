using Oxide.Core.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Carbon.Core.Modules.BaseModule<T>;

namespace Carbon.Core.Modules
{
    public class BaseModule<T> : IModule
    {
        public DynamicConfigFile Config { get; private set; }
        public Configuration<T> ConfigInstance { get; private set; }

        public virtual string Id => "Not set";

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
            Config.WriteObject ( ConfigInstance );
        }
    }

    public class Configuration<T>
    {
        public bool Enabled { get; set; }
        public T Config { get; set; }
    }
}
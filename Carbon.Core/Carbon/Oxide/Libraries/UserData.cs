using Oxide.Plugins;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oxide.Core.Libraries
{
    [ProtoContract ( ImplicitFields = ImplicitFields.AllFields )]
    public class UserData
    {
        public string LastSeenNickname { get; set; } = "Unnamed";

        public HashSet<string> Perms { get; set; } = new HashSet<string> ();
        public HashSet<string> Groups { get; set; } = new HashSet<string> ();
    }
}
///
/// Copyright (c) 2022 Carbon Community 
/// All rights reserved
/// 

using ProtoBuf;
using System.Collections.Generic;

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
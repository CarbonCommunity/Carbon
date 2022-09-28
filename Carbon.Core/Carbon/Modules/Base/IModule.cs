
using System;
///
/// Copyright (c) 2022 Carbon Community 
/// All rights reserved
/// 
namespace Carbon.Core.Modules
{
    public interface IModule : IDisposable
    {
        void Init ();
        void Save ();
        void Load ();
    }
}

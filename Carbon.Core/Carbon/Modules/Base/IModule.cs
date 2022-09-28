
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
        void InitEnd ();
        void Save ();
        void Load ();

        void OnEnableStatus ();

        void OnEnabled ();
        void OnDisabled ();

        void OnEnabledServerInit ();
        void OnDisabledServerInit ();
    }
}

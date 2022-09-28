
using System;
///
/// Copyright (c) 2022 Carbon Community 
/// All rights reserved
/// 
namespace Carbon.Core.Modules
{
    public interface IModule : IDisposable
    {
        string Name { get; }

        void Init ();
        void InitEnd ();
        void Save ();
        void Load ();

        void SetEnabled ( bool enabled );
        bool GetEnabled ();
        void OnEnableStatus ();

        void OnEnabled ();
        void OnDisabled ();

        void OnEnabledServerInit ();
        void OnDisabledServerInit ();
    }
}

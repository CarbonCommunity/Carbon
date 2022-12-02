///
/// Copyright (c) 2022 Carbon Community 
/// All rights reserved
/// 

using System;

namespace Carbon.Base.Interfaces;

public interface IModule : IHookableModule, IDisposable
{
	string Name { get; }

	void Init();
	void InitEnd();
	void Save();
	void Load();

	void SetEnabled(bool enabled);
	bool GetEnabled();
	void OnEnableStatus();

	void OnEnabled(bool initialized);
	void OnDisabled(bool initialized);
}

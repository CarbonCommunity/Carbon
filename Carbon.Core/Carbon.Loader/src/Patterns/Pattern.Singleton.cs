///
/// Copyright (c) 2022 Carbon Community 
/// All rights reserved
/// 
using System;

namespace Carbon.Patterns;

internal abstract class Singleton<T> where T : class
{
	private static readonly Lazy<T> Instance
		= new Lazy<T>(() => NewInstanceOfT());

	internal static T GetInstance()
		=> Instance.Value;

	internal static T NewInstanceOfT()
		=> Activator.CreateInstance(typeof(T), true) as T;
}

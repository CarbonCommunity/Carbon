///
/// Copyright (c) 2022 Carbon Community 
/// All rights reserved
/// 

using System;

namespace Carbon.Patterns
{
	public abstract class Singleton<T> where T : class
	{
		private static readonly Lazy<T> Instance
			= new Lazy<T>(() => NewInstanceOfT());

		public static T GetInstance()
			=> Instance.Value;

		private static T NewInstanceOfT()
			=> Activator.CreateInstance(typeof(T), true) as T;
	}
}
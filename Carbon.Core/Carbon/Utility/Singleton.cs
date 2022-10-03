///
/// Copyright (c) 2022 Carbon Community 
/// All rights reserved
/// 

using System;

namespace Carbon.Patterns
{
	// TODO: Investigate a thread safe disposable lazy based singleton
	// Singleton vs Disposable are usually contradictory patterns but
	// as the context here is an addon to the main application, when 
	// unloading the addon (Carbon) makes sense to dispose all the
	// instances of the singletons instantiated inside the addon ?
	// Or will the GC take care of this for us automatically ?

	/// <summary>
	/// A base class for the singleton design pattern.
	/// </summary>
	/// <typeparam name="T">Class type of the singleton</typeparam>
	public abstract class Singleton<T> where T : class
	{
		/// <summary>
		/// Static instance. Needs to use lambda expression
		/// to construct an instance (since constructor is private).
		/// </summary>
		private static readonly Lazy<T> lazy = new Lazy<T>(() => NewT());

		/// <summary>
		/// Gets the instance of this singleton.
		/// </summary>
		public static T Instance
		{
			get
			{
				return lazy.Value;
			}
		}

		/// <summary>
		/// Creates an instance of T via reflection since T's constructor
		/// is expected to be private.
		/// </summary>
		/// <returns></returns>
		private static T NewT()
			=> Activator.CreateInstance(typeof(T), true) as T;
	}
}
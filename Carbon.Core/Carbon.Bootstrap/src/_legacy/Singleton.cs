using System;
using Utility;

/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * All rights reserved.
 *
 */

namespace Contracts;

public abstract class Singleton<T> where T : class
{
	private static readonly Lazy<T> Instance
		= new(() => CreateInstance());

	public static T GetInstance()
		=> Instance.Value;

	private static T CreateInstance()
	{
		Logger.Debug($"A singleton of {typeof(T)} was created");
		return Activator.CreateInstance(typeof(T), true) as T;
	}
}

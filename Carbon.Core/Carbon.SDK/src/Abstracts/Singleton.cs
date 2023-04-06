using System;

/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * All rights reserved.
 *
 */

namespace API.Abstracts;

public abstract class Singleton<T> where T : class
{
	private static readonly Lazy<T> Instance
		= new(() => CreateInstance());

	public static T GetInstance()
		=> Instance.Value;

	private static T CreateInstance()
		=> Activator.CreateInstance(typeof(T), true) as T;
}

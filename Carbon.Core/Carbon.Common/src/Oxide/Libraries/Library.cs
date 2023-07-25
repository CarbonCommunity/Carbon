/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * All rights reserved.
 *
 */

namespace Oxide.Core.Libraries;

public class Library : IDisposable
{
	public static implicit operator bool(Library library)
	{
		return library != null;
	}
	public static bool operator !(Library library)
	{
		return !library;
	}

	public virtual void Dispose() { }

	public MethodInfo GetFunction(string name)
	{
		return null;
	}
	public PropertyInfo GetProperty(string name)
	{
		return null;
	}

	public IEnumerable<string> GetFunctionNames()
	{
		return default;
	}
	public IEnumerable<string> GetPropertyNames()
	{
		return default;
	}
}

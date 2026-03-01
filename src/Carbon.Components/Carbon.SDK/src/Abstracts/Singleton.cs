using System;

namespace API.Abstracts;

public abstract class Singleton<T> where T : class
{
	private static readonly Lazy<T> Instance = new(() => CreateInstance());

	public static T GetInstance() => Instance.Value;

	private static T CreateInstance() => Activator.CreateInstance(typeof(T), true) as T;
}

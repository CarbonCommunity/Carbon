/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * All rights reserved.
 *
 */

using HarmonyLib;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Reflection;
using System;
using System.Linq;

public static class HarmonyEx
{
	public static List<HarmonyMethod> GetHarmonyMethods(this Type type)
	{
		return (from HarmonyAttribute attr in from attr in type.GetCustomAttributes(inherit: true)
											  where attr is HarmonyAttribute
											  select attr
				select attr.info).ToList();
	}

	public static List<HarmonyMethod> GetHarmonyMethods(this MethodBase method)
	{
		if (method is DynamicMethod)
		{
			return new List<HarmonyMethod>();
		}

		return (from HarmonyAttribute attr in from attr in method.GetCustomAttributes(inherit: true)
											  where attr is HarmonyAttribute
											  select attr
				select attr.info).ToList();
	}
}

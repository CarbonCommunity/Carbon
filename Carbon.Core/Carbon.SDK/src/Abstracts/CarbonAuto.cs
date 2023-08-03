/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * All rights reserved.
 *
 */

namespace API.Abstracts;

public abstract class CarbonAuto
{
	public static CarbonAuto Singleton;

	public virtual void Refresh()
	{

	}
	public virtual bool IsChanged()
	{
		return false;
	}
	public virtual void Save()
	{

	}
	public virtual void Load()
	{

	}
}

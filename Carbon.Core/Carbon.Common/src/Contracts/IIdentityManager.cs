using System;
using API.Events;

/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * All rights reserved.
 *
 */

namespace API.Contracts;

public interface IIdentityManager
{
	public string GetSystemUID
	{ get; }
}
using System;
using System.Linq;
using System.Security.Cryptography;

/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * All rights reserved.
 *
 */

namespace Utility;

public static class Util
{
	public static string SHA1(byte[] raw)
	{
		using SHA1Managed sha1 = new SHA1Managed();
		byte[] bytes = sha1.ComputeHash(raw);
		return string.Concat(bytes.Select(b => b.ToString("x2")));
	}
}
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

internal static class Crypto
{

	public static string md5(byte[] raw)
	{
		if (raw == null || raw.Length == 0) return null;
		using MD5 md5 = MD5.Create();
		byte[] hash = md5.ComputeHash(raw);
		return BitConverter.ToString(hash).Replace("-", "").ToLower();
	}

	public static string sha1(byte[] raw)
	{
		if (raw == null || raw.Length == 0) return null;
		using SHA1Managed sha1 = new SHA1Managed();
		byte[] bytes = sha1.ComputeHash(raw);
		return string.Concat(bytes.Select(b => b.ToString("x2"))).ToLower();
	}
}

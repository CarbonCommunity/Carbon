using System;

/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * All rights reserved.
 *
 */

namespace API.Contracts;

public interface IDownloadManager
{
	public void DownloadAsync(string url, Action<string, byte[]> callback);
}

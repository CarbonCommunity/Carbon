using Facepunch;
using Facepunch.Math;
using JetBrains.Annotations;
using Newtonsoft.Json;

// ReSharper disable once CheckNamespace
namespace Carbon;

public static partial class WebControlPanel
{
	[WebCall]
	[UsedImplicitly]
	private static Response CMD_GetConfigsInfo(ConsoleSystem.Arg arg)
	{
		var configsFolder = Defines.GetConfigsFolder();
		var files = Directory.GetFiles(configsFolder);

		using var list = Pool.Get<PooledList<RConFileInfo>>();
		foreach (var file in files)
		{
			list.Add(new RConFileInfo(new FileInfo(file)));
		}
		return GetResponse(list.ToArray());
	}

	[WebCall]
	[UsedImplicitly]
	private static Response CMD_GetConfigInfo(ConsoleSystem.Arg arg)
	{
		var fileName = arg.GetString(1);

		if (string.IsNullOrWhiteSpace(fileName))
			return GetResponse(new ResponseError(ResponseErrorCodes.InvalidArgs, "Invalid args"));

		var configsFolder = Defines.GetConfigsFolder();

		if (!IsPathSafe(configsFolder, fileName))
			return GetResponse(new ResponseError(ResponseErrorCodes.InvalidArgs, "Invalid path arg"));

		var filePath = Path.Combine(configsFolder, fileName);

		if (!File.Exists(filePath))
			return GetResponse(new ResponseError(ResponseErrorCodes.NoSuchFile, "No such file found"));

		var fileInfo = new FileInfo(filePath);

		return GetResponse(new RConFileInfo(fileInfo));
	}

	[WebCall]
	[UsedImplicitly]
	private static Response CMD_GetConfigContent(ConsoleSystem.Arg arg)
	{
		var fileName = arg.GetString(1);

		if (string.IsNullOrWhiteSpace(fileName))
			return GetResponse(new ResponseError(ResponseErrorCodes.InvalidArgs, "Invalid args"));

		var configsFolder = Defines.GetConfigsFolder();

		if (!IsPathSafe(configsFolder, fileName))
			return GetResponse(new ResponseError(ResponseErrorCodes.InvalidArgs, "Invalid path arg"));

		var filePath = Path.Combine(configsFolder, fileName);

		if (!File.Exists(filePath))
			return GetResponse(new ResponseError(ResponseErrorCodes.NoSuchFile, "No such file found"));

		// todo handle error
		var content = File.ReadAllText(filePath);

		var useGzip = arg.GetString(2, "--gzip") == "--gzip";
		if (!useGzip)
			return GetResponse(content);

		var data = CompressStringToBase64(content);

		return GetResponse(data);
	}

	[WebCall]
	[UsedImplicitly]
	private static Response CMD_SetConfigContent(ConsoleSystem.Arg arg)
	{
		var fileName = arg.GetString(1);

		if (string.IsNullOrWhiteSpace(fileName))
			return GetResponse(new ResponseError(ResponseErrorCodes.InvalidArgs, "Invalid path arg"));

		var configsFolder = Defines.GetConfigsFolder();

		if (!IsPathSafe(configsFolder, fileName))
			return GetResponse(new ResponseError(ResponseErrorCodes.InvalidArgs, "Invalid path arg"));

		var filePath = Path.Combine(configsFolder, fileName);

		var content = arg.GetString(2);

		var useGzip = arg.GetString(3, "--gzip") == "--gzip";
		if (!useGzip)
		{
			File.WriteAllText(filePath, content);
			return GetResponse(Ok);
		}

		var data = DecompressBase64ToString(content);
		File.WriteAllText(filePath, data);

		return GetResponse(Ok);
	}

	private static bool IsPathSafe(string basePath, string secondPath)
	{
		try
		{
			var normalizedBasePath = Path.GetFullPath(basePath).TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
			var combinedPath = Path.Combine(normalizedBasePath, secondPath);
			var resolvedPath = Path.GetFullPath(combinedPath);

			if (string.IsNullOrEmpty(resolvedPath))
				return false;

			if (resolvedPath.StartsWith(normalizedBasePath + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase))
				return true;

			if (resolvedPath.StartsWith(normalizedBasePath + Path.AltDirectorySeparatorChar, StringComparison.OrdinalIgnoreCase))
				return true;
		}
		catch (ArgumentException)
		{
			return false;
		}

		return false;
	}

	private struct RConFileInfo(FileInfo fileInfo)
	{
		[JsonProperty] public string Name = fileInfo.Name;
		[JsonProperty] public int ModEpoch = Epoch.FromDateTime(fileInfo.LastWriteTimeUtc);
		[JsonProperty] public long Size = fileInfo.Length;
	}
}

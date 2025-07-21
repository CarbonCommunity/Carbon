using Facepunch;
using Facepunch.Math;
using JetBrains.Annotations;
using Newtonsoft.Json;

// ReSharper disable once CheckNamespace
namespace Carbon.Documentation;

public static partial class WebRCon
{
	[DocsRpc]
	[UsedImplicitly]
	private static DocsRpcResponse GetConfigsInfo(ConsoleSystem.Arg arg)
	{
		var configsFolder = Defines.GetConfigsFolder();

		var files = Directory.GetFiles(configsFolder);

		using var list = Pool.Get<PooledList<RConFileInfo>>();

		foreach (var file in files)
		{
			var fileInfo = new FileInfo(file);

			list.Add(new RConFileInfo(fileInfo));
		}

		return Response(list.ToArray());
	}

	[DocsRpc]
	[UsedImplicitly]
	private static DocsRpcResponse GetConfigInfo(ConsoleSystem.Arg arg)
	{
		var fileName = arg.GetString(1);

		if (string.IsNullOrWhiteSpace(fileName))
			return Response(new RConError(RConErrorEnum.InvalidArgs, "Invalid args"));

		var configsFolder = Defines.GetConfigsFolder();

		// todo prevent path traversal
		var filePath = Path.Combine(configsFolder, fileName);

		if (!File.Exists(filePath))
			return Response(new RConError(RConErrorEnum.NoSuchFile, "No such file found"));

		var fileInfo = new FileInfo(filePath);

		return Response(new RConFileInfo(fileInfo));
	}

	[DocsRpc]
	[UsedImplicitly]
	private static DocsRpcResponse GetConfigContent(ConsoleSystem.Arg arg)
	{
		var fileName = arg.GetString(1);

		if (string.IsNullOrWhiteSpace(fileName))
			return Response(new RConError(RConErrorEnum.InvalidArgs, "Invalid args"));

		var configsFolder = Defines.GetConfigsFolder();

		// todo prevent path traversal
		var filePath = Path.Combine(configsFolder, fileName);

		if (!File.Exists(filePath))
			return Response(new RConError(RConErrorEnum.NoSuchFile, "No such file found"));

		// todo handle error
		var content = File.ReadAllText(filePath);

		var useGzip = arg.GetString(2, "--gzip") == "--gzip";
		if (!useGzip)
			return Response(content);

		var data = CompressStringToBase64(content);

		return Response(data);
	}


	[DocsRpc]
	[UsedImplicitly]
	private static DocsRpcResponse SetConfigContent(ConsoleSystem.Arg arg)
	{
		var fileName = arg.GetString(1);

		if (string.IsNullOrWhiteSpace(fileName))
			return Response(new RConError(RConErrorEnum.InvalidArgs, "Invalid args"));

		var configsFolder = Defines.GetConfigsFolder();

		var filePath = Path.Combine(configsFolder, fileName);

		var content = arg.GetString(2);

		var useGzip = arg.GetString(3, "--gzip") == "--gzip";
		if (!useGzip)
		{
			File.WriteAllText(filePath, content);
			return Response(Ok);
		}

		var data = DecompressBase64ToString(content);
		File.WriteAllText(filePath, data);

		return Response(Ok);
	}

	private struct RConFileInfo
	{
		[JsonProperty] public string Name;
		[JsonProperty] public int ModEpoch;
		[JsonProperty] public long Size;

		public RConFileInfo(FileInfo fileInfo)
		{
			Name = fileInfo.Name;
			ModEpoch = Epoch.FromDateTime(fileInfo.LastWriteTimeUtc);
			Size = fileInfo.Length;
		}
	}
}

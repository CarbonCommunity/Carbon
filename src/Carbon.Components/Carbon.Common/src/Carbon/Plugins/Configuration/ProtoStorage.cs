using ProtoBuf;
using Logger = Carbon.Logger;

namespace Carbon.Plugins;

/// <summary>
/// Protobuf-serialized data files under carbon/data (*.data).
/// </summary>
public class ProtoStorage
{
	/// <summary>Converts a JSON data file with the same name into protobuf storage, once.</summary>
	public static void DatafileToProto<T>(string name, bool deleteAfter = true)
	{
		var dataFileSystem = Community.Runtime.DataFileSystem;

		if (!dataFileSystem.ExistsDatafile(name))
		{
			return;
		}

		if (Exists(name))
		{
			Logger.Warn($"Failed to import JSON file: {name} already exists.");
			return;
		}

		try
		{
			Save(dataFileSystem.ReadObject<T>(name), name);

			if (deleteAfter)
			{
				File.Delete(dataFileSystem.GetFile(name).Filename);
			}
		}
		catch (Exception ex)
		{
			Logger.Error("Failed to convert datafile to proto storage: " + name, ex);
		}
	}

	public static IEnumerable<string> GetFiles(string subDirectory)
	{
		var fileDataPath = GetFileDataPath(subDirectory.Replace("..", ""));

		if (!Directory.Exists(fileDataPath))
		{
			yield break;
		}

		foreach (string value in Directory.GetFiles(fileDataPath, "*.data"))
		{
			yield return Path.GetFileNameWithoutExtension(value);
		}
	}

	public static T Load<T>(params string[] subPaths)
	{
		var fileName = GetFileName(subPaths);
		var fileDataPath = GetFileDataPath(fileName);

		try
		{
			if (File.Exists(fileDataPath))
			{
				using var fileStream = new MemoryStream(OsEx.File.ReadBytes(fileDataPath));
				return Serializer.Deserialize<T>(fileStream);
			}
		}
		catch (Exception ex)
		{
			Logger.Error("Failed to load protobuf data from " + fileName, ex);
		}

		return default;
	}

	public static void Save<T>(T data, params string[] subPaths)
	{
		var fileName = GetFileName(subPaths);
		var fileDataPath = GetFileDataPath(fileName);
		var directoryName = Path.GetDirectoryName(fileDataPath);

		try
		{
			if (directoryName != null && !Directory.Exists(directoryName))
			{
				Directory.CreateDirectory(directoryName);
			}

			using var stream = new MemoryStream();
			Serializer.Serialize(stream, data);
			OsEx.File.Create(fileDataPath, stream.ToArray());
		}
		catch (Exception ex)
		{
			Logger.Error("Failed to save protobuf data to " + fileName, ex);
		}
	}

	public static bool Exists(params string[] subPaths)
	{
		return File.Exists(GetFileDataPath(GetFileName(subPaths)));
	}

	public static string GetFileName(params string[] subPaths)
	{
		return string.Join(Path.DirectorySeparatorChar.ToString(), subPaths).Replace("..", "") + ".data";
	}

	public static string GetFileDataPath(string name)
	{
		return Path.Combine(Defines.GetDataFolder(), name);
	}
}

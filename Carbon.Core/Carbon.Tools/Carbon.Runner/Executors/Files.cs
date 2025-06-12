using System.Security.Cryptography;

namespace Carbon.Runner.Executors;

public class Files : Executor
{
	public override string? Name => "Files";

	[Expose("Gets a list of all files in a directory")]
	public string[] Get(string folder, string search = "*") => Directory.GetFiles(folder, search);

	[Expose("Create a file with text inside of it")]
	public void Create(string target, string content) => File.WriteAllText(target, content);

	[Expose("Copies a file if the file exists")]
	public void Copy(string target, string destination)
	{
		if (!File.Exists(target))
		{
			Error($"File '{target}' not found!");
			return;
		}

		if (Directory.Exists(destination))
		{
			destination = Path.Combine(destination, Path.GetFileName(target));
		}

		var alreadyExisted = File.Exists(destination);
		File.Copy(target, destination, true);
		Warn($"{(alreadyExisted ? "Overwrote" : "Copied")} file to '{destination}'");
	}

	[Expose("Checks if a file exists")]
	public bool Exists(string target) => File.Exists(target);

	[Expose("Deletes a file if the file exists")]
	public void Delete(string target)
	{
		if (!File.Exists(target))
		{
			Log($"File '{target}' not found. Skipping..");
			return;
		}

		File.Delete(target);
		Warn($"Deleted file: '{target}'");
	}

	[Expose("Deletes all files in a folder that contain a string in their name")]
	public void DeleteContains(string folder, string contains)
	{
		if (!Directory.Exists(folder))
		{
			Log($"Folder '{folder}' not found. Skipping..");
			return;
		}

		var files = Get(folder);
		foreach (var file in files)
		{
			if (Path.GetFileNameWithoutExtension(file).Contains(contains, StringComparison.CurrentCultureIgnoreCase))
			{
				Delete(file);
			}
		}
	}

	[Expose("Gets the hash of a file")]
	public uint Hash(string fileName)
	{
		if (!File.Exists(fileName))
		{
			Error($"File not found: {fileName}");
			return 0;
		}

		return BitConverter.ToUInt32(new MD5CryptoServiceProvider().ComputeHash(File.ReadAllBytes(fileName)), 0);
	}
}

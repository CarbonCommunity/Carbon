using System.Formats.Tar;
using System.IO.Compression;

namespace Carbon.Runner.Executors;

public class Archive : Executor
{
	public override string Name => "Archive";

	[Expose("Creates a new ZIP file of a directory")]
	public void Zip(string directory, string destination)
	{
		if (File.Exists(destination))
		{
			File.Delete(destination);
		}
		ZipFile.CreateFromDirectory(directory, destination, CompressionLevel.Optimal, false);
		Warn($"Created ZIP file: {destination}");
	}

	[Expose("Creates a new TAR file of a directory")]
	public void Tar(string directory, string destination)
	{
		if (File.Exists(destination))
		{
			File.Delete(destination);
		}

		var temp = InternalRunner.Path(destination + ".tmp");

		TarFile.CreateFromDirectory(directory, temp, false);

		using (var originalFileStream = new FileStream(temp, FileMode.Open, FileAccess.Read))
		using (var compressedFileStream = new FileStream(destination, FileMode.Create))
		using (var gzipStream = new GZipStream(compressedFileStream, CompressionLevel.Optimal))
		{
			originalFileStream.CopyTo(gzipStream);
		}

		Warn($"Created TAR file: {destination}");
	}
}

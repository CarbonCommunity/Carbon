using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using Xunit;

namespace Carbon.UnitTests;

public class UtilsTests : IDisposable
{
	private readonly string _tempRoot;

	public UtilsTests()
	{
		_tempRoot = Path.Combine(Path.GetTempPath(), $"CarbonUnitTests_{Guid.NewGuid():N}");
		Directory.CreateDirectory(_tempRoot);
	}

	public void Dispose()
	{
		if (Directory.Exists(_tempRoot))
		{
			Directory.Delete(_tempRoot, true);
		}
	}

	// ── Copy ──────────────────────────────────────────────────────────────────

	[Fact]
	public void Copy_EmptySourcePath_ThrowsException()
	{
		Assert.Throws<Exception>(() => Tests.Utils.Copy("", _tempRoot));
	}

	[Fact]
	public void Copy_NullSourcePath_ThrowsException()
	{
		Assert.Throws<Exception>(() => Tests.Utils.Copy(null!, _tempRoot));
	}

	[Fact]
	public void Copy_EmptyDestinationPath_ThrowsException()
	{
		Assert.Throws<Exception>(() => Tests.Utils.Copy(_tempRoot, ""));
	}

	[Fact]
	public void Copy_NullDestinationPath_ThrowsException()
	{
		Assert.Throws<Exception>(() => Tests.Utils.Copy(_tempRoot, null!));
	}

	[Fact]
	public void Copy_NonExistentSourceDirectory_ThrowsDirectoryNotFoundException()
	{
		var nonExistent = Path.Combine(_tempRoot, "does_not_exist");
		Assert.Throws<DirectoryNotFoundException>(() => Tests.Utils.Copy(nonExistent, _tempRoot));
	}

	[Fact]
	public void Copy_EmptyDirectory_CreatesDestinationAndReturnsMappings()
	{
		var src = Path.Combine(_tempRoot, "src_empty");
		var dst = Path.Combine(_tempRoot, "dst_empty");
		Directory.CreateDirectory(src);

		var result = Tests.Utils.Copy(src, dst);

		Assert.True(Directory.Exists(dst));
		Assert.Single(result); // only the folder itself
		Assert.True(result.ContainsValue(dst));
	}

	[Fact]
	public void Copy_DirectoryWithFiles_CopiesAllFiles()
	{
		var src = Path.Combine(_tempRoot, "src_files");
		var dst = Path.Combine(_tempRoot, "dst_files");
		Directory.CreateDirectory(src);

		const string fileName = "hello.txt";
		const string fileContent = "Hello, Carbon!";
		File.WriteAllText(Path.Combine(src, fileName), fileContent);

		var result = Tests.Utils.Copy(src, dst);

		var dstFile = Path.Combine(dst, fileName);
		Assert.True(File.Exists(dstFile));
		Assert.Equal(fileContent, File.ReadAllText(dstFile));
		// result should contain: the folder itself + the file
		Assert.Equal(2, result.Count);
	}

	[Fact]
	public void Copy_DirectoryWithSubdirectories_CopiesRecursively()
	{
		var src = Path.Combine(_tempRoot, "src_sub");
		var dst = Path.Combine(_tempRoot, "dst_sub");
		var subDir = Path.Combine(src, "sub");
		Directory.CreateDirectory(subDir);

		const string fileName = "deep.txt";
		File.WriteAllText(Path.Combine(subDir, fileName), "deep content");

		Tests.Utils.Copy(src, dst);

		Assert.True(File.Exists(Path.Combine(dst, "sub", fileName)));
	}

	[Fact]
	public void Copy_MultipleFiles_ReturnsMappingForEachFile()
	{
		var src = Path.Combine(_tempRoot, "src_multi");
		var dst = Path.Combine(_tempRoot, "dst_multi");
		Directory.CreateDirectory(src);

		File.WriteAllText(Path.Combine(src, "a.txt"), "a");
		File.WriteAllText(Path.Combine(src, "b.txt"), "b");
		File.WriteAllText(Path.Combine(src, "c.txt"), "c");

		var result = Tests.Utils.Copy(src, dst);

		// 1 folder + 3 files
		Assert.Equal(4, result.Count);
		Assert.True(File.Exists(Path.Combine(dst, "a.txt")));
		Assert.True(File.Exists(Path.Combine(dst, "b.txt")));
		Assert.True(File.Exists(Path.Combine(dst, "c.txt")));
	}

	[Fact]
	public void Copy_Overwrite_ReplacesExistingFiles()
	{
		var src = Path.Combine(_tempRoot, "src_overwrite");
		var dst = Path.Combine(_tempRoot, "dst_overwrite");
		Directory.CreateDirectory(src);
		Directory.CreateDirectory(dst);

		const string fileName = "file.txt";
		File.WriteAllText(Path.Combine(src, fileName), "new content");
		File.WriteAllText(Path.Combine(dst, fileName), "old content");

		Tests.Utils.Copy(src, dst, overwrite: true);

		Assert.Equal("new content", File.ReadAllText(Path.Combine(dst, fileName)));
	}

	// ── MakeExecutableExecutable ──────────────────────────────────────────────

	[Fact]
	public void MakeExecutableExecutable_ExistingFile_DoesNotThrow()
	{
		var file = Path.Combine(_tempRoot, "test_exec.sh");
		File.WriteAllText(file, "#!/bin/sh");

		// Should not throw on any platform
		var ex = Record.Exception(() => Tests.Utils.MakeExecutableExecutable(file));
		Assert.Null(ex);
	}

	[SkippableFact]
	public void MakeExecutableExecutable_OnLinux_SetsUserExecuteBit()
	{
		Skip.IfNot(RuntimeInformation.IsOSPlatform(OSPlatform.Linux) || RuntimeInformation.IsOSPlatform(OSPlatform.OSX),
			"Only runs on Linux/macOS");

		var file = Path.Combine(_tempRoot, "exec_linux.sh");
		File.WriteAllText(file, "#!/bin/sh");

		Tests.Utils.MakeExecutableExecutable(file);

#pragma warning disable CA1416 // Platform compatibility – guarded by Skip.IfNot above
		var mode = File.GetUnixFileMode(file);
#pragma warning restore CA1416
		Assert.True((mode & UnixFileMode.UserExecute) != 0, "UserExecute bit should be set");
		Assert.True((mode & UnixFileMode.UserRead) != 0, "UserRead bit should be set");
		Assert.True((mode & UnixFileMode.UserWrite) != 0, "UserWrite bit should be set");
	}
}

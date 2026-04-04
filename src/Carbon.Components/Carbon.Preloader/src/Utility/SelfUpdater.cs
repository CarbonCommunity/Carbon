using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.InteropServices;
using Carbon.Core;
using Carbon.Extensions;
using Mono.Cecil;

namespace Doorstop.Utility;

public static class SelfUpdater
{
	private const string Repository = "CarbonCommunity/Carbon";
	private const string CarbonVersionsEndpoint = "https://api.carbonmod.gg/releases";

	private static OsType Platform;
	private static ReleaseType Release;
	private static string Target;
	private static bool IsMinimal;
	private static Version LocalCarbonProtocol;
	private static Version LocalRustProtocol;
	private static readonly string[] Files =
	[
		"carbon/managed",
		"carbon/native"
	];
	private static string Tag => Release switch
	{
		ReleaseType.Edge => "edge_build",
		ReleaseType.Preview => "preview_build",
		ReleaseType.RustRelease => "rustbeta_release_build",
		ReleaseType.RustStaging => "rustbeta_staging_build",
		ReleaseType.RustAux01 => "rustbeta_aux01_build",
		ReleaseType.RustAux02 => "rustbeta_aux02_build",
		ReleaseType.RustAux03 => "rustbeta_aux03_build",
		ReleaseType.Production => "production_build",
		ReleaseType.QA => "qa_build",
		_ => throw new ArgumentOutOfRangeException()
	};
	private static string File => Platform switch
	{
		OsType.Windows => $"Carbon.Windows.{Target}.zip",
		OsType.Linux => $"Carbon.Linux.{Target}.tar.gz",
		_ => throw new ArgumentOutOfRangeException()
	};
	private static string LocalProtocolFile => Path.Combine(Defines.GetRootFolder(), ".protocol");

	private enum OsType { Windows, Linux }
	private enum ReleaseType { Edge, Preview, RustRelease, RustStaging, RustAux01, RustAux02, RustAux03, Production, QA }

	internal static void Init()
	{
		Platform = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) switch
		{
			true => OsType.Windows,
			false => OsType.Linux
		};

		Release =
#if PROD
		ReleaseType.Production;
#elif PREVIEW
		ReleaseType.Preview;
#elif RUST_STAGING
		ReleaseType.RustStaging;
#elif RUST_RELEASE
		ReleaseType.RustRelease;
#elif RUST_AUX01
		ReleaseType.RustAux01;
#elif RUST_AUX02
		ReleaseType.RustAux02;
#elif RUST_AUX03
		ReleaseType.RustAux03;
#elif QA
		ReleaseType.QA;
#else
		ReleaseType.Edge;
#endif

		IsMinimal =
#if MINIMAL
			true;
#else
			false;
#endif

		Target = IsMinimal ? "Minimal" :
#if DEBUG
		"Debug";
#else
		"Release";
#endif

		if (System.IO.File.Exists(LocalProtocolFile))
		{
			var lines = System.IO.File.ReadAllLines(LocalProtocolFile);
			if (lines.Length >= 1 && Version.TryParse(lines[0], out var rustProtocol))
			{
				LocalRustProtocol = rustProtocol;
			}
			if (lines.Length >= 2 && Version.TryParse(lines[1], out var carbonProtocol))
			{
				LocalCarbonProtocol = carbonProtocol;
			}
		}
	}

	internal static void Execute()
	{
		var versionOverride = GetVersionOverride();
		var hasVersionOverride = !string.IsNullOrEmpty(versionOverride);
		var version = Versions.GetVersion(Tag);

		if (version == null || string.IsNullOrEmpty(version.Version))
		{
			return;
		}

		if (!hasVersionOverride && version.Version.Equals(Versions.CurrentVersion))
		{
			Logger.Log($" Carbon {Target} is up to date, no self-updating necessary. Running {Release} build [{Versions.CurrentVersion}] on tag '{Tag}'.");
			return;
		}

#if PROD
		var currentRustProtocol = string.Empty;
		if (!hasVersionOverride && !HasValidLocalProtocol(version, out currentRustProtocol))
		{
			Logger.Log($" Skipped self-updating since the pending Carbon update has changed its protocol. Update the Rust server to self-update!");
			return;
		}
#endif

		var url = versionOverride ?? GithubReleaseUrl();

		if (hasVersionOverride)
		{
			Logger.Log($" Carbon version override detected and now self-updating - {Release} [{Tag}] on {Platform} [{url}]");
		}
		else
		{
			Logger.Log($" Carbon {Target} is out of date and now self-updating - {Release} [{Tag}] on {Platform} [{Versions.CurrentVersion} -> {version.Version}]");
		}

#if PROD
		if (!hasVersionOverride)
		{
			WriteLocalProtocol(currentRustProtocol, version.Protocol);
		}
#endif

		OsEx.ExecuteProcess("curl", $"-s -H \"Cache-Control: no-store, no-cache, must-revalidate, max-age=0\" -H \"Pragma: no-cache\" -fSL -o \"{Path.Combine(Defines.GetTempFolder(), "patch.zip")}\" \"{url}\"");

		var count = 0;

		try
		{
			var patchPath = Path.Combine(Defines.GetTempFolder(), "patch.zip");
			var carbonRoot = Defines.GetRootFolder();

			Console.Write(" Updating Carbon... ");

#if UNIX

			var archive = new TarGzReader(patchPath);
			foreach (var entry in archive.Entries)
			{
				if (string.IsNullOrEmpty(entry.Name) || !Files.Any(x => entry.Name.Contains(x)))
				{
					continue;
				}

				var relativeFilePath = entry.Name.Replace("carbon/", string.Empty).Replace("carbon\\", string.Empty);
				var destination = Path.Combine(carbonRoot, relativeFilePath);
				var destDir = Path.GetDirectoryName(destination);
				if (!string.IsNullOrEmpty(destDir))
				{
					Directory.CreateDirectory(destDir);
				}

				try
				{
					using (var fileStream = new FileStream(destination, FileMode.Create, FileAccess.Write, FileShare.None))
					using (var entryStream = entry.Open())
					{
						entryStream.CopyTo(fileStream);
					}
					Console.Write($"{Environment.NewLine} - {relativeFilePath} ({entry.Size.Format().ToUpper()})");
				}
				catch
				{
					Console.Write($"{Environment.NewLine} File used by another process, skipping '{relativeFilePath}' ({entry.Size.Format().ToUpper()})");
				}

				count++;
			}

#else

			using var archive = ZipFile.OpenRead(patchPath);
			foreach (var entry in archive.Entries)
			{
				if (string.IsNullOrEmpty(entry.Name) || !Files.Any(x => entry.FullName.Contains(x)))
				{
					continue;
				}

				var relativeFilePath = entry.FullName.Replace("carbon/", string.Empty).Replace("carbon\\", string.Empty);
				var destination = Path.Combine(carbonRoot, relativeFilePath);
				var destDir = Path.GetDirectoryName(destination);
				if (!string.IsNullOrEmpty(destDir))
				{
					Directory.CreateDirectory(destDir);
				}

				try
				{
					using (var fileStream = new FileStream(destination, FileMode.Create, FileAccess.Write, FileShare.None))
					using (var entryStream = entry.Open())
					{
						entryStream.CopyTo(fileStream);
					}
					Console.Write($"{Environment.NewLine} - {relativeFilePath} ({entry.Length.Format().ToUpper()})");
				}
				catch
				{
					Console.Write($"{Environment.NewLine} File used by another process, skipping '{relativeFilePath}' ({entry.Length.Format().ToUpper()})");
				}

				count++;
			}

#endif

			Console.WriteLine(string.Empty);
		}
		catch (Exception e)
		{
			Logger.Error($"Error while updating 'Carbon [{Platform}]'", e);
		}

		if (hasVersionOverride)
		{
			Logger.Log($" Carbon finished self-updating the custom version override with {count:n0} files. You're now running the latest build.");
		}
		else
		{
			Logger.Log($" Carbon {Target} finished self-updating {count:n0} files. You're now running the latest {Release} build.");
		}
	}

	internal static bool HasValidLocalProtocol(Versions.VersionValue version, out string currentRustProtocol)
	{
		using var rustGlobal = new MemoryStream(System.IO.File.ReadAllBytes(Path.Combine(Defines.GetRustManagedFolder(), "Rust.Global.dll")));
		var rustGlobalAssembly = AssemblyDefinition.ReadAssembly(rustGlobal);
		var protocol = rustGlobalAssembly.MainModule.GetType("Rust.Protocol");
		var networkProtocol = protocol.Fields.FirstOrDefault(x => x.Name == "network").Constant;
		var saveProtocol = protocol.Fields.FirstOrDefault(x => x.Name == "save").Constant;
		var reportProtocol = protocol.Fields.FirstOrDefault(x => x.Name == "report").Constant;
		currentRustProtocol = $"{networkProtocol}.{saveProtocol}.{reportProtocol}";
		var rustProtocol = Version.Parse(currentRustProtocol);
		rustGlobalAssembly.Dispose();
		rustGlobalAssembly = null;

		if (LocalCarbonProtocol == null || LocalRustProtocol == null)
		{
			return true;
		}
		if (!Version.TryParse(version.Protocol, out var carbonUpdateProtocol))
		{
			return true;
		}
		// If the Carbon update has a higher revision, assume it's a mandatory Carbon update regardless of protocol
		if (LocalCarbonProtocol.Revision < carbonUpdateProtocol.Revision)
		{
			return true;
		}

		// Rust protocol changed - meaning Rust has updated its protocol since the last boot time
		if (LocalRustProtocol != rustProtocol)
		{
			return true;
		}

		// Carbon protocol is the same, allow the update to go through
		if (LocalCarbonProtocol == carbonUpdateProtocol)
		{
			return true;
		}

		// Don't update Carbon if the pending Carbon update protocol changed and the Rust server hasn't updated yet
		return false;
	}

	internal static void WriteLocalProtocol(string rustProtocol, string carbonProtocol)
	{
		System.IO.File.WriteAllText(LocalProtocolFile, $"{rustProtocol}\n{carbonProtocol}");
	}

	internal static bool GetCarbonVersions()
	{
		var tempPath = Path.Combine(Defines.GetTempFolder(), "versions.json");
		var gotVersions = OsEx.ExecuteProcess("curl", $"-s -H \"Cache-Control: no-store, no-cache, must-revalidate, max-age=0\" -H \"Pragma: no-cache\" -fSL -o \"{tempPath}\" \"{CarbonVersionsEndpoint}\"");

		return gotVersions && Versions.Init(System.IO.File.ReadAllText(tempPath));
	}

	internal static string GithubReleaseUrl()
	{
		return $"http://github.com/{Repository}/releases/download/{Tag}/{File}";
	}

	internal static string GetVersionOverride()
	{
		var path = Path.Combine(Defines.GetTempFolder(), "versionoverride.txt");
		if (System.IO.File.Exists(path))
		{
			var text = System.IO.File.ReadAllText(path);
			System.IO.File.Delete(path);
			return text;
		}
		if (!string.IsNullOrEmpty(Config.Singleton.SelfUpdating.RedirectUri))
		{
			return Config.Singleton.SelfUpdating.RedirectUri;
		}
		return null;
	}
}

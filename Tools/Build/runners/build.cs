var target = GetArg(1, "Debug");
var defines = GetArg(2);
var tag = GetArg(3, "edge_build");
var version = GetVariable("VERSION");
var cargoTarget = target.Equals("Debug") || target.Equals("DebugUnix") || target.Equals("Minimal") || target.Equals("MinimalUnix") ? "release" : "prod";
var isUnix = target.Contains("Unix");
var noArchive = HasArg("-noarchive");

Run(Path(Home, "Tools", "Build", "runners", "git.cs"), tag);

Warn($"Tag: {tag}");
Warn($"Target: {target}");
Warn($"Defines: {defines ?? "N/A"}");
Warn($"Version: {version ?? "N/A"}");
Warn($"Cargo Target: {cargoTarget}");

Directories.Delete(Path(Home, "Release", ".tmp", target));
Files.Delete(Path(Home, "Release", $"Carbon.{target}.tar.gz"));

DotNet.ExitOnError(true);
DotNet.Run("restore", PathEnquotes(Home, "Carbon.Core"));
DotNet.Run("clean", PathEnquotes(Home, "Carbon.Core"), "--configuration", target);
DotNet.Run("build", PathEnquotes(Home, "Carbon.Core"), "--configuration", target, "--no-restore",
	$"/p:UserConstants=\"{defines}\"", $"/p:UserVersion=\"{version}\"");

Files.Copy(Path(Home, "Tools", "Helpers", "Carbon.targets"), Path(Home, "Release", ".tmp", target, "Carbon.targets"));

var tos = isUnix ? "Linux" : "Windows";
var finalTarget = target.Replace("Unix", string.Empty);

Files.DeleteContains(Path(Home, "Release", ".tmp", target, "carbon", "managed", "lib"), "carbon");
Directories.Delete(Path(Home, "Release", ".tmp", target, "profiler"));

if (isUnix)
{
	Files.Copy(Path(Home, "Tools", "Helpers", "carbon.sh"), Path(Home, "Release", ".tmp", target));
	Files.Copy(Path(Home, "Tools", "Helpers", "environment.sh"), Path(Home, "Release", ".tmp", target, "carbon", "tools"));
	Files.Copy(Path(Home, "Tools", "UnityDoorstop", "linux", "x64", "libdoorstop.so"), Path(Home, "Release", ".tmp", target));
	Files.Copy(Path(Home, "Carbon.Core", "Carbon.Native", "target", "x86_64-unknown-linux-gnu", cargoTarget, "libCarbonNative.so"), Path(Home, "Release", ".tmp", target, "carbon", "native"), optional: true);

	if (!noArchive)
	{
		Archive.Tar(Path(Home, "Release", ".tmp", target), Path(Home, "Release", $"Carbon.{tos}.{finalTarget}.tar.gz"));
	}
}
else
{
	Files.Copy(Path(Home, "Tools", "Helpers", "doorstop_config.ini"), Path(Home, "Release", ".tmp", target));
	Files.Copy(Path(Home, "Tools", "UnityDoorstop", "windows", "x64", "doorstop.dll"), Path(Home, "Release", ".tmp", target, "winhttp.dll"));
	Files.Copy(Path(Home, "Carbon.Core", "Carbon.Native", "target", "x86_64-pc-windows-gnu", cargoTarget, "CarbonNative.dll"), Path(Home, "Release", ".tmp", target, "carbon", "native"), optional: true);

	if (!noArchive)
	{
		Archive.Zip(Path(Home, "Release", ".tmp", target), Path(Home, "Release", $"Carbon.{tos}.{finalTarget}.zip"));
	}
}
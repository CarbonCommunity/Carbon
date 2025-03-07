var target = GetArg(1, "Debug");
var defines = GetArg(2);
var version = GetVariable("VERSION");
var cargoTarget = target.Equals("Debug") || target.Equals("DebugUnix") || target.Equals("Minimal") || target.Equals("MinimalUnix") ? "release" : "prod";
var isUnix = target.Contains("Unix");

DotNet.ExitOnError(true);
DotNet.Run("restore", PathEnquotes(Home, "Carbon.Core"));
DotNet.Run("clean", PathEnquotes(Home, "Carbon.Core"), "--configuration", target);
DotNet.Run("build", PathEnquotes(Home, "Carbon.Core"), "--configuration", target, "--no-restore",
	$"/p:UserConstants=\"{defines}\"", $"/p:UserVersion=\"{version}\"");

var tos = isUnix ? "Linux" : "Windows";

if(isUnix)
{
	Files.Copy(Path(Home, "Carbon.Core", "Carbon.Native", "target", "x86_64-unknown-linux-gnu", cargoTarget, "libCarbonNative.so"), Path(Home, "Release", ".tmp", target, "profiler", "native"));
	Archive.Tar(Path(Home, "Release", ".tmp", target, "profiler"), Path(Home, "Release", $"Carbon.{tos}.Profiler.tar.gz"));
}
else
{
	Files.Copy(Path(Home, "Carbon.Core", "Carbon.Native", "target", "x86_64-pc-windows-gnu", cargoTarget, "CarbonNative.dll"), Path(Home, "Release", ".tmp", target, "profiler", "native"));
	Archive.Zip(Path(Home, "Release", ".tmp", target, "profiler"), Path(Home, "Release", $"Carbon.{tos}.Profiler.zip"));
}


var key = GetArg(1);
var version = GetArg(2);

Warn($"Key: {key.Length}");
Warn($"Version: {version}");

DotNet.WorkingDirectory(Path(Home, "src"));
DotNet.Run("pack", "-o", PathEnquotes(Home, "src", ".nugets"), 
	"-c", "Release", $"/p:PackageVersion={version}", "--no-build");

DotNet.SetQuiet(true);
DotNet.Run("nuget", "push", PathEnquotes(Home, "src", ".nugets", $"Carbon.Community.{version}.nupkg"),
	"--api-key", key, "--source", "https://api.nuget.org/v3/index.json");

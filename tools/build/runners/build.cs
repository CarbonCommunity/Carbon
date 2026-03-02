var target = GetArg(1, "Debug");
var defines = GetArg(2);
var tag = GetArg(3, "edge_build");
var version = GetVariable("VERSION");
var cargoTarget = target.Equals("Debug") || target.Equals("DebugUnix") || target.Equals("Minimal") || target.Equals("MinimalUnix") ? "release" : "prod";
var isUnix = target.Contains("Unix");
var noArchive = HasArg("-noarchive");

var noClean = !HasArg("-clean");
var noRestore = HasArg("-norestore");
if (HasArg("-restore"))
{
	noRestore = false;
}
var buildVerbosity = "minimal";

System.IO.File.WriteAllText(Path(Home, "src", "Carbon.Components", "Carbon.Common", "src", "Carbon", "Build.cs"),
	System.IO.File.ReadAllText(Path(Home, "src", "Carbon.Components", "Carbon.Common", "src", "Carbon", "Build.cs.template"))
		.Replace("[GIT_BRANCH]", Git.RunOutput("branch", "--show-current").Trim())
		.Replace("[GIT_AUTHOR]", Git.RunOutput("show", "-s", "--format=\"%an\"", "HEAD").Trim())
		.Replace("[GIT_COMMENT]", Git.RunOutput("log -1", "--pretty=\"%B\"", "HEAD").Trim())
		.Replace("[GIT_DATE]", Git.RunOutput("log -1", "--format=\"%ci\"", "HEAD").Trim())
		.Replace("[GIT_TAG]", string.IsNullOrEmpty(tag) ? Git.RunOutput("describe", "--tags") : tag)
		.Replace("[GIT_HASH_SHORT]", Git.RunOutput("rev-parse", "--short", "HEAD").Trim())
		.Replace("[GIT_HASH_LONG]", Git.RunOutput("rev-parse", "--long", "HEAD").Replace("--long", null).Trim())
		.Replace("[GIT_URL]", Git.RunOutput("remote", "get-url", "origin").Replace(".git", null).Trim() + "/commit/" + Git.RunOutput("rev-parse", "--long", "HEAD").Replace("--long", null).Trim()));

Warn($"Tag: {tag}");
Warn($"Target: {target}");
Warn($"Defines: {defines ?? "N/A"}");
Warn($"Version: {version ?? "N/A"}");
Warn($"Cargo Target: {cargoTarget}");
Warn($"Clean: {(noClean ? "Skip" : "Run")}");
Warn($"Restore: {(noRestore ? "Skip" : "Run")}");
Warn($"Verbosity: {buildVerbosity}");

if (!noClean)
{
	Directories.Delete(Path(Home, "release", ".tmp", target));
	Files.Delete(Path(Home, "release", $"Carbon.{target}.tar.gz"));
}

DotNet.ExitOnError(true);
if (!noClean)
{
	DotNet.Run("clean", PathEnquotes(Home, "src"), "--configuration", target, "--verbosity", buildVerbosity);
}
if (noRestore)
{
	DotNet.Run("build", PathEnquotes(Home, "src"), "--configuration", target, "--verbosity", buildVerbosity, "--no-restore",
		$"/p:UserConstants=\"{defines}\"", $"/p:UserVersion=\"{version}\"");
}
else
{
	DotNet.Run("build", PathEnquotes(Home, "src"), "--configuration", target, "--verbosity", buildVerbosity,
		$"/p:UserConstants=\"{defines}\"", $"/p:UserVersion=\"{version}\"");
}

Files.Copy(Path(Home, "tools", "helpers", "Carbon.targets"), Path(Home, "release", ".tmp", target, "Carbon.targets"));

var tos = isUnix ? "Linux" : "Windows";
var finalTarget = target.Replace("Unix", string.Empty);

Files.DeleteContains(Path(Home, "release", ".tmp", target, "carbon", "managed", "lib"), "carbon");
Directories.Delete(Path(Home, "release", ".tmp", target, "profiler"));

if (isUnix)
{
	Files.Copy(Path(Home, "tools", "helpers", "carbon.sh"), Path(Home, "release", ".tmp", target));
	Files.Copy(Path(Home, "tools", "helpers", "environment.sh"), Path(Home, "release", ".tmp", target, "carbon", "tools"));
	Files.Copy(Path(Home, "tools", "unitydoorstop", "linux", "x64", "libdoorstop.so"), Path(Home, "release", ".tmp", target));
	Files.Copy(Path(Home, "src", "Carbon.Native", "target", "x86_64-unknown-linux-gnu", cargoTarget, "libCarbonNative.so"), Path(Home, "release", ".tmp", target, "carbon", "native"), optional: true);

	if (!noArchive)
	{
		Archive.Tar(Path(Home, "release", ".tmp", target), Path(Home, "release", $"Carbon.{tos}.{finalTarget}.tar.gz"));
	}
}
else
{
	Files.Copy(Path(Home, "tools", "helpers", "doorstop_config.ini"), Path(Home, "release", ".tmp", target));
	Files.Copy(Path(Home, "tools", "unitydoorstop", "windows", "x64", "doorstop.dll"), Path(Home, "release", ".tmp", target, "winhttp.dll"));
	Files.Copy(Path(Home, "src", "Carbon.Native", "target", "x86_64-pc-windows-gnu", cargoTarget, "CarbonNative.dll"), Path(Home, "release", ".tmp", target, "carbon", "native"), optional: true);

	if (!noArchive)
	{
		Archive.Zip(Path(Home, "release", ".tmp", target), Path(Home, "release", $"Carbon.{tos}.{finalTarget}.zip"));
	}
}

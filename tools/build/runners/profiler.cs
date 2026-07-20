var target = GetArg(1, "Debug");
var defines = GetArg(2);
var tag = GetArg(3, Git.RunOutput("describe", "--tags"));
var version = GetVariable("VERSION");
var cargoTarget = target.Equals("Debug") || target.Equals("DebugUnix") || target.Equals("Minimal") || target.Equals("MinimalUnix") ? "release" : "prod";
var isUnix = target.Contains("Unix");
var noArchive = HasArg("-noarchive");

Run(Path(Home, "tools", "build", "runners", "git.cs"), tag);

var temp = Path(Home, "src", ".tmp");
System.IO.File.WriteAllText(Path(Home, "src", "Carbon.Components", "Carbon.Common", "src", "Carbon", "Build.cs"),
	System.IO.File.ReadAllText(Path(Home, "src", "Carbon.Components", "Carbon.Common", "src", "Carbon", "Build.cs.template"))
		.Replace("[GIT_BRANCH]", System.IO.File.ReadAllText(Path(temp, ".gitbranch")))
		.Replace("[GIT_AUTHOR]", System.IO.File.ReadAllText(Path(temp, ".gitauthor")))
		.Replace("[GIT_COMMENT]", System.IO.File.ReadAllText(Path(temp, ".gitcomment")))
		.Replace("[GIT_DATE]", System.IO.File.ReadAllText(Path(temp, ".gitdate")))
		.Replace("[GIT_TAG]", string.IsNullOrEmpty(tag) ? System.IO.File.ReadAllText(Path(temp, ".gittag")) : tag)
		.Replace("[GIT_HASH_SHORT]", System.IO.File.ReadAllText(Path(temp, ".gitchs")))
		.Replace("[GIT_HASH_LONG]", System.IO.File.ReadAllText(Path(temp, ".gitchl")))
		.Replace("[GIT_URL]", System.IO.File.ReadAllText(Path(temp, ".giturl")) + "/commit/" + System.IO.File.ReadAllText(Path(temp, ".gitchl"))));

DotNet.ExitOnError(true);
DotNet.Run("restore", PathEnquotes(Home, "src"));
DotNet.Run("clean", PathEnquotes(Home, "src"), "--configuration", target);
DotNet.Run("build", PathEnquotes(Home, "src"), "--configuration", target, "--no-restore",
	$"/p:UserConstants=\"{defines}\"", $"/p:UserVersion=\"{version}\"");

var tos = isUnix ? "Linux" : "Windows";

if(isUnix)
{
	Files.Copy(Path(Home, "src", "Carbon.Native", "target", "x86_64-unknown-linux-gnu", cargoTarget, "libCarbonNative.so"), Path(Home, "release", ".tmp", target, "profiler", "native"));

	if(!noArchive)
	{
		Archive.Tar(Path(Home, "release", ".tmp", target, "profiler"), Path(Home, "release", $"Carbon.{tos}.Profiler.tar.gz"));
	}
}
else
{
	Files.Copy(Path(Home, "src", "Carbon.Native", "target", "x86_64-pc-windows-gnu", cargoTarget, "CarbonNative.dll"), Path(Home, "release", ".tmp", target, "profiler", "native"));

	if(!noArchive)
	{
		Archive.Zip(Path(Home, "release", ".tmp", target, "profiler"), Path(Home, "release", $"Carbon.{tos}.Profiler.zip"));
	}
}


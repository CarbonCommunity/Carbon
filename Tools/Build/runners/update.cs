var branch = GetArg(1, "release");

Warn($"Branch: {branch}");

DownloadRustFiles("windows");
DownloadRustFiles("linux");

void DownloadRustFiles(string platform)
{
	Log($"Downloading {platform} Rust files..");
	DotNet.Run("run", "--project", PathEnquotes(Home, "Tools", "DepotDownloader", "DepotDownloader"), 
		"-os", platform, 
		"-validate", 
		"-app 258550",
		"-branch", branch, 
		"-filelist", PathEnquotes(Home, "Tools", "Helpers", "258550_refs.txt"),
		"-dir", PathEnquotes(Home, "Rust", platform));
		
	var hash = Files.Hash(Path(Home, "Rust", platform, "RustDedicated_Data", "Managed", "Assembly-CSharp.dll")).ToString();
	Files.Create(Path(Home, "Rust", platform, "RustDedicated_Data", "Managed", ".hash"), hash);
	Log($"Assembly-CSharp = {hash} [hash]");

	DotNet.Run("run", "--project", PathEnquotes(Home, "Carbon.Core", "Carbon.Tools", "Carbon.Publicizer"), 
		PathEnquotes(Home, "Rust", platform, "RustDedicated_Data", "Managed"));
}

DotNet.Run("run", "--project", PathEnquotes(Home, "Carbon.Core", "Carbon.Tools", "Carbon.Generator"),
	"--plugininput", PathEnquotes(Home, "Carbon.Core", "Carbon.Components", "Carbon.Common", "src", "Carbon", "CorePlugin"));

var modules = new System.Collections.Generic.List<string>();
modules.AddRange(Directories.Get(Path(Home, "Carbon.Core", "Carbon.Components", "Carbon.Common", "src", "Carbon", "Modules")));
modules.AddRange(Directories.Get(Path(Home, "Carbon.Core", "Carbon.Components", "Carbon.Modules", "src")));

var modulePaths = string.Join(";", modules);
DotNet.Run("run", "--project", PathEnquotes(Home, "Carbon.Core", "Carbon.Tools", "Carbon.Generator"),
	"--plugininput", $"\"{modulePaths}\"",
	"--pluginnamespace", "Carbon.Modules",
	"--basename", "module");

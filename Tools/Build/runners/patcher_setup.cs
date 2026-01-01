var branch = GetArg(1, "release");
var output = GetArg(2, Home);

Warn($"Branch: {branch}");

DotNet.Run("build", PathEnquotes(Home, "Tools", "DepotDownloader", "DepotDownloader"));
DotNet.Run("build", PathEnquotes(Home, "Carbon.Core", "Carbon.Tools", "Carbon.Publicizer"));

System.Threading.Tasks.Task.WaitAll(
    System.Threading.Tasks.Task.Run(() => DownloadRustFiles("windows")),
    System.Threading.Tasks.Task.Run(() => DownloadRustFiles("linux"))
);

void DownloadRustFiles(string platform)
{
	Log($"Downloading {platform} Rust files..");
	DotNet.Run("run", "--no-build", "--project", PathEnquotes(Home, "Tools", "DepotDownloader", "DepotDownloader"),
		"-os", platform, 
		"-validate", 
		"-app 258550",
		"-branch", branch, 
		"-filelist", PathEnquotes(Home, "Tools", "Helpers", "258550_refs.txt"),
		"-dir", PathEnquotes(output, platform));
		
	DotNet.Run("run", "--no-build", "--project", PathEnquotes(Home, "Carbon.Core", "Carbon.Tools", "Carbon.Publicizer"), 
		PathEnquotes(Home, "Rust", platform, "RustDedicated_Data", "Managed"));
}
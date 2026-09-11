var branch = GetArg(1, "release");

Warn($"Branch: {branch}");

DotNet.Run("build", PathEnquotes(Home, "tools", "depot", "DepotDownloader"));
DotNet.Run("build", PathEnquotes(Home, "src", "Carbon.Tools", "Carbon.Publicizer"));

System.Threading.Tasks.Task.WaitAll(
    System.Threading.Tasks.Task.Run(() => DownloadRustFiles("windows")),
    System.Threading.Tasks.Task.Run(() => DownloadRustFiles("linux"))
);

void DownloadRustFiles(string platform)
{
	Log($"Downloading {platform} Rust files..");
	DotNet.Run("run", "--no-build", "--project", PathEnquotes(Home, "tools", "depot", "DepotDownloader"),
		"-os", platform, 
		"-validate", 
		"-app 258550",
		"-branch", branch, 
		"-filelist", PathEnquotes(Home, "tools", "helpers", "258550_refs.txt"),
		"-dir", PathEnquotes(Home, "rust", platform));
		
	var hash = Files.Hash(Path(Home, "rust", platform, "RustDedicated_Data", "Managed", "Assembly-CSharp.dll")).ToString();
	Files.Create(Path(Home, "rust", platform, "RustDedicated_Data", "Managed", ".hash"), hash);
	Log($"Assembly-CSharp = {hash} [hash]");

	DotNet.Run("run", "--no-build", "--project", PathEnquotes(Home, "src", "Carbon.Tools", "Carbon.Publicizer"), 
		PathEnquotes(Home, "rust", platform, "RustDedicated_Data", "Managed"));
}

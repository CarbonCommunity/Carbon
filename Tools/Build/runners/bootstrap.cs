Log(Home);

Warn("Git Hooks");
{
	Copy.Folder(".githooks", ".git/hooks", optional: true);
}

Git.Run("config", "--global", "--add", "safe.directory", "'*'");
Git.Run("config", "--local", "--add", "safe.directory", "'*'");

Warn("Git Setup");
{
	Git.Run("-C", Home, "submodule", "init");
	Git.Run("-C", Home, "submodule", "update");
	Git.Run("-C", Home, "submodule", "foreach", "git", "checkout");
}

Warn("Building Submodules");
{	
	DotNet.Run("restore", PathEnquotes(Home, "Tools", "DepotDownloader"));
	DotNet.Run("clean", PathEnquotes(Home, "Tools", "DepotDownloader"));
	DotNet.Run("build", PathEnquotes(Home, "Tools", "DepotDownloader"), "--no-restore", "--no-incremental");
}
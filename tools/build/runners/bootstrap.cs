Log(Home);

Git.Run("config", "--global", "--add", "safe.directory", "*");
Git.Run("config", "--local", "--add", "safe.directory", "*");

Warn("Git Setup");
{
	Git.Run("-C", Home, "submodule", "init");
	Git.Run("-C", Home, "submodule", "update");
	Git.Run("-C", Home, "submodule", "foreach", "git", "checkout");
}

Warn("Building Submodules");
{	
	DotNet.Run("restore", PathEnquotes(Home, "tools", "depot"));
	DotNet.Run("clean", PathEnquotes(Home, "tools", "depot"));
	DotNet.Run("build", PathEnquotes(Home, "tools", "depot"), "--no-restore", "--no-incremental");
}
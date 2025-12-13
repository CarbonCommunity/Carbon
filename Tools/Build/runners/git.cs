var localTag = GetArg(0);
Warn($"Local Tag: {localTag}");

var temp = Path(Home, "Carbon.Core", ".tmp");
Directories.Create(temp);

Files.Create(Path(temp, ".gitbranch"), Git.RunOutput("branch", "--show-current"));
Files.Create(Path(temp, ".gitchs"), Git.RunOutput("rev-parse", "--short", "HEAD"));
Files.Create(Path(temp, ".gitchl"), Git.RunOutput("rev-parse", "--long", "HEAD"));
Files.Create(Path(temp, ".gitauthor"), Git.RunOutput("show", "-s", "--format=\"%an\"", "HEAD"));
Files.Create(Path(temp, ".gitcomment"), Git.RunOutput("log -1", "--pretty=\"%B\"", "HEAD"));
Files.Create(Path(temp, ".gitdate"), Git.RunOutput("log -1", "--format=\"%ci\"", "HEAD"));

if(string.IsNullOrEmpty(localTag))
{
	Files.Create(Path(temp, ".gittag"), Git.RunOutput("describe", "--tags"));
}
else
{
	Files.Create(Path(temp, ".gittag"), localTag);
}

Files.Create(Path(temp, ".giturl"), Git.RunOutput("remote", "get-url", "origin"));
Files.Create(Path(temp, ".gitchanges"), Git.RunOutput("log -1", "--name-status", "--format=\"\""));

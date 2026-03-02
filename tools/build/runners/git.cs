var localTag = GetArg(0);
Warn($"Local Tag: {localTag}");

var temp = Path(Home, "src", ".tmp");
Directories.Create(temp);

Files.Create(Path(temp, ".gitbranch"), Git.RunOutput("branch", "--show-current").Trim());
Files.Create(Path(temp, ".gitchs"), Git.RunOutput("rev-parse", "--short", "HEAD").Trim());
Files.Create(Path(temp, ".gitchl"), Git.RunOutput("rev-parse", "--long", "HEAD").Replace("--long", null).Trim());
Files.Create(Path(temp, ".gitauthor"), Git.RunOutput("show", "-s", "--format=\"%an\"", "HEAD").Trim());
Files.Create(Path(temp, ".gitcomment"), Git.RunOutput("log -1", "--pretty=\"%B\"", "HEAD").Trim());
Files.Create(Path(temp, ".gitdate"), Git.RunOutput("log -1", "--format=\"%ci\"", "HEAD").Trim());

if(string.IsNullOrEmpty(localTag))
{
    Files.Create(Path(temp, ".gittag"), Git.RunOutput("describe", "--tags").Trim());
}
else
{
    Files.Create(Path(temp, ".gittag"), localTag);
}

Files.Create(Path(temp, ".giturl"), Git.RunOutput("remote", "get-url", "origin").Trim());
Files.Create(Path(temp, ".gitchanges"), Git.RunOutput("log -1", "--name-status", "--format=\"\"").Trim());

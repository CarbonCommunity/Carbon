var home = Home;

using var stream = new System.IO.MemoryStream();
{
	using var writer = new System.IO.BinaryWriter(stream);
	{
		//ProcessRepository(Path(home));
		ProcessRepository(Path(home, "Carbon.Core", "Carbon.Components", "Carbon.Common"));

		System.IO.File.WriteAllBytes("git_out.dat", stream.ToArray());
	}
}

void ProcessRepository(string path)
{
	SetHome(path);

	Log(path);
	Log(string.Empty);

	Git.Run("fetch", "--all", "--prune");

	var origin = Git.RunOutput("remote", "get-url", "--push", "--all", "origin");
	var output = Git.RunOutput("log", "--all", "--date=iso-strict", "--format=%H%x1f%ad%x1f%S%x1f%s");
	var lines = output.Split('\n', StringSplitOptions.RemoveEmptyEntries);

	for(int i = 0; i < lines.Length; i++)
	{
		var parts = lines[i].Split('\x1f');
		if (parts.Length >= 4)
		{
			var commit = parts[0];
			var date = parts[1];
			var branch = parts[2].Replace("refs/heads/", string.Empty).Replace("refs/remotes/origin/", string.Empty);
			var subject = parts[3];
			if (branch.Contains("refs/tags"))
			{
				continue;
			}
			Log($"{commit} | {branch} | {date} | {subject}");
		}
	}
}

Log(Home);

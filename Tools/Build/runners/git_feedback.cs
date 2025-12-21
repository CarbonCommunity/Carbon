var home = Home;

var tasks = new System.Collections.Generic.List<Task>();
var repositoryCommits = new System.Collections.Generic.Dictionary<string, System.Collections.Generic.List<(
	string commit, 
	string date,
	string author,
	string message
)>>();
using var stream = new System.IO.MemoryStream();
{
	using var writer = new System.IO.BinaryWriter(stream);
	{
		Log("Starting");

		ProcessRepository(Path(home, "Carbon.Core", "Carbon.Components", "Carbon.Common"));

		Task.WaitAll(tasks);

		Log("Done");

		System.IO.File.WriteAllBytes("git_out.dat", stream.ToArray());
	}
}

void ProcessRepository(string path)
{
	SetHome(path);

	Log(path);
	Log(string.Empty);

	Git.SetQuiet(true);
	Git.Run("fetch", "--all", "--prune");

	var origin = Git.RunOutput("remote", "get-url", "--push", "--all", "origin");
	var output = Git.RunOutput("log", "--all", "--date=iso-strict", "--format=%H%x1f%ad%x1f%an%x1f%s");
	var lines = output.Split('\n', StringSplitOptions.RemoveEmptyEntries);

	for(int i = 0; i < lines.Length; i++)
	{
		var parts = lines[i].Split('\x1f');
		tasks.Add(Task.Run(TaskCallback));
		void TaskCallback()
		{
			if (parts.Length >= 4)
			{
				var commit = parts[0];
				var date = parts[1];
				var author = parts[2];
				var subject = parts[3];
				Log($"{commit} | {author} | {date} | {subject}");
			}
		}
	}
}
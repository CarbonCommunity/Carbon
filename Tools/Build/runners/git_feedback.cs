            var locker = new object();
			var home = Home;

var tasks = new System.Collections.Generic.List<Task>();
var commits = new System.Collections.Generic.List<(
    string repo,
    string sha, 
    string date,
    string author,
    string message,
    int changeset
    )>();
using var stream = new System.IO.MemoryStream();
{
    using var writer = new System.IO.BinaryWriter(stream);
    {
        ProcessRepository("Carbon", Path(home));
        ProcessRepository("Carbon.Bootstrap", Path(home, "Carbon.Core", "Carbon.Components", "Carbon.Bootstrap"));
        ProcessRepository("Carbon.Common", Path(home, "Carbon.Core", "Carbon.Components", "Carbon.Common"));
        ProcessRepository("Carbon.Compat", Path(home, "Carbon.Core", "Carbon.Components", "Carbon.Compat"));
        ProcessRepository("Carbon.Modules", Path(home, "Carbon.Core", "Carbon.Components", "Carbon.Modules"));
        ProcessRepository("Carbon.Preloader", Path(home, "Carbon.Core", "Carbon.Components", "Carbon.Preloader"));
        ProcessRepository("Carbon.SDK", Path(home, "Carbon.Core", "Carbon.Components", "Carbon.SDK"));
        ProcessRepository("Carbon.Startup", Path(home, "Carbon.Core", "Carbon.Components", "Carbon.Startup"));
        ProcessRepository("Carbon.Test", Path(home, "Carbon.Core", "Carbon.Components", "Carbon.Test"));
        ProcessRepository("Carbon.Native", Path(home, "Carbon.Core", "Carbon.Native"));
        ProcessRepository("Carbon.Profiler", Path(home, "Carbon.Core", "Carbon.Profiler"));
        ProcessRepository("Carbon.UniTask", Path(home, "Carbon.Core", "Carbon.UniTask"));
        ProcessRepository("Carbon.Hooks.Base", Path(home, "Carbon.Core", "Carbon.Hooks", "Carbon.Hooks.Base"));
        ProcessRepository("Carbon.Hooks.Community", Path(home, "Carbon.Core", "Carbon.Hooks", "Carbon.Hooks.Community"));

        Task.WaitAll(tasks);
        
        writer.Write(commits.Count);
        for (int i = 0; i < commits.Count; i++)
        {
            var commit = commits[i];
            writer.Write(commit.repo);
            writer.Write(commit.sha);
            writer.Write(commit.date);
            writer.Write(commit.author);
            writer.Write(commit.message);
            writer.Write(commit.changeset);
        }
        
        System.IO.File.WriteAllBytes("git_out.dat", stream.ToArray());
    }
}

void ProcessRepository(string repo, string workspace)
{
    tasks.Add(Task.Run(TaskProcess));

    void TaskProcess()
    {
        var git = new Carbon.Runner.Executors.Git();
        git.SetQuiet(true);
        git.WorkingDirectory(workspace);
        git.Run("fetch", "--all", "--prune");

        var origin = git.RunOutput("remote", "get-url", "--push", "--all", "origin");
        var output = git.RunOutput("log", "--all", "--reverse", "--date=iso-strict", "--format=%H%x1f%ad%x1f%an%x1f%s");

        var lines = output.Split('\n', StringSplitOptions.RemoveEmptyEntries);
        var count = 0;
        for (int i = 0; i < lines.Length; i++)
        {
            var parts = lines[i].Split('\x1f');
            if (parts.Length >= 4)
            {
                var sha = parts[0];
                var date = parts[1];
                var author = parts[2];
                var message = parts[3];
                lock (locker)
                {
                    commits.Add((repo, sha, date, author, message, i));
                }
                count++;
            }
        }
        Log($"Collected {count} commits for {repo}");
    }
}
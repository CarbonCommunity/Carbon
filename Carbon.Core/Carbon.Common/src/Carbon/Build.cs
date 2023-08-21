namespace Carbon;

using System;
using System.Linq;

public class Build
{
	public class Git
	{
		static Git()
		{
			var changes = @"M Carbon.Core/.msbuild/Tasks.targets";
			var newlineSplit = new string[] { Environment.NewLine };
			var spaceSplit = new char[] { ' ' };
			var lines = changes.Split(newlineSplit, StringSplitOptions.RemoveEmptyEntries);
			var temp = new List<AssetChange>();

			foreach(var line in lines)
			{
				var lineSplit = line.Split(spaceSplit);
				var type = lineSplit[0];
				var path = lineSplit[1];
				var changeType = (AssetChange.ChangeTypes)default;

				switch(type)
				{
					case "A":
						changeType = AssetChange.ChangeTypes.Added;
						break;
					case "M":
						changeType = AssetChange.ChangeTypes.Modified;
						break;
					case "D":
						changeType = AssetChange.ChangeTypes.Deleted;
						break;
				}

				temp.Add(new AssetChange(path, changeType));

				Array.Clear(lineSplit, 0, lineSplit.Length);
			}

			Changes = temp.ToArray();

			Array.Clear(lines, 0, lines.Length);
			Array.Clear(newlineSplit, 0, newlineSplit.Length);
			Array.Clear(spaceSplit, 0, spaceSplit.Length);

			temp.Clear();
		}

		public static readonly string Branch = @"develop";

		public static readonly string Author = @"raul";
		public static readonly string Comment = @"Question mark";
		public static readonly string Date = @"2023-08-21 13:00:23 +0200";

		public static readonly string Url = @"https://github.com/CarbonCommunity/Carbon.Core.git/commit/f717d5b738f71ddc39a33955c5b9b9ebed6421dd";		

		public static readonly AssetChange[] Changes;

		public struct AssetChange
		{
			public string Path { get; private set; }
			public ChangeTypes Type { get; private set; }

			internal AssetChange(string path, ChangeTypes type)
			{
				Path = path;
				Type = type;
			}

			public enum ChangeTypes
			{
				Added,
				Modified,
				Deleted
			}
		}
	}
}

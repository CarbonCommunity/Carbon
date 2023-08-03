using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Carbon.Components;

internal class CarbonAuto
{
	public static void Save()
	{
		using (TimeMeasure.New("CarbonAuto.Save", 100))
		{
			try
			{
				var type = typeof(CorePlugin);
				var flags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public;
				var fields = type.GetFields(flags);
				var properties = type.GetProperties(flags);
				var core = Community.Runtime.CorePlugin;

				using var sb = new StringBody();

				foreach (var field in fields)
				{
					var commandVarAttr = field.GetCustomAttribute<CommandVarAttribute>();
					if (commandVarAttr == null || !commandVarAttr.Saved) continue;

					sb.Add($"c.{commandVarAttr.Name} \"{(field.IsStatic ? field.GetValue(null) : field.GetValue(core))}\"");
				}

				OsEx.File.Create(Defines.GetCarbonAutoFile(), sb.ToNewLine());
			}
			catch (Exception ex)
			{
				Logger.Error($"Failed saving Carbon auto file", ex);
			}
		}
	}
	public static void Load()
	{
		using (TimeMeasure.New("CarbonAuto.Load", 100))
		{
			try
			{
				var file = Defines.GetCarbonAutoFile();

				if (!OsEx.File.Exists(file))
				{
					Save();
					return;
				}

				var lines = OsEx.File.ReadTextLines(Defines.GetCarbonAutoFile());

				foreach (var line in lines)
				{
					ConsoleSystem.Run(ConsoleSystem.Option.Server.Quiet(), line, Array.Empty<string>());
				}

			}
			catch (Exception ex)
			{
				Logger.Error($"Failed saving Carbon auto file", ex);
			}
		}
	}
}

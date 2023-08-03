namespace Carbon.Components;

public class CarbonAuto : API.Abstracts.CarbonAuto
{
	internal Dictionary<string, object> _autoCache = new();

	public static void Init()
	{
		Singleton = new CarbonAuto();
	}
	public override void Refresh()
	{
		var type = typeof(CorePlugin);
		var flags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public;
		var fields = type.GetFields(flags);
		var properties = type.GetProperties(flags);

		_autoCache.Clear();

		foreach (var field in fields)
		{
			var commandVarAttr = field.GetCustomAttribute<CommandVarAttribute>();
			if (commandVarAttr == null || !commandVarAttr.Saved) continue;

			_autoCache.Add($"c.{commandVarAttr.Name}", field);
		}

		foreach (var property in properties)
		{
			var commandVarAttr = property.GetCustomAttribute<CommandVarAttribute>();
			if (commandVarAttr == null || !commandVarAttr.Saved) continue;

			_autoCache.Add($"c.{commandVarAttr.Name}", property);
		}
	}
	public override bool IsChanged()
	{
		using (TimeMeasure.New("CarbonAuto.IsChanged", 100))
		{
			var core = Community.Runtime.CorePlugin;

			foreach (var cache in _autoCache)
			{
				switch (cache.Value)
				{
					case FieldInfo field:
						{
							var value = field.IsStatic ? field.GetValue(null) : field.GetValue(core);
							if (value is float floatValue && floatValue != -1) return true;
						}
						break;

					case PropertyInfo property:
						{
							var value = property.GetValue(core);
							if (value is float floatValue && floatValue != -1) return true;
						}
						break;
				}
			}
		}

		return false;
	}
	public override void Save()
	{
		using (TimeMeasure.New("CarbonAuto.Save", 100))
		{
			try
			{
				Refresh();

				var type = typeof(CorePlugin);
				var flags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public;
				var fields = type.GetFields(flags);
				var properties = type.GetProperties(flags);
				var core = Community.Runtime.CorePlugin;

				using var sb = new StringBody();

				foreach (var cache in _autoCache)
				{
					switch (cache.Value)
					{
						case FieldInfo field:
							sb.Add($"{cache.Key} \"{(field.IsStatic ? field.GetValue(null) : field.GetValue(core))}\"");
							break;

						case PropertyInfo property:
							sb.Add($"{cache.Key} \"{property.GetValue(core)}\"");
							break;
					}
				}

				OsEx.File.Create(Defines.GetCarbonAutoFile(), sb.ToNewLine());
			}
			catch (Exception ex)
			{
				Logger.Error($"Failed saving Carbon auto file", ex);
			}
		}
	}
	public override void Load()
	{
		using (TimeMeasure.New("CarbonAuto.Load", 100))
		{
			try
			{
				Refresh();

				var file = Defines.GetCarbonAutoFile();

				if (!OsEx.File.Exists(file))
				{
					Save();
					return;
				}

				var lines = OsEx.File.ReadTextLines(Defines.GetCarbonAutoFile());
				var option = ConsoleSystem.Option.Server.Quiet();

				foreach (var line in lines)
				{
					ConsoleSystem.Run(option, line, Array.Empty<string>());
				}

				if (IsChanged())
				{
					Logger.Warn($" The server Carbon auto options have been changed.\n" +
								$" Any values that aren't \"-1\" will force the server to modded!");
				}
			}
			catch (Exception ex)
			{
				Logger.Error($"Failed saving Carbon auto file", ex);
			}
		}
	}
}

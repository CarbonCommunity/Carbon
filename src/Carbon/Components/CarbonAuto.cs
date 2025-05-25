namespace Carbon.Components;

/// <summary>
/// Synonymous to Rust's own 'serverauto' tweakable ConVar variables. Carbon expands on Rust or Carbon-related features designed for customisation.
/// </summary>
public class CarbonAuto : API.Abstracts.CarbonAuto
{
	public static Dictionary<string, AutoVar> AutoCache = new();

	internal bool _initialized;

	public struct AutoVar
	{
		public CarbonAutoVar Variable;
		public object ReflectionInfo;

		public readonly Type GetVarType()
		{
			switch(ReflectionInfo)
			{
				case FieldInfo field:
					return field.FieldType;
				case PropertyInfo property:
					return property.PropertyType;
			}

			return null;
		}
		public readonly object GetValue()
		{
			var core = Community.Runtime.Core;

			return ReflectionInfo switch
			{
				FieldInfo field => field.IsStatic ? field.GetValue(null) : field.GetValue(core),
				PropertyInfo property => property.GetValue(core),
				_ => null
			};
		}
		public void SetValue(object value)
		{
			var core = Community.Runtime.Core;

			switch(ReflectionInfo)
			{
				case FieldInfo field:
					field.SetValue(field.IsStatic ? null : core, Convert.ChangeType(value, GetVarType()));
					break;
				case PropertyInfo property:
					property.SetValue(core, Convert.ChangeType(value, GetVarType()));
					break;
			}
		}
		public readonly bool IsChanged()
		{
			var value = GetValue();

			if (value == null)
			{
				return false;
			}

			return !value.Equals(Convert.ChangeType(-1, GetVarType()));
		}
	}

	public static void Init()
	{
		Singleton = new CarbonAuto();
		Singleton.Refresh();
	}
	public override void Refresh()
	{
		if (_initialized)
		{
			return;
		}

		_initialized = true;

		var type = typeof(CorePlugin);
		var flags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public;
		var fields = type.GetFields(flags);
		var properties = type.GetProperties(flags);

		AutoCache.Clear();

		foreach (var field in fields)
		{
			var attribute = field.GetCustomAttribute<CarbonAutoVar>();
			if (attribute == null) continue;

			AutoVar var = default;
			var.Variable = attribute;
			var.ReflectionInfo = field;

			AutoCache.Add($"c.{attribute.Name}", var);
		}
		foreach (var property in properties)
		{
			var attribute = property.GetCustomAttribute<CarbonAutoVar>();
			if (attribute == null) continue;

			AutoVar var = default;
			var.Variable = attribute;
			var.ReflectionInfo = property;

			AutoCache.Add($"c.{attribute.Name}", var);
		}
	}
	public override bool IsForceModded()
	{
		using (TimeMeasure.New("CarbonAuto.IsChanged"))
		{
			var core = Community.Runtime.Core;

			foreach (var cache in AutoCache)
			{
				if (!cache.Value.Variable.ForceModded)
				{
					continue;
				}

				if (cache.Value.GetValue() is float and not -1) return true;
			}
		}

		return false;
	}
	public override void Save()
	{
		using (TimeMeasure.New("CarbonAuto.Save"))
		{
			try
			{
				Refresh();

				using var sb = new StringBody();

				foreach (var cache in AutoCache)
				{
					sb.Add($"{cache.Key} \"{cache.Value.GetValue()}\"");
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
		using (TimeMeasure.New("CarbonAuto.Load"))
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

				var lines = OsEx.File.ReadTextLines(file);
				var option = ConsoleSystem.Option.Server;

				Logger.Log($"Initialized Carbon Auto ({lines.Length:n0} {lines.Length.Plural("variable", "variables")})");


				foreach (var line in lines)
				{
					try
					{
						var split = line.Split(' ');
						var convar = split.Length > 0 ? split[0] : default;
						var conval = split.Skip(1).ToString(" ").Replace("\"", string.Empty);

						if (AutoCache.TryGetValue(convar, out var auto))
						{
							auto.SetValue(conval);
							Logger.Warn($" {convar} \"{auto.GetValue()}\"{(auto.Variable.ForceModded ? " [modded]" : string.Empty)}");
						}
					}
					catch (Exception ex)
					{
						Logger.Error($"Failed processing line '{line}'", ex);
					}
				}

				if (IsForceModded())
				{
					Logger.Warn($" The server Carbon auto options have been changed which are gameplay significant.\n" +
								$" Any values that aren't \"-1\" will force the server to modded!");
				}
			}
			catch (Exception ex)
			{
				Logger.Error($"Failed loading Carbon auto file", ex);
			}
		}
	}
}

/// <summary>
/// Carbon-auto variable definition attribute.
/// </summary>
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public class CarbonAutoVar : CommandVarAttribute
{
	public string DisplayName;
	public bool ForceModded;

	public CarbonAutoVar(string name, string displayName, string help = null, bool @protected = false, bool forceModded = false) : base(name, @protected, help)
	{
		DisplayName = displayName;
		ForceModded = forceModded;
	}
}

/// <summary>
/// Carbon-auto variable definition attribute which when its value is not '-1' (default), will enforce the server to become modded to reduce the risk of being blacklisted.
/// </summary>
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public class CarbonAutoModdedVar : CarbonAutoVar
{
	public CarbonAutoModdedVar(string name, string displayName, string help = null, bool @protected = false, bool forceModded = false) : base(name, displayName, help, @protected, forceModded)
	{
		ForceModded = true;
	}
}

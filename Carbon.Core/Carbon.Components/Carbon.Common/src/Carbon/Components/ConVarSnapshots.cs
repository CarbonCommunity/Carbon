namespace Carbon.Components;

/// <summary>
/// A dedicated place of keeping track of all default Rust variable (ConVar) values.
/// </summary>
public class ConVarSnapshots
{
	public static readonly Dictionary<string, Snapshot> Snapshots = new();

	public static void TakeSnapshot()
	{
		try
		{
			Snapshots.Clear();

			using (TimeMeasure.New("ConVarSnapshots.TakeSnapshot", 150))
			{
				var convarTypes = typeof(BasePlayer).Assembly.GetExportedTypes();

				foreach (var type in convarTypes)
				{
					var factory = type.GetCustomAttribute<ConsoleSystem.Factory>();
					var factoryName = factory == null ? type.Name.ToLower() : factory.Name;
					var fields = type.GetFields(BindingFlags.Static | BindingFlags.Public)
						.Where(x => x.GetCustomAttribute<ServerVar>() != null);

					foreach (var field in fields)
					{
						Snapshot snapshot = default;
						snapshot.Value = field.GetValue(null);
						snapshot.Field = new(field, field.GetCustomAttribute<ServerVar>());
						snapshot.Factory = new(type, factory);

						Snapshots.Add($"{factoryName}.{field.Name}", snapshot);
					}
				}
			}
		}
		catch (Exception ex)
		{
			Logger.Error($"Failed taking snapshot of all default Rust ConVar values", ex);
		}
	}

	public struct Snapshot
	{
		public object Value;
		public KeyValuePair<FieldInfo, ServerVar> Field;
		public KeyValuePair<Type, ConsoleSystem.Factory> Factory;
	}
}

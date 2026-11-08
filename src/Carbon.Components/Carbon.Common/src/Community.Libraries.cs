using Newtonsoft.Json;

namespace Carbon;

public partial class Community
{
	/// <summary>Server-wide permission system. Backend picked by config (protobuf, SQL or storeless).</summary>
	public Permission Permission { get; set; }

	/// <summary>JSON data files under carbon/data.</summary>
	public DataFileSystem DataFileSystem { get; set; }

	/// <summary>
	/// Creates the shared plugin libraries. Override <see cref="CreatePermission"/> to swap the permission backend.
	/// </summary>
	protected virtual void InstallLibraries()
	{
		var previous = JsonConvert.DefaultSettings;
		JsonConvert.DefaultSettings = () =>
		{
			var settings = previous?.Invoke() ?? new JsonSerializerSettings();
			settings.Culture = CultureInfo.InvariantCulture;
			settings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
			return settings;
		};

		DataFileSystem = new DataFileSystem(Defines.GetDataFolder());
		Permission = CreatePermission();

		Events.Trigger(CarbonEvent.LibrariesInstalled, EventArgs.Empty);
	}

	protected virtual Permission CreatePermission()
	{
		return Config.Permissions.PermissionSerialization switch
		{
			Permission.SerializationMode.Storeless => new PermissionStoreless(),
			Permission.SerializationMode.SQL => new PermissionSql(),
			_ => new Permission()
		};
	}
}

#if !MINIMAL

namespace Carbon.Modules;

public partial class AdminModule
{
	public class EnvironmentTab
	{
		private static int LastWeatherPresetSelectedIndex;
		private static string[] Options;

		public static Tab Get()
		{
			var tab = (Tab)null;

			tab = new Tab("env", "Environment", Community.Runtime.Core, access: "environment.use",
				onChange: (ap, tab) =>
				{
					Options ??= SingletonComponent<Climate>.Instance?.WeatherPresets.Select(x => x.name).ToArray();
					Draw(tab);
				});

			return tab;
		}

		static void Draw(Tab tab)
		{
			var presets = SingletonComponent<Climate>.Instance.WeatherPresets;
			var overrides = SingletonComponent<Climate>.Instance.WeatherOverrides;

			tab.AddColumn(0, true);
			tab.AddColumn(1, true);

			tab.AddName(0, "Time");
			{
				tab.AddInputButton(0, "Date", 0.3f,
					new Tab.OptionInput(null, ap => TOD_Sky.Instance.Cycle.DateTime.ToString(), 0,
						true, null), new Tab.OptionButton("Change", ap =>
					{
						Singleton.DatePicker.Open(ap.Player, date =>
						{
							var hour = TOD_Sky.Instance.Cycle.Hour;

							TOD_Sky.Instance.Cycle.DateTime = date;
							TOD_Sky.Instance.Cycle.Hour = hour;

							Draw(tab);
							Singleton.Draw(ap.Player);
						});
					}));

				tab.AddToggle(0, "Progress Time",
					ap => TOD_Sky.Instance.Components.Time.ProgressTime =
						!TOD_Sky.Instance.Components.Time.ProgressTime,
					ap => TOD_Sky.Instance.Components.Time.ProgressTime);
				tab.AddRange(0, "Time", 0, 24, ap => TOD_Sky.Instance.Cycle.Hour,
					(ap, value) => { TOD_Sky.Instance.Cycle.Hour = value; },
					ap => $"{TOD_Sky.Instance.Cycle.Hour:0.0}");

				tab.AddName(0, "Ocean");
				tab.AddRange(0, "Scale", -100f, 500f, ap => overrides.OceanScale * 100f,
					(ap, value) =>
					{
						overrides.OceanScale = value * .01f;
						ServerMgr.SendReplicatedVars("weather.");
					}, ap => $"{overrides.OceanScale:0.0}");
				tab.AddRange(0, "Level", 0, 500f, ap => WaterSystem.OceanLevel,
					(ap, value) =>
					{
						WaterSystem.OceanLevel = value;
						ServerMgr.SendReplicatedVars("env.");
					}, ap => $"{WaterSystem.OceanLevel:0.0}");
			}

			tab.AddDropdown(1, "Weather Preset", ap => LastWeatherPresetSelectedIndex, (ap, index) =>
			{
				overrides.Set(presets[LastWeatherPresetSelectedIndex = index]);
				ServerMgr.SendReplicatedVars("weather.");
			}, Options);

			tab.AddName(1, "Environment");
			{
				tab.AddRange(1, "Wind", -100f, 100f, ap => overrides.Wind * 100f,
					(ap, value) =>
					{
						overrides.Wind = value * .01f;
						ServerMgr.SendReplicatedVars("weather.");
					}, ap => $"{overrides.Wind:0.0}");
				tab.AddRange(1, "Rain", -100f, 100f, ap => overrides.Rain * 100f,
					(ap, value) =>
					{
						overrides.Rain = value * .01f;
						ServerMgr.SendReplicatedVars("weather.");
					}, ap => $"{overrides.Rain:0.0}");
				tab.AddRange(1, "Thunder", -100f, 100f, ap => overrides.Thunder * 100f,
					(ap, value) =>
					{
						overrides.Thunder = value * .01f;
						ServerMgr.SendReplicatedVars("weather.");
					}, ap => $"{overrides.Thunder:0.0}");
				tab.AddRange(1, "Rainbow", -100f, 100f, ap => overrides.Rainbow * 100f,
					(ap, value) =>
					{
						overrides.Rainbow = value * .01f;
						ServerMgr.SendReplicatedVars("weather.");
					}, ap => $"{overrides.Rainbow:0.0}");

				tab.AddName(1, "Atmosphere");
				tab.AddRange(1, "RayleighMultiplier", -100f, 500f, ap => overrides.Atmosphere.RayleighMultiplier * 100f,
					(ap, value) =>
					{
						overrides.Atmosphere.RayleighMultiplier = value * .01f;
						ServerMgr.SendReplicatedVars("weather.");
					}, ap => $"{overrides.Atmosphere.RayleighMultiplier:0.0}");
				tab.AddRange(1, "MieMultiplier", -100f, 500f, ap => overrides.Atmosphere.MieMultiplier * 100f,
					(ap, value) =>
					{
						overrides.Atmosphere.MieMultiplier = value * .01f;
						ServerMgr.SendReplicatedVars("weather.");
					}, ap => $"{overrides.Atmosphere.MieMultiplier:0.0}");
				tab.AddRange(1, "Brightness", -100f, 500f, ap => overrides.Atmosphere.Brightness * 100f,
					(ap, value) =>
					{
						overrides.Atmosphere.Brightness = value * .01f;
						ServerMgr.SendReplicatedVars("weather.");
					}, ap => $"{overrides.Atmosphere.Brightness:0.0}");
				tab.AddRange(1, "Contrast", -100f, 500f, ap => overrides.Atmosphere.Contrast * 100f,
					(ap, value) =>
					{
						overrides.Atmosphere.Contrast = value * .01f;
						ServerMgr.SendReplicatedVars("weather.");
					}, ap => $"{overrides.Atmosphere.Contrast:0.0}");
				tab.AddRange(1, "Directionality", -100f, 500f, ap => overrides.Atmosphere.Directionality * 100f,
					(ap, value) =>
					{
						overrides.Atmosphere.Directionality = value * .01f;
						ServerMgr.SendReplicatedVars("weather.");
					}, ap => $"{overrides.Atmosphere.Directionality:0.0}");
				tab.AddRange(1, "Fogginess", -100f, 500f, ap => overrides.Atmosphere.Fogginess * 100f,
					(ap, value) =>
					{
						overrides.Atmosphere.Fogginess = value * .01f;
						ServerMgr.SendReplicatedVars("weather.");
					}, ap => $"{overrides.Atmosphere.Fogginess:0.0}");

				tab.AddName(1, "Clouds");
				tab.AddRange(1, "Size", -100f, 500f, ap => overrides.Clouds.Size * 100f,
					(ap, value) =>
					{
						overrides.Clouds.Size = value * .01f;
						ServerMgr.SendReplicatedVars("weather.");
					}, ap => $"{overrides.Clouds.Size:0.0}");
				tab.AddRange(1, "Opacity", -100f, 500f, ap => overrides.Clouds.Opacity * 100f,
					(ap, value) =>
					{
						overrides.Clouds.Opacity = value * .01f;
						ServerMgr.SendReplicatedVars("weather.");
					}, ap => $"{overrides.Clouds.Opacity:0.0}");
				tab.AddRange(1, "Coverage", -100f, 500f, ap => overrides.Clouds.Coverage * 100f,
					(ap, value) =>
					{
						overrides.Clouds.Coverage = value * .01f;
						ServerMgr.SendReplicatedVars("weather.");
					}, ap => $"{overrides.Clouds.Coverage:0.0}");
				tab.AddRange(1, "Sharpness", -100f, 500f, ap => overrides.Clouds.Sharpness * 100f,
					(ap, value) =>
					{
						overrides.Clouds.Sharpness = value * .01f;
						ServerMgr.SendReplicatedVars("weather.");
					}, ap => $"{overrides.Clouds.Sharpness:0.0}");
				tab.AddRange(1, "Coloring", -100f, 500f, ap => overrides.Clouds.Coloring * 100f,
					(ap, value) =>
					{
						overrides.Clouds.Coloring = value * .01f;
						ServerMgr.SendReplicatedVars("weather.");
					}, ap => $"{overrides.Clouds.Coloring:0.0}");
				tab.AddRange(1, "Attenuation", -100f, 500f, ap => overrides.Clouds.Attenuation * 100f,
					(ap, value) =>
					{
						overrides.Clouds.Attenuation = value * .01f;
						ServerMgr.SendReplicatedVars("weather.");
					}, ap => $"{overrides.Clouds.Attenuation:0.0}");
				tab.AddRange(1, "Saturation", -100f, 500f, ap => overrides.Clouds.Saturation * 100f,
					(ap, value) =>
					{
						overrides.Clouds.Saturation = value * .01f;
						ServerMgr.SendReplicatedVars("weather.");
					}, ap => $"{overrides.Clouds.Saturation:0.0}");
				tab.AddRange(1, "Scattering", -100f, 500f, ap => overrides.Clouds.Scattering * 100f,
					(ap, value) =>
					{
						overrides.Clouds.Scattering = value * .01f;
						ServerMgr.SendReplicatedVars("weather.");
					}, ap => $"{overrides.Clouds.Scattering:0.0}");
				tab.AddRange(1, "Brightness", -100f, 500f, ap => overrides.Clouds.Brightness * 100f,
					(ap, value) =>
					{
						overrides.Clouds.Brightness = value * .01f;
						ServerMgr.SendReplicatedVars("weather.");
					}, ap => $"{overrides.Clouds.Brightness:0.0}");

			}
		}
	}
}

#endif

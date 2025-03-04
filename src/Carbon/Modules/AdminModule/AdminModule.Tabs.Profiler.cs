using Facepunch;

#if !MINIMAL

namespace Carbon.Modules;

public partial class AdminModule
{
	public class ProfilerTab : Tab
	{
		public static MonoProfiler.Sample sample;

		internal static ProfilerTab _instance;
		internal static Color intenseColor;
		internal static Color calmColor;
		internal static Color niceColor;
		internal static MonoProfiler.TimelineRecording recording = new();

		public static readonly System.Drawing.Color[] ChartColors =
		[
			System.Drawing.Color.Tomato,
			System.Drawing.Color.MediumVioletRed,
			System.Drawing.Color.Violet,
			System.Drawing.Color.SteelBlue,
			System.Drawing.Color.SlateBlue,
			System.Drawing.Color.Orange,
			System.Drawing.Color.LightSeaGreen,
			System.Drawing.Color.Red,
			System.Drawing.Color.Chocolate,
			System.Drawing.Color.DarkCyan
		];

		public enum SubtabTypes
		{
			Calls,
			Memory
		}

		internal static string[] timelineChartOptions =
		[
			"Assembly Calls",
			"Assembly Memory",
			"Assembly Time",
			"Assembly Exceptions",
			"Calls",
			"Call Time (Total)",
			"Call Time (Own)",
			"Call Memory (Total)",
			"Call Memory (Own)",
			"Call Exceptions (Total)",
			"Call Exceptions (Own)",
			"Memory Allocs",
			"Memory Allocs (Memory)",
		];
		internal static string[] sortAssemblyOptions =
		[
			"Name",
			"Time",
			"Calls",
			"Memory",
			"Exceptions"
		];
		internal static string[] sortCallsOptions =
		[
			"Method",
			"Calls",
			"Time (Total)",
			"Time (Own)",
			"Memory (Total)",
			"Memory (Own)",
			"Exceptions (Total)",
			"Exceptions (Own)"
		];
		internal static string[] sortMemoryOptions =
		[
			"Type",
			"Allocations",
			"Memory"
		];

		public ProfilerTab(string id, string name, RustPlugin plugin, Action<PlayerSession, Tab> onChange = null) : base(id, name, plugin, onChange)
		{
			ColorUtility.TryParseHtmlString("#d13b38", out intenseColor);
			ColorUtility.TryParseHtmlString("#3882d1", out calmColor);
			ColorUtility.TryParseHtmlString("#60a848", out niceColor);
		}

		public static ProfilerTab GetOrCache(PlayerSession session) => _instance ??= Make(session);
		public static ProfilerTab Make(PlayerSession session)
		{
			if (sample.Assemblies == null)
			{
				sample = MonoProfiler.Sample.Create();
				sample.Clear();
			}

			var profiler = new ProfilerTab("profiler", "Profiler", Community.Runtime.Core);
			profiler.OnChange = (ap, _) =>
			{
				profiler.Draw(ap);
			};
			profiler.Over = (_, cui, container, parent, _) =>
			{
				var message = MonoProfiler.Crashed ? "<b>Mono profiler has failed initializing properly</b>\nPlease ensure " +
#if UNIX
				                                     "libCarbonNative.so"
#else
				                                     "CarbonNative.dll"
#endif
													+ " is located in <b>carbon/native</b> or contact developers" :
						!MonoProfiler.Enabled ? "<b>Mono profiler is disabled</b>\nEnable it in the config, then reboot the server" : null;

				if (string.IsNullOrEmpty(message))
				{
					return;
				}

				var blur = cui.CreatePanel(container, parent, "0 0 0 0.5", blur: true);
				cui.CreateText(container, blur, "1 1 1 0.5", message, 10);
			};
			profiler.Draw(session);

			return profiler;
		}

		public static IEnumerable<MonoProfiler.AssemblyRecord> GetSortedAssemblies(int sort, string search)
		{
			if (sample.Assemblies == null)
			{
				return default;
			}

			return (sort switch
			{
				0 => sample.Assemblies.OrderBy(x => x.assembly_name.GetDisplayName(sample.IsCleared)),
				1 => sample.Assemblies.OrderByDescending(x => x.total_time),
				2 => sample.Assemblies.OrderByDescending(x => x.calls),
				3 => sample.Assemblies.OrderByDescending(x => x.alloc),
				4 => sample.Assemblies.OrderByDescending(x => x.total_exceptions),
				_ => default
			})!.Where(x => string.IsNullOrEmpty(search) || x.assembly_name.GetDisplayName(sample.IsCleared).Contains(search, CompareOptions.OrdinalIgnoreCase));
		}
		public static IEnumerable<MonoProfiler.CallRecord> GetSortedCalls(string assembly, int sort, string search)
		{
			if (sample.Calls == null)
			{
				return default;
			}

			var advancedRecords = sample.Calls.Where(x => string.IsNullOrEmpty(assembly) || x.assembly_name.name == assembly);

			if (!advancedRecords.Any())
			{
				return advancedRecords;
			}

			return (sort switch
			{
				0 => advancedRecords.OrderBy(x => x.method_name),
				1 => advancedRecords.OrderByDescending(x => x.calls),
				2 => advancedRecords.OrderByDescending(x => x.total_time),
				3 => advancedRecords.OrderByDescending(x => x.own_time),
				4 => advancedRecords.OrderByDescending(x => x.total_alloc),
				5 => advancedRecords.OrderByDescending(x => x.own_alloc),
				6 => advancedRecords.OrderByDescending(x => x.total_exceptions),
				7 => advancedRecords.OrderByDescending(x => x.own_exceptions),
				_ => advancedRecords
			})!.Where(x => string.IsNullOrEmpty(search) || x.method_name.Contains(search, CompareOptions.OrdinalIgnoreCase));
		}
		public static IEnumerable<MonoProfiler.MemoryRecord> GetSortedMemory(int sort, string search)
		{
			if (sample.Memory == null)
			{
				return default;
			}

			var records = sample.Memory.AsEnumerable();

			if (!records.Any()) return records;

			return (sort switch
			{
				0 => records.OrderBy(x => x.class_name),
				1 => records.OrderByDescending(x => x.allocations),
				2 => records.OrderByDescending(x => x.total_alloc_size),
				_ => records
			})!.Where(x => string.IsNullOrEmpty(search) || x.class_name.Contains(search, CompareOptions.OrdinalIgnoreCase));
		}

		internal void Draw(PlayerSession ap)
		{
			var selection = ap.GetStorage<string>(null, "profilerval");

			DrawSubtabs(ap, selection);
			DrawAssemblies(ap, selection);
		}

		static void Stripe(Tab tab,
			int column,
			float value,
			float maxValue,
			Color intenseColor,
			Color calmColor,
			string title,
			string subtitle,
			string side,
			string command,
			bool selected = false)
		{
			if (maxValue <= value)
			{
				maxValue = value;
			}

			tab.AddWidget(column, 0, (ap, cui, container, parent) =>
			{
				var percentage = value.Scale(0, maxValue, 0f, 1f);
				var color = Color32.Lerp(calmColor, intenseColor, percentage);

				var button = string.IsNullOrEmpty(command) ?
					cui.CreatePanel(container, parent, "0.15 0.15 0.15 0.7",
						xMin: 0.01f, xMax: 0.99f).Id :
					cui.CreateProtectedButton(container, parent, "0.15 0.15 0.15 0.7", Cache.CUI.BlankColor,
					string.Empty, 0, xMin: 0.01f, xMax: 0.99f, command: command).Id;

				var bar = cui.CreatePanel(container, button, $"#{ColorUtility.ToHtmlStringRGB(color)}",
					xMax: percentage);

				if (selected)
				{
					cui.CreatePanel(container, button, "#d13b38", xMax: 0.005f);
				}

				cui.CreateText(container, button, Cache.CUI.WhiteColor, title, 9, xMin: selected ? 0.02f : 0.01f,
					yMax: 0.9f, align: TextAnchor.UpperLeft, font: CUI.Handler.FontTypes.RobotoCondensedRegular);
				cui.CreateText(container, button, "1 1 1 0.6", subtitle, 8, xMin: selected ? 0.02f : 0.01f, yMin: 0.05f,
					align: TextAnchor.LowerLeft, font: CUI.Handler.FontTypes.RobotoCondensedRegular);
				cui.CreateText(container, button, "1 1 1 0.2", side, 8, xMax: 0.99f, yMin: 0.05f,
					align: TextAnchor.MiddleRight, font: CUI.Handler.FontTypes.RobotoCondensedRegular);

				cui.CreateImage(container, bar, "fade", Cache.CUI.WhiteColor);
			});
		}

		public void DrawAssemblies(PlayerSession session, string assembly)
		{
			AddColumn(0, true);

			var timelineMode = session.GetStorage<bool>(null, "timeline");

			if (timelineMode)
			{
				DrawTimeline(session);
				return;
			}

			var searchInput = session.GetStorage(this, "bsearch", string.Empty);
			var sortIndex = session.GetStorage(this, "bsort", 1);
			var filtered = Pool.Get<List<MonoProfiler.AssemblyRecord>>();
			var maxVal = 0f;

			filtered.AddRange(GetSortedAssemblies(sortIndex, searchInput));

			if (filtered.Count > 0)
			{
				maxVal = sortIndex switch
				{
					0 or 1 => filtered.Max(x => (float)x.total_time),
					2 => filtered.Max(x => (float)x.calls),
					3 => filtered.Max(x => (float)x.alloc),
					4 => filtered.Max(x => (float)x.total_exceptions),
					_ => maxVal
				};
			}

			AddWidget(-1, 0, (ap, cui, container, panel) =>
			{
				var tabSpacing = 1;
				const float offset = -46f;

				cui.CreateProtectedButton(container, panel, "0.2 0.2 0.2 0.7", $"1 1 1 {(!timelineMode ? 0.2 : 0.5)}",
					"TIMELINE\nMODE", 8, xMin: 0.83f, xMax: 0.925f, OxMin: offset * tabSpacing, OxMax: offset * tabSpacing,
					command: "adminmodule.timelinemode");
				tabSpacing++;

				cui.CreateProtectedButton(container, panel, "0.2 0.2 0.2 0.7", $"1 1 1 {(sample.IsCompared ? 0.2 : 0.5)}",
					$"<size=6>{(!sample.IsCleared ? "COMPARE" : "IMPORT")}\n</size>PROTO", 8,
					xMin: 0.83f, xMax: 0.925f, OxMin: offset * tabSpacing, OxMax: offset * tabSpacing, command: "adminmodule.profilerimport");
				tabSpacing++;

				cui.CreateProtectedButton(container, panel, "0.2 0.2 0.2 0.7", $"1 1 1 {(sample.IsCleared ? 0.2 : 0.5)}",
					"<size=6>EXPORT\n</size>PROTO", 8,
					xMin: 0.83f, xMax: 0.925f, OxMin: offset * tabSpacing, OxMax: offset * tabSpacing, command: "adminmodule.profilerexport 3");
				tabSpacing++;

				cui.CreateProtectedButton(container, panel, "0.2 0.2 0.2 0.7", $"1 1 1 {(sample.IsCleared ? 0.2 : 0.5)}",
					"<size=6>EXPORT\n</size>CSV", 8,
					xMin: 0.83f, xMax: 0.925f, OxMin: offset * tabSpacing, OxMax: offset * tabSpacing, command: "adminmodule.profilerexport 2");
				tabSpacing++;

				cui.CreateProtectedButton(container, panel, "0.2 0.2 0.2 0.7", $"1 1 1 {(sample.IsCleared ? 0.2 : 0.5)}",
					"<size=6>EXPORT\n</size>JSON", 8,
					xMin: 0.83f, xMax: 0.925f, OxMin: offset * tabSpacing, OxMax: offset * tabSpacing, command: "adminmodule.profilerexport 1");
				tabSpacing++;

				cui.CreateProtectedButton(container, panel, "0.2 0.2 0.2 0.7", $"1 1 1 {(sample.IsCleared ? 0.2 : 0.5)}",
					"<size=6>EXPORT\n</size>TABLE", 8,
					xMin: 0.83f, xMax: 0.925f, OxMin: offset * tabSpacing, OxMax: offset * tabSpacing, command: "adminmodule.profilerexport 0");
				tabSpacing++;

				cui.CreateProtectedButton(container, panel, !sample.IsCleared || MonoProfiler.IsRecording ? "0.9 0.1 0.1 1" : "0.2 0.2 0.2 0.7", "1 1 1 0.5",
					MonoProfiler.IsRecording ? "ABORT" : "CLEAR", 8,
					xMin: 0.83f, xMax: 0.925f, command: "adminmodule.profilerclear");

				cui.CreateProtectedButton(container, panel,
					MonoProfiler.IsRecording ? "0.9 0.1 0.1 1" : "0.2 0.2 0.2 0.7", "1 1 1 0.5", "REC<size=6>\n[SHIFT]</size>", 8,
					xMin: 0.93f, xMax: 0.99f, command: "adminmodule.profilertoggle");
			});

			Stripe(this, 0, (float)filtered.Sum(x => x.total_time_percentage), 100, niceColor, niceColor,
				"All",
				$"{filtered.Sum(x => (float)x.total_time_ms):n0}ms | {filtered.Sum(x => (float)x.total_time_percentage):0.0}%",
				$"<size=7>{MonoProfiler.Sample.GetDifferenceString(sample.Comparison.Duration)}{TimeEx.Format(sample.Duration, false).ToLower()}\n{sample.Calls.Count:n0} calls</size>",
				$"adminmodule.profilerselect -1",
				string.IsNullOrEmpty(assembly));

			AddDropdown(0, $"<b>ASSEMBLIES ({sample.Assemblies.Count:n0})</b>", ap => sortIndex, (ap, i) =>
			{
				ap.SetStorage(this, "bsort", i);
				DrawAssemblies(session, assembly);
			}, sortAssemblyOptions);

			AddInputButton(0, "Search", 0.075f, new OptionInput(null, ap => searchInput, 0, false, (ap, args) =>
			{
				ap.SetStorage(this, "bsearch", args.ToString(" "));
				DrawAssemblies(ap, assembly);
			}), new OptionButton("X", ap =>
			{
				ap.SetStorage(this, "bsearch", string.Empty);
				DrawAssemblies(ap, assembly);

			}, _ => string.IsNullOrEmpty(searchInput) ? OptionButton.Types.None : OptionButton.Types.Important));

			for (int i = 0; i < filtered.Count; i++)
			{
				var record = filtered[i];

				var value = sortIndex switch
				{
					0 or 1 => record.total_time,
					2 => record.calls,
					3 => record.alloc,
					4 => record.total_exceptions,
					_ => 0f
				};

				Stripe(this, 0, value, maxVal, intenseColor, calmColor,
					record.assembly_name.GetDisplayName(record.comparison.isCompared),
					$"{MonoProfiler.Sample.GetDifferenceString(record.comparison.total_time)}{record.GetTotalTime()} ({record.total_time_percentage:0.0}%) | {MonoProfiler.Sample.GetDifferenceString(record.comparison.alloc)}{ByteEx.Format(record.alloc).ToUpper()} | {MonoProfiler.Sample.GetDifferenceString(record.comparison.total_exceptions)}{record.total_exceptions:n0} excep.",
					$"{record.assembly_name.profileType}\n{MonoProfiler.Sample.GetDifferenceString(record.comparison.calls)}<b>{record.calls:n0}</b> calls", $"adminmodule.profilerselect {i}", record.assembly_name.name == assembly);
			}

			if (filtered.Count == 0)
			{
				AddText(0, "No assemblies available", 8, "1 1 1 0.5");
			}

			Pool.FreeUnmanaged(ref filtered);
		}
		public void DrawSubtabs(PlayerSession session, string assembly)
		{
			AddColumn(1, true);

			var timelineMode = session.GetStorage<bool>(null, "timeline");

			if (timelineMode)
			{
				var timelineChartType = session.GetStorage<int>(this, "timelinect");

				AddSpace(1);
				AddDropdown(1, "Chart Options", ap => timelineChartType, (ap, i) =>
				{
					ap.SetStorage(this, "timelinect", i);
					DrawSubtabs(session, assembly);
					DrawAssemblies(session, assembly);
				}, timelineChartOptions);

				return;
			}

			var subtab = session.GetStorage<SubtabTypes>(this, "subtab");

			AddButtonArray(-2, new OptionButton("Calls", ap =>
				{
					session.SetStorage(this, "subtab", SubtabTypes.Calls);
					DrawSubtabs(session, assembly);
				}, ap => subtab == SubtabTypes.Calls ? OptionButton.Types.Selected : OptionButton.Types.None),
				new OptionButton("Memory", ap =>
				{
					session.SetStorage(this, "subtab", SubtabTypes.Memory);
					DrawSubtabs(session, assembly);
				}, ap => subtab == SubtabTypes.Memory ? OptionButton.Types.Selected : OptionButton.Types.None));

			Stripe(this, 1, 100, 100, niceColor, niceColor,
				"GC", $"{MonoProfiler.Sample.GetDifferenceString(sample.GC.comparison.calls_c)}{sample.GC.calls:n0} calls | {MonoProfiler.Sample.GetDifferenceString(sample.GC.comparison.total_time_c)}{sample.GC.GetTotalTime()}", string.Empty, null, true);

			switch (subtab)
			{
				case SubtabTypes.Memory:
				{
					var searchInput = session.GetStorage(this, "msearch", string.Empty);
					var sort = session.GetStorage(this, "msort", 1);
					var memoryRecords = GetSortedMemory(sort, searchInput);
					var maxVal = 0f;

					if (memoryRecords.Any())
					{
						maxVal = sort switch
						{
							0 or 1 => memoryRecords.Max(x => (float)x.allocations),
							2 => memoryRecords.Max(x => (float)x.total_alloc_size),
							_ => maxVal
						};
					}

					AddDropdown(1, $"<b>MEMORY ({memoryRecords.Count():n0})</b>", ap => sort, (ap, i) =>
					{
						ap.SetStorage(this, "msort", i);
						DrawSubtabs(session, assembly);
					}, sortMemoryOptions);

					AddInputButton(1, "Search", 0.075f, new OptionInput(null, ap => searchInput, 0, false, (ap, args) =>
					{
						ap.SetStorage(this, "msearch", args.ToString(" "));
						DrawSubtabs(ap, assembly);
					}), new OptionButton("X", ap =>
					{
						ap.SetStorage(this, "msearch", string.Empty);
						DrawSubtabs(ap, assembly);

					}, _ => string.IsNullOrEmpty(searchInput) ? OptionButton.Types.None : OptionButton.Types.Important));

					var index = 0;
					foreach (var record in memoryRecords)
					{
						var value = sort switch
						{
							0 or 1 => record.allocations,
							2 => record.total_alloc_size,
							_ => 0f
						};

						Stripe(this, 1, value, maxVal, intenseColor, calmColor,
							record.class_name,
							$"{MonoProfiler.Sample.GetDifferenceString(record.comparison.allocations)}{record.allocations:n0} allocated | {MonoProfiler.Sample.GetDifferenceString(record.comparison.total_alloc_size)}{ByteEx.Format(record.total_alloc_size).ToUpper()} total",
							$"<b>{record.instance_size} B</b>",
							string.Empty);

						index++;
					}

					if (!memoryRecords.Any())
					{
						AddText(1, "No memory records available", 8, "1 1 1 0.5");
					}

					break;
				}
				case SubtabTypes.Calls:
				{
					var searchInput = session.GetStorage(this, "asearch", string.Empty);
					var sort = session.GetStorage(this, "asort", 1);
					var advancedRecords = GetSortedCalls(assembly, sort, searchInput);
					var maxVal = 0f;

					if (advancedRecords.Any())
					{
						maxVal = sort switch
						{
							0 or 1 => advancedRecords.Max(x => (float)x.calls),
							2 => advancedRecords.Max(x => (float)x.total_time),
							3 => advancedRecords.Max(x => (float)x.own_time),
							4 => advancedRecords.Max(x => (float)x.total_alloc),
							5 => advancedRecords.Max(x => (float)x.own_alloc),
							6 => advancedRecords.Max(x => (float)x.total_exceptions),
							7 => advancedRecords.Max(x => (float)x.own_exceptions),
							_ => maxVal
						};
					}

					AddDropdown(1, $"<b>CALLS ({advancedRecords.Count():n0})</b>", ap => sort, (ap, i) =>
					{
						ap.SetStorage(this, "asort", i);
						DrawSubtabs(session, assembly);
					}, sortCallsOptions);

					AddInputButton(1, "Search", 0.075f, new OptionInput(null, ap => searchInput, 0, false, (ap, args) =>
					{
						ap.SetStorage(this, "asearch", args.ToString(" "));
						DrawSubtabs(ap, assembly);
					}), new OptionButton("X", ap =>
					{
						ap.SetStorage(this, "asearch", string.Empty);
						DrawSubtabs(ap, assembly);

					}, _ => string.IsNullOrEmpty(searchInput) ? OptionButton.Types.None : OptionButton.Types.Important));

					var index = 0;
					foreach (var record in advancedRecords)
					{
						var value = sort switch
						{
							0 or 1 => record.calls,
							2 => record.total_time,
							3 => record.own_time,
							4 => record.total_alloc,
							5 => record.own_alloc,
							6 => record.total_exceptions,
							7 => record.own_exceptions,
							_ => 0f
						};

						Stripe(this, 1, value, maxVal, intenseColor, calmColor,
							record.method_name.Truncate(105, "..."),
							$"{MonoProfiler.Sample.GetDifferenceString(record.comparison.total_time)}{record.GetTotalTime()} total ({record.total_time_percentage:0.0}%) | {MonoProfiler.Sample.GetDifferenceString(record.comparison.own_time)}{record.GetOwnTime()} own ({record.own_time_percentage:0.0}%) | {MonoProfiler.Sample.GetDifferenceString(record.comparison.total_exceptions)}{record.total_exceptions:n0} total / {MonoProfiler.Sample.GetDifferenceString(record.comparison.own_exceptions)}{record.own_exceptions:n0} own excep.",
							$"{MonoProfiler.Sample.GetDifferenceString(record.comparison.calls)}<b>{record.calls:n0}</b> {((record.calls).Plural("call", "calls"))}\n{MonoProfiler.Sample.GetDifferenceString(record.comparison.total_alloc)}{ByteEx.Format(record.total_alloc).ToUpper()} total | {MonoProfiler.Sample.GetDifferenceString(record.comparison.own_alloc)}{ByteEx.Format(record.own_alloc).ToUpper()} own",
							Community.Runtime.MonoProfilerConfig.SourceViewer && !sample.FromDisk ? $"adminmodule.profilerselectcall {index}" : string.Empty);

						index++;
					}

					if (!advancedRecords.Any())
					{
						AddText(1, "No call records available", 8, "1 1 1 0.5");
					}

					break;
				}
			}
		}
		public void DrawTimeline(PlayerSession session)
		{
			var timelineChartType = session.GetStorage<int>(this, "timelinect");

			AddWidget(0, 0, (ap, cui, container, panel) =>
			{
				var tabSpacing = 1;
				const float offset = -46f;

				cui.CreateProtectedButton(container, panel, "0.2 0.2 0.2 0.7", $"1 1 1 0.5",
					"TIMELINE\nMODE", 8, xMin: 0.83f, xMax: 0.925f, OxMin: offset * tabSpacing, OxMax: offset * tabSpacing,
					command: "adminmodule.timelinemode");
				tabSpacing++;

				cui.CreateProtectedButton(container, panel, !recording.IsDiscarded() || recording.IsRecording() ? "0.9 0.1 0.1 1" : "0.2 0.2 0.2 0.7", "1 1 1 0.5",
					recording.IsRecording() ? "ABORT" : "CLEAR", 8,
					xMin: 0.83f, xMax: 0.925f, command: "adminmodule.timelineclear");

				cui.CreateProtectedButton(container, panel,
					recording.IsRecording() ? "0.9 0.1 0.1 1" : "0.2 0.2 0.2 0.7", "1 1 1 0.5", "REC", 8,
					xMin: 0.93f, xMax: 0.99f, command: "adminmodule.timelinetoggle");
			});

			Components.Graphics.Chart.ChartSettings settings = default;
			settings.HorizontalLabels = true;

			Components.Graphics.Chart.Layer[] layers = default;
			string[] vLabels = default;
			string[] hLabels = default;

			switch (timelineChartType)
			{
				case 0:
					GenerateProfilerDataChart_Assembly(recording, assembly => assembly.calls,
						value => value.ToString("n0"),
						5, 7, out layers, out vLabels, out hLabels);
					break;

				case 1:
					GenerateProfilerDataChart_Assembly(recording, assembly => assembly.alloc,
						value => ByteEx.Format(value).ToUpper(),
						5, 7, out layers, out vLabels, out hLabels);
					break;

				case 2:
					GenerateProfilerDataChart_Assembly(recording, assembly => (ulong)assembly.total_time_ms,
						value => $"{value:n0}ms",
						5, 7, out layers, out vLabels, out hLabels);
					break;

				case 3:
					GenerateProfilerDataChart_Assembly(recording, assembly => assembly.total_exceptions,
						value => value.ToString("n0"),
						3, 5, out layers, out vLabels, out hLabels);
					break;

				case 4:
					GenerateProfilerDataChart_Call(recording, call => call.calls, assembly => assembly.calls,
						value => value.ToString("n0"),
						5, 7, out layers, out vLabels, out hLabels);
					break;

				case 5:
					GenerateProfilerDataChart_Call(recording, call => (ulong)call.total_time_ms, assembly => (ulong)assembly.total_time_ms,
						value => $"{value:n0}ms",
						5, 7, out layers, out vLabels, out hLabels);
					break;

				case 6:
					GenerateProfilerDataChart_Call(recording, call => (ulong)call.own_time_ms, assembly => (ulong)assembly.total_time_ms,
						value => $"{value:n0}ms",
						5, 7, out layers, out vLabels, out hLabels);
					break;

				case 7:
					GenerateProfilerDataChart_Call(recording, call => call.total_alloc, assembly => assembly.alloc,
						value => ByteEx.Format(value).ToUpper(),
						5, 7, out layers, out vLabels, out hLabels);
					break;

				case 8:
					GenerateProfilerDataChart_Call(recording, call => call.own_alloc, assembly => assembly.alloc,
						value => ByteEx.Format(value).ToUpper(),
						5, 7, out layers, out vLabels, out hLabels);
					break;

				case 9:
					GenerateProfilerDataChart_Call(recording, call => call.total_exceptions, assembly => assembly.total_exceptions,
						value => value.ToString("n0"),
						3, 5, out layers, out vLabels, out hLabels);
					break;

				case 10:
					GenerateProfilerDataChart_Call(recording, call => call.own_exceptions, assembly => assembly.total_exceptions,
						value => value.ToString("n0"),
						3, 5, out layers, out vLabels, out hLabels);
					break;

				case 11:
					GenerateProfilerDataChart_Memory(recording, memory => memory.total_alloc_size,
						value => ByteEx.Format(value).ToUpper(),
						6, 6, out layers, out vLabels, out hLabels);
					break;

				case 12:
					GenerateProfilerDataChart_Memory(recording, memory => memory.total_alloc_size,
						value => ByteEx.Format(value).ToUpper(),
						6, 6, out layers, out vLabels, out hLabels);
					break;

				case 13:
					GenerateProfilerDataChart_Memory(recording, memory => memory.allocations,
						value => ByteEx.Format(value).ToUpper(),
						6, 6, out layers, out vLabels, out hLabels);
					break;
			}

			AddChart(0, timelineChartOptions[timelineChartType], TextAnchor.UpperLeft, 18, layers, vLabels, hLabels,
				settings, responsive: false);

			AddName(0, "Recording Info");
			AddInput(0, "Status", ap => recording.Status.ToString());
			AddInput(0, "Duration", ap => $"{recording.Duration:0.0}s ({recording.Rate:0.0}s rate)");
			AddInput(0, "Flags", ap => recording.Args.ToString());
			AddName(0, $"Samples ({recording.Timeline.Count:n0})");

			var assemblies = recording.Timeline.Sum(x => x.Value.Assemblies.Count);
			var calls = recording.Timeline.Sum(x => x.Value.Calls.Count);
			var memory = recording.Timeline.Sum(x => x.Value.Memory.Count);
			var highest = 0;

			if (assemblies > highest) highest = assemblies;
			if (calls > highest) highest = calls;
			if (memory > highest) highest = memory;

			Stripe(session.SelectedTab, 0, assemblies, highest, intenseColor, niceColor,
				"Assemblies", $"{(recording.Timeline.Any() ? recording.Timeline.Max(x => x.Value.Assemblies.Max(y => y.calls)) : 0):n0} calls | " +
				              $"{ByteEx.Format((recording.Timeline.Any() ? recording.Timeline.Max(x => x.Value.Assemblies.Max(y => y.alloc)) : 0)).ToUpper()} allocs. | " +
				              $"{(recording.Timeline.Any() ? recording.Timeline.Max(x => x.Value.Assemblies.Max(y => y.total_time_ms)) : 0):n0}ms time | " +
				              $"{(recording.Timeline.Any() ? recording.Timeline.Max(x => x.Value.Assemblies.Max(y => y.total_exceptions)) : 0):n0} excep.",
				assemblies.ToString("n0"), null);

			Stripe(session.SelectedTab, 0, calls, highest, intenseColor, niceColor,
				"Calls", $"{(recording.Timeline.Any() ? recording.Timeline.Max(x => x.Value.Calls.Max(y => y.calls)) : 0):n0} calls | " +
				         $"{ByteEx.Format((recording.Timeline.Any() ? recording.Timeline.Max(x => x.Value.Calls.Max(y => y.total_alloc)) : 0)).ToUpper()} total / " +
				         $"{ByteEx.Format((recording.Timeline.Any() ? recording.Timeline.Max(x => x.Value.Calls.Max(y => y.own_alloc)) : 0)).ToUpper()} own allocs. | " +
				         $"{(recording.Timeline.Any() ? recording.Timeline.Max(x => x.Value.Calls.Max(y => y.total_time_ms)) : 0):n0}ms total / " +
				         $"{(recording.Timeline.Any() ? recording.Timeline.Max(x => x.Value.Calls.Max(y => y.own_time_ms)) : 0):n0}ms own time | " +
				         $"{(recording.Timeline.Any() ? recording.Timeline.Max(x => x.Value.Calls.Max(y => y.total_exceptions)) : 0):n0} total / " +
				         $"{(recording.Timeline.Any() ? recording.Timeline.Max(x => x.Value.Calls.Max(y => y.own_exceptions)) : 0):n0} own excep.",
				calls.ToString("n0"), null);

			Stripe(session.SelectedTab, 0, memory, highest, intenseColor, niceColor,
				"Memory", $"{(recording.Timeline.Any() ? recording.Timeline.Max(x => x.Value.Memory.Max(y => y.allocations)) : 0):n0} allocs. | " +
				          $"{ByteEx.Format((recording.Timeline.Any() ? recording.Timeline.Max(x => x.Value.Memory.Max(y => y.total_alloc_size)) : 0)).ToUpper()} total alloc. | " +
				          $"{ByteEx.Format((recording.Timeline.Any() ? recording.Timeline.Max(x => x.Value.Memory.Max(y => y.instance_size)) : 0)).ToUpper()} inst. size",
				memory.ToString("n0"), null);
		}

		public static void GenerateProfilerDataChart_Assembly(MonoProfiler.TimelineRecording recording,
			Func<MonoProfiler.AssemblyRecord, ulong> value, Func<ulong, string> valueFormat,
			int valueCuts, int assemblyCount, out Components.Graphics.Chart.Layer[] layers, out string[] vLabels, out string[] hLabels)
		{
			var pooledLayers = Pool.Get<List<Components.Graphics.Chart.Layer>>();
			var pooledVerticalLabels = Pool.Get<List<string>>();
			var pooledHorizontalLabels = Pool.Get<List<string>>();
			pooledHorizontalLabels.AddRange(recording.Timeline.Select(sample => $"{sample.Key.Hour:00}:{sample.Key.Minute:00}:{sample.Key.Second:00}"));

			var records = recording.Timeline.SelectMany(x => x.Value.Assemblies.OrderByDescending(value));

			var maxValue = records.Any() ? records.Max(value) : 0;

			if (records.Any())
			{
				for (int i = 0; i < valueCuts; i++)
				{
					pooledVerticalLabels.Add(valueFormat(((ulong)i).Scale(0, (ulong)valueCuts, 0, maxValue)));
				}
			}

			pooledVerticalLabels.Add(valueFormat(maxValue));

			var index = 0;
			foreach (var assembly in records.Take(assemblyCount))
			{
				var color = ChartColors[index];

				pooledLayers.Add(new Components.Graphics.Chart.Layer
				{
					Name = assembly.assembly_name.GetDisplayName(false),
					Data = recording.Timeline.Select(x => x.Value.Assemblies.Where(x => x.assembly_handle == assembly.assembly_handle).SumULong(value)).ToArray(),
					LayerSettings = new()
					{
						Color = color,
						Shadows = 0
					}
				});

				index++;
			}

			layers = [.. pooledLayers];
			vLabels = [.. pooledVerticalLabels];
			hLabels = [.. pooledHorizontalLabels];

			Pool.FreeUnmanaged(ref pooledVerticalLabels);
			Pool.FreeUnmanaged(ref pooledHorizontalLabels);
			Pool.FreeUnmanaged(ref pooledLayers);
		}

		public static void GenerateProfilerDataChart_Call(MonoProfiler.TimelineRecording recording,
			Func<MonoProfiler.CallRecord, ulong> callValue, Func<MonoProfiler.AssemblyRecord, ulong> assemblyValue, Func<ulong, string> valueFormat,
			int valueCuts, int callCount, out Components.Graphics.Chart.Layer[] layers, out string[] vLabels, out string[] hLabels)
		{
			var pooledLayers = Pool.Get<List<Components.Graphics.Chart.Layer>>();
			var pooledVerticalLabels = Pool.Get<List<string>>();
			var pooledHorizontalLabels = Pool.Get<List<string>>();
			pooledHorizontalLabels.AddRange(recording.Timeline.Select(sample => $"{sample.Key.Hour:00}:{sample.Key.Minute:00}:{sample.Key.Second:00}"));

			var records = recording.Timeline.SelectMany(x =>
				x.Value.Assemblies.OrderByDescending(assemblyValue));

			var maxValue = records.Any() ? records.Max(assemblyValue) : 0;

			if (records.Any())
			{
				for (int i = 0; i < valueCuts; i++)
				{
					pooledVerticalLabels.Add(valueFormat(((ulong)i).Scale(0, (ulong)valueCuts, 0, maxValue)));
				}
			}

			pooledVerticalLabels.Add(valueFormat(maxValue));

			var index = 0;
			foreach (var assembly in records.Take(callCount))
			{
				var color = ChartColors[index];

				MonoProfiler.AssemblyMap.TryGetValue(assembly.assembly_handle, out var name);

				pooledLayers.Add(new Components.Graphics.Chart.Layer
				{
					Name = name.GetDisplayName(false),
					Data = recording.Timeline.Select(x => x.Value.Calls.Where(x => x.assembly_handle == assembly.assembly_handle).SumULong(callValue)).ToArray(),
					LayerSettings = new()
					{
						Color = color,
						Shadows = 0
					}
				});

				index++;
			}

			layers = [.. pooledLayers];
			vLabels = [.. pooledVerticalLabels];
			hLabels = [.. pooledHorizontalLabels];

			Pool.FreeUnmanaged(ref pooledVerticalLabels);
			Pool.FreeUnmanaged(ref pooledHorizontalLabels);
			Pool.FreeUnmanaged(ref pooledLayers);
		}

		public static void GenerateProfilerDataChart_Memory(MonoProfiler.TimelineRecording recording,
			Func<MonoProfiler.MemoryRecord, ulong> value, Func<ulong, string> valueFormat,
			int valueCuts, int memoryCount, out Components.Graphics.Chart.Layer[] layers, out string[] vLabels, out string[] hLabels)
		{
			var pooledLayers = Pool.Get<List<Components.Graphics.Chart.Layer>>();
			var pooledVerticalLabels = Pool.Get<List<string>>();
			var pooledHorizontalLabels = Pool.Get<List<string>>();
			pooledHorizontalLabels.AddRange(recording.Timeline.Select(sample => $"{sample.Key.Hour:00}:{sample.Key.Minute:00}:{sample.Key.Second:00}"));

			var records = recording.Timeline.SelectMany(x =>
				x.Value.Memory.OrderByDescending(value));

			var maxValue = records.Any() ? records.Max(value) : 0;

			if (records.Any())
			{
				for (int i = 0; i < valueCuts; i++)
				{
					pooledVerticalLabels.Add(valueFormat(((ulong)i).Scale(0, (ulong)valueCuts, 0, maxValue)));
				}
			}

			pooledVerticalLabels.Add(valueFormat(maxValue));

			var index = 0;
			foreach (var assembly in records.Take(memoryCount))
			{
				var color = ChartColors[index];

				pooledLayers.Add(new Components.Graphics.Chart.Layer
				{
					Name = assembly.class_name,
					Data = recording.Timeline.Select(x => x.Value.Memory.Where(x => x.assembly_handle == assembly.assembly_handle).SumULong(value)).ToArray(),
					LayerSettings = new()
					{
						Color = color,
						Shadows = 0
					}
				});

				index++;
			}

			layers = [.. pooledLayers];
			vLabels = [.. pooledVerticalLabels];
			hLabels = [.. pooledHorizontalLabels];

			Pool.FreeUnmanaged(ref pooledVerticalLabels);
			Pool.FreeUnmanaged(ref pooledHorizontalLabels);
			Pool.FreeUnmanaged(ref pooledLayers);
		}
	}

	[Conditional("!MINIMAL")]
	[ProtectedCommand("adminmodule.profilerselect")]
	private void ProfilerSelect(ConsoleSystem.Arg arg)
	{
		var ap = GetPlayerSession(arg.Player());

		var selection = ProfilerTab.GetSortedAssemblies(ap.GetStorage(ap.SelectedTab, "bsort", 1), ap.GetStorage(ap.SelectedTab, "bsearch", string.Empty))
			.FindAt(arg.GetInt(0));
		ap.SetStorage(null, "profilerval", selection.assembly_name == null ? string.Empty : selection.assembly_name.name);
		ap.SelectedTab.OnChange(ap, ap.SelectedTab);

		Draw(ap.Player);
	}

	[Conditional("!MINIMAL")]
	[ProtectedCommand("adminmodule.profilerselectcall")]
	private void ProfilerSelectCall(ConsoleSystem.Arg arg)
	{
		var player = arg.Player();

		if (!HasAccess(player, "profiler.sourceviewer") || ProfilerTab.sample.FromDisk)
		{
			return;
		}

		var index = arg.GetInt(0);
		var ap = GetPlayerSession(player);

		var assembly = ap.GetStorage<string>(null, "profilerval");
		var call = ProfilerTab.GetSortedCalls(assembly, ap.GetStorage(ap.SelectedTab, "asort", 1), ap.GetStorage(ap.SelectedTab, "asearch", string.Empty))
			.FindAt(index);

		var currentTab = ap.SelectedTab;
		var tab = SourceViewerTab.MakeMethod(call);

		tab.Close = ap =>
		{
			SetTab(player, currentTab, true);
		};

		SetTab(player, tab, true);
	}

	[Conditional("!MINIMAL")]
	[ProtectedCommand("adminmodule.profilertoggle")]
	private void ProfilerToggle(ConsoleSystem.Arg arg)
	{
		var player = arg.Player();

		if (!HasAccess(player, "profiler.startstop"))
		{
			return;
		}

		var ap = GetPlayerSession(player);

		if (!MonoProfiler.Enabled)
		{
			return;
		}

		if (!MonoProfiler.IsRecording && ap.Player.serverInput.IsDown(BUTTON.SPRINT))
		{
			var dictionary = Pool.Get<Dictionary<string, ModalModule.Modal.Field>>();

			dictionary["duration"] = ModalModule.Modal.Field.Make("Duration", ModalModule.Modal.Field.FieldTypes.Float, true, 3f, customIsInvalid: field => field.Value.ToString().ToFloat() <= 0 ? "Duration must be above zero." : string.Empty);
			dictionary["calls"] = ModalModule.Modal.Field.Make("Calls", ModalModule.Modal.Field.FieldTypes.Boolean, false, true);
			dictionary["advancedmemory"] = ModalModule.Modal.Field.Make("Advanced Memory", ModalModule.Modal.Field.FieldTypes.Boolean, false, true);
			dictionary["callmemory"] = ModalModule.Modal.Field.Make("Call Memory", ModalModule.Modal.Field.FieldTypes.Boolean, false, true);
			dictionary["swa"] = ModalModule.Modal.Field.Make("Stack Walk Allocations", ModalModule.Modal.Field.FieldTypes.Boolean, false, true);
			dictionary["gc"] = ModalModule.Modal.Field.Make("GC Events", ModalModule.Modal.Field.FieldTypes.Boolean, false, true);
			dictionary["timings"] = ModalModule.Modal.Field.Make("Timings (Performance Intensive)", ModalModule.Modal.Field.FieldTypes.Boolean, false, true);

			Modal.Open(player, "Profile Recording", dictionary, (_, _) =>
			{
				MonoProfiler.ProfilerArgs profilerArgs = default;

				if (dictionary["advancedmemory"].Get<bool>()) profilerArgs |= MonoProfiler.ProfilerArgs.AdvancedMemory;
				if (dictionary["callmemory"].Get<bool>()) profilerArgs |= MonoProfiler.ProfilerArgs.CallMemory;
				if (dictionary["calls"].Get<bool>()) profilerArgs |= MonoProfiler.ProfilerArgs.Calls;
				if (dictionary["timings"].Get<bool>()) profilerArgs |= MonoProfiler.ProfilerArgs.Timings;
				if (dictionary["gc"].Get<bool>()) profilerArgs |= MonoProfiler.ProfilerArgs.GCEvents;
				if (dictionary["swa"].Get<bool>()) profilerArgs |= MonoProfiler.ProfilerArgs.StackWalkAllocations;

				var duration = dictionary["duration"].Get<float>();

				MonoProfiler.Clear();
				MonoProfiler.ToggleProfilingTimed(duration, profilerArgs, args =>
				{
					if (ap.IsInMenu && ap.SelectedTab != null && ap.SelectedTab.Id == "profiler")
					{
						ap.SelectedTab.OnChange(ap, ap.SelectedTab);
						Draw(ap.Player);
					}

					ProfilerTab.sample.Resample();
					Analytics.profiler_ended(profilerArgs, ProfilerTab.sample.Duration, true);
				}, false);

				Analytics.profiler_started(profilerArgs, true);

				Pool.FreeUnmanaged(ref dictionary);

				ap.SelectedTab.OnChange(ap, ap.SelectedTab);
				Draw(player);
			}, onCancel: () =>
			{
				Pool.FreeUnmanaged(ref dictionary);

				ap.SelectedTab.OnChange(ap, ap.SelectedTab);
				Draw(player);
			});
		}
		else
		{
			MonoProfiler.ToggleProfiling(logging: false);

			if (!MonoProfiler.IsRecording)
			{
				ProfilerTab.sample.Resample();
				MonoProfiler.Clear();
				Analytics.profiler_ended(MonoProfiler.AllFlags, ProfilerTab.sample.Duration, false);
			}

			ap.SelectedTab.OnChange(ap, ap.SelectedTab);

			Draw(player);
		}
	}

	[Conditional("!MINIMAL")]
	[ProtectedCommand("adminmodule.profilerexport")]
	private void ProfilerExport(ConsoleSystem.Arg arg)
	{
		if (ProfilerTab.sample.IsCleared)
		{
			return;
		}

		var ap = GetPlayerSession(arg.Player());
		var index = arg.GetInt(0);

		switch (index)
		{
			case 0:
				WriteFileString("txt", ProfilerTab.sample.ToTable(), ap.Player);
				break;

			case 1:
				WriteFileString("json", ProfilerTab.sample.ToJson(true), ap.Player);
				break;

			case 2:
				WriteFileString("csv", ProfilerTab.sample.ToCSV(), ap.Player);
				break;

			case 3:
				WriteFileBytes(MonoProfiler.ProfileExtension, ProfilerTab.sample.ToProto(), ap.Player);
				break;
		}

		static void WriteFileString(string extension, string data, BasePlayer player)
		{
			var date = DateTime.Now;
			var file = Path.Combine(Defines.GetProfilesFolder(), $"profile-{date.Year}_{date.Month}_{date.Day}_{date.Hour}{date.Minute}{date.Second}.{extension}");
			OsEx.File.Create(file, data);

			Notifications.Add(player, $"Exported profile output at '{file}'");
		}
		static void WriteFileBytes(string extension, byte[] data, BasePlayer player)
		{
			var date = DateTime.Now;
			var file = Path.Combine(Defines.GetProfilesFolder(), $"profile-{date.Year}_{date.Month}_{date.Day}_{date.Hour}{date.Minute}{date.Second}.{extension}");
			OsEx.File.Create(file, data);

			Notifications.Add(player, $"Exported profile output at '{file}'");
		}
	}

	[Conditional("!MINIMAL")]
	[ProtectedCommand("adminmodule.profilerimport")]
	private void ProfilerImport(ConsoleSystem.Arg arg)
	{
		if (ProfilerTab.sample.IsCompared)
		{
			return;
		}

		var player = arg.Player();

		File.Open(player, "Profiles", Defines.GetProfilesFolder(), Defines.GetProfilesFolder(), MonoProfiler.ProfileExtension,
			onConfirm: (player, file) =>
			{
				using var buffer = TempArray<byte>.New(OsEx.File.ReadBytes(file.SelectedFile));

				if (ProfilerTab.sample.IsCleared)
				{
					ProfilerTab.sample = MonoProfiler.Sample.Load(buffer.array);
				}
				else
				{
					var comparingSample = MonoProfiler.Sample.Load(buffer.array);
					ProfilerTab.sample = ProfilerTab.sample.Compare(comparingSample);
				}

				var ap = Singleton.GetPlayerSession(player);
				ap.SelectedTab.OnChange(ap, ap.SelectedTab);
				Singleton.Draw(player);
			}, onExtraInfo: item =>
			{
				if (item.IsDirectory)
				{
					return string.Empty;
				}

				if (MonoProfiler.ValidateFile(item.Path, out var protocol, out var duration, out var isCompared))
				{
					return $"Duration: {TimeEx.FormatPlayer(duration).ToLower()}s (protocol {protocol}){(isCompared ? " [C]" : string.Empty)}";
				}

				return $"Invalid protocol {protocol}";
			});
	}

	[Conditional("!MINIMAL")]
	[ProtectedCommand("adminmodule.profilerclear")]
	private void ProfilerClear(ConsoleSystem.Arg arg)
	{
		var ap = GetPlayerSession(arg.Player());

		if (MonoProfiler.IsRecording)
		{
			MonoProfiler.ToggleProfiling(MonoProfiler.ProfilerArgs.Abort);
		}
		else
		{
			ProfilerTab.sample.Clear();
			MonoProfiler.Clear();
			ap.SetStorage(null, "profilerval", string.Empty);
		}

		ap.SelectedTab.OnChange(ap, ap.SelectedTab);

		Draw(ap.Player);
	}

	[Conditional("!MINIMAL")]
	[ProtectedCommand("adminmodule.timelinemode")]
	private void TimelineMode(ConsoleSystem.Arg arg)
	{
		var ap = GetPlayerSession(arg.Player());

		ap.SetStorage(null, "timeline", !ap.GetStorage<bool>(null, "timeline"));

		ap.SelectedTab.OnChange(ap, ap.SelectedTab);

		Draw(ap.Player);
	}

	[Conditional("!MINIMAL")]
	[ProtectedCommand("adminmodule.timelinetoggle")]
	private void TimelineToggle(ConsoleSystem.Arg arg)
	{
		var player = arg.Player();

		if (!HasAccess(player, "profiler.startstop"))
		{
			return;
		}

		var ap = GetPlayerSession(player);

		if (!MonoProfiler.Enabled)
		{
			return;
		}

		if (!MonoProfiler.IsRecording)
		{
			var dictionary = Pool.Get<Dictionary<string, ModalModule.Modal.Field>>();
			dictionary["duration"] = ModalModule.Modal.Field.Make("Duration", ModalModule.Modal.Field.FieldTypes.Float, true, 3f,
				customIsInvalid: field => field.Get<float>() <= 0 ? "Duration must be above zero." : field.Get<float>() > 100 ? $"You cannot record above {TimeEx.Format(100, shortName: false).ToLower()}." : string.Empty);
			dictionary["rate"] = ModalModule.Modal.Field.Make("Rate", ModalModule.Modal.Field.FieldTypes.Float, true, 1f,
				customIsInvalid: field => field.Get<float>() < 1 ? "Rate must be above or equal to one second." : field.Get<float>() > 10 ? $"Rate must be under or equal to 10 seconds." : string.Empty);
			dictionary["calls"] = ModalModule.Modal.Field.Make("Calls", ModalModule.Modal.Field.FieldTypes.Boolean, false, true);
			dictionary["advancedmemory"] = ModalModule.Modal.Field.Make("Advanced Memory", ModalModule.Modal.Field.FieldTypes.Boolean, false, true);
			dictionary["callmemory"] = ModalModule.Modal.Field.Make("Call Memory", ModalModule.Modal.Field.FieldTypes.Boolean, false, true);
			dictionary["swa"] = ModalModule.Modal.Field.Make("Stack Walk Allocations", ModalModule.Modal.Field.FieldTypes.Boolean, false, true);
			dictionary["timings"] = ModalModule.Modal.Field.Make("Timings (Performance Intensive)", ModalModule.Modal.Field.FieldTypes.Boolean, false, true);

			Modal.Open(player, "Timeline Profiling", dictionary, (player, _) =>
			{
				MonoProfiler.ProfilerArgs profilerArgs = default;

				if (dictionary["advancedmemory"].Get<bool>()) profilerArgs |= MonoProfiler.ProfilerArgs.AdvancedMemory;
				if (dictionary["callmemory"].Get<bool>()) profilerArgs |= MonoProfiler.ProfilerArgs.CallMemory;
				if (dictionary["calls"].Get<bool>()) profilerArgs |= MonoProfiler.ProfilerArgs.Calls;
				if (dictionary["timings"].Get<bool>()) profilerArgs |= MonoProfiler.ProfilerArgs.Timings;
				if (dictionary["swa"].Get<bool>()) profilerArgs |= MonoProfiler.ProfilerArgs.StackWalkAllocations;

				ProfilerTab.recording.Discard();
				ProfilerTab.recording.Start(dictionary["rate"].Get<float>(), dictionary["duration"].Get<float>(), profilerArgs, discarded =>
				{
					if (discarded)
					{
						Notifications.Add(player, "Timeline profiling has been discarded.");
					}

					if (ap.IsInMenu && ap.SelectedTab != null && ap.SelectedTab.Id == "profiler")
					{
						ap.SelectedTab.OnChange(ap, ap.SelectedTab);
						Draw(ap.Player);
					}

					Analytics.profiler_tl_ended(profilerArgs, ProfilerTab.recording.CurrentDuration, ProfilerTab.recording.Status);
				});
				Analytics.profiler_tl_started(profilerArgs);

				Pool.FreeUnmanaged(ref dictionary);

				ap.SelectedTab.OnChange(ap, ap.SelectedTab);
				Draw(player);
			}, onCancel: () =>
			{
				Pool.FreeUnmanaged(ref dictionary);

				ap.SelectedTab.OnChange(ap, ap.SelectedTab);
				Draw(player);
			});
		}
		else
		{
			ProfilerTab.recording?.Stop();
			ap.SelectedTab.OnChange(ap, ap.SelectedTab);

			Analytics.profiler_tl_ended(ProfilerTab.recording.Args, ProfilerTab.recording.CurrentDuration, ProfilerTab.recording.Status);

			Draw(player);
		}
	}

	[Conditional("!MINIMAL")]
	[ProtectedCommand("adminmodule.timelineclear")]
	private void TimelineClear(ConsoleSystem.Arg arg)
	{
		var ap = GetPlayerSession(arg.Player());

		if (ProfilerTab.recording.IsRecording())
		{
			ProfilerTab.recording.Stop(true);
			Analytics.profiler_tl_ended(ProfilerTab.recording.Args, ProfilerTab.recording.CurrentDuration, ProfilerTab.recording.Status);
		}
		else
		{
			ProfilerTab.recording.Discard();
		}

		ap.SelectedTab.OnChange(ap, ap.SelectedTab);

		Draw(ap.Player);
	}
}

#endif

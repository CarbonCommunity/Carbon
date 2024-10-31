#if !MINIMAL

using Newtonsoft.Json.Linq;
using StringEx = Carbon.Extensions.StringEx;
using static Carbon.Components.CUI;

namespace Carbon.Modules;

public partial class AdminModule
{
	public class ConfigEditor : Tab
	{
		internal JObject Entry { get; set; }
		internal Action<PlayerSession, JObject> OnSave, OnSaveAndReload, OnCancel;
		internal const string Spacing = " ";
		internal string[] Blacklist;

		public ConfigEditor(string id, string name, RustPlugin plugin, Action<PlayerSession, Tab> onChange = null) : base(id, name, plugin, onChange)
		{
		}

		public static ConfigEditor Make(string json, Action<PlayerSession, JObject> onCancel, Action<PlayerSession, JObject> onSave, Action<PlayerSession, JObject> onSaveAndReload, bool fullscreen = false, string[] blacklist = null)
		{
			var tab = new ConfigEditor("configeditor", "Config Editor", Community.Runtime.Core)
			{
				Entry = JObject.Parse(json),
				OnSave = onSave,
				OnSaveAndReload = onSaveAndReload,
				OnCancel = onCancel,
				Blacklist = blacklist,
				IsFullscreen = fullscreen
			};

			tab._draw();
			return tab;
		}

		internal void _draw()
		{
			AddColumn(0);
			AddColumn(1);

			var list = Facepunch.Pool.Get<List<OptionButton>>();
			if (OnCancel != null) list.Add(new OptionButton("Cancel", ap => { OnCancel?.Invoke(ap, Entry); }));
			if (OnSave != null) list.Add(new OptionButton("Save", ap => { OnSave?.Invoke(ap, Entry); }));
			if (OnSaveAndReload != null) list.Add(new OptionButton("Save & Reload", ap => { OnSaveAndReload?.Invoke(ap, Entry); }));

			AddButtonArray(0, list.ToArray());
			Facepunch.Pool.FreeUnmanaged(ref list);

			foreach (var token in Entry)
			{
				if (token.Value is JObject) AddName(0, $"{token.Key}");

				_recurseBuild(token.Key, token.Value, 0, 0);
			}
		}
		internal void _recurseBuild(string name, JToken token, int level, int column, bool removeButtons = false)
		{
			if (Blacklist != null && Enumerable.Contains(Blacklist, name))
			{
				return;
			}

			switch (token)
			{
				case JArray array:
					{
						AddName(column, $"{StringEx.SpacedString(Spacing, level, false)}{name}");
						AddButton(column, $"Edit", ap =>
						{
							_drawArray(name, array, level, column, ap);
						});
					}
					break;

				default:
					var usableToken = token is JProperty property ? property.Value : token;
					switch (usableToken?.Type)
					{
						case JTokenType.String:
							var value = usableToken.ToObject<string>();
							var valueSplit = value.Split(' ');
							if (value.StartsWith("#") || (valueSplit.Length >= 3 && valueSplit.All(x => float.TryParse(x, out _))))
							{
								AddColor(column, name, () => value.StartsWith("#") ? HexToRustColor(value) : value, (ap, hex, rust, alpha) =>
								{
									value = value.StartsWith("#") ? hex : rust;
									usableToken.Replace(usableToken = $"#{value}");
									Community.Runtime.Core.NextFrame(() => Singleton.SetTab(ap.Player, Make(Entry.ToString(), OnCancel, OnSave, OnSaveAndReload), false));
								}, tooltip: $"The color value of the '{name.Trim()}' property.");
							}
							else AddInput(column, name, ap => usableToken.ToObject<string>(), (ap, args) => { usableToken.Replace(usableToken = args.ToString(" ")); });
							Array.Clear(valueSplit, 0, valueSplit.Length);
							valueSplit = null;
							break;

						case JTokenType.Integer:
							AddInput(column, name, ap => usableToken?.ToObject<long>().ToString(), (ap, args) => { usableToken.Replace(usableToken = args.ToString(" ").ToLong()); }, tooltip: $"The integer/long value of the '{name.Trim()}' property.");
							break;

						case JTokenType.Float:
							AddInput(column, name, ap => usableToken?.ToObject<float>().ToString(), (ap, args) => { usableToken.Replace(usableToken = args.ToString(" ").ToFloat()); }, tooltip: $"The float value of the '{name.Trim()}' property.");
							break;

						case JTokenType.Boolean:
							AddToggle(column, name,
								ap => { usableToken.Replace(usableToken = !usableToken.ToObject<bool>()); },
								ap => usableToken.ToObject<bool>(), tooltip: $"The boolean value of the '{name.Trim()}' property.");
							break;

						case JTokenType.Array:
							{
								var array2 = usableToken as JArray;
								AddName(column, $"{StringEx.SpacedString(Spacing, level, false)}{name}");
								AddButton(column, $"Edit", ap =>
								{
									_drawArray(name, array2, level, column, ap);
								});
							}
							break;

						case JTokenType.Object:
							var newLevel = level + 1;
							if (token.Parent is JArray array)
							{
								AddInputButton(column, null, 0.2f,
									new OptionInput(null, ap => $"{array.IndexOf(token)}", 0, true, null),
									new OptionButton("Remove", TextAnchor.MiddleCenter, ap =>
									{
										array.Remove(token);
										ClearColumn(column);
										// DrawArray(name, array, 0, true);
										_drawArray(name, array, level, column, ap);
									}, ap => OptionButton.Types.Important));

							}
							DrawArray(name, token, newLevel);

							break;
					}
					break;
			}

			void DrawArray(string title, JToken tok, int ulevel, bool editRefresh = false)
			{
				if (editRefresh)
				{
					AddName(column, $"Editing '{(tok.Parent as JProperty)?.Name}'");
				}

				foreach (var subToken in tok)
				{
					if (subToken is JObject && !editRefresh)
						AddName(column, $"{StringEx.SpacedString(Spacing, ulevel, false)}{(subToken.Parent as JProperty)?.Name}");
					_recurseBuild($"{StringEx.SpacedString(Spacing, ulevel + 1, false)}{(subToken as JProperty)?.Name}", subToken, ulevel + 1, column);

					if (removeButtons)
					{
						var jproperty = (subToken as JProperty);
						AddButton(column, $"Remove '{jproperty?.Name.Trim()}'", ap2 =>
						{
							(tok as JObject).Remove(jproperty.Name);
							_drawArray(name, tok.Parent as JArray, ulevel, column, ap2);
						}, ap2 => OptionButton.Types.Important);
					}
				}
			}
		}
		internal void _drawArray(string name, JArray array, int level, int column, PlayerSession ap)
		{
			var index = 0;
			var subColumn = column + 1;
			ClearAfter(subColumn, true);
			AddName(subColumn, $"Editing '{name.Trim()}'");
			foreach (var element in array)
			{
				_recurseBuild($"{StringEx.SpacedString(Spacing, level, false)}{index:n0}", element, 0, subColumn, array.Count == 1);

				AddButton(subColumn, $"Remove", ap2 =>
				{
					array.Remove(element);
					_drawArray(name, array, level, column, ap);
				}, ap2 => OptionButton.Types.Important);

				index++;
			}
			if (array.Count <= 1)
			{
				var sample = array.FirstOrDefault() as JObject;
				var newPropertyName = ap.GetStorage(this, "jsonprop", "New Property");

				if (array.Count == 1)
				{
					AddButton(subColumn, $"Duplicate", ap2 =>
					{
						array.Add(array.LastOrDefault());
						_drawArray(name, array, level, column, ap);
					}, ap2 => OptionButton.Types.Warned);
				}
				else if (array.Count == 0) AddText(subColumn, $"{StringEx.SpacedString(Spacing, 0, false)}No entries", 10, "1 1 1 0.6", TextAnchor.MiddleLeft);

				AddInput(subColumn, "Property Name", ap => ap.GetStorage(this, "jsonprop", "New Property"), (ap, args) => { ap.SetStorage(this, "jsonprop", newPropertyName = args.ToString(" ")); });
				AddButtonArray(subColumn, 0.01f,
					new OptionButton("Add Label", ap => { if (sample == null) array.Add(sample = JObject.Parse("{ }")); if (!(sample as IDictionary<string, JToken>).ContainsKey(newPropertyName)) { sample.Add(newPropertyName, string.Empty); _drawArray(name, array, level, column, ap); } }),
					new OptionButton("Add Toggle", ap => { if (sample == null) array.Add(sample = JObject.Parse("{ }")); if (!(sample as IDictionary<string, JToken>).ContainsKey(newPropertyName)) { sample.Add(newPropertyName, false); _drawArray(name, array, level, column, ap); } }),
					new OptionButton("Add Int", ap => { if (sample == null) array.Add(sample = JObject.Parse("{ }")); if (!(sample as IDictionary<string, JToken>).ContainsKey(newPropertyName)) { sample.Add(newPropertyName, 0); _drawArray(name, array, level, column, ap); } }),
					new OptionButton("Add Float", ap => { if (sample == null) array.Add(sample = JObject.Parse("{ }")); if (!(sample as IDictionary<string, JToken>).ContainsKey(newPropertyName)) { sample.Add(newPropertyName, 0.0f); _drawArray(name, array, level, column, ap); } }));
			}
			else
			{
				AddButton(subColumn, $"Duplicate", ap2 =>
				{
					array.Add(array.LastOrDefault());
					_drawArray(name, array, level, column, ap);
				}, ap2 => OptionButton.Types.Selected);
			}
		}
	}
}

#endif

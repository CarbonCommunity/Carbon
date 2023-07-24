/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * All rights reserved.
 *
 */

using System.Windows.Controls;
using API.Abstracts;
using ConVar;
using Oxide.Game.Rust.Cui;
using static Carbon.Components.CUI;
using static ConsoleSystem;

namespace Carbon.Modules;

public partial class ModalModule : CarbonModule<EmptyModuleConfig, EmptyModuleData>
{
	public static ModalModule Instance { get; internal set; }
	public AdminModule Admin { get; internal set; }
	public ColorPickerModule ColorPicker { get; internal set; }
	public readonly Handler Handler = new();

	public override string Name => "Modal";
	public override VersionNumber Version => new(1, 0, 0);
	public override Type Type => typeof(ModalModule);
	public override bool EnabledByDefault => true;
	public override bool ForceEnabled => true;

	public override bool InitEnd()
	{
		Instance = this;
		Admin = GetModule<AdminModule>();
		ColorPicker = GetModule<ColorPickerModule>();
		return base.InitEnd();
	}

	public Modal Open(BasePlayer player,
					  string title,
					  Dictionary<string, Modal.Field> fields,
					  Action<BasePlayer, Modal> onConfirm = null,
					  Action onCancel = null,
					  Action<Modal, string, Modal.Field, object, object> onFieldChanged = null)
	{
		var tab = new Modal()
		{
			Title = title,
			Fields = fields,
			OnCancel = onCancel,
			OnConfirm = onConfirm,
			Player = player,
			OnFieldChanged = onFieldChanged,
			Handler = new()
		};

		NextFrame(() => tab.Draw(player));

		return tab;
	}
	public void Close(BasePlayer player)
	{
		using var cui = new CUI(Handler);
		cui.Destroy(Modal.PanelId, player);
	}

	public class Modal
	{
		public string Title;
		public Dictionary<string, Field> Fields;
		public Action OnCancel;
		public Action<BasePlayer, Modal> OnConfirm;
		public int Page;
		public string BackgroundColor = "0 0 0 0.99";
		public float PositionXMin = 0.3f;
		public float PositionXMax = 0.7f;
		public float PositionYMin = 0.275f;
		public float PositionYMax = 0.825f;

		internal Handler Handler;
		internal const string PanelId = "carbonmodalui";
		internal BasePlayer Player;
		internal Action<Modal, string, Field, object, object> OnFieldChanged;

		public bool IsValid()
		{
			foreach (var field in Fields)
			{
				if (field.Value.IsInvalid()) return false;
			}

			return true;
		}
		public int Pages => Fields.Count > 4 ? (Fields.Count - 1) / 4 : 0;
		public string[] InvalidMessages => Fields.Where(x => x.Value.Value != null && x.Value.CustomIsInvalid != null).Select(x =>
		{
			try
			{
				var value = x.Value?.CustomIsInvalid?.Invoke(x.Value);
				return $"    <color=red>{value?.ToUpper()?.SpacedString(1)}</color>";
			}
			catch { }

			return string.Empty;
		}).ToArray();

		public void Draw(BasePlayer player)
		{
			var ap = Instance.Admin.GetPlayerSession(player);

			using var cui = new CUI(Handler);
			var container = cui.CreateContainer(PanelId,
				color: BackgroundColor,
				xMin: 0, xMax: 1, yMin: 0, yMax: 1,
				needsCursor: true, destroyUi: PanelId);

			var color = cui.CreatePanel(container, parent: PanelId, id: PanelId + ".color",
				color: "0 0 0 0.6",
				xMin: PositionXMin, xMax: PositionXMax, yMin: PositionYMin, yMax: PositionYMax);
			var main = cui.CreatePanel(container, parent: PanelId + ".color", id: PanelId + ".main",
				color: "0 0 0 0.5", blur: true);

			_drawInternal(cui, container, main);

			ap.SetStorage(null, "modal", this);
			cui.Send(container, player);
		}

		internal void _drawInternal(CUI cui, CuiElementContainer container, string panel)
		{
			var subText = $"<b><color=red>*</color></b>  {"Assign all required field values.".ToUpper().SpacedString(1)}" +
				$"\n    {(IsValid() ? $"<b><color=green>{"The modal is valid.".ToUpper().SpacedString(1)}</color></b>" : $"<b><color=red>{"The modal has invalid fields.".ToUpper().SpacedString(1)}</color></b>")}" +
				$"{(InvalidMessages != null ? $"\n{InvalidMessages.ToString("\n")}" : "")}";
			cui.CreateText(container, panel, null, "1 1 1 1", subText.Trim(), 9, align: TextAnchor.LowerLeft, xMin: 0.05f, yMin: 0.05f);

			var main = cui.CreatePanel(container, panel, null, "0 0 0 0", xMin: 0.1f, xMax: 0.9f, yMin: 0.2f, yMax: 0.9f);
			cui.CreateText(container, main, null, "1 1 1 0.7", Title, 20, xMin: 0.025f, yMax: 0.985f, align: TextAnchor.UpperLeft);

			var offset = 0f;
			var spacing = 60f;

			var content = cui.CreatePanel(container, main, null, "0 0 0 0", xMin: 0, yMin: 0, OyMin: -35, OyMax: -35);

			var pageContent = Fields.Skip(Page * 4).Take(4);
			foreach (var field in pageContent)
			{
				var fieldPanel = cui.CreatePanel(container, content, null, "0 0 0 0.5", yMin: 0.8f, OyMin: offset, OyMax: offset);
				cui.CreateText(container, fieldPanel, null, "1 1 1 1", $"<b>{field.Value.DisplayName.ToUpper().SpacedString(1)}</b>{(field.Value.IsRequired ? "  <b><color=red>*</color></b>" : "")}", 11, xMin: 0.03f, yMax: 0.85f, align: TextAnchor.UpperLeft);

				var option = cui.CreatePanel(container, fieldPanel, null, $"0.1 0.1 0.1 {(field.Value.IsReadOnly ? "0.45" : "0.75")}", yMax: 0.55f);
				var textColor = field.Value.IsReadOnly ? "1 1 1 0.15" : "1 1 1 1";

				if (field.Value.IsInvalid())
				{
					cui.CreatePanel(container, option, null, HexToRustColor("#b8302e", 0.5f));
				}

				switch (field.Value.Type)
				{
					case Field.FieldTypes.String:
					case Field.FieldTypes.Float:
					case Field.FieldTypes.Integer:
					case Field.FieldTypes.ULong:
						var value = field.Value.Value?.ToString();
						cui.CreateProtectedInputField(container, option, null, textColor, value, 15, 256, false, xMin: 0.025f, align: TextAnchor.MiddleLeft, command: $"modal.action {field.Key}", needsKeyboard: true);
						break;

					case Field.FieldTypes.Boolean:
						cui.CreateProtectedButton(container, option, null, "0 0 0 0", "0 0 0 0", string.Empty, 0, command: $"modal.action {field.Key} {field.Value.Value}");
						var toggle = cui.CreateProtectedButton(container, option, null, "0.1 0.1 0.1 0.8", "0 0 0 0", string.Empty, 0, xMin: 0.025f, xMax: 0.085f, yMin: 0.1f, yMax: 0.9f, command: $"modal.action {field.Key} {field.Value.Value}");
						if (field.Value.Value is bool booleanValue && booleanValue)
						{
							cui.CreateImage(container, toggle, null, "checkmark", textColor, 0.2f, xMax: 0.8f, yMin: 0.2f, yMax: 0.8f);
						}
						break;

					case Field.FieldTypes.RustColor:
					case Field.FieldTypes.HexColor:
						var originalColor = string.IsNullOrEmpty($"{field.Value.Value}") ? (field.Value.Type == Field.FieldTypes.RustColor ? "1 1 1" : "#ffffff") : field.Value.Value.ToString();
						var hexColor = field.Value.Type == Field.FieldTypes.RustColor ? RustToHexColor(originalColor, includeAlpha: true) : originalColor;
						var rustColor = field.Value.Type == Field.FieldTypes.HexColor ? HexToRustColor(originalColor, includeAlpha: true) : originalColor;
						var rustColorSplit = rustColor.Split(' ');
						rustColor = $"R:{rustColorSplit[0].ToFloat() * 255:0}   G:{rustColorSplit[1].ToFloat() * 255:0}   B:{rustColorSplit[2].ToFloat() * 255:0}   A:{(rustColorSplit.Length == 4 ? rustColorSplit[3].ToFloat() : 1f) * 255:0}";
						Array.Clear(rustColorSplit, 0, rustColorSplit.Length);

						cui.CreateText(container, option, null, textColor, $"<b>{"HEX".SpacedString(1)}:</b>  {hexColor}", 12, xMin: 0.7f, align: TextAnchor.MiddleLeft);
						cui.CreateText(container, option, null, textColor, $"<b>{"RUST".SpacedString(1)}:</b>  {rustColor}", 12, xMin: 0.115f, align: TextAnchor.MiddleLeft);
						cui.CreateProtectedButton(container, option, null, hexColor, "0 0 0 0", string.Empty, 0, xMin: 0.025f, xMax: 0.085f, yMin: 0.1f, yMax: 0.9f, command: $"modal.action {field.Key}");
						break;

					case Field.FieldTypes.Enum:
						var @enum = field.Value as EnumField;
						cui.CreateText(container, option, null, textColor, @enum.Options[@enum.Value == null ? 0 : @enum.Value.ToString().ToInt()], 12, align: TextAnchor.MiddleCenter);

						cui.CreateProtectedButton(container, option, null, "0.1 0.1 0.1 0.75", "1 1 1 0.7", "<", 10, xMin: 0f, xMax: 0.5f, command: $"modal.action {field.Key} -");
						cui.CreateProtectedButton(container, option, null, "0.1 0.1 0.1 0.75", "1 1 1 0.7", ">", 10, xMin: 0.5f, xMax: 1f, command: $"modal.action {field.Key} +");
						break;

					case Field.FieldTypes.Button:
						var button = field.Value as ButtonField;
						cui.CreateProtectedButton(container, option, null, "0.1 0.1 0.1 0.85", "1 1 1 0.7", button.ButtonName?.ToUpper().SpacedString(1), 10, command: $"modal.action {field.Key}");
						break;
				}

				if (field.Value.IsReadOnly)
				{
					cui.CreatePanel(container, option, null, "0 0 0 0");
				}

				offset -= spacing;
			}

			var buttons = cui.CreatePanel(container, panel, null, "0 0 0 0", xMin: 0.075f, xMax: 0.925f, yMin: 0.025f, yMax: 0.1f);
			cui.CreateProtectedButton(container, buttons, null, "0.1 0.1 0.1 0.85", "1 1 1 0.7", "CANCEL".SpacedString(1), 10, xMin: 0.7f, xMax: 0.84f, command: "modal.cancel");
			cui.CreateProtectedButton(container, buttons, null, IsValid() ? HexToRustColor("#7ebf37", 0.6f) : "0.1 0.1 0.1 0.85", "1 1 1 0.7", "CONFIRM".SpacedString(1), 10, xMin: 0.85f, command: "modal.confirm");

			if (Pages > 0)
			{
				var pages = cui.CreatePanel(container, panel, null, "0 0 0 0", xMin: 0.1f, xMax: 0.9f, yMin: 0.15f, yMax: 0.2f);
				cui.CreateText(container, pages, null, "1 1 1 0.7", $"{Page + 1:n0} / {Pages + 1:n0}", 10, xMin: 0.82f, xMax: 0.92f);
				cui.CreateProtectedButton(container, pages, null, Page == 0 ? "0.2 0.2 0.2 0.6" : HexToRustColor("#7ebf37", 0.6f), "1 1 1 0.7", "<", 10, xMin: 0.82f, xMax: 0.90f, OxMin: -30, OxMax: -30, command: "modal.page -");
				cui.CreateProtectedButton(container, pages, null, Page == Pages ? "0.2 0.2 0.2 0.6" : HexToRustColor("#7ebf37", 0.6f), "1 1 1 0.7", ">", 10, xMin: 0.92f, command: "modal.page +");
			}
		}

		public T Get<T>(string key)
		{
			if (Fields.TryGetValue(key, out var field)) return (T)field.Value;
			return default;
		}

		public class Field : IDisposable
		{
			public string DisplayName { get; set; }
			public object Value { get; set; }
			public FieldTypes Type { get; set; }
			public bool IsRequired { get; set; }
			public bool IsReadOnly { get; set; }

			public T Get<T>()
			{
				return (T)Value;
			}

			public Func<Field, string> CustomIsInvalid { get; set; }

			public bool IsInvalid()
			{
				return IsRequired && (Value == null || string.IsNullOrEmpty(Value.ToString())) || !string.IsNullOrEmpty(CustomIsInvalid?.Invoke(this));
			}

			public enum FieldTypes
			{
				String,
				Integer,
				Float,
				ULong,
				Boolean,
				Enum,
				RustColor,
				HexColor,
				Button
			}

			public static Field Make(string displayName, FieldTypes type, bool required = false, object @default = null, bool isReadOnly = false, Func<Field, string> customIsInvalid = null)
			{
				return new Field
				{
					DisplayName = displayName,
					Type = type,
					IsRequired = required,
					Value = @default,
					IsReadOnly = isReadOnly,
					CustomIsInvalid = customIsInvalid
				};
			}

			public void Dispose()
			{
				DisplayName = null;
				Value = null;
			}
		}
		public class EnumField : Field
		{
			public string[] Options { get; set; }

			public static EnumField MakeEnum(string displayName, string[] options, bool required = false, object @default = null, bool isReadOnly = false, Func<Field, string> customIsInvalid = null)
			{
				return new EnumField
				{
					DisplayName = displayName,
					Type = FieldTypes.Enum,
					IsRequired = required,
					Value = @default,
					Options = options,
					IsReadOnly = isReadOnly,
					CustomIsInvalid = customIsInvalid
				};
			}
		}
		public class ButtonField : Field
		{
			public string ButtonName { get; set; }
			public Action<Modal> Callback { get; set; }

			public static ButtonField MakeButton(string displayName, string buttonName, Action<Modal> callback, bool isReadOnly = false)
			{
				return new ButtonField
				{
					DisplayName = displayName,
					ButtonName = buttonName,
					Type = FieldTypes.Button,
					IsRequired = false,
					Callback = callback,
					IsReadOnly = isReadOnly,
					CustomIsInvalid = null
				};
			}
		}
	}

	#region Commands

	[ProtectedCommand("modal.action")]
	private void ModalAction(Arg arg)
	{
		var ap = Admin.GetPlayerSession(arg.Player());
		var modal = ap.GetStorage<Modal>(null, "modal");

		var fieldName = arg.Args[0];
		var field = modal.Fields[fieldName];
		var oldValue = field.Value;

		if (!field.IsReadOnly)
		{
			var value = arg.Args[1];

			switch (field.Type)
			{
				case Modal.Field.FieldTypes.String:
					field.Value = value;
					modal.OnFieldChanged?.Invoke(modal, fieldName, field, oldValue, value);
					break;

				case Modal.Field.FieldTypes.Integer:
					field.Value = value.ToInt();
					modal.OnFieldChanged?.Invoke(modal, fieldName, field, oldValue, value);
					break;

				case Modal.Field.FieldTypes.ULong:
					field.Value = value.ToUlong();
					modal.OnFieldChanged?.Invoke(modal, fieldName, field, oldValue, value);
					break;

				case Modal.Field.FieldTypes.Float:
					field.Value = value.ToFloat();
					modal.OnFieldChanged?.Invoke(modal, fieldName, field, oldValue, value);
					break;

				case Modal.Field.FieldTypes.Boolean:
					field.Value = !value.ToBool(false);
					modal.OnFieldChanged?.Invoke(modal, fieldName, field, oldValue, value);
					break;

				case Modal.Field.FieldTypes.RustColor:
				case Modal.Field.FieldTypes.HexColor:
					Community.Runtime.CorePlugin.NextFrame(() =>
					{
						ColorPicker.Open(ap.Player, (hexColor, rustColor, alpha) =>
						{
							var before = field.Value;

							if (field.Type == Modal.Field.FieldTypes.RustColor)
								field.Value = CUI.HexToRustColor($"#{hexColor}", alpha);
							else field.Value = CUI.RustToHexColor(rustColor, alpha);

							modal.OnFieldChanged?.Invoke(modal, fieldName, field, oldValue, value);
							modal.Draw(ap.Player);
						});
					});
					break;

				case Modal.Field.FieldTypes.Enum:
					var @enum = field as Modal.EnumField;
					var enumValue = field.Value == null ? 0 : field.Value.ToString().ToInt();
					switch (value)
					{
						case "+":
							enumValue++;
							break;

						case "-":
							enumValue--;
							field.Value = enumValue - 1;
							break;
					}

					if (enumValue > @enum.Options.Length - 1) enumValue = 0;
					else if (enumValue < 0) enumValue = @enum.Options.Length - 1;

					field.Value = enumValue;
					modal.OnFieldChanged?.Invoke(modal, fieldName, field, oldValue, field.Value);
					break;

				case Modal.Field.FieldTypes.Button:
					var button = field as Modal.ButtonField;
					button.Callback?.Invoke(modal);
					break;
			}
		}

		modal.Draw(ap.Player);
	}

	[ProtectedCommand("modal.confirm")]
	private void ModalConfirm(Arg arg)
	{
		var ap = Admin.GetPlayerSession(arg.Player());
		var modal = ap.GetStorage<Modal>(null, "modal");

		if (!modal.IsValid())
		{
			modal.Draw(ap.Player);
			return;
		}

		modal.OnConfirm?.Invoke(ap.Player, modal);
		Close(ap.Player);
	}

	[ProtectedCommand("modal.cancel")]
	private void ModalCancel(Arg arg)
	{
		var ap = Admin.GetPlayerSession(arg.Player());
		var modal = ap.GetStorage<Modal>(null, "modal");
		modal?.OnCancel?.Invoke();
		Close(ap.Player);
	}

	[ProtectedCommand("modal.page")]
	private void ModalPage(Arg arg)
	{
		var ap = Admin.GetPlayerSession(arg.Player());
		var modal = ap.GetStorage<Modal>(null, "modal");
		switch (arg.Args[0])
		{
			case "+":
				modal.Page++;
				break;

			case "-":
				modal.Page--;
				break;
		}

		if (modal.Page > modal.Pages) modal.Page = 0;
		else if (modal.Page < 0) modal.Page = modal.Pages;

		modal.Draw(ap.Player);
	}

	#endregion
}

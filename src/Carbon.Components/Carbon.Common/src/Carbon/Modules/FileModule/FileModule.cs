using Oxide.Game.Rust.Cui;
using UnityEngine.UI;
using static Carbon.Components.CUI;
using static ConsoleSystem;

namespace Carbon.Modules;

public partial class FileModule : CarbonModule<EmptyModuleConfig, EmptyModuleData>
{
	public static FileModule Instance { get; internal set; }
	public AdminModule Admin { get; internal set; }
	public readonly Handler Handler = new();

	public override string Name => "File";
	public override VersionNumber Version => new(1, 0, 0);
	public override Type Type => typeof(FileModule);
	public override bool EnabledByDefault => true;
	public override bool ForceEnabled => true;

	public override bool InitEnd()
	{
		Instance = this;
		Admin = GetModule<AdminModule>();
		return base.InitEnd();
	}

	public FileBrowser Open(BasePlayer player,
					  string title,
					  string directory,
					  string directoryLimit,
					  string extension,
					  Action<BasePlayer, FileBrowser> onConfirm = null,
					  Action<BasePlayer, FileBrowser> onCancel = null,
					  Func<FileBrowser.Item, string> onExtraInfo = null)
	{
		var file = new FileBrowser
		{
			Title = title,
			DirectoryLimit = directoryLimit,
			Extension = string.IsNullOrEmpty(extension) ? string.Empty : "." + extension,
			OnCancel = onCancel,
			OnConfirm = onConfirm,
			OnExtraInfo = onExtraInfo,
			Handler = new()
		};

		NextFrame(() =>
		{
			file.ChangeDirectory(directory);
			file.Draw(player);
		});

		return file;
	}
	public void Close(BasePlayer player)
	{
		using var cui = new CUI(Handler);
		cui.Destroy(FileBrowser.PanelId, player);
	}

	public class FileBrowser
	{
		public string Title;
		public string DirectoryLimit;
		public string Extension;
		public bool AllowBackFromParent;
		public Action<BasePlayer, FileBrowser> OnConfirm;
		public Action<BasePlayer, FileBrowser> OnCancel;
		public Func<Item, string> OnExtraInfo;
		public string BackgroundColor = "0 0 0 0.99";

		public string SelectedFile;
		public string DeletingFile;

		internal string CurrentDirectory;
		internal Handler Handler;
		internal const string PanelId = "carbonfileui";
		internal List<Item> Items = new();

		public struct Item
		{
			public string Name;
			public string Path;
			public bool IsDirectory;
			public FileInfo Info;

			public static Item Make(string path)
			{
				Item item = default;
				item.Name = System.IO.Path.GetFileName(path);
				item.Path = path;
				item.IsDirectory = OsEx.Folder.Exists(path);

				if (!item.IsDirectory)
				{
					item.Info = new(path);
				}

				return item;
			}
		}

		public void ChangeDirectory(string directory)
		{
			Items.Clear();

			if (string.IsNullOrEmpty(DirectoryLimit) || directory.Contains(DirectoryLimit))
			{
				CurrentDirectory = directory;
			}

			if (!OsEx.Folder.Exists(directory))
			{
				return;
			}

			Items.AddRange(Directory.EnumerateDirectories(CurrentDirectory, "*")
				.Select(x => Item.Make(x)).OrderBy(x => x.Name));

			Items.AddRange(Directory.EnumerateFiles(CurrentDirectory, $"*{Extension}")
				.Select(x => Item.Make(x)).OrderByDescending(x => x.Info.LastWriteTime));
		}

		public void Draw(BasePlayer player)
		{
			var ap = Instance.Admin.GetPlayerSession(player);

			using var cui = new CUI(Handler);
			var container = cui.CreateContainer(PanelId,
				color: BackgroundColor,
				xMin: 0, xMax: 1, yMin: 0, yMax: 1,
				needsCursor: true, destroyUi: PanelId);

			var background = cui.CreatePanel(container, PanelId, "0.05 0.05 0.05 0.9", xMin: 0.5f, xMax: 0.5f,
				yMin: 0.5f, yMax: 0.5f, OxMin: -300, OxMax: 300, OyMin: -250, OyMax: 250);
			cui.CreateText(container, background, Cache.CUI.WhiteColor,
				$"{Title.ToUpper()} ({(string.IsNullOrEmpty(Extension) ? "*" : Extension)}) ({Items.Count(x => !x.IsDirectory):n0})", 20,
				align: TextAnchor.UpperLeft, xMin: 0.03f, yMax: 0.97f, font: Handler.FontTypes.RobotoCondensedBold);

			const float nameSpace = 0f;
			const float extraSpace = 0.33f;
			const float dateSpace = 0.63f;
			const float sizeSpace = 0.8f;

			var bar = cui.CreatePanel(container, background, Cache.CUI.BlankColor, yMax: 0.9f);
			cui.CreateText(container, bar, "1 1 1 0.2", "NAME", 12, align: TextAnchor.UpperLeft, xMin: nameSpace + 0.03f);
			cui.CreateText(container, bar, "1 1 1 0.2", "INFO", 12, align: TextAnchor.UpperLeft, xMin: extraSpace);
			cui.CreateText(container, bar, "1 1 1 0.2", "DATE", 12, align: TextAnchor.UpperLeft, xMin: dateSpace);
			cui.CreateText(container, bar, "1 1 1 0.2", "SIZE", 12, align: TextAnchor.UpperLeft, xMin: sizeSpace);

			var exit = cui.CreateProtectedButton(container, background, "0.5 0 0 0.4", Cache.CUI.BlankColor,
				string.Empty,
				0, xMin: 0.955f, xMax: 0.99f, yMin: 0.95f, yMax: 0.99f, command: "file.action cancel");
			cui.CreateImage(container, exit, "close", "1 0.5 0.5 0.3", xMin: 0.2f, xMax: 0.8f, yMin: 0.2f, yMax: 0.8f);

			var scroll = cui.CreateScrollView(container, background, true, false,
				ScrollRect.MovementType.Elastic, 0.1f,
				true, 0.1f, 30, out var content, out _,
				out var verticalScrollbar,
				yMax: 0.85f, xMin: 0.01f, OyMin: 0.04f);

			cui.CreatePanel(container, scroll, Cache.CUI.BlankColor);

			verticalScrollbar.Size = 3f;
			verticalScrollbar.Invert = true;

			var offset = 0f;
			const float scale = 25f;
			const float spacing = 3f;
			var pageScale = (Items.Count + 1) * (scale + spacing);

			content.AnchorMin = "0 0";
			content.AnchorMax = "0.985 0";
			content.OffsetMin = $"0 {-pageScale.Clamp(425, float.MaxValue)}";
			content.OffsetMax = $"0 0";

			static void DrawItem(FileBrowser browser, Item item, CUI cui, CuiElementContainer container, int i, Pair<string, CuiElement> scroll, ref float offset)
			{
				var fileButton = cui.CreateProtectedButton(container, scroll, "0.3 0.3 0.3 0.5",
					Cache.CUI.BlankColor, string.Empty, 0, yMin: 1, yMax: 1, OyMin: offset - scale,
					OyMax: offset, align: TextAnchor.MiddleLeft, command: $"file.action select {i}");

				cui.CreateText(container, fileButton, "1 1 1 0.7",
					i == -1 ? "..." : Path.GetFileName(item.Path), 10, xMin: 0.045f,
					align: TextAnchor.MiddleLeft);
				cui.CreateImage(container, fileButton, item.IsDirectory ? "folder" : "file", "0.7 0.7 0.7 0.5", xMin: 0.01f, xMax: 0.04f, yMin: 0.15f,
					yMax: 0.85f);

				cui.CreateText(container, fileButton, "1 1 1 0.4",
					browser.OnExtraInfo?.Invoke(item),
					10, xMin: extraSpace, xMax: 0.65f, align: TextAnchor.MiddleLeft);

				if (!item.IsDirectory)
				{
					var modificationDate = item.Info.LastWriteTime;
					cui.CreateText(container, fileButton, "1 1 1 0.4",
						$"{modificationDate.Month:00}.{modificationDate.Day:00}.{modificationDate.Year:0000} {modificationDate.Hour:00}:{modificationDate.Minute:00}",
						10, xMin: dateSpace, align: TextAnchor.MiddleLeft);
					cui.CreateText(container, fileButton, "1 1 1 0.4",
						$"{ByteEx.Format(item.Info.Length).ToUpper()}",
						10, xMin: sizeSpace, align: TextAnchor.MiddleLeft);

					var deleteButton = cui.CreateProtectedButton(container, fileButton, "0.5 0 0 0.4", "1 0.5 0.5 0.3",
						"DELETE", 8,
						xMin: 0.9f, command: $"file.action delete {i}", id: $"filedelete{i}");
					deleteButton.Element2.Name = $"filedeletetext{i}";
				}

				offset -= scale + spacing;
			}

			Item previous = default;
			previous.IsDirectory = true;
			previous.Path = OsEx.Folder.GetPreviousFolder(CurrentDirectory);

			DrawItem(this, previous, cui, container, -1, scroll, ref offset);

			for (int i = 0; i < Items.Count; i++)
			{
				DrawItem(this, Items[i], cui, container, i, scroll, ref offset);
			}

			ap.SetStorage(null, "file", this);
			cui.Send(container, player);
		}
	}

	#region Commands

	[ProtectedCommand("file.action")]
	private void FileAction(Arg arg)
	{
		var ap = Admin.GetPlayerSession(arg.Player());
		var file = ap.GetStorage<FileBrowser>(null, "file");

		var mode = arg.GetString(0);

		switch (mode)
		{
			case "select":
			{
				var index = arg.GetInt(1);

				if (index == -1)
				{
					file.ChangeDirectory(OsEx.Folder.GetPreviousFolder(file.CurrentDirectory));
					file.Draw(ap.Player);
					break;
				}

				var item = file.Items[index];
				if (item.IsDirectory)
				{
					file.ChangeDirectory(item.Path);
					file.Draw(ap.Player);
				}
				else
				{
					Instance.Close(ap.Player);
					file.SelectedFile = item.Path;
					file.OnConfirm?.Invoke(ap.Player, file);
				}
				break;
			}

			case "cancel":
				Instance.Close(ap.Player);
				file.OnCancel?.Invoke(ap.Player, file);
				break;

			case "delete":
			{
				var index = arg.GetInt(1);
				var path = file.Items[index].Path;

				if (file.DeletingFile == path)
				{
					OsEx.File.Delete(path);
					file.Items.RemoveAll(x => x.Path == path);
					file.DeletingFile = null;
					file.Draw(ap.Player);
				}

				if (string.IsNullOrEmpty(file.DeletingFile))
				{
					using var cui = new CUI(Handler);
					using var update = cui.UpdatePool();

					update.Add(cui.UpdateProtectedButton($"filedelete{index}", "0 0.5 0 0.4", Cache.CUI.BlankColor,
						string.Empty, 0, command: $"file.action delete {index}"));
					update.Add(cui.UpdateText($"filedeletetext{index}", "0 1 0 0.4", "CONFIRM", 8));
					update.Send(ap.Player);

					file.DeletingFile = path;

					Community.Runtime.Core.timer.In(0.75f, () =>
					{
						using var cui = new CUI(Handler);
						using var update = cui.UpdatePool();

						update.Add(cui.UpdateProtectedButton($"filedelete{index}", "0.5 0 0 0.4", Cache.CUI.BlankColor,
							string.Empty, 0, command: $"file.action delete {index}"));
						update.Add(cui.UpdateText($"filedeletetext{index}", "1 0.5 0.5 0.3", "DELETE", 8));
						update.Send(ap.Player);

						file.DeletingFile = null;
					});
				}

				break;
			}
		}
	}

	#endregion
}

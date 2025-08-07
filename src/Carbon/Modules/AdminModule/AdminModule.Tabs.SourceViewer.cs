#if !MINIMAL

using System.Text;
using Facepunch;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;
using ProtoBuf;
using UnityEngine.UI;

namespace Carbon.Modules;

public partial class AdminModule
{
	public class SourceViewerTab : Tab
	{
		public Action<PlayerSession> Close;

		public SourceViewerTab(string id, string name, RustPlugin plugin, Action<PlayerSession, Tab> onChange = null, string access = null) : base(id, name, plugin, onChange, access)
		{
		}

		public static SourceViewerTab Make(string fileName, string content, string context, int size = 8)
		{
			var tab = new SourceViewerTab("sourceviewer", "Source Viewer", Community.Runtime.Core);
			tab.OnChange += (_, tab1) =>
			{
				tab1.AddColumn(0, true);
			};
			tab.Over += (_, cui, container, panel, ap) =>
			{
				var blur = cui.CreatePanel(container, panel, "0.1 0.1 0.1 0.8", blur: true);

				var lines = content.Split('\n');
				var temp = Pool.Get<List<string>>();

				var resultContent = lines.ToString("\n");

				for (int i = 0; i < lines.Length; i++) temp.Add($"{i + 1}");

				cui.CreateImage(container, blur, "fade", Cache.CUI.WhiteColor, yMin: 0.96f);

				cui.CreateText(container, blur, "0.8 0.8 0.8 1",
					$"{fileName} <color=orange>*</color>", 8,
					align: TextAnchor.UpperLeft, font: CUI.Handler.FontTypes.RobotoCondensedRegular,
					xMin: 0.036f, xMax: 1f, yMin: 0.2f, yMax: 0.9875f);

				cui.CreateText(container, blur, "0.8 0.8 0.8 0.5",
					context, 8,
					align: TextAnchor.UpperRight, font: CUI.Handler.FontTypes.RobotoCondensedRegular,
					xMin: 0.036f, xMax: 0.97f, yMin: 0.2f, yMax: 0.9875f);

				var exit = cui.CreateProtectedButton(container, blur, "0.9 0.2 0.1 1", Cache.CUI.BlankColor,
					string.Empty, 0,
					xMin: 0.978f, xMax: 1f, yMin: 0.96f, command: "adminmodule.profilerpreviewclose");
				cui.CreateImage(container, exit, "close", "1 1 1 0.8", xMin: 0.2f, xMax: 0.8f, yMin: 0.2f, yMax: 0.8f);

				var scrollview = cui.CreateScrollView(container, blur,
					vertical: true, horizontal: true, movementType: ScrollRect.MovementType.Clamped, elasticity: 0.5f,
					inertia: true, decelerationRate: 0.2f, scrollSensitivity: 75,
					contentTransformComponent: out var contentTransform, verticalScrollBar: out var verticalScroll,
					horizontalScrollBar: out var horizontalScroll,
					yMax: 0.96f);

				cui.CreatePanel(container, blur, "0.2 0.2 0.2 1", yMin: 1, yMax: 1, OyMin: -20f, OyMax: -19f);
				cui.CreatePanel(container, scrollview, "0.2 0.2 0.2 1", xMin: 0, xMax: 0, OxMin: 29, OxMax: 30);
				cui.CreatePanel(container, blur, "0.2 0.2 0.2 1", xMin: 0, xMax: 0, OxMin: 29, OxMax: 30, yMin: 0.96f);

				var longestLine = lines.Max(x => x.Length);
				var height = -(11.2f * lines.Length.Clamp(45, int.MaxValue));
				var width = 2.75f * longestLine.Clamp(547, int.MaxValue);

				contentTransform.AnchorMin = "0 1";
				contentTransform.AnchorMax = "0 1";
				contentTransform.OffsetMin = $"0 {height}";
				contentTransform.OffsetMax = $"{width} 0";
				verticalScroll.Size = 2;
				horizontalScroll.Size = 2;
				horizontalScroll.AutoHide = false;
				horizontalScroll.Invert = true;

				cui.CreateText(container, scrollview, "0.3 0.7 0.9 0.5",
					string.Join("\n", temp), size,
					align: TextAnchor.UpperRight, font: CUI.Handler.FontTypes.DroidSansMono,
					xMin: 0, xMax: 0, OxMin: 0, OxMax: 20, yMin: 1, yMax: 1f, OyMax: -7.5f, OyMin: height);

				cui.CreateText(container, scrollview, "0.8 0.8 0.8 1",
					resultContent
						.Replace("\r", "")
						.Replace("\"", "'")
						.Replace("\t", "<color=#454545>————</color>"), size,
					align: TextAnchor.UpperLeft, font: CUI.Handler.FontTypes.DroidSansMono,
					xMin: 0, xMax: 0, yMin: 1f, yMax: 1f, OxMin: 40, OxMax: 40 + width, OyMax: -7.5f, OyMin: height);

				Pool.FreeUnmanaged(ref temp);
			};

			return tab;
		}
		public static SourceViewerTab MakeMethod(string assembly, string type, string method, int size = 8)
		{
			var code = SourceCodeBank.Parse(assembly);
			var codeResult = code.ParseMethod(type, method).Trim();
			return Make($"<color=#878787>{type}.</color>{method}<color=#878787>.cs</color>",
				ProcessSyntaxHighlight(codeResult), $"{Path.GetFileNameWithoutExtension(assembly)}.dll", size);
		}
		public unsafe static SourceViewerTab MakeMethod(MonoProfiler.CallRecord call, int size = 8)
		{
			var code = SourceCodeBank.Parse(call.assembly_name.name, call.assembly_handle);
			var codeResult = code.ParseMethod(call.method_handle, out var type, out var method).Trim();

			return Make(
				$"<color=#878787>{type}.</color>{method}",
				ProcessSyntaxHighlight(codeResult), call.assembly_name.GetDisplayName(true), size);
		}

		public static string ProcessSyntaxHighlight(string content)
		{
			var syntaxTree = CSharpSyntaxTree.ParseText(SourceText.From(content, Encoding.UTF8),
				options: new CSharpParseOptions()
					.WithDocumentationMode(DocumentationMode.Parse)
					.WithKind(SourceCodeKind.Script)
					.WithLanguageVersion(LanguageVersion.Preview));

			// Syntax highlighting configuration
			var syntaxHighlighter = new SyntaxHighlighter();
			syntaxHighlighter.AddPattern(SyntaxKind.UsingDirective, "#0000FF");
			syntaxHighlighter.AddPattern(SyntaxKind.IfStatement, "#caf553");
			syntaxHighlighter.AddPattern(SyntaxKind.ElseKeyword, "#dcdcaa");
			syntaxHighlighter.AddPattern(SyntaxKind.IdentifierName, "#e0e0e0");
			syntaxHighlighter.AddPattern(SyntaxKind.PredefinedType, "#caf553");
			syntaxHighlighter.AddPattern(SyntaxKind.OpenBracketToken, "#b3b1b1");
			syntaxHighlighter.AddPattern(SyntaxKind.CloseBracketToken, "#b3b1b1");
			syntaxHighlighter.AddPattern(SyntaxKind.PublicKeyword, "#caf553");
			syntaxHighlighter.AddPattern(SyntaxKind.PrivateKeyword, "#caf553");
			syntaxHighlighter.AddPattern(SyntaxKind.AttributeList, "#b3b1b1");
			syntaxHighlighter.AddPattern(SyntaxKind.MethodKeyword, "#caf553");
			syntaxHighlighter.AddPattern(SyntaxKind.MethodDeclaration, "#caf553");
			syntaxHighlighter.AddPattern(SyntaxKind.IdentifierToken, "#e0e0e0");
			syntaxHighlighter.AddPattern(SyntaxKind.StaticKeyword, "#caf553");
			syntaxHighlighter.AddPattern(SyntaxKind.ReturnStatement, "#caf553");
			syntaxHighlighter.AddPattern(SyntaxKind.NamespaceDeclaration, "#caf553");
			syntaxHighlighter.AddPattern(SyntaxKind.UsingKeyword, "#caf553");
			syntaxHighlighter.AddPattern(SyntaxKind.VoidKeyword, "#caf553");
			syntaxHighlighter.AddPattern(SyntaxKind.ThisExpression, "#caf553");
			syntaxHighlighter.AddPattern(SyntaxKind.StringLiteralToken, "#d69d85");
			syntaxHighlighter.AddPattern(SyntaxKind.VariableDeclarator, "#b3b1b1");
			syntaxHighlighter.AddPattern(SyntaxKind.TrueLiteralExpression, "#caf553");
			syntaxHighlighter.AddPattern(SyntaxKind.FalseLiteralExpression, "#caf553");
			syntaxHighlighter.AddPattern(SyntaxKind.ForEachStatement, "#caf553");
			syntaxHighlighter.AddPattern(SyntaxKind.NumericLiteralExpression, "#caf553");

			return syntaxHighlighter.Process(syntaxTree);
		}

		public class SyntaxHighlighter
		{
			public void AddPattern(SyntaxKind syntaxKind, string color)
			{
				_resolver[syntaxKind] = $"<color={color}>";
			}

			public string Process(SyntaxTree syntaxTree)
			{
				var root = syntaxTree.GetRoot();
				var builder = Pool.Get<StringBuilder>();

				WriteNode(root, builder);

				var result = builder.ToString();
				Pool.FreeUnmanaged(ref builder);
				return result;
			}

			private void WriteNode(SyntaxNode node, StringBuilder builder)
			{
				foreach (var token in node.DescendantTokens())
				{
					var text = token.ToFullString();
					var kind = token.Parent.Kind();
					var styleTag = _resolver[kind];

					if (!string.IsNullOrEmpty(styleTag))
					{
						builder.Append($"{styleTag}{text}</color>");
					}
					else
					{
						builder.Append($"{text}");
					}
				}
			}

			private readonly StyleResolver _resolver = new();
		}

		public class StyleResolver
		{
			private static readonly string[] _names = Enum.GetNames(typeof(SyntaxKind));
			private static readonly string[] _styles = new string[_names.Length];

			public string this[SyntaxKind syntaxKind]
			{
				get => _styles[_names.IndexOf(syntaxKind.ToString())];
				set => _styles[_names.IndexOf(syntaxKind.ToString())] = value;
			}
		}
	}

	[Conditional("!MINIMAL")]
	[ProtectedCommand("adminmodule.profilerpreviewclose")]
	private void ProfilerPreviewClose(ConsoleSystem.Arg arg)
	{
		var ap = GetPlayerSession(arg.Player());

		if (ap.SelectedTab is SourceViewerTab codePreview)
		{
			codePreview.Close?.Invoke(ap);
		}
	}
}

#endif

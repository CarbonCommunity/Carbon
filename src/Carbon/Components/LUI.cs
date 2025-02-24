using System.Runtime.CompilerServices;
using Network;
using StringEx = Carbon.Extensions.StringEx;

namespace Carbon.Common.Carbon.Components;

public class LUI : IDisposable
{
	public readonly List<LuiContainer> elements = new();

	private readonly CUI _parent;

	public bool generateNames;

	public LUI(CUI cui, bool generate = false)
	{
		_parent = cui;
		generateNames = generate;
	}

	#region Core Panel

	public LuiContainer CreateParent(CUI.ClientPanels parent, LuiPosition pos, string name = "") => CreateParent(_parent.GetClientPanel(parent), pos, name);

	public LuiContainer CreateParent(LuiContainer container, LuiPosition pos, string name = "") => CreateParent(container.name, pos, name);
	public LuiContainer CreateParent(string parent, LuiPosition pos, string name = "")
	{
		LuiContainer cont = LuiPool.GetContainer();
		cont.parent = parent;
		if (name != string.Empty)
			cont.name = name;
		else if (generateNames)
			cont.name = RandomEx.GetRandomString(4);
		cont.SetAnchors(pos);
		elements.Add(cont);
		return cont;
	}

	#endregion

	#region Panels

	public LuiContainer CreatePanel(LuiContainer container, string color) => CreatePanel(container.name, color);

	public LuiContainer CreatePanel(string parent, string color)
	{
		LuiContainer cont = LuiPool.GetContainer();
		cont.parent = parent;
		cont.AddColor(color);
		elements.Add(cont);
		return cont;
	}

	#endregion

	public void SendUi(BasePlayer player) => Send(new SendInfo(player.Connection));

	public void SendUiJson(BasePlayer player) => SendJson(new SendInfo(player.Connection));

	public string ToJson()
	{
		using LuiBuilderInstance cbi = new LuiBuilderInstance(this);
		return cbi.GetJsonString();
	}

	private void Send(SendInfo send)
	{
		using LuiBuilderInstance cbi = new LuiBuilderInstance(this);
		NetWrite write = Net.sv.StartWrite();
		write.PacketID(Message.Type.RPCMessage);
		write.EntityID(CommunityEntity.ServerInstance.net.ID);
		write.UInt32(StringPool.Get("AddUI"));
		write.BytesWithSize(cbi.GetMergedBytes());
		write.Send(send);
	}

	private void SendJson(SendInfo send)
	{
		using LuiBuilderInstance cbi = new LuiBuilderInstance(this);
		NetWrite write = Net.sv.StartWrite();
		write.PacketID(Message.Type.RPCMessage);
		write.EntityID(CommunityEntity.ServerInstance.net.ID);
		write.UInt32(StringPool.Get("AddUI"));
		write.String(cbi.GetJsonString());
		write.Send(send);
	}

	public void Dispose()
	{
		for (var i = 0; i < elements.Count; i++)
		{
			LuiContainer element = elements[i];
			foreach (LuiCompBase component in element.luiComponents)
			{
				if (component.type == LuiCompType.RectTransform)
					LuiPool.ReturnRect(component as LuiRectTransform);
				if (component.type == LuiCompType.Image)
					LuiPool.ReturnImage(component as LuiImagePanel);
			}
			LuiPool.ReturnContainer(element);
		}
		elements.Clear();
	}

	public struct LuiContainer
	{
		public string name;
		public string parent;
		public LuiComponentDictionary luiComponents;
		public string destroyUi;
		public float fadeOut;
		public bool update;

		public LuiContainer SetAnchors(LuiPosition pos)
		{
			if (luiComponents.TryGetValue<LuiRectTransform>(LuiCompType.RectTransform, out var rect))
			{
				rect.anchor = pos;
			}
			else
			{
				rect = LuiPool.GetRect();
				rect.anchor = pos;
				luiComponents.Add(LuiCompType.RectTransform, rect);
			}
			return this;
		}

		public LuiContainer SetOffset(LuiOffset off)
		{
			if (luiComponents.TryGetValue<LuiRectTransform>(LuiCompType.RectTransform, out var rect))
			{
				rect.offset = off;
			}
			else
			{
				rect = LuiPool.GetRect();
				rect.offset = off;
				luiComponents.Add(LuiCompType.RectTransform, rect);
			}
			return this;
		}
		public LuiContainer AddColor(string color)
		{
			if (luiComponents.TryGetValue<LuiImagePanel>(LuiCompType.Image, out var img))
			{
				img.color = color;
			}
			else
			{
				img = LuiPool.GetImage();
				img.color = color;
				luiComponents.Add(LuiCompType.Image, img);
			}
			return this;
		}

		public LuiContainer SetMaterial(string material)
		{
			if (luiComponents.TryGetValue<LuiImagePanel>(LuiCompType.Image, out var img))
			{
				img.material = material;
			}
			else
			{
				img = LuiPool.GetImage();
				img.material = material;
				luiComponents.Add(LuiCompType.Image, img);
			}
			return this;
		}
	}
}

public class LuiComponentDictionary : IEnumerable
{
	private readonly LuiCompBase[] _values;
	private int _count;
	private const int DictionarySize = 10; //Dictionary based on most possible types of components in one element, at the date od 24.02.2025 it's 8 (including draggables), so adding 2 more for safety.

	public LuiComponentDictionary()
	{
		_values = new LuiCompBase[DictionarySize];
		_count = 0;
	}

	public int Count => _count;

	public void Add<T>(LuiCompType key, T value) where T : LuiCompBase
	{
		if (_count >= _values.Length)
			throw new InvalidOperationException("Dictionary is full");

		_values[_count] = value;
		_count++;
	}

	public void Clear()
	{
		_count = 0;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool TryGetValue<T>(LuiCompType key, out T value) where T : LuiCompBase
	{
		for (int i = 0; i < _count; i++)
		{
			if (_values[i].type == key && _values[i] is T typedValue)
			{
				value = typedValue;
				return true;
			}
		}
		value = null;
		return false;
	}

	public IEnumerator GetEnumerator()
	{
		for (int i = 0; i < _count; i++)
		{
			yield return _values[i];
		}
	}
}

public readonly struct LuiOffset
{
	public static readonly LuiOffset None = new(0, 0, 0, 0);

	public readonly Vector2 offsetMin;
	public readonly Vector2 offsetMax;

	public LuiOffset(float xMin, float yMin, float xMax, float yMax)
	{
		offsetMin = new Vector2(xMin, yMin);
		offsetMax = new Vector2(xMax, yMax);
	}
}

public readonly struct LuiPosition
{
	public static readonly LuiPosition None = new(0, 0, 0, 0);
	public static readonly LuiPosition Full = new(0, 0, 1, 1);
	public static readonly LuiPosition UpperLeft = new(0, 1, 0, 1);
	public static readonly LuiPosition UpperCenter = new(.5f, 1, .5f, 1);
	public static readonly LuiPosition UpperRight = new(1, 1, 1, 1);
	public static readonly LuiPosition MiddleLeft = new(0, .5f, 0, .5f);
	public static readonly LuiPosition MiddleCenter = new(.5f, .5f, .5f, .5f);
	public static readonly LuiPosition MiddleRight = new(1, .5f, 1, .5f);
	public static readonly LuiPosition LowerLeft = new(0, 0, 0, 0);
	public static readonly LuiPosition LowerCenter = new(.5f, 0, .5f, 0);
	public static readonly LuiPosition LowerRight = new(1, 0, 1, 0);

	public static readonly LuiPosition Upper = new(0, 1, 1, 1);
	public static readonly LuiPosition Lower = new(0, 0, 1, 0);
	public static readonly LuiPosition Left = new(0, 0, 0, 1);
	public static readonly LuiPosition Right = new(1, 0, 1, 1);

	public readonly Vector2 anchorMin;
	public readonly Vector2 anchorMax;

	public LuiPosition(float xMin, float yMin, float xMax, float yMax)
	{
		anchorMin = new Vector2(xMin, yMin);
		anchorMax = new Vector2(xMax, yMax);
	}
}

public enum LuiCompType : int
{
	Text = 0,
	Image = 1,
	RawImage = 2,
	Button = 3,
	Outline = 4,
	InputField = 5,
	NeedsCursor = 6,
	RectTransform = 7,
	Countdown = 8,
	Draggable = 9,
	Slot = 10,
	NeedsKeyboard = 11,
	ScrollView = 12,
}

public class LuiCompBase
{
	public LuiCompType type;
}

public class LuiRectTransform : LuiCompBase
{
	public LuiPosition anchor = LuiPosition.Full;
	public LuiOffset offset = LuiOffset.None;
	public string setParent;
	public int setTransformIndex;

	public LuiRectTransform()
	{
		type = LuiCompType.RectTransform;
	}
}

public class LuiImagePanel : LuiCompBase
{
	public string sprite;
	public string material;
	public string color;
	public UnityEngine.UI.Image.Type imageType;
	public uint png;
	public int itemid;
	public ulong skinid;
	public float fadeIn;

	public LuiImagePanel()
	{
		type = LuiCompType.Image;
	}
}

public static class LuiPool
{

	private static readonly Stack<LUI.LuiContainer> _containers = new();
	private static readonly Stack<LuiRectTransform> _rects = new();
	private static readonly Stack<LuiImagePanel> _images  = new();

	public static LuiRectTransform GetRect() => _rects.Count > 0 ? _rects.Pop() : new();

	public static void ReturnRect(LuiRectTransform rect) => _rects.Push(rect);

	public static LuiImagePanel GetImage() => _images.Count > 0 ? _images.Pop() : new();

	public static void ReturnImage(LuiImagePanel rect) => _images.Push(rect);


	public static LUI.LuiContainer GetContainer()
	{
		if (_containers.Count == 0)
		{
			LUI.LuiContainer cont = new LUI.LuiContainer();
			cont.luiComponents = new();
			return cont;
		}
		return _containers.Pop();
	}

	public static void ReturnContainer(LUI.LuiContainer container) => _containers.Push(container);


}

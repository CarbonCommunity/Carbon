/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * All rights reserved.
 *
 */

namespace Carbon.Components;

public struct StringBody : IDisposable
{
	public List<object> Items => _items ??= new List<object>();

	internal List<object> _items { get; set; }

	public StringBody Add(object data)
	{
		Items.Add(data ?? string.Empty);

		return this;
	}
	public StringBody Add(object[] datas)
	{
		Items.AddRange(datas);

		return this;
	}
	public StringBody Empty()
	{
		Add(data: null);

		return this;
	}
	public StringBody Remove(object data)
	{
		Items.Remove(data);

		return this;
	}
	public StringBody Clear()
	{
		Items.Clear();

		return this;
	}

	public enum ExportTypes
	{
		Append,
		NewLine
	}

	#region "To" Family

	public string ToAppended(string inBetweenString = " ")
	{
		var result = string.Empty;

		foreach (var item in Items)
		{
			result += $"{item}{inBetweenString}";
		}

		return result.TrimEnd();
	}
	public string ToNewLine()
	{
		var result = string.Empty;

		foreach (var item in Items)
		{
			result += $"{item}\n";
		}

		return result.TrimEnd();
	}
	public override string ToString()
	{
		return ToAppended(string.Empty);
	}

	#endregion

	#region "Export" Family

	public StringBody Export(string filePath, ExportTypes exportType, bool @override = true, string inBetweenString = " ", int padding = 50, char paddingCharacter = ' ', bool spacing = true)
	{
		if (!@override && OsEx.File.Exists(filePath))
		{
			return this;
		}

		try
		{
			switch (exportType)
			{
				case ExportTypes.Append:
					OsEx.File.Create(filePath, ToAppended(inBetweenString));
					break;

				case ExportTypes.NewLine:
					OsEx.File.Create(filePath, ToNewLine());
					break;
			}
		}
		catch { }

		return this;
	}

	#endregion

	public void Dispose()
	{
		Items.Clear();
		_items = null;
	}
}

namespace Carbon.Extensions;

public static class ConsoleArgEx
{
	public static char[] CommandSpacing = new char[] { ' ' };

	public static bool TryParseCommand(string input, out string command, out string[] args)
	{
		if (input == null)
		{
			command = string.Empty;
			args = [];
			return false;
		}

		return TryParseCommand(input.AsSpan(), out command, out args);
	}

	public static bool TryParseCommand(ReadOnlySpan<char> input, out string command, out string[] args)
	{
		command = string.Empty;
		args = [];

		if (input.IsEmpty)
		{
			return false;
		}

		var foundOtherChar = false;
		var leftSpacingIdx = -1;
		var rightSpacingIdx = -1;

		for (var i = 0; i < input.Length; i++)
		{
			var ch = input[i];

			if (IsCommandSpacing(ch))
			{
				if (foundOtherChar)
				{
					rightSpacingIdx = i;
					break;
				}

				leftSpacingIdx = i;
			}
			else if (!foundOtherChar)
			{
				foundOtherChar = true;
			}
		}

		if (!foundOtherChar)
		{
			return false;
		}

		var commandStart = leftSpacingIdx + 1;
		var commandEnd = rightSpacingIdx == -1 ? input.Length : rightSpacingIdx;
		command = input[commandStart..commandEnd].ToString();

		if (rightSpacingIdx == -1)
		{
			return true;
		}

		var argsStart = -1;

		for (var i = commandEnd; i < input.Length; i++)
		{
			if (!IsCommandSpacing(input[i]))
			{
				argsStart = i;
				break;
			}
		}

		if (argsStart == -1)
		{
			return true;
		}

		args = Facepunch.Extend.StringExtensions.SplitQuotesStrings(input[argsStart..].ToString());
		return true;
	}

	public static bool IsPlayerCalledOrAdmin(this ConsoleSystem.Arg arg)
	{
		return arg.Player() == null || arg.IsAdmin;
	}

	private static bool IsCommandSpacing(char ch)
	{
		for (var i = 0; i < CommandSpacing.Length; i++)
		{
			if (ch == CommandSpacing[i])
			{
				return true;
			}
		}

		return false;
	}
}

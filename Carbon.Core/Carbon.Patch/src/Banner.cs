namespace Carbon.Patch
{
	internal partial class Program
	{
		public static void PrintBanner()
		{
			Utility.LogNone(@"  ______ _______ ______ ______ _______ _______ ");
			Utility.LogNone(@" |      |   _   |   __ \   __ \       |    |  |");
			Utility.LogNone(@" |   ---|       |      <   __ <   -   |       |");
			Utility.LogNone(@" |______|___|___|___|__|______/_______|__|____|");
			Utility.LogNone(string.Empty);
			Utility.LogNone($" {Utility.GetAssemblyName()} {Utility.GetCopyright()}");
			Utility.LogNone(string.Empty);
		}
	}
}
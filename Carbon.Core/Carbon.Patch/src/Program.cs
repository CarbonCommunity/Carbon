using System;
using CommandLine;

namespace Carbon.Patch
{
	internal partial class Program
	{
		private static Options Arguments;

		public static void Main ( string [] args )
		{
			// ASCii art
			PrintBanner ();

			CommandLine.Parser.Default.ParseArguments<Options> ( args )
				.WithNotParsed ( x => Environment.Exit ( 1 ) )
				.WithParsed ( x => Arguments = x );

			if ( Arguments.versionUpdate )
			{
				UpdateVersion ();
			}
			else
			{
				CreateReleaseDirectory ();
				ProcessCommonFiles ();
				ProcessWindowsFiles ();
				ProcessUnixFiles ();
			}
		}
	}
}
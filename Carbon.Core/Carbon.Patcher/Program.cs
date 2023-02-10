using System.IO;
using Carbon.Utility;
using Doorstop;

namespace Carbon.Patcher
{
	internal class Program
	{
		static void Main(string[] args)
		{
			Context.Base = Context.Carbon = ".";
			Context.RustManaged = Path.GetDirectoryName(args[0]);
			Context.Modules = args[1];
			Entrypoint.Execute(args[0]);
		}
	}
}

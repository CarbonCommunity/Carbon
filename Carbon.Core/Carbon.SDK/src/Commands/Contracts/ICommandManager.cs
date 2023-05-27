using System;
using System.Collections.Generic;

namespace API.Commands;

public interface ICommandManager
{
	List<Command> Chat { get; set; }
	List<Command> ClientConsole { get; set; }
	List<Command> RCon { get; set; }

	bool Contains(IList<Command> factory, string command, out Command outCommand);
	List<T> GetFactory<T>() where T : Command;
	List<Command> GetFactory(Command command);
	public IEnumerable<T> GetCommands<T>() where T : Command;

	void ClearCommands(Func<Command, bool> condition);
	bool Execute(Command command, Command.Args args);

	bool RegisterCommand(Command command, out string reason);
	bool UnregisterCommand(Command command, out string reason);
}

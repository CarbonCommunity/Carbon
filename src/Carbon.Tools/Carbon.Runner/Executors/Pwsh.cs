namespace Carbon.Runner.Executors;

public class Pwsh : Program
{
	public override string Name => "Pwsh";

	public Pwsh()
	{
		programFile = "pwsh";
	}
}

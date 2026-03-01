namespace API.Abstracts;

public abstract class CarbonAuto
{
	public static CarbonAuto Singleton;

	public abstract bool IsForceModded();
	public abstract void Refresh();
	public abstract void Save();
	public abstract void Load();
}

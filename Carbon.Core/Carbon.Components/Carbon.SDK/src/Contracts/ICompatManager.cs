namespace API.Assembly;

public interface ICompatManager
{
	public void Init();

	public ConversionResult AttemptOxideConvert(ref byte[] asm);

	public bool ConvertHarmonyMod(ref byte[] data, bool noEntrypoint = false);
}

public enum ConversionResult
{
	Success,
	Fail,
	Skip
}

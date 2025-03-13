namespace Oxide.Core.Plugins;

[AttributeUsage(AttributeTargets.Class)]
public class AutoPatchAttribute : Attribute
{
	public bool IsRequired;

	public AutoPatchAttribute()
	{
		IsRequired = false;
	}

	public AutoPatchAttribute(bool isRequired)
	{
		IsRequired = isRequired;
	}
}

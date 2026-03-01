namespace API.Permissions;

public partial interface IPermissionManager
{
	public IUserManagement Users { get; }

	public IGroupManagement Groups { get; }
}

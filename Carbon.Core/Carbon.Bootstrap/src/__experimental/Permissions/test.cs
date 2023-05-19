using System;
using API.Abstracts;
using API.Permissions;

/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * All rights reserved.
 *
 */

public sealed class Test1 : Singleton<Test1>, ITestInterface
{
	private Test1() { }

	public void DoStuff()
	{
		Console.WriteLine($"Stuff done by {this}");
	}
}

public sealed class Test2
{
	public void DoStuff(IPermissionManager Permissions)
	{
		Permissions.Users.Insert("10", null, null);
		Permissions.Users.Insert("20", "manel", null);
		Permissions.Users.Insert("30", null, "pt");
		Permissions.Users.Insert("40", "jose", "fr");

		Permissions.Groups.Insert("test1", "This is test 1", 50, null);
		Permissions.Groups.Insert("test2", "This is test 2", 10, null);
		Permissions.Groups.Insert("test3", "This is test 3", 60, "test1");

		Permissions.Groups.Insert("test4", "This is test 4", default, null);
		Permissions.Groups.Remove("test4");

		Permissions.Groups.AddGroupPermission("test1", "test1.1");
		Permissions.Groups.AddGroupPermission("test1", "test1.2");
		Permissions.Groups.AddGroupPermission("test1", "test1.3");

		Permissions.Groups.AddGroupPermission("test2", "test2.1");
		Permissions.Groups.AddGroupPermission("test2", "test2.2");
		Permissions.Groups.AddGroupPermission("test2", "test2.3");

		Permissions.Groups.AddGroupPermission("test3", "test3.1");
		Permissions.Groups.AddGroupPermission("test3", "test3.2");
		Permissions.Groups.AddGroupPermission("test3", "test3.3");

		Permissions.Groups.Remove("test2");
		Permissions.Groups.RemoveGroupPermission("test3", "test3.2");

		Permissions.Groups.AddGroupPermission("test4", "test4.1");
		Permissions.Groups.AddGroupPermission("test4", "test4.1");

		Permissions.Users.AddUserPermission("10", "foo.1");
		Permissions.Users.AddUserPermission("10", "foo.1");
		Permissions.Users.AddUserPermission("10", "foo.2");

		Permissions.Users.AddUserPermission("20", "foo.3");
		Permissions.Users.AddUserPermission("20", "foo.4");
		Permissions.Users.AddUserPermission("20", "foo.5");

		Permissions.Users.AddUserPermission("30", "foo.3");
		Permissions.Users.AddUserPermission("30", "foo.4");
		Permissions.Users.AddUserPermission("30", "foo.5");

		Permissions.Users.RemoveUserPermission("20", "foo.5");
		Permissions.Users.ResetPermissions("30");
	}
}

public interface ITestInterface
{
	public void DoStuff();
}

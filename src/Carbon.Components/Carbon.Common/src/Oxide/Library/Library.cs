namespace Oxide.Core.Libraries;

public class LibraryFunction : Attribute
{
	public string Name { get; set; }

	public LibraryFunction() { }
	public LibraryFunction(string name) { Name = name; }
}

[AttributeUsage(AttributeTargets.Property)]
public class LibraryProperty : Attribute
{
	public string Name { get; private set; }

	public LibraryProperty() { }
	public LibraryProperty(string name) => this.Name = name;
}

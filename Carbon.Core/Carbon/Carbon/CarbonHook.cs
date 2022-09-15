using System;

[AttributeUsage ( AttributeTargets.Class )]
public class Hook : Attribute
{
    public string Name { get; set; }
    public Type ReturnType { get; set; }

    public Hook ()
    {

    }
    public Hook ( string name )
    {
        Name = name;
        ReturnType = typeof ( void );
    }
    public Hook ( string name, Type returnType = null )
    {
        Name = name;
        ReturnType = returnType ?? typeof ( void );
    }

    [AttributeUsage ( AttributeTargets.Class, AllowMultiple = true )]
    public class Parameter : Attribute
    {
        public string Name { get; set; }
        public Type Type { get; set; }

        public Parameter () { }
        public Parameter ( string name, Type type = null )
        {
            Name = name;
            Type = type ?? typeof ( object );
        }
    }
}
using Oxide.Core;
using System.Linq;
using System;
using UnityEngine;

[AttributeUsage ( AttributeTargets.Class )]
public class InfoAttribute : Attribute
{
    public string Title { get; }

    public string Author { get; }

    public VersionNumber Version { get; private set; }

    public int ResourceId { get; set; }

    public InfoAttribute ( string Title, string Author, string Version )
    {
        this.Title = Title;
        this.Author = Author;
        SetVersion ( Version );
    }

    private void SetVersion ( string version )
    {
        ushort result;
        var list = ( from part in version.Split ( '.' )
                     select ( ushort )( ushort.TryParse ( part, out result ) ? result : 0 ) ).ToList ();
        while ( list.Count < 3 )
        {
            list.Add ( 0 );
        }

        if ( list.Count > 3 )
        {
            Debug.LogWarning ( "Version `" + version + "` is invalid for " + Title + ", should be `major.minor.patch`" );
        }

        Version = new VersionNumber ( list [ 0 ], list [ 1 ], list [ 2 ] );
    }
}

[AttributeUsage ( AttributeTargets.Class )]
public class DescriptionAttribute : Attribute
{
    public string Description { get; }

    public DescriptionAttribute ( string description )
    {
        Description = description;
    }
}

[AttributeUsage ( AttributeTargets.Method | AttributeTargets.Field )]
public class PluginReferenceAttribute : Attribute
{
    public PluginReferenceAttribute ()
    {
    }
}

[AttributeUsage ( AttributeTargets.Method, AllowMultiple = true )]
public class ChatCommandAttribute : Attribute
{
    public string Name { get; }
    public string Help { get; }

    public ChatCommandAttribute ( string name, string help = null )
    {
        Name = name;
        Help = help;
    }
}

[AttributeUsage ( AttributeTargets.Method, AllowMultiple = true )]
public class ConsoleCommandAttribute : Attribute
{
    public string Name { get; }
    public string Help { get; }
    public bool Skip { get; } = true;

    public ConsoleCommandAttribute ( string name )
    {
        Name = name;
    }

    public ConsoleCommandAttribute ( string name, string help )
    {
        Name = name;
        Help = help;
    }

    public ConsoleCommandAttribute ( string name, bool skip )
    {
        Name = name;
        Skip = skip;
    }

    public ConsoleCommandAttribute ( string name, string help, bool skip )
    {
        Name = name;
        Help = help;
        Skip = skip;
    }
}

[AttributeUsage ( AttributeTargets.Method )]
public class HookMethodAttribute : Attribute
{
    public string Name { get; set; }

    public HookMethodAttribute () { }
    public HookMethodAttribute ( string name )
    {
        Name = name;
    }
}
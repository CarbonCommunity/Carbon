using System.IO;
using System;

namespace ProtoBuf
{
    public static class Serializer
    {
        public static void Serialize ( MemoryStream stream, object obj )
        {

        }

        public static T Deserialize<T> ( MemoryStream stream )
        {
            return default;
        }
    }

    [AttributeUsage ( AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum | AttributeTargets.Interface, AllowMultiple = false, Inherited = false )]
    public sealed class ProtoContractAttribute : Attribute { }

    [AttributeUsage ( AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = true )]
    public class ProtoMemberAttribute : Attribute
    {
        public ProtoMemberAttribute () { }
        public ProtoMemberAttribute ( int i ) { }
    }
}

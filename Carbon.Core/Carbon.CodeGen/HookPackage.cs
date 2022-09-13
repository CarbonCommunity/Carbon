using System;

namespace Carbon.CodeGen
{
    [Serializable]
    public class HookPackage
    {
        public Manifest [] Manifests { get; set; }

        public class Manifest
        {
            public HookInfo [] Hooks { get; set; }

            public class HookInfo
            {
                public string Type { get; set; }
                public HookData Hook { get; set; }
            }
            public class HookData
            {
                public int InjectionIndex { get; set; }
                public string HookTypeName { get; set; }
                public string Name { get; set; }
                public string HookName { get; set; }
                public string AssemblyName { get; set; }
                public string TypeName { get; set; }
                public bool Flagged { get; set; }
                public HookSignature Signature { get; set; }
                public string MSILHash { get; set; }
                public string HookCategory { get; set; }
            }

            public class HookSignature
            {
                public int Exposure { get; set; }
                public string Name { get; set; }
                public string ReturnType { get; set; }
                public string [] Parameters { get; set; }
            }
        }
    }
}

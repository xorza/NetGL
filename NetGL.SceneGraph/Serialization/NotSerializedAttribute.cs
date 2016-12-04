using System;

namespace NetGL.SceneGraph.Serialization {
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class NotSerializedAttribute : Attribute {
    }
}
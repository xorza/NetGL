using System;

namespace NetGL.SceneGraph.Components
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ColorAttribute : Attribute
    {
        public ColorAttribute() { }
    }
}
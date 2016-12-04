using System;

namespace NetGL.SceneGraph.Components
{
    [AttributeUsage(AttributeTargets.Property)]
    public class NumberRangeAttribute : Attribute
    {
        public float Min { get; private set; }
        public float Max { get; private set; }

        public NumberRangeAttribute(float min, float max)
        {
            Assert.True(max > min);

            this.Min = min;
            this.Max = max;
        }
    }
}
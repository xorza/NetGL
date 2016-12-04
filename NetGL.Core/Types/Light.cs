using NetGL.Core.Mathematics;

namespace NetGL.Core.Types {
    public enum LightType {
        Directional = 0,
        Point = 1,
        //Spot = 2
    }

    public class Light {
        public Vector3 Diffuse { get; set; }
        public Vector3 Position { get; set; }
        public Vector3 Direction { get; set; }
        public LightType Type { get; set; }
        public Vector3 Attenuation { get; set; }

        public Light() {
            Type = LightType.Point;
            Attenuation = new Vector3(1, 0.3f, 0f);
        }
    }
}
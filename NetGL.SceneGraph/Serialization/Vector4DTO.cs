using NetGL.Core.Mathematics;

namespace NetGL.SceneGraph.Serialization {
    public class Vector4DTO {
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }
        public float W { get; set; }

        public Vector4DTO() {
        }

        public Vector4DTO(Vector4 v) {
            this.X = v.X;
            this.Y = v.Y;
            this.Z = v.Z;
            this.W = v.W;
        }

        public Vector4 ToVector4() {
            return new Vector4(X, Y, Z, W);
        }

        public static implicit operator Vector4(Vector4DTO dto) {
            return dto.ToVector4();
        }
    }
}
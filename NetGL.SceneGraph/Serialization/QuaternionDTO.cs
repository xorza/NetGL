using NetGL.Core.Mathematics;

namespace NetGL.SceneGraph.Serialization {
    public class QuaternionDTO {
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }
        public float W { get; set; }

        public QuaternionDTO() {
        }

        public QuaternionDTO(Quaternion v) {
            this.X = v.X;
            this.Y = v.Y;
            this.Z = v.Z;
            this.W = v.W;
        }

        public Quaternion ToQuaternion() {
            return new Quaternion(X, Y, Z, W);
        }

        public static implicit operator Quaternion(QuaternionDTO dto) {
            return dto.ToQuaternion();
        }
    }
}
using NetGL.Core.Mathematics;

namespace NetGL.SceneGraph.Serialization {
    public class Vector2DTO  {
        public float X { get; set; }
        public float Y { get; set; }

        public Vector2DTO() {
        }

        public Vector2DTO(Vector2 v) {
            this.X = v.X;
            this.Y = v.Y;
        }

        public Vector2 ToVector2() {
            return new Vector2(X, Y);
        }

        public static implicit operator Vector2(Vector2DTO dto) {
            return dto.ToVector2();
        }
    }
}
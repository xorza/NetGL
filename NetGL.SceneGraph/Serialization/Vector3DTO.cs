using NetGL.Core.Mathematics;

namespace NetGL.SceneGraph.Serialization
{
    public class Vector3DTO 
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }

        public Vector3DTO()
        {
        }

        public Vector3DTO(Vector3 v)
        {
            this.X = v.X;
            this.Y = v.Y;
            this.Z = v.Z;
        }

        public Vector3 ToVector3()
        {
            return new Vector3(X, Y, Z);
        }

        public static implicit operator Vector3(Vector3DTO dto)
        {
            return dto.ToVector3();
        }
    }
}
using NetGL.Core.Mathematics;
using NetGL.SceneGraph.Scene;

namespace NetGL.SceneGraph.Components {
    public class Shaker : Component, IUpdatable {
        public float Speed { get; set; }
        public Vector3 Scale { get; set; }
        public Vector3 Offset { get; set; }


        public Shaker(Node owner)
            : base(owner) {
            Speed = 1f;
            Scale = new Vector3(0.1f);
            Offset = new Vector3(0);
        }

        public void Update() {
            Transform.WorldPosition = Offset + Scale * MathF.Noise3(Time.CurrentFloat * Speed);
        }
    }
}

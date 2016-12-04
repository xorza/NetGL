using System.Runtime.InteropServices;
using NetGL.Core.Mathematics;
using NetGL.SceneGraph.Scene;

namespace NetGL.SceneGraph.Components {
    [Guid("57B20D0C-0F1B-4F9B-AC8B-C6D81F549F23")]
    public class Rotator : Component, IUpdatable {
        public Vector3 Rotation { get; set; }

        public Rotator(Node owner) : base(owner) { }

        public void Update() {
            Transform.WorldRotation = Quaternion.CreateFromYawPitchRoll(Rotation * Time.CurrentFloat);
        }
    }
}
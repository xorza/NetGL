using NetGL.Core.Mathematics;
using NetGL.SceneGraph.Scene;

namespace NetGL.SceneGraph.Tweens {
    public class ShakeTween : ITween {
        private readonly SceneTime _time;
        private readonly Vector3 _startPosition;

        public Transform Transform { get; set; }
        public object CancellationToken { get { return Transform; } }
        public float Speed { get; set; }
        public Vector3 Scale { get; set; }

        public ShakeTween(SceneTime time, Transform transform, float speed, Vector3 scale) {
            Assert.NotNull(transform);
            Assert.NotNull(time);

            _time = time;
            Speed = 1;
            Transform = transform;
            Scale = scale;

            _startPosition = Transform.LocalPosition;
        }

        public bool MoveNext() {
            if (Transform.IsDisposed)
                return false;

            Transform.LocalPosition = _startPosition + Scale * MathF.Noise3(_time.CurrentFloat * Speed);
            return true;
        }
    }
}

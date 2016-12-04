using System;
using NetGL.Core.Mathematics;
using NetGL.SceneGraph.Scene;

namespace NetGL.SceneGraph.Tweens {
    public class PositionTween : TweenBase {
        public Vector3 StartPosition { get; private set; }
        public Vector3 FinishPosition { get; private set; }
        public Transform Transform { get; private set; }

        public PositionTween(SceneTime time, EasingType type, float duration, Vector3 startPosition, Vector3 finishPosition, Transform transform, Action finishCallback)
            : base(time, type, duration, 0, 1, finishCallback) {

            Assert.NotNull(transform);

            CancellationToken = transform;
            StartPosition = startPosition;
            FinishPosition = finishPosition;
            Transform = transform;
        }
        public PositionTween(SceneTime time, EasingType type, float duration, Vector3 finishPosition, Transform transform, Action finishCallback)
            : base(time, type, duration, 0, 1, finishCallback) {

            Assert.NotNull(transform);

            StartPosition = transform.WorldPosition;
            FinishPosition = finishPosition;
            Transform = transform;
        }

        protected override void SetValue(float v) {
            if (Transform.IsDisposed) {
                Finish();
                return;
            }

            var value = Vector3.Lerp(StartPosition, FinishPosition, v);
            Transform.WorldPosition = value;
        }
    }
}

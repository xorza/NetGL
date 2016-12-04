using System;
using NetGL.Core.Mathematics;
using NetGL.SceneGraph.Scene;

namespace NetGL.SceneGraph.Tweens {
    public class RotationTween : TweenBase {
        public Quaternion StartRotation { get; private set; }
        public Quaternion FinishRotation { get; private set; }
        public Transform Transform { get; private set; }

        public RotationTween(SceneTime time, EasingType type, float duration, Quaternion startRotation, Quaternion finishRotation, Transform transform, Action finishCallback)
            : base(time, type, duration, 0, 1, finishCallback) {

            Assert.NotNull(transform);

            CancellationToken = transform;
            StartRotation = startRotation;
            FinishRotation = finishRotation;
            Transform = transform;
        }
        public RotationTween(SceneTime time, EasingType type, float duration, Quaternion finishRotation, Transform transform, Action finishCallback)
            : base(time, type, duration, 0, 1, finishCallback) {

            Assert.NotNull(transform);

            StartRotation = transform.WorldRotation;
            FinishRotation = finishRotation;
            Transform = transform;
        }

        protected override void SetValue(float v) {
            if (Transform.IsDisposed) {
                Finish();
                return;
            }

            var value = Quaternion.Lerp(StartRotation, FinishRotation, v);
            Transform.WorldRotation = value;
        }
    }
}

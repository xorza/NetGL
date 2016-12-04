using System;
using NetGL.Core.Mathematics;
using NetGL.SceneGraph.Scene;

namespace NetGL.SceneGraph.Tweens {
    internal enum TweenState {
        Working,
        Callback,
        Finished
    }

    public class FloatTween : TweenBase, ITween {
        private readonly Action<float> _setter;

        public FloatTween(SceneTime time, EasingType type, Action<float> setter, float duration, float startValue, float finishValue, Action finishCallback)
            : base(time, type, duration, startValue, finishValue, finishCallback) {
            Assert.NotNull(setter);

            _setter = setter;
        }

        protected override void SetValue(float v) {
            _setter(v);
        }
    }
}

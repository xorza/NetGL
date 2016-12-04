using System;
using NetGL.Core.Mathematics;
using NetGL.SceneGraph.Scene;

namespace NetGL.SceneGraph.Tweens {
    public abstract class TweenBase : ITween {
        private readonly SceneTime _time;

        private readonly float _startTime;
        private readonly float _finishTime;
        private readonly float _duration;
        private float _current;
        private readonly float _startValue;
        private readonly float _finishValue;
        private readonly EasingFunctionDelegate _function;

        private readonly Action _finishCallback;

        private TweenState _state;

        public object CancellationToken { get; set; }

        protected TweenBase(SceneTime time, EasingType type, float duration, float startValue, float finishValue, Action finishCallback) {
            Assert.NotNull(time);

            _time = time;
            _startTime = time.CurrentFloat;
            _current = 0;
            _duration = duration;
            _finishTime = _duration + _startTime;
            _function = EasingFunctions.Get(type);
            _state = TweenState.Working;
            _startValue = startValue;
            _finishValue = finishValue;
            _finishCallback = finishCallback;
        }

        public bool MoveNext() {
            switch (_state) {
                case TweenState.Working:
                    return Iterate();
                case TweenState.Callback:
                    _state = TweenState.Finished;
                    _finishCallback();
                    return false;
                case TweenState.Finished:
                    return false;
                default:
                    throw new NotImplementedException();
            }
        }
        public void Finish() {
            _state = TweenState.Finished;
        }

        private bool Iterate() {
            if (_time.CurrentFloat >= _finishTime) {
                SetValue(_finishValue);

                if (_finishCallback == null) {
                    _state = TweenState.Finished;
                    return false;
                }
                else {
                    _state = TweenState.Callback;
                    return true;
                }
            }
            else {
                var time = MathF.Clamp(_time.CurrentFloat - _startTime, 0, _duration);
                var v = _function(time, 0, 1, _duration);
                _current = MathF.Lerp(_startValue, _finishValue, v);

                SetValue(_current);
                return true;
            }
        }

        protected abstract void SetValue(float v);
    }
}

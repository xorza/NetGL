using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;
using System.Threading;

namespace NetGL.SceneGraph.Scene {
    public class SceneTime {
        private readonly double _frequency;
        private readonly bool _isWorking;
        private readonly long _start;

        private double _prevTime;
        private float _delta;

        private float _currentF;
        private double _currentD;

        internal SceneTime() {
            long temp;

            _isWorking = NativeMethods.QueryPerformanceFrequency(out temp);
            if (_isWorking == false)
                return;

            _frequency = temp;
            NativeMethods.QueryPerformanceCounter(out _start);
        }

        public float CurrentFloat {
            get {
                return _currentF;
            }
        }
        public double CurrentDouble {
            get {
                return (float)_currentD;
            }
        }
        public float Delta {
            get {
                return _delta;
            }
        }

        internal void CalcCurrent() {
            if (_isWorking == false) {
                _currentD = 0;
                return;
            }

            long temp;
            NativeMethods.QueryPerformanceCounter(out temp);

            _prevTime = _currentD;
            _currentD = (temp - _start) / _frequency;
            _currentF = (float)_currentD;
            _delta = (float)(_currentD - _prevTime);
        }
    }
}
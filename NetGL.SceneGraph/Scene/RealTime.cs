
namespace NetGL.SceneGraph.Scene {
    public static class RealTime {
        private static readonly double _frequency;
        private static readonly bool _isWorking;

        private static readonly long _start;

        static RealTime() {
            long temp;
            _isWorking = NativeMethods.QueryPerformanceFrequency(out temp);
            if (_isWorking == false)
                return;

            _frequency = temp;
            NativeMethods.QueryPerformanceCounter(out _start);
        }

        public static double Time {
            get {
                if (_isWorking == false)
                    return 0;

                long temp;
                NativeMethods.QueryPerformanceCounter(out temp);
                return (temp - _start) / _frequency;
            }
        }
    }
}

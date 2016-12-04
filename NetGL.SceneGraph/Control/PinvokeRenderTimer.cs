using NetGL.Core.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetGL.SceneGraph.Control {
    public delegate void PinvokeRenderTimerEventHandler(PinvokeRenderTimer sender);

    public sealed class PinvokeRenderTimer : IDisposable {
        private readonly object _locker = new object();

        private bool _isDisposed;
        private bool _isStarted;

        private UInt32 _timerId;
        private readonly TimerEventHandler _callback;
        private readonly PinvokeRenderTimerEventHandler _handler;

        public UInt32 Resolution { get; private set; }
        public UInt32 Interval { get; private set; }

        public PinvokeRenderTimer(UInt32 resolution, UInt32 interval, PinvokeRenderTimerEventHandler handler) {
            if (handler == null)
                throw new ArgumentNullException("handler");

            _isDisposed = false;
            _callback = TimerCallback;
            _isStarted = false;
            _handler = handler;

            Resolution = 0;
            Interval = 30;
        }

        private void TimerCallback(UInt32 id, UInt32 msg, ref UInt32 user, UInt32 dw1, UInt32 dw2) {
            lock (_locker) {
                if (_isDisposed)
                    return;

                Assert.True(_isStarted);

                _handler(this);
            }
        }

        public void Start() {
            if (_isDisposed)
                throw new ObjectDisposedException("RenderTimer");

            lock (_locker) {
                if (_isStarted == true)
                    return;
                _isStarted = true;

                NativeMethods.TimeBeginPeriod(1);
                UInt32 userCtx = 0;
                _timerId = NativeMethods.TimeSetEvent(Interval, Resolution, _callback, ref userCtx, EventType.TimePeriodic);
            }
        }

        public void Dispose() {
            lock (_locker) {
                if (_isDisposed)
                    return;
                _isDisposed = true;

                GC.SuppressFinalize(this);

                if (_isStarted == false)
                    return;
                _isStarted = false;

                NativeMethods.TimeEndPeriod(1);
                NativeMethods.TimeKillEvent(_timerId);
            }
        }
        ~PinvokeRenderTimer() {
            Dispose();
        }
    }
}

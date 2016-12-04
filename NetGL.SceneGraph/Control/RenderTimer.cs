using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NetGL.SceneGraph.Control {
    public delegate void RenderTimerEventHandler(RenderTimer sender);

    public sealed class RenderTimer : IDisposable {
        private Thread _thread;
        private bool _isStarted, _isDisposed;
        private int _interval;

        private readonly ManualResetEvent _waitHandle = new ManualResetEvent(false);

        public Int32 Interval {
            get {
                return _interval;
            }
            set {
                if (value < 1)
                    throw new ArgumentOutOfRangeException("Interval");

                _interval = value;
            }
        }

        public event RenderTimerEventHandler Render;

        public RenderTimer() {
            _isStarted = false;
            _isDisposed = false;
            _interval = 20;
        }

        public void Start() {
            if (_isDisposed)
                throw new ObjectDisposedException("RenderTimer");

            if (_isStarted)
                return;
            _isStarted = true;
            _thread = new Thread(MainLoop);
            _thread.Name = "RenderTimer main loop";
            _thread.Start();
        }

        private void MainLoop() {
            try {
                NativeMethods.TimeBeginPeriod(1);

                while (_isStarted) {
                    if (_waitHandle.WaitOne(Interval))
                        break;

                    if (Render != null)
                        Render(this);
                }
            }
            finally {
                NativeMethods.TimeEndPeriod(1);
            }
        }

        public void Dispose() {
            OnDispose(true);
            GC.SuppressFinalize(this);
        }
        private void OnDispose(bool isDisposing) {
            if (_isDisposed)
                return;
            _isDisposed = true;
            _isStarted = false;

            _waitHandle.Set();

            if (isDisposing) {
                GC.SuppressFinalize(this);
                _thread.Join();
            }
        }
        ~RenderTimer() {
            OnDispose(false);
        }
    }
}

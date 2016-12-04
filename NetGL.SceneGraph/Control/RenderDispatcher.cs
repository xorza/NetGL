using System;
using System.Runtime.InteropServices;
using System.Security;
using System.Threading;

namespace NetGL.SceneGraph.Control {
    public class RenderDispatcher : IDisposable {
        private static readonly ThreadLocal<RenderDispatcher> _current = new ThreadLocal<RenderDispatcher>(true);

        public static RenderDispatcher Current {
            get {
                return _current.Value;
            }
        }


        private bool _isWorking;

        public event Action Render;

        public RenderDispatcher() {
            if (Current != null)
                throw new InvalidOperationException("RenderDispatcher is already created for this thread.");

            _isWorking = true;
            _current.Value = this;
        }

        public void Run() {
            Message msg;
            bool messagePeeked = true;

            while (_isWorking || messagePeeked) {
                messagePeeked = NativeMethods.PeekMessage(out msg, IntPtr.Zero, 0, 0, MessageFlag.Remove);
                if (messagePeeked) {
                    NativeMethods.TranslateMessage(ref msg);
                    NativeMethods.DispatchMessage(ref msg);
                }
                else {
                    if (Render != null)
                        Render();
                    else
                        NativeMethods.WaitMessage();
                }
            }
        }
        public void Shutdown() {
            if (_isWorking == false)
                return;

            Assert.True(Current == this);

            Render = null;
            _isWorking = false;

            _current.Value = null;
        }

        public void Dispose() {
            Shutdown();
        }
    }
}
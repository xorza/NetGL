using System;
using System.Diagnostics;
using NetGL.Core.Infrastructure;
using System.Runtime.InteropServices;

namespace NetGL.Core.Types {
    public delegate void DisposeAction(uint handle);

    public abstract class UIntObject : IDisposable {
        public static GL CurrentContext {
            get { return GL.GetCurrent(true); }
        }

        private readonly GL _glContext;
        private string _creationStackTrace;
        private GCHandle _creationStackTraceHandle;

        private bool _isDisposed = false;
        private bool _isInitialized = false;

        private ResourceReference _reference;
        private bool _referenceTracked = false;

        public uint Handle {
            get;
            private set;
        }
        public GL Context { get { return _glContext; } }
        public bool IsDisposed { get { return _isDisposed; } }

        protected UIntObject() {
            _glContext = CurrentContext;
            Handle = 0;
        }
        protected UIntObject(uint handle)
            : this() {
            if (handle == 0)
                throw new ArgumentException("handle");

            Initialize(handle);
        }

        protected void Initialize(uint handle) {
            if (_isDisposed)
                throw new ObjectDisposedException(ToString());
            if (_isInitialized)
                throw new InvalidOperationException("already initialized");
            if (handle == 0)
                throw new ArgumentException("handle");

            Handle = handle;
            _isInitialized = true;

            if (this.Context.ResourceTracker.SaveCreationStackTrace) {
                _creationStackTrace = Environment.StackTrace;
                _creationStackTraceHandle = GCHandle.Alloc(_creationStackTrace, GCHandleType.Normal);
            }

            if (this.Context.ResourceTracker.IsActive) {
                _reference = this.Context.ResourceTracker.AddTrackedResource(this, _creationStackTrace);
                _referenceTracked = true;
                Assert.NotNull(_reference);
            }
        }

        [Conditional("DEBUG")]
        [DebuggerStepThrough]
        protected void CheckContext() {
            if (Context.IsCurrent() == false)
                throw new GLException("Current context error");
        }

        internal abstract DisposeAction GetDisposeAction();
        protected virtual void OnDispose(bool isDisposing) { }
        public void Dispose() {
            if (_isDisposed)
                return;
            _isDisposed = true;

            GC.SuppressFinalize(this);

            OnDispose(true);

            if (_isInitialized == false)
                return;

            Assert.True(_creationStackTraceHandle.IsAllocated);
            Assert.True(Handle != 0);

            if (_referenceTracked == true) {
                Assert.NotNull(_reference);
                _reference.Dispose();
            }
            else
                Assert.Null(_reference);
                        
            _creationStackTraceHandle.Free();

            if (Context == null)
                return;
            if (Context.IsDisposed)
                return;

            CheckContext();
            GetDisposeAction()(Handle);
        }

        ~UIntObject() {
            OnDispose(false);

            if (_referenceTracked == false) {
                if (_isInitialized == false)
                    return;

                var context = Context;
                if (context != null && context.IsDisposed)
                    return;

                var text = string.Format("Object was not disposed.\n{0}\n{1}", ToString(), this._creationStackTrace);
                Log.Error(text);
                _creationStackTraceHandle.Free();
            }
        }
    }
}
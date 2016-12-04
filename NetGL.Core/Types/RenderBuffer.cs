using System.Drawing;

namespace NetGL.Core.Types {
    public class Renderbuffer : UIntObject, IHaveSize {
        private bool _storageSet = false;

        public Size Size {
            get;
            private set;
        }

        internal Renderbuffer()
            : base(CurrentContext.CreateRenderbuffer()) { }

        public void Bind() {
            Context.BindRenderbuffer(Handle);
        }

        public void Storage(RenderbufferStorage component, Size size) {
            Assert.False(_storageSet);

            Bind();
            Context.RenderbufferStorage(component, size.Width, size.Height);

            this.Size = size;
            _storageSet = true;
        }
        public void StorageMultisample(int sampleCount, RenderbufferStorage component, Size size) {
            Assert.False(_storageSet);

            Bind();
            Context.RenderbufferStorageMultisample(component, sampleCount, size.Width, size.Height);

            this.Size = size;
            _storageSet = true;
        }

        internal override DisposeAction GetDisposeAction() {
            return Context.DeleteRenderbuffer;
        }
    }
}

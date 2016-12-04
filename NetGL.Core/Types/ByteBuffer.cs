using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace NetGL.Core.Types {
    public unsafe class ByteBuffer : UnsafeBuffer {
        private readonly byte* _ptr;

        public int Length { get; private set; }

        public ByteBuffer(int size)
            : base(size) {
            _ptr = (byte*)Pointer;
        }
        public ByteBuffer(params byte[] bytes)
            : base(bytes.Length) {
            _ptr = (byte*)Pointer;

            Marshal.Copy(bytes, 0, Pointer, bytes.Length);
        }

        public byte this[int index] {
            get {
                CheckIndex(index);
                return _ptr[index];
            }
            set {
                CheckIndex(index);
                _ptr[index] = value;
            }
        }

        public static implicit operator ByteBuffer(byte[] bytes) {
            return new ByteBuffer(bytes);
        }

        [DebuggerStepThrough]
        protected void CheckIndex(int index) {
            if (index < 0 || index >= Length)
                throw new ArgumentOutOfRangeException("index");
        }
    }
}
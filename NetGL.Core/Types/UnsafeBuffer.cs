using System;
using System.IO;
using System.Runtime.InteropServices;

namespace NetGL.Core.Types {
    public class UnsafeBuffer : IDisposable {
        private bool _isDisposed = false;
        private readonly IntPtr _ptr;

        public IntPtr Pointer {
            get { return _ptr; }
        }
        public int SizeInBytes {
            get;
            private set;
        }
        public BufferUsage Usage { get; set; }

        public UnsafeBuffer(int bytes) {
            if (bytes <= 0)
                throw new ArgumentException("bytes");

            SizeInBytes = bytes;
            _ptr = Marshal.AllocHGlobal(SizeInBytes);
            Usage = BufferUsage.StaticDraw;
        }
        public UnsafeBuffer(byte[] bytes) {
            if (bytes == null)
                throw new ArgumentNullException("bytes");
            if (bytes.Length == 0)
                throw new ArgumentException("bytes");

            SizeInBytes = BitConverter.ToInt32(bytes, 0);
            _ptr = Marshal.AllocHGlobal(SizeInBytes);
            Usage = BufferUsage.StaticDraw;
            Marshal.Copy(bytes, sizeof(Int32), Pointer, SizeInBytes);
        }
        public UnsafeBuffer(BinaryReader reader) {
            if (reader == null)
                throw new ArgumentNullException("reader");

            SizeInBytes = reader.ReadInt32();
            var bytes = reader.ReadBytes(SizeInBytes);
            _ptr = Marshal.AllocHGlobal(SizeInBytes);
            Usage = BufferUsage.StaticDraw;
            Marshal.Copy(bytes, 0, Pointer, SizeInBytes);
        }
        public UnsafeBuffer(Stream stream) : this(new BinaryReader(stream)) { }

        public void Dispose() {
            if (_isDisposed)
                return;
            _isDisposed = true;

            Marshal.FreeHGlobal(_ptr);
            GC.SuppressFinalize(this);
        }
        ~UnsafeBuffer() {
            Dispose();
        }

        public byte[] ToBytes() {
            var prefix = BitConverter.GetBytes(SizeInBytes);
            var result = new byte[SizeInBytes + prefix.Length];

            Array.Copy(prefix, result, prefix.Length);
            Marshal.Copy(Pointer, result, prefix.Length, SizeInBytes);

            return result;
        }
        public void ToStream(Stream stream) {
            var bytes = ToBytes();
            stream.Write(bytes, 0, bytes.Length);
        }
    }
}

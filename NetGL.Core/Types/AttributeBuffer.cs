using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Linq;

namespace NetGL.Core.Types {
    public abstract class AttributeBuffer : UnsafeBuffer {
        internal protected abstract int StructInLine { get; }
        internal protected abstract int BytesInStruct { get; }

        public abstract VertexAttribPointerType DataType { get; }

        public int Length { get; private set; }

        protected AttributeBuffer(int bytes)
            : base(bytes) {
            Length = bytes / BytesInStruct;
        }
        protected AttributeBuffer(byte[] bytes)
            : base(bytes) {
            Length = bytes.Length / BytesInStruct;
        }
        protected AttributeBuffer(BinaryReader reader)
            : base(reader) {
            Length = SizeInBytes / BytesInStruct;
        }
        protected AttributeBuffer(Stream stream)
            : base(stream) {
            Length = SizeInBytes / BytesInStruct;
        }
    }

    public abstract class AttributeBuffer<T> : AttributeBuffer, IEnumerable<T> where T : struct {
        public abstract T this[int index] { get; set; }

        protected AttributeBuffer(byte[] bytes)
            : base(bytes) {
        }
        public AttributeBuffer(int totalBytes)
            : base(totalBytes) {
        }
        public AttributeBuffer(int count, T values, int totalBytes)
            : this(totalBytes) {
            for (int i = 0; i < count; i++)
                this[i] = values;
        }
        public AttributeBuffer(IList<T> array, int totalBytes)
            : this(totalBytes) {
            for (int i = 0; i < array.Count; i++)
                this[i] = array[i];
        }
        public AttributeBuffer(IEnumerable<T> array, int totalBytes)
            : this(totalBytes) {
            using (var enumerator = array.GetEnumerator()) {
                for (int i = 0; i < Length; i++) {
                    this[i] = enumerator.Current;
                    enumerator.MoveNext();
                }
            }
        }
        public AttributeBuffer(IReadOnlyCollection<T> array, int totalBytes)
            : this(totalBytes) {
            using (var enumerator = array.GetEnumerator()) {
                for (int i = 0; i < Length; i++) {
                    this[i] = enumerator.Current;
                    enumerator.MoveNext();
                }
            }
        }
        protected AttributeBuffer(BinaryReader reader) : base(reader) { }
        protected AttributeBuffer(Stream stream) : base(stream) { }

        public T[] ToArray() {
            var result = new T[Length];
            for (int i = 0; i < Length; i++)
                result[i] = this[i];

            return result;
        }
        public List<T> ToList() {
            var result = new List<T>(Length);
            for (int i = 0; i < Length; i++)
                result[i] = this[i];

            return result;
        }

        public IEnumerator<T> GetEnumerator() {
            for (int i = 0; i < Length; i++)
                yield return this[i];
        }
        IEnumerator IEnumerable.GetEnumerator() {
            for (int i = 0; i < Length; i++)
                yield return this[i];
        }

        [DebuggerStepThrough]
        protected void CheckIndex(int index) {
            if (index < 0 || index >= Length)
                throw new ArgumentOutOfRangeException("index");
        }
    }
}
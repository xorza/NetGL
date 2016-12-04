using System;
using System.Collections;
using System.Collections.Generic;

namespace NetGL.Core.Types {
    public sealed unsafe class FloatBuffer : AttributeBuffer<Single> {
        private const int _bytesInStruct = 4;

        private readonly Single* _arrPtr;

        internal protected override int BytesInStruct {
            get { return _bytesInStruct; }
        }
        public override VertexAttribPointerType DataType {
            get { return VertexAttribPointerType.Float; }
        }
        internal protected override int StructInLine {
            get { return 1; }
        }

        public int Count {
            get { return Length; }
        }
        public bool IsReadOnly {
            get { return false; }
        }

        public FloatBuffer(int length)
            : base(length * _bytesInStruct) {
            _arrPtr = (Single*)Pointer.ToPointer();
        }

        public FloatBuffer(uint size)
            : this((int)size) {
        }

        public FloatBuffer(Single[] array)
            : this(array.Length) {
            for (int i = 0; i < array.Length; i++)
                this[i] = array[i];
        }

        public FloatBuffer(List<Single> array)
            : this(array.Count) {
            for (int i = 0; i < array.Count; i++)
                this[i] = array[i];
        }

        public FloatBuffer(byte[] bytes)
            : base(bytes) {
            _arrPtr = (Single*)Pointer.ToPointer();
        }

        public override Single this[int index] {
            get {
                CheckIndex(index);
                return _arrPtr[index];
            }
            set {
                CheckIndex(index);
                _arrPtr[index] = value;
            }
        }

        public static implicit operator FloatBuffer(Single[] array) {
            return new FloatBuffer(array);
        }
        public static implicit operator FloatBuffer(List<Single> array) {
            return new FloatBuffer(array);
        }
    }
}
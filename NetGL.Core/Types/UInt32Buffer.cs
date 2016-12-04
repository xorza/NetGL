using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace NetGL.Core.Types {
    public sealed unsafe class UInt32Buffer : AttributeBuffer<UInt32> {
        private const int _bytesInStruct = 4;

        private readonly UInt32* _arrPtr;

        internal protected override int BytesInStruct {
            get { return _bytesInStruct; }
        }
        public override VertexAttribPointerType DataType {
            get { return VertexAttribPointerType.UnsignedInt; }
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

        public UInt32Buffer(int length)
            : base(length * _bytesInStruct) {
            _arrPtr = (UInt32*)Pointer.ToPointer();
        }

        public UInt32Buffer(uint size)
            : this((int)size) {
        }

        public UInt32Buffer(UInt32[] array)
            : this(array.Length) {
            for (int i = 0; i < array.Length; i++)
                this[i] = array[i];
        }

        public UInt32Buffer(List<UInt32> array)
            : this(array.Count) {
            for (int i = 0; i < array.Count; i++)
                this[i] = array[i];
        }

        public UInt32Buffer(byte[] bytes)
            : base(bytes) {
            _arrPtr = (UInt32*)Pointer.ToPointer();
        }

        public UInt32Buffer(BinaryReader reader) : base(reader) { }
        public UInt32Buffer(Stream stream) : base(stream) { }

        public override UInt32 this[int index] {
            get {
                CheckIndex(index);
                return _arrPtr[index];
            }
            set {
                CheckIndex(index);
                _arrPtr[index] = value;
            }
        }
        
        public static implicit operator UInt32Buffer(UInt32[] array) {
            var result = new UInt32Buffer(array.Length);

            for (int i = 0; i < array.Length; i++)
                result[i] = (UInt32)array[i];

            return result;
        }
        public static implicit operator UInt32Buffer(Int32[] array) {
            var result = new UInt32Buffer(array.Length);

            for (int i = 0; i < array.Length; i++)
                result[i] = (UInt32)array[i];

            return result;
        }
        public static implicit operator UInt32Buffer(List<UInt32> array) {
            var result = new UInt32Buffer(array.Count);

            for (int i = 0; i < array.Count; i++)
                result[i] = (UInt32)array[i];

            return result;
        }
        public static implicit operator UInt32Buffer(List<Int32> array) {
            var result = new UInt32Buffer(array.Count);

            for (int i = 0; i < array.Count; i++)
                result[i] = (UInt32)array[i];

            return result;
        }
    }
}
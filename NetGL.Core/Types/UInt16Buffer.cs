using System;
using System.Collections;
using System.Collections.Generic;

namespace NetGL.Core.Types {
    public sealed unsafe class UInt16Buffer : AttributeBuffer<ushort> {
        private const int _bytesInStruct = 2;
        
        private readonly UInt16* _arrPtr;

        internal protected override int BytesInStruct {
            get { return _bytesInStruct; }
        }
        public override VertexAttribPointerType DataType {
            get { return VertexAttribPointerType.UnsignedShort; }
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

        public UInt16Buffer(int length)
            : base(length * _bytesInStruct) {
            _arrPtr = (UInt16*)Pointer.ToPointer();
        }

        public UInt16Buffer(uint size)
            : this((int)size) {
        }

        public UInt16Buffer(UInt16[] array)
            : this(array.Length) {
            for (int i = 0; i < array.Length; i++)
                this[i] = array[i];
        }

        public UInt16Buffer(List<UInt16> array)
            : this(array.Count) {
            for (int i = 0; i < array.Count; i++)
                this[i] = array[i];
        }

        public UInt16Buffer(byte[] bytes)
            : base(bytes) {
            _arrPtr = (UInt16*)Pointer.ToPointer();
        }

        public override UInt16 this[int index] {
            get {
                CheckIndex(index);
                return _arrPtr[index];
            }
            set {
                CheckIndex(index);
                _arrPtr[index] = value;
            }
        }
        
        public static implicit operator UInt16Buffer(UInt16[] array) {
            return new UInt16Buffer(array);
        }
        public static implicit operator UInt16Buffer(List<UInt16> array) {
            return new UInt16Buffer(array);
        }
        public static implicit operator UInt16Buffer(Int32[] array) {
            var result = new UInt16Buffer(array.Length);

            for (int i = 0; i < array.Length; i++)
                result[i] = (UInt16)array[i];

            return result;
        }
        public static implicit operator UInt16Buffer(List<Int32> array) {
            var result = new UInt16Buffer(array.Count);

            for (int i = 0; i < array.Count; i++)
                result[i] = (UInt16)array[i];

            return result;
        }
    }
}
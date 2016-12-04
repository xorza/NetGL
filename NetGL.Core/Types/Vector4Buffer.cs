using System.Collections;
using System.Collections.Generic;
using NetGL.Core.Mathematics;

namespace NetGL.Core.Types {
    public sealed unsafe class Vector4Buffer : AttributeBuffer<Vector4> {
        private const int _bytesInStruct = 16;

        private readonly Vector4* _arrPtr;

        internal protected override int BytesInStruct {
            get { return _bytesInStruct; }
        }
        public override VertexAttribPointerType DataType {
            get { return VertexAttribPointerType.Float; }
        }
        internal protected override int StructInLine {
            get { return 4; }
        }

        public Vector4Buffer(uint length) : this((int)length) { }
        public Vector4Buffer(int length)
            : base(length * _bytesInStruct) {
            _arrPtr = (Vector4*)Pointer.ToPointer();
        }
        public Vector4Buffer(Vector4[] array)
            : this(array.Length) {
            for (int i = 0; i < array.Length; i++)
                this[i] = array[i];
        }
        public Vector4Buffer(IList<Vector4> array)
            : this(array.Count) {
            for (int i = 0; i < array.Count; i++)
                this[i] = array[i];
        }
        public Vector4Buffer(byte[] bytes)
            : base(bytes) {
            _arrPtr = (Vector4*)Pointer.ToPointer();
        }

        public override Vector4 this[int index] {
            get {
                CheckIndex(index);
                return _arrPtr[index];
            }
            set {
                CheckIndex(index);
                _arrPtr[index] = value;
            }
        }

        public static implicit operator Vector4Buffer(Vector4[] array) {
            return new Vector4Buffer(array);
        }
        public static implicit operator Vector4Buffer(List<Vector4> array) {
            return new Vector4Buffer(array);
        }

    }
}
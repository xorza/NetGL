using System.Collections.Generic;
using NetGL.Core.Mathematics;
using System.Collections;
using System.IO;

namespace NetGL.Core.Types {
    public sealed unsafe class Vector2Buffer : AttributeBuffer<Vector2> {
        private const int _bytesInStruct = 8;

        private readonly Vector2* _arrPtr;

        internal protected override int BytesInStruct {
            get { return _bytesInStruct; }
        }

        public override VertexAttribPointerType DataType {
            get { return VertexAttribPointerType.Float; }
        }

        internal protected override int StructInLine {
            get { return 2; }
        }

        public Vector2Buffer(int length)
            : base(length * _bytesInStruct) {
            _arrPtr = (Vector2*)Pointer.ToPointer();
        }

        public Vector2Buffer(byte[] bytes)
            : base(bytes) {
            _arrPtr = (Vector2*)Pointer.ToPointer();
        }

        public Vector2Buffer(Vector2[] array)
            : this(array.Length) {
            for (int i = 0; i < array.Length; i++)
                this[i] = array[i];
        }
        public Vector2Buffer(List<Vector2> array)
            : this(array.Count) {
            for (int i = 0; i < array.Count; i++)
                this[i] = array[i];
        }

        public Vector2Buffer(BinaryReader reader) : base(reader) { }
        public Vector2Buffer(Stream stream) : base(stream) { }

        public static implicit operator Vector2Buffer(Vector2[] array) {
            return new Vector2Buffer(array);
        }
        public static implicit operator Vector2Buffer(List<Vector2> array) {
            return new Vector2Buffer(array);
        }

        public override Vector2 this[int index] {
            get {
                CheckIndex(index);
                return _arrPtr[index];
            }
            set {
                CheckIndex(index);
                _arrPtr[index] = value;
            }
        }
    }
}
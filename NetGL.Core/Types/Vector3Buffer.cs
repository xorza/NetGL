using System.Collections;
using System.Collections.Generic;
using NetGL.Core.Mathematics;
using System.IO;
using System.Linq;

namespace NetGL.Core.Types {
    public sealed unsafe class Vector3Buffer : AttributeBuffer<Vector3> {
        private const int _bytesInStruct = 12;

        private readonly Vector3* _arrPtr;

        public override VertexAttribPointerType DataType {
            get { return VertexAttribPointerType.Float; }
        }

        internal protected override int BytesInStruct {
            get { return _bytesInStruct; }
        }
        internal protected override int StructInLine {
            get { return 3; }
        }

        public override Vector3 this[int index] {
            get {
                CheckIndex(index);
                return _arrPtr[index];
            }
            set {
                CheckIndex(index);
                _arrPtr[index] = value;
            }
        }

        public Vector3Buffer(byte[] bytes)
            : base(bytes) {
            _arrPtr = (Vector3*)Pointer.ToPointer();
        }
        public Vector3Buffer(int length)
            : base(length * _bytesInStruct) {
            _arrPtr = (Vector3*)Pointer.ToPointer();
        }
        public Vector3Buffer(uint length) : this((int)length) { }
        public Vector3Buffer(int count, Vector3 values)
            : this(count) {

            for (int i = 0; i < count; i++)
                this[i] = values;
        }
        public Vector3Buffer(IList<Vector3> array)
            : this(array.Count) {
            for (int i = 0; i < array.Count; i++)
                this[i] = array[i];
        }
        public Vector3Buffer(IEnumerable<Vector3> array)
            : this(array.Count()) {
            using (var enumerator = array.GetEnumerator()) {
                for (int i = 0; i < Length; i++) {
                    this[i] = enumerator.Current;
                    enumerator.MoveNext();
                }
            }
        }

        public Vector3Buffer(BinaryReader reader) : base(reader) { }
        public Vector3Buffer(Stream stream) : base(stream) { }

        public static implicit operator Vector3Buffer(Vector3[] array) {
            return new Vector3Buffer(array);
        }
        public static implicit operator Vector3Buffer(List<Vector3> array) {
            return new Vector3Buffer(array);
        }
    }
}
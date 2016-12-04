using System;
using NetGL.Core.Infrastructure;
using NetGL.Core.Mathematics;

namespace NetGL.Core.Types {
    public class VertexArrayObject : UIntObject {
        private BufferObject _indexBuffer;
        private readonly BufferObject[] _attributeBuffers = new BufferObject[10];

        public VertexArrayObject()
            : base(CurrentContext.GenVertexArray()) {
        }

        public void Bind() {
            Context.BindVertexArray(this.Handle);
        }

        public void SetIndices(AttributeBuffer data) {
            if (data == null) {
                Disposer.Dispose(ref _indexBuffer);
                return;
            }

            Bind();
            if (_indexBuffer == null)
                _indexBuffer = new BufferObject(BufferObjectTypes.Index);

            _indexBuffer.BufferData(data);
        }

        public BufferObject SetAttributeData(StandardAttribute attribute, AttributeBuffer data, bool normalize) {
            var attrIndex = (uint)attribute;
            Bind();

            if (data == null) {
                Context.DisableVertexAttribArray(attrIndex);
                Disposer.Dispose(ref _attributeBuffers[attrIndex]);
                return null;
            }

            BufferObject buffer;
            if (_attributeBuffers[attrIndex] == null) {
                buffer = new BufferObject(BufferObjectTypes.Vertex);
                _attributeBuffers[attrIndex] = buffer;
            }
            else
                buffer = _attributeBuffers[attrIndex];

            Context.EnableVertexAttribArray((uint)attribute);
            buffer.BufferData(data);
            Context.VertexAttribPointer(attrIndex, data.StructInLine, data.DataType, normalize, 0, 0);

            return buffer;
        }

        public void SetMesh(Mesh mesh) {
            Bind();

            SetAttributeData(StandardAttribute.Position, mesh.Vertices, false);
            SetAttributeData(StandardAttribute.Color, mesh.Colors, false);
            SetAttributeData(StandardAttribute.Normal, mesh.Normals, true);
            SetAttributeData(StandardAttribute.TexCoord, mesh.TexCoords, false);
            SetAttributeData(StandardAttribute.Tangent, mesh.Tangents, true);

            if (mesh.Indices != null)
                SetIndices(mesh.Indices);
        }

        public void UpdateBuffers() {
            if (_indexBuffer != null)
                _indexBuffer.UpdateBuffer();
            for (int i = 0; i < _attributeBuffers.Length; i++)
                if (_attributeBuffers[i] != null)
                    _attributeBuffers[i].UpdateBuffer();
        }

        protected override void OnDispose(bool isDisposing) {
            if (isDisposing == false)
                return;

            for (int i = 0; i < _attributeBuffers.Length; i++)
                Disposer.Dispose(ref _attributeBuffers[i]);

            Disposer.Dispose(ref _indexBuffer);
        }

        internal override DisposeAction GetDisposeAction() {
            return Context.DeleteVertexArray;
        }
    }
}
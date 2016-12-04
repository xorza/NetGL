using System;

namespace NetGL.Core.Types {
    public enum BufferObjectTypes {
        Vertex,
        Index,
        Uniform,
        ShaderStorage
    }

    public class BufferObject : UIntObject {
        private readonly BindBufferTarget _bindTarget;
        private UnsafeBuffer _data;

        public BufferObjectTypes Type { get; private set; }

        public BufferObject(BufferObjectTypes type)
            : base() {

            this.Type = type;

            switch (type) {
                case BufferObjectTypes.Vertex:
                    _bindTarget = BindBufferTarget.ArrayBuffer;
                    break;
                case BufferObjectTypes.Index:
                    _bindTarget = BindBufferTarget.ElementArrayBuffer;
                    break;
                case BufferObjectTypes.Uniform:
                    _bindTarget = BindBufferTarget.UniformBuffer;
                    break;
                case BufferObjectTypes.ShaderStorage:
                    _bindTarget = BindBufferTarget.ShaderStorageBuffer;
                    break;
                default:
                    throw new NotSupportedException(type.ToString());
            }
            
            Initialize(Context.CreateBuffer());
        }

        private void Bind() {
            switch (_bindTarget) {
                case BindBufferTarget.ArrayBuffer:
                    Context.BindArrayBuffer(this.Handle);
                    return;
                case BindBufferTarget.ElementArrayBuffer:
                    Context.BindElementArrayBuffer(this.Handle);
                    return;
                case BindBufferTarget.UniformBuffer:
                    Context.BindUniformBuffer(this.Handle);
                    return;
                case BindBufferTarget.ShaderStorageBuffer:
                    Context.BindShaderStorageBuffer(Handle);
                    return;
                default:
                    throw new NotSupportedException(_bindTarget.ToString());
            }
        }

        public void BufferData(UnsafeBuffer data) {
            if (data == null)
                throw new ArgumentNullException("data");

            _data = data;

            Bind();
            Context.BufferData(_bindTarget, _data, _data.Usage);
        }
        public void UpdateBuffer() {
            if (_data == null)
                throw new ArgumentNullException("data");

            if (Context.DirectStateAccess)
                Context.BufferSubData(Handle, _data);
            else {
                Bind();
                Context.BufferSubData(_bindTarget, _data);
            }
        }
        public void ReadBuffer() {
            if (_data == null)
                throw new ArgumentNullException("data");

            Bind();
            Context.GetBufferSubData(_bindTarget, _data);
        }

        public void BindBufferBase(BindBufferTarget bindTarget, uint globalBindingIndex) {
            Context.BindBufferBase(bindTarget, globalBindingIndex, this.Handle);
        }

        internal override DisposeAction GetDisposeAction() {
            return Context.DeleteBuffer;
        }
    }
}
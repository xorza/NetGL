using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using NetGL.Core.Mathematics;

namespace NetGL.Core.Types {
    public unsafe class GlobalUniformsBufferObject : UIntObject {
        public const int MaxLightCount = 3;
        private const int BytesInLight = 16 * 4;
        private const int SizeInBytes = 32 * 4 + BytesInLight * MaxLightCount;

        private readonly IntPtr _pointer;
        private readonly List<Light> _lights = new List<Light>();

        public Vector3 Ambient { get; set; }
        public Vector3 CameraPosition { get; set; }
        public int LightCastingShadowNumber { get; set; }

        public GlobalUniformsBufferObject() :
            base() {
            LightCastingShadowNumber = -1;

            var handle = Context.CreateBuffer();

            try {
                Context.BindUniformBuffer(handle);
                Context.BufferData(BindBufferTarget.UniformBuffer, SizeInBytes, IntPtr.Zero, BufferUsage.DynamicDraw);
                Context.BindBufferBase(BindBufferTarget.UniformBuffer, ShaderProgram.GlobalUniformsBindingPoint, handle);
            }
            catch {
                Context.DeleteBuffer(handle);
                throw;
            }

            Initialize(handle);
            _pointer = Marshal.AllocHGlobal(SizeInBytes);
        }

        public void ClearLights() {
            _lights.Clear();
        }
        public void AddLight(Light l) {
            Assert.True(_lights.Count < MaxLightCount);

            _lights.Add(l);
        }
        public void Update() {
            Assert.True(_lights.Count <= MaxLightCount);

            var vec = (Vector4*)_pointer;
            var ivec = (IVector4*)_pointer;

            vec[0] = new Vector4(CameraPosition);
            vec[1] = new Vector4(Ambient);
            ivec[2] = new IVector4(_lights.Count, LightCastingShadowNumber, 0, 0);

            for (int i = 0; i < _lights.Count; i++) {
                var l = _lights[i];
                var p = _pointer + 48 + i * BytesInLight;

                vec = (Vector4*)(p);

                vec[0] = new Vector4(l.Position);
                vec[1] = new Vector4(-l.Direction);
                vec[2] = new Vector4(l.Diffuse);
                vec[3] = new Vector4(l.Attenuation);

                var type = (int*)(p + BytesInLight - 4);
                type[0] = (int)l.Type;
            }

            if (Context.DirectStateAccess)
                Context.BufferSubData(Handle, 0, SizeInBytes, _pointer);
            else {
                Context.BindUniformBuffer(Handle);
                Context.BufferSubData(BindBufferTarget.UniformBuffer, 0, SizeInBytes, _pointer);
            }
        }

        protected override void OnDispose(bool isDisposing) {
            Marshal.FreeHGlobal(_pointer);
        }

        internal override DisposeAction GetDisposeAction() {
            return Context.DeleteBuffer;
        }
    }
}
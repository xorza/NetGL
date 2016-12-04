using System;
using System.Runtime.InteropServices;
using NetGL.Core.Mathematics;

namespace NetGL.Core.Types {
    public class StandardUniformsBufferObject : UIntObject {
        private const int SizeInBytes = (4 + 16 + 16 + 16 + 16 + 12) * 4;

        private readonly IntPtr _pointer;
        private readonly Matrix _inversedModel = new Matrix();

        public StandardUniformsBufferObject() :
            base() {

            var handle = CurrentContext.CreateBuffer();

            try {
                Context.BindUniformBuffer(handle);
                Context.BufferData(BindBufferTarget.UniformBuffer, SizeInBytes, IntPtr.Zero, BufferUsage.StreamDraw);
                Context.BindBufferBase(BindBufferTarget.UniformBuffer, ShaderProgram.StandartUniformsBindingPoint, handle);
            }
            catch {
                Context.DeleteBuffer(handle);
                throw;
            }

            _pointer = Marshal.AllocHGlobal(SizeInBytes);
            Initialize(handle);
        }

        public unsafe void Update(Matrix modelViewProjection, Matrix modelView, Matrix model, Matrix normal, bool useInversedModel, float time, uint id) {
            var fPtr = (float*)_pointer;
            var uiPtr = (uint*)_pointer;

            fPtr[0] = time;
            uiPtr[1] = id;
            //2
            //3

            CopyMatrix4(fPtr, modelViewProjection, 4);
            var sizeInBytesToUpdate = 20 * 4;

            if (modelView != null) {
                CopyMatrix4(fPtr, modelView, 20);
                sizeInBytesToUpdate = 36 * 4;
            }

            if (model != null) {
                CopyMatrix4(fPtr, model, 36);
                sizeInBytesToUpdate = 52 * 4;
            }

            if (useInversedModel == true) {
                Matrix.Invert(model, _inversedModel);
                CopyMatrix4(fPtr, _inversedModel, 52);
                sizeInBytesToUpdate = 68 * 4;
            }

            if (normal != null) {
                CopyNormalMatrix(fPtr, normal, 68);
                sizeInBytesToUpdate = 80 * 4;
            }

            if (Context.DirectStateAccess)
                Context.BufferSubData(Handle, 0, sizeInBytesToUpdate, _pointer);
            else {
                Context.BindUniformBuffer(Handle);
                Context.BufferSubData(BindBufferTarget.UniformBuffer, 0, sizeInBytesToUpdate, _pointer);
            }
        }

        unsafe private static void CopyMatrix4(float* p, Matrix matrix, int offset) {
            p[offset + 0] = matrix.M11;
            p[offset + 1] = matrix.M12;
            p[offset + 2] = matrix.M13;
            p[offset + 3] = matrix.M14;

            p[offset + 4] = matrix.M21;
            p[offset + 5] = matrix.M22;
            p[offset + 6] = matrix.M23;
            p[offset + 7] = matrix.M24;

            p[offset + 8] = matrix.M31;
            p[offset + 9] = matrix.M32;
            p[offset + 10] = matrix.M33;
            p[offset + 11] = matrix.M34;

            p[offset + 12] = matrix.M41;
            p[offset + 13] = matrix.M42;
            p[offset + 14] = matrix.M43;
            p[offset + 15] = matrix.M44;
        }
        unsafe private static void CopyNormalMatrix(float* p, Matrix matrix, int offset) {
            p[offset + 0] = matrix.M11;
            p[offset + 1] = matrix.M21;
            p[offset + 2] = matrix.M31;

            p[offset + 4] = matrix.M12;
            p[offset + 5] = matrix.M22;
            p[offset + 6] = matrix.M32;

            p[offset + 8] = matrix.M13;
            p[offset + 9] = matrix.M23;
            p[offset + 10] = matrix.M33;
        }

        protected override void OnDispose(bool isDisposing) {
            Marshal.FreeHGlobal(_pointer);
        }
        internal override DisposeAction GetDisposeAction() {
            return Context.DeleteBuffer;
        }
    }
}

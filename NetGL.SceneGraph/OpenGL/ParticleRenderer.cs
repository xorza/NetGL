using NetGL.Core;
using NetGL.Core.Infrastructure;
using NetGL.Core.Mathematics;
using NetGL.Core.Types;
using NetGL.SceneGraph.Components;
using NetGL.SceneGraph.Scene;
using System;
using System.Runtime.InteropServices;

namespace NetGL.SceneGraph.OpenGL {
    internal class ParticleRenderer : IRenderer, IDisposable {
        private bool _isDisposed = false;

        private readonly GL _glContext;
        private readonly VertexArrayObject _vertexBufferArray;

        private readonly Vector3Buffer _particlePositions;
        private readonly Vector4Buffer _particleColors, _particleData1, _particleData2, _particleData3;
        private readonly DataBuffer _computeData;
        private readonly BufferObject _positionsBuffer, _colorsBuffer, _dataBuffer1, _dataBuffer2, _dataBuffer3, _computeDataBuffer;

        private readonly SceneTime _time;
        private readonly Sphere _boundSphere;

        private float _prevTime;
        private uint _particleCount;

        private readonly ShaderProgram _computeShader;

        public uint MaxParticles { get; private set; }
        public Sphere BoundSphere { get { return _boundSphere; } }

        public ParticleRenderer(SceneTime time) {
            if (time == null)
                throw new ArgumentNullException("time");

            MaxParticles = 1000;
            _particleCount = MaxParticles;

            _time = time;
            _glContext = GL.GetCurrent(true);

            _particlePositions = new Vector3Buffer(MaxParticles);
            _particleColors = new Vector4Buffer(MaxParticles);
            _particleData1 = new Vector4Buffer(MaxParticles);
            _particleData2 = new Vector4Buffer(MaxParticles);
            _particleData3 = new Vector4Buffer(MaxParticles);
            _computeData = new DataBuffer();
            _computeData.Usage = BufferUsage.DynamicDraw;

            InitBufferValues();

            _vertexBufferArray = new VertexArrayObject();
            _positionsBuffer = _vertexBufferArray.SetAttributeData(StandardAttribute.Position, _particlePositions, false);
            _colorsBuffer = _vertexBufferArray.SetAttributeData(StandardAttribute.Color, _particleColors, false);
            _dataBuffer1 = _vertexBufferArray.SetAttributeData(StandardAttribute.Data1, _particleData1, false);
            _dataBuffer2 = _vertexBufferArray.SetAttributeData(StandardAttribute.Data2, _particleData2, false);
            _dataBuffer3 = _vertexBufferArray.SetAttributeData(StandardAttribute.Data3, _particleData3, false);
            _computeDataBuffer = new BufferObject(BufferObjectTypes.ShaderStorage);

            _computeShader = ShaderProgram.CreateCompute(Resources.Particles_comp);
            _computeDataBuffer.BufferData(_computeData);

            _prevTime = _time.CurrentFloat;
            _boundSphere = new Sphere(Vector3.Zero, 0.5f);
        }

        private void InitBufferValues() {
            for (int i = 0; i < MaxParticles; i++) {
                _particlePositions[i] = RandomF.InsideUnitSphere();
                var color = RandomF.InsideUnitCube();
                var maxColor = MathF.Max(color.X, color.Y, color.Z);
                color /= maxColor;
                _particleColors[i] = new Vector4(color, 1);
                _particleData1[i] = new Vector4(50, 0, 0, 0);
                _particleData2[i] = new Vector4(0);
                _particleData3[i] = new Vector4(0);
            }

            var data = _computeData.Data;
            data.CurrentParticles = MaxParticles;
            data.MaxParticles = MaxParticles;
            data.Time = 0;
            data.DeltaTime = 0;
            _computeData.Data = data;
        }
        public void Prerender() {
            if (_isDisposed)
                throw new ObjectDisposedException("OpenGLMeshRenderer");

            var data = _computeData.Data;
            data.CurrentParticles = MaxParticles;
            data.MaxParticles = MaxParticles;
            data.Time = _time.CurrentFloat;
            data.DeltaTime = _time.CurrentFloat - _prevTime;
            _computeData.Data = data;
            _computeDataBuffer.UpdateBuffer();

            _prevTime = _time.CurrentFloat;

            _computeDataBuffer.BindBufferBase(BindBufferTarget.ShaderStorageBuffer, 0);
            _positionsBuffer.BindBufferBase(BindBufferTarget.ShaderStorageBuffer, 1);
            _colorsBuffer.BindBufferBase(BindBufferTarget.ShaderStorageBuffer, 2);
            _dataBuffer1.BindBufferBase(BindBufferTarget.ShaderStorageBuffer, 3);
            _dataBuffer2.BindBufferBase(BindBufferTarget.ShaderStorageBuffer, 4);
            _dataBuffer3.BindBufferBase(BindBufferTarget.ShaderStorageBuffer, 5);

            _computeShader.Bind();

            _glContext.DispatchCompute((uint)(MaxParticles / _computeShader.WorkGroupSizeX), 1, 1);
            _glContext.MemoryBarrier(MemoryBarrier.ShaderStorageBarrierBit | MemoryBarrier.VertexAttribArrayBarrierBit);
        }
        public void Render() {
            if (_isDisposed)
                throw new ObjectDisposedException("OpenGLMeshRenderer");

            if (_particleCount <= 0)
                return;

            _vertexBufferArray.Bind();
            _glContext.DrawArrays(PrimitiveType.Points, 0u, _particleCount);
        }

        public void Dispose() {
            if (_isDisposed)
                return;
            _isDisposed = true;

            Disposer.Dispose(_vertexBufferArray);
            Disposer.Dispose(_particlePositions);
            Disposer.Dispose(_particleColors);
            Disposer.Dispose(_particleData1);
            Disposer.Dispose(_particleData2);
            Disposer.Dispose(_particleData3);
            Disposer.Dispose(_computeShader);
            Disposer.Dispose(_computeDataBuffer);
        }

        internal struct ComputeData {
            public uint MaxParticles;
            public uint CurrentParticles;
            public float Time;
            public float DeltaTime;
        }
        internal unsafe class DataBuffer : UnsafeBuffer {
            private readonly ComputeData* _data;

            public ComputeData Data {
                get {
                    return _data[0];
                }
                set {
                    _data[0] = value;
                }
            }

            public DataBuffer()
                : base(Marshal.SizeOf<ComputeData>()) {
                _data = (ComputeData*)Pointer.ToPointer();
            }
        }
    }
}
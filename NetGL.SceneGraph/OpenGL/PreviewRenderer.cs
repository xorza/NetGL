using System.Drawing;
using NetGL.Core;
using NetGL.Core.Infrastructure;
using NetGL.Core.Mathematics;
using NetGL.Core.Shaders;
using NetGL.Core.Types;
using NetGL.SceneGraph.Components;
using Sphere = NetGL.Core.Meshes.Sphere;
using System;

namespace NetGL.SceneGraph.OpenGL {
    public class PreviewRenderer {
        private readonly Graphics _graphics;
        private readonly GL _glContext;

        private readonly Size _size;
        private readonly Matrix _mvpMatrix = Matrix.Identity;
        private readonly Matrix _normalMatrix = Matrix.Identity;
        private readonly Matrix _modelMatrix = Matrix.Identity;
        private readonly Light[] _lights = new Light[2];
        private readonly MeshRenderer _renderer;
        private readonly Material _defaultMaterial;
        private readonly Mesh _defaultMesh = Sphere.Create(0.45f, 40, 40);
        private readonly Framebuffer _frameBuffer;

        public Vector4 BackgroundColor { get; set; }

        public PreviewRenderer(Size size) {
            _graphics = Graphics.GetCurrent(true);
            _glContext = _graphics.GLContext;
            _size = size;

            _renderer = new MeshRenderer();

            _frameBuffer = new Framebuffer(_size, _glContext.MaxSamples);
            _frameBuffer.AddColorRenderbuffer(FramebufferAttachment.ColorAttachment0);
            _frameBuffer.AddDepthRenderbuffer();
            _frameBuffer.CheckStatus(true);

            _lights[0] = new Light() { Type = LightType.Directional, Position = Vector3.One, Diffuse = Vector3.One, Direction = -Vector3.One.Normalized };
            _lights[1] = new Light() { Type = LightType.Directional, Position = -Vector3.One, Diffuse = Vector3.One, Direction = Vector3.One.Normalized };

            _defaultMaterial = new Material(MaterialType.DiffuseColor, ShadingTechnique.Forward, RenderQueue.Opaque);

            BackgroundColor = new Vector4(0);
        }
        public Bitmap RenderPreview(Mesh mesh, Quaternion rotation) {
            return RenderPreview(_defaultMaterial, mesh, rotation);
        }
        public Bitmap RenderPreview(Mesh mesh) {
            return RenderPreview(_defaultMaterial, mesh, Quaternion.Identity);
        }
        public Bitmap RenderPreview(Material material) {
            return RenderPreview(material, _defaultMesh, Quaternion.Identity);
        }
        public Bitmap RenderPreview(Material material, Mesh mesh, Quaternion rotation) {
            var shader = material.Shader;

            _modelMatrix.LoadIdentity();
            _modelMatrix.Transform(-mesh.Bounds.Center, rotation, new Vector3(0.45f / mesh.Bounds.Radius));

            if (shader.UsesNormalMatrix)
                Matrix.Invert(_modelMatrix, _normalMatrix);

            _mvpMatrix.LoadOrthographicAspect(1, _size.Width / (float)_size.Height, -1, 1);
            _mvpMatrix.MultiplyFromLeft(_modelMatrix);

            _graphics.StandartUniforms.Update(_mvpMatrix, _modelMatrix, _modelMatrix, _normalMatrix, material.Shader.UsesInversedModelMatrix, 0, 0);

            _graphics.GlobalUniforms.ClearLights();
            _graphics.GlobalUniforms.LightCastingShadowNumber = -1;
            _graphics.GlobalUniforms.Ambient = new Vector3(0.05f);
            _graphics.GlobalUniforms.AddLight(_lights[0]);
            _graphics.GlobalUniforms.AddLight(_lights[1]);
            _graphics.GlobalUniforms.Update();

            _renderer.Mesh = mesh;

            _frameBuffer.Bind();

            _glContext.Viewport(0u, 0u, (uint)_size.Width, (uint)_size.Height);
            _glContext.Enable(EnableCap.Multisample);
            _glContext.DrawBuffer(FramebufferAttachment.ColorAttachment0);
            _glContext.ClearColor(BackgroundColor);
            _glContext.Clear(BufferMask.ColorBufferBit | BufferMask.DepthBufferBit);

            switch (material.RenderQueue) {
                case RenderQueue.Opaque:
                    _glContext.Disable(EnableCap.Blend);
                    break;
                case RenderQueue.Transparent:
                    _glContext.Enable(EnableCap.Blend);
                    break;
                default:
                    throw new NotSupportedException();
            }

            _graphics.BindMaterial(material);

            _renderer.Render();

            var img = _frameBuffer.ReadColorImage(FramebufferAttachment.ColorAttachment0);
            return img;
        }

        public void Dispose() {
            Disposer.Dispose(_frameBuffer);
            Disposer.Dispose(_renderer);
        }
    }
}

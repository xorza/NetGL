using NetGL.Core;
using NetGL.Core.Infrastructure;
using NetGL.Core.Mathematics;
using NetGL.Core.Types;
using NetGL.SceneGraph.Components;
using NetGL.SceneGraph.Scene;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;

namespace NetGL.SceneGraph.OpenGL {
    internal class DeferredRender : IDisposable, IRenderTechnique {
        private Size _screenSize;

        private readonly GL _glContext;
        private readonly Graphics _graphics;
        private Framebuffer _gbuffer;
        private readonly Material _secondPassMaterial;

        public bool IsDisposed { get; private set; }

        public Size ScreenSize {
            get { return _screenSize; }
            set {
                if (_screenSize == value)
                    return;
                _screenSize = value;
                ChangeScreenSize();
            }
        }

        public DeferredRender() {
            _glContext = GL.GetCurrent(true);
            _graphics = Graphics.GetCurrent(true);

            var shader = new ShaderProgram();
            shader.Code = Resources.Deferred_2pass;
            _secondPassMaterial = new Material(shader, RenderQueue.CustomShaderOpaque);

            ScreenSize = _graphics.ScreenSize;
        }

        private void ChangeScreenSize() {
            Disposer.Dispose(ref _gbuffer);

            _gbuffer = new Framebuffer(_screenSize);
            var albedoTexture = _gbuffer.AddColorTexture(FramebufferAttachment.ColorAttachment0, PixelInternalFormat.Rgba8, PixelFormat.Rgba);        //albedo - xyz, spec intensity - w
            var emissionTexture = _gbuffer.AddColorTexture(FramebufferAttachment.ColorAttachment1, PixelInternalFormat.Rgb8, PixelFormat.Rgb);      //emission - xyz, spec power - w
            var positionTexture = _gbuffer.AddColorTexture(FramebufferAttachment.ColorAttachment2, PixelInternalFormat.Rgb16f, PixelFormat.Rgba);     //view position
            var normalTexture = _gbuffer.AddColorTexture(FramebufferAttachment.ColorAttachment3, PixelInternalFormat.Rgba16f, PixelFormat.Rgba);       //normal
            var dataTexture = _gbuffer.AddColorTexture(FramebufferAttachment.ColorAttachment4, PixelInternalFormat.R16ui, PixelFormat.Red);          //material id          

            if (_graphics.Backbuffer.DepthAttachment is Renderbuffer)
                _gbuffer.AttachRenderbuffer(FramebufferAttachment.DepthAttachment, (Renderbuffer)_graphics.Backbuffer.DepthAttachment);
            else
                _gbuffer.AttachTexture(FramebufferAttachment.DepthAttachment, (Texture2)_graphics.Backbuffer.DepthAttachment);

            _secondPassMaterial.SetTexture("uniform_AlbedoTexture", albedoTexture);
            _secondPassMaterial.SetTexture("uniform_EmissionTexture", emissionTexture);
            _secondPassMaterial.SetTexture("uniform_NormalTexture", normalTexture);
            _secondPassMaterial.SetTexture("uniform_PositionTexture", positionTexture);
            _secondPassMaterial.SetTexture("uniform_DataTexture", dataTexture);

            //Texture2 depthTexture = null;
            //if (depthTexture != null)
            //    _secondPassMaterial.SetTexture("uniform_DepthTexture", depthTexture);
        }

        public void RenderAll(IReadOnlyList<Camera> cameras, IReadOnlyList<LightSource> lights, RendererCollection renderables) {
            Assert.True(cameras.Count > 0);

            _glContext.Viewport(0u, 0u, (uint)_screenSize.Width, (uint)_screenSize.Height);
            _graphics.DrawCalls = 0;

            for (int camNo = 0; camNo < cameras.Count; camNo++) {
                var camera = cameras[camNo];
                if (camera.IsEnabled == false)
                    continue;

                _graphics.SetupCameraAndLights(camera, lights);

                FirstPass(renderables, camera);
                SecondPass(camNo == 0, camera);

                _glContext.DepthMask(false);
                _glContext.Enable(EnableCap.Blend);
                var opaqueCount = renderables.Opaque.Count;
                var transparent = renderables.Transparent;
                for (int i = 0; i < transparent.Count; i++)
                    _graphics.UpdateAndRender(camera, transparent[i], (uint)(i + 1 + opaqueCount));
                _glContext.DepthMask(true);
            }

            DrawGBuffer();
        }

        private void FirstPass(RendererCollection renderables, Camera camera) {
            _gbuffer.Bind();
            _glContext.DrawBuffers(
                FramebufferAttachment.ColorAttachment0,
                FramebufferAttachment.ColorAttachment1,
                FramebufferAttachment.ColorAttachment2,
                FramebufferAttachment.ColorAttachment3,
                FramebufferAttachment.ColorAttachment4);

            _glContext.ClearBuffer(ClearBuffer.Depth, 0, 1f);
            _glContext.ClearBuffer(ClearBuffer.Color, 0, Vector3.Zero);
            _glContext.ClearBuffer(ClearBuffer.Color, 1, Vector3.Zero);
            _glContext.ClearBuffer(ClearBuffer.Color, 2, Vector3.Zero);
            _glContext.ClearBuffer(ClearBuffer.Color, 3, UIVector4.Zero);
            _glContext.ClearBuffer(ClearBuffer.Color, 4, Vector3.Zero);

            _glContext.DepthMask(true);
            _glContext.Disable(EnableCap.Blend);

            var opaque = renderables.Opaque;
            for (int j = 0; j < opaque.Count; j++)
                _graphics.UpdateAndRender(camera, opaque[j], (uint)(j + 1));
        }
        private void SecondPass(bool firstCamera, Camera camera) {
            if (firstCamera)
                _secondPassMaterial.Color = new Vector4(camera.ClearColor, 1);
            else {
                _glContext.Enable(EnableCap.Blend);
                _glContext.BlendFunc(BlendMode.SrcAlpha, BlendMode.OneMinusSrcAlpha);
                _secondPassMaterial.Color = Vector4.Zero;
            }
            _graphics.Backbuffer.Bind(FramebufferTarget.Framebuffer);
            _glContext.DrawBuffer(FramebufferAttachment.ColorAttachment0);
            _graphics.Blit(_secondPassMaterial);
            _glContext.Disable(EnableCap.Blend);
        }
        [Conditional("GL_DEBUG")]
        private void DrawGBuffer() {
            var height = 200;
            var width = (height * _screenSize.Width) / _screenSize.Height;

            _gbuffer.Bind(FramebufferTarget.ReadFramebuffer);
            _graphics.Backbuffer.Bind(FramebufferTarget.DrawFramebuffer);
            _glContext.ReadBuffer(FramebufferAttachment.ColorAttachment0);
            _glContext.BlitFramebuffer(0, 0, _screenSize.Width, _screenSize.Height, 0, 0, width, height, BufferMask.ColorBufferBit, BlitFramebufferFilter.Linear);

            _glContext.ReadBuffer(FramebufferAttachment.ColorAttachment1);
            _glContext.BlitFramebuffer(0, 0, _screenSize.Width, _screenSize.Height, width + 10, 0, width * 2 + 10, height, BufferMask.ColorBufferBit, BlitFramebufferFilter.Linear);

            _glContext.ReadBuffer(FramebufferAttachment.ColorAttachment2);
            _glContext.BlitFramebuffer(0, 0, _screenSize.Width, _screenSize.Height, (width + 10) * 2, 0, width * 3 + 20, height, BufferMask.ColorBufferBit, BlitFramebufferFilter.Linear);

            _glContext.ReadBuffer(FramebufferAttachment.ColorAttachment3);
            _glContext.BlitFramebuffer(0, 0, _screenSize.Width, _screenSize.Height, (width + 10) * 3, 0, width * 4 + 30, height, BufferMask.ColorBufferBit, BlitFramebufferFilter.Linear);
        }

        public void Dispose() {
            if (IsDisposed)
                return;
            IsDisposed = true;

            Disposer.Dispose(ref _gbuffer);
            Disposer.Dispose(_secondPassMaterial);
        }
    }
}
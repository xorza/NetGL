using NetGL.Core;
using NetGL.SceneGraph.Components;
using NetGL.SceneGraph.Scene;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetGL.SceneGraph.OpenGL {
    internal class ForwardRender : IRenderTechnique {
        private Size _screenSize;

        private readonly GL _glContext;
        private readonly Graphics _graphics;

        public Size ScreenSize {
            get { return _screenSize; }
            set {
                if (_screenSize == value)
                    return;
                _screenSize = value;
            }
        }

        public ForwardRender() {
            _glContext = GL.GetCurrent(true);
            _graphics = Graphics.GetCurrent(true);

            ScreenSize = _graphics.ScreenSize;
        }

        public void RenderAll(IReadOnlyList<Camera> cameras, IReadOnlyList<LightSource> lights, RendererCollection renderables) {

            Prepare();

            for (int i = 0; i < cameras.Count; i++) {
                var camera = cameras[i];
                if (camera.IsEnabled == false)
                    continue;

                if (i == 0) {
                    _glContext.ClearColor(camera.ClearColor);
                    _glContext.Clear(BufferMask.ColorBufferBit | BufferMask.DepthBufferBit | BufferMask.StencilBufferBit);
                }
                else
                    _glContext.Clear(BufferMask.DepthBufferBit);

                _graphics.SetupCameraAndLights(camera, lights);
                RenderCamera(camera, renderables);
            }
        }

        private void Prepare() {
            _graphics.DrawCalls = 0;
            _glContext.Viewport(0u, 0u, (uint)_screenSize.Width, (uint)_screenSize.Height);

            _graphics.Backbuffer.Bind();
            _glContext.DrawBuffer(FramebufferAttachment.ColorAttachment0);
            if (_graphics.Backbuffer.Samples > 1)
                _glContext.Enable(EnableCap.Multisample);
        }
        private void RenderCamera(Camera camera, RendererCollection renderables) {
            renderables.OpenGLSort(camera);

            _glContext.DepthMask(true);
            _glContext.Disable(EnableCap.Blend);
            var opaque = renderables.Opaque;
            for (int i = 0; i < opaque.Count; i++)
                _graphics.UpdateAndRender(camera, opaque[i], (uint)(i + 1));

            _glContext.DepthMask(false);
            _glContext.Enable(EnableCap.Blend);
            var transparent = renderables.Transparent;
            for (int i = 0; i < transparent.Count; i++)
                _graphics.UpdateAndRender(camera, transparent[i], (uint)(i + 1 + opaque.Count));
            _glContext.DepthMask(true);
        }
    }
}

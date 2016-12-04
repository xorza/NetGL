using NetGL.Core;
using NetGL.Core.Mathematics;
using NetGL.Core.Types;
using NetGL.SceneGraph.Shaders;
using System.Drawing;

namespace NetGL.SceneGraph.OpenGL {
    internal class UIRender {
        private Framebuffer _framebuffer;
        private Size _screenSize;
        private Renderbuffer _renderBuffer;
        private Texture2 _idTexture;

        private readonly UIShader _shader;
        private readonly GL _context;

        public Size ScreenSize {
            get { return _screenSize; }
        }
        
        public UIRender(Renderbuffer renderBuffer) {
            this._screenSize = renderBuffer.Size;

            _context = GL.GetCurrent(true);
            _shader = new UIShader();
            _renderBuffer = renderBuffer;

            _framebuffer = new Framebuffer(_screenSize);

             _framebuffer.AttachRenderbuffer(FramebufferAttachment.ColorAttachment0, renderBuffer);
            _idTexture = _framebuffer.AddColorTexture(Core.FramebufferAttachment.ColorAttachment1, PixelInternalFormat.R32ui, PixelFormat.Red);

            _framebuffer.CheckStatus(true);
        }

        internal void Render() {
            _framebuffer.Bind();
            _context.ClearBuffer(ClearBuffer.Color, 1, 0u);

            _context.Viewport(0, 0, _screenSize.Width, _screenSize.Height);
            _context.Enable(EnableCap.Blend);
            _context.BlendFunc(BlendMode.SrcAlpha, BlendMode.OneMinusSrcAlpha);
            _context.Disable(EnableCap.DepthTest);
            _context.Disable(EnableCap.Multisample);
            _context.DepthMask(false);
            _context.Disable(EnableCap.CullFace);

            _shader.Bind();
            _shader.Color = new Vector4(1.0f, 0f, 0f, 0.4f);
            _shader.Position = new Vector2(10, 10);
            _shader.Size = new Vector2(140, 140);
            _shader.ViewportSize = new Vector2(_screenSize);
            _shader.ID = 1;

            _context.DrawArrays(PrimitiveType.TriangleStrip, 0, 4);

            _context.Disable(EnableCap.Blend);
            _context.Enable(EnableCap.DepthTest);
            _context.DepthMask(true);
        }
    }
}

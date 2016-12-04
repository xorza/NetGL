using System;
using System.Drawing;
using NetGL.Core.Infrastructure;
using NetGL.Core.Mathematics;

namespace NetGL.Core.Types {
    public sealed class Framebuffer : UIntObject, IHaveSize {
        private readonly int _samples;

        private UIntObject _depthAttachment;
        private readonly UIntObject[] _colorAttachments;

        public Size Size { get; private set; }
        public int Samples { get { return _samples; } }
        public UIntObject DepthAttachment { get { return _depthAttachment; } }

        public Framebuffer(Size size) : this(size, 1) { }
        public Framebuffer(Size size, int samples)
            : base() {
            if (size.Width > Context.MaxTextureSize || size.Height > Context.MaxTextureSize)
                throw new ArgumentOutOfRangeException("size");
            if (MathF.InRangeInclusive(samples, 1, Context.MaxSamples) == false)
                throw new ArgumentOutOfRangeException("samples");

            Initialize(Context.CreateFramebuffer());

            _colorAttachments = new UIntObject[Context.FramebufferMaxColorAttachments];

            _samples = samples;
            this.Size = size;
        }

        public void Bind() {
            Context.BindFramebuffer(FramebufferTarget.Framebuffer, Handle);
        }
        public void Bind(FramebufferTarget target) {
            Context.BindFramebuffer(target, Handle);
        }

        public Texture2 AddColorTexture(FramebufferAttachment attachment, PixelInternalFormat pixelInternalFormat = PixelInternalFormat.Rgba8, PixelFormat pixelFormat = PixelFormat.Rgba) {
            Texture2 colorTexture;

            if (_samples == 1) {
                colorTexture = new Texture2();
                colorTexture.SetImage(Size, pixelInternalFormat, pixelFormat);
            }
            else {
                colorTexture = new Texture2(true);
                colorTexture.SetMultisampleImage(Samples, pixelInternalFormat, Size);
            }

            AttachTexture(attachment, colorTexture);
            return colorTexture;
        }
        public Renderbuffer AddColorRenderbuffer(FramebufferAttachment attachment) {
            var colorRenderBuffer = new Renderbuffer();
            if (_samples == 1)
                colorRenderBuffer.Storage(RenderbufferStorage.Rgba8, Size);
            else
                colorRenderBuffer.StorageMultisample(Samples, RenderbufferStorage.Rgba8, Size);
            AttachRenderbuffer(attachment, colorRenderBuffer);

            return colorRenderBuffer;
        }

        public Texture2 AddDepthTexture() {
            Texture2 depthTexture;
            if (_samples == 1) {
                depthTexture = new Texture2();
                depthTexture.SetImage(Size, PixelInternalFormat.DepthComponent32, PixelFormat.DepthComponent);
            }
            else {
                depthTexture = new Texture2();
                depthTexture.SetMultisampleImage(Samples, PixelInternalFormat.DepthComponent32, Size);
            }
            AttachTexture(FramebufferAttachment.DepthAttachment, depthTexture);

            return depthTexture;
        }
        public void AddDepthRenderbuffer() {
            var depthRenderBuffer = new Renderbuffer();
            if (_samples == 1)
                depthRenderBuffer.Storage(RenderbufferStorage.DepthComponent32, Size);
            else
                depthRenderBuffer.StorageMultisample(Samples, RenderbufferStorage.DepthComponent32, Size);
            AttachRenderbuffer(FramebufferAttachment.DepthAttachment, depthRenderBuffer);
        }

        public void AttachTexture(FramebufferAttachment attachment, Texture2 texture) {
            Assert.True(texture.Size == Size);

            texture.Anisotropy = 1;
            texture.MagFilter = TextureFilter.Nearest;
            texture.MinFilter = TextureFilter.Nearest;
            texture.Bind();

            Bind();
            if (Samples == 1)
                Context.FramebufferTexture2D(FramebufferTarget.Framebuffer, attachment, TextureTarget.Texture2D, texture.Handle);
            else
                Context.FramebufferTexture2D(FramebufferTarget.Framebuffer, attachment, TextureTarget.Texture2DMultisample, texture.Handle);

            SaveAttachment(attachment, texture);
        }
        public void AttachRenderbuffer(FramebufferAttachment attachment, Renderbuffer renderBuffer) {
            Assert.True(renderBuffer.Size == Size);

            Bind();
            Context.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, attachment, renderBuffer.Handle);

            SaveAttachment(attachment, renderBuffer);
        }

        public FramebufferStatus CheckStatus(bool fireException) {
            var status = Context.CheckFramebufferStatus(FramebufferTarget.Framebuffer);
            if (status != FramebufferStatus.Complete && fireException == true)
                throw new GLException(status.ToString());
            return status;
        }

        public Bitmap ReadColorImage(FramebufferAttachment attachment) {
            var index = attachment - FramebufferAttachment.ColorAttachment0;
            var texture = _colorAttachments[index] as Texture2;

            if (_samples == 1 && texture != null)
                return texture.GetImage();

            using (var tempfb = new Framebuffer(Size, 1)) {
                tempfb.AddColorTexture(FramebufferAttachment.ColorAttachment0);

                this.Bind(FramebufferTarget.ReadFramebuffer);
                tempfb.Bind(FramebufferTarget.DrawFramebuffer);
                Context.ReadBuffer(attachment);
                Context.DrawBuffer(FramebufferAttachment.ColorAttachment0);

                Context.BlitFramebuffer(0, 0, Size.Width, Size.Height, 0, 0, Size.Width, Size.Height, BufferMask.ColorBufferBit, BlitFramebufferFilter.Nearest);

                return tempfb.ReadColorImage(FramebufferAttachment.ColorAttachment0);
            }
        }
        public Bitmap ReadDepthImage() {
            var texture = _depthAttachment as Texture2;

            if (_samples == 1 && texture != null)
                return texture.GetImage();

            using (var tempfb = new Framebuffer(Size, 1)) {
                tempfb.AddDepthTexture();

                this.Bind(FramebufferTarget.ReadFramebuffer);
                tempfb.Bind(FramebufferTarget.DrawFramebuffer);

                Context.BlitFramebuffer(0, 0, Size.Width, Size.Height, 0, 0, Size.Width, Size.Height, BufferMask.DepthBufferBit, BlitFramebufferFilter.Nearest);

                return tempfb.ReadDepthImage();
            }
        }

        private void SaveAttachment(FramebufferAttachment attachmentPosition, UIntObject attachment) {
            switch (attachmentPosition) {
                case FramebufferAttachment.DepthAttachment:
                    Disposer.Dispose(_depthAttachment);
                    _depthAttachment = attachment;
                    break;

                case FramebufferAttachment.ColorAttachment0:
                case FramebufferAttachment.ColorAttachment1:
                case FramebufferAttachment.ColorAttachment2:
                case FramebufferAttachment.ColorAttachment3:
                case FramebufferAttachment.ColorAttachment4:
                case FramebufferAttachment.ColorAttachment5:
                case FramebufferAttachment.ColorAttachment6:
                case FramebufferAttachment.ColorAttachment7:
                case FramebufferAttachment.ColorAttachment8:
                case FramebufferAttachment.ColorAttachment9:
                case FramebufferAttachment.ColorAttachment10:
                case FramebufferAttachment.ColorAttachment11:
                case FramebufferAttachment.ColorAttachment12:
                case FramebufferAttachment.ColorAttachment13:
                case FramebufferAttachment.ColorAttachment14:
                case FramebufferAttachment.ColorAttachment15:
                    var index = attachmentPosition - FramebufferAttachment.ColorAttachment0;
                    Disposer.Dispose(_colorAttachments[index]);
                    _colorAttachments[index] = attachment;

                    break;
                case FramebufferAttachment.StencilAttachement:
                    throw new NotSupportedException(attachmentPosition.ToString());

                default:
                    throw new NotSupportedException(attachmentPosition.ToString());
            }
        }

        protected override void OnDispose(bool isDisposing) {
            if (isDisposing == false)
                return;

            Disposer.Dispose(ref _depthAttachment);
            for (int i = 0; i < _colorAttachments.Length; i++)
                Disposer.Dispose(ref _colorAttachments[i]);
        }

        internal override DisposeAction GetDisposeAction() {
            return Context.DeleteFramebuffer;
        }
    }
}

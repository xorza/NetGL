using System;
using System.Drawing;
using System.Drawing.Imaging;
using NetGL.Core.Helpers;
using NetGL.Core.Infrastructure;
using NetGL.Core.Mathematics;

namespace NetGL.Core.Types {
    public class Texture2 : Texture, IHaveSize {
        private bool _ownsImage = false;

        public Size Size { get; private set; }
        public Bitmap Image { get; private set; }
        public string Filename { get; private set; }
        public bool IsNormalMap { get; private set; }

        public Texture2() : this(false) { }
        public Texture2(bool multisample) : base(multisample ? TextureTarget.Texture2DMultisample : TextureTarget.Texture2D) { }
        public Texture2(string filename, bool normalMap)
            : this(false) {
            try {
                SetImage(filename, true, normalMap);
            }
            catch {
                Dispose();
                throw;
            }
        }

        public void SetImage(Bitmap bmp, bool saveImage, bool ownsImage, bool normalmap) {
            if (Target != TextureTarget.Texture2D)
                throw new GLException("Current target is" + Target);

            if (ReferenceEquals(bmp, Image))
                return;

            var maxSize = Context.MaxTextureSize;
            if (bmp.Width > maxSize)
                throw new GLException("Width is larger than maxsize");
            if (bmp.Height > maxSize)
                throw new GLException("Height is larger than maxsize");

            PixelInternalFormat = GetImagePixelInternalFormat(bmp);
            PixelFormat = GetImagePixelFormat(bmp);
            Size = bmp.Size;

            var bitmapData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly, bmp.PixelFormat);
            try {
                if (normalmap)
                    bitmapData.FlipGreen();

                Bind();
                Context.TexImage2D(Target, 0, PixelInternalFormat, Size.Width, Size.Height, 0, PixelFormat, PixelType.UnsignedByte, bitmapData.Scan0);
                Context.GenerateMipmap(GenerateMipmapTarget.Texture2D);
                UpdateParameters(true);
            }
            finally {
                bmp.UnlockBits(bitmapData);
            }

            _ownsImage = ownsImage;

            Image = null;
            if (saveImage == true)
                Image = bmp;
        }
        public void SetImage(ByteBuffer buffer, Size size, PixelFormat pixelformat, PixelInternalFormat internalFormat) {
            if (Target != TextureTarget.Texture2D)
                throw new GLException("Current target is " + Target);

            var maxSize = Context.MaxTextureSize;
            if (size.Width > maxSize)
                throw new GLException("Width is larger than maxsize");
            if (size.Height > maxSize)
                throw new GLException("Height is larger than maxsize");

            PixelInternalFormat = internalFormat;
            PixelFormat = pixelformat;
            Size = size;

            Bind();
            Context.TexImage2D(Target, 0, PixelInternalFormat, size.Width, size.Height, 0, PixelFormat, PixelType.UnsignedByte, buffer.Pointer);

            UpdateParameters(true);
        }
        public void SetImage(Size size, PixelInternalFormat internalFormat = PixelInternalFormat.Rgba8, PixelFormat pixelformat = PixelFormat.Rgba) {
            if (Target != TextureTarget.Texture2D)
                throw new GLException("Current target is" + Target);

            PixelInternalFormat = internalFormat;
            PixelFormat = pixelformat;
            Size = size;

            Bind();
            Context.TexStorage2D(Target, 1, PixelInternalFormat, Size.Width, Size.Height);

            UpdateParameters(true);
        }

        public Bitmap GetImage() {
            if (Target != TextureTarget.Texture2D)
                throw new NotSupportedException(Target.ToString());

            var result = new Bitmap(Size.Width, Size.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            try {
                var data = result.LockBits(new Rectangle(0, 0, Size.Width, Size.Height), ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

                Bind();
                Context.GetTexImage(Target, 0, Core.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);

                BitmapHelper.FlipVertically(data);

                result.UnlockBits(data);
            }
            catch {
                result.Dispose();
                throw;
            }

            return result;
        }

        public void SetMultisampleImage(int sampleCount, PixelInternalFormat pixelFormat, Size size) {
            if (Target != TextureTarget.Texture2DMultisample)
                throw new GLException("Current target is" + Target);

            Bind();
            Context.TexImage2DMultisample(Target, sampleCount, pixelFormat, size.Width, size.Height, true);

            Size = size;
        }

        protected override void OnDispose(bool isDisposing) {
            if (_ownsImage)
                Disposer.Dispose(Image);
            Image = null;
            base.OnDispose(isDisposing);
        }

        public void SetImage(string filename, bool saveImage, bool normalmap) {
            Bitmap bmp = null;
            try {
                bmp = (Bitmap)Bitmap.FromFile(filename);
                SetImage(bmp, saveImage, true, normalmap);
            }
            finally {
                if (bmp != null && saveImage == false)
                    bmp.Dispose();
            }
            Filename = filename;
            IsNormalMap = normalmap;
        }
        public void SetCheckers() {
            SetImage((ByteBuffer)new byte[]
            {
                255, 000, 255, 255,
                000, 000, 000, 255,
                000, 000, 000, 255,
                255, 000, 255, 255,
            }, new Size(2, 2), Core.PixelFormat.Rgba, Core.PixelInternalFormat.Rgba8);
        }
        public void SetColor(Vector4 color) {
            SetImage((ByteBuffer)new byte[]
            {
                (byte)(color.X * 255),
                (byte)(color.Y * 255),
                (byte)(color.Z * 255),
                (byte)(color.W * 255)
            }, new Size(1, 1), Core.PixelFormat.Rgba, Core.PixelInternalFormat.Rgba8);
        }
    }
}
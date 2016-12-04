using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetGL.Core.Types {
    public abstract class Texture : UIntObject {
        private bool _shouldUpdateParameters = false;
        private TextureWrapMode _wrap = TextureWrapMode.Repeat;
        private int _anisotropy = 1;
        private TextureFilter _minFilter = TextureFilter.Linear;
        private TextureFilter _magFilter = TextureFilter.Linear;

        public string Name { get; set; }

        public TextureWrapMode Wrap {
            get { return _wrap; }
            set {
                if (_wrap == value)
                    return;

                _wrap = value;
                _shouldUpdateParameters = true;
            }
        }
        public int Anisotropy {
            get { return _anisotropy; }
            set {
                if (value < 1 || value > 16)
                    throw new ArgumentOutOfRangeException("0..16");

                if (_anisotropy == value)
                    return;

                _anisotropy = value;
                _shouldUpdateParameters = true;
            }
        }
        public TextureFilter MinFilter {
            get { return _minFilter; }
            set {
                if (_minFilter == value)
                    return;

                _minFilter = value;
                _shouldUpdateParameters = true;
            }
        }
        public TextureFilter MagFilter {
            get { return _magFilter; }
            set {
                if (_magFilter == value)
                    return;

                switch (value) {
                    case TextureFilter.Nearest:
                    case TextureFilter.Linear:
                        break;
                    case TextureFilter.NearestMipmapNearest:
                    case TextureFilter.LinearMipmapNearest:
                    case TextureFilter.NearestMipmapLinear:
                    case TextureFilter.LinearMipmapLinear:
                        throw new GLException("MagFilter");
                    default:
                        throw new NotSupportedException(value.ToString());
                }

                _magFilter = value;
                _shouldUpdateParameters = true;
            }
        }
        public PixelFormat PixelFormat { get; protected set; }
        public PixelInternalFormat PixelInternalFormat { get; protected set; }

        public TextureTarget Target { get; private set; }

        protected Texture(TextureTarget target)
            : base() {
            Target = target;

            Initialize(Context.CreateTexture(target));
        }

        public void Bind() {
            Bind(TextureUnit.Texture0);
        }
        public void Bind(TextureUnit textureUnit) {
            if (IsDisposed)
                throw new ObjectDisposedException("Texture2");

            Context.ActiveTexture(textureUnit);
            Context.BindTexture(Target, this.Handle);

            UpdateParameters();
        }

        internal override DisposeAction GetDisposeAction() {
            return Context.DeleteTexture;
        }
        protected void UpdateParameters(bool force = false) {
            if (_shouldUpdateParameters == false && force == false)
                return;
            _shouldUpdateParameters = false;

            if (Context.DirectStateAccess) {
                Context.TextureParameter(Handle, TextureParameters.TextureMinFilter, _minFilter);
                Context.TextureParameter(Handle, TextureParameters.TextureMagFilter, _magFilter);
                Context.TextureParameter(Handle, TextureParameters.TextureWrapS, _wrap);
                Context.TextureParameter(Handle, TextureParameters.TextureWrapT, _wrap);
                Context.TextureParameter(Handle, TextureParameters.TextureWrapR, _wrap);
                Context.TextureParameter(Handle, TextureParameters.TextureMaxAnisotropy, _anisotropy);
            }
            else {
                Context.BindTexture(Target, this.Handle);
                Context.TexParameter(Target, TextureParameters.TextureMinFilter, _minFilter);
                Context.TexParameter(Target, TextureParameters.TextureMagFilter, _magFilter);
                Context.TexParameter(Target, TextureParameters.TextureWrapS, _wrap);
                Context.TexParameter(Target, TextureParameters.TextureWrapT, _wrap);
                Context.TexParameter(Target, TextureParameters.TextureWrapR, _wrap);
                Context.TexParameter(Target, TextureParameters.TextureMaxAnisotropy, _anisotropy);
            }
        }

        public static PixelInternalFormat GetImagePixelInternalFormat(Bitmap bmp) {
            if (BitConverter.IsLittleEndian == false)
                throw new NotSupportedException("Bigendian");

            switch (bmp.PixelFormat) {
                case System.Drawing.Imaging.PixelFormat.Format24bppRgb:
                    return PixelInternalFormat.Rgb8;
                case System.Drawing.Imaging.PixelFormat.Format32bppArgb:
                    return PixelInternalFormat.Rgba8;

                default:
                    throw new NotSupportedException(bmp.PixelFormat.ToString());
            }
        }
        public static PixelInternalFormat GetImagePixelInternalFormat(System.Drawing.Imaging.PixelFormat pixelformat) {
            if (BitConverter.IsLittleEndian == false)
                throw new NotSupportedException("Bigendian");

            switch (pixelformat) {
                case System.Drawing.Imaging.PixelFormat.Format24bppRgb:
                    return PixelInternalFormat.Rgb8;
                case System.Drawing.Imaging.PixelFormat.Format32bppArgb:
                    return PixelInternalFormat.Rgba8;

                default:
                    throw new NotSupportedException(pixelformat.ToString());
            }
        }
        public static PixelFormat GetImagePixelFormat(Bitmap bmp) {
            if (BitConverter.IsLittleEndian == false)
                throw new NotSupportedException("Bigendian");

            switch (bmp.PixelFormat) {
                case System.Drawing.Imaging.PixelFormat.Format24bppRgb:
                    return PixelFormat.Bgr;
                case System.Drawing.Imaging.PixelFormat.Format32bppArgb:
                    return PixelFormat.Bgra;

                default:
                    throw new NotSupportedException(bmp.PixelFormat.ToString());
            }
        }
        public static PixelFormat GetImagePixelFormat(System.Drawing.Imaging.PixelFormat pixelformat) {
            if (BitConverter.IsLittleEndian == false)
                throw new NotSupportedException("Bigendian");

            switch (pixelformat) {
                case System.Drawing.Imaging.PixelFormat.Format24bppRgb:
                    return PixelFormat.Bgr;
                case System.Drawing.Imaging.PixelFormat.Format32bppArgb:
                    return PixelFormat.Bgra;

                default:
                    throw new NotSupportedException(pixelformat.ToString());
            }
        }
    }
}
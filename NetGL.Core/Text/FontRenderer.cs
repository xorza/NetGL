using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.Linq;
using NetGL.Core.Mathematics;
using NetGL.Core.Types;

namespace NetGL.Core.Text {
    public sealed class FontRenderer : IDisposable {
        private const string Alphabet = " ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789,./<>?;':\"[]{}`~!@#$%^&*()-=\\_+|";
        private const int ImageSize = 2048;

        internal class FontChar {
            public char Char;
            public Rectangle Rectangle;
        }

        private readonly List<FontChar> _chars = new List<FontChar>();
        private readonly Bitmap _bmp;
        private readonly Font _font;
        private readonly int _fontSize;

        public Bitmap Image {
            get {
                return _bmp;
            }
        }

        public FontRenderer(Font font) {
            Assert.NotNull(font);

            _fontSize = font.Height;
            _font = font;
            _bmp = new Bitmap(ImageSize, ImageSize, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            Render();
        }

        private void Render() {
            using (var graphics = Graphics.FromImage(_bmp)) {
                graphics.TextRenderingHint = TextRenderingHint.AntiAlias;
                graphics.SmoothingMode = SmoothingMode.AntiAlias;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.Clear(Color.FromArgb(255, 0, 0, 0));

                var size = Size.Empty;
                var padding = (int)(0.1f * _fontSize);
                var location = new Point(padding, padding);

                foreach (var c in Alphabet) {
                    var fontChar = new FontChar();
                    _chars.Add(fontChar);
                    fontChar.Char = c;

                    if (c == ' ')
                        size = new Size(_fontSize / 3, _fontSize);
                    else {
                        size = FloorSize(graphics.MeasureString(c.ToString(), _font, PointF.Empty, StringFormat.GenericTypographic));
                        if (location.X + size.Width >= ImageSize) {
                            location.X = 0;
                            location.Y += _fontSize + padding;
                        }
                    }

                    fontChar.Rectangle = new Rectangle(location, size);
                    graphics.DrawString(c.ToString(), _font, Brushes.White, location, StringFormat.GenericTypographic);
                    location.X += size.Width + padding;
                }

                graphics.Flush();
            }

            CalculateAlpha();
        }

        private void CalculateAlpha() {
            var data = _bmp.LockBits(new Rectangle(0, 0, _bmp.Width, _bmp.Height), ImageLockMode.ReadWrite, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            try {
                unsafe {
                    var bytes = (byte*)data.Scan0;
                    for (int i = 0; i < data.Height * data.Width * 4; i += 4) {
                        bytes[i + 3] = Math.Max(Math.Max(bytes[i + 0], bytes[i + 1]), bytes[i + 2]);
                        bytes[i + 0] = bytes[i + 1] = bytes[i + 2] = 255;
                    }
                }
            }
            finally {
                _bmp.UnlockBits(data);
            }
        }
        private RectangleF GetCharUV(char c) {
            var charInfo = _chars.SingleOrDefault(_ => _.Char == c);

            var rect = charInfo.Rectangle;
            var imgSize = (float)ImageSize;

            return new RectangleF(
                rect.X / imgSize,
                rect.Y / imgSize,
                rect.Width / imgSize,
                rect.Height / imgSize
                );
        }

        public Texture2 CreateTexture() {
            var result = new Texture2();
            result.MinFilter = TextureFilter.LinearMipmapLinear;
            result.MagFilter = TextureFilter.Linear;
            result.Wrap = TextureWrapMode.ClampToEdge;
            result.Anisotropy = 4;
            result.SetImage(_bmp, false, false, false);
            return result;
        }
        public Mesh CreateMesh(string text) {
            var vertices = new List<Vector3>(text.Length * 4);
            var texCoords = new List<Vector2>(text.Length * 4);

            var charSize = _fontSize / (float)ImageSize;
            var offset = new Vector2();

            foreach (var c in text) {
                var rect = GetCharUV(c);
                var width = rect.Width / charSize;
                var height = rect.Height / charSize;
                var scale = 0.1f;

                vertices.Add((Vector3)(scale * (new Vector2(0, 0) + offset)));
                vertices.Add((Vector3)(scale * (new Vector2(width, 0) + offset)));
                vertices.Add((Vector3)(scale * (new Vector2(width, height) + offset)));
                vertices.Add((Vector3)(scale * (new Vector2(0, height) + offset)));

                texCoords.Add(new Vector2(rect.X, rect.Y + rect.Height));
                texCoords.Add(new Vector2(rect.X + rect.Width, rect.Y + rect.Height));
                texCoords.Add(new Vector2(rect.X + rect.Width, rect.Y));
                texCoords.Add(new Vector2(rect.X, rect.Y));

                offset.X += width;
            }

            var indices = new UInt32Buffer(text.Length * 6);
            for (int i = 0; i < text.Length; i++) {
                indices[i * 6 + 0] = (ushort)(i * 4 + 0);
                indices[i * 6 + 1] = (ushort)(i * 4 + 1);
                indices[i * 6 + 2] = (ushort)(i * 4 + 2);
                indices[i * 6 + 3] = (ushort)(i * 4 + 0);
                indices[i * 6 + 4] = (ushort)(i * 4 + 2);
                indices[i * 6 + 5] = (ushort)(i * 4 + 3);
            }

            var result = new Mesh();
            result.Vertices = vertices;
            result.TexCoords = texCoords;
            result.Indices = indices;
            result.CalculateBounds();
            return result;
        }

        private static Size FloorSize(SizeF s) {
            return new Size((int)Math.Floor(s.Width), (int)Math.Floor(s.Height));
        }

        public void Dispose() {
            _bmp.Dispose();
            _chars.Clear();
        }
    }
}
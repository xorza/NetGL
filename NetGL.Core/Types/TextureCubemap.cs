using System;
using System.Collections.Generic;
using System.Drawing;

namespace NetGL.Core.Types {
    public class TextureCubemap : Texture {
        public TextureCubemap() : base(TextureTarget.TextureCubeMap) { }

        internal override DisposeAction GetDisposeAction() {
            return Context.DeleteTexture;
        }

        public void SetImages(IReadOnlyList<Bitmap> images) {
            if (images.Count != 6)
                throw new ArgumentException("Images count should be 6!");
            if (images[0] == null)
                throw new ArgumentException("All images should not be null");

            var pixelFormat = GetImagePixelFormat(images[0]);
            var pixelInternalFormat = GetImagePixelInternalFormat(images[0]);
            var height = images[0].Height;
            var width = images[0].Width;

            var cubemapTargets = new TextureTarget[6] {
                TextureTarget.TextureCubeMapNegativeX,
                TextureTarget.TextureCubeMapPositiveX,
                TextureTarget.TextureCubeMapNegativeY,
                TextureTarget.TextureCubeMapPositiveY,
                TextureTarget.TextureCubeMapNegativeZ,
                TextureTarget.TextureCubeMapPositiveZ
            };

            Bind();

            //var buffer = new ByteBuffer(255, 255, 255, 255);

            for (int i = 0; i < cubemapTargets.Length; i++) {
                var image = images[i];

                if (GetImagePixelFormat(images[i]) != pixelFormat)
                    throw new ArgumentException("Pixel formats for all images should be same");
                if (GetImagePixelInternalFormat(images[i]) != pixelInternalFormat)
                    throw new ArgumentException("Pixel formats for all images should be same");
                if (images[i].Height != height || images[i].Width != width)
                    throw new ArgumentException("All images should be same size");

                var bmpdata = images[i].LockBits(new Rectangle(0, 0, width, height), System.Drawing.Imaging.ImageLockMode.ReadOnly, images[0].PixelFormat);
                try {
                    Context.TexImage2D(cubemapTargets[i], 0, pixelInternalFormat, width, height, 0, pixelFormat, PixelType.UnsignedByte, bmpdata.Scan0);
                }
                finally {
                    images[i].UnlockBits(bmpdata);
                }
            }

            PixelFormat = pixelFormat;
            PixelInternalFormat = pixelInternalFormat;

            MagFilter = TextureFilter.Linear;
            MinFilter = TextureFilter.Linear;
            Wrap = TextureWrapMode.ClampToEdge;
            Anisotropy = 1;
            UpdateParameters(true);
        }
    }
}

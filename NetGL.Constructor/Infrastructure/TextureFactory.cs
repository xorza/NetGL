using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using NetGL.Core.Helpers;
using NetGL.Core.Types;

namespace NetGL.Constructor.Infrastructure {
    public class TextureFactory {
        private readonly WeakCollection<Texture2> _textures = new WeakCollection<Texture2>();

        //public Texture2 Create(string fileName, bool normal) {
        //    var bmp = (Bitmap)Bitmap.FromFile(fileName);
        //    if (normal)
        //        bmp.FlipGreen();

        //    var texture = new Texture2();
        //    texture.SetImage(bmp, saveImage: true, ownsImage: true);

        //    _textures.Add(texture);

        //    return texture;
        //}
        //public Texture2 Create(Bitmap bmp, bool normal) {
        //    if (normal)
        //        bmp.FlipGreen();

        //    var texture = new Texture2();
        //    texture.SetImage(bmp, saveImage: true, ownsImage: false);

        //    _textures.Add(texture);

        //    return texture;
        //}

        public IEnumerable<Texture2> GetCreatedTextures() {
            return _textures
                .Where(_ => _.IsDisposed == false);
        }
    }
}

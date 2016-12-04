using System.Drawing;
using NetGL.Core.Types;

namespace NetGL.Constructor.Infrastructure {
    public class MeshPreviewViewModel {
        public Mesh Mesh { get; private set; }
        public Bitmap Preview { get; private set; }
        public string Name { get; private set; }

        public MeshPreviewViewModel(Mesh mesh, Bitmap preview) {
            Assert.NotNull(mesh);

            Name = mesh.ToString();
            Mesh = mesh;
            Preview = preview;
        }
    }
}

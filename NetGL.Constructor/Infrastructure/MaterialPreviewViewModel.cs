using System.Drawing;
using NetGL.SceneGraph.Components;

namespace NetGL.Constructor.Infrastructure {
    public class MaterialPreviewViewModel {
        public Material Material { get; private set; }
        public Bitmap Preview { get; private set; }
        public string Name { get; private set; }

        public MaterialPreviewViewModel(Material material, Bitmap preview) {
            Assert.NotNull(material);

            Name = material.Name;
            Material = material;
            Preview = preview;
        }
    }
}

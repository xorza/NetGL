using System.Windows.Controls;
using NetGL.Constructor.Infrastructure;
using NetGL.SceneGraph.Components;

namespace NetGL.Constructor.Inspectors {
    public partial class CameraInspector : UserControl {
        public CameraInspector() {
            InitializeComponent();
        }
        public CameraInspector(Camera camera) {
            Assert.NotNull(camera);
            this.DataContext = new CameraViewModel(camera);
            InitializeComponent();
        }
    }
}

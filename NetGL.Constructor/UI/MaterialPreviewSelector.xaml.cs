using System;
using System.Drawing;
using System.Linq;
using System.Windows.Controls;
using NetGL.Constructor.Infrastructure;
using NetGL.Constructor.Scene;
using NetGL.Core.Mathematics;
using NetGL.SceneGraph.OpenGL;

namespace NetGL.Constructor.UI {
    public partial class MaterialPreviewSelector : UserControl, IDisposable {
        public MaterialPreviewSelector() {
            InitializeComponent();

            var previewRenderer = new PreviewRenderer(new Size(200, 200));
            previewRenderer.BackgroundColor = new Vector4(0.75f, 0.75f, 0.75f, 1f);

            //var materials = SceneViewModel.Instance.MaterialFactory
            //    .GetCreatedMaterials();

            //MaterialsListBox.ItemsSource = materials
            //    .Select(_ => new MaterialPreviewViewModel(_, previewRenderer.RenderPreview(_)))
            //    .ToArray();

            previewRenderer.Dispose();
        }


        public void Dispose() {
            GC.SuppressFinalize(this);
        }
    }
}

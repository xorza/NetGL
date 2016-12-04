using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Controls;
using NetGL.Constructor.Infrastructure;
using NetGL.Core.Mathematics;
using NetGL.Core.Meshes;
using NetGL.Core.Types;
using NetGL.SceneGraph.OpenGL;

namespace NetGL.Constructor.UI {
    public partial class MeshPreviewSelector : UserControl, IDisposable {
        public MeshPreviewSelector() {
            InitializeComponent();

            var previewRenderer = new PreviewRenderer(new Size(200, 200));
            previewRenderer.BackgroundColor = new Vector4(0.75f, 0.75f, 0.75f, 1f);

            var meshes = new List<Mesh>();

            meshes.Add(Icosahedron.Create(0.45f, 0));
            meshes.Add(Icosahedron.Create(0.45f, 1));
            meshes.Add(Tube.Create(0.3f, 0.35f, 0.7f, 20));
            meshes.Add(Cloud.Create(CloudShape.Sphere, 0.9f, 50000));
            meshes.Add(Cone.Create(0.3f, 0.8f));
            meshes.Add(TrefoilKnot.Create());

            var rotation = Quaternion.CreateFromYawPitchRoll(0, -0.25f, -0.2f);

            MeshesListBox.ItemsSource = meshes
                .Select(_ => new MeshPreviewViewModel(_, previewRenderer.RenderPreview(_, rotation)))
                .ToArray();

            previewRenderer.Dispose();
        }

        public void Dispose() {
            GC.SuppressFinalize(this);
        }
    }
}

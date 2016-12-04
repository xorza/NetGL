using NetGL.SceneGraph.Components;
using NetGL.SceneGraph.Scene;
using System.Collections.Generic;
using System.Drawing;

namespace NetGL.SceneGraph.OpenGL {
    internal interface IRenderTechnique {
        Size ScreenSize { get; set; }

        void RenderAll(IReadOnlyList<Camera> cameras, IReadOnlyList<LightSource> lights, RendererCollection renderables);
    }
}
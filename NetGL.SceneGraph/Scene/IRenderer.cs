using NetGL.Core.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetGL.SceneGraph.Scene {
    internal interface IRenderer {
        Sphere BoundSphere { get; }

        void Prerender();
        void Render();
    }
}

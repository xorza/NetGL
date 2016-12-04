using System;
using System.Drawing;
using System.Windows.Forms;
using NetGL.Core.Mathematics;
using NetGL.SceneGraph.Scene;

namespace NetGL.SceneGraph.Control {
    public interface INetGLControl : IDisposable {
        RenderDispatcher RenderDispatcher { get; }
        Scene.Scene Scene { get; }
        Vector2 ScreenMousePositionDelta { get; }
        Vector2 ScreenMousePosition { get; }
        Vector2 ViewportMousePosition { get; }
        MouseButtons PressedMouseButtons { get; }
        bool IsMouseOver { get; }
        Size Size { get; }

        event MouseEventHandler MouseDown;
        event MouseEventHandler MouseMove;
        event MouseEventHandler MouseUp;
        
        void Frame();

        bool IsKeyDown(Keys key);
        bool IsKeyToggled(Keys key);

        Bitmap ReadImage();
    }
}

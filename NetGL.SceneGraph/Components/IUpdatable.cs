using System;

namespace NetGL.SceneGraph.Components {
    public interface IUpdatable : IDisposable {
        bool IsEnabled { get; }
        bool IsDisposed { get; }

        void Update();
    }
}
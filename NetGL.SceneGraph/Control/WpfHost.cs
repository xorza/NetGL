using NetGL.Core.Infrastructure;
using NetGL.SceneGraph.Scene;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms.Integration;
using System.Windows.Media;

namespace NetGL.SceneGraph.Control {
    public class WpfHost : UserControl, IDisposable {
        public static readonly DependencyProperty SceneProperty =
            DependencyProperty.Register("Scene", typeof(Scene.Scene), typeof(WpfHost), new PropertyMetadata(null));

        private readonly WindowsFormsHost _host;
        private Scene.Scene _scene;
        private NetGLControl _netGLControl;

        public INetGLControl NetGLControl {
            get {
                return _netGLControl;
            }
        }
        public NetGL.SceneGraph.Scene.Scene Scene {
            get { return _scene; }
            set {
                if (_scene == value)
                    return;

                _scene = value;
                SetValue(SceneProperty, value);
            }
        }

        public WpfHost() {
            if (DesignerProperties.GetIsInDesignMode(this)) {
                var border = new Border();
                border.Background = Brushes.LightGray;
                border.BorderThickness = new Thickness(1);
                border.BorderBrush = Brushes.DarkGray;

                var text = new TextBlock();
                text.Text = "3D will be here";
                text.VerticalAlignment = VerticalAlignment.Center;
                text.HorizontalAlignment = HorizontalAlignment.Center;
                border.Child = text;

                Content = border;
                return;
            }

            Content = _host = new WindowsFormsHost();
            _host.Child = _netGLControl = new NetGLControl();
            Scene = _netGLControl.Scene;
        }

        public void Dispose() {
            Disposer.Dispose(ref _netGLControl);
        }
    }
}

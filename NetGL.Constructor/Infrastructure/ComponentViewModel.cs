using System;
using System.Windows;
using System.Windows.Input;
using NetGL.SceneGraph.Components;
using NetGL.SceneGraph.Scene;

namespace NetGL.Constructor.Infrastructure {
    internal class ComponentViewModel : NotifyPropertyChange, IRefresh {
        private readonly Component _component;
        private Visibility _visibility = Visibility.Visible;

        public Visibility Visibility {
            get { return _visibility; }
            set {
                if (_visibility == value)
                    return;

                _visibility = value;
                OnPropertyChanged();
            }
        }
        public string Name {
            get { return _component.GetType().Name; }
        }
        public bool IsExpanded { get; set; }
        public bool IsEnabled {
            get { return _component.IsEnabled; }
            set {
                if (_component.IsEnabled == value)
                    return;

                _component.IsEnabled = value;
                OnPropertyChanged();                               
            }
        }
        public object Content { get; private set; }
        public ICommand RemoveCommand {
            get {
                return new Command(() => {
                    if (_component.IsDisposed)
                        return;

                    Visibility = Visibility.Collapsed;
                    _component.Dispose();
                });
            }
        }
        public Component Component {
            get { return _component; }
        }

        public ComponentViewModel(Component component) {
            if (component == null)
                throw new ArgumentNullException("component");

            _component = component;
            IsExpanded = true;

            var camera = component as Camera;
            if (camera != null) {
                Content = new CameraViewModel(camera);
                return;
            }

            Content = new GenericInspectorViewModel(_component);
        }

        public void Refresh() {
            if (_component.IsDisposed) {
                Visibility = Visibility.Collapsed;
                return;
            }
            if (IsExpanded == false)
                return;

            IsEnabled = _component.IsEnabled;
            var refresh = Content as IRefresh;
            if (refresh != null)
                refresh.Refresh();
        }
    }
}
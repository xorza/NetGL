using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using NetGL.Constructor.Infrastructure;
using NetGL.SceneGraph.Scene;

namespace NetGL.Constructor.UI {
    public partial class ComponentsInspector : UserControl, IRefresh {
        public static readonly DependencyProperty SceneObjectProperty =
            DependencyProperty.Register("SceneObject", typeof(Node), typeof(ComponentsInspector), new PropertyMetadata(null, DependencyPropertyChangedCallback));

        private Node _sceneObject;
        private readonly DispatcherTimer _timer;

        private readonly ObservableCollection<ComponentViewModel> _components = new ObservableCollection<ComponentViewModel>();

        public Node SceneObject {
            get { return _sceneObject; }
            set {
                if (_sceneObject != value)
                    SetValue(SceneObjectProperty, value);
            }
        }

        public ComponentsInspector() {
            InitializeComponent();

            componentsItemsControl.ItemsSource = _components;

            _timer = new DispatcherTimer(DispatcherPriority.Normal,Dispatcher);
            _timer.Interval = TimeSpan.FromSeconds(0.5);

            _timer.Tick += (s, ea) => { Refresh(); };
            this.Loaded += (s, ea) => { _timer.Start(); };
            this.Unloaded += (s, ea) => { _timer.Stop(); };
        }

        public void Refresh() {
            if (_sceneObject == null) {
                _components.Clear();
                return;
            }

            for (int i = _components.Count - 1; i >= 0; i--) {
                var componentWrapper = _components[i];
                if (_sceneObject.Components.All(_ => _ != componentWrapper.Component))
                    _components.Remove(componentWrapper);
            }

            foreach (var component in _sceneObject.Components)
                if (_components.All(_ => _.Component != component))
                    _components.Add(new ComponentViewModel(component));

            _components.ForEach(_ => _.Refresh());
        }

        private static void DependencyPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            var view = (ComponentsInspector)d;

            if (e.Property == SceneObjectProperty) {
                var value = (Node)e.NewValue;

                view._sceneObject = value;
                view._components.Clear();

                if (value == null)
                    return;

                value.Components
                    .Select(_ => new ComponentViewModel(_))
                    .ForEach(_ => view._components.Add(_));

                return;
            }
        }
    }
}
using NetGL.Constructor.Infrastructure;
using NetGL.SceneGraph.Scene;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace NetGL.Constructor.UI {
    public partial class SceneTreeView : UserControl, INotifyPropertyChanged {
        public static readonly DependencyProperty SelectedSceneObjectProperty =
            DependencyProperty.Register("SelectedSceneObject", typeof(Node), typeof(SceneTreeView), new PropertyMetadata(null, OnDependencyPropertyChanged));
        public static readonly DependencyProperty SceneProperty =
            DependencyProperty.Register("Scene", typeof(SceneGraph.Scene.Scene), typeof(SceneTreeView), new PropertyMetadata(null, OnDependencyPropertyChanged));

        private readonly ObservableCollection<SceneObjectViewModel> _sceneObjectsViewModels = new ObservableCollection<SceneObjectViewModel>();

        private readonly DispatcherTimer _timer;
        private SceneGraph.Scene.Scene _scene;
        private Node _selectedSceneObject;
        private SceneObjectViewModel _selectedObjectViewModel;

        public NetGL.SceneGraph.Scene.Scene Scene {
            get { return _scene; }
            set {
                if (value != _scene)
                    SetValue(SceneProperty, value);
            }
        }
        public Node SelectedSceneObject {
            get { return _selectedSceneObject; }
            set {
                if (value != _selectedSceneObject)
                    SetValue(SelectedSceneObjectProperty, value);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public SceneTreeView() {
            InitializeComponent();

            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(0.5);

            treeView.ItemsSource = _sceneObjectsViewModels;
            treeView.SelectedItemChanged += TreeView_SelectedItemChanged;

            _timer.Tick += (s, ea) => { Refresh(); };
            this.Loaded += (s, ea) => { _timer.Start(); };
            this.Unloaded += (s, ea) => { _timer.Stop(); };
        }

        public void RemoveSelected() {
            _selectedSceneObject.Dispose();
            var sceneObjVm = _selectedObjectViewModel;
            _selectedSceneObject = null;
            _selectedObjectViewModel = null;
            if (sceneObjVm.Parent == null)
                _sceneObjectsViewModels.Remove(sceneObjVm);
            else
                sceneObjVm.Parent.Children.Remove(sceneObjVm);
        }

        private void Refresh() {
            if (_scene == null)
                return;

            var roots = _scene.SceneObjects
                .Where(_ => _.Transform.Parent == null);
            foreach (var so in roots) {
                if (_sceneObjectsViewModels.All(_ => _.SceneObject != so)) {
                    var item = new SceneObjectViewModel(so);
                    item.LoadChildren();
                    _sceneObjectsViewModels.Add(item);
                }
            }
            for (int i = _sceneObjectsViewModels.Count - 1; i >= 0; i--) {
                var item = _sceneObjectsViewModels[i];
                if (false == roots.Contains(item.SceneObject))
                    _sceneObjectsViewModels.RemoveAt(i);
            }
            _sceneObjectsViewModels.ForEach(_ => _.Refresh());
        }
        private void SelectTreeViewItem() {
            if (_selectedSceneObject == null) {
                _selectedObjectViewModel = null;
                return;
            }

            if (_selectedObjectViewModel != null)
                if (_selectedObjectViewModel.SceneObject == _selectedSceneObject)
                    return;

            var parents = new Stack<Node>();
            var sceneObj = _selectedSceneObject;
            do {
                parents.Push(sceneObj);
                if (sceneObj.Transform.Parent != null)
                    sceneObj = sceneObj.Transform.Parent.SceneObject;
                else
                    sceneObj = null;
            }
            while (sceneObj != null);

            treeView.SelectedItemChanged -= TreeView_SelectedItemChanged;

            sceneObj = parents.Pop();
            var viewModel = _sceneObjectsViewModels.Single(_ => _.SceneObject == sceneObj);
            viewModel.IsExpanded = true;
            while (parents.Count > 0) {
                sceneObj = parents.Pop();

                viewModel.IsExpanded = true;
                viewModel = viewModel.Children.Single(_ => _.SceneObject == sceneObj);
            }

            viewModel.IsSelected = true;

            treeView.SelectedItemChanged += TreeView_SelectedItemChanged;
        }

        private void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e) {
            treeView.SelectedItemChanged -= TreeView_SelectedItemChanged;

            var value = (SceneObjectViewModel)e.NewValue;

            if (_selectedObjectViewModel == value)
                return;
            _selectedObjectViewModel = value;

            if (_selectedObjectViewModel == null)
                SelectedSceneObject = null;
            else
                SelectedSceneObject = _selectedObjectViewModel.SceneObject;

            treeView.SelectedItemChanged += TreeView_SelectedItemChanged;
        }
        private void DeleteSceneObjectMenuItem_Click(object sender, RoutedEventArgs e) {
            var menuItem = (sender as MenuItem);
            if (menuItem == null)
                return;
            var sceneObjVm = menuItem.DataContext as SceneObjectViewModel;
            if (sceneObjVm == null)
                return;

            if (sceneObjVm.Parent == null)
                _sceneObjectsViewModels.Remove(sceneObjVm);
            else
                sceneObjVm.Parent.Children.Remove(sceneObjVm);

            sceneObjVm.SceneObject.Dispose();
        }
        private void CreateChildSceneObjectMenuItem_Click(object sender, RoutedEventArgs e) {
            var menuItem = (sender as MenuItem);
            if (menuItem == null)
                return;
            var sceneObjVm = menuItem.DataContext as SceneObjectViewModel;
            if (sceneObjVm == null)
                return;

            sceneObjVm.IsExpanded = true;
            var so = new Node(sceneObjVm.SceneObject.Scene);
            so.Transform.Parent = sceneObjVm.SceneObject.Transform;
            var sovm = new SceneObjectViewModel(so);
            sceneObjVm.Children.Add(sovm);
            sovm.IsSelected = true;
        }

        private void NotifyPropertyChanged([CallerMemberName] string propName = null) {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propName));
        }
        private static void OnDependencyPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            var view = (SceneTreeView)d;

            if (e.Property == SelectedSceneObjectProperty) {
                view._selectedSceneObject = (Node)e.NewValue;
                view.SelectTreeViewItem();
                return;
            }
            if (e.Property == SceneProperty) {
                view._scene = (SceneGraph.Scene.Scene)e.NewValue;

                if (view._scene == null)
                    view._sceneObjectsViewModels.Clear();
                else
                    view.Refresh();
                return;
            }
        }
    }
}
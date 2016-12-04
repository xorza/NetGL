using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using NetGL.SceneGraph.Scene;

namespace NetGL.Constructor.Infrastructure {
    internal class SceneObjectViewModel : INotifyPropertyChanged {
        private bool _isLoaded = false;
        private bool _isExpanded = false;
        private string _name;
        private bool _isSelected;

        public event PropertyChangedEventHandler PropertyChanged;

        public string Name {
            get {
                return _name;
            }
            private set {
                if (_name == value)
                    return;

                _name = value;
                NotifyPropertyChanged();
            }
        }
        public bool IsExpanded {
            get {
                return _isExpanded;
            }
            set {
                if (_isExpanded == value)
                    return;

                _isExpanded = value;

                if (_isExpanded) {
                    LoadChildren();
                    Children.ForEach(_ => _.LoadChildren());
                    Refresh();
                }
                else
                    Children.ForEach(_ => _.ClearChildren());

                NotifyPropertyChanged();
            }
        }
        public bool IsSelected {
            get { return _isSelected; }
            set {
                if (_isSelected == value)
                    return;
                _isSelected = value;
                NotifyPropertyChanged();
            }
        }
        public ObservableCollection<SceneObjectViewModel> Children { get; private set; }
        public Node SceneObject { get; private set; }
        public SceneObjectViewModel Parent { get; private set; }

        public SceneObjectViewModel(Node so) {
            Assert.NotNull(so);

            SceneObject = so;
            Children = new ObservableCollection<SceneObjectViewModel>();

            Refresh();
        }

        public void LoadChildren() {
            if (_isLoaded)
                return;
            _isLoaded = true;

            foreach (var transform in SceneObject.Transform.Children)
                AddChild(transform);
        }

        private void AddChild(Transform treansform) {
            var treeViewItem = new SceneObjectViewModel(treansform.SceneObject);
            treeViewItem.Parent = this;
            if (IsExpanded)
                treeViewItem.LoadChildren();
            Children.Add(treeViewItem);
        }
        public void ClearChildren() {
            _isLoaded = false;
            Children.ForEach(_ => _.ClearChildren());
            Children.Clear();
        }
        public void Refresh() {
            Name = SceneObject.ToString();

            if (!IsExpanded)
                return;

            for (int i = Children.Count - 1; i >= 0; i--) {
                var item = Children[i];
                if (!SceneObject.Transform.Children.Contains(item.SceneObject.Transform))
                    Children.RemoveAt(i);
            }
            foreach (var item in SceneObject.Transform.Children)
                if (Children.All(_ => _.SceneObject.Transform != item))
                    AddChild(item);

            Children.ForEach(_ => _.Refresh());
        }

        private void NotifyPropertyChanged([CallerMemberName] string propName = "") {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propName));
        }
    }
}

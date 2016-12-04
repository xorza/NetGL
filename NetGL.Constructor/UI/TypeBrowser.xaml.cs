using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Win32;
using NetGL.Constructor.Infrastructure;
using NetGL.SceneGraph.Scene;

namespace NetGL.Constructor.UI {
    public partial class TypeBrowser : UserControl {
        public List<AssemblyViewModel> Assemblies { get; private set; }
        public Type ParentTypeFilter { get; private set; }

        public event EventHandler<EventArgs<Type>> TypeSelected;

        public TypeBrowser() {
            InitializeComponent();

            ParentTypeFilter = typeof(Component);
            Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e) {
            Assemblies = new List<AssemblyViewModel>();

            AppDomain.CurrentDomain.GetAssemblies()
                .ForEach(AddAssembly);

            treeView.ItemsSource = Assemblies;
            treeView.SelectedItemChanged += TreeView_SelectedItemChanged;
        }

        private void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e) {
            var isEnabled = false;
            var typeVm = treeView.SelectedItem as TypeViewModel;
            if (typeVm != null)
                isEnabled = true;

            AddComponentButton.IsEnabled = isEnabled;
        }

        private void AddAssembly(Assembly assembly) {
            var assebmlyVM = new AssemblyViewModel(assembly, ParentTypeFilter);
            if (assebmlyVM.Types.Count > 0)
                Assemblies.Add(assebmlyVM);
        }

        private void TreeviewItem_MouseDoubleClick(object sender, MouseButtonEventArgs e) {

        }
        private void LoadAssemblyButton_Click(object sender, RoutedEventArgs e) {
            var parentWindow = Window.GetWindow(this);
            var ofd = new OpenFileDialog();
            ofd.Filter = "Assemblies (.dll,.exe)|*.dll;*.exe|All Files (.*)|*.*";
            if (ofd.ShowDialog(parentWindow) != true)
                return;

            Assembly loadedAssembly = null;
            try {
                loadedAssembly = Assembly.LoadFrom(ofd.FileName);
            }
            catch (Exception ex) {
                ChildWindow.ShowError(ex, parentWindow);
            }

            if (loadedAssembly != null)
                AddAssembly(loadedAssembly);
        }

        private void AddComponentButton_Click(object sender, RoutedEventArgs e) {
            Assert.NotNull(treeView.SelectedItem);
            var typeVm = (TypeViewModel)treeView.SelectedItem;

            if (TypeSelected != null)
                TypeSelected(this, new EventArgs<Type>(typeVm.Type));
        }
    }
}

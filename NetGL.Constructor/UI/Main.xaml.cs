using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using Microsoft.Win32;
using NetGL.Constructor.Components.TextureSelection;
using NetGL.Constructor.Scene;
using System.Drawing.Imaging;
using NetGL.Core.Helpers;

namespace NetGL.Constructor.UI {
    public partial class Main : UserControl {
        private readonly DispatcherTimer _timer;

        public Main() {
            InitializeComponent();

            if (DesignerProperties.GetIsInDesignMode(this))
                return;

            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(0.2);
            _timer.Tick += Timer_Tick;

            DataContext = SceneViewModel.Instance;
            SceneViewModel.Instance.Scene = WpfHost.NetGLControl.Scene;

            Loaded += (s, ea) => { _timer.Start(); };
            Unloaded += (s, ea) => { _timer.Stop(); };
        }

        private void Timer_Tick(object sender, EventArgs e) {
            if (WpfHost.NetGLControl.Scene != null)
                statusTextBlock.Text = string.Format("FPS: {0:F1}; Draw calls: {1:D}; Frametime: {2:F2}ms; GC count: {3} {4} {5}.", WpfHost.NetGLControl.Scene.FPS, WpfHost.NetGLControl.Scene.DrawCalls, WpfHost.NetGLControl.Scene.FrameTime * 1000f, GC.CollectionCount(0), GC.CollectionCount(1), GC.CollectionCount(2));
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e) {
            var sfd = new SaveFileDialog();
            sfd.Filter = "JSON files (.json)|*.json";
            if (sfd.ShowDialog(this.GetWindow()) != true)
                return;

            WpfHost.NetGLControl.Scene.Serialize(sfd.FileName);
        }
        private void LoadButton_Click(object sender, RoutedEventArgs e) {
            var ofd = new OpenFileDialog();
            ofd.Filter = "JSON files (.json)|*.json";
            if (ofd.ShowDialog(this.GetWindow()) != true)
                return;

            WpfHost.NetGLControl.Scene.Derialize(ofd.FileName);
        }
        private void ClearButton_Click(object sender, RoutedEventArgs e) {
            WpfHost.NetGLControl.Scene.Clear();
        }

        private void RemoveSOButton_Click(object sender, RoutedEventArgs e) {
            sceneTreeView.RemoveSelected();
        }
        private void AddComponentButton_Click(object sender, RoutedEventArgs e) {
            var typeBrowser = new TypeBrowser();
            var wnd = ChildWindow.Create("Components", typeBrowser, this.GetWindow(), true);

            typeBrowser.TypeSelected += (s, ea) => {
                wnd.Close();

                sceneTreeView.SelectedSceneObject.AddComponent(ea.Value);
            };

            wnd.ShowDialog();
        }
        private void CloseButton_Click(object sender, RoutedEventArgs e) {
            this.GetWindow().Close();
        }

        private void PerformFullGC_Click(object sender, RoutedEventArgs e) {
            GC.Collect();
        }

        private void ShowMaterialSelector_Click(object sender, RoutedEventArgs e) {
            var materialPreviewSelector = new MaterialPreviewSelector();
            var wnd = new ChildWindow(materialPreviewSelector);
            wnd.Owner = this.GetWindow();
            wnd.Show();
        }
        private void ShowTextureSelector_Click(object sender, RoutedEventArgs e) {
            var texturePreviewSelector = new TexturePreviewSelector();
            var wnd = new ChildWindow(texturePreviewSelector);
            wnd.Owner = this.GetWindow();
            wnd.Show();
        }
        private void ShowMeshSelector_Click(object sender, RoutedEventArgs e) {
            var meshPreviewSelector = new MeshPreviewSelector();
            var wnd = new ChildWindow(meshPreviewSelector);
            wnd.Owner = this.GetWindow();
            wnd.Show();
        }

        private void TakeScreenshotButton_Click(object sender, RoutedEventArgs e) {
            var ofd = new SaveFileDialog();
            ofd.Filter = "Image Files (*.png)|*.png";
            if (ofd.ShowDialog(this.GetWindow()) != true)
                return;

            var bitmap = WpfHost.NetGLControl.ReadImage();
            bitmap.Save(ofd.FileName, ImageFormat.Png);
        }
    }
}
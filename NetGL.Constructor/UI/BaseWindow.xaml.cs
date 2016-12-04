using System;
using System.Windows;
using System.Windows.Media;
using NetGL.Core.Infrastructure;

namespace NetGL.Constructor.UI {
    public partial class BaseWindow : Window {
        public bool HasMaximizeMinimizeButton {
            get { return (bool)GetValue(HasMaximizeMinimizeButtonProperty); }
            set { SetValue(HasMaximizeMinimizeButtonProperty, value); }
        }
        public static readonly DependencyProperty HasMaximizeMinimizeButtonProperty = DependencyProperty.Register("HasMaximizeMinimizeButton", typeof(bool), typeof(BaseWindow), new PropertyMetadata(true));

        public BaseWindow() {
            InitializeComponent();

            TextOptions.SetTextRenderingMode(this, TextRenderingMode.Auto);
            TextOptions.SetTextFormattingMode(this, TextFormattingMode.Display);

            this.Closed += OnClosed;
            this.StateChanged += BaseWindow_StateChanged;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e) {
            Close();
        }
        private void MinimizeButton_Click(object sender, RoutedEventArgs e) {
            WindowState = WindowState.Minimized;
        }
        private void MaximizeButton_Click(object sender, RoutedEventArgs e) {
            MaximizeNormalize();
        }
        private void BaseWindow_StateChanged(object sender, EventArgs e) {
            if (WindowState == WindowState.Maximized) {
            }
        }
        private void OnClosed(object sender, EventArgs e) {
            DisposeLogicalTree(this);
        }
        private void DisposeLogicalTree(object current) {
            var depObj = current as DependencyObject;
            if (depObj != null)
                foreach (object logicalChild in LogicalTreeHelper.GetChildren(depObj))
                    DisposeLogicalTree(logicalChild);

            Disposer.Dispose(ref current);
        }

        private void MaximizeNormalize() {
            switch (WindowState) {
                case (WindowState.Maximized): {
                        ResizeMode = ResizeMode.NoResize;
                        WindowStyle = WindowStyle.None;
                        WindowState = WindowState.Normal;
                        break;
                    }
                case (WindowState.Normal): {
                        ResizeMode = ResizeMode.NoResize;
                        WindowStyle = WindowStyle.None;
                        WindowState = WindowState.Maximized;
                        break;
                    }
            }
        }
    }
}
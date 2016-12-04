using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace NetGL.Constructor.UI {
    internal class ChildWindow : BaseWindow {
        public ChildWindow(UserControl content = null) {

            TextOptions.SetTextRenderingMode(this, TextRenderingMode.Auto);
            TextOptions.SetTextFormattingMode(this, TextFormattingMode.Display);

            this.SnapsToDevicePixels = true;
            this.UseLayoutRounding = true;
            this.ShowInTaskbar = false;
            this.Width = double.NaN;
            this.Height = double.NaN;
            this.VerticalContentAlignment = VerticalAlignment.Stretch;
            this.HorizontalContentAlignment = HorizontalAlignment.Stretch;
            this.ShowInTaskbar = false;
            this.SizeToContent = SizeToContent.WidthAndHeight;
            this.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            this.MinHeight = 150;
            this.MinWidth = 150;
            this.Content = content;
            this.Activated += ChildWindow_Activated;
        }

        private void ChildWindow_Activated(object sender, EventArgs e) {
            this.Activated -= ChildWindow_Activated;

            this.InvalidateVisual();

            this.Width = this.ActualWidth;
            this.Height = this.ActualHeight;
            this.SizeToContent = SizeToContent.Manual;
        }

        public static Window Create(string title, UserControl control, Window owner = null, bool resizable = true) {
            var wnd = new ChildWindow(control);
            wnd.Title = title;
            wnd.Owner = owner;

            if (resizable)
                wnd.ResizeMode = ResizeMode.CanResize;
            else
                wnd.ResizeMode = ResizeMode.NoResize;

            return wnd;
        }

        public static void ShowError(Exception ex, Window owner) {
            var control = new ErrorControl(ex);
            var wnd = Create("Error", control, owner, false);
            control.OKButtonClick += (s, ea) => { wnd.Close(); };

            wnd.ShowDialog();
        }
    }
}

using System;
using System.Drawing;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Media.Imaging;
using NetGL.Constructor.Infrastructure;
using Image = System.Windows.Controls.Image;

namespace NetGL.Constructor.UI {
    public class BitmapPickerButton : ButtonBase {
        public static readonly DependencyProperty BitmapProperty =
            DependencyProperty.Register("Bitmap", typeof(Bitmap), typeof(BitmapPickerButton), new PropertyMetadata(null, PropertyChangedCallback));

        private readonly Image _image;

        public Bitmap Bitmap {
            get { return (Bitmap)GetValue(BitmapProperty); }
            set { SetValue(BitmapProperty, value); }
        }

        public BitmapPickerButton() {
            Content = _image = new Image();

            _image.Margin = new Thickness(0);
            _image.Source = null;
        }

        protected override void OnClick() {
            var dialog = FileDialogHelper.CreateOpenImageDialog();
            if (dialog.ShowDialog(this.GetWindow()) != true)
                return;

            Bitmap bmp;
            try {
                bmp = (Bitmap)Bitmap.FromFile(dialog.FileName);
            }
            catch (Exception ex) {
                ChildWindow.ShowError(ex, this.GetWindow());
                return;
            }
            Bitmap = bmp;

            Refresh();
        }

        private void Refresh() {
            _image.Source = Bitmap.GetBitmapSource(BitmapSizeOptions.FromWidthAndHeight(64, 64));
        }

        private static void PropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            var view = (BitmapPickerButton)d;
            if (e.Property == BitmapProperty) {
                var bitmap = (Bitmap)e.NewValue;
                if (bitmap == null) {
                    view._image.Source = null;
                    return;
                }

                view.Refresh();
                return;
            }
        }
    }
}

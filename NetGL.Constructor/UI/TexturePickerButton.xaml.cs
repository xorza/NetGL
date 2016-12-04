using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using NetGL.Constructor.Components.TextureSelection;
using NetGL.Constructor.Infrastructure;
using NetGL.Core.Types;

namespace NetGL.Constructor.UI {
    public partial class TexturePickerButton : UserControl {
        public static readonly DependencyProperty TextureProperty =
            DependencyProperty.Register("Texture", typeof(Texture2), typeof(TexturePickerButton), new PropertyMetadata(null, TexturePropertyChangedCallback));

        public Texture2 Texture {
            get { return (Texture2)GetValue(TextureProperty); }
            set { SetValue(TextureProperty, value); }
        }

        public TexturePickerButton() {
            InitializeComponent();
        }

        private void RemoveTexture_Click(object sender, RoutedEventArgs e) {
            Texture = null;
        }

        private static void TexturePropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            var view = (TexturePickerButton)d;
            var texture = e.NewValue as Texture2;
            Bitmap bitmap = null;
            if (texture != null)
                bitmap = texture.Image;

            if (bitmap == null)
                view.textureImage.Source = null;
            else
                view.textureImage.Source = bitmap.GetBitmapSource(BitmapSizeOptions.FromWidthAndHeight(64, 64));
        }

        private void SelectTexture_Click(object sender, RoutedEventArgs e) {
            var wnd = new ChildWindow();
            wnd.Owner = this.GetWindow();

            var textureSelector = new TexturePreviewSelector();
            textureSelector.Canceled += (s, ea) => {
                wnd.Close();
            };
            textureSelector.TextureSelected += (s, ea) => {
                wnd.Close();
                Texture = ea.Value;
            };

            wnd.Content = textureSelector;
            wnd.Show();
        }
    }
}

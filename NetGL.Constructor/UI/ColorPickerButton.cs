using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using NetGL.Core.Mathematics;

namespace NetGL.Constructor.UI {
    public class ColorPickerButton : ButtonBase {
        public static readonly DependencyProperty ColorProperty =
            DependencyProperty.Register("Color", typeof(Vector4), typeof(ColorPickerButton), new PropertyMetadata(Vector4.One, PropertyChangedCallback));

        private readonly Border _border;

        public Vector4 Color {
            get { return (Vector4)GetValue(ColorProperty); }
            set { SetValue(ColorProperty, value); }
        }

        public ColorPickerButton() {
            Content = _border = new Border();

            _border.Margin = new Thickness(0);
            _border.Background = this.Color.AsBrush();
        }

        protected override void OnClick() {
            var rgbColor = this.Color;
            var picker = new ColorPicker(rgbColor);
            var popup = new RegularPopup(picker);
            popup.Closed += (s, ea) => {
                this.Color = rgbColor = picker.SelectedRGBAColor;
                _border.Background = this.Color.AsBrush();
            };
        }

        private static void PropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            var view = (ColorPickerButton)d;
            if (e.Property == ColorProperty) {
                view._border.Background = view.Color.AsBrush();
                return;
            }
        }
    }
}

using System.Windows;
using System.Windows.Controls;
using NetGL.Core.Mathematics;

namespace NetGL.Constructor.Inspectors {
    public partial class Vector3Editor : UserControl {
        public static readonly DependencyProperty VectorProperty =
            DependencyProperty.Register("Vector", typeof(Vector3), typeof(Vector3Editor), new PropertyMetadata(Vector3.Zero, PropertyChangedCallback));

        private bool _isRefreshing = false;
        private Vector3 _vector;

        public Vector3 Vector {
            get { return _vector; }
            set {
                if (_vector == value)
                    return;

                _vector = value;
                Refresh();
                SetValue(VectorProperty, value);
            }
        }

        public Vector3Editor() {
            InitializeComponent();

            Refresh();
        }

        private static void PropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            var view = (Vector3Editor)d;

            if (e.Property == VectorProperty) {
                var value = e.NewValue != null ? (Vector3)e.NewValue : Vector3.Zero;
                view._vector = value;
                view.Refresh();
                return;
            }
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e) {
            if (_isRefreshing)
                return;

            var vec = Vector;
            vec.X = GetValue(vec.X, PosX.Text);
            vec.Y = GetValue(vec.Y, PosY.Text);
            vec.Z = GetValue(vec.Z, PosZ.Text);
            Vector = vec;
        }
        private void TextBox_LostFocus(object sender, RoutedEventArgs e) {
            Refresh();
        }
        private void ResetPosition_Click(object sender, RoutedEventArgs e) {
            Vector = Vector3.Zero;
        }

        private float GetValue(float current, string input) {
            float value;
            if (float.TryParse(input, out value))
                return value;
            else
                return current;
        }
        private void Refresh() {
            _isRefreshing = true;

            if (PosX.IsFocused == false)
                PosX.Text = _vector.X.ToString("R");
            if (PosY.IsFocused == false)
                PosY.Text = _vector.Y.ToString("R");
            if (PosZ.IsFocused == false)
                PosZ.Text = _vector.Z.ToString("R");

            _isRefreshing = false;
        }
    }
}
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using NetGL.Core.Mathematics;
using NetGL.SceneGraph.Scene;

namespace NetGL.Constructor.Inspectors {
    public partial class TransformInspector : UserControl {
        public static readonly DependencyProperty TransformProperty =
            DependencyProperty.Register("Transform", typeof(Transform), typeof(TransformInspector), new PropertyMetadata(null, DependencyPropertyChangedCallback));

        private Transform _transform;
        private float _prevX;
        private bool _isRefreshing = false;
        private readonly DispatcherTimer _timer;
        private Vector3 _displayRotation;
        private Quaternion _displayRotationQuaternion;

        private Vector3 DisplayRotation {
            get {
                if (_displayRotationQuaternion != _transform.LocalRotation) {
                    _displayRotationQuaternion = _transform.LocalRotation;
                    _displayRotation = _displayRotationQuaternion.ToYawPitchRoll();
                }

                return _displayRotation;
            }
            set {
                _displayRotation = value;
                _displayRotationQuaternion = _transform.LocalRotation = Quaternion.CreateFromYawPitchRoll(_displayRotation * MathF.PI);
            }
        }

        public Transform Transform {
            get { return _transform; }
            set {
                if (_transform != value)
                    SetValue(TransformProperty, value);
            }
        }

        public TransformInspector() {
            InitializeComponent();

            if (DesignerProperties.GetIsInDesignMode(this))
                return;

            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(0.25);

            _timer.Tick += (s, ea) => { Refresh(); };
            Loaded += (s, ea) => { _timer.Start(); };
            Unloaded += (s, ea) => { _timer.Stop(); };
        }

        private void Refresh() {
            if (expander.IsExpanded == false)
                return;

            if (_transform == null) {
                PosX.Text = string.Empty;
                PosY.Text = string.Empty;
                PosZ.Text = string.Empty;

                RotX.Text = string.Empty;
                RotY.Text = string.Empty;
                RotZ.Text = string.Empty;

                ScaleX.Text = string.Empty;
                ScaleY.Text = string.Empty;
                ScaleZ.Text = string.Empty;

                return;
            }

            _isRefreshing = true;

            var pos = _transform.LocalPosition;
            var sca = _transform.LocalScale;
            var rot = DisplayRotation;

            if (!PosX.IsFocused)
                PosX.Text = pos.X.ToString("R");
            if (!PosY.IsFocused)
                PosY.Text = pos.Y.ToString("R");
            if (!PosZ.IsFocused)
                PosZ.Text = pos.Z.ToString("R");

            if (!RotX.IsFocused)
                RotX.Text = rot.X.ToString("R");
            if (!RotY.IsFocused)
                RotY.Text = rot.Y.ToString("R");
            if (!RotZ.IsFocused)
                RotZ.Text = rot.Z.ToString("R");

            if (!ScaleX.IsFocused)
                ScaleX.Text = sca.X.ToString("R");
            if (!ScaleY.IsFocused)
                ScaleY.Text = sca.Y.ToString("R");
            if (!ScaleZ.IsFocused)
                ScaleZ.Text = sca.Z.ToString("R");

            _isRefreshing = false;
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e) {
            if (_isRefreshing)
                return;
            if (_transform == null)
                return;

            var position = _transform.LocalPosition;
            position.X = GetValue(position.X, PosX.Text);
            position.Y = GetValue(position.Y, PosY.Text);
            position.Z = GetValue(position.Z, PosZ.Text);

            var rot = DisplayRotation;
            rot.X = GetValue(rot.X, RotX.Text);
            rot.Y = GetValue(rot.Y, RotY.Text);
            rot.Z = GetValue(rot.Z, RotZ.Text);

            var scale = _transform.LocalScale;
            scale.X = GetValue(scale.X, ScaleX.Text);
            scale.Y = GetValue(scale.Y, ScaleY.Text);
            scale.Z = GetValue(scale.Z, ScaleZ.Text);

            _transform.LocalPosition = position;
            DisplayRotation = rot;
            _transform.LocalScale = scale;
        }
        private void TextBox_LostFocus(object sender, RoutedEventArgs e) {
            Refresh();
        }

        private float GetValue(float current, string input) {
            float value;
            if (float.TryParse(input, out value))
                return value;
            else
                return current;
        }

        private void TextBlock_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
            if (_transform == null)
                return;

            var tb = (TextBlock)sender;
            tb.CaptureMouse();

            _prevX = (float)Mouse.GetPosition(tb).X;
            tb.PreviewMouseMove += TextBlock_PreviewMouseMove;
            tb.MouseLeftButtonUp += TextBlock_MouseLeftButtonUp;
        }
        private void TextBlock_PreviewMouseMove(object sender, MouseEventArgs e) {
            var tb = (TextBlock)sender;
            var x = (float)Mouse.GetPosition(tb).X;
            var delta = x - _prevX;
            _prevX = x;

            var pos = _transform.LocalPosition;
            var sca = _transform.LocalScale;
            var rot = _displayRotation;

            switch (tb.Name) {
                case "PosXHeader":
                    pos.X += delta / 100;
                    break;
                case "PosYHeader":
                    pos.Y += delta / 100;
                    break;
                case "PosZHeader":
                    pos.Z += delta / 100;
                    break;

                case "RotXHeader":
                    rot.X += delta / 100;
                    break;
                case "RotYHeader":
                    rot.Y += delta / 100;
                    break;
                case "RotZHeader":
                    rot.Z += delta / 100;
                    break;

                case "ScaleXHeader":
                    sca.X += delta / 100;
                    break;
                case "ScaleYHeader":
                    sca.Y += delta / 100;
                    break;
                case "ScaleZHeader":
                    sca.Z += delta / 100;
                    break;

                default:
                    throw new NotImplementedException();
            }

            _transform.LocalPosition = pos;
            DisplayRotation = rot;
            _transform.LocalScale = sca;

            Refresh();
        }

        private void TextBlock_MouseLeftButtonUp(object sender, MouseButtonEventArgs e) {
            var tb = (TextBlock)sender;
            tb.ReleaseMouseCapture();

            tb.PreviewMouseMove -= TextBlock_PreviewMouseMove;
            tb.MouseLeftButtonUp -= TextBlock_MouseLeftButtonUp;
        }

        private void ResetScale_Click(object sender, RoutedEventArgs e) {
            _transform.LocalScale = Vector3.One;
            Refresh();
        }
        private void ResetPosition_Click(object sender, RoutedEventArgs e) {
            _transform.LocalPosition = Vector3.Zero;
            Refresh();
        }
        private void ResetRotation_Click(object sender, RoutedEventArgs e) {
            _displayRotation = Vector3.Zero;
            _transform.LocalRotation = _displayRotationQuaternion = Quaternion.Identity;
            Refresh();
        }

        private static void DependencyPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            var view = (TransformInspector)d;

            if (e.Property == TransformProperty) {
                var value = (Transform)e.NewValue;

                view._transform = value;
                if (value != null) {
                    view._displayRotation = value.LocalRotation.ToYawPitchRoll() / MathF.PI;
                    view._displayRotationQuaternion = value.LocalRotation;
                }

                view.Refresh();

                return;
            }
        }
    }
}

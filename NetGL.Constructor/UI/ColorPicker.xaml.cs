using System;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using NetGL.Constructor.Infrastructure;
using NetGL.Core.Infrastructure;
using NetGL.Core.Mathematics;
using Color = System.Windows.Media.Color;
using PixelFormat = System.Drawing.Imaging.PixelFormat;

namespace NetGL.Constructor.UI {
    public partial class ColorPicker : UserControl, IDisposable {
        private Bitmap _bmp;
        private int _size;
        private Vector4 _selectedHSBColor;
        private bool _isUpdatingTextBoxes = false;
        private readonly ObservableCollection<Color> _colors = new ObservableCollection<Color>();

        public ICommand PredefinedColorSelected {
            get {
                return new Command(PredefinedColorClicked);
            }
        }

        public Vector4 SelectedHSBAColor {
            get {
                return _selectedHSBColor;
            }
            set {
                if (_selectedHSBColor == value)
                    return;

                _selectedHSBColor = value;

                UpdateUI();
            }
        }
        public Vector4 SelectedRGBAColor {
            get {
                return SelectedHSBAColor.Hsba2Rgba();
            }
            set {
                SelectedHSBAColor = value.Rgba2Hsba();
            }
        }

        public ColorPicker() {
            _selectedHSBColor = new Vector4(0, 1, 1, 1);

            InitializeComponent();

            Loaded += OnLoaded;
        }
        public ColorPicker(Vector4 rgbaColor)
            : this() {
            _colors.Add(rgbaColor.AsColor1());
            _selectedHSBColor = rgbaColor.Rgba2Hsba();
        }

        private void RgbTextBox_TextChanged(object sender, TextChangedEventArgs e) {
            if (_isUpdatingTextBoxes)
                return;

            int v;
            var color = SelectedRGBAColor;

            if (int.TryParse(rTextBox.Text, out v))
                color.X = v / 255f;
            if (int.TryParse(gTextBox.Text, out v))
                color.Y = v / 255f;
            if (int.TryParse(bTextBox.Text, out v))
                color.Z = v / 255f;
            if (int.TryParse(aTextBox.Text, out v))
                color.W = v / 255f;

            ClampVectorComponents(ref color);
            SelectedRGBAColor = color;
        }
        private void ImageHostControl_MouseLeftButtonUp(object sender, MouseButtonEventArgs e) {
            SelectColor(e);
        }
        private void ImageHostControl_MouseMove(object sender, MouseEventArgs e) {
            if (e.LeftButton != MouseButtonState.Pressed)
                return;

            SelectColor(e);
        }

        private void OnLoaded(object sender, RoutedEventArgs e) {
            RenderPickerImage();
            UpdateUI();

            imageHostControl.SizeChanged += OnSizeChanged;
            imageHostControl.MouseMove += ImageHostControl_MouseMove;
            imageHostControl.MouseLeftButtonUp += ImageHostControl_MouseLeftButtonUp;

            opacitySlider.ValueChanged += SliderValueChanged;
            brightnessSlider.ValueChanged += SliderValueChanged;

            rTextBox.TextChanged += RgbTextBox_TextChanged;
            gTextBox.TextChanged += RgbTextBox_TextChanged;
            bTextBox.TextChanged += RgbTextBox_TextChanged;
            aTextBox.TextChanged += RgbTextBox_TextChanged;

            AddPredefinedColors();
            colorsItemsControl.ItemsSource = _colors;
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs e) {
            RenderPickerImage();
        }
        private void SliderValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e) {
            _selectedHSBColor.Z = (float)brightnessSlider.Value;
            _selectedHSBColor.W = (float)opacitySlider.Value;

            UpdateUI();
        }

        private void PredefinedColorClicked(object obj) {
            SelectedRGBAColor = ((Color)obj).AsVector4();
        }

        private void AddPredefinedColors() {
            _colors.Add(Colors.White);
            _colors.Add(Colors.Black);
            _colors.Add(Colors.Red);
            _colors.Add(Colors.Green);
            _colors.Add(Colors.Blue);
            _colors.Add(Colors.Yellow);
            _colors.Add(Colors.Orange);
            _colors.Add(Colors.Purple);
            _colors.Add(Colors.Pink);
            _colors.Add(Colors.Gray);
        }

        private void UpdateUI() {
            UpdateTextboxes();

            var saturated = (Vector3)_selectedHSBColor;
            saturated.Z = 1;
            saturated = saturated.Hsb2Rgb();

            selectedColorBorder.Background = _selectedHSBColor.Hsba2Rgba().AsBrush();
            saturationSliderBackground.Background = new LinearGradientBrush(saturated.AsColor1(), Colors.Black, 90);
            opacitySliderBackground.Background = new LinearGradientBrush(saturated.AsColor1(), Colors.Transparent, 90);
            opacitySlider.Value = _selectedHSBColor.W;
            brightnessSlider.Value = _selectedHSBColor.Z;
        }
        private void UpdateTextboxes() {
            _isUpdatingTextBoxes = true;
            var color = _selectedHSBColor.Hsba2Rgba();
            rTextBox.Text = ((byte)(color.X * 255)).ToString();
            gTextBox.Text = ((byte)(color.Y * 255)).ToString();
            bTextBox.Text = ((byte)(color.Z * 255)).ToString();
            aTextBox.Text = ((byte)(color.W * 255)).ToString();
            _isUpdatingTextBoxes = false;
        }

        private void SelectColor(MouseEventArgs e) {
            var position = e.GetPosition(colorPickerImage).AsVector2() / _size;
            var pickedColor = GetHSLColor(position, true);
            pickedColor.W = (float)opacitySlider.Value;
            pickedColor.Z = (float)brightnessSlider.Value;
            SelectedHSBAColor = pickedColor;
        }
        private void RenderPickerImage(bool force = false) {
            var newSize = (int)Math.Min(imageHostControl.ActualWidth, imageHostControl.ActualHeight) - 13;
            if (newSize < 1)
                return;
            if (_size == newSize)
                return;

            _size = newSize;

            if (_size < 1)
                return;
            if (_bmp != null)
                _bmp.Dispose();

            _bmp = new Bitmap(_size, _size, PixelFormat.Format32bppArgb);
            var data = _bmp.LockBits(new Rectangle(0, 0, _bmp.Width, _bmp.Height), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);

            Parallel.For(0, _size, (line) => { RenderLine(data, line); });
            //for (int i = 0; i < _size; i++)
            //    RenderLine(data, i);

            _bmp.UnlockBits(data);

            colorPickerImage.Source = _bmp.GetBitmapSource(BitmapSizeOptions.FromEmptyOptions());
        }
        private unsafe void RenderLine(BitmapData bmpData, int line) {
            var pointer = (byte*)bmpData.Scan0;

            for (int j = 0; j < _size; j++) {
                var point = new Vector2(j / (float)_size, line / (float)_size);
                var color = GetHSLColor(point).Hsba2Rgba();
                var offset = line * bmpData.Stride + 4 * j;

                SetColor(pointer, color, offset);
            }
        }
        private Vector4 GetHSLColor(Vector2 point, bool clampToRadius = false) {
            const float RadiusSquared = 0.25f;

            var direction = point - new Vector2(0.5f);
            var distandeSq = direction.LengthSquared;
            if (distandeSq > RadiusSquared) {
                if (clampToRadius)
                    distandeSq = RadiusSquared;
                else
                    return new Vector4(1, 0, 1, 0);
            }

            var dot = Vector2.Dot(direction, new Vector2(0, 1)) / direction.Length;
            var hue = 0.5f * MathF.Acos(dot) / MathF.PI;
            if (direction.X < 0)
                hue = 1 - hue;

            var saturation = distandeSq / RadiusSquared;

            var opacity = 1f;
            var distanceToEdge = Math.Abs(distandeSq - RadiusSquared);
            if (distanceToEdge < 0.005f)
                opacity = distanceToEdge * 200;

            return new Vector4(hue, saturation, 1, opacity);
        }

        private static unsafe void SetColor(byte* pointer, Vector4 color, int offset) {
            color = color * 255;
            if (BitConverter.IsLittleEndian) {
                pointer[offset + 0] = (byte)color.Z;
                pointer[offset + 1] = (byte)color.Y;
                pointer[offset + 2] = (byte)color.X;
                pointer[offset + 3] = (byte)color.W;
            }
            else {
                pointer[offset + 0] = (byte)color.X;
                pointer[offset + 1] = (byte)color.Y;
                pointer[offset + 2] = (byte)color.Z;
                pointer[offset + 3] = (byte)color.W;
            }
        }

        private static void ClampVectorComponents(ref Vector3 v) {
            v.X = MathF.Clamp01(v.X);
            v.Y = MathF.Clamp01(v.Y);
            v.Z = MathF.Clamp01(v.Z);
        }
        private static void ClampVectorComponents(ref Vector4 v) {
            v.X = MathF.Clamp01(v.X);
            v.Y = MathF.Clamp01(v.Y);
            v.Z = MathF.Clamp01(v.Z);
            v.W = MathF.Clamp01(v.W);
        }
        private static void ClampVectorComponents(ref Vector2 v) {
            v.X = MathF.Clamp01(v.X);
            v.Y = MathF.Clamp01(v.Y);
        }

        public void Dispose() {
            Disposer.Dispose(ref _bmp);
        }
    }
}


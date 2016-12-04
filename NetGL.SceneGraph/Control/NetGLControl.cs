using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using NetGL.Core.Mathematics;
using NetGL.SceneGraph.Scene;
using System.Threading;
using NetGL.Core.Infrastructure;
using NetGL.SceneGraph.OpenGL;
using NetGL.Core;
using Graphics = NetGL.SceneGraph.OpenGL.Graphics;

namespace NetGL.SceneGraph.Control {
    public sealed class NetGLControl : UserControl, INetGLControl {
        private readonly object _locker = new object();

        private bool _isDisposed = false;
        private bool _canAddNewFrame = true;
        private RenderTimer _timer;

        private readonly Scene.Scene _scene;

        private bool _isRealtime = true;
        private int _targetFrameRate = 50;

        public Scene.Scene Scene {
            get { return _scene; }
        }
        public RenderDispatcher RenderDispatcher { get; private set; }
        internal Graphics Graphics { get; private set; }

        public bool IsRealTime {
            get {
                return _isRealtime;
            }
            set {
                if (value == true)
                    value = RenderDispatcher != null;

                if (_isRealtime == value)
                    return;

                _isRealtime = value;

                StartRenderLoop();
            }
        }
        public int TargetFrameRate {
            get { return _targetFrameRate; }
            set {
                if (value < 1 || value > 1000)
                    throw new ArgumentOutOfRangeException("TargetFrameRate");

                _targetFrameRate = value;
                if (_timer != null)
                    _timer.Interval = (int)(1000f / value);
            }
        }

        public NetGLControl() {
            if (DesignMode) {
                AddLabel("3D will be here");
                return;
            }

            var time = new SceneTime();
            Graphics graphics = null;
            try {
                graphics = new Graphics(this, time, ShadingTechnique.Forward);
            }
            catch (GLException ex) {
                Log.Exception(ex);

                AddLabel("Failed to initialize 3D: " + ex.Message);
                return;
            }
            Graphics = graphics;
            _scene = new Scene.Scene(this, graphics, time);

            RenderDispatcher = RenderDispatcher.Current;

            SetStyle(ControlStyles.UserPaint, false);
            StartRenderLoop();
        }

        private void AddLabel(string text) {
            var label = new Label();
            label.Text = text;

            label.AutoSize = false;
            label.TextAlign = ContentAlignment.MiddleCenter;
            label.Dock = DockStyle.Fill;
            Controls.Add(label);
        }

        protected override void OnSizeChanged(EventArgs e) {
            if (_isDisposed)
                return;

            base.OnSizeChanged(e);

            if (_scene != null)
                _scene.ScreenSize = this.ClientSize;
        }

        protected override sealed void OnPaint(PaintEventArgs pea) {
            // Замещаем методы OnPaint и OnSizeChanged класса Control, 
            // соответствеющие обработчикам WM_PAINT и WM_SIZE
            pea.Graphics.FillRectangle(Brushes.Bisque, pea.ClipRectangle);
        }
        protected override sealed void OnPaintBackground(PaintEventArgs pea) {
            //Окно OpenGL не должно позволять Windows стирать свой фон
            pea.Graphics.FillRectangle(Brushes.Bisque, pea.ClipRectangle);
        }

        private void StartRenderLoop() {
            if (RenderDispatcher != null)
                RenderDispatcher.Render -= Frame;
            Disposer.Dispose(ref _timer);

            if (_isRealtime && RenderDispatcher != null)
                this.RenderDispatcher.Render += Frame;
            else {
                _timer = new RenderTimer();
                _timer.Interval = 1000 / _targetFrameRate;
                var frame = new Action(Frame);
                _timer.Render += (sender) => {
                    lock (_locker) {
                        if (_canAddNewFrame == false)
                            return;
                        _canAddNewFrame = false;
                    }
                    BeginInvoke(frame);
                };
                _timer.Start();
            }
        }
        public void Frame() {
            if (_isDisposed)
                throw new ObjectDisposedException("NetGLControl");

            _scene.Frame();

            lock (_locker)
                _canAddNewFrame = true;
        }

        public Bitmap ReadImage() {
            return Graphics.ReadImage();
        }

        protected override void Dispose(bool disposing) {
            if (disposing == false)
                return;

            if (_isDisposed)
                return;
            _isDisposed = true;

            if (this.RenderDispatcher != null)
                this.RenderDispatcher.Render -= Frame;

            Disposer.Dispose(_timer);
            Disposer.Dispose(_scene);

            base.Dispose(disposing);
        }

        #region mouse
        private Vector2 _screenMousePosition;

        public Vector2 ScreenMousePositionDelta {
            get;
            private set;
        }
        public Vector2 ScreenMousePosition {
            get {
                return _screenMousePosition;
            }
            set {
                var screenPos = this.PointToScreen(value.ToPoint());
                Cursor.Position = screenPos;
            }
        }
        public Vector2 ViewportMousePosition {
            get {
                return _screenMousePosition / new Vector2(this.ClientSize);
            }
            set {
                var clientPos = value * new Vector2(this.ClientSize);
                var screenPos = this.PointToScreen(clientPos.ToPoint());
                Cursor.Position = screenPos;
            }
        }
        public MouseButtons PressedMouseButtons {
            get {
                return MouseButtons;
            }
        }
        public bool IsMouseOver {
            get {
                return this.Focused && ClientRectangle.Contains(PointToClient(MousePosition));
            }
        }

        protected override void OnMouseMove(MouseEventArgs e) {
            var screenPos = new Vector2(e.Location);

            if (_screenMousePosition == screenPos)
                return;

            ScreenMousePositionDelta = _screenMousePosition - screenPos;
            _screenMousePosition = screenPos;

            base.OnMouseMove(e);
        }
        #endregion

        #region keystates
        [Flags]
        public enum KeyStates {
            None = 0,
            Down = 1,
            Toggled = 2
        }
        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true, EntryPoint = "GetKeyState")]
        private static extern short GetKeyState(int keyCode);

        private static KeyStates GetKeyState(Keys key) {
            KeyStates state = KeyStates.None;

            short retVal = GetKeyState((int)key);

            //If the high-order bit is 1, the key is down
            //otherwise, it is up.
            if ((retVal & 0x8000) == 0x8000)
                state |= KeyStates.Down;

            //If the low-order bit is 1, the key is toggled.
            if ((retVal & 1) == 1)
                state |= KeyStates.Toggled;

            return state;
        }
        public bool IsKeyDown(Keys key) {
            if (!Focused)
                return false;

            return KeyStates.Down == (GetKeyState(key) & KeyStates.Down);
        }
        public bool IsKeyToggled(Keys key) {
            if (!Focused)
                return false;

            return KeyStates.Toggled == (GetKeyState(key) & KeyStates.Toggled);
        }

        protected override void OnKeyDown(KeyEventArgs e) {
            base.OnKeyDown(e);
        }
        protected override void OnKeyUp(KeyEventArgs e) {
            base.OnKeyUp(e);
        }
        #endregion
    }
}
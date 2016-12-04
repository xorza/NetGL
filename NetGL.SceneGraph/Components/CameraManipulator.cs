using System.Runtime.InteropServices;
using System.Windows.Forms;
using NetGL.Core.Mathematics;
using NetGL.SceneGraph.Control;
using NetGL.SceneGraph.Scene;

namespace NetGL.SceneGraph.Components {
    [Guid("38A1C547-A9D8-42D3-AD5D-A37CD66907AE")]
    public class CameraManipulator : Component, IUpdatable {
        private INetGLControl _control;

        private Vector2 _prevMousePosition;
        private Vector3 _rotation;

        [NumberRange(0.1f, 20)]
        public float MouseSensivity {
            get;
            set;
        }
        [NumberRange(0.1f, 20)]
        public float KeyboardSensivity {
            get;
            set;
        }

        public CameraManipulator(Node owner)
            : base(owner) {
            MouseSensivity = 4f;
            KeyboardSensivity = 2;
        }

        protected override void OnInit() {
            _control = Scene.Control;
        }
        protected override void OnStart() {
            _rotation = Transform.LocalRotation.ToYawPitchRoll();

            Scene.Control.MouseMove += Control_MouseMove;
            Scene.Control.MouseDown += Control_MouseDown;
        }
        public void Update() {
            Move();
        }
        protected override void OnDispose() {
            Scene.Control.MouseMove -= Control_MouseMove;
            Scene.Control.MouseDown -= Control_MouseDown;
        }

        private void Control_MouseDown(object sender, MouseEventArgs e) {
            _prevMousePosition = _control.ScreenMousePosition;
        }
        private void Control_MouseMove(object sender, MouseEventArgs e) {
            if ((e.Button & MouseButtons.Middle) != MouseButtons.Middle)
                return;

            var mousePosition = _control.ScreenMousePosition;
            var deltaPosition = MouseSensivity * (mousePosition - _prevMousePosition) / new Vector2(_control.Size);
            _prevMousePosition = mousePosition;

            _rotation.X -= deltaPosition.Y;
            _rotation.Y -= deltaPosition.X;
            _rotation.X = MathF.Clamp(_rotation.X, -MathF.PI / 2, MathF.PI / 2);
            Transform.LocalRotation = Quaternion.CreateFromYawPitchRoll(_rotation);
        }

        private void Move() {
            var deltaPos = new Vector3(0);

            if (_control.IsKeyDown(Keys.D))
                deltaPos.X += 1;
            if (_control.IsKeyDown(Keys.A))
                deltaPos.X -= 1;
            if (_control.IsKeyDown(Keys.W))
                deltaPos.Z -= 1;
            if (_control.IsKeyDown(Keys.S))
                deltaPos.Z += 1;
            if (_control.IsKeyDown(Keys.Q))
                deltaPos.Y -= 1;
            if (_control.IsKeyDown(Keys.E))
                deltaPos.Y += 1;

            if (deltaPos.Length < 0.5f)
                return;

            deltaPos = Vector3
                .Transform(deltaPos, Transform.LocalRotation)
                .Normalized;
            Transform.LocalPosition += deltaPos * Time.Delta * KeyboardSensivity;
        }
    }
}

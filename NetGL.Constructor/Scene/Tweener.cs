using NetGL.Core.Mathematics;
using NetGL.SceneGraph.Scene;
using NetGL.SceneGraph.Tweens;

namespace NetGL.Constructor.Scene {
    public class Tweener : Component {
        private ITween _tween;

        public Tweener(Node owner) : base(owner) { }

        protected override void OnStart() {
            base.OnStart();

            Transform.LocalPosition = new Vector3(0, 1.3f, 0);
            ForwardTween();
        }
        protected override void OnDispose() {
            base.OnDispose();

            Scene.Tweens.Cancel(Transform);
        }

        private void ForwardTween() {
            _tween = new ShakeTween(Time, Transform, 1, Vector3.One);
            Scene.Tweens.Add(_tween);
        }
    }
}

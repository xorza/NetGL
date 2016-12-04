using System.Collections.Generic;

namespace NetGL.SceneGraph.Tweens {
    public class TweenCollection {
        private readonly List<ITween> _tweens = new List<ITween>(20);

        public TweenCollection() { }

        public void Add(ITween tween) {
            Cancel(tween.CancellationToken);
            _tweens.Add(tween);
        }
        public void Cancel(object cancellationToken) {
            if (ReferenceEquals(cancellationToken, null))
                return;

            for (int i = _tweens.Count - 1; i >= 0; i--)
                if (ReferenceEquals(_tweens[i].CancellationToken, cancellationToken)) {
                    _tweens.RemoveAt(i);
                    return;
                }
        }
        public void Update() {
            for (int i = _tweens.Count - 1; i >= 0; i--) {
                var tween = _tweens[i];
                var moveNext = tween.MoveNext();

                if (moveNext == false && ReferenceEquals(tween, _tweens[i]))
                    _tweens.RemoveAt(i);
            }
        }
    }
}

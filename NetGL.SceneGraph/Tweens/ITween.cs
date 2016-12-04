
namespace NetGL.SceneGraph.Tweens {
    public interface ITween {
        object CancellationToken { get; }

        bool MoveNext();
    }
}

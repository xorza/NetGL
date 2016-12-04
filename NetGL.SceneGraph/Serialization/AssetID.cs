using System;

namespace NetGL.SceneGraph.Serialization {
    internal class AssetID : IEquatable<AssetID> {
        public Guid Guid { get; set; }

        public AssetID() {
            Guid = Guid.NewGuid();
        }

        public override string ToString() {
            return Guid.ToString();
        }

        public override bool Equals(object obj) {
            var id = obj as AssetID;
            if (id == null)
                return false;

            return Equals(id);
        }
        public override int GetHashCode() {
            return Guid.GetHashCode();
        }

        public bool Equals(AssetID other) {
            if (other == null)
                return false;
            return this.Guid.Equals(other.Guid);
        }
    }
}

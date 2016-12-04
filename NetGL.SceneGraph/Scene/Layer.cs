namespace NetGL.SceneGraph.Scene {
    public struct Layer {
        private readonly int _mask;

        public static Layer All {
            get {
                var result = new Layer(~0);
                return result;
            }
        }
        public static Layer Default {
            get {
                var result = new Layer(1);
                return result;
            }
        }
        public static Layer UI {
            get {
                var result = new Layer(2);
                return result;
            }
        }
        public static Layer IgnoreRaycast {
            get {
                var result = (Layer)31;
                return result;
            }
        }

        public int Mask {
            get {
                return _mask;
            }
        }

        public Layer(int mask) {
            _mask = mask;
        }

        private static int GetBitMask(int bit) {
            return 1 << bit;
        }
        public static implicit operator Layer(int bit) {
            Assert.True(bit >= 0 && bit < 32);

            var result = new Layer(GetBitMask(bit));
            return result;
        }

        public static Layer operator +(Layer layer, int bit) {
            Assert.True(bit >= 0 && bit < 32);

            var mask = GetBitMask(bit);
            return new Layer(layer._mask | mask);
        }
        public static Layer operator -(Layer layer, int bit) {
            Assert.True(bit >= 0 && bit < 32);

            var mask = 1 >> bit;
            mask = ~mask;
            return new Layer(layer._mask & mask);
        }

        public bool Intersects(Layer l) {
            return (this._mask & l._mask) != 0;
        }

        public override bool Equals(object obj) {
            if (!(obj is Layer))
                return false;

            var l = (Layer)obj;

            return this._mask.Equals(l._mask);
        }
        public override int GetHashCode() {
            return _mask.GetHashCode();
        }
    }
}
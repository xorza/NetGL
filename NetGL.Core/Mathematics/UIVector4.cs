
namespace NetGL.Core.Mathematics {
    public struct UIVector4 {
        public static UIVector4 Zero { get { return new UIVector4(0); } }
        public static UIVector4 One { get { return new UIVector4(1); } }

        public uint X;
        public uint Y;
        public uint Z;
        public uint W;

        public UIVector4(uint v) {
            X = Y = Z = W = v;
        }
        public UIVector4(uint x, uint y, uint z, uint w) {
            X = x;
            Y = y;
            Z = z;
            W = w;
        }
    }
}

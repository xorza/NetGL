
namespace NetGL.Core.Mathematics {
    public struct IVector4 {
        public static IVector4 Zero { get { return new IVector4(0); } }
        public static IVector4 One { get { return new IVector4(1); } }

        public int X;
        public int Y;
        public int Z;
        public int W;

        public IVector4(int v) {
            X = Y = Z = W = v;
        }
        public IVector4(int x, int y, int z, int w) {
            X = x;
            Y = y;
            Z = z;
            W = w;
        }
    }
}

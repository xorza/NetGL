using System.Runtime.CompilerServices;
namespace NetGL.Core.Mathematics {
    public class Frustum {
        private readonly Plane[] _planes;

        public Frustum() {
            _planes = new Plane[6];
            for (int i = 0; i < 6; i++)
                _planes[i] = new Plane();
        }

        public void LoadFromViewProjectionMatrix(Matrix m) {
            //left 
            _planes[0].A = m.M14 + m.M11;
            _planes[0].B = m.M24 + m.M21;
            _planes[0].C = m.M34 + m.M31;
            _planes[0].D = m.M44 + m.M41;

            //right 
            _planes[1].A = m.M14 - m.M11;
            _planes[1].B = m.M24 - m.M21;
            _planes[1].C = m.M34 - m.M31;
            _planes[1].D = m.M44 - m.M41;

            //bottom 
            _planes[2].A = m.M14 + m.M12;
            _planes[2].B = m.M24 + m.M22;
            _planes[2].C = m.M34 + m.M32;
            _planes[2].D = m.M44 + m.M42;

            //top 
            _planes[3].A = m.M14 - m.M12;
            _planes[3].B = m.M24 - m.M22;
            _planes[3].C = m.M34 - m.M32;
            _planes[3].D = m.M44 - m.M42;

            //near 
            _planes[4].A = m.M13;
            _planes[4].B = m.M23;
            _planes[4].C = m.M33;
            _planes[4].D = m.M43;

            //far 
            _planes[5].A = m.M14 - m.M13;
            _planes[5].B = m.M24 - m.M23;
            _planes[5].C = m.M34 - m.M33;
            _planes[5].D = m.M44 - m.M43;

            for (int i = 0; i < 6; i++)
                _planes[i].Normalize();
        }
        public bool SphereInsideFrustum(Vector3 center, float radius) {
            for (int i = 0; i < 6; i++) {
                var plane = _planes[i];
                var distance = plane.A * center.X + plane.B * center.Y + plane.C * center.Z + plane.D;

                if (distance < -radius)
                    return false;

                //if (distance < radius)
                //    result = intersect;
            }

            return true;
        }
        private static float Distance(Plane p, Vector3 v) {
            return p.A * v.X + p.B * v.Y + p.C * v.Z + p.D;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FbxConverterTypes {
    public class Mesh {
        public List<Vec3> vertices;
        public List<Vec3> normals;
        public List<Vec3> tangents;
        public List<Vec2> uvs;

        public Mesh() {
            vertices = new List<Vec3>();
            normals = new List<Vec3>();
            tangents = new List<Vec3>();
            uvs = new List<Vec2>();
        }
    }
}

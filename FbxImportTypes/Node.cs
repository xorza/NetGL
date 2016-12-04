using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FbxConverterTypes {
    public sealed class Node {
        public String name;
        public Vec3 position;
        public Vec3 rotation;
        public Vec3 scale;
        public List<Node> nodes;
        public Mesh mesh;

        public Node() {
            nodes = new List<Node>();
        }
    }
}

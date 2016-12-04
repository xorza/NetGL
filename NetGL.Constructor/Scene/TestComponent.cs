using NetGL.SceneGraph.Scene;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetGL.Constructor.Scene {
    public class TestComponent : Component {
        public int integer { get; set; }
        public sbyte signedbyte { get; set; }
        public float FLOAT { get; set; }
        public string stringg { get; set; }
        public char char1 { get; set; }
        public List<int> listint { get; set; }

        public TestComponent(Node so) : base(so) { }
    }
}

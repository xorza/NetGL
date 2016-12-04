using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetGL.SceneGraph.UI {
    public abstract class UserControl {
        internal uint ID { get; set; }

        public Size Size { get; set; }
        public System.Drawing.Point Position { get; set; }
        public Color Color { get; set; }
    }
}

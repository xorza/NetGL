using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FbxConverterTypes {
    public interface ISceneLoader {
        Node Load(String filename);
    }
}

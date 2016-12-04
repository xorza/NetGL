using NetGL.SceneGraph.Scene;
using System.Collections.Generic;

namespace NetGL.SceneGraph.Serialization {
    internal class SceneDTO {
        public List<SceneObjectDTO> SceneObjects { get; set; }
        public List<Asset> Assets { get; set; }

        public SceneDTO() {
            SceneObjects = new List<SceneObjectDTO>();
            Assets = new List<Asset>();
        }

        public List<Node> Create(DeserializationContext context) {
            var result = new List<Node>();
            foreach (var soDTO in SceneObjects)
                result.Add(soDTO.Create(context));
            return result;
        }
    }
}
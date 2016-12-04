using NetGL.Core.Types;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetGL.SceneGraph.Serialization {
    internal class MeshAsset : Asset {
        public string Filename { get; set; }
        [JsonIgnore]
        [NotSerialized]
        public Mesh Mesh { get; set; }

        public MeshAsset() { }

        public MeshAsset(SerializationContext context, Mesh mesh) {
            Assert.NotNull(context);
            Assert.NotNull(mesh);

            this.Mesh = mesh;
        }

        public override void PreSerialize(SerializationContext serializationContext) {
            Filename = Path.Combine(serializationContext.WorkingDirectory.FullName, string.Format("{0}.mesh", ID.Guid));

            using (var stream = File.Create(Filename))
                Mesh.ToStream(stream);
            Mesh = null;
        }

        public override void PostDeserialize(DeserializationContext deserializationContext) {
            Filename = Path.Combine(deserializationContext.WorkingDirectory.FullName, string.Format("{0}.mesh", ID.Guid));

            using (var stream = File.OpenRead(Filename)) {
                Mesh = new Mesh();
                Mesh.FromStream(stream);
            }
        }
    }
}

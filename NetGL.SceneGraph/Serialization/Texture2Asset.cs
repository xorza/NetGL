using System;
using System.IO;
using NetGL.Core.Types;
using NetGL.Core.Infrastructure;
using Newtonsoft.Json;
using NetGL.Core;

namespace NetGL.SceneGraph.Serialization {
    internal class Texture2Asset : Asset {
        [NotSerialized]
        [JsonIgnore]
        public Texture2 Texture { get; set; }
        public string Filename { get; set; }
        public string Name { get; set; }
        public bool IsNormalMap { get; set; }
        public int Anisotropy { get; set; }
        public TextureFilter MagFilter { get; set; }
        public TextureFilter MinFilter { get; set; }
        public TextureWrapMode WrapMode { get; set; }

        public Texture2Asset() {
        }

        public Texture2Asset(SerializationContext context, Texture2 texture) {
            Assert.NotNull(texture);

            Filename = texture.Filename;
            Name = texture.Name;
            Texture = texture;
            IsNormalMap = texture.IsNormalMap;
            Anisotropy = texture.Anisotropy;
            MagFilter = texture.MagFilter;
            MinFilter = texture.MinFilter;
            WrapMode = texture.Wrap;
        }

        public override void PreSerialize(SerializationContext serializationContext) {
            if (string.IsNullOrWhiteSpace(Filename)) {
                Filename = Path.Combine(serializationContext.WorkingDirectory.FullName, string.Format("{0}.png", ID.Guid));
                Texture.Image.Save(Filename);
            }
            else {

            }
        }

        public override void PostDeserialize(DeserializationContext deserializationContext) {
            Texture = new Texture2(Filename, IsNormalMap);
            Texture.Anisotropy = Anisotropy;
            Texture.MagFilter = MagFilter;
            Texture.MinFilter = MinFilter;
            Texture.Wrap = WrapMode;           
        }
    }
}
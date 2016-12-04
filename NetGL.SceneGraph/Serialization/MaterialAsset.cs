using System;
using NetGL.SceneGraph.Components;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using NetGL.Core.Types;

namespace NetGL.SceneGraph.Serialization {
    internal class MaterialAsset : Asset {
        [NotSerialized]
        [JsonIgnore]
        public Material Material { get; set; }

        public string VertexCode { get; set; }
        public string FragmentCode { get; set; }
        public RenderQueue RenderQueue { get; set; }
        public BlendingMode BlendMode { get; set; }
        public MaterialType MaterialType { get; set; }

        public Dictionary<string, object> UniformValues { get; set; }
        public Dictionary<string, object> Textures { get; set; }

        public MaterialAsset() { }

        public MaterialAsset(SerializationContext context, Material material) {
            Assert.NotNull(material);

            if (material.MaterialType == Components.MaterialType.Custom) {
                throw new NotImplementedException();
            }
            else {

            }

            this.Material = material;
            this.MaterialType = material.MaterialType;
            this.BlendMode = material.BlendMode;
            this.RenderQueue = material.RenderQueue;

            UniformValues = material.Values
                .ToDictionary(_ => _.Name, _ => context.ConvertValue(_.GetValue()));
            Textures = material.Textures
                  .ToDictionary(_ => _.Name, _ => context.ConvertValue(_.Texture));
        }

        public override void PreSerialize(SerializationContext serializationContext) {
            ;
        }
        public override void PostDeserialize(DeserializationContext deserializationContext) {
            Material = new Material(MaterialType, RenderQueue);
            Material.BlendMode = this.BlendMode;

            Material.Values.ForEach(_ => _.SetValue(deserializationContext.ConvertValue(UniformValues[_.Name], _.UniformType)));
            Material.Textures.ForEach(_ => _.Texture = (Texture)deserializationContext.ConvertValue(Textures[_.Name], typeof(Texture2)));
        }
    }
}
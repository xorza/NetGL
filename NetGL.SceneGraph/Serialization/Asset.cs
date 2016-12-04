using NetGL.Core.Infrastructure;
using NetGL.Core.Types;
using NetGL.SceneGraph.Components;
using Newtonsoft.Json;
using System;
using System.IO;

namespace NetGL.SceneGraph.Serialization {
    internal class Asset {
        public AssetID ID { get; set; }

        public Asset() {
            ID = new AssetID();
        }

        //public Asset() {
        //Assert.NotNull(value);

        //if (value is Mesh) {
        //    Type = AssetType.Mesh;
        //    DTO = new MeshDTO((Mesh)value);
        //}
        //else if (value is Texture2) {
        //    Type = AssetType.Texture2;
        //    DTO = new Texture2DTO((Texture2)value);
        //}
        //else if (value is Material) {
        //    Type = AssetType.Material;
        //    DTO = new MaterialDTO(context, (Material)value);
        //}
        //else
        //    throw new NotSupportedException(value.GetType().ToString());
        //}

        public virtual void PreSerialize(SerializationContext serializationContext) { }
        public virtual void PostDeserialize(DeserializationContext deserializationContext) { }
    }
}

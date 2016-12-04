using NetGL.Core.Infrastructure;
using NetGL.Core.Mathematics;
using NetGL.Core.Types;
using NetGL.SceneGraph.Components;
using NetGL.SceneGraph.Scene;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace NetGL.SceneGraph.Serialization {
    internal delegate object ValueConverterDelegate(object value);
    internal delegate TOut ValueConverterDelegate<TIn, TOut>(TIn value);
    internal delegate Asset AssetConstructor();

    internal class SerializationContext {
        private readonly String _fileName;
        private readonly JsonSerializerSettings _jsonSettings;
        private readonly SceneDTO _sceneDTO;
        private readonly Scene.Scene _sceneRoot;

        private readonly Dictionary<Type, ValueConverterDelegate> _valueMappers = new Dictionary<Type, ValueConverterDelegate>();
        private readonly Dictionary<object, Asset> _assets = new Dictionary<object, Asset>();

        public DirectoryInfo WorkingDirectory { get; private set; }

        internal SerializationContext(Scene.Scene sceneRoot, string filename) {
            AddMapping<Vector2>(v => new Vector2DTO(v));
            AddMapping<Vector3>(v => new Vector3DTO(v));
            AddMapping<Vector4>(v => new Vector4DTO(v));
            AddMapping<Quaternion>(v => new QuaternionDTO(v));
            AddMapping<Layer>(v => v.Mask);
            AddMapping<Mesh>(mesh => AddAsset(mesh, () => new MeshAsset(this, mesh)));
            AddMapping<Material>(mat => AddAsset(mat, () => new MaterialAsset(this, mat)));
            AddMapping<Texture2>(tex => AddAsset(tex, () => new Texture2Asset(this, tex)));

            _fileName = filename;
            WorkingDirectory = new FileInfo(filename).Directory;

            _sceneDTO = new SceneDTO();
            _sceneRoot = sceneRoot;

            _jsonSettings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All };
        }

        private void AddMapping<TIn>(ValueConverterDelegate<TIn, object> dtoConverter) {
            _valueMappers.Add(typeof(TIn), _ => dtoConverter((TIn)_));
        }
        private object UseMapper(Type propType, object value) {
            var mapper = _valueMappers[propType];
            var dto = mapper(value);
            return dto;
        }
        private AssetID AddAsset<T>(T value, AssetConstructor constructor) {
            Asset asset = null;
            if (_assets.TryGetValue(value, out asset) == false) {
                asset = constructor();
                _assets.Add(value, asset);
                _sceneDTO.Assets.Add(asset);
            }

            return asset.ID;
        }

        public void Serialize() {
            _sceneRoot.SceneObjects
                .ForEach(_ => Serialize(_));

            SaveAssets();

            var settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All };
            var json = JsonConvert.SerializeObject(_sceneDTO, Formatting.Indented, settings);

            File.WriteAllText(_fileName, json);
        }
        private void Serialize(Node so) {
            if (so.IsSerialized == false)
                return;

            var dto = new SceneObjectDTO(this, so);
            _sceneDTO.SceneObjects.Add(dto);
        }

        private void SaveAssets() {
            _assets.Clear();

            _sceneDTO.Assets
                .ForEach(_ => _.PreSerialize(this));
        }

        public object ConvertValue(object value) {
            if (value == null)
                throw new ArgumentNullException("value");

            var type = value.GetType();

            if (type.IsEnum)
                return value;
            if (type.IsPrimitive)
                return value;

            if (_valueMappers.ContainsKey(type))
                return UseMapper(type, value);

            Log.Warning(string.Format("Value: {0}, of type: {1} skipped while serializing", value, type));
            return null;
        }
    }
}
using NetGL.Core.Mathematics;
using NetGL.Core.Types;
using NetGL.SceneGraph.Components;
using NetGL.SceneGraph.Scene;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace NetGL.SceneGraph.Serialization {


    internal class DeserializationContext {
        private readonly List<Tuple<Node, Guid>> _needParentObjects = new List<Tuple<Node, Guid>>();
        private readonly Dictionary<AssetID, Asset> _assets = new Dictionary<AssetID, Asset>();

        private readonly Dictionary<Type, ValueConverterDelegate> _valueMappers = new Dictionary<Type, ValueConverterDelegate>();

        private readonly Scene.Scene _sceneRoot;
        private SceneDTO _sceneDTO;
        private readonly JsonSerializerSettings _jsonSettings;
        private readonly String _fileName;

        public NetGL.SceneGraph.Scene.Scene Scene {
            get { return _sceneRoot; }
        }
        public DirectoryInfo WorkingDirectory { get; private set; }

        public DeserializationContext(Scene.Scene scene, string filename) {
            Assert.NotNull(scene);

            _fileName = filename;
            WorkingDirectory = new FileInfo(filename).Directory;

            _sceneRoot = scene;
            _jsonSettings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All };

            AddMapping<Vector2DTO, Vector2>(v => v.ToVector2());
            AddMapping<Vector3DTO, Vector3>(v => v.ToVector3());
            AddMapping<Vector4DTO, Vector4>(v => v.ToVector4());
            AddMapping<QuaternionDTO, Quaternion>(v => v.ToQuaternion());
            AddMapping<Int64, Layer>(v => new Layer((Int32)v));
            AddMapping<AssetID, Mesh>(v => ((MeshAsset)GetAsset(v)).Mesh);
            AddMapping<AssetID, Material>(v => ((MaterialAsset)GetAsset(v)).Material);
            AddMapping<AssetID, Texture2>(v => ((Texture2Asset)GetAsset(v)).Texture);
        }

        private void AddMapping<TIn, TOut>(ValueConverterDelegate<TIn, TOut> dtoConverter) {
            _valueMappers.Add(typeof(TOut), _ => (TOut)dtoConverter((TIn)_));
        }
        public Asset GetAsset(AssetID id) {
            return _assets[id];
        }
        private object UseMapper(Type propType, object value) {
            var mapper = _valueMappers[propType];
            var dto = mapper(value);
            return dto;
        }

        public void NeedParent(Node so, Guid parentID) {
            _needParentObjects.Add(new Tuple<Node, Guid>(so, parentID));
        }
        public void SetReferences(List<Node> deserializedObjects) {
            foreach (var item in _needParentObjects) {
                var so = deserializedObjects.Find(_ => _.Id == item.Item2);
                if (so == null)
                    throw new Exception("Cannot find parent for element");

                item.Item1.Transform.Parent = so.Transform;
            }
        }

        public void Deserialize() {
            var json = File.ReadAllText(_fileName);
            _sceneDTO = JsonConvert.DeserializeObject<SceneDTO>(json, _jsonSettings);

            foreach (var asset in _sceneDTO.Assets) {
                asset.PostDeserialize(this);
                _assets.Add(asset.ID, asset);
            }

            var sceneObjects = _sceneDTO.Create(this);
            SetReferences(sceneObjects);
        }

        public object ConvertValue(object value, Type targetType) {
            if (value == null)
                throw new ArgumentNullException("value");

            var typecode = Type.GetTypeCode(targetType);
            switch (typecode) {
                case TypeCode.SByte:
                    return (sbyte)(Int64)value;
                case TypeCode.Byte:
                    return (byte)(Int64)value;
                case TypeCode.Int16:
                    return (Int16)(Int64)value;
                case TypeCode.Int32:
                    return (Int32)(Int64)value;
                case TypeCode.Single:
                    return (Single)(Double)value;
                case TypeCode.String:
                    return value;
                case TypeCode.UInt16:
                    return (UInt16)(Int64)value;
                case TypeCode.UInt32:
                    return (UInt32)(Int64)value;
                case TypeCode.UInt64:
                    return (UInt64)(Int64)value;
                case TypeCode.Decimal:
                    return (Decimal)(Double)value;
                case TypeCode.Char:
                    return ((string)value)[0];

                case TypeCode.DateTime:
                case TypeCode.Double:
                case TypeCode.Boolean:
                case TypeCode.Int64:
                    return value;

                case TypeCode.Object:
                    break;

                default:
                    throw new NotSupportedException(typecode.ToString());
            }

            if (_valueMappers.ContainsKey(targetType))
                return UseMapper(targetType, value);
                      
            throw new NotSupportedException(targetType.ToString());
        }
    }
}
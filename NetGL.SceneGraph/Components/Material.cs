using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using NetGL.Core.Infrastructure;
using NetGL.Core.Mathematics;
using NetGL.Core.Types;
using NetGL.SceneGraph.OpenGL;
using System.Text.RegularExpressions;

namespace NetGL.SceneGraph.Components {
    public delegate void TextureEventHandler(string textureName, Texture2 texture);

    public enum BlendingMode {
        Normal = 0,
        Additive
    }

    public enum MaterialType {
        FlatColor,
        FlatTextureColor,
        FlatVertexColor,
        DiffuseColor,
        DiffuseColorTexture,
        DiffuseNormalTextureColor,
        RimDiffuseColorTexture,
        ReflectionDiffuseColor,
        RimReflectionDiffuseTextureColor,
        Custom
    }
    public enum RenderQueue {
        Opaque,
        Transparent,
        CustomShaderOpaque
    }
    public enum MaterialOptions {
        NoLighting = 1,
    }

    public class Material : IDisposable {
        public const string MainTextureUniformName = "uniform_MainTexture";
        public const string MainColorUniformName = "uniform_Color";

        private List<TextureUniform> _textures;

        private TextureUniform _mainTextureUnit;
        private Vector4Uniform _mainColorUniform;

        private ShaderProgram _shader;

        internal ShaderProgram Shader {
            get {
                Assert.NotNull(_shader);
                return _shader;
            }
        }

        public Vector4 Color {
            get {
                if (_mainColorUniform == null)
                    return Vector4.One;

                return _mainColorUniform.Value;
            }
            set {
                if (_mainColorUniform == null)
                    return;

                if (RenderQueue == RenderQueue.Opaque)
                    value.W = 1;
                _mainColorUniform.Value = value;
            }
        }
        public Texture MainTexture {
            get {
                if (_mainTextureUnit == null)
                    return null;

                return _mainTextureUnit.Texture;
            }
            set {
                if (_mainTextureUnit == null)
                    return;

                if (ReferenceEquals(_mainTextureUnit.Texture, value))
                    return;

                _mainTextureUnit.Texture = value;
            }
        }
        public RenderQueue RenderQueue { get; private set; }
        public BlendingMode BlendMode { get; set; }
        public ShadingTechnique ShadingTechnique { get; private set; }
        public string Name { get; set; }
        public ReadOnlyCollection<Uniform> Values {
            get {
                return _shader.Uniforms;
            }
        }
        public bool UsesMainTexture {
            get {
                return _mainTextureUnit != null;
            }
        }
        public bool UsesMainColor {
            get {
                return _mainColorUniform != null;
            }
        }
        public IReadOnlyList<TextureUniform> Textures { get; private set; }
        public MaterialType MaterialType { get; private set; }

        public Material(ShaderProgram shader, RenderQueue renderQueue) {
            if (shader == null)
                throw new ArgumentNullException("shader");

            this.RenderQueue = renderQueue;
            this.MaterialType = MaterialType.Custom;
            this.ShadingTechnique = Graphics.GetCurrent(true).CurrentShadingTechnique;

            _shader = shader;

            InitShader();
        }
        public Material(string vertexShaderMethod, string fragmentShaderMethod, RenderQueue renderQueue, MaterialOptions options = 0) {
            this.MaterialType = MaterialType.Custom;
            this.RenderQueue = renderQueue;
            this.ShadingTechnique = Graphics.GetCurrent(true).CurrentShadingTechnique;

            OpenGLShader shader;
            this._shader = shader = new OpenGLShader();
            shader.Code = GetDefaultShaderCode();

            if (options.HasFlag(MaterialOptions.NoLighting) == false)
                shader.Flags |= ShaderDefines.USE_LIGHTING;

            if (string.IsNullOrWhiteSpace(vertexShaderMethod) == false) {
                shader.Flags |= ShaderDefines.CUSTOM_VERTEX;
                shader.Code += string.Format("\n#if defined VERTEX_SHADER\n{0}\n#endif\n", vertexShaderMethod);
            }
            shader.Code += string.Format("\n#if defined FRAGMENT_SHADER\n{0}\n#endif\n", fragmentShaderMethod);
            InitShader();
        }
        public Material(MaterialType material)
            : this(material, RenderQueue.Opaque) { }
        public Material(MaterialType material, RenderQueue renderQueue)
            : this(material, Graphics.GetCurrent(true).CurrentShadingTechnique, renderQueue) { }
        public Material(MaterialType material, ShadingTechnique shadingTechnique, RenderQueue renderQueue) {
            CreateShader(material, shadingTechnique, renderQueue);
        }

        private void CreateShader(MaterialType material, ShadingTechnique shadingTechnique, RenderQueue renderQueue) {
            if (shadingTechnique == ShadingTechnique.Deferred && RenderQueue == RenderQueue.CustomShaderOpaque)
                throw new NotSupportedException("CustomOpaque shaders are not supported yet in Deferred Shading mode.");

            this.RenderQueue = renderQueue;
            this.MaterialType = material;
            this.ShadingTechnique = shadingTechnique;

            OpenGLShader shader;
            this._shader = shader = new OpenGLShader();
            _shader.Code = GetDefaultShaderCode();
            _shader.Code += string.Format("\n#if defined FRAGMENT_SHADER\n{0}\n#endif\n", Resources.StandardShaders);
            shader.Define(material.ToString());

            switch (material) {
                case MaterialType.FlatColor:
                case MaterialType.FlatTextureColor:
                case MaterialType.FlatVertexColor:
                    break;
                case MaterialType.DiffuseColor:
                case MaterialType.DiffuseColorTexture:
                case MaterialType.DiffuseNormalTextureColor:
                case MaterialType.ReflectionDiffuseColor:
                case MaterialType.RimDiffuseColorTexture:
                case MaterialType.RimReflectionDiffuseTextureColor:
                    shader.Flags = ShaderDefines.USE_LIGHTING;
                    break;
                default:
                    throw new NotSupportedException(material.ToString());
            }

            InitShader();
        }
        private void InitShader() {
            if (_shader.IsCompiled == false)
                _shader.Compile();

            this._mainColorUniform = _shader.GetUniform(MainColorUniformName) as Vector4Uniform;
            if (this._mainColorUniform != null)
                this.Color = this._mainColorUniform.Value;

            this._textures = new List<TextureUniform>(this._shader.Textures.Count);
            this.Textures = this._textures.AsReadOnly();

            for (int i = 0; i < this._shader.Textures.Count; i++) {
                var uniform = this._shader.Textures[i];

                var unit = new TextureUniform(uniform);
                _textures.Add(unit);

                if (MainTextureUniformName.Equals(uniform.Name))
                    _mainTextureUnit = unit;
            }
        }

        private string GetDefaultShaderCode() {
            switch (ShadingTechnique) {
                case ShadingTechnique.Forward:
                    return Resources.Forward;
                case ShadingTechnique.Deferred:
                    switch (this.RenderQueue) {
                        case RenderQueue.Opaque:
                            return Resources.Deferred_1pass;
                        case RenderQueue.Transparent:
                            return Resources.Forward;
                        default:
                            throw new NotSupportedException(this.RenderQueue.ToString());
                    }
                default:
                    throw new NotSupportedException(ShadingTechnique.ToString());
            }
        }

        public void SetTexture(string uniformName, Texture texture) {
            Assert.NotNull(texture);
            Assert.NotEmpty(uniformName);

            var unit = _textures.SingleOrDefault(_ => _.Name.Equals(uniformName));
            if (unit == null) {
                Log.Warning("Texture with name " + uniformName + " absent");
                return;
            }

            unit.Texture = texture;
        }
        public Texture GetTexture(string uniformName) {
            var unit = _textures.SingleOrDefault(_ => _.Name.Equals(uniformName));
            if (unit == null)
                return null;
            else
                return unit.Texture;
        }

        public void SetValue(string name, float value) {
            _shader.SetUniform(name, value);
        }
        public void SetValue(string name, Vector3 value) {
            _shader.SetUniform(name, value);
        }

        public void Dispose() {
            Disposer.Dispose(_shader);
        }
    }
}
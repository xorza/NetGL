using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using NetGL.Core.Infrastructure;
using NetGL.Core.Mathematics;
using System.Linq;
using System.Diagnostics;
using System.Text;
using System.Runtime.CompilerServices;
using System.Reflection;

namespace NetGL.Core.Types {
    public enum StandardAttribute : int {
        Absent = -1,
        Position = 0,
        Color,
        Normal,
        TexCoord,
        Tangent,
        Data1,
        Data2,
        Data3
    }

    public class ShaderProgram : UIntObject, IComparable<ShaderProgram> {
        private const string GlobalLightsUniformBlockName = "uniform_GlobalLights";
        private const string StandartUniformBlockName = "uniform_Standart";

        private const string NormalMatrixUniformName = "uniform_NormalMatrix";
        private const string ModelViewMatrixUniformName = "uniform_ModelViewMatrix";
        private const string ModelMatrixUniformName = "uniform_ModelMatrix";
        private const string InversedModelMatrixUniformName = "uniform_InversedModelMatrix";

        private const string PositionAttributeName = "in_Position";
        private const string ColorAttributeName = "in_Color";
        private const string NormalAttributeName = "in_Normal";
        private const string TexCoordAttributeName = "in_TexCoord0";
        private const string TangentAttributeName = "in_Tangent";

        private const string DataAttributeName1 = "in_Data1";
        private const string DataAttributeName2 = "in_Data2";
        private const string DataAttributeName3 = "in_Data3";

        public const uint StandartUniformsBindingPoint = 1;
        public const uint GlobalUniformsBindingPoint = 2;

        private readonly Dictionary<string, Uniform> _uniformsDictionary = new Dictionary<string, Uniform>();
        private readonly List<Uniform> _uniforms = new List<Uniform>();
        private readonly List<IntUniform> _textures = new List<IntUniform>();
        private readonly List<SubroutineUniform> _subroutineUniforms = new List<SubroutineUniform>();
        private uint[] _subroutineFragmentStage, _subroutineVertexStage, _subroutineGeometryStage;

        private readonly StringBuilder _defines = new StringBuilder();
        private string _code;
        private readonly Dictionary<string, Uniform> _propertyUniforms = new Dictionary<string, Uniform>();

        public bool UsesNormalMatrix { get; private set; }
        public bool UsesModelViewMatrix { get; private set; }
        public bool UsesModelMatrix { get; private set; }
        public bool UsesInversedModelMatrix { get; private set; }

        public string Name { get; set; }
        public bool IsCompiled { get; private set; }
        public string Code {
            get {
                return _code;
            }
            set {
                if (IsCompiled)
                    throw new InvalidOperationException("Cannot change code after compilation.");

                _code = value;
            }
        }

        public int WorkGroupSizeX { get; private set; }
        public int WorkGroupSizeY { get; private set; }
        public int WorkGroupSizeZ { get; private set; }

        public ReadOnlyCollection<IntUniform> Textures { get; private set; }
        public ReadOnlyCollection<Uniform> Uniforms { get; private set; }
        public ReadOnlyCollection<SubroutineUniform> Subroutines { get; private set; }

        public string ComputeCode { get; private set; }

        public ShaderProgram()
            : base() {
            this.IsCompiled = false;

            Initialize(Context.CreateProgram());
        }

        public static ShaderProgram CreateCompute(string computeShaderCode) {
            var context = GL.GetCurrent(true);
            if (context.Version < new GLVersion(4, 4))
                throw new GLException("Compute shaders are not supported in OpenGL versions lower than 4.4");

            var result = new ShaderProgram();
            result.ComputeCode = computeShaderCode;

            using (var vertexShader = new Shader(ShaderType.ComputeShader, computeShaderCode))
                result.Context.AttachShader(result.Handle, vertexShader.Handle);

            result.Link();
            result.Context.UseProgram(result.Handle);
            result.InitUniforms();
            result.GetComputeGroupSize();

            result.IsCompiled = true;

            return result;
        }

        public void Bind() {
            if (IsCompiled == false)
                throw new InvalidOperationException("Program is not yet compiled!");

            Context.UseProgram(this.Handle);

            if (_subroutineGeometryStage != null)
                Context.UniformSubroutinesuiv(ShaderType.GeometryShader, _subroutineGeometryStage);
            if (_subroutineFragmentStage != null)
                Context.UniformSubroutinesuiv(ShaderType.FragmentShader, _subroutineFragmentStage);
            if (_subroutineVertexStage != null)
                Context.UniformSubroutinesuiv(ShaderType.VertexShader, _subroutineVertexStage);
        }

        public void SetUniform(string uniformName, float value) {
            var uniform = GetUniform(uniformName);
            if (uniform == null)
                return;

            Assert.True(uniform.Type == ActiveUniformType.Float);
            ((FloatUniform)uniform).Value = value;
        }
        public void SetUniform(string uniformName, Vector2 value) {
            var uniform = GetUniform(uniformName);
            if (uniform == null)
                return;

            Assert.True(uniform.Type == ActiveUniformType.FloatVec2);
            ((Vector2Uniform)uniform).Value = value;
        }
        public void SetUniform(string uniformName, Vector3 value) {
            var uniform = GetUniform(uniformName);
            if (uniform == null)
                return;

            Assert.True(uniform.Type == ActiveUniformType.FloatVec3);
            ((Vector3Uniform)uniform).Value = value;
        }
        public void SetUniform(string uniformName, Vector4 value) {
            var uniform = GetUniform(uniformName);
            if (uniform == null)
                return;

            Assert.True(uniform.Type == ActiveUniformType.FloatVec4);
            ((Vector4Uniform)uniform).Value = value;
        }
        public void SetUniform(string uniformName, int value) {
            var uniform = GetUniform(uniformName);
            if (uniform == null)
                return;

            switch (uniform.Type) {
                case ActiveUniformType.Int:
                case ActiveUniformType.Sampler2D:
                case ActiveUniformType.Sampler2DShadow:
                    break;
                default:
                    throw new ArgumentException(string.Format("Uniform {0} is of type {1}", uniformName, uniform.Type));
            }

            ((IntUniform)uniform).Value = value;
        }
        public void SetUniform(string uniformName, Matrix value) {
            var uniform = GetUniform(uniformName);
            if (uniform == null)
                return;

            Assert.True(uniform.Type == ActiveUniformType.FloatMat4);
            ((Matrix4Uniform)uniform).Value = value;
        }

        public bool HasUniform(string uniformName) {
            return _uniformsDictionary.ContainsKey(uniformName);
        }
        public Uniform GetUniform(string uniformName) {
            Uniform uniform = null;
            _uniformsDictionary.TryGetValue(uniformName, out uniform);
            return uniform;
        }
        public void SetSubroutine(string uniformName, string subroutineName) {
            if (Context.Version < new GLVersion(4, 4))
                throw new GLException("Subroutines are not supported in OpenGL versions lower than 4.4");

            if (string.IsNullOrWhiteSpace(uniformName))
                throw new ArgumentException("uniformName");
            if (string.IsNullOrWhiteSpace(subroutineName))
                throw new ArgumentException("subroutineName");

            var uniform = _subroutineUniforms.SingleOrDefault(_ => _.Name.Equals(uniformName, StringComparison.InvariantCultureIgnoreCase));
            if (uniform == null)
                throw new GLException(string.Format("Subroutine uniform {0} not found", uniformName));

            var subroutineIndex = uniform.GetSubroutineIndex(subroutineName);
            if (subroutineIndex < 0)
                throw new GLException(string.Format("Subroutine {0} not found", subroutineName));

            uniform.SubroutineIndex = (uint)subroutineIndex;
        }

        private int GetUniformLocation(string uniformName) {
            Uniform uniform = null;
            if (_uniformsDictionary.TryGetValue(uniformName, out uniform) == false)
                return -1;
            else
                return uniform.Index;
        }

        [Conditional("DEBUG")]
        private void CheckAttributeLocations() {
            var location = (StandardAttribute)Context.GetAttributeLocation(Handle, PositionAttributeName);
            Assert.True(location == StandardAttribute.Absent || location == StandardAttribute.Position);

            location = (StandardAttribute)Context.GetAttributeLocation(Handle, ColorAttributeName);
            Assert.True(location == StandardAttribute.Absent || location == StandardAttribute.Color);

            location = (StandardAttribute)Context.GetAttributeLocation(Handle, NormalAttributeName);
            Assert.True(location == StandardAttribute.Absent || location == StandardAttribute.Normal);

            location = (StandardAttribute)Context.GetAttributeLocation(Handle, TangentAttributeName);
            Assert.True(location == StandardAttribute.Absent || location == StandardAttribute.Tangent);
           
            location = (StandardAttribute)Context.GetAttributeLocation(Handle, TexCoordAttributeName);
            Assert.True(location == StandardAttribute.Absent || location == StandardAttribute.TexCoord);

            location = (StandardAttribute)Context.GetAttributeLocation(Handle, DataAttributeName1);
            Assert.True(location == StandardAttribute.Absent || location == StandardAttribute.Data1);

            location = (StandardAttribute)Context.GetAttributeLocation(Handle, DataAttributeName2);
            Assert.True(location == StandardAttribute.Absent || location == StandardAttribute.Data2);

            location = (StandardAttribute)Context.GetAttributeLocation(Handle, DataAttributeName3);
            Assert.True(location == StandardAttribute.Absent || location == StandardAttribute.Data3);
        }
        private void BindAttributeLocation(StandardAttribute standardAttributeLocation, string attributeName) {
            Context.BindAttributeLocation(Handle, (int)standardAttributeLocation, attributeName);
        }
        private void Link() {
            Context.LinkProgram(Handle);

            var compileMessage = Context.GetProgramInfoLog(Handle);
            var linkStatus = Context.GetProgramiv(Handle, ProgramInfo.LinkStatus);

            if (linkStatus == 0)
                throw new GLException(string.Format("Error while linking {0}. Compiler message:\n{1}", this, compileMessage));

            if (string.IsNullOrWhiteSpace(compileMessage) == false)
                Log.Info(compileMessage);
        }
   
        private void InitUniforms() {
            InitUniformCollection();

            var uniformBlockIndex = Context.GetUniformBlockIndex(Handle, GlobalLightsUniformBlockName);
            if (uniformBlockIndex != -1)
                Context.UniformBlockBinding(Handle, (uint)uniformBlockIndex, GlobalUniformsBindingPoint);

            uniformBlockIndex = Context.GetUniformBlockIndex(Handle, StandartUniformBlockName);
            if (uniformBlockIndex != -1)
                Context.UniformBlockBinding(Handle, (uint)uniformBlockIndex, StandartUniformsBindingPoint);

            InitSubroutines(Handle);

            Textures = _textures.AsReadOnly();
            Uniforms = _uniforms.AsReadOnly();
            Subroutines = _subroutineUniforms.AsReadOnly();
        }
        private void InitUniformCollection() {
            UsesNormalMatrix = false;
            UsesModelViewMatrix = false;
            UsesModelMatrix = false;
            UsesInversedModelMatrix = false;

            //old style to check if uniform inside uniform block
            //var activeUniformBlocks = GL.GetProgramiv(_handle, ShaderStatus.ActiveUniformBlocks);
            //var uniformsInsideBlocksIndices = new HashSet<int>();

            //for (int i = 0; i < activeUniformBlocks; i++) {
            //    var p = new int[1];
            //    GL.GetActiveUniformBlockiv(_handle, i, ActiveUniformBlockParameter.UniformBlockActiveUniforms, p);
            //    p = new int[p[0]];
            //    GL.GetActiveUniformBlockiv(_handle, i, ActiveUniformBlockParameter.UniformBlockActiveUniformIndices, p);
            //    p.ForEach(_ => uniformsInsideBlocksIndices.Add(_));
            //}

            var activeUniforms = Context.GetProgramiv(Handle, ProgramInfo.ActiveUniforms);

            for (int index = 0; index < activeUniforms; index++) {
                int size = 0;
                ActiveUniformType type = 0;
                string name = null;
                Context.GetActiveUniform(Handle, index, out size, out type, out name);

                switch (name) {
                    case NormalMatrixUniformName:
                        UsesNormalMatrix = true;
                        break;
                    case ModelViewMatrixUniformName:
                        UsesModelViewMatrix = true;
                        break;
                    case ModelMatrixUniformName:
                        UsesModelMatrix = true;
                        break;
                    case InversedModelMatrixUniformName:
                        UsesInversedModelMatrix = true;
                        break;
                    default:
                        break;
                }

                var blockIndex = Context.GetActiveUniformsiv(Handle, index, ActiveUniformParameter.UniformBlockIndex);
                if (blockIndex != -1)
                    continue;

                var location = Context.GetUniformLocation(Handle, name);
                var uniform = CreateTypedUniform(location, size, type, name);

                _uniformsDictionary.Add(name, uniform);
                if (IsSampler(type))
                    _textures.Add((IntUniform)uniform);
                else
                    _uniforms.Add(uniform);
            }
        }
        private Uniform CreateTypedUniform(int location, int size, ActiveUniformType type, string name) {
            switch (type) {
                case ActiveUniformType.Int:
                case ActiveUniformType.Sampler2D:
                case ActiveUniformType.Sampler2DShadow:
                case ActiveUniformType.UnsignedIntSampler2D:
                case ActiveUniformType.SamplerCube:
                    return new IntUniform(Handle, location, size, type, name);
                case ActiveUniformType.UnsignedInt:
                    return new UIntUniform(Handle, location, size, type, name);
                case ActiveUniformType.Float:
                    return new FloatUniform(Handle, location, size, type, name);
                case ActiveUniformType.FloatVec2:
                    return new Vector2Uniform(Handle, location, size, type, name);
                case ActiveUniformType.FloatVec3:
                    return new Vector3Uniform(Handle, location, size, type, name);
                case ActiveUniformType.FloatVec4:
                    return new Vector4Uniform(Handle, location, size, type, name);
                case ActiveUniformType.FloatMat4:
                    return new Matrix4Uniform(Handle, location, size, type, name);
                case ActiveUniformType.FloatMat3:
                    return new Matrix3Uniform(Handle, location, size, type, name);
                default:
                    throw new NotSupportedException(string.Format("Uniforms of type {0} are not yet supported.", type.ToString()));
            }
        }
        private void GetComputeGroupSize() {
            var values = new int[3];
            Context.GetProgramiv(Handle, ProgramInfo.ComputeWorkGroupSize, values);
            WorkGroupSizeX = values[0];
            WorkGroupSizeY = values[1];
            WorkGroupSizeZ = values[2];
        }

        private void InitSubroutines(uint handle) {
            if (Context.Version < new GLVersion(4, 4))
                return;

            InitSubroutines(handle, ShaderType.VertexShader);
            InitSubroutines(handle, ShaderType.FragmentShader);
            InitSubroutines(handle, ShaderType.GeometryShader);
        }
        private void InitSubroutines(uint handle, ShaderType shaderType) {
            if (shaderType == ShaderType.ComputeShader)
                throw new NotSupportedException(ShaderType.ComputeShader.ToString());

            var activeSubroutineUniformLocations = Context.GetProgramStage(handle, shaderType, ShaderSubroutine.ActiveSubroutineUniformLocations);
            var activeSubroutineUniformsCount = Context.GetProgramStage(handle, shaderType, ShaderSubroutine.ActiveSubroutineUniforms);
            Assert.True(activeSubroutineUniformLocations == activeSubroutineUniformsCount);

            if (activeSubroutineUniformsCount == 0)
                return;

            switch (shaderType) {
                case ShaderType.VertexShader:
                    Assert.Null(_subroutineVertexStage);
                    _subroutineVertexStage = new uint[activeSubroutineUniformLocations];
                    break;
                case ShaderType.FragmentShader:
                    Assert.Null(_subroutineFragmentStage);
                    _subroutineFragmentStage = new uint[activeSubroutineUniformLocations];
                    break;
                case ShaderType.GeometryShader:
                    Assert.Null(_subroutineGeometryStage);
                    _subroutineGeometryStage = new uint[activeSubroutineUniformLocations];
                    break;
                default:
                    throw new NotSupportedException(shaderType.ToString());
            }

            for (uint i = 0; i < activeSubroutineUniformsCount; i++) {
                var uniformName = Context.GetActiveSubroutineUniformName(handle, shaderType, i);
                var uniformLocation = Context.GetSubroutineUniformLocation(handle, shaderType, uniformName);

                var uniform = new SubroutineUniform(uniformName, uniformLocation, shaderType);
                uniform.SubroutineUniformIndexChanged += SubroutineUniformIndexChanged;
                _subroutineUniforms.Add(uniform);

                var compatibleCount = Context.GetActiveSubroutineUniform(handle, shaderType, i, ShaderSubroutine.NumCompatibleSubroutines);

                var indexes = new int[compatibleCount];
                Context.GetActiveSubroutineUniform(handle, shaderType, i, ShaderSubroutine.CompatibleSubroutines, indexes);

                for (uint j = 0; j < compatibleCount; j++) {
                    var subroutineName = Context.GetActiveSubroutineName(handle, shaderType, j);
                    var subroutineIndex = Context.GetSubroutineIndex(handle, shaderType, subroutineName);
                    Assert.True(subroutineIndex == j);

                    uniform.AddSubroutine(subroutineName, subroutineIndex);
                }
            }
        }

        private void SubroutineUniformIndexChanged(SubroutineUniform uniform) {
            switch (uniform.ShaderType) {
                case ShaderType.VertexShader:
                    _subroutineVertexStage[uniform.Location] = uniform.SubroutineIndex;
                    break;
                case ShaderType.FragmentShader:
                    _subroutineFragmentStage[uniform.Location] = uniform.SubroutineIndex;
                    break;
                case ShaderType.GeometryShader:
                    _subroutineGeometryStage[uniform.Location] = uniform.SubroutineIndex;
                    break;
                default:
                    throw new NotSupportedException(uniform.ShaderType.ToString());
            }
        }

        private static bool IsSampler(ActiveUniformType uniformType) {
            switch (uniformType) {
                case ActiveUniformType.Sampler1D:
                case ActiveUniformType.Sampler2D:
                case ActiveUniformType.Sampler3D:
                case ActiveUniformType.SamplerCube:
                case ActiveUniformType.Sampler1DShadow:
                case ActiveUniformType.Sampler2DShadow:
                case ActiveUniformType.Sampler2DRect:
                case ActiveUniformType.Sampler2DRectShadow:
                case ActiveUniformType.Sampler1DArray:
                case ActiveUniformType.Sampler2DArray:
                case ActiveUniformType.SamplerBuffer:
                case ActiveUniformType.Sampler1DArrayShadow:
                case ActiveUniformType.Sampler2DArrayShadow:
                case ActiveUniformType.SamplerCubeShadow:
                case ActiveUniformType.IntSampler1D:
                case ActiveUniformType.IntSampler2D:
                case ActiveUniformType.IntSampler3D:
                case ActiveUniformType.IntSamplerCube:
                case ActiveUniformType.IntSampler2DRect:
                case ActiveUniformType.IntSampler1DArray:
                case ActiveUniformType.IntSampler2DArray:
                case ActiveUniformType.IntSamplerBuffer:
                case ActiveUniformType.UnsignedIntSampler1D:
                case ActiveUniformType.UnsignedIntSampler2D:
                case ActiveUniformType.UnsignedIntSampler3D:
                case ActiveUniformType.UnsignedIntSamplerCube:
                case ActiveUniformType.UnsignedIntSampler2DRect:
                case ActiveUniformType.UnsignedIntSampler1DArray:
                case ActiveUniformType.UnsignedIntSampler2DArray:
                case ActiveUniformType.UnsignedIntSamplerBuffer:
                case ActiveUniformType.SamplerCubeMapArray:
                case ActiveUniformType.SamplerCubeMapArrayShadow:
                case ActiveUniformType.IntSamplerCubeMapArray:
                case ActiveUniformType.UnsignedIntSamplerCubeMapArray:
                case ActiveUniformType.Sampler2DMultisample:
                case ActiveUniformType.IntSampler2DMultisample:
                case ActiveUniformType.UnsignedIntSampler2DMultisample:
                case ActiveUniformType.Sampler2DMultisampleArray:
                case ActiveUniformType.IntSampler2DMultisampleArray:
                case ActiveUniformType.UnsignedIntSampler2DMultisampleArray:
                    return true;
                default:
                    return false;
            }
        }

        public int CompareTo(ShaderProgram other) {
            return Handle.CompareTo(other.Handle);
        }

        internal override DisposeAction GetDisposeAction() {
            return Context.DeleteProgram;
        }

        public virtual IEnumerable<string> Defines { get { return Enumerable.Empty<string>(); } }

        public virtual void Compile() {
            if (IsCompiled)
                throw new InvalidOperationException("Program is already compiled!");

            Defines.ForEach(_ => Define(_));
            Define("MAX_LIGHTS", GlobalUniformsBufferObject.MaxLightCount.ToString());

            IsCompiled = true;

            var definedCode = _defines.ToString() + Code;

            var fullCode = "#version 330 core\n#define VERTEX_SHADER\n" + definedCode;
            using (var vertexShader = new Shader(ShaderType.VertexShader, fullCode))
                Context.AttachShader(Handle, vertexShader.Handle);

            fullCode = "#version 330 core\n#define FRAGMENT_SHADER\n" + definedCode;
            using (var fragmentShader = new Shader(ShaderType.FragmentShader, fullCode))
                Context.AttachShader(Handle, fragmentShader.Handle);

            BindAttributeLocation(StandardAttribute.Position, PositionAttributeName);
            BindAttributeLocation(StandardAttribute.Color, ColorAttributeName);
            BindAttributeLocation(StandardAttribute.Normal, NormalAttributeName);
            BindAttributeLocation(StandardAttribute.TexCoord, TexCoordAttributeName);
            BindAttributeLocation(StandardAttribute.Tangent, TangentAttributeName);

            BindAttributeLocation(StandardAttribute.Data1, DataAttributeName1);
            BindAttributeLocation(StandardAttribute.Data2, DataAttributeName2);
            BindAttributeLocation(StandardAttribute.Data3, DataAttributeName3);

            Link();

            CheckAttributeLocations();

            Context.UseProgram(Handle);
            InitUniforms();

            InitPropertyUniforms();
        }
        public void Define(string name) {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("name");
            if (IsCompiled)
                throw new InvalidOperationException("Program is already compiled!");

            _defines.AppendLine(string.Format("#define {0}\n", name));
        }
        public void Define(params string[] defines) {
            if (defines == null)
                throw new ArgumentNullException("defines");

            defines.ForEach(_ => Define(_));
        }
        public void Define(string name, string value) {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("name");
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("value");
            if (IsCompiled)
                throw new InvalidOperationException("Program is already compiled!");

            _defines.AppendLine(string.Format("#define {0} {1}\n", name, value));
        }

        protected void OnUniformValueChanged<T>(T newValue, [CallerMemberName]string propertyName = null) where T : IEquatable<T> {
            if (IsCompiled == false)
                return;

            var uniform = _propertyUniforms[propertyName] as Uniform<T>;
            uniform.SetValue(newValue);
        }

        private void InitPropertyUniforms() {
            var properties = this
                .GetType()
                .GetProperties(BindingFlags.FlattenHierarchy | BindingFlags.GetProperty | BindingFlags.Public | BindingFlags.SetProperty | BindingFlags.Instance)
                .Where(_ => Attribute.IsDefined(_, typeof(UniformAttribute)))
                .ToArray();
            foreach (var property in properties) {
                var attribute = (UniformAttribute)Attribute.GetCustomAttribute(property, typeof(UniformAttribute));
                var uniformName = attribute.UniformName ?? property.Name;
                var uniform = GetUniform(uniformName);

                //if (uniform == null)
                //    throw new GLException("Cannot find uniform for property");

                _propertyUniforms.Add(property.Name, uniform);
            }
        }
    }
}
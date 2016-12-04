using System;
using System.Collections.Generic;
using NetGL.Core.Mathematics;
using System.Linq;

namespace NetGL.Core.Types {
    public delegate void SubroutineUniformIndexChangedEventHandler(SubroutineUniform sender);

    public abstract class Uniform {
        protected GL Context { get; private set; }

        public int Index { get; private set; }
        public int Size { get; private set; }
        public ActiveUniformType Type { get; private set; }
        public string Name { get; private set; }
        public UInt32 ProgramHandle { get; private set; }
        public abstract Type UniformType { get; }

        protected Uniform(UInt32 programHandle, int index, int size, ActiveUniformType type, string name) {
            Assert.NotNull(name);
            Assert.True(programHandle > 0);

            Context = GL.GetCurrent(true);

            ProgramHandle = programHandle;
            Index = index;
            Size = size;
            Type = type;
            Name = name;
        }

        protected abstract void Set();
        public abstract object GetValue();
        public abstract void SetValue(object value);

        public override string ToString() {
            return string.Format("Uniform {0} of type {1}", Name, Type);
        }
    }
    public abstract class Uniform<T> : Uniform where T : IEquatable<T> {
        private readonly EqualityComparer<T> _comparer = EqualityComparer<T>.Default;

        protected T _value;

        public T Value {
            get {
                return _value;
            }
            set {
                if (_comparer.Equals(_value, value))
                    return;

                _value = value;
                Set();
            }
        }

        public override Type UniformType {
            get { return typeof(T); }
        }

        protected Uniform(UInt32 programHandle, int index, int size, ActiveUniformType type, string name)
            : base(programHandle, index, size, type, name) { }

        public override object GetValue() {
            return _value;
        }
        public override void SetValue(object value) {
            this.Value = (T)value;
        }
        public void SetValue(T value) {
            this.Value = value;
        }

        public override string ToString() {
            return string.Format("Uniform {0} of type {1} value {2}", Name, Type, _value);
        }
    }
    public class IntUniform : Uniform<int> {
        public IntUniform(UInt32 programHandle, int location, int size, ActiveUniformType type, string name)
            : base(programHandle, location, size, type, name) {
            _value = Context.GetUniformInt32(programHandle, location);
        }

        protected override void Set() {
            Context.Uniform(ProgramHandle, Index, Value);
        }
    }
    public class UIntUniform : Uniform<UInt32> {
        public UIntUniform(UInt32 programHandle, int location, int size, ActiveUniformType type, string name)
            : base(programHandle, location, size, type, name) {
            _value = Context.GetUniformUInt32(programHandle, location);
        }

        protected override void Set() {
            Context.Uniform(ProgramHandle, Index, Value);
        }
    }
    public class FloatUniform : Uniform<float> {
        public FloatUniform(UInt32 programHandle, int location, int size, ActiveUniformType type, string name)
            : base(programHandle, location, size, type, name) {
            _value = Context.GetUniformSingle(programHandle, location);
        }

        protected override void Set() {
            Context.Uniform(ProgramHandle, Index, Value);
        }
    }
    public class Vector2Uniform : Uniform<Vector2> {
        public Vector2Uniform(UInt32 program, int location, int size, ActiveUniformType type, string name)
            : base(program, location, size, type, name) {
            _value = Context.GetUniformVector2(program, location);
        }

        protected override void Set() {
            Context.Uniform(ProgramHandle, Index, Value);
        }
    }
    public class Vector3Uniform : Uniform<Vector3> {
        public Vector3Uniform(UInt32 program, int location, int size, ActiveUniformType type, string name)
            : base(program, location, size, type, name) {
            _value = Context.GetUniformVector3(program, location);
        }

        protected override void Set() {
            Context.Uniform(ProgramHandle, Index, Value);
        }
    }
    public class Vector4Uniform : Uniform<Vector4> {
        public Vector4Uniform(UInt32 program, int location, int size, ActiveUniformType type, string name)
            : base(program, location, size, type, name) {
            _value = Context.GetUniformVector4(program, location);
        }

        protected override void Set() {
            Context.Uniform(ProgramHandle, Index, Value);
        }
    }
    public class Matrix4Uniform : Uniform<Matrix> {
        private static readonly float[] Identity = Matrix.Identity.As4x4Array();

        public Matrix4Uniform(UInt32 program, int location, int size, ActiveUniformType type, string name)
            : base(program, location, size, type, name) { }

        protected override void Set() {
            if (Value != null)
                Context.UniformMatrix(ProgramHandle, Index, 1, false, Value.As4x4Array());
            else
                Context.UniformMatrix(ProgramHandle, Index, 1, false, Identity);
        }
    }
    public class Matrix3Uniform : Uniform<Matrix> {
        private static readonly float[] Identity = Matrix.Identity.As3x3Array();

        public Matrix3Uniform(UInt32 program, int location, int size, ActiveUniformType type, string name)
            : base(program, location, size, type, name) {
        }

        protected override void Set() {
            if (Value != null)
                Context.UniformMatrix(ProgramHandle, Index, 1, false, Value.As3x3Array());
            else
                Context.UniformMatrix(ProgramHandle, Index, 1, false, Identity);
        }
    }

    public class Subroutine {
        public uint Index { get; private set; }
        public string Name { get; private set; }

        public Subroutine(uint index, string name) {
            Assert.NotEmpty(name);
            Assert.True(index >= 0);

            Name = name;
            Index = index;
        }

        public override string ToString() {
            return string.Format("Subroutine {0}, index {1}", Name, Index);
        }
    }
    public class SubroutineUniform {
        private uint _subroutineIndex;

        private readonly List<Subroutine> _subroutines = new List<Subroutine>();

        internal event SubroutineUniformIndexChangedEventHandler SubroutineUniformIndexChanged = delegate { };

        public string Name { get; private set; }
        public int Location { get; private set; }
        public ShaderType ShaderType { get; private set; }
        public uint SubroutineIndex {
            get { return _subroutineIndex; }
            set {
                if (_subroutineIndex == value)
                    return;

                _subroutineIndex = value;
                SubroutineUniformIndexChanged(this);
            }
        }
        public IReadOnlyList<Subroutine> Subroutines { get; private set; }

        public SubroutineUniform(string name, int location, ShaderType shaderType) {
            Assert.NotEmpty(name);
            Assert.True(location >= 0);

            Name = name;
            Location = location;
            ShaderType = shaderType;
            Subroutines = _subroutines.AsReadOnly();
        }
        public int GetSubroutineIndex(string subroutineName) {
            if (string.IsNullOrWhiteSpace(subroutineName))
                throw new ArgumentException(subroutineName);

            var subroutine = _subroutines.SingleOrDefault(_ => _.Name.Equals(subroutineName, StringComparison.InvariantCultureIgnoreCase));
            if (subroutine == null)
                return -1;
            else
                return (int)subroutine.Index;
        }

        internal void AddSubroutine(string name, uint index) {
            if (_subroutines.Count == 0)
                SubroutineIndex = index;
            _subroutines.Add(new Subroutine(index, name));
        }

        public override string ToString() {
            return string.Format("Subroutine {0}, location {1}, shadertype {2}", Name, Location, ShaderType);
        }
    }
}

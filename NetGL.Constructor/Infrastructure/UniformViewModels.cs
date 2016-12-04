using System;
using System.Collections.Generic;
using System.Windows.Input;
using NetGL.Core.Mathematics;
using NetGL.Core.Types;
using NetGL.SceneGraph.Components;

namespace NetGL.Constructor.Infrastructure {
    public class TextureViewModel : NotifyPropertyChange, IRefresh {
        public string Name { get; private set; }
        public TextureUniform TextureUniform { get; set; }

        private Texture _texture;
        public Texture Texture {
            get { return _texture; }
            set {
                if (_texture == value)
                    return;

                _texture = value;
                TextureUniform.Texture = value;

                OnPropertyChanged();
            }
        }

        public ICommand RemoveTexture {
            get {
                return new Command(() => {
                    Texture = null;
                });
            }
        }

        public TextureViewModel(TextureUniform uniform) {
            Assert.NotNull(uniform);

            TextureUniform = uniform;
            var name = uniform.Name;
            if (name.StartsWith("uniform_", StringComparison.InvariantCultureIgnoreCase))
                name = name.Substring(8);
            Name = name;

            Refresh();
        }

        public void Refresh() {
            Texture = TextureUniform.Texture;
        }
    }

    public abstract class UniformViewModel : NotifyPropertyChange, IRefresh {
        public String Name { get; private set; }

        protected UniformViewModel(Uniform uniform) {
            Assert.NotNull(uniform);

            var name = uniform.Name;
            if (name.StartsWith("uniform_", StringComparison.InvariantCultureIgnoreCase))
                name = name.Substring(8);
            Name = name;
        }

        public abstract void Refresh();
    }
    public class UniformViewModel<T> : UniformViewModel where T : IEquatable<T> {
        private readonly EqualityComparer<T> _comparer = EqualityComparer<T>.Default;

        public Uniform<T> Uniform { get; private set; }

        private T _value;
        public T Value {
            get { return _value; }
            set {
                if (_comparer.Equals(_value, value))
                    return;

                _value = value;
                Uniform.Value = value;
                OnPropertyChanged();
            }
        }

        public UniformViewModel(Uniform<T> uniform)
            : base(uniform) {
            Assert.True(uniform != null);

            this.Uniform = uniform;
            Refresh();
        }

        public sealed override void Refresh() {
            this.Value = Uniform.Value;
        }
    }

    public class FloatUniformViewModel : UniformViewModel<float> {
        public FloatUniformViewModel(FloatUniform uniform)
            : base(uniform) { }
    }
    public class IntUniformViewModel : UniformViewModel<int> {
        public IntUniformViewModel(IntUniform uniform)
            : base(uniform) { }
    }
    public class Vector4UniformViewModel : UniformViewModel<Vector4> {
        public Vector4UniformViewModel(Vector4Uniform uniform)
            : base(uniform) { }
    }
    public class Vector3UniformViewModel : UniformViewModel<Vector3> {
        private bool _asVector;
        public bool AsVector {
            get { return _asVector; }
            set {
                if (_asVector == value)
                    return;

                _asVector = value;
                OnPropertyChanged();
            }
        }

        public Vector3UniformViewModel(Vector3Uniform uniform)
            : base(uniform) {
            _asVector = true;
        }
    }
}

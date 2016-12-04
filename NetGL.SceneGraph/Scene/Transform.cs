using System;
using System.Collections.Generic;
using NetGL.Core.Mathematics;
using System.Runtime.CompilerServices;

namespace NetGL.SceneGraph.Scene {
    public class Transform {
        private Vector3 _localPosition, _worldPosition;
        private Vector3 _localScale;
        private Quaternion _localRotation, _worldRotation;

        private readonly Matrix _matrix = Matrix.Identity;
        private readonly Matrix _invertedMatrix = Matrix.Identity;
        private bool _isModelMatrixDirty = true, _isInvertedModelMatrixDirty = true;
        private Transform _parent;
        private readonly List<Transform> _children = new List<Transform>();

        internal bool IsDisposed { get; private set; }

        public Node SceneObject { get; private set; }
        public IReadOnlyList<Transform> Children { get; private set; }

        public Transform Parent {
            get { return _parent; }
            set {
                if (this.IsDisposed)
                    throw new ObjectDisposedException("Transform");
                if (SceneObject.IsDisposed)
                    throw new ObjectDisposedException("SceneObject");

                if (_parent == value)
                    return;

                if (value != null) {
                    if (value.IsDisposed)
                        throw new ObjectDisposedException("Parent");
                    if (value.SceneObject.IsDisposed)
                        throw new ObjectDisposedException("Parent.SceneObject");
                    if (value.SceneObject.Scene != this.SceneObject.Scene)
                        throw new InvalidOperationException("Parent and child should be both attached to same scene");
                }

                if (_parent != null)
                    _parent._children.Remove(this);
                _parent = value;
                if (_parent != null)
                    _parent._children.Add(this);

                SetMatrixDirty();
            }
        }

        public Vector3 LocalPosition {
            get { return _localPosition; }
            set {
                if (_localPosition == value)
                    return;
                _localPosition = value;

                SetMatrixDirty();
            }
        }
        public Vector3 WorldPosition {
            get {
                if (_parent == null)
                    return _localPosition;
                else {
                    UpdateModelMatrix();
                    return _worldPosition;
                }
            }
            set {
                if (_parent == null)
                    _localPosition = value;
                else
                    _localPosition = Vector3.Transform(value, _parent.InvertedModelMatrix);

                SetMatrixDirty();
            }
        }

        public Quaternion LocalRotation {
            get {
                return _localRotation;
            }
            set {
                if (_localRotation == value)
                    return;
                _localRotation = value;

                SetMatrixDirty();
            }
        }
        public Quaternion WorldRotation {
            get {
                if (_parent == null)
                    return _localRotation;
                else {
                    UpdateModelMatrix();
                    return _worldRotation;
                }
            }
            set {
                if (_parent == null)
                    _localRotation = value;
                else
                    _localRotation = _parent.WorldRotation.Conjugate * value;

                SetMatrixDirty();
            }
        }

        public Vector3 LocalScale {
            get { return _localScale; }
            set {
                if (_localScale == value)
                    return;

                _localScale = value;
                SetMatrixDirty();
            }
        }

        public Matrix InvertedModelMatrix {
            get {
                if (_isInvertedModelMatrixDirty) {
                    UpdateModelMatrix();
                    Matrix.Invert(_matrix, _invertedMatrix);
                    _isInvertedModelMatrixDirty = false;
                }

                return _invertedMatrix;
            }
        }
        public Matrix ModelMatrix {
            get {
                UpdateModelMatrix();

                return _matrix;
            }
        }

        internal Transform(Node owner) {
            if (owner == null)
                throw new ArgumentNullException("owner");

            IsDisposed = false;
            SceneObject = owner;

            _localPosition = Vector3.Zero;
            _localRotation = Quaternion.Identity;
            _localScale = Vector3.One;

            Children = _children.AsReadOnly();

            SetMatrixDirty();
        }

        internal void Dispose() {
            if (IsDisposed)
                return;
            IsDisposed = true;

            if (_parent != null)
                _parent._children.Remove(this);
            _parent = null;

            _children.ForEach(child => child._parent = null);
            _children.Clear();
        }

        private void SetMatrixDirty() {
            if (_isModelMatrixDirty)
                return;

            _isModelMatrixDirty = true;
            _isInvertedModelMatrixDirty = true;

            for (int i = 0; i < _children.Count; i++)
                _children[i].SetMatrixDirty();
        }
        private void UpdateModelMatrix() {
            if (_isModelMatrixDirty == false)
                return;
            _isModelMatrixDirty = false;

            if (_parent != null)
                _matrix.Load(_parent.ModelMatrix);
            else
                _matrix.LoadIdentity();

            _matrix.Transform(_localPosition, _localRotation, _localScale);

            if (_parent != null) {
                _worldPosition = _matrix.Translation;
                _worldRotation = _parent.WorldRotation * _localRotation;
            }
            else {
                _worldPosition = _localPosition;
                _worldRotation = _localRotation;
            }
        }

        public Vector3 TransformVector(Vector3 vector) {
            return Vector3.Transform(vector, ModelMatrix);
        }
    }
}
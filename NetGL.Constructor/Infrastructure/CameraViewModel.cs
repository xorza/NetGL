using System;
using System.Linq;
using NetGL.Core.Mathematics;
using NetGL.SceneGraph.Components;

namespace NetGL.Constructor.Infrastructure {
    public class CameraViewModel : NotifyPropertyChange, IRefresh {
        private readonly Camera _camera;
        private int _order;
        private CameraType _cameraType;
        public int Order {
            get { return _order; }
            set {
                if (_order == value)
                    return;

                _order = value;
                _camera.Order = value;
                OnPropertyChanged();
            }
        }
        public CameraType CameraType {
            get { return _cameraType; }
            set {
                if (_cameraType == value)
                    return;

                _cameraType = value;
                _camera.Type = value;
                OnPropertyChanged();
            }
        }
        private float _fov;
        public float FOV {
            get { return _fov; }
            set {
                if (value.InRangeExclusive(0, MathF.PI) == false)
                    throw new ArgumentOutOfRangeException("FOV");
                if (_fov == value)
                    return;

                _fov = value;
                _camera.FOV = value;
                OnPropertyChanged();
            }
        }
        private float _size;
        public float Size {
            get { return _size; }
            set {
                if (value < 0)
                    throw new ArgumentOutOfRangeException();
                if (_size == value)
                    return;

                _size = value;
                _camera.Size = value;
                OnPropertyChanged();
            }
        }
        private float _near;
        public float Near {
            get { return _near; }
            set {
                //if (value < 0)
                //    throw new ArgumentOutOfRangeException();
                if (_near == value)
                    return;

                _near = value;
                _camera.Near = value;
                OnPropertyChanged();
            }
        }
        private float _far;
        public float Far {
            get { return _far; }
            set {
                //if (value < 0)
                //    throw new ArgumentOutOfRangeException();
                if (_far == value)
                    return;

                _far = value;
                _camera.Far = value;
                OnPropertyChanged();
            }
        }
        private Vector3 _background;
        public Vector3 Background {
            get { return _background; }
            set {
                if (value.X < 0 || value.Y < 0 || value.Z < 0)
                    throw new ArgumentException();
                if (_background == value)
                    return;

                _background = value;
                _camera.ClearColor = value;
                OnPropertyChanged();
            }
        }
        private Vector3 _ambient;
        public Vector3 Ambient {
            get { return _ambient; }
            set {
                if (value.X < 0 || value.Y < 0 || value.Z < 0)
                    throw new ArgumentException();
                if (_ambient == value)
                    return;

                _ambient = value;
                _camera.AmbientLight = value;
                OnPropertyChanged();
            }
        }
        public CameraType[] CameraTypes {
            get {
                return Enum
                    .GetValues(typeof(CameraType))
                    .Cast<CameraType>()
                    .ToArray();
            }
        }

        public CameraViewModel(Camera camera) {
            this._camera = camera;

            Refresh();
        }

        public void Refresh() {
            CameraType = _camera.Type;
            Order = _camera.Order;
            FOV = _camera.FOV;
            Size = _camera.Size;
            Far = _camera.Far;
            Near = _camera.Near;
            Background = _camera.ClearColor;
            Ambient = _camera.AmbientLight;
        }
    }
}

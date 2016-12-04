using System;
using System.Runtime.InteropServices;
using NetGL.Core.Mathematics;
using NetGL.Core.Types;
using NetGL.SceneGraph.Scene;

namespace NetGL.SceneGraph.Components
{
    [Guid("C0CC387D-63C9-42E4-8B21-A3F39E1489C1")]
    public class LightSource : Component
    {
        internal Light Light { get; private set; }

        [Color]
        public Vector3 Diffuse { get; set; }
        public bool CastShadow { get; set; }
        public Vector3 Attenuation { get; set; }
        public LightType Type { get; set; }

        public LightSource(Node owner)
            : base(owner)
        {
            Diffuse = Vector3.One;
            Light = new Light();
            CastShadow = false;
            Type = Light.Type;
            Attenuation = Light.Attenuation;
        }

        internal void Update(Camera camera)
        {
            Light.Diffuse = Diffuse;
            Light.Attenuation = Attenuation;
            Light.Type = Type;

            switch (Light.Type)
            {
                case LightType.Directional:
                    var direction = -Vector3.Forward;
                    direction = Vector3.NormalTransform(direction, Transform.ModelMatrix);
                    direction = Vector3.NormalTransform(direction, camera.ViewMatrix);
                    Light.Direction = direction.Normalized;
                    break;

                case LightType.Point:
                    Light.Position = Vector3.Transform(Transform.WorldPosition, camera.ViewMatrix);
                    break;

                default:
                    throw new NotImplementedException();
            }
        }
    }
}
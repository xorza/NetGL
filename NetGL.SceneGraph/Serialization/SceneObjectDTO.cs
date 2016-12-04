using System;
using System.Collections.Generic;
using NetGL.SceneGraph.Scene;

namespace NetGL.SceneGraph.Serialization {
    internal class SceneObjectDTO {
        public string Name { get; set; }
        public Guid Id { get; set; }
        public Guid? ParentId { get; set; }

        public Vector3DTO Position { get; set; }
        public QuaternionDTO Rotation { get; set; }
        public Vector3DTO Scale { get; set; }

        public List<ComponentDTO> Components { get; set; }

        public SceneObjectDTO() {
        }

        public SceneObjectDTO(SerializationContext context, Node so) {
            Components = new List<ComponentDTO>();

            Id = so.Id;
            if (so.Transform.Parent != null)
                ParentId = so.Transform.Parent.SceneObject.Id;
            else
                ParentId = null;

            Position = new Vector3DTO(so.Transform.LocalPosition);
            Rotation = new QuaternionDTO(so.Transform.LocalRotation);
            Scale = new Vector3DTO(so.Transform.LocalScale);
            Name = so.Name;

            foreach (var component in so.Components) {
                if (false == Attribute.IsDefined(component.GetType(), typeof(NotSerializedAttribute)))
                    Components.Add(new ComponentDTO(context, component));
            }
        }

        public Node Create(DeserializationContext context) {
            var sceneObject = new Node(context.Scene, Name);
            sceneObject.Transform.LocalPosition = Position;
            sceneObject.Transform.LocalRotation = Rotation;
            sceneObject.Transform.LocalScale = Scale;
            sceneObject.Name = Name;
            sceneObject.Id = Id;

            if (ParentId.HasValue)
                context.NeedParent(sceneObject, ParentId.Value);

            foreach (var component in Components)
                component.Create(context, sceneObject);

            return sceneObject;
        }
    }
}
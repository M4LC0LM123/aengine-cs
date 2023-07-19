using System.Numerics;

namespace aengine.ecs
{
    public class TransformComponent : Component
    {
        public Vector3 position;
        public Vector3 rotation;
        public Vector3 scale;

        public TransformComponent(Entity entity)
        {
            position = new Vector3();
            rotation = new Vector3();
            scale = new Vector3();
        }

        public AABB getBoundingBox()
        {
            return new AABB(this.position.X, this.position.Y, this.position.Z, this.scale.X, this.scale.Y, this.scale.Z);
        }

    }
}
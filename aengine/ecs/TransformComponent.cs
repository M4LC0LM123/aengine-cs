using System.Numerics;

namespace aengine.ecs
{
    public class TransformComponent : Component
    {
        public Vector3 position;
        public Vector3 rotation;
        public Vector3 scale;

        private string m_name = "transform";
        
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

        public void update(Entity entity) { }

        public void render() { }

        public void dispose() { }

        public string fileName() {
            return m_name;
        }
        
        public string getType() {
            return "TransformComponent";
        }

        public override string ToString() {
            return position + "," + scale + ", " + rotation;
        }
    }
}
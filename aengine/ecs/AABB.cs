using System.Numerics;

namespace aengine.ecs
{
    public struct AABB
    {
        public float x = 0;
        public float y = 0;
        public float z = 0;
        public float width = 0;
        public float height = 0;
        public float depth = 0;

        public AABB(float x, float y, float z, float width, float height, float depth)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.width = width;
            this.height = height;
            this.depth = depth;
        }
        
        public bool overlaps(AABB other)
        {
            return !(x + width < other.x || other.x + other.width < x ||
                     y + height < other.y || other.y + other.height < y ||
                     z + depth < other.z || other.z + other.depth < z);
        }
    }
}
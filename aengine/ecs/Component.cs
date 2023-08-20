using System.Numerics;

namespace aengine.ecs
{
    public interface Component
    {
        public void update(Entity entity);
        public void render();
        public void dispose();
    }
}
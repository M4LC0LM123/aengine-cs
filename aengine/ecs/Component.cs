using System.Numerics;
using aengine_cs.aengine.parser;

namespace aengine.ecs;

public interface Component {
    public void update(Entity entity);
    public void render();
    public void dispose();

    public string fileName();
    public string getType();
}

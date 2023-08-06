using System.Numerics;

namespace aengine.ecs;

public class RandomrSideBehaviour : ParticleBehaviour
{
    public RandomrSideBehaviour(float gravity = 9.81f)
    {
        this.gravity = gravity;
        Vector2 direction = new Vector2(core.aengine.getRandomInt(-2, 2), core.aengine.getRandomInt(-2, 2));
        acceleration.X = direction.X;
        acceleration.Z = direction.Y;
    }
}
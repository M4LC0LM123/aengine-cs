using System.Numerics;
using Raylib_CsLo;

namespace aengine.ecs;

public class ScaleBehaviour : ParticleBehaviour
{
    public float speed;
    public bool increase;
    public Vector2 max;

    public ScaleBehaviour(bool increase = false, float max = 3, float maxY = 3, float speed = 2.5f)
    {
        this.speed = speed;
        move = false;
        this.increase = increase;
        this.max = new Vector2(max, maxY);
    }

    public override void update(ParticleComponent particle)
    {
        base.update(particle);
        if (particle.transform.scale.X > 0 && !increase) particle.transform.scale.X -= speed * Raylib.GetFrameTime();
        if (particle.transform.scale.X < max.X && increase) particle.transform.scale.X += speed * Raylib.GetFrameTime();
        if (particle.transform.scale.Y > 0 && !increase) particle.transform.scale.Y -= speed * Raylib.GetFrameTime();
        if (particle.transform.scale.Y < max.Y && increase) particle.transform.scale.Y += speed * Raylib.GetFrameTime();
    }
}
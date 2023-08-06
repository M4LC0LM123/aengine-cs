using Raylib_CsLo;

namespace aengine.ecs;

public class DecayBehaviour : ParticleBehaviour
{
    public float speed;

    public DecayBehaviour(float speed = 100)
    {
        this.speed = speed;
        move = false;
    }

    public override void update(ParticleComponent particle)
    {
        base.update(particle);
        if (particle.color.a > 0)
        {
            particle.color = new Color(particle.color.r, particle.color.g, particle.color.b, (int)(particle.color.a - speed * Raylib.GetFrameTime()));
        }
    }
}
using Raylib_CsLo;

namespace aengine.ecs;

public class GradientBehaviour : ParticleBehaviour
{
    public Color colorB;
    public float speed;

    private int r = 0;
    private int g = 0;
    private int b = 0;

    public GradientBehaviour(Color colorA, Color colorB, float speed = 100)
    {
        r = colorA.r;
        g = colorA.g;
        b = colorA.b;
        this.colorB = colorB;
        this.speed = speed;
        move = false;
    }

    public override void update(ParticleComponent particle)
    {
        base.update(particle);
        if (r > colorB.r) r -= (int)(speed * Raylib.GetFrameTime());
        if (g > colorB.g) g -= (int)(speed * Raylib.GetFrameTime());
        if (b > colorB.b) b -= (int)(speed * Raylib.GetFrameTime());
        
        if (r < colorB.r) r += (int)(speed * Raylib.GetFrameTime());
        if (g < colorB.g) g += (int)(speed * Raylib.GetFrameTime());
        if (b < colorB.b) b += (int)(speed * Raylib.GetFrameTime());

        particle.color = new Color(r, g, b, particle.color.a);
    }
}
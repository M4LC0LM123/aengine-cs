using aengine.graphics;
using Raylib_CsLo;

namespace aengine.ecs;

public class ParticleSystem : Entity
{
    public List<ParticleComponent> particles;
    private Camera camera;

    public ParticleSystem()
    {
        particles = new List<ParticleComponent>();
    }

    public void setCamera(Camera camera)
    {
        this.camera = camera;
    }

    public void addParticle(Color color, bool move = true, float gravity = 9.81f)
    {
        ParticleComponent p = new ParticleComponent(new ParticleBehaviour(move, gravity), color);
        p.transform.position = transform.position;
        particles.Add(p);
    }
    
    public void addParticle(ParticleComponent particle)
    {
        particle.transform.position = transform.position;
        particles.Add(particle);
    }
    
    public override void update()
    {
        base.update();
        for (int i = 0; i < particles.Count; i++)
        {
            if (particles[i].lifeTime > 0)
            {
                particles[i].update(null);
                particles[i].lifeTime -= 10 * Raylib.GetFrameTime();
            }

            if (particles[i].lifeTime <= 0)
            {
                particles.RemoveAt(i);
            }
        }
    }
    
    public override void render()
    {
        base.render();
        foreach (var particle in particles)
        {
            particle.setCamera(camera);
            particle.render();
        }
    }

    public override void dispose()
    {
        base.dispose();
        foreach (var particle in particles)
        {
            particle.dispose();
        }
    }
}
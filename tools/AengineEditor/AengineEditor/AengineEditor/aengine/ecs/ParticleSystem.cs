using System.Numerics;
using aengine.core;
using aengine.graphics;
using Raylib_CsLo;

namespace aengine.ecs;

public class ParticleSystem : Entity
{
    public List<ParticleComponent> particles;
    private Camera m_camera;

    public ParticleSystem()
    {
        particles = new List<ParticleComponent>();
    }

    public void setCamera(Camera camera)
    {
        this.m_camera = camera;
    }

    public void addParticle(Color color, bool move = true, float gravity = 9.81f, int density = 1)
    {
        for (int i = 0; i < density; i++)
        {
            ParticleComponent p = new ParticleComponent(new ParticleBehaviour(move, gravity), color);
            p.transform.position = transform.position + ErrorValues.PARTICLE_POS_ERROR * core.aengine.getRandomInt(-2, 2);
            particles.Add(p);
        }
    }
    
    public void addParticle(ParticleComponent particle, int density = 1)
    {
        for (int i = 0; i < density; i++)
        {
            ParticleComponent clone = particle.clone();
            clone.transform.position = transform.position + ErrorValues.PARTICLE_POS_ERROR * core.aengine.getRandomInt(-2, 2);
            particles.Add(clone);
        }
    }
    
    public void addParticle(Vector3 position, Color color, bool move = true, float gravity = 9.81f, int density = 1)
    {
        for (int i = 0; i < density; i++)
        {
            ParticleComponent p = new ParticleComponent(new ParticleBehaviour(move, gravity), color);
            p.transform.position = transform.position + position + ErrorValues.PARTICLE_POS_ERROR * core.aengine.getRandomInt(-2, 2);
            particles.Add(p);
        }
    }
    
    public void addParticle(ParticleComponent particle, Vector3 position, int density = 1)
    {
        for (int i = 0; i < density; i++)
        {
            ParticleComponent clone = particle.clone();
            clone.transform.position = transform.position + position + ErrorValues.PARTICLE_POS_ERROR * core.aengine.getRandomInt(-2, 2);
            particles.Add(clone);
        }
    }
    
    public override void update()
    {
        base.update();

        if (World.camera != null) m_camera = World.camera;
        
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
        int len = particles.Count;
        for (int i = 0; i < len; i++)
        {
            particles[i].setCamera(m_camera);
            particles[i].render();
        }
    }

    public override void dispose()
    {
        base.dispose();
        int len = particles.Count;
        for (int i = 0; i < len; i++)
            particles[i].dispose();
    }
}
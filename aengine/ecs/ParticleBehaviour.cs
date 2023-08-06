using System.Numerics;
using Raylib_CsLo;

namespace aengine.ecs;

public class ParticleBehaviour
{
    public Vector3 velocity;
    public Vector3 acceleration;
    public bool move;
    public float gravity;

    public ParticleBehaviour(bool move = true, float gravity = 9.81f)
    {
        this.gravity = gravity;
        velocity = new Vector3();
        acceleration = new Vector3();
        this.move = move;
    }
    
    public virtual void update(ParticleComponent particle)
    {
        if (move)
        {
            acceleration.Y = -gravity;
            velocity = core.aengine.mullAddV3(velocity, acceleration, Raylib.GetFrameTime());
            particle.transform.position = core.aengine.mullAddV3(particle.transform.position, velocity, Raylib.GetFrameTime());
        }
    }
}
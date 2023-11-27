using System.Numerics;
using aengine.graphics;
using Raylib_CsLo;
using static Raylib_CsLo.Raylib;

namespace aengine.ecs;

public class ParticleComponent : Component {
    public TransformComponent transform;
    public Texture texture;
    public float maxLifeTime;
    public float lifeTime;
    public Color color;
    public List<ParticleBehaviour> behaviours;

    private Camera camera;
    private string m_name = "particle";

    public ParticleComponent() {
        transform = new TransformComponent(null);
        transform.scale = Vector3.One;
        texture = Raylib.LoadTextureFromImage(Raylib.GenImageColor(1, 1, Raylib.WHITE));
        maxLifeTime = 50;
        lifeTime = maxLifeTime;
        behaviours = new List<ParticleBehaviour>();
        behaviours.Add(new ParticleBehaviour());
        color = Raylib.WHITE;
    }

    public ParticleComponent(ParticleBehaviour behaviour, Color color, float maxLifeTime = 50) {
        transform = new TransformComponent(null);
        transform.scale = Vector3.One;
        texture = Raylib.LoadTextureFromImage(Raylib.GenImageColor(1, 1, Raylib.WHITE));
        this.maxLifeTime = maxLifeTime;
        lifeTime = this.maxLifeTime;
        behaviours = new List<ParticleBehaviour>();
        behaviours.Add(behaviour);
        this.color = color;
    }

    public ParticleComponent(ParticleBehaviour behaviour, Color color, Texture texture, Vector2 scale,
        float maxLifeTime = 50) {
        transform = new TransformComponent(null);
        transform.scale = new Vector3(scale.X, scale.Y, 0);
        this.texture = texture;
        this.maxLifeTime = maxLifeTime;
        lifeTime = this.maxLifeTime;
        behaviours = new List<ParticleBehaviour>();
        behaviours.Add(behaviour);
        this.color = color;
    }

    public ParticleComponent clone() {
        ParticleComponent copy = new ParticleComponent(behaviours[0], color, texture,
            new Vector2(transform.scale.X, transform.scale.Y), lifeTime);
        foreach (var behaviour in behaviours) {
            copy.addBehaviour(behaviour);
        }

        return copy;
    }

    public void addBehaviour(ParticleBehaviour behaviour) {
        behaviours.Add(behaviour);
    }

    public void setCamera(Camera camera) {
        this.camera = camera;
    }

    public void update(Entity entity) {
        foreach (var behaviour in behaviours) {
            behaviour.update(this);
        }
    }

    public void render() {
        transform.rotation.Y =
            (float)Math.Atan2(camera.position.X - transform.position.X, camera.position.Z - transform.position.Z) *
            RayMath.RAD2DEG;
        transform.rotation.X = (float)Math.Atan2(RayMath.Vector3Subtract(camera.position, transform.position).Y,
            Math.Sqrt(
                RayMath.Vector3Subtract(camera.position, transform.position).X *
                RayMath.Vector3Subtract(camera.position, transform.position).X +
                RayMath.Vector3Subtract(camera.position, transform.position).Z *
                RayMath.Vector3Subtract(camera.position, transform.position).Z)) * (180.0f / MathF.PI);

        Rendering.drawSprite3D(texture, transform.position, transform.scale.X, transform.scale.Y, transform.rotation.Y,
            -transform.rotation.X, color);
    }

    public void dispose() {
        UnloadTexture(texture);
    }

    public string fileName() {
        return m_name;
    }
    
    public string getType() {
        return "ParticleComponent";
    }
}
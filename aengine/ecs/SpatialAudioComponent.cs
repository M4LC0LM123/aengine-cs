using System.Numerics;
using aengine.graphics;
using Raylib_CsLo;
using static Raylib_CsLo.Raylib;

namespace aengine.ecs;

public class SpatialAudioComponent : Component
{
    public aSound sound;
    public Vector3 position;
    public Vector3 target;
    public float strength;
    public bool canPlay;
    public bool debug = false;
    
    private string m_name = "audio";

    public SpatialAudioComponent(Entity entity, aSound sound)
    {
        this.sound = sound;
        position = entity.transform.position;
        target = new Vector3();
        strength = 1;
        canPlay = true;
    }

    public void setTarget(Vector3 target)
    {
        this.target = target;
    }

    public void play()
    {
        if (!IsSoundPlaying(sound.data) && canPlay) PlaySound(sound.data);
    }
    
    public void update(Entity entity)
    {
        position = entity.transform.position;

        if (World.camera != null) target = World.camera.position;

        float distance = (float) Math.Sqrt(Math.Pow(target.X - position.X, 2) + Math.Pow(target.Y - position.Y, 2) + Math.Pow(target.Z - position.Z, 2));
        strength = 1.0f / (distance / core.aengine.MAX_SOUND_DISTANCE + 1.0f);

        strength = Math.Clamp(strength, 0.0f, 1.0f);
        
        SetSoundVolume(sound.data, strength);
    }

    public void render()
    {
        if (debug)
            DrawCube(position, 0.5f, 0.5f, 0.5f, WHITE);
    }

    public void dispose()
    {
        if (sound != null) sound.dispose();
    }

    public string fileName() {
        return m_name;
    }
    
    public string getType() {
        return "SpatialAudioComponent";
    }
}
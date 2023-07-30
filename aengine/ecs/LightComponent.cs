using Raylib_CsLo;
using static Raylib_CsLo.Raylib;
using System.Numerics;
using aengine.graphics;

namespace aengine.ecs;

public class LightComponent : Component
{
    public float intensity;
    public Shader shader;
    
    public RLights.Light core;
    public bool enabled;
    public Vector3 updateVector;
    
    public unsafe LightComponent(Entity entity, aShader shader, Color color, RLights.LightType type = RLights.LightType.LIGHT_POINT)
    {
        updateVector = new Vector3();
        intensity = 2.5f;
        enabled = true;
        this.shader = shader.shader;
        
        this.shader.locs[(int)SHADER_LOC_MAP_DIFFUSE] = GetShaderLocation(this.shader, "viewPos");

        // Ambient light level (some basic lighting)
        int ambientLoc = GetShaderLocation(this.shader, "ambient");
        SetShaderValue(this.shader, ambientLoc, new Vector4(intensity, intensity, intensity, 1.0f), ShaderUniformDataType.SHADER_UNIFORM_VEC4);
        
        core = World.lights.CreateLight(type, entity.transform.position, Vector3.Zero, color, this.shader);
    }

    public override unsafe void update(Entity entity)
    {
        base.update(entity);
        core.enabled = enabled;
        
        SetShaderValue(shader, shader.locs[(int)SHADER_LOC_MAP_DIFFUSE], updateVector, ShaderUniformDataType.SHADER_UNIFORM_VEC4);
        
        World.lights.UpdateLightValues(shader, core);
    }

    public void setUpdateVector(Vector3 updateVector)
    {
        this.updateVector = updateVector;
    }

    public void setIntensity(float intensity)
    {
        this.intensity = intensity;
        int ambientLoc = GetShaderLocation(this.shader, "ambient");
        SetShaderValue(this.shader, ambientLoc, new Vector4(this.intensity, this.intensity, this.intensity, 1.0f), ShaderUniformDataType.SHADER_UNIFORM_VEC4);
    }
    
}
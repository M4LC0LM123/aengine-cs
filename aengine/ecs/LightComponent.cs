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

    public bool debug = false;

    private Model cube;
    private string m_name = "light";
    
    public unsafe LightComponent(Entity entity, aShader shader, Color color, LightType type = LightType.POINT)
    {
        updateVector = new Vector3();
        intensity = 2.5f;
        enabled = true;
        this.shader = shader.shader;
        
        this.shader.locs[(int)SHADER_LOC_MAP_DIFFUSE] = GetShaderLocation(this.shader, "viewPos");

        // Ambient light level (some basic lighting)
        int ambientLoc = GetShaderLocation(this.shader, "ambient");
        SetShaderValue(this.shader, ambientLoc, new Vector4(intensity, intensity, intensity, 1.0f), ShaderUniformDataType.SHADER_UNIFORM_VEC4);
        
        core = World.lights.CreateLight((RLights.LightType)type, entity.transform.position, Vector3.Zero, color, this.shader);

        cube = LoadModelFromMesh(GenMeshCube(0.5f, 0.5f, 0.5f));
        cube.materials[0].shader = this.shader;
    }

    public unsafe void update(Entity entity) {
        core.position = entity.transform.position;
        core.enabled = enabled;
        
        SetShaderValue(shader, shader.locs[(int)SHADER_LOC_MAP_DIFFUSE], updateVector, ShaderUniformDataType.SHADER_UNIFORM_VEC4);

        if (World.camera != null) updateVector = World.camera.position;
        
        World.lights.UpdateLightValues(shader, core);
    }

    public void render()
    {
        if (debug)
            DrawModel(cube, core.position, 1, core.color);
    }

    public void dispose()
    {
        UnloadShader(shader);
    }

    public string fileName() {
        return m_name;
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
using Raylib_CsLo;
using static Raylib_CsLo.Raylib;
using System.Numerics;
using aengine.graphics;

namespace aengine.ecs;

public class LightComponent : Component
{
    public float intensity;
    public aShader shader;
    
    public RLights.Light core;
    public bool enabled;
    public Vector3 updateVector;

    public bool debug = false;

    private Model cube;
    private string m_name = "light";
    
    public unsafe LightComponent(Entity entity, Color color, LightType type = LightType.POINT)
    {
        updateVector = new Vector3();
        intensity = 2.5f;
        enabled = true;
        shader = LightShader.DEFAULT_LIGHT;
        
        shader.handle.locs[(int)SHADER_LOC_MAP_DIFFUSE] = GetShaderLocation(shader.handle, "viewPos");

        // Ambient light level (some basic lighting)
        int ambientLoc = GetShaderLocation(shader.handle, "ambient");
        SetShaderValue(shader.handle, ambientLoc, new Vector4(intensity, intensity, intensity, 1.0f), ShaderUniformDataType.SHADER_UNIFORM_VEC4);
        
        core = World.lights.CreateLight((RLights.LightType)type, entity.transform.position, Vector3.Zero, color, shader.handle);

        cube = LoadModelFromMesh(GenMeshCube(0.5f, 0.5f, 0.5f));
        cube.materials[0].shader = shader.handle;
    }

    public void setColor(Color color) {
        core.color = color;
    }

    public unsafe void update(Entity entity) {
        core.position = entity.transform.position;
        core.enabled = enabled;
        
        SetShaderValue(shader.handle, shader.handle.locs[(int)SHADER_LOC_MAP_DIFFUSE], updateVector, ShaderUniformDataType.SHADER_UNIFORM_VEC4);

        if (World.camera != null) updateVector = World.camera.position;
        
        World.lights.UpdateLightValues(shader.handle, core);
    }

    public void render()
    {
        if (debug)
            DrawModel(cube, core.position, 1, core.color);
    }

    public void dispose()
    {
        UnloadShader(shader.handle);
    }

    public string fileName() {
        return m_name;
    }
    
    public string getType() {
        return "LightComponent";
    }

    public void setUpdateVector(Vector3 updateVector)
    {
        this.updateVector = updateVector;
    }

    public void setIntensity(float intensity)
    {
        this.intensity = intensity;
        int ambientLoc = GetShaderLocation(shader.handle, "ambient");
        SetShaderValue(shader.handle, ambientLoc, new Vector4(this.intensity, this.intensity, this.intensity, 1.0f), ShaderUniformDataType.SHADER_UNIFORM_VEC4);
    }
    
}
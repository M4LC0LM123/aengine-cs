using aengine.graphics;
using Raylib_CsLo;
using static Raylib_CsLo.Raylib;
using System.Numerics;

namespace aengine.ecs;

public class FluidComponent : Component {
    public aShader shader;
    public Texture texture;
    public Color color;

    public float freqX;
    public float freqY;
    public float ampX;
    public float ampY;
    public float speedX;
    public float speedY;

    private float seconds;

    private int _secondsLoc;
    private int _freqXLoc;
    private int _freqYLoc;
    private int _ampXLoc;
    private int _ampYLoc;
    private int _speedXLoc;
    private int _speedYLoc;

    public Vector3 position;
    public Vector2 scale;
    public float rotation;
    
    private string m_name = "fluid";

    public unsafe FluidComponent(Entity entity, aShader shader, Texture texture, Color color) {
        this.texture = texture;
        this.shader = shader;

        _secondsLoc = GetShaderLocation(this.shader.handle, "seconds");
        _freqXLoc = GetShaderLocation(this.shader.handle, "freqX");
        _freqYLoc = GetShaderLocation(this.shader.handle, "freqY");
        _ampXLoc = GetShaderLocation(this.shader.handle, "ampX");
        _ampYLoc = GetShaderLocation(this.shader.handle, "ampY");
        _speedXLoc = GetShaderLocation(this.shader.handle, "speedX");
        _speedYLoc = GetShaderLocation(this.shader.handle, "speedY");

        freqX = 25.0f;
        freqY = 25.0f;
        ampX = 5.0f;
        ampY = 5.0f;
        speedX = 8.0f;
        speedY = 8.0f;

        Vector2 screenSize = new(GetScreenWidth(), GetScreenHeight());
        SetShaderValue(this.shader.handle, GetShaderLocation(this.shader.handle, "size"), &screenSize,
            ShaderUniformDataType.SHADER_UNIFORM_VEC2);
        SetShaderValue(this.shader.handle, _freqXLoc, freqX, ShaderUniformDataType.SHADER_UNIFORM_FLOAT);
        SetShaderValue(this.shader.handle, _freqYLoc, freqY, ShaderUniformDataType.SHADER_UNIFORM_FLOAT);
        SetShaderValue(this.shader.handle, _ampXLoc, ampX, ShaderUniformDataType.SHADER_UNIFORM_FLOAT);
        SetShaderValue(this.shader.handle, _ampYLoc, ampY, ShaderUniformDataType.SHADER_UNIFORM_FLOAT);
        SetShaderValue(this.shader.handle, _speedXLoc, speedX, ShaderUniformDataType.SHADER_UNIFORM_FLOAT);
        SetShaderValue(this.shader.handle, _speedYLoc, speedY, ShaderUniformDataType.SHADER_UNIFORM_FLOAT);

        seconds = 0f;

        position = entity.transform.position;
        scale = new Vector2(entity.transform.scale.X, entity.transform.scale.Z);
        rotation = entity.transform.rotation.Y;

        this.color = color;
    }

    public void resetValues() {
        SetShaderValue(shader.handle, _freqXLoc, freqX, ShaderUniformDataType.SHADER_UNIFORM_FLOAT);
        SetShaderValue(shader.handle, _freqYLoc, freqY, ShaderUniformDataType.SHADER_UNIFORM_FLOAT);
        SetShaderValue(shader.handle, _ampXLoc, ampX, ShaderUniformDataType.SHADER_UNIFORM_FLOAT);
        SetShaderValue(shader.handle, _ampYLoc, ampY, ShaderUniformDataType.SHADER_UNIFORM_FLOAT);
        SetShaderValue(shader.handle, _speedXLoc, speedX, ShaderUniformDataType.SHADER_UNIFORM_FLOAT);
        SetShaderValue(shader.handle, _speedYLoc, speedY, ShaderUniformDataType.SHADER_UNIFORM_FLOAT);
    }

    public void update(Entity entity) {
        seconds += GetFrameTime();
        SetShaderValue(shader.handle, _secondsLoc, seconds, ShaderUniformDataType.SHADER_UNIFORM_FLOAT);

        position = entity.transform.position;
        scale = new Vector2(entity.transform.scale.X, entity.transform.scale.Z);
        rotation = entity.transform.rotation.Y;
    }

    public void render() {
        BeginShaderMode(shader.handle);
        Rendering.drawTexturedPlane(texture, position, scale.X, scale.Y, rotation, color);
        EndShaderMode();
    }

    public void dispose() {
        UnloadTexture(texture);
    }

    public string fileName() {
        return m_name;
    }
    
    public string getType() {
        return "FluidComponent";
    }
}
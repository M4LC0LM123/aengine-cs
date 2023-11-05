using aengine.graphics;
using Raylib_CsLo;
using static Raylib_CsLo.Raylib;
using System.Numerics;

namespace aengine.ecs;

public class FluidComponent : Component {
    public Shader shader;
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

    public unsafe FluidComponent(Entity entity, aShader shader, Texture texture, Color color) {
        this.texture = texture;
        this.shader = shader.shader;

        _secondsLoc = GetShaderLocation(this.shader, "seconds");
        _freqXLoc = GetShaderLocation(this.shader, "freqX");
        _freqYLoc = GetShaderLocation(this.shader, "freqY");
        _ampXLoc = GetShaderLocation(this.shader, "ampX");
        _ampYLoc = GetShaderLocation(this.shader, "ampY");
        _speedXLoc = GetShaderLocation(this.shader, "speedX");
        _speedYLoc = GetShaderLocation(this.shader, "speedY");

        freqX = 25.0f;
        freqY = 25.0f;
        ampX = 5.0f;
        ampY = 5.0f;
        speedX = 8.0f;
        speedY = 8.0f;

        Vector2 screenSize = new(GetScreenWidth(), GetScreenHeight());
        SetShaderValue(this.shader, GetShaderLocation(this.shader, "size"), &screenSize,
            ShaderUniformDataType.SHADER_UNIFORM_VEC2);
        SetShaderValue(this.shader, _freqXLoc, freqX, ShaderUniformDataType.SHADER_UNIFORM_FLOAT);
        SetShaderValue(this.shader, _freqYLoc, freqY, ShaderUniformDataType.SHADER_UNIFORM_FLOAT);
        SetShaderValue(this.shader, _ampXLoc, ampX, ShaderUniformDataType.SHADER_UNIFORM_FLOAT);
        SetShaderValue(this.shader, _ampYLoc, ampY, ShaderUniformDataType.SHADER_UNIFORM_FLOAT);
        SetShaderValue(this.shader, _speedXLoc, speedX, ShaderUniformDataType.SHADER_UNIFORM_FLOAT);
        SetShaderValue(this.shader, _speedYLoc, speedY, ShaderUniformDataType.SHADER_UNIFORM_FLOAT);

        seconds = 0f;

        position = entity.transform.position;
        scale = new Vector2(entity.transform.scale.X, entity.transform.scale.Z);
        rotation = entity.transform.rotation.Y;

        this.color = color;
    }

    public void resetValues() {
        SetShaderValue(shader, _freqXLoc, freqX, ShaderUniformDataType.SHADER_UNIFORM_FLOAT);
        SetShaderValue(shader, _freqYLoc, freqY, ShaderUniformDataType.SHADER_UNIFORM_FLOAT);
        SetShaderValue(shader, _ampXLoc, ampX, ShaderUniformDataType.SHADER_UNIFORM_FLOAT);
        SetShaderValue(shader, _ampYLoc, ampY, ShaderUniformDataType.SHADER_UNIFORM_FLOAT);
        SetShaderValue(shader, _speedXLoc, speedX, ShaderUniformDataType.SHADER_UNIFORM_FLOAT);
        SetShaderValue(shader, _speedYLoc, speedY, ShaderUniformDataType.SHADER_UNIFORM_FLOAT);
    }

    public void update(Entity entity) {
        seconds += GetFrameTime();
        SetShaderValue(shader, _secondsLoc, seconds, ShaderUniformDataType.SHADER_UNIFORM_FLOAT);

        position = entity.transform.position;
        scale = new Vector2(entity.transform.scale.X, entity.transform.scale.Z);
        rotation = entity.transform.rotation.Y;
    }

    public void render() {
        BeginShaderMode(shader);
        Rendering.drawTexturedPlane(texture, position, scale.X, scale.Y, rotation, color);
        EndShaderMode();
    }

    public void dispose() {
        UnloadTexture(texture);
    }
}
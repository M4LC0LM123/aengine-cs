using Raylib_CsLo;
using static Raylib_CsLo.Raylib;

namespace aengine.graphics;

public class aShader
{
    public Shader shader;

    public aShader(string vert, string frag)
    {
        shader = LoadShader(TextFormat("assets/shaders/light.vert", core.aengine.GLSL_VERSION),
            TextFormat("assets/shaders/light.frag", core.aengine.GLSL_VERSION));
    }
}
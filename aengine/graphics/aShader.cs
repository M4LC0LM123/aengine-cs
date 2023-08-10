using Raylib_CsLo;
using static Raylib_CsLo.Raylib;

namespace aengine.graphics;

public class aShader
{
    public Shader shader;

    public aShader(string vert, string frag)
    {
        if (vert != null && frag != null)
            shader = LoadShader(TextFormat(vert, core.aengine.GLSL_VERSION),
                TextFormat(frag, core.aengine.GLSL_VERSION));
        
        if (vert == null) 
            shader = LoadShader(null,
                TextFormat(frag, core.aengine.GLSL_VERSION));
        
        if (frag == null) 
            shader = LoadShader(TextFormat(vert, core.aengine.GLSL_VERSION),
                null);
    }
}
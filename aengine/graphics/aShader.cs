using Raylib_CsLo;
using static Raylib_CsLo.Raylib;

namespace aengine.graphics;

public class aShader
{
    public Shader handle;
    public string vertPath;
    public string fragPath;

    public aShader(string vert, string frag)
    {
        if (vert != null && frag != null)
            handle = LoadShader(TextFormat(vert, core.aengine.GLSL_VERSION),
                TextFormat(frag, core.aengine.GLSL_VERSION));
        
        if (vert == null) 
            handle = LoadShader(null,
                TextFormat(frag, core.aengine.GLSL_VERSION));
        
        if (frag == null) 
            handle = LoadShader(TextFormat(vert, core.aengine.GLSL_VERSION),
                null);

        vertPath = vert != null ? vert : String.Empty;
        fragPath = frag != null ? frag : String.Empty;
    }
}
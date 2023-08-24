using System.Numerics;
using OpenTK.Graphics.OpenGL;

namespace aengine.graphics; 

public class Fog {

    public static void enable() {
        GL.Enable(EnableCap.Fog);
    }
    
    public static void create(float near = 5.0f, float far = 100.0f, FogMode mode = FogMode.Linear) {
        GL.Fog(FogParameter.FogMode, (int)mode);
        GL.Fog(FogParameter.FogStart, near);
        GL.Fog(FogParameter.FogEnd, far);
    }

    public static void create(int id, Vector3 position, Color color, float near = 5.0f, float far = 100.0f, FogMode mode = FogMode.Linear) {
        GL.Fog(FogParameter.FogMode, (int)mode);
        GL.Fog(FogParameter.FogIndex, id);
        
        float[] pos = { position.X, position.Y, position.Z };
        GL.Fog(FogParameter.FogCoordSrc, pos);
        
        float[] clr = { color.r, color.g, color.b, color.a };
        GL.Fog(FogParameter.FogColor, clr);
        
        GL.Fog(FogParameter.FogStart, near);
        GL.Fog(FogParameter.FogEnd, far);
    }
    
}
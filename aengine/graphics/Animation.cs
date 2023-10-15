using System.Numerics;
using Raylib_CsLo;

namespace aengine.graphics;

public class Animation {
    public Texture texture;
    
    public int frame;
    public int speed;
    public int frameCount;
    public int frameCounter;
    
    public Vector2 frameSize;
}
using System.Numerics;
using Raylib_CsLo;

namespace aengine.graphics;

public struct Animation
{
    public Texture texture;
    public int frame;
    public int speed;
    public int frames;
    public int frameCounter;
    public Vector2 frameSize;
}
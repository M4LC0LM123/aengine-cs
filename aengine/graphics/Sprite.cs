using static Raylib_CsLo.Raylib;
using Raylib_CsLo;
using System.Numerics;
namespace aengine.graphics;

public class Sprite
{
    public Texture texture;
    public Vector2 position;
    public Vector2 scale;
    public float rotation;
    public Color color;
    public Color tint;
    public float frame;
    public Vector2 frameScale;
    public float originX;
    public float originY;
    public bool flipH;
    public bool flipV;
    public bool isAnimActive;

    public Sprite()
    {
        texture = new Texture();
        position = Vector2.Zero;
        scale = Vector2.One * 40;
        rotation = 0;
        color = WHITE;
        tint = WHITE;
        frame = 0;
        frameScale = Vector2.Zero;
        originX = 0;
        originY = 0;
        flipH = false;
        flipV = false;
        isAnimActive = false;
    }
    
    public Sprite(Texture texture)
    {
        this.texture = texture;
        position = Vector2.Zero;
        scale = Vector2.One * 40;
        rotation = 0;
        color = WHITE;
        tint = WHITE;
        frame = 0;
        frameScale = Vector2.Zero;
        originX = 0;
        originY = 0;
        flipH = false;
        flipV = false;
        isAnimActive = false;
    }

    public void setFrame(Vector2 frameScale, float frame)
    {
        this.frame = frame;
        this.frameScale = frameScale;
    }
    
    public void animate(Animation animation) {
        animation.frameCounter++;
        if (animation.frameCounter >= GetFPS() / animation.speed) {
            Console.WriteLine("animating");
            animation.frameCounter = 0;
            animation.frame++;
            if (animation.frame > animation.frames)
                animation.frame = 0;
        }
        if (!flipH && !flipV)
            DrawTexturePro(animation.texture, new Rectangle(animation.frame * animation.frameSize.X, 0, animation.frameSize.X, animation.frameSize.Y), new Rectangle(position.X, position.Y, scale.X, scale.Y), new Vector2(originX, originY), rotation, tint);
        else if (flipH && !flipV)
            DrawTexturePro(animation.texture, new Rectangle((animation.frame * animation.frameSize.X) + animation.frameSize.X, 0, -animation.frameSize.X, animation.frameSize.Y), new Rectangle(position.X, position.Y, scale.X, scale.Y), new Vector2(originX, originY), rotation, tint);
        else if (!flipH && flipV)
            DrawTexturePro(animation.texture, new Rectangle(animation.frame * animation.frameSize.X, animation.frameSize.Y, animation.frameSize.X, -animation.frameSize.Y), new Rectangle(position.X, position.Y, scale.X, scale.Y), new Vector2(originX, originY), rotation, tint);
        else if (flipH && flipV)
            DrawTexturePro(animation.texture, new Rectangle((animation.frame * animation.frameSize.X) + animation.frameSize.X, animation.frameSize.Y, -animation.frameSize.X, -animation.frameSize.Y), new Rectangle(position.X, position.Y, scale.X, scale.Y), new Vector2(originX, originY), rotation, tint);
        isAnimActive = true;
    }
    
    public void render()
    {
        DrawRectanglePro(new Rectangle(position.X, position.Y, scale.X, scale.Y), new Vector2(originX, originY), rotation, color);
        
        if (!isAnimActive)
        {
            if (!flipH && !flipV)
                DrawTexturePro(texture, new Rectangle(frame * frameScale.X, 0, frameScale.X, frameScale.Y), new Rectangle(position.X, position.Y, scale.X, scale.Y), new Vector2(originX, originY), rotation, tint);
            else if (flipH && !flipV)
                DrawTexturePro(texture, new Rectangle(frame * frameScale.X + frameScale.X, 0, -frameScale.X, frameScale.Y), new Rectangle(position.X, position.Y, scale.X, scale.Y), new Vector2(originX, originY), rotation, tint);
            if (!flipH && flipV)
                DrawTexturePro(texture, new Rectangle(frame * frameScale.X, frameScale.Y, frameScale.X, -frameScale.Y), new Rectangle(position.X, position.Y, scale.X, scale.Y), new Vector2(originX, originY), rotation, tint);
            else if (flipH && flipV)
                DrawTexturePro(texture, new Rectangle(frame * frameScale.X + frameScale.X, frameScale.Y, -frameScale.X, -frameScale.Y), new Rectangle(position.X, position.Y, scale.X, scale.Y), new Vector2(originX, originY), rotation, tint);
        }
    }

    public void dispose()
    {
        UnloadTexture(texture);
    }
    
}
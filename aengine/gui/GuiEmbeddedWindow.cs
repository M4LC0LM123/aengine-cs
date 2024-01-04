using System.Numerics;
using aengine_cs.aengine.windowing;
using Raylib_CsLo;
using static Raylib_CsLo.Raylib;

namespace Sandbox.aengine.Gui; 

public class GuiEmbeddedWindow {
    public float x;
    public float y;
    public int width;
    public int height;
    public Vector2 mousePosition = Vector2.Zero;
    public RenderTexture target;

    public GuiEmbeddedWindow(int width, int height) {
        this.width = width;
        this.height = height;

        target = LoadRenderTexture(this.width, this.height);
    }

    public void beginRender() {
        BeginTextureMode(target);
        
        mousePosition.X = Window.mousePosition.X - x;
        mousePosition.Y = Window.mousePosition.Y - y;
    }
    
    public void endRender(float x, float y, GuiWindow window = null) {
        EndTextureMode();
        
        BeginDrawing();

        this.x = x;
        this.y = y;

        float rx = this.x;
        float ry = this.y;
        
        if (window != null) {
            rx = window.rec.x + this.x;
            ry = window.rec.y + this.y;
        }
        
        DrawTexturePro(target.texture, new Rectangle(0, 0, target.texture.width, -target.texture.height), 
            new Rectangle(rx, ry, width, height), Vector2.Zero, 0, WHITE);
        
        EndDrawing();
    }
    
    public void setWidth(int width) {
        this.width = width;
        reloadTarget();
    }
    
    public void setHeight(int height) {
        this.height = height;
        reloadTarget();
    }

    public void reloadTarget() {
        target = LoadRenderTexture(width, height);
    }
}
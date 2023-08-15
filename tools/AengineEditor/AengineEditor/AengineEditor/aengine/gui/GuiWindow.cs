using System.Numerics;
using Raylib_CsLo;
using static Raylib_CsLo.Raylib;

namespace Sandbox.aengine.Gui;

public class GuiWindow
{
    public Rectangle rec;
    public Rectangle topBar;
    public bool movable;
    public bool scalable;
    public string title;
    public float titleSize;
    
    private bool resizing;
    private Vector2 defaultScale;
    private bool moving;

    public int id;

    public GuiWindow(string title = "GuiWindow", float x = 10, float y = 10, float width = 300, float height = 200)
    {
        topBar = new Rectangle(x, y, width, Gui.topBarHeight);
        rec = new Rectangle(x, y + topBar.height, width, height);
        defaultScale = new Vector2(width, height);
        movable = true;
        scalable = true;
        this.title = title;
        titleSize = Gui.font.baseSize;
        
        Gui.windowCount++;
        id = Gui.windowCount;
    }

    public void setPosition(Vector2 positon)
    {
        topBar.x = positon.X;
        topBar.y = positon.Y;
    }
    
    public void setPosition(float x, float y)
    {
        topBar.x = x;
        topBar.y = y;
    }

    public Vector2 getPosition()
    {
        return new Vector2(topBar.x, topBar.y);
    }
    
    public void render()
    {
        if (IsMouseButtonDown(MouseButton.MOUSE_BUTTON_LEFT) && CheckCollisionPointRec(GetMousePosition(), topBar) && !Gui.GuiInteractiveRec(new GuiIcon(), topBar.x + topBar.width - Gui.exitScale, topBar.y, Gui.exitScale, Gui.exitScale, IsMouseButtonPressed(MouseButton.MOUSE_BUTTON_LEFT), null, false))
        {
            if (movable && Gui.activeWindowID == 0 || Gui.activeWindowID == id)
            {
                moving = true;
            }
        }

        if (moving)
        {
            Gui.activeWindowID = id;
            if (IsMouseButtonReleased(MouseButton.MOUSE_BUTTON_LEFT))
            {
                moving = false;
                Gui.activeWindowID = 0;
            }

            topBar.x = GetMousePosition().X - topBar.width/2;
            topBar.y = GetMousePosition().Y - topBar.height/2;
        }

        rec.x = topBar.x;
        rec.y = topBar.y + topBar.height;
        Gui.GuiPlainRec(topBar.x, topBar.y, topBar.width, topBar.height);
        Gui.GuiPlainRec(rec.x, rec.y, rec.width, rec.height);

        if (Gui.GuiInteractiveRec(new ExitIcon(), topBar.x + topBar.width - Gui.exitScale, topBar.y, Gui.exitScale, Gui.exitScale, IsMouseButtonPressed(MouseButton.MOUSE_BUTTON_LEFT)))
        {
            topBar.x = -5000;
            topBar.y = -5000;
        }
        
        Gui.GuiTextPro(Gui.font, title, new Vector2(topBar.x + 5, topBar.y), titleSize, WHITE);
        
        if (Gui.GuiInteractiveRec(new ResizeIcon(), rec.width - Gui.exitScale, rec.height - Gui.exitScale, Gui.exitScale, Gui.exitScale, IsMouseButtonDown(MouseButton.MOUSE_BUTTON_LEFT), this))
        {
            if (scalable)
            {
                resizing = true;
            }
        }

        if (resizing)
        {
            if (IsMouseButtonReleased(MouseButton.MOUSE_BUTTON_LEFT))
                resizing = false;
            
            rec.width = GetMousePosition().X - rec.x + 7.5f;
            rec.height = GetMousePosition().Y - rec.y + 7.5f;
            rec.width = Math.Clamp(rec.width, defaultScale.X, 1000);
            rec.height = Math.Clamp(rec.height, defaultScale.Y, 1000);
        }

        topBar.width = rec.width;
    }

}
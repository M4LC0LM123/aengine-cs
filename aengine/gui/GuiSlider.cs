using aengine_cs.aengine.windowing;
using Raylib_CsLo;
using static Raylib_CsLo.Raylib;
namespace Sandbox.aengine.Gui;

public class GuiSlider
{
    public int value;
    private bool sliding;
    private int min;
    private int max;

    public GuiSlider(int min = -100, int max = 100)
    {
        sliding = false;
        this.min = min;
        this.max = max;
    }

    public void render(float x, float y, float width, float height, GuiWindow window = null)
    {
        float rx = x + max;
        float ry = y;

        if (window != null)
        {
            rx = window.rec.x + x + max;
            ry = window.rec.y + y;
        }

        DrawLine((int)rx - max, (int)(ry + height / 2), (int)(rx - max * 1.25f + width), (int)(ry + height / 2),WHITE);

        Rectangle rec = new Rectangle(rx + value, ry, 15, height);
        if (CheckCollisionPointRec(Window.mousePosition, rec) && IsMouseButtonDown(MouseButton.MOUSE_BUTTON_LEFT))
        {
            sliding = true;
        }

        if (sliding)
        {
            if (IsMouseButtonReleased(MouseButton.MOUSE_BUTTON_LEFT))
                sliding = false;

            value = (int)(GetMouseX() - rx);
            value = Math.Clamp(value, min, max);
        }

        DrawRectangleRec(rec, Gui.lighterColor);
    }
    
}
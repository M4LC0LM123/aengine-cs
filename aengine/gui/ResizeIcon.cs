using static Raylib_CsLo.Raylib;

namespace Sandbox.aengine.Gui;

public class ResizeIcon : GuiIcon
{
    public void render(int x, int y, int width, int height)
    {
        DrawLine(x, y + height, x + width, y, WHITE);
        DrawLine(x + width/2, y + height, x + width, y + height/2, WHITE);
    }
}
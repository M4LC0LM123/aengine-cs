using static Raylib_CsLo.Raylib;

namespace Sandbox.aengine.Gui;

public class ExitIcon : GuiIcon
{
    public override void render(int x, int y, int width, int height)
    {
        base.render(x, y, width, height);
        DrawLine(x, y, x + width, y + height, WHITE);
        DrawLine(x, y + height, x + width, y, WHITE);
    }
}
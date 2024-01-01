using Raylib_CsLo;
using static Raylib_CsLo.Raylib;

namespace Sandbox.aengine.Gui; 

public class FileIcon : GuiIcon{
    public void render(int x, int y, int width, int height) {
        DrawLine(x, y + height, x + width, y + height, WHITE);
        DrawLine(x + width, y + height, x + width, y, WHITE);
        DrawLine(x, y, x + width, y, WHITE);
        DrawLine(x, y, x, y + height, WHITE);
    }
}
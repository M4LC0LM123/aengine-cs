using System.Numerics;
using aengine_cs.aengine.windowing;
using Raylib_CsLo;
using static Raylib_CsLo.Raylib;

namespace Sandbox.aengine.Gui;

public class GuiTextBox
{
    public string text = string.Empty;
    public bool active;

    public void render(float x, float y, float width, float height, GuiWindow window = null)
    {
        float rx = x;
        float ry = y;

        if (window != null)
        {
            rx = window.rec.x + x;
            ry = window.rec.y + y;
        }

        Rectangle temp = new Rectangle(rx, ry, width, height);
        // rec = temp;
        if (CheckCollisionPointRec(Window.mousePosition, temp) && IsMouseButtonPressed(MouseButton.MOUSE_BUTTON_LEFT))
        {
            active = !active;
        }

        float textScale = (temp.height - Gui.bezelSize * 2.0f) / Gui.font.baseSize;
        
        if (textScale * Gui.font.baseSize > temp.width - Gui.bezelSize * 2.0f) {
            textScale = (temp.width - Gui.bezelSize * 2.0f) / Gui.font.baseSize;
        }
        float textY = ry + (temp.height - Gui.font.baseSize * textScale) / 2;
        
        float textX = rx + Gui.bezelSize;

        Gui.GuiInverseRec(rx, ry, temp.width, temp.height);
        
        Gui.GuiTextPro(Gui.font, text + "_", new Vector2(textX, textY), Gui.font.baseSize * textScale, WHITE);

        if (active)
        {
            // DrawLine((int)textX, (int)(textY + Gui.font.baseSize * textScale), (int)(textX + temp.width - 5), (int)(textY + Gui.font.baseSize * textScale), WHITE);
            // DrawRectangle((int)(textX + text.Length * 15), (int)textY, 1, (int)temp.height, WHITE);
            
            // DrawCircle((int)textX, (int)textY, 5, RED);
            
            if (IsKeyPressed(KeyboardKey.KEY_BACKSPACE))
            {
                if (text.Length > 0)
                    text = text.Remove(text.Length - 1);
            }
            else
            {
                int key = GetCharPressed();
                if (key >= 32 && key <= 125)
                {
                    text += (char)key;
                }
            }

            if (!CheckCollisionPointRec(Window.mousePosition, temp) && IsMouseButtonPressed(MouseButton.MOUSE_BUTTON_LEFT))
                active = false;

            if (IsKeyPressed(KeyboardKey.KEY_ESCAPE))
                active = false;
        }
    }
    
}
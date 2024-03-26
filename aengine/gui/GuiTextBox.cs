using System.Numerics;
using aengine_cs.aengine.misc;
using aengine_cs.aengine.windowing;
using Raylib_CsLo;
using static Raylib_CsLo.Raylib;

namespace Sandbox.aengine.Gui;

public class GuiTextBox {
    public string text = string.Empty;
    public bool active;
    public int pos;
    
    public void render(float x, float y, float width, float height, GuiWindow window = null) {
        float rx = x;
        float ry = y;

        if (window != null) {
            rx = window.rec.x + x;
            ry = window.rec.y + y;
        }

        Rectangle temp = new Rectangle(rx, ry, width, height);
        // rec = temp;
        if (CheckCollisionPointRec(Window.mousePosition, temp) && IsMouseButtonPressed(MouseButton.MOUSE_BUTTON_LEFT)) {
            active = !active;
        }

        float textScale = (temp.height - Gui.bezelSize * 2.0f) / Gui.font.baseSize;

        if (textScale * Gui.font.baseSize > temp.width - Gui.bezelSize * 2.0f) {
            textScale = (temp.width - Gui.bezelSize * 2.0f) / Gui.font.baseSize;
        }

        float textY = ry + (temp.height - Gui.font.baseSize * textScale) / 2;

        float textX = rx + Gui.bezelSize;

        Gui.GuiInverseRec(rx, ry, temp.width, temp.height);

        if (!OperatingSystem.IsMacOS()) {
            BeginScissorMode((int)rx, (int)ry, (int)temp.width, (int)temp.height);    
        }
        
        Gui.GuiTextPro(Gui.font, text, new Vector2(textX, textY), Gui.font.baseSize * textScale, WHITE);
        
        // cursor
        if (active) {
            float charLen = 0;
            float textLen = MeasureTextEx(Gui.font, text, Gui.font.baseSize * textScale, 2).X;
            
            if (text.Length != 0) charLen = MeasureTextEx(Gui.font, text[(text.Length - pos - 1) % text.Length].ToString(),
                Gui.font.baseSize, 2).X;
            
            float curX = textX + (textLen - pos * charLen);
            
            Gui.GuiTextPro(Gui.font, Gui.cursorShape.ToString(),
                new Vector2(curX, textY),
                Gui.font.baseSize * textScale, WHITE);
        }
        
        if (!OperatingSystem.IsMacOS()) {
            EndScissorMode();    
        }

        if (active) {
            if (IsKeyDown(KeyboardKey.KEY_LEFT_CONTROL) || IsKeyDown(KeyboardKey.KEY_LEFT_SUPER)) {
                if (IsKeyPressed(KeyboardKey.KEY_V)) text += GetClipboardText_();
                if (IsKeyPressed(KeyboardKey.KEY_C)) SetClipboardText(text);
            }
            
            if (IsKeyPressed(KeyboardKey.KEY_RIGHT)) {
                if (pos > 0) {
                    pos--;
                }
            }
            
            if (IsKeyPressed(KeyboardKey.KEY_LEFT)) {
                if (pos < text.Length - 1) {
                    pos++;
                }
            }

            if (IsKeyPressed(KeyboardKey.KEY_BACKSPACE)) {
                if (text.Length > 0) {
                    if (pos > 0) pos--;
                    text = text.Remove(text.Length - 1);
                }
            }
            else {
                int key = GetCharPressed();
                if (key >= 32 && key <= 125) {
                    text += (char)key;
                }
            }

            if (!CheckCollisionPointRec(Window.mousePosition, temp) &&
                IsMouseButtonPressed(MouseButton.MOUSE_BUTTON_LEFT))
                active = false;

            if (IsKeyPressed(KeyboardKey.KEY_ESCAPE))
                active = false;
        }
    }
}
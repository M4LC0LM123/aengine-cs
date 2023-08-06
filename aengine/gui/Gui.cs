using System;
using System.Numerics;
using Raylib_CsLo;
using static Raylib_CsLo.Raylib;
namespace Sandbox.aengine.Gui;

public class Gui
{
    public static Color mainColor = new Color(99, 141, 160, 255);
    public static Color accentColor = new Color(63, 105, 135, 255);
    public static Color darkerColor = new Color(41, 59, 68, 255);
    public static Color lighterColor = new Color(119, 169, 191, 255);

    public static float bezelSize = 2.5f;
    public static float textSpacing = 2;
    public static float topBarHeight = 30;
    public static float exitScale = 25;

    public static Font font = GetFontDefault();

    public static int windowCount = 0;
    public static int activeWindowID = 0;

    public static void GuiText(string text, float x, float y, float size, Color color)
    {
        DrawText(text, x, y, size, color);
    }

    public static void GuiPlainRec(float x, float y, float width, float height)
    {
        DrawRectangle((int)(x - bezelSize), (int)(y - bezelSize), (int)(width + bezelSize*2f), (int)(height + bezelSize*2f), darkerColor);
        DrawRectangle((int)(x - bezelSize), (int)(y - bezelSize), (int)(width + bezelSize), (int)(height + bezelSize), lighterColor);
        DrawRectangle((int)x, (int)y, (int)width, (int)height, mainColor);
    }
    
    public static void GuiInverseRec(float x, float y, float width, float height)
    {
        DrawRectangle((int)(x - bezelSize), (int)(y - bezelSize), (int)(width + bezelSize*2f), (int)(height + bezelSize*2f), lighterColor);
        DrawRectangle((int)(x - bezelSize), (int)(y - bezelSize), (int)(width + bezelSize), (int)(height + bezelSize), darkerColor);
        DrawRectangle((int)x, (int)y, (int)width, (int)height, mainColor);
    }

    public static void GuiTextPro(Font font, string text, Vector2 position, float size, Color color, GuiWindow window = null)
    {
        Vector2 rp = position;
        
        if (window != null)
        {
            rp = new Vector2(position.X + window.rec.x, position.Y + window.rec.y);
        }
        DrawTextEx(Gui.font, text, rp, size, textSpacing, color);
    }

    public static bool GuiButton(string text, float x, float y, float width, float height, GuiWindow window = null, Positioning positioning = Positioning.CENTER)
    {
        float rx = x;
        float ry = y;

        if (window != null)
        {
            rx = window.rec.x + x;
            ry = window.rec.y + y;
        }
        
        Rectangle rec = new Rectangle(rx, ry, width, height);
        bool pressed = CheckCollisionPointRec(GetMousePosition(), rec) && IsMouseButtonReleased(MouseButton.MOUSE_BUTTON_LEFT);
        bool held = CheckCollisionPointRec(GetMousePosition(), rec) && IsMouseButtonDown(MouseButton.MOUSE_BUTTON_LEFT);

        DrawRectangle((int)(rx - bezelSize), (int)(ry - bezelSize), (int)(width + bezelSize*2f), (int)(height + bezelSize*2f), darkerColor);
        DrawRectangle((int)(rx - bezelSize), (int)(ry - bezelSize), (int)(width + bezelSize), (int)(height + bezelSize), lighterColor);

        if (!held)
            DrawRectangleRec(rec, mainColor);
        else 
            DrawRectangleRec(rec, accentColor);

        if (positioning == Positioning.CENTER)
        {
            float textScale = (width - bezelSize * 2f) / MeasureText(text, font.baseSize);
                    
            if (textScale * font.baseSize > height - bezelSize * 2f) {
                textScale = (height - bezelSize * 2f) / font.baseSize;
            }
                    
            float textX = rx + (width - MeasureText(text, (int)(font.baseSize * textScale))) / 2;
            float textY = ry + (height - font.baseSize * textScale) / 2;
            
            GuiTextPro(font, text, new Vector2(textX, textY), font.baseSize * textScale, WHITE);
        }
        else if (positioning == Positioning.LEFT)
        {
            float textScale = (height - bezelSize * 2.0f) / font.baseSize;
        
            if (textScale * font.baseSize > width - bezelSize * 2.0f) {
                textScale = (width - bezelSize * 2.0f) / font.baseSize;
            }
            float textY = ry + (height - font.baseSize * textScale) / 2;
        
            float textX = rx + bezelSize;
            
            GuiTextPro(font, text, new Vector2(textX, textY), font.baseSize * textScale, WHITE);
        }

        return pressed;
    }

    public static bool GuiInteractiveRec(GuiIcon icon, float x, float y, float width, float height, bool pressMode, GuiWindow window = null, bool show = true)
    {
        float rx = x;
        float ry = y;

        if (window != null)
        {
            rx = window.rec.x + x;
            ry = window.rec.y + y;
        }
        
        Rectangle rec = new Rectangle(rx, ry, width, height);
        bool pressed = CheckCollisionPointRec(GetMousePosition(), rec) && pressMode;

        if (show)
        {
            icon.render((int)rec.x, (int)rec.y, (int)rec.width, (int)rec.height);
        }

        return pressed;
    }

}
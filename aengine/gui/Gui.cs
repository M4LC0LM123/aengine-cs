using System;
using System.Text.RegularExpressions;
using aengine.graphics;
using aengine.input;

namespace aengine.gui
{
    public class Gui 
    {
        public static float FONT_SIZE = 0.25f;
        
        public static Texture btn_released;
        public static Texture btn_pressed;
        public static Font font;
        public static Texture exit;
        public static Texture scale;

        public static bool Button(string text, Font font, Texture released, Texture pressed, float x, float y, float width, float height, Color color, GuiWindow window = null)
        {
            float rx = x;
            float ry = y;
            float rw = width;
            float rh = height;

            if (window != null)
            {
                rx = window.rect.x + x;
                ry = window.rect.y + y;
            }

            // Draw the button texture
            if (released != null) Rendering.drawTexturedRectangle(released, rx, ry, rw, rh, color);

            // Check if the mouse is over the button and it's pressed
            bool isMouseOver = GuiRect.isMouseOver(new GuiRect(rx, ry, rw/2, rh/2));
            if (isMouseOver && Input.IsMouseDown(OpenTK.Windowing.GraphicsLibraryFramework.MouseButton.Left))
            {
                // If the mouse is over the button and it's pressed, draw the pressed texture
                if (pressed != null) Rendering.drawTexturedRectangle(pressed, rx, ry, rw, rh, color);
            }

            // Measure the text width and height
            float textWidth = aengine.core.aengine.MeasureText(font, text, FONT_SIZE).X;
            float textHeight = aengine.core.aengine.MeasureText(font, text, FONT_SIZE).Y;

            // Calculate the centered position for the X-coordinate
            float centeredX = rx + (rw - textWidth) / 2;

            // Calculate the centered position for the Y-coordinate
            float centeredY = ry + (rh - textHeight) / 2 + textHeight; // Note the '+' instead of '-'

            // Draw the text at the centered position
            Rendering.drawText(font, text, centeredX - rw/2.25f, centeredY - rh/3f, FONT_SIZE, color);

            // Return true if the button is pressed
            return isMouseOver && Input.IsMouseDown(OpenTK.Windowing.GraphicsLibraryFramework.MouseButton.Left);
        }

        public static bool Button(string text, float x, float y, float width, float height, Color color, GuiWindow window = null)
        {
            float rx = x;
            float ry = y;
            float rw = width;
            float rh = height;

            if (window != null)
            {
                rx = window.rect.x + x;
                ry = window.rect.y + y;
            }

            // Draw the button texture
            if (btn_released != null) Rendering.drawTexturedRectangle(btn_released, rx, ry, rw, rh, color);

            // Check if the mouse is over the button and it's pressed
            bool isMouseOver = GuiRect.isMouseOver(new GuiRect(rx, ry, rw/2, rh/2));
            if (isMouseOver && Input.IsMouseDown(OpenTK.Windowing.GraphicsLibraryFramework.MouseButton.Left))
            {
                // If the mouse is over the button and it's pressed, draw the pressed texture
                if (btn_pressed != null) Rendering.drawTexturedRectangle(btn_pressed, rx, ry, rw, rh, color);
            }

            // Measure the text width and height
            float textWidth = aengine.core.aengine.MeasureText(font, text, FONT_SIZE).X;
            float textHeight = aengine.core.aengine.MeasureText(font, text, FONT_SIZE).Y;

            // Calculate the centered position for the X-coordinate
            float centeredX = rx + (rw - textWidth) / 2;

            // Calculate the centered position for the Y-coordinate
            float centeredY = ry + (rh - textHeight) / 2 + textHeight; // Note the '+' instead of '-'

            // Draw the text at the centered position
            Rendering.drawText(font, text, centeredX - rw/2.25f, centeredY - rh/3f, FONT_SIZE, color);

            // Return true if the button is pressed
            return isMouseOver && Input.IsMouseDown(OpenTK.Windowing.GraphicsLibraryFramework.MouseButton.Left);
        }

    }
}
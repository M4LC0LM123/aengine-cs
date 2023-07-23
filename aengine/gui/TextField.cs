using System;
using aengine.graphics;
using aengine.input;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace aengine.gui
{
    public class TextField 
    {
        public string text;
        private bool isActive;

        public TextField()
        {
            text = String.Empty;
            isActive = false;
        }

        public void render(float x, float y, float width, float height, Color color, GuiWindow window = null)
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

            bool isMouseOver = GuiRect.isMouseOver(new GuiRect(rx, ry, rw/2, rh/2));
            if (isMouseOver && Input.IsMouseDown(OpenTK.Windowing.GraphicsLibraryFramework.MouseButton.Left))
            {
                isActive = !isActive;
            }

            if (isActive)
            {
                Rendering.drawTexturedRectangle(null, rx-2.5f, ry-2.5f, rw+7.5f, rh+7.5f, Colors.WHITE);
                if (Input.IsAnyKeyPressed() && !Input.IsKeyDown(Keys.Backspace))
                    text = text.Insert(text.Length, Input.GetKeyChar().ToString());

                if (Input.IsKeyDown(Keys.Backspace))
                {
                    if (text.Length > 0)
                        text = text.Remove(text.Length-1);
                }

                if (Input.IsKeyDown(Keys.Enter))
                {
                    Console.WriteLine(text);
                }
            }

            if (Gui.btn_pressed != null) Rendering.drawTexturedRectangle(Gui.btn_pressed, rx, ry, rw, rh, color);

            // Measure the text width and height
            float textWidth = aengine.core.aengine.MeasureText(Gui.font, text, Gui.FONT_SIZE).X;
            float textHeight = aengine.core.aengine.MeasureText(Gui.font, text, Gui.FONT_SIZE).Y;

            // Calculate the centered position for the X-coordinate
            float centeredX = rx + (rw - textWidth) / 2;

            // Calculate the centered position for the Y-coordinate
            float centeredY = ry + (rh - textHeight) / 2 + textHeight; 

            // draw text cursor
            // Rendering.drawTexturedRectangle(null, (rx + 5) + text.Length, ry + rh/5, 3.5f, rh/3, Colors.WHITE);

            // Draw the text at the centered position
            Rendering.drawText(Gui.font, text, centeredX - rw/2.25f, centeredY - rh/3f, Gui.FONT_SIZE, color);
        }

    }
}
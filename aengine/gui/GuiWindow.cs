using System;
using System.Drawing;
using System.Numerics;
using aengine.graphics;
using aengine.input;
using OpenTK.Graphics.OpenGL;

namespace aengine.gui
{
    public class GuiWindow 
    {
        public static float EXIT_SCALE = 30;
        public GuiRect rect;
        private Vector2 defaultScale;

        public GuiWindow(float x, float y, float width, float height)
        {
            rect = new GuiRect(x, y, width, height);
            defaultScale = new Vector2(width, height);
        }

        public void update()
        {
            if (Input.IsMouseDown(OpenTK.Windowing.GraphicsLibraryFramework.MouseButton.Left) && GuiRect.isMouseOver(rect) && graphics.Graphics.getMousePos().Y <= rect.y + 10)
            {
                rect.x = graphics.Graphics.getMousePos().X - 7.5f;
                rect.y = graphics.Graphics.getMousePos().Y - 7.5f;
            }
        }

        public void render()
        {
            aengine.graphics.Rendering.drawTexturedRectangle(null, rect.x, rect.y, rect.width, rect.height, Colors.GUI_DEFAULT);

            if (Gui.Button(String.Empty, Gui.font, Gui.exit, null, rect.width/2 - EXIT_SCALE/2, 0, EXIT_SCALE, EXIT_SCALE, Colors.WHITE, this))
            {
                this.rect.x = -500;
                this.rect.y = -500;
            }
            if (Gui.Button(String.Empty, Gui.font, Gui.scale, Gui.scale, rect.width/2 - EXIT_SCALE/2, rect.height/2 - EXIT_SCALE/2, EXIT_SCALE, EXIT_SCALE, Colors.WHITE, this))
            {
                this.rect.width = ((graphics.Graphics.getMousePos().X - this.rect.x) * 2f) + 7.5f;
                this.rect.height = ((graphics.Graphics.getMousePos().Y - this.rect.y) * 2f) + 7.5f;
                this.rect.width = Math.Clamp(this.rect.width, this.defaultScale.X, 1000);
                this.rect.height = Math.Clamp(this.rect.height, this.defaultScale.Y, 1000);
            }
        }
    }
}
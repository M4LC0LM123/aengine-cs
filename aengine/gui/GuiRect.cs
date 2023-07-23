using System;

namespace aengine.gui
{
    public struct GuiRect 
    {
        public float x;
        public float y;
        public float width;
        public float height;

        public GuiRect(float x, float y, float width, float height)
        {
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;
        }

        public static bool isMouseOver(GuiRect rect)
        {
            float mx = graphics.Graphics.getMousePos().X;
            float my = graphics.Graphics.getMousePos().Y;
            return mx >= rect.x && mx <= rect.x + rect.width &&    
                my >= rect.y && my <= rect.y + rect.height;
        }

    }
}
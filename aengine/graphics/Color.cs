namespace aengine.graphics
{
    public struct Color
    {
        public float r = 0;
        public float g = 0;
        public float b = 0;
        public float a = 0;

        public Color(float r, float g, float b, float a)
        {
            this.r = r;
            this.g = g;
            this.b = b;
            this.a = a;
        }
    }

    public class Colors 
    {
        public static Color RED = new Color(1.0f, 0.0f, 0.0f, 1.0f);
        public static Color GREEN = new Color(0.0f, 1.0f, 0.0f, 1.0f);
        public static Color BLUE = new Color(0.0f, 0.0f, 1.0f, 1.0f);
        public static Color YELLOW = new Color(1.0f, 1.0f, 0.0f, 1.0f);
        public static Color CYAN = new Color(0.0f, 1.0f, 1.0f, 1.0f);
        public static Color MAGENTA = new Color(1.0f, 0.0f, 1.0f, 1.0f);
        public static Color WHITE = new Color(1.0f, 1.0f, 1.0f, 1.0f);
        public static Color BLACK = new Color(0.0f, 0.0f, 0.0f, 1.0f);
        public static Color ORANGE = new Color(1.0f, 0.5f, 0.0f, 1.0f);
        public static Color PURPLE = new Color(0.5f, 0.0f, 0.5f, 1.0f);
        public static Color PINK = new Color(1.0f, 0.5f, 0.5f, 1.0f);
        public static Color LIME = new Color(0.0f, 1.0f, 0.5f, 1.0f);
        public static Color TEAL = new Color(0.0f, 0.5f, 0.5f, 1.0f);
        public static Color BROWN = new Color(0.6f, 0.4f, 0.2f, 1.0f);
        public static Color GRAY = new Color(0.5f, 0.5f, 0.5f, 1.0f);
        public static Color NAVY = new Color(0.0f, 0.0f, 0.5f, 1.0f);
        public static Color MAROON = new Color(0.5f, 0.0f, 0.0f, 1.0f);
        public static Color OLIVE = new Color(0.5f, 0.5f, 0.0f, 1.0f);

        public static Color GUI_DEFAULT = new Color(0.38f, 0.55f, 0.62f, 1.0f);

        public static Color BLANK = new Color(0.0f, 0.0f, 0.0f, 0.0f);
    }

}
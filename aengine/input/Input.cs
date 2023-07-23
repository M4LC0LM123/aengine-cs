using System;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common; 
using OpenTK.Windowing.Desktop;
using aengine.graphics;

namespace aengine.input
{
    public class Input 
    {
        private static bool[] keys = new bool[(int)Keys.LastKey];

        public static unsafe bool IsKeyDown(Keys key)
        {
            return GLFW.GetKey(graphics.Graphics.window, key) == InputAction.Press;
        }

        public static unsafe bool IsKeyReleased(Keys key)
        {
            return GLFW.GetKey(graphics.Graphics.window, key) == InputAction.Release;
        }

        public static unsafe bool IsMouseDown(MouseButton mouseButton)
        {
            return GLFW.GetMouseButton(graphics.Graphics.window, mouseButton) == InputAction.Press;
        }

        public static unsafe bool IsMouseReleased(MouseButton mouseButton)
        {
            return GLFW.GetMouseButton(graphics.Graphics.window, mouseButton) == InputAction.Release;
        }

        public static bool IsKeyPressed(Keys key)
        {
            return keys[(int)key];
        }

        public static char GetKeyChar()
        {
            return Graphics.GetKeyChar();
        }

        public static bool IsAnyKeyPressed()
        {
            return Graphics.isKeyPressed;
        }

    }
}
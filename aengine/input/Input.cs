using System;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common; 
using OpenTK.Windowing.Desktop;

namespace aengine.input
{
    public class Input 
    {
        public static unsafe bool IsKeyDown(Keys key)
        {
            if (GLFW.GetKey(graphics.Graphics.window, key) == InputAction.Press)
            {
                return true;
            }
            return false;
        }

        public static unsafe bool IsKeyReleased(Keys key)
        {
            if (GLFW.GetKey(graphics.Graphics.window, key) == InputAction.Release)
            {
                return true;
            }
            return false;
        }

    }
}
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
        public static Dictionary<Keys, bool> keyStates = new Dictionary<Keys, bool>();

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

        public static unsafe bool IsKeyPressed(Keys key)
        {
            // Check if the key exists in the keyStates dictionary
            if (keyStates.TryGetValue(key, out bool isPressed))
            {
                return isPressed;
            }

            // Key was not found in the dictionary, treat as not pressed
            return false;
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
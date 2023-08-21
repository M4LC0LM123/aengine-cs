using OpenTK.Windowing.GraphicsLibraryFramework;
using Window = aengine.window.Window;

namespace aengine.input;

public class Input {
    public static Dictionary<Keys, bool> keyStates = new Dictionary<Keys, bool>();

    public delegate void keyDelegate(Keys key);
    public static keyDelegate keyHandle;

    public delegate void mouseDelegate(MouseButton mouseButton);
    public static mouseDelegate mouseHandle;
    
    public static unsafe bool isKeyDown(Keys key) {
        return GLFW.GetKey(Window.window, key) == InputAction.Press;
    }

    public static unsafe bool isKeyReleased(Keys key) {
        return GLFW.GetKey(Window.window, key) == InputAction.Release;
    }

    public static unsafe bool isMouseDown(MouseButton mouseButton) {
        return GLFW.GetMouseButton(Window.window, mouseButton) == InputAction.Press;
    }

    public static unsafe bool isMouseReleased(MouseButton mouseButton) {
        return GLFW.GetMouseButton(Window.window, mouseButton) == InputAction.Release;
    }

    public static char getKeyChar() {
        return Window.GetKeyChar();
    }
}
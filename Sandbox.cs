using OpenTK;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Graphics.OpenGL;
using Monitor = OpenTK.Windowing.GraphicsLibraryFramework.Monitor;

namespace Sandbox;

public class Sandbox {
    public static unsafe void Main(string[] args) {
        if (!GLFW.Init()) {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Failed to initialize GLFW");
            Console.ForegroundColor = ConsoleColor.White;
            return;
        }

        Window* window = GLFW.CreateWindow(800, 600, "learn opentk", (Monitor*)IntPtr.Zero, (Window*)IntPtr.Zero);

        if (window == (Window*)IntPtr.Zero) {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Failed to create GLFW window");
            Console.ForegroundColor = ConsoleColor.White;
            GLFW.Terminate();
            return;
        }
        
        GLFW.MakeContextCurrent(window);
        GL.LoadBindings(new GLFWBindingsContext());
        GL.Enable(EnableCap.DepthTest);
        GL.Viewport(0, 0, 800, 600);

        while (!GLFW.WindowShouldClose(window)) {
            GLFW.PollEvents();
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.ClearColor(0.07f, 0.13f, 0.17f, 1.0f);
            GLFW.SwapBuffers(window);
        }

        GLFW.DestroyWindow(window);
        GLFW.Terminate();
    }
}
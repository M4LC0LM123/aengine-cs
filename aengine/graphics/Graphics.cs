using System;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common; 
using OpenTK.Windowing.Desktop;

namespace aengine.graphics
{
    public unsafe class Graphics 
    {
        public static Window* window;
        private static double prevTime;
        private static double prevFPStime;
        private static float dt;
        private static int frameCount;
        private static int currFPS;

        // initialize GLFW window
        public static unsafe void init(int width, int height, string title)
        {
            if (!GLFW.Init())
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Failed to initialize GLFW");
                Console.ForegroundColor = ConsoleColor.White;
                return;
            }

            window = GLFW.CreateWindow(width, height, title, (OpenTK.Windowing.GraphicsLibraryFramework.Monitor*)IntPtr.Zero, (Window*)IntPtr.Zero);

            if (window == (Window*)IntPtr.Zero)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Failed to create GLFW window");
                Console.ForegroundColor = ConsoleColor.White;
                GLFW.Terminate();
                return;
            }

            prevTime = GLFW.GetTime();
            prevFPStime = GLFW.GetTime();
            frameCount = 0;

            GLFW.MakeContextCurrent(window);
            GL.LoadBindings(new GLFWBindingsContext());
            GL.Enable(EnableCap.DepthTest);
            GL.Viewport(0, 0, width, height);
        }

        // check if a window should close often used in a while loop
        public static bool WindowShouldClose()
        {
            return GLFW.WindowShouldClose(window);
        }

        // update for delta time and fps
        public static void update()
        {
            double currTime = GLFW.GetTime();
            dt = (float)(currTime - prevTime);
            prevTime = currTime;

            double currFPStime = GLFW.GetTime();
            frameCount++;

            if (currFPStime - prevFPStime >= 1.0f)
            {
                currFPS = frameCount;
                frameCount = 0;
                prevFPStime = currFPStime;
            }
        }

        // get delta time
        public static float getDeltaTime()
        {
            return dt;
        }

        // get current fps
        public static int getFPS()
        {
            return currFPS;
        }

        // set the window title 
        public static void setTitle(string title)
        {
            GLFW.SetWindowTitle(window, title);
        }

        // gets screen size (width and height)
        public static System.Numerics.Vector2 getScreenSize()
        {
            GLFW.GetWindowSize(window, out int width, out int height);
            return new System.Numerics.Vector2((float)width, (float)height);
        }

        // begin drawing (polls for window events(resizing, scaling, ...))
        // and clears the color and depth buffers
        public static void begin()
        {
            // poll for events
            GLFW.PollEvents();

            // clear the color and depth buffers
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
        }

        // call after drawing everything (essentially just swaps glfw buffers)
        public static void end()
        {
            GLFW.SwapBuffers(window);
        }

        // set projection and view matrix
        public static void setProjectionMatrix(Camera camera)
        {
            // set the projection matrix
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadMatrix(ref camera.projectionMatrix);

            // set the view matrix
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadMatrix(ref camera.viewMatrix);
        }

        // destroys the window and cleans up GLFW and opengl
        public static void dispose()
        {
            GLFW.DestroyWindow(window);
            GLFW.Terminate();
        }

    }
}
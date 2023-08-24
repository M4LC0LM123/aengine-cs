using System.Diagnostics;
using aengine.graphics;
using OpenTK;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Graphics.OpenGL;
using aengine.input;
using GLFW_WINDOW = OpenTK.Windowing.GraphicsLibraryFramework.Window;

namespace aengine.window;

public unsafe class Window {
    public static GLFW_WINDOW* window;
    private static double prevTime;
    private static double prevFPStime;
    private static float dt;
    private static int frameCount;
    private static int currFPS;
    private static int maxFPS;
    private static readonly Stopwatch timer = new Stopwatch();

    // event callbacks
    private static GLFWCallbacks.CharCallback charCallback;
    private static GLFWCallbacks.FramebufferSizeCallback framebufferSizeCallback;
    private static GLFWCallbacks.KeyCallback keyCallback;
    private static GLFWCallbacks.MouseButtonCallback mouseCallback;

    // initialize GLFW window
    public static void init(int width, int height, string title, int fps = 60) {
        timer.Start();

        if (!GLFW.Init()) {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Failed to initialize GLFW");
            Console.ForegroundColor = ConsoleColor.White;
            return;
        }

        Console.WriteLine("[aengine]: Successfully initialized GLFW window!");
        
        window = GLFW.CreateWindow(width, height, title,
            (OpenTK.Windowing.GraphicsLibraryFramework.Monitor*)IntPtr.Zero, (GLFW_WINDOW*)IntPtr.Zero);

        if (window == (GLFW_WINDOW*)IntPtr.Zero) {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Failed to create GLFW window");
            Console.ForegroundColor = ConsoleColor.White;
            GLFW.Terminate();
            return;
        }
        
        Console.WriteLine("[aengine]: Successfully created GLFW window!");

        prevTime = GLFW.GetTime();
        prevFPStime = GLFW.GetTime();
        frameCount = 0;

        maxFPS = fps;

        GLFW.MakeContextCurrent(window);
        GL.LoadBindings(new GLFWBindingsContext());
        GL.Enable(EnableCap.DepthTest);
        GL.Viewport(0, 0, width, height);

        Input.keyHandle += key => { };
        Input.mouseHandle += MouseButton => { };
        
        charCallback = CharCallback;
        GLFW.SetCharCallback(window, charCallback);

        framebufferSizeCallback = OnFramebufferSizeChanged;
        GLFW.SetFramebufferSizeCallback(window, framebufferSizeCallback);
        
        keyCallback = OnKeyPressed;
        GLFW.SetKeyCallback(window, keyCallback);

        mouseCallback = OnMousePressed;
        GLFW.SetMouseButtonCallback(window, mouseCallback);
    }

    // check if a window should close often used in a while loop
    public static bool windowShouldClose() {
        return GLFW.WindowShouldClose(window);
    }

    // update for delta time and fps
    public static void update() {
        double currTime = GLFW.GetTime();
        dt = (float)(currTime - prevTime);
        prevTime = currTime;

        double currFPStime = GLFW.GetTime();
        frameCount++;

        if (currFPStime - prevFPStime >= 1.0f) {
            currFPS = frameCount;
            frameCount = 0;
            prevFPStime = currFPStime;
        }

        currFPS = Math.Clamp(currFPS, 0, maxFPS);
    }

    // get delta time
    public static float getDeltaTime() {
        return dt;
    }

    // get current fps
    public static int getFPS() {
        return currFPS;
    }

    public static void setMaxFPS(int fps) {
        maxFPS = fps;
    }

    public static Stopwatch getTimer() {
        return timer;
    }

    // gets mouse pos in 2d space 
    public static System.Numerics.Vector2 getMousePos() {
        GLFW.GetCursorPos(window, out double mx, out double my);
        return new System.Numerics.Vector2((float)mx, (float)my);
    }

    // set the window title 
    public static void setTitle(string title) {
        GLFW.SetWindowTitle(window, title);
    }

    // gets screen size (width and height)
    public static System.Numerics.Vector2 getScreenSize() {
        GLFW.GetWindowSize(window, out int width, out int height);
        return new System.Numerics.Vector2((float)width, (float)height);
    }

    public static void setIcon(Texture icon) {
        fixed (byte* bytePtr = icon.data) {
            Image image = new Image(icon.width, icon.height, bytePtr);
            GLFW.SetWindowIconRaw(window, 1, &image);
        }
    }

    public static void setScreenSize(int width, int height) {
        GLFW.SetWindowSize(window, width, height);
    }

    // begin drawing (polls for window events(resizing, scaling, ...))
    // and clears the color and depth buffers
    public static void begin() {
        // poll for events
        GLFW.PollEvents();

        // clear the color and depth buffers
        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
    }

    // call after drawing everything (essentially just swaps glfw buffers)
    public static void end() {
        GLFW.SwapBuffers(window);
    }

    // set projection and view matrix
    public static void setProjectionMatrix(Camera camera) {
        // set the projection matrix
        GL.MatrixMode(MatrixMode.Projection);
        GL.LoadMatrix(ref camera.projectionMatrix);

        // set the view matrix
        GL.MatrixMode(MatrixMode.Modelview);
        GL.LoadMatrix(ref camera.viewMatrix);
    }

    private static void CharCallback(GLFW_WINDOW* window, uint codepoint) {
        char character = (char)codepoint;
        lastInputChar = character;
    }

    private static char lastInputChar = '\0';

    public static char GetKeyChar() {
        char inputChar = lastInputChar;
        lastInputChar = '\0';
        return inputChar;
    }

    private static void OnFramebufferSizeChanged(GLFW_WINDOW* window, int width, int height) {
        GL.Viewport(0, 0, width, height);
    }

    private static void OnKeyPressed(GLFW_WINDOW* window, Keys keys, int scanCode, InputAction state, KeyModifiers mods) {
        Input.keyHandle.Invoke(keys);
    }

    private static void OnMousePressed(GLFW_WINDOW* window, MouseButton button, InputAction action, KeyModifiers modifiers) {
        Input.mouseHandle.Invoke(button);
    }

    // destroys the window and cleans up GLFW and opengl
    public static void dispose() {
        timer.Stop();
        GLFW.DestroyWindow(window);
        Console.WriteLine("[aengine]: successfully destroyed GLFW window");;
        GLFW.Terminate();
        Console.WriteLine("[aengine]: successfully closed GLFW window");
    }
}
using Raylib_CsLo;
using static Raylib_CsLo.Raylib;

namespace Sandbox
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            InitWindow(1280, 720, "Hello, Raylib-CsLo");
            SetTargetFPS(60);
            // Main game loop
            while (!WindowShouldClose()) // Detect window close button or ESC key
            {
                BeginDrawing();
                ClearBackground(SKYBLUE);
                DrawFPS(10, 10);
                DrawText("Raylib is easy!!!", 640, 360, 50, RED);
                EndDrawing();
            }
            CloseWindow();
        }
    }
}
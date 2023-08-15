using System.Numerics;
using aengine.graphics;
using aengine.graphics;
using Raylib_CsLo;
using Sandbox.aengine.Gui;
using static Raylib_CsLo.Raylib;

namespace Editor
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            SetConfigFlags(ConfigFlags.FLAG_WINDOW_RESIZABLE);
            SetTraceLogLevel((int)TraceLogLevel.LOG_ERROR);
            InitWindow(1280, 720, "aengine-editor");
            SetWindowIcon(LoadImage("logo.png"));
            SetExitKey(KeyboardKey.KEY_NULL);
            SetTargetFPS(60);

            Camera camera = new Camera(Vector3.One, 90);
            bool isMouseLocked = false;

            AxieMover mover = new AxieMover();

            ObjectManager manager = new ObjectManager();
            GuiTextBox textId = new GuiTextBox();
            textId.text = "0";

            // Main game loop
            while (!WindowShouldClose()) // Detect window close button or ESC key
            {
                manager.update(mover);

                if (IsKeyPressed(KeyboardKey.KEY_ESCAPE))
                    isMouseLocked = !isMouseLocked;
                
                camera.setFirstPerson(0.1f, isMouseLocked);
                camera.setDefaultFPSControls(10, isMouseLocked, true);
                camera.defaultFpsMatrix();
                camera.update();

                if (IsKeyPressed(KeyboardKey.KEY_F1))
                    AxieMover.CURRENT_MODE = Mode.ROAM;
                if (IsKeyPressed(KeyboardKey.KEY_F2))
                    AxieMover.CURRENT_MODE = Mode.MOVE;
                if (IsKeyPressed(KeyboardKey.KEY_F3))
                    AxieMover.CURRENT_MODE = Mode.SCALE;
                if (IsKeyPressed(KeyboardKey.KEY_F4))
                    AxieMover.CURRENT_MODE = Mode.ROTATE;
                
                if (IsKeyDown(KeyboardKey.KEY_LEFT_CONTROL) && IsKeyPressed(KeyboardKey.KEY_L) && !isMouseLocked)
                    manager.load();
                
                if (IsKeyDown(KeyboardKey.KEY_LEFT_CONTROL) && IsKeyPressed(KeyboardKey.KEY_S) && !isMouseLocked)
                    manager.save();
                
                mover.update(camera);
                
                if (IsKeyDown(KeyboardKey.KEY_LEFT_CONTROL) && IsKeyPressed(KeyboardKey.KEY_SPACE))
                    manager.addObject(AxieMover.CURRENT_ID, camera.position);

                BeginDrawing();
                ClearBackground(BLACK);
                BeginMode3D(camera.matrix);
                
                DrawGrid(100, 1);
                
                manager.render();
                
                if (AxieMover.IS_OBJ_ACTIVE)
                    mover.render();

                EndMode3D();
                DrawText(string.Format("fps: {0}", GetFPS()), 10, 5, 20, WHITE);
                DrawText(string.Format("mode: {0}", AxieMover.CURRENT_MODE.ToString()), 10, 30, 20, WHITE);
                DrawText(string.Format("id: {0}", AxieMover.CURRENT_ID), 10, 55, 20, WHITE);
                if (AxieMover.IS_OBJ_ACTIVE) DrawText(string.Format("object id: {0}", AxieMover.ACTIVE_OBJ.id), 10, 80, 20, WHITE);
                if (AxieMover.IS_OBJ_ACTIVE) DrawText(string.Format("object position: {0}, {1}, {2}", AxieMover.ACTIVE_OBJ.position.X, AxieMover.ACTIVE_OBJ.position.Y, AxieMover.ACTIVE_OBJ.position.Z), 10, 105, 20, WHITE);
                if (AxieMover.IS_OBJ_ACTIVE) DrawText(string.Format("object scale: {0}, {1}, {2}", AxieMover.ACTIVE_OBJ.scale.X, AxieMover.ACTIVE_OBJ.scale.Y, AxieMover.ACTIVE_OBJ.scale.Z), 10, 130, 20, WHITE);
                if (AxieMover.IS_OBJ_ACTIVE) DrawText(string.Format("object rotation: {0}, {1}, {2}", AxieMover.ACTIVE_OBJ.rotation.X, AxieMover.ACTIVE_OBJ.rotation.Y, AxieMover.ACTIVE_OBJ.rotation.Z), 10, 155, 20, WHITE);
                
                textId.render(10, 180, 100, 30);
                Int32.TryParse(textId.text, out AxieMover.CURRENT_ID);
                
                EndDrawing();
            }
            CloseWindow();
        }
    }
}
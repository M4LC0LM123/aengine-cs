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

                if (IsKeyPressed(KeyboardKey.KEY_RIGHT_ALT)) {
                    AxieMover.CAMERA_MODE++;

                    if (AxieMover.CAMERA_MODE == CameraMode.XY) {
                        camera.target.Z = 0;
                        camera.front = Vector3.UnitZ;
                        camera.up = Vector3.UnitY;
                        camera.position = Vector3.Zero with { Z = 10 };
                        camera.rotation = Vector3.Zero;
                    } else if (AxieMover.CAMERA_MODE == CameraMode.ZY) {
                        camera.target.X = 0;
                        camera.front = Vector3.UnitX;
                        camera.up = Vector3.UnitY;
                        camera.position = Vector3.Zero with { X = -10 };
                        camera.rotation = Vector3.Zero;
                    } else if (AxieMover.CAMERA_MODE == CameraMode.XZ) {
                        camera.target.Y = 0;
                        camera.front = Vector3.UnitY;
                        camera.up = Vector3.UnitZ;
                        camera.position = Vector3.Zero with { Y = 10 };
                        camera.rotation = Vector3.Zero;
                    }
                    
                    if ((int)AxieMover.CAMERA_MODE > 3) {
                        AxieMover.CAMERA_MODE = CameraMode.FPS;
                    }
                }
                
                if (AxieMover.CAMERA_MODE is CameraMode.FPS) {
                    camera.setFirstPerson(0.1f, isMouseLocked);
                    camera.setDefaultFPSControls(10, isMouseLocked, true);
                    camera.defaultFpsMatrix();
                } else if (AxieMover.CAMERA_MODE is CameraMode.XY) {
                    if (IsKeyDown(KeyboardKey.KEY_LEFT)) {
                        camera.position.X -= 10 * GetFrameTime();
                    }
                    if (IsKeyDown(KeyboardKey.KEY_RIGHT)) {
                        camera.position.X += 10 * GetFrameTime();
                    }
                    if (IsKeyDown(KeyboardKey.KEY_UP)) {
                        camera.position.Y += 10 * GetFrameTime();
                    }
                    if (IsKeyDown(KeyboardKey.KEY_DOWN)) {
                        camera.position.Y -= 10 * GetFrameTime();
                    }

                    if (camera.position.Z > 0) camera.position.Z -= GetMouseWheelMoveV().Y;
                    else camera.position.Z += GetMouseWheelMoveV().Y;
                    
                    camera.target.X = camera.position.X;
                    camera.target.Y = camera.position.Y;
                } else if (AxieMover.CAMERA_MODE is CameraMode.ZY) {
                    if (IsKeyDown(KeyboardKey.KEY_LEFT)) {
                        camera.position.Z -= 10 * GetFrameTime();
                    }
                    if (IsKeyDown(KeyboardKey.KEY_RIGHT)) {
                        camera.position.Z += 10 * GetFrameTime();
                    }
                    if (IsKeyDown(KeyboardKey.KEY_UP)) {
                        camera.position.Y += 10 * GetFrameTime();
                    }
                    if (IsKeyDown(KeyboardKey.KEY_DOWN)) {
                        camera.position.Y -= 10 * GetFrameTime();
                    }

                    if (camera.position.X > 0) camera.position.X -= GetMouseWheelMoveV().X;
                    else camera.position.X += GetMouseWheelMoveV().Y;
                    
                    camera.target.Z = camera.position.Z;
                    camera.target.Y = camera.position.Y;
                } else if (AxieMover.CAMERA_MODE is CameraMode.XZ) {
                    if (IsKeyDown(KeyboardKey.KEY_LEFT)) {
                        camera.position.X += 10 * GetFrameTime();
                    }
                    if (IsKeyDown(KeyboardKey.KEY_RIGHT)) {
                        camera.position.X -= 10 * GetFrameTime();
                    }
                    if (IsKeyDown(KeyboardKey.KEY_UP)) {
                        camera.position.Z += 10 * GetFrameTime();
                    }
                    if (IsKeyDown(KeyboardKey.KEY_DOWN)) {
                        camera.position.Z -= 10 * GetFrameTime();
                    }

                    if (camera.position.Y > 0) camera.position.Y -= GetMouseWheelMoveV().Y;
                    else camera.position.Y += GetMouseWheelMoveV().Y;
                    
                    camera.target.X = camera.position.X;
                    camera.target.Z = camera.position.Z;
                }
                
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

                if (IsKeyDown(KeyboardKey.KEY_LEFT_CONTROL) && IsKeyPressed(KeyboardKey.KEY_O) && !isMouseLocked)
                    manager.outlined = !manager.outlined;
                
                mover.update(camera);

                if (IsKeyDown(KeyboardKey.KEY_LEFT_CONTROL) && IsKeyPressed(KeyboardKey.KEY_SPACE)) {
                    if (AxieMover.CAMERA_MODE is CameraMode.FPS) {
                        manager.addObject(AxieMover.CURRENT_ID, camera.position);
                    } else {
                        manager.addObject(AxieMover.CURRENT_ID, camera.target);  
                    }
                }

                if (AxieMover.IS_OBJ_ACTIVE) {
                    if (IsKeyDown(KeyboardKey.KEY_LEFT_CONTROL) && IsKeyPressed(KeyboardKey.KEY_D)) {
                        Object newObj = new Object(AxieMover.CURRENT_ID, Vector3.Zero);
                        newObj.position = AxieMover.ACTIVE_OBJ.position with {
                            X = AxieMover.ACTIVE_OBJ.position.X + 1.5f,
                            Y = AxieMover.ACTIVE_OBJ.position.Y + 1.5f,
                            Z = AxieMover.ACTIVE_OBJ.position.Z + 1.5f,
                        };
                        newObj.scale = AxieMover.ACTIVE_OBJ.scale;
                        newObj.rotation = AxieMover.ACTIVE_OBJ.rotation;
                        
                        manager.objects.Add(newObj);
                    } 
                }

                BeginDrawing();
                ClearBackground(BLACK);
                BeginMode3D(camera.matrix);
                
                Rendering.drawDebugAxies(100);
                Rendering.drawDebugAxies(-100);
                
                if (AxieMover.CAMERA_MODE is CameraMode.XY) {
                    for (int i = 0; i < 100; i++) {
                        for (int j = 0; j < 100; j++) {
                            DrawCubeWiresV(Vector3.Zero with { X = j - 50, Y = i - 50}, Vector3.One with { Z = 0 }, Utils.semiWhite);
                        }
                    }
                } else if (AxieMover.CAMERA_MODE is CameraMode.ZY) {
                    for (int i = 0; i < 100; i++) {
                        for (int j = 0; j < 100; j++) {
                            DrawCubeWiresV(Vector3.Zero with { Z = j - 50, Y = i - 50}, Vector3.One with { X = 0 }, Utils.semiWhite);
                        }
                    }
                } else if (AxieMover.CAMERA_MODE is CameraMode.XZ) {
                    for (int i = 0; i < 100; i++) {
                        for (int j = 0; j < 100; j++) {
                            DrawCubeWiresV(Vector3.Zero with { X = j - 50, Z = i - 50}, Vector3.One with { Y = 0 }, Utils.semiWhite);
                        }
                    }
                }
                
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
                DrawText("objects: " + manager.objects.Count, 10, 180, 20, WHITE);
                
                textId.render(10, 205, 100, 30);
                Int32.TryParse(textId.text, out AxieMover.CURRENT_ID);
                
                EndDrawing();
            }
            CloseWindow();
        }
    }
}
using System.Numerics;
using aengine_cs.aengine.windowing;
using aengine.ecs;
using aengine.graphics;
using aengine.graphics;
using Raylib_CsLo;
using Sandbox.aengine.Gui;
using static Raylib_CsLo.Raylib;

namespace Editor
{
    public class Editor
    {
        public static void main()
        {
            Directory.SetCurrentDirectory("../../../");
            
            Window.create();
            Window.title = "aengine-editor";
            Window.renderWidth = 1280;
            Window.renderHeight = 720;
            Window.width = 1280;
            Window.height = 720;
            Window.traceLogLevel = TraceLogLevel.LOG_ALL;
            SetWindowIcon(LoadImage("assets/logo.png"));

            Camera camera = new Camera(Vector3.One, 90);
            bool isMouseLocked = false;

            AxieMover mover = new AxieMover();

            ObjectManager manager = new ObjectManager();
            GuiTextBox textId = new GuiTextBox();
            textId.text = "0";

            GuiWindow infoWindow = new GuiWindow("Editor", 0, 0, 200, 130);
            GuiWindow entityDataWindow = new GuiWindow("Entity data", 0, 100 + Gui.topBarHeight, 400, 300);
            
            // Main game loop
            while (!WindowShouldClose()) // Detect window close button or ESC key
            {
                Window.tick();
                World.update(false);
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
                    manager.load("scene");
                
                if (IsKeyDown(KeyboardKey.KEY_LEFT_CONTROL) && IsKeyPressed(KeyboardKey.KEY_S) && !isMouseLocked)
                    manager.saveJson();

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

                Window.beginRender();
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
                World.render();
                
                if (AxieMover.IS_OBJ_ACTIVE)
                    mover.render();

                EndMode3D();
                
                infoWindow.render();
                
                Gui.GuiTextPro(GetFontDefault(), 
                    "fps: " + GetFPS(), 
                    new Vector2(10, 10),
                    20, WHITE, infoWindow);

                Gui.GuiTextPro(GetFontDefault(), "mode: " + AxieMover.CURRENT_MODE, 
                    new Vector2(10, 30), 
                    20, WHITE, infoWindow);
                
                Gui.GuiTextPro(GetFontDefault(), "id: " + AxieMover.CURRENT_ID,
                    new Vector2(10, 50),
                    20,WHITE, infoWindow);
                
                Gui.GuiTextPro(GetFontDefault(), "entities: " + World.entities.Count, new Vector2(10, 70), 20, WHITE, infoWindow);
                
                textId.render(10, 95, 90, 30, infoWindow);
                Int32.TryParse(textId.text, out AxieMover.CURRENT_ID);
                
                if (AxieMover.IS_OBJ_ACTIVE) {
                    entityDataWindow.render();
                    
                    Gui.GuiTextPro(GetFontDefault(), "object id: " + AxieMover.ACTIVE_OBJ.id, 
                        new Vector2(10, 10), 
                        20, WHITE, entityDataWindow);
                    
                    Gui.GuiTextPro(GetFontDefault(), "object position: " + Utils.roundVectorDecimals(AxieMover.ACTIVE_OBJ.position, 2), 
                        new Vector2(10, 30), 
                        20, WHITE, entityDataWindow);
                    
                    Gui.GuiTextPro(GetFontDefault(), "object scale: " + AxieMover.ACTIVE_OBJ.scale, 
                        new Vector2(10, 50),
                        20, WHITE, entityDataWindow);
                    
                    Gui.GuiTextPro(GetFontDefault(), "object rotation: " + AxieMover.ACTIVE_OBJ.rotation, 
                        new Vector2(10, 70), 
                        20, WHITE, entityDataWindow);
                }
                
                Window.endRender();
            }
            CloseWindow();
        }
    }
}
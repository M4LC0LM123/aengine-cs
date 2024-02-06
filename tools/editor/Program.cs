using System.Numerics;
using aengine_cs.aengine.parser;
using aengine_cs.aengine.windowing;
using aengine.ecs;
using aengine.graphics;
using aengine.graphics;
using NativeFileDialogSharp;
using Raylib_CsLo;
using Sandbox.aengine.Gui;
using static Raylib_CsLo.Raylib;
using ae_Console = aengine.core.Console;

namespace Editor {
    public unsafe class Editor {
        public static GuiTextBox xPos = new GuiTextBox();
        public static GuiTextBox yPos = new GuiTextBox();
        public static GuiTextBox zPos = new GuiTextBox();

        public static GuiTextBox xScale = new GuiTextBox();
        public static GuiTextBox yScale = new GuiTextBox();
        public static GuiTextBox zScale = new GuiTextBox();

        public static GuiTextBox xRot = new GuiTextBox();
        public static GuiTextBox yRot = new GuiTextBox();
        public static GuiTextBox zRot = new GuiTextBox();

        public static void main() {
            Directory.SetCurrentDirectory("../../../");

            Window.create();
            Window.title = "aengine-editor";
            Window.renderWidth = GetMonitorWidth(GetCurrentMonitor());
            Window.renderHeight = GetMonitorHeight(GetCurrentMonitor());
            MaximizeWindow();
            Window.traceLogLevel = TraceLogLevel.LOG_NONE;
            Window.isEditor = true;
            SetWindowIcon(LoadImage("assets/logo.png"));

            Camera camera = new Camera(Vector3.One, 90);
            bool isMouseLocked = false;

            TransformGizmo mover = new TransformGizmo();

            ObjectManager manager = new ObjectManager();

            GuiWindow infoWindow = new GuiWindow("Editor", 0, 0, 180, 340);

            GuiWindow entityDataWindow = new GuiWindow("Entity data", 0, 110 + Gui.topBarHeight, 400, 300);
            GuiWindow saveAndLoadWindow = new GuiWindow("Open or save", 0, 410 + Gui.topBarHeight, 150, 150);
            saveAndLoadWindow.active = false;
            GuiTextBox sceneName = new GuiTextBox();
            sceneName.text = "scene";

            GuiWindow componentListWindow = new GuiWindow("Add component", 100, 10, 300, 210);
            componentListWindow.active = false;

            GuiWindow componentWindow = new GuiWindow("Component info", 100, 230, 300, 250);
            componentWindow.active = false;

            GuiWindow prefabWindow = new GuiWindow("Prefabs", 100, 300, 300, 400);
            prefabWindow.active = false;
            string prefabDir = String.Empty;
            bool defaultPrefabs = true;

            GuiWindow prefabSpawnWindow = new GuiWindow("Prefab properties", 150, 320);
            prefabSpawnWindow.active = false;
            string loadDir = String.Empty;

            GuiWindow loadSceneWindow = new GuiWindow("Load scene", 200, 320);
            loadSceneWindow.active = false;
            string sceneDir = String.Empty;
            Dictionary<string, ParsedObject> objs = new Dictionary<string, ParsedObject>();

            ae_Console console = new ae_Console();

            PerspectiveWindow.init();

            // Main game loop
            while (Window.tick()) // Detect window close button or ESC key
            {
                World.update(false);
                manager.update(mover);
                infoWindow.active = true;

                if (IsWindowResized()) {
                    Window.renderWidth = Window.width;
                    Window.renderHeight = Window.height;
                }

                if (IsKeyPressed(KeyboardKey.KEY_ESCAPE))
                    isMouseLocked = !isMouseLocked;

                if (IsKeyPressed(KeyboardKey.KEY_GRAVE)) {
                    console.window.setPosition(10, 10);
                    console.toggleActive();
                }

                if (IsKeyPressed(KeyboardKey.KEY_RIGHT_ALT)) {
                    TransformGizmo.CAMERA_MODE++;

                    if (TransformGizmo.CAMERA_MODE == CameraMode.XY) {
                        camera.target.Z = 0;
                        camera.front = Vector3.UnitZ;
                        camera.up = Vector3.UnitY;
                        camera.position = Vector3.Zero with { Z = 10 };
                        camera.rotation = Vector3.Zero;
                    }
                    else if (TransformGizmo.CAMERA_MODE == CameraMode.ZY) {
                        camera.target.X = 0;
                        camera.front = Vector3.UnitX;
                        camera.up = Vector3.UnitY;
                        camera.position = Vector3.Zero with { X = -10 };
                        camera.rotation = Vector3.Zero;
                    }
                    else if (TransformGizmo.CAMERA_MODE == CameraMode.XZ) {
                        camera.target.Y = 0;
                        camera.front = Vector3.UnitY;
                        camera.up = Vector3.UnitZ;
                        camera.position = Vector3.Zero with { Y = 10 };
                        camera.rotation = Vector3.Zero;
                    }

                    if ((int)TransformGizmo.CAMERA_MODE > 3) {
                        TransformGizmo.CAMERA_MODE = CameraMode.FPS;
                        camera.matrix.projection = (int)CameraProjection.CAMERA_PERSPECTIVE;
                    }
                }

                if (TransformGizmo.CAMERA_MODE is CameraMode.FPS) {
                    camera.setFirstPerson(0.1f, isMouseLocked);
                    camera.setDefaultFPSControls(10, isMouseLocked, true);
                    camera.defaultFpsMatrix();
                }
                else if (TransformGizmo.CAMERA_MODE is CameraMode.XY) {
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
                }
                else if (TransformGizmo.CAMERA_MODE is CameraMode.ZY) {
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
                }
                else if (TransformGizmo.CAMERA_MODE is CameraMode.XZ) {
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
                    TransformGizmo.CURRENT_MODE = Mode.ROAM;
                if (IsKeyPressed(KeyboardKey.KEY_F2))
                    TransformGizmo.CURRENT_MODE = Mode.MOVE;
                if (IsKeyPressed(KeyboardKey.KEY_F3))
                    TransformGizmo.CURRENT_MODE = Mode.SCALE;
                if (IsKeyPressed(KeyboardKey.KEY_F4))
                    TransformGizmo.CURRENT_MODE = Mode.ROTATE;

                if ((IsKeyDown(KeyboardKey.KEY_LEFT_CONTROL) || IsKeyDown(KeyboardKey.KEY_LEFT_SUPER)) &&
                    IsKeyPressed(KeyboardKey.KEY_L) && !isMouseLocked) {
                    manager.load("scene");
                }

                if ((IsKeyDown(KeyboardKey.KEY_LEFT_CONTROL) || IsKeyDown(KeyboardKey.KEY_LEFT_SUPER)) &&
                    IsKeyPressed(KeyboardKey.KEY_S) && !isMouseLocked)
                    manager.save("scene");

                if ((IsKeyDown(KeyboardKey.KEY_LEFT_CONTROL) || IsKeyDown(KeyboardKey.KEY_LEFT_SUPER)) &&
                    IsKeyPressed(KeyboardKey.KEY_O) && !isMouseLocked)
                    manager.outlined = !manager.outlined;

                mover.update(camera);

                if (TransformGizmo.IS_OBJ_ACTIVE) {
                    if (IsKeyDown(KeyboardKey.KEY_LEFT_CONTROL) && IsKeyDown(KeyboardKey.KEY_LEFT_ALT) &&
                        IsKeyPressed(KeyboardKey.KEY_D)) {
                        Entity newEnt = new Entity();
                        newEnt.transform.position = TransformGizmo.ACTIVE_ENT.transform.position with {
                            X = TransformGizmo.ACTIVE_ENT.transform.position.X + 1.5f,
                            Y = TransformGizmo.ACTIVE_ENT.transform.position.Y + 1.5f,
                            Z = TransformGizmo.ACTIVE_ENT.transform.position.Z + 1.5f
                        };
                        newEnt.transform.scale = TransformGizmo.ACTIVE_ENT.transform.scale;
                        newEnt.transform.rotation = TransformGizmo.ACTIVE_ENT.transform.rotation;

                        foreach (Component component in TransformGizmo.ACTIVE_ENT.components) {
                            Component copy = component.copy();
                            if (copy != null) newEnt.addComponent(copy);
                        }

                        TransformGizmo.ACTIVE_ENT = newEnt;
                    }
                }
                else {
                    EditorComponentData.activeComponent = null;
                }

                // 2d perspective render and update
                PerspectiveWindow.update();
                PerspectiveWindow.render();

                Window.beginRender();
                ClearBackground(BLACK);
                BeginMode3D(camera.matrix);

                Rendering.drawDebugAxies(100);
                Rendering.drawDebugAxies(-100);

                if (TransformGizmo.CAMERA_MODE is CameraMode.XY) {
                    for (int i = 0; i < 100; i++) {
                        for (int j = 0; j < 100; j++) {
                            DrawCubeWiresV(Vector3.Zero with { X = j - 50, Y = i - 50 }, Vector3.One with { Z = 0 },
                                Utils.semiWhite);
                        }
                    }
                }
                else if (TransformGizmo.CAMERA_MODE is CameraMode.ZY) {
                    for (int i = 0; i < 100; i++) {
                        for (int j = 0; j < 100; j++) {
                            DrawCubeWiresV(Vector3.Zero with { Z = j - 50, Y = i - 50 }, Vector3.One with { X = 0 },
                                Utils.semiWhite);
                        }
                    }
                }
                else if (TransformGizmo.CAMERA_MODE is CameraMode.XZ) {
                    for (int i = 0; i < 100; i++) {
                        for (int j = 0; j < 100; j++) {
                            DrawCubeWiresV(Vector3.Zero with { X = j - 50, Z = i - 50 }, Vector3.One with { Y = 0 },
                                Utils.semiWhite);
                        }
                    }
                }

                manager.render();
                World.render();

                if (TransformGizmo.IS_OBJ_ACTIVE)
                    mover.render();

                EndMode3D();

                infoWindow.render();
                saveAndLoadWindow.render();
                componentListWindow.render();
                componentWindow.render();
                prefabWindow.render();
                prefabSpawnWindow.render();
                loadSceneWindow.render();
                PerspectiveWindow.window.render();

                if (infoWindow.active) {
                    Gui.GuiTextPro(Gui.font,
                        "fps: " + GetFPS(),
                        new Vector2(10, 10),
                        20, WHITE, infoWindow);

                    Gui.GuiTextPro(Gui.font, "mode: " + TransformGizmo.CURRENT_MODE,
                        new Vector2(10, 30),
                        20, WHITE, infoWindow);

                    Gui.GuiTextPro(Gui.font, "entities: " + World.entities.Count, new Vector2(10, 50), 20, WHITE,
                        infoWindow);

                    if (Gui.GuiButton("Scene", 10, 80, 150, 30, infoWindow)) {
                        saveAndLoadWindow.active = true;
                    }

                    if (Gui.GuiButton("Debug Render", 10, 120, 150, 30, infoWindow)) {
                        World.renderColliders = !World.renderColliders;
                    }

                    World.debugRenderTerrain = Gui.GuiTickBox(World.debugRenderTerrain, 10, 160, 30, 30, infoWindow);
                    Gui.GuiTextPro(Gui.font, "debug terrain", 50, 160, 15, WHITE, infoWindow);

                    if (Gui.GuiButton("Add new", 10, 200, 150, 30, infoWindow)) {
                        if (TransformGizmo.CAMERA_MODE is CameraMode.FPS) {
                            Entity temp = new Entity();
                            temp.transform.position = camera.position;
                            temp.transform.scale = Vector3.One;
                        }
                        else {
                            Entity temp = new Entity();
                            temp.transform.position = camera.target;
                            temp.transform.scale = Vector3.One;
                        }
                    }

                    if (Gui.GuiButton("Prefabs", 10, 240, 150, 30, infoWindow)) {
                        prefabWindow.active = true;
                    }

                    if (Gui.GuiButton("Perspective", 10, 280, 150, 30, infoWindow)) {
                        PerspectiveWindow.window.active = true;
                    }
                }

                if (prefabWindow.active) {
                    if (Gui.GuiButton("Open directory", 10, 10, 280, 25, prefabWindow)) {
                        DialogResult result = Dialog.FolderPicker();
                        if (result.IsOk) {
                            prefabDir = result.Path.Replace("\\", "/");
                        }
                        else {
                            Console.WriteLine("cancelled");
                        }

                        defaultPrefabs = false;
                    }

                    if (Gui.GuiButton("Load Default", 10, 40, 280, 25, prefabWindow)) {
                        prefabDir = String.Empty;
                        defaultPrefabs = true;
                    }

                    if (defaultPrefabs) {
                        if (Gui.GuiButton("#", 10, 75, 25, 25, prefabWindow)) {
                            prefabSpawnWindow.active = true;
                        }
                    }

                    if (!defaultPrefabs && prefabDir != String.Empty) {
                        string[] files = Directory.GetFiles(prefabDir);

                        int columns = 9;

                        int rows = (int)Math.Ceiling((double)files.Length / columns);
                        int width = 25;
                        int height = 25;

                        for (int row = 0; row < rows; row++) {
                            for (int col = 0; col < columns; col++) {
                                int currentIndex = row * columns + col;
                                if (currentIndex < files.Length) {
                                    float x = 10 + col * (width + 10); // Adjust spacing between rectangles
                                    float y = 75 + row * (height + 10); // Adjust spacing between rectangles
                                    files[currentIndex] = files[currentIndex].Replace("\\", "/");
                                    if (files[currentIndex].EndsWith(".od")) {
                                        if (Gui.GuiButton("#", x, y, width, height, prefabWindow)) {
                                            ParsedData data = Parser.parse(Parser.read(files[currentIndex]));

                                            foreach (string name in data.data.Keys) {
                                                prefabSpawnWindow.active = true;
                                                loadDir = files[currentIndex];
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                if (prefabSpawnWindow.active && !defaultPrefabs) {
                    ParsedData data = Parser.parse(Parser.read(loadDir));

                    for (var i = 0; i < data.data.Keys.Count; i++) {
                        Gui.GuiTextPro(Gui.font,
                            data.data.Keys.ElementAt(i) + " - " +
                            data.getObject(data.data.Keys.ElementAt(i)).modifier, 10, 10 + i * 30, 15, WHITE,
                            prefabSpawnWindow);

                        if (Gui.GuiButton("x", 10 + MeasureTextEx(Gui.font, data.data.Keys.ElementAt(i) +
                                                                            " - " +
                                                                            data.getObject(data.data.Keys.ElementAt(i))
                                                                                .modifier, 15, 2.5f).X, 10 + i * 30, 15,
                                15,
                                prefabSpawnWindow)) {
                            Prefab.loadPrefab(loadDir, data.data.Keys.ElementAt(i), false, false);
                        }

                        prefabSpawnWindow.rec.height = i * 30 + 50;
                    }

                    if (Gui.GuiButton("Go", prefabSpawnWindow.rec.width - 50, 10, 40, 40, prefabSpawnWindow)) {
                        foreach (string name in data.data.Keys) {
                            Prefab.loadPrefab(loadDir, name, false, false);
                        }
                    }
                }

                if (prefabSpawnWindow.active && defaultPrefabs) {
                    ParsedData data = Parser.parse(Prefabs.PREFABS_CONTENT);

                    for (var i = 0; i < data.data.Keys.Count; i++) {
                        Gui.GuiTextPro(Gui.font,
                            data.data.Keys.ElementAt(i) + " - " +
                            data.getObject(data.data.Keys.ElementAt(i)).modifier, 10, 10 + i * 30, 15, WHITE,
                            prefabSpawnWindow);

                        if (Gui.GuiButton("x", 10 + MeasureTextEx(Gui.font, data.data.Keys.ElementAt(i) +
                                                                            " - " +
                                                                            data.getObject(data.data.Keys.ElementAt(i))
                                                                                .modifier, 15, 2.5f).X, 10 + i * 30, 15,
                                15,
                                prefabSpawnWindow)) {
                            Prefab.loadPrefab(Prefabs.PREFABS_CONTENT, data.data.Keys.ElementAt(i), false).transform
                                .position = camera.position;
                        }

                        prefabSpawnWindow.rec.height = i * 30 + 50;
                    }

                    if (Gui.GuiButton("Go", prefabSpawnWindow.rec.width - 50, 10, 40, 40, prefabSpawnWindow)) {
                        foreach (string name in data.data.Keys) {
                            Prefab.loadPrefab(loadDir, name, false, false);
                        }
                    }
                }

                if (saveAndLoadWindow.active) {
                    sceneName.render(10, 10, 120, 25, saveAndLoadWindow);

                    if (Gui.GuiButton("Save", 10, 45, 60, 25, saveAndLoadWindow)) {
                        manager.save(sceneName.text);
                    }

                    if (Gui.GuiButton("Load", 10, 80, 60, 25, saveAndLoadWindow)) {
                        loadSceneWindow.active = true;
                        // manager.load(sceneName.text);
                    }

                    if (Gui.GuiButton("Clean", 10, 115, 60, 25, saveAndLoadWindow)) {
                        World.entities.Clear();
                        TransformGizmo.ACTIVE_ENT = null;
                        TransformGizmo.IS_OBJ_ACTIVE = false;
                    }
                }

                if (loadSceneWindow.active) {
                    if (Gui.GuiButton("Find", 10, 10, loadSceneWindow.rec.width - 20, 30, loadSceneWindow)) {
                        DialogResult result = Dialog.FileOpen();
                        if (result.IsOk) {
                            if (result.Path != null) sceneDir = result.Path.Replace("\\", "/");
                        }
                        else {
                            Console.WriteLine("cancelled");
                        }
                    }

                    if (sceneDir != String.Empty) {
                        ParsedData data = Parser.parse(Parser.read(sceneDir));

                        for (var i = 0; i < data.data.Keys.Count; i++) {
                            string name = data.data.Keys.ElementAt(i);
                            ParsedObject obj = data.getObject(name);

                            if (obj.modifier == "scene" && !objs.ContainsKey(name)) {
                                objs.Add(name, obj);
                            }
                        }

                        for (var i = 0; i < objs.Count; i++) {
                            KeyValuePair<string, ParsedObject> pair = objs.ElementAt(i);

                            Gui.GuiTextPro(Gui.font,
                                pair.Key + " - " +
                                pair.Value.modifier, 10, 60 + i * 30, 15, WHITE,
                                loadSceneWindow);

                            if (Gui.GuiButton("x", 10 + MeasureTextEx(Gui.font, pair.Key + " - " +
                                        pair.Value.modifier, 15, 2.5f).X, 60 + i * 30, 15, 15,
                                    loadSceneWindow)) {
                                Prefab.loadScene(sceneDir, pair.Key, true, true);
                                // manager.load(pair.Key);
                            }

                            loadSceneWindow.rec.height = i * 30 + 100;
                        }
                    }
                }

                if (PerspectiveWindow.window.active) {
                    PerspectiveWindow.perspective.finalRender(10, 10, false, PerspectiveWindow.window);

                    if (PerspectiveWindow.window.finishedResizing()) {
                        PerspectiveWindow.perspective.setWidth((int)(PerspectiveWindow.window.rec.width - 20));
                        PerspectiveWindow.perspective.setHeight((int)(PerspectiveWindow.window.rec.height - 20));
                        PerspectiveWindow.perspective.reloadTarget();
                        PerspectiveWindow.reload();
                    }
                }

                if (componentListWindow.active) {
                    if (Gui.GuiButton("MeshComponent", 10, 10, 250, 30, componentListWindow, TextPositioning.LEFT)) {
                        if (!TransformGizmo.ACTIVE_ENT.hasComponent<MeshComponent>()) {
                            TransformGizmo.ACTIVE_ENT.addComponent(new MeshComponent(TransformGizmo.ACTIVE_ENT, ShapeType.BOX,
                                WHITE));
                        }
                        else {
                            Console.WriteLine("Already has a mesh component");
                        }
                    }

                    if (Gui.GuiButton("RigidBodyComponent", 10, 50, 250, 30, componentListWindow,
                            TextPositioning.LEFT)) {
                        if (!TransformGizmo.ACTIVE_ENT.hasComponent<RigidBodyComponent>()) {
                            TransformGizmo.ACTIVE_ENT.addComponent(new RigidBodyComponent(TransformGizmo.ACTIVE_ENT));
                        }
                        else {
                            Console.WriteLine("Already has a rigidbody component");
                        }
                    }

                    if (Gui.GuiButton("LightComponent", 10, 90, 250, 30, componentListWindow, TextPositioning.LEFT)) {
                    }

                    if (Gui.GuiButton("FluidComponent", 10, 130, 250, 30, componentListWindow, TextPositioning.LEFT)) {
                    }

                    if (Gui.GuiButton("SpAudioComponent", 10, 170, 250, 30, componentListWindow,
                            TextPositioning.LEFT)) {
                    }
                }

                if (componentWindow.active) {
                    EditorComponentData.render(componentWindow);
                }

                if (TransformGizmo.IS_OBJ_ACTIVE) {
                    if (!entityDataWindow.active) {
                        entityDataWindow.active = true;
                    }

                    entityDataWindow.render();

                    if (entityDataWindow.active) {
                        Gui.GuiTextPro(Gui.font, "tag: " + TransformGizmo.ACTIVE_ENT.tag,
                            new Vector2(10, 10),
                            20, WHITE, entityDataWindow);

                        xPos.render(10, 40, 60, 20, entityDataWindow);
                        yPos.render(85, 40, 60, 20, entityDataWindow);
                        zPos.render(160, 40, 60, 20, entityDataWindow);

                        xScale.render(10, 65, 60, 20, entityDataWindow);
                        yScale.render(85, 65, 60, 20, entityDataWindow);
                        zScale.render(160, 65, 60, 20, entityDataWindow);

                        xRot.render(10, 90, 60, 20, entityDataWindow);
                        yRot.render(85, 90, 60, 20, entityDataWindow);
                        zRot.render(160, 90, 60, 20, entityDataWindow);

                        if (TransformGizmo.CURRENT_MODE != Mode.MOVE) {
                            if (float.TryParse(xPos.text, out float x))
                                TransformGizmo.ACTIVE_ENT.transform.position.X = x;
                            if (float.TryParse(yPos.text, out float y))
                                TransformGizmo.ACTIVE_ENT.transform.position.Y = y;
                            if (float.TryParse(zPos.text, out float z))
                                TransformGizmo.ACTIVE_ENT.transform.position.Z = z;
                        }

                        if (TransformGizmo.CURRENT_MODE != Mode.SCALE) {
                            if (float.TryParse(xScale.text, out float w))
                                TransformGizmo.ACTIVE_ENT.transform.scale.X = w;
                            if (float.TryParse(yScale.text, out float h))
                                TransformGizmo.ACTIVE_ENT.transform.scale.Y = h;
                            if (float.TryParse(zScale.text, out float d))
                                TransformGizmo.ACTIVE_ENT.transform.scale.Z = d;
                        }

                        if (TransformGizmo.CURRENT_MODE != Mode.ROTATE) {
                            if (float.TryParse(xRot.text, out float rx))
                                TransformGizmo.ACTIVE_ENT.transform.rotation.X = rx;
                            if (float.TryParse(yRot.text, out float ry))
                                TransformGizmo.ACTIVE_ENT.transform.rotation.Y = ry;
                            if (float.TryParse(zRot.text, out float rz))
                                TransformGizmo.ACTIVE_ENT.transform.rotation.Z = rz;
                        }

                        string components = String.Empty;
                        foreach (Component component in TransformGizmo.ACTIVE_ENT.components) {
                            components += component.fileName() + ", ";
                        }

                        Gui.GuiTextPro(Gui.font, "components:\n" + components,
                            new Vector2(
                                235,
                                10),
                            20, WHITE, entityDataWindow);

                        for (var i = 0; i < TransformGizmo.ACTIVE_ENT.components.Count; i++) {
                            if (Gui.GuiButton(TransformGizmo.ACTIVE_ENT.components[i].fileName(), 10, 120 + i * 35, 250, 30,
                                    entityDataWindow, TextPositioning.LEFT)) {
                                componentWindow.active = true;
                                componentWindow.title = TransformGizmo.ACTIVE_ENT.components[i].fileName();
                                EditorComponentData.setComponent(TransformGizmo.ACTIVE_ENT.components[i]);
                            }

                            if (Gui.GuiInteractiveRec(new ExitIcon(), 270, 120 + i * 35, 30, 30,
                                    IsMouseButtonPressed(MouseButton.MOUSE_BUTTON_LEFT), entityDataWindow)) {
                                TransformGizmo.ACTIVE_ENT.components.RemoveAt(i);
                            }
                        }

                        if (Gui.GuiButton("Add component", 10, 145 + TransformGizmo.ACTIVE_ENT.components.Count * 35, 175,
                                30,
                                entityDataWindow)) {
                            componentListWindow.active = true;
                        }
                    }
                }

                console.render();

                Window.endRender();
            }

            CloseWindow();
        }
    }
}
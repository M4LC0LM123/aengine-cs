using Raylib_CsLo;
using static Raylib_CsLo.Raylib;
using System.Numerics;
using aengine.graphics;
using aengine.ecs;
using Sandbox.aengine;
using Sandbox.aengine.Gui;
using World = aengine.ecs.World;

namespace Sandbox
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            // use only in ides like visual studio and rider,
            // the final build should have the assets folder in the same directory as the exe so remove this line below then
            Directory.SetCurrentDirectory("../../../");
            
            SetConfigFlags(ConfigFlags.FLAG_WINDOW_RESIZABLE);
            InitWindow(1280, 720, "Hi Hi Hi Ha");
            SetWindowIcon(LoadImage("assets/logo.png"));
            SetTargetFPS(60);
            SetExitKey(KeyboardKey.KEY_NULL);

            Texture albedo = LoadTexture("assets/albedo.png");
            Gui.font = LoadFont("assets/fonts/font.ttf");
            
            Camera camera = new Camera(new Vector3(0, 1, 0), 90);
            bool isMouseLocked = false;

            Entity body = new Entity();
            body.transform.position.Y = 15;
            body.transform.scale = Vector3.One;
            body.addComponent(new MeshComponent(body, GREEN, albedo));
            body.addComponent(new RigidBodyComponent(body));
            
            Entity body2 = new Entity();
            body2.transform.position.Y = 15;
            body2.transform.position.X = 2.5f;
            body2.transform.scale = Vector3.One;
            body2.addComponent(new MeshComponent(body2, GenMeshSphere(1f, 15, 15), YELLOW, albedo));
            body2.addComponent(new RigidBodyComponent(body2, 1, BodyType.DYNAMIC, ShapeType.SPHERE));

            GuiWindow window = new GuiWindow("SUIIIIIIIII", 10, 10, 300, 400);
            GuiTextBox textBox = new GuiTextBox();
            GuiSlider slider = new GuiSlider();

            Scene scene = new Scene("assets/maps/map2.json");

            foreach (var obj in scene.data)
            {
                switch (obj.id)
                {
                    case 0:
                        Entity cube = new Entity();
                        cube.transform.position = new Vector3(obj.x, obj.y, obj.z);
                        cube.transform.scale = new Vector3(obj.w, obj.h, obj.d);
                        cube.transform.rotation = new Vector3(obj.rx, obj.ry, obj.rz);
                        cube.addComponent(new MeshComponent(cube, Rendering.getRandomColor(), albedo));
                        cube.addComponent(new RigidBodyComponent(cube));
                        break;
                    case 1:
                        Entity sphere = new Entity();
                        sphere.transform.position = new Vector3(obj.x, obj.y, obj.z);
                        sphere.transform.scale = new Vector3(obj.w, obj.h, obj.d);
                        sphere.transform.rotation = new Vector3(obj.rx, obj.ry, obj.rz);
                        sphere.addComponent(new MeshComponent(sphere, GenMeshSphere(sphere.transform.scale.X, 15, 15), Rendering.getRandomColor(), albedo));
                        sphere.addComponent(new RigidBodyComponent(sphere, 1, BodyType.DYNAMIC, ShapeType.SPHERE));
                        break;
                    case 2:
                        Entity wall = new Entity();
                        wall.transform.position = new Vector3(obj.x, obj.y, obj.z);
                        wall.transform.scale = new Vector3(obj.w, obj.h, obj.d);
                        wall.transform.rotation = new Vector3(obj.rx, obj.ry, obj.rz);
                        wall.addComponent(new MeshComponent(wall, Rendering.getRandomColor(), albedo));
                        wall.addComponent(new RigidBodyComponent(wall, 1.0f, BodyType.STATIC));
                        break;
                }
            }

            // Main game loop
            while (!WindowShouldClose()) // Detect window close button or ESC key
            {
                World.update();
                
                if (IsKeyPressed(KeyboardKey.KEY_ESCAPE))
                    isMouseLocked = !isMouseLocked;

                if (IsKeyPressed(KeyboardKey.KEY_R))
                {
                    body2.getComponent<RigidBodyComponent>().applyImpulse(0, 10, 0);
                    body.getComponent<RigidBodyComponent>().applyImpulse(0, 10, 0);
                }

                camera.setFirstPerson(0.1f, isMouseLocked);
                camera.setDefaultFPSControls(10, isMouseLocked, true);
                camera.defaultFpsMatrix();
                camera.update();

                BeginDrawing();
                ClearBackground(SKYBLUE);

                BeginMode3D(camera.matrix);
                World.render();

                Rendering.drawDebugAxies();
                Rendering.drawArrow(Vector3.Zero, Vector3.One, GREEN);
                
                EndMode3D();
                window.render();

                Gui.GuiTextPro(Gui.font, "FPS: " + GetFPS(), new Vector2(10, 10), Gui.font.baseSize, WHITE, window);
                Gui.GuiTextPro(Gui.font, "Entities: " + World.entities.Count, new Vector2(10, 50), Gui.font.baseSize, WHITE, window);
                
                if (Gui.GuiButton("Render Colliders", 10, 100, 240, 40, window, Positioning.LEFT))
                {
                    World.RenderColliders = !World.RenderColliders;
                }
                
                textBox.render(10, 150, 240, 40, window);
                slider.render(10, 200, 240, 40, window);

                EndDrawing();
            }
            World.dispose();
            CloseWindow();
        }
    }
}
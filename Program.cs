using System;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Graphics;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common; 
using OpenTK.Windowing.Desktop;
using StbImageSharp;
using aengine;
using static aengine.core.aengine;
using static aengine.graphics.Graphics;
using static aengine.graphics.Rendering;
using aengine.ecs;
using aengine.graphics;
using aengine.input;
using SharpFNT;
using aengine.loader;
using System.Diagnostics;
using aengine.gui;

namespace Sandbox
{
    class Program
    {
        static void Main()
        {
            // doesnt work in dotnet cli and vscode so comment it then
            // Directory.SetCurrentDirectory("../../../");

            init(1024, 720, "aengine");
            setIcon(new Texture("assets/logo.png"));

            Camera camera = new Camera(new System.Numerics.Vector3(2, 2, 2), 90);
            bool isMouseLocked = false;

            float rotationX = 0;
            float rotationZ = 0;

            Texture albedo = new Texture("assets/albedo.png");
            Texture normal = new Texture("assets/orm.png");
            Texture caco = new Texture("assets/cacodemon.png");
            Model model = new Model("assets/models/einstein.obj", "assets/models/einstein.mtl", albedo);

            Gui.btn_released = new Texture("assets/default-gui/btn.png");
            Gui.btn_pressed = new Texture("assets/default-gui/btn_pressed.png");
            Gui.font = new Font("assets/fonts/arial.fnt");
            Gui.exit = new Texture("assets/default-gui/exit.png");
            Gui.scale = new Texture("assets/default-gui/scale.png");

            Texture[] skyboxT = new Texture[]
            {
                new Texture("assets/skybox/front.png"),
                new Texture("assets/skybox/back.png"),
                new Texture("assets/skybox/left.png"),
                new Texture("assets/skybox/right.png"),
                new Texture("assets/skybox/top.png"),
                new Texture("assets/skybox/bottom.png"),
            };

            float speed = 10;

            Color rclr = new Color(getRandomFloat(-1, 2), getRandomFloat(-1, 2), getRandomFloat(-1, 2), 0.5f);

            Entity ground = new Entity();
            ground.transform.position = new System.Numerics.Vector3(0, -9, 0);
            ground.transform.scale = new System.Numerics.Vector3(30, 2, 30);
            MeshComponent mc = new MeshComponent(ground);
            mc.texture = albedo;
            ground.addComponent(mc);
            RigidBodyComponent rb = new RigidBodyComponent(ground, 1.0f, BodyType.STATIC, ShapeType.BOX);
            ground.addComponent(rb);

            Entity body = new Entity();
            body.transform.position = new System.Numerics.Vector3(0, 12, 0);
            body.transform.scale = new System.Numerics.Vector3(2, 2, 2);
            MeshComponent mc2 = new MeshComponent(body);
            mc2.texture = albedo;
            mc2.color = Colors.TEAL;
            mc2.shape = ShapeType.BOX;
            body.addComponent(mc2);
            RigidBodyComponent rb2 = new RigidBodyComponent(body, 1.0f, BodyType.DYNAMIC, mc2.shape);
            body.addComponent(new RigidBodyComponent(body, 1.0f, BodyType.DYNAMIC, mc2.shape));

            GuiWindow window = new GuiWindow(10, 10, 400, 400);

            TextField textField = new TextField();

            // Main loop
            while (!WindowShouldClose())
            {
                update();
                World.update();

                if (Input.IsKeyPressed(Keys.Escape))
                {
                    isMouseLocked = !isMouseLocked;
                }

                if (Input.IsKeyDown(Keys.LeftShift))
                {
                    speed = 50;
                }
                else 
                {
                    speed = 10;
                }

                if (Input.IsKeyDown(Keys.Left))
                {
                    rb2.applyImpulse(new System.Numerics.Vector3(1, 0, 0));
                }
                if (Input.IsKeyDown(Keys.Right))
                {
                    rb2.applyImpulse(new System.Numerics.Vector3(-1, 0, 0));
                }

                rclr.r = sineWave();

                camera.setFirstPerson(0.1f, isMouseLocked);
                camera.setDefaultFPSControls(speed, isMouseLocked, true);
                camera.defaultFpsMatrix();
                camera.update();

                rotationX += 100 * getDeltaTime();
                rotationZ += 100 * getDeltaTime();

                window.update();

                begin();
                setProjectionMatrix(camera);
                clearBackground(Colors.TEAL);
                drawSkyBox(skyboxT, Colors.WHITE);

                World.render();

                drawDebugAxies();

                drawTexturedCube(albedo, new System.Numerics.Vector3(4, 2, 2), new System.Numerics.Vector3(1, 2, 3), new System.Numerics.Vector3(rotationX, 0, rotationZ), Colors.LIME);

                drawTexturedSphere(albedo, new System.Numerics.Vector3(-4, 6, 8), new System.Numerics.Vector3(1, 2, 3), new System.Numerics.Vector3(rotationX, 0, rotationZ), Colors.MAGENTA);

                drawTexturedCylinder(albedo, new System.Numerics.Vector3(4, 2, 8), new System.Numerics.Vector3(1, 1, 2), new System.Numerics.Vector3(0, 0, 0), Colors.ORANGE);

                drawTexturedCapsule(albedo, new System.Numerics.Vector3(9, 2, 8), new System.Numerics.Vector3(1, 3, 1), new System.Numerics.Vector3(0, 0, 0), Colors.TEAL);

                model.render(new System.Numerics.Vector3(8, 4, 6), new System.Numerics.Vector3(1, 1, 1), new System.Numerics.Vector3(0, 180, 0), Colors.GREEN);

                drawSprite3D(caco, new System.Numerics.Vector3(-5, 2, -3), 3, 3, rotationX, Colors.WHITE);

                window.render();

                if (Gui.Button("reset physics", 10, 10, 325, 75, Colors.WHITE, window))
                {
                    rb2.body.Position = new Jitter.LinearMath.JVector(0, 12, 0);
                    rb2.body.Update();
                    rb2.body.IsActive = true;
                }

                textField.render(10, 100, 325, 75, Colors.WHITE, window);

                drawText(Gui.font, "FPS: " + getFPS().ToString(), 10, 60, 0.25f, rclr, window);
                drawText(Gui.font, "mouse: " + getMousePos().ToString(), 10, 80, 0.25f, rclr, window);

                end();
            }

            // Clean up GLFW resources
            dispose();
        }

    }
}

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

namespace Sandbox
{
    class Program
    {
        static void Main()
        {
            init(800, 600, "aengine");
            setIcon(new Texture("assets/logo.png"));

            Camera camera = new Camera(new System.Numerics.Vector3(2, 2, 2), 90);
            bool isMouseLocked = false;

            float rotationX = 0;
            float rotationZ = 0;

            Texture albedo = new Texture("assets/albedo.png");
            Texture normal = new Texture("assets/orm.png");
            Texture skybox = new Texture("assets/skybox.jpeg");
            Texture caco = new Texture("assets/cacodemon.png");
            Model model = new Model("assets/models/einstein.obj", "assets/models/einstein.mtl", null);

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

            Font font = new Font("assets/fonts/arial.fnt");

            Color rclr = new Color(getRandomFloat(-1, 2), getRandomFloat(-1, 2), getRandomFloat(-1, 2), 1f);

            // Main loop
            while (!WindowShouldClose())
            {
                update();

                if (Input.IsKeyDown(Keys.Escape))
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

                rclr.r = sineWave();

                camera.setFirstPerson(0.1f, isMouseLocked);
                camera.setDefaultFPSControls(speed, isMouseLocked, true);
                camera.defaultFpsMatrix();
                camera.update();

                rotationX += 100 * getDeltaTime();
                rotationZ += 100 * getDeltaTime();

                begin();
                setProjectionMatrix(camera);
                clearBackground(Colors.TEAL);
                drawSkyBox(skyboxT, Colors.WHITE);

                drawDebugAxies();

                drawTexturedCube(albedo, new System.Numerics.Vector3(4, 2, 2), new System.Numerics.Vector3(1, 2, 3), new System.Numerics.Vector3(rotationX, 0, rotationZ), Colors.LIME);

                drawTexturedSphere(albedo, new System.Numerics.Vector3(-4, 6, 8), new System.Numerics.Vector3(1, 2, 3), new System.Numerics.Vector3(rotationX, 0, rotationZ), Colors.MAGENTA);

                model.render(new System.Numerics.Vector3(8, 4, 6), new System.Numerics.Vector3(1, 1, 1), new System.Numerics.Vector3(0, 180, 0), Colors.GREEN);

                drawSprite3D(caco, new System.Numerics.Vector3(-5, 2, -3), 3, 3, rotationX, Colors.WHITE);

                drawText(font, "FPS: " + getFPS().ToString(), 10, getScreenSize().Y - 20, 0.25f, rclr);
                drawText(font, "mouse: " + getMousePos().ToString(), 10, getScreenSize().Y - 40, 0.25f, rclr);

                end();
            }

            // Clean up GLFW resources
            dispose();
        }

    }
}

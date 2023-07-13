using System;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Graphics.OpenGL;
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

namespace Sandbox
{
    class Program
    {
        static void Main()
        {
            init(800, 600, "HEHE GLFW GO BRRR");

            Camera camera = new Camera(new System.Numerics.Vector3(2, 2, 2), 90);
            bool isMouseLocked = false;

            float rotationX = 0;
            float rotationZ = 0;

            Texture albedo = new Texture("assets/albedo.png");
            Model model = new Model("assets/models/einstein.obj", "assets/models/einstein.mtl");

            // Main loop
            while (!WindowShouldClose())
            {
                update();
                setTitle("HEHE GLFW GO BRRR: " + getFPS());

                if (Input.IsKeyDown(Keys.Escape))
                {
                    isMouseLocked = !isMouseLocked;
                }

                camera.setFirstPerson(0.1f, isMouseLocked);
                camera.setDefaultFPSControls(10, isMouseLocked, true);
                camera.defaultFpsMatrix();
                camera.update();

                rotationX += 100 * getDeltaTime();
                rotationZ += 100 * getDeltaTime();

                begin();
                setProjectionMatrix(camera);
                clearBackground(Colors.TEAL);

                Rendering.drawDebugAxies();

                drawTexturedCube(albedo, new System.Numerics.Vector3(2, 1, 1), new System.Numerics.Vector3(1, 2, 3), new System.Numerics.Vector3(rotationX, 0, rotationZ), Colors.LIME);

                drawTexturedSphere(albedo, new System.Numerics.Vector3(-2, 3, 4), new System.Numerics.Vector3(1, 2, 3), new System.Numerics.Vector3(rotationX, 0, rotationZ), Colors.MAGENTA);

                model.render();

                end();
            }

            // Clean up GLFW resources
            dispose();
        }

    }
}

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
using aengine.ecs;

namespace Sandbox
{
    class Program
    {

        static void drawSphere(double r, int lats, int longs, int textureId)
        {
            GL.Enable(EnableCap.Texture2D);
            GL.BindTexture(TextureTarget.Texture2D, textureId); 
            int i, j;
            for (i = 0; i <= lats; i++)
            {
                double lat0 = Math.PI * (-0.5 + (double)(i - 1) / lats);
                double z0 = Math.Sin(lat0);
                double zr0 = Math.Cos(lat0);

                double lat1 = Math.PI * (-0.5 + (double)i / lats);
                double z1 = Math.Sin(lat1);
                double zr1 = Math.Cos(lat1);

                GL.Begin(PrimitiveType.QuadStrip);
                for (j = 0; j <= longs; j++)
                {
                    double lng = 2 * Math.PI * (double)(j - 1) / longs;
                    double x = Math.Cos(lng);
                    double y = Math.Sin(lng);

                    // Calculate texture coordinates
                    double s = (double)j / longs;
                    double t0 = (double)(i - 1) / lats;
                    double t1 = (double)i / lats;

                    GL.Color4(Color4.White);
                    GL.TexCoord2(s, t0); GL.Normal3(x * zr0, y * zr0, z0);
                    GL.Vertex3(r * x * zr0, r * y * zr0, r * z0);

                    GL.TexCoord2(s, t1);
                    GL.Normal3(x * zr1, y * zr1, z1);
                    GL.Vertex3(r * x * zr1, r * y * zr1, r * z1);
                }
                GL.End();
            }
            GL.Disable(EnableCap.Texture2D);
            GL.BindTexture(TextureTarget.Texture2D, 0);
        }

        static unsafe void Main()
        {
            // Initialize GLFW
            if (!GLFW.Init())
            {
                Console.WriteLine("Failed to initialize GLFW");
                return;
            }

            int SCREEN_WIDTH = 800;
            int SCREEN_HEIGHT = 600;

            // Create a window
            var window = GLFW.CreateWindow(SCREEN_WIDTH, SCREEN_HEIGHT, "GLFW HEHEH", (OpenTK.Windowing.GraphicsLibraryFramework.Monitor*)IntPtr.Zero, (Window*)IntPtr.Zero);
            if (window == (Window*)IntPtr.Zero)
            {
                Console.WriteLine("Failed to create GLFW window");
                GLFW.Terminate();
                return;
            }

            double prevTime = GLFW.GetTime();
            double prevFPStime = GLFW.GetTime();
            int frameCount = 0;

            // Set the window's context as the current context
            GLFW.MakeContextCurrent(window);

            GL.LoadBindings(new GLFWBindingsContext());

            GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);

            GL.Enable(EnableCap.DepthTest);

            GL.Viewport(0, 0, SCREEN_WIDTH, SCREEN_HEIGHT);

            Matrix4 projectionMatrix;
            Matrix4 viewMatrix;
            Matrix4 modelMatrix;

            float rotationSpeed = 0;

            float aspectRatio = SCREEN_WIDTH / (float)SCREEN_HEIGHT;
            float fieldOfView = MathHelper.DegreesToRadians(45.0f);
            float nearPlane = 0.1f;
            float farPlane = 100.0f;
            projectionMatrix = Matrix4.CreatePerspectiveFieldOfView(fieldOfView, aspectRatio, nearPlane, farPlane);

            // Set up the view matrix
            Vector3 cameraPosition = new Vector3(2.0f, 2.0f, 2.0f);
            Vector3 cameraTarget = Vector3.Zero;
            Vector3 cameraUp = Vector3.UnitY;
            viewMatrix = Matrix4.LookAt(cameraPosition, cameraTarget, cameraUp);

            // Initialize the model matrix
            modelMatrix = Matrix4.Identity;

            int textureId;
            using (var imageStream = File.OpenRead("assets/albedo.png")) // Replace with the actual path to your texture image
            {
                var imageResult = ImageResult.FromStream(imageStream, ColorComponents.RedGreenBlueAlpha);
                if (imageResult == null)
                {
                    Console.WriteLine("FAILED TO LOAD IMAGE");
                    return;
                }
                textureId = GL.GenTexture();
                GL.BindTexture(TextureTarget.Texture2D, textureId);
                GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, imageResult.Width, imageResult.Height, 0, OpenTK.Graphics.OpenGL.PixelFormat.Rgba, PixelType.UnsignedByte, imageResult.Data);
                GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
            }

            // Main loop
            while (!GLFW.WindowShouldClose(window))
            {
                double currTime = GLFW.GetTime();
                float deltaTime = (float)(currTime - prevTime);
                prevTime = currTime;

                double currFPStime = GLFW.GetTime();
                frameCount++;

                if (currFPStime - prevFPStime >= 1.0f)
                {
                    GLFW.SetWindowTitle(window, "GLFW HEHEH, fps: " + frameCount);
                    frameCount = 0;
                    prevFPStime = currFPStime;
                }

                if (GLFW.GetKey(window, Keys.Right) == InputAction.Press)
                {
                    rotationSpeed += 100 * deltaTime;
                }
                if (GLFW.GetKey(window, Keys.Left) == InputAction.Press)
                {
                    rotationSpeed -= 100 * deltaTime;
                }

                if (GLFW.GetKey(window, Keys.Left) != InputAction.Press && GLFW.GetKey(window, Keys.Right) != InputAction.Press)
                {
                    if (rotationSpeed > 1)
                        rotationSpeed -= 100 * deltaTime;
                    if (rotationSpeed < -1)
                        rotationSpeed += 100 * deltaTime;
                }

                // Poll for events
                GLFW.PollEvents();

                // Update cube rotation
                float rotationAngle = rotationSpeed * deltaTime;
                modelMatrix = Matrix4.CreateFromAxisAngle(Vector3.UnitY, MathHelper.DegreesToRadians(rotationAngle)) * modelMatrix;

                // Render your graphics here...
                // Clear the color and depth buffers
                GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
                GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);

                // Set the projection matrix
                GL.MatrixMode(MatrixMode.Projection);
                GL.LoadMatrix(ref projectionMatrix);

                // Set the view matrix
                GL.MatrixMode(MatrixMode.Modelview);
                GL.LoadMatrix(ref viewMatrix);

                // set texture

                GL.Enable(EnableCap.Texture2D);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.LinearMipmapLinear);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
                GL.BindTexture(TextureTarget.Texture2D, textureId);

                // Set the model matrix
                GL.MultMatrix(ref modelMatrix);

                // Render the cube
                GL.Begin(PrimitiveType.Quads);

                // Front face
                GL.Color3((System.Drawing.Color)Color4.Red);
                GL.TexCoord2(0, 0); GL.Vertex3(-0.5f, -0.5f, 0.5f);
                GL.TexCoord2(1, 0); GL.Vertex3(0.5f, -0.5f, 0.5f);
                GL.TexCoord2(1, 1); GL.Vertex3(0.5f, 0.5f, 0.5f);
                GL.TexCoord2(0, 1); GL.Vertex3(-0.5f, 0.5f, 0.5f);

                // Back face
                GL.Color3((System.Drawing.Color)Color4.Green);
                GL.TexCoord2(0, 0); GL.Vertex3(-0.5f, -0.5f, -0.5f);
                GL.TexCoord2(1, 0); GL.Vertex3(0.5f, -0.5f, -0.5f);
                GL.TexCoord2(1, 1); GL.Vertex3(0.5f, 0.5f, -0.5f);
                GL.TexCoord2(0, 1); GL.Vertex3(-0.5f, 0.5f, -0.5f);

                // Left face
                GL.Color3((System.Drawing.Color)Color4.Blue);
                GL.TexCoord2(0, 0); GL.Vertex3(-0.5f, -0.5f, -0.5f);
                GL.TexCoord2(1, 0); GL.Vertex3(-0.5f, 0.5f, -0.5f);
                GL.TexCoord2(1, 1); GL.Vertex3(-0.5f, 0.5f, 0.5f);
                GL.TexCoord2(0, 1); GL.Vertex3(-0.5f, -0.5f, 0.5f);

                // Right face
                GL.Color3((System.Drawing.Color)Color4.Yellow);
                GL.TexCoord2(0, 0); GL.Vertex3(0.5f, -0.5f, -0.5f);
                GL.TexCoord2(1, 0); GL.Vertex3(0.5f, 0.5f, -0.5f);
                GL.TexCoord2(1, 1); GL.Vertex3(0.5f, 0.5f, 0.5f);
                GL.TexCoord2(0, 1); GL.Vertex3(0.5f, -0.5f, 0.5f);

                // Top face
                GL.Color3((System.Drawing.Color)Color4.Cyan);
                GL.TexCoord2(0, 0); GL.Vertex3(-0.5f, 0.5f, -0.5f);
                GL.TexCoord2(1, 0); GL.Vertex3(0.5f, 0.5f, -0.5f);
                GL.TexCoord2(1, 1); GL.Vertex3(0.5f, 0.5f, 0.5f);
                GL.TexCoord2(0, 1); GL.Vertex3(-0.5f, 0.5f, 0.5f);

                // Bottom face
                GL.Color3((System.Drawing.Color)Color4.Magenta);
                GL.TexCoord2(0, 0); GL.Vertex3(-0.5f, -0.5f, -0.5f);
                GL.TexCoord2(1, 0); GL.Vertex3(0.5f, -0.5f, -0.5f);
                GL.TexCoord2(1, 1); GL.Vertex3(0.5f, -0.5f, 0.5f);
                GL.TexCoord2(0, 1); GL.Vertex3(-0.5f, -0.5f, 0.5f);

                GL.End();
                // GL.PopMatrix();
                // GL.PushMatrix();
                
                GL.Disable(EnableCap.Texture2D);
                GL.BindTexture(TextureTarget.Texture2D, 0);

                if (GLFW.GetKey(window, Keys.S) == InputAction.Press)
                {
                    drawSphere(1, 50, 50, textureId);
                }

                // Swap buffers
                GLFW.SwapBuffers(window);
            }

            // Clean up GLFW resources
            GLFW.DestroyWindow(window);
            GLFW.Terminate();
        }

    }
}

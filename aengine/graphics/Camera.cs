using System;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common; 
using OpenTK.Windowing.Desktop;

namespace aengine.graphics
{
    public class Camera 
    {
        public Matrix4 projectionMatrix;    
        public Matrix4 viewMatrix;

        public float aspectRatio;
        public float fieldOfView;
        public float nearPlane;
        public float farPlane;

        public Vector3 position;
        public Vector3 target;
        public Vector3 up;
        public Vector3 right;
        public Vector3 rotation;
        public Vector3 front;

        private Vector2 prevMousePos;
        private Vector2 currMousePos;

        public unsafe Camera(System.Numerics.Vector3 position, float fov)
        {
            aspectRatio = Graphics.getScreenSize().X / Graphics.getScreenSize().Y;
            fieldOfView = MathHelper.DegreesToRadians(fov);
            nearPlane = 0.1f;
            farPlane = 500.0f;
            projectionMatrix = Matrix4.CreatePerspectiveFieldOfView(fieldOfView, aspectRatio, nearPlane, farPlane);

            // Set up the view matrix
            this.position = new Vector3(position.X, position.Y, position.Z);
            target = Vector3.Zero;
            up = Vector3.UnitY;
            viewMatrix = Matrix4.LookAt(this.position, target, up);

            prevMousePos = new Vector2();
            currMousePos = new Vector2();

            right = new Vector3();
            rotation = new Vector3();
            front = new Vector3(0, 0, -1);
        }

        public void update()
        {
            projectionMatrix = Matrix4.CreatePerspectiveFieldOfView(fieldOfView, aspectRatio, nearPlane, farPlane);
            viewMatrix = Matrix4.LookAt(position, target, up);
        }

        public unsafe void setFirstPerson(float sensitivity, bool isMouseLocked)
        {
            if (isMouseLocked)
            {
                GLFW.SetInputMode(Graphics.window, CursorStateAttribute.Cursor, CursorModeValue.CursorHidden);
                GLFW.GetCursorPos(Graphics.window, out double mx, out double my);
                currMousePos = new Vector2((float)mx, (float)my);
                Vector2 mouseDelta = new Vector2(currMousePos.X - prevMousePos.X, currMousePos.Y - prevMousePos.Y);

                rotation.Y -= mouseDelta.X * sensitivity;
                rotation.X -= mouseDelta.Y * sensitivity;

                // Clamp camera pitch to avoid flipping
                if (rotation.X > 89.0f)
                    rotation.X = 89.0f;
                if (rotation.X < -89.0f)
                    rotation.X = -89.0f;

                prevMousePos = currMousePos;

                GLFW.SetCursorPos(Graphics.window, Graphics.getScreenSize().X/2, Graphics.getScreenSize().Y/2);
                GLFW.GetCursorPos(Graphics.window, out double pmx, out double pmy);
                prevMousePos = new Vector2((float)pmx, (float)pmy);
            }
            else
            {
                GLFW.SetInputMode(Graphics.window, CursorStateAttribute.Cursor, CursorModeValue.CursorNormal);
            }

            Matrix4 cameraRotation = aengine.core.aengine.MatrixRotateZYX(new Vector3(aengine.core.aengine.deg2Rad(this.rotation.X), aengine.core.aengine.deg2Rad(this.rotation.Y), 0));
            front = Vector3.TransformNormal(new Vector3(0, 0, -1), cameraRotation);
            right = Vector3.TransformNormal(new Vector3(1, 0, 0), cameraRotation);
            up = Vector3.TransformNormal(new Vector3(0, 1, 0), cameraRotation);
        }

        public unsafe void setDefaultFPSControls(float speed, bool isMouseLocked, bool fly)
        {
            if (GLFW.GetKey(Graphics.window, Keys.W) == InputAction.Press && isMouseLocked)
            {
                this.position.X = this.position.X + this.front.X * speed * Graphics.getDeltaTime();
                this.position.Z = this.position.Z + this.front.Z * speed * Graphics.getDeltaTime();
            }
            if (GLFW.GetKey(Graphics.window, Keys.S) == InputAction.Press && isMouseLocked)
            {
                this.position.X = this.position.X - this.front.X * speed * Graphics.getDeltaTime();
                this.position.Z = this.position.Z - this.front.Z * speed * Graphics.getDeltaTime();
            }
            if (GLFW.GetKey(Graphics.window, Keys.A) == InputAction.Press && isMouseLocked)
            {
                this.position.X = this.position.X - this.right.X * speed * Graphics.getDeltaTime();
                this.position.Z = this.position.Z - this.right.Z * speed * Graphics.getDeltaTime();
            }
            if (GLFW.GetKey(Graphics.window, Keys.D) == InputAction.Press && isMouseLocked)
            {
                this.position.X = this.position.X + this.right.X * speed * Graphics.getDeltaTime();
                this.position.Z = this.position.Z + this.right.Z * speed * Graphics.getDeltaTime();
            }
            if (fly)
            {
                if (GLFW.GetKey(Graphics.window, Keys.Space) == InputAction.Press && isMouseLocked)
                {
                    this.position.Y += speed * Graphics.getDeltaTime();
                }
                if (GLFW.GetKey(Graphics.window, Keys.LeftControl) == InputAction.Press && isMouseLocked)
                {
                    this.position.Y -= speed * Graphics.getDeltaTime();
                }
            }
        }

        public void defaultFpsMatrix()
        {
            target = Vector3.Add(position, front);
        }

    }
}
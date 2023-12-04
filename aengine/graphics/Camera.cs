using System;
using System.Numerics;
using aengine_cs.aengine.windowing;
using aengine.core;
using Raylib_CsLo;
using static Raylib_CsLo.Raylib;
using static Raylib_CsLo.RayMath;

namespace aengine.graphics
{
    public class Camera 
    {

        public float fov;
        public float near;
        public float far;

        public Camera3D matrix;

        public Vector3 position;
        public Vector3 target;
        public Vector3 up;
        public Vector3 right;
        public Vector3 rotation;
        public Vector3 front;

        private Vector2 prevMousePos;
        private Vector2 currMousePos;

        public Raycast raycast;

        public Camera(Vector3 position, float fov, float near = 0.1f, float far = 100.0f)
        {
            this.fov = fov;
            this.near = near;
            this.far = far;

            this.position = new Vector3(position.X, position.Y, position.Z);
            target = Vector3.Zero;
            up = Vector3.UnitY;

            prevMousePos = new Vector2();
            currMousePos = new Vector2();

            right = new Vector3();
            rotation = new Vector3();
            front = new Vector3(0, 0, -1);
                
            raycast = new Raycast(position, position * front * far);
        }

        public void update()
        {
            matrix.position = position;
            matrix.target = target;
            matrix.up = up;
            matrix.fovy = fov;
            
            raycast = new Raycast(position, position + Vector3.Normalize(front) * far);
        }

        public void setFirstPerson(float sensitivity, bool isMouseLocked)
        {
            if (isMouseLocked)
            {
                HideCursor();
                currMousePos = GetMousePosition();
                Vector2 mouseDelta = new Vector2(currMousePos.X - prevMousePos.X, -1 * (currMousePos.Y - prevMousePos.Y));

                rotation.Y -= mouseDelta.X * sensitivity;
                rotation.X += mouseDelta.Y * sensitivity;

                // Clamp camera pitch to avoid flipping
                if (rotation.X > 89.0f)
                    rotation.X = 89.0f;
                if (rotation.X < -89.0f)
                    rotation.X = -89.0f;

                prevMousePos = currMousePos;

                SetMousePosition(GetScreenWidth() / 2, GetScreenHeight() / 2);
                prevMousePos = GetMousePosition();
            }
            else
            {
                ShowCursor();
            }

            Matrix4x4 cameraRotation = MatrixRotateZYX(new Vector3(DEG2RAD * rotation.X, DEG2RAD * rotation.Y, DEG2RAD * rotation.Z));
            front = Vector3Transform(new Vector3(0, 0, -1), cameraRotation);
            right = Vector3Transform(new Vector3(1, 0, 0), cameraRotation);
            up = Vector3Transform(new Vector3(0, 1, 0), cameraRotation);
        }

        public void setDefaultFPSControls(float speed, bool isMouseLocked, bool fly)
        {
            if (IsKeyDown(KeyboardKey.KEY_W) && isMouseLocked)
            {
                position.X += front.X * speed * GetFrameTime();
                position.Z += front.Z * speed * GetFrameTime();
            }
            if (IsKeyDown(KeyboardKey.KEY_S) && isMouseLocked)
            {
                position.X -= front.X * speed * GetFrameTime();
                position.Z -= front.Z * speed * GetFrameTime();
            }
            if (IsKeyDown(KeyboardKey.KEY_A) && isMouseLocked)
            {
                position.X -= right.X * speed * GetFrameTime();
                position.Z -= right.Z * speed * GetFrameTime();
            }
            if (IsKeyDown(KeyboardKey.KEY_D) && isMouseLocked)
            {
                position.X += right.X * speed * GetFrameTime();
                position.Z += right.Z * speed * GetFrameTime();
            }
            if (fly)
            {
                if (IsKeyDown(KeyboardKey.KEY_SPACE) && isMouseLocked)
                {
                    position.Y += speed * GetFrameTime();
                }
                if ((IsKeyDown(KeyboardKey.KEY_LEFT_CONTROL) || IsKeyDown(KeyboardKey.KEY_LEFT_SUPER)) && isMouseLocked)
                {
                    position.Y -= speed * GetFrameTime();
                }
            }
        }

        public void defaultFpsMatrix()
        {
            target = Vector3.Add(position, front);
        }

    }
}
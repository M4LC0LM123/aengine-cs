using System;
using System.Numerics;
using Raylib_CsLo;
using static Raylib_CsLo.Raylib;
using static Raylib_CsLo.RayMath;

namespace aengine.graphics
{
    public class Camera 
    {

        public float fov;

        public Camera3D matrix;

        public Vector3 position;
        public Vector3 target;
        public Vector3 up;
        public Vector3 right;
        public Vector3 rotation;
        public Vector3 front;

        private Vector2 prevMousePos;
        private Vector2 currMousePos;

        public unsafe Camera(Vector3 position, float fov)
        {
            this.fov = fov;

            this.position = new Vector3(position.X, position.Y, position.Z);
            target = Vector3.Zero;
            up = Vector3.UnitY;

            prevMousePos = new Vector2();
            currMousePos = new Vector2();

            right = new Vector3();
            rotation = new Vector3();
            front = new Vector3(0, 0, -1);
        }

        public void update()
        {
            matrix.position = position;
            matrix.target = target;
            matrix.up = up;
            matrix.fovy = fov;
        }

        public unsafe void setFirstPerson(float sensitivity, bool isMouseLocked)
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

            Matrix4x4 cameraRotation = MatrixRotateZYX(new Vector3(DEG2RAD * rotation.X, DEG2RAD * this.rotation.Y, 0));
            front = Vector3Transform(new Vector3(0, 0, -1), cameraRotation);
            right = Vector3Transform(new Vector3(1, 0, 0), cameraRotation);
            up = Vector3Transform(new Vector3(0, 1, 0), cameraRotation);
        }

        public unsafe void setDefaultFPSControls(float speed, bool isMouseLocked, bool fly)
        {
            if (IsKeyDown(KeyboardKey.KEY_W) && isMouseLocked)
            {
                this.position.X = this.position.X + this.front.X * speed * GetFrameTime();
                this.position.Z = this.position.Z + this.front.Z * speed * GetFrameTime();
            }
            if (IsKeyDown(KeyboardKey.KEY_S) && isMouseLocked)
            {
                this.position.X = this.position.X - this.front.X * speed * GetFrameTime();
                this.position.Z = this.position.Z - this.front.Z * speed * GetFrameTime();
            }
            if (IsKeyDown(KeyboardKey.KEY_A) && isMouseLocked)
            {
                this.position.X = this.position.X - this.right.X * speed * GetFrameTime();
                this.position.Z = this.position.Z - this.right.Z * speed * GetFrameTime();
            }
            if (IsKeyDown(KeyboardKey.KEY_D) && isMouseLocked)
            {
                this.position.X = this.position.X + this.right.X * speed * GetFrameTime();
                this.position.Z = this.position.Z + this.right.Z * speed * GetFrameTime();
            }
            if (fly)
            {
                if (IsKeyDown(KeyboardKey.KEY_SPACE) && isMouseLocked)
                {
                    this.position.Y += speed * GetFrameTime();
                }
                if (IsKeyDown(KeyboardKey.KEY_LEFT_CONTROL) && isMouseLocked)
                {
                    this.position.Y -= speed * GetFrameTime();
                }
            }
        }

        public void defaultFpsMatrix()
        {
            target = Vector3.Add(position, front);
        }

    }
}
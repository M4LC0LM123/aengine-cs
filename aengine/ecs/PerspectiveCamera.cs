using Raylib_CsLo;
using static Raylib_CsLo.Raylib;
using System.Numerics;
using aengine.core;

namespace aengine.ecs
{
    public class PerspectiveCamera 
    {
        private Vector2 prevMousePos;
        private Vector2 currMousePos;

        public Camera3D matrix;
        public Vector3 position;
        public Vector3 target;
        public Vector3 up;
        public Vector3 front;
        public Vector3 right;
        public Vector2 rotation;
        public float fov;

        public PerspectiveCamera()
        {
            this.prevMousePos = new Vector2();
            this.currMousePos = new Vector2();

            this.matrix = new Camera3D();
            this.matrix.projection = (int) CameraProjection.CAMERA_PERSPECTIVE;
            this.position = new Vector3(0, 2, 0);
            this.target = new Vector3(0, 2, 0);
            this.up = new Vector3(0, 1, 0);
            this.front = new Vector3(0, 0, -1);
            this.right = new Vector3();
            this.rotation = new Vector2();
            this.fov = 90;
        }

        public void update() 
        {
            this.matrix.position = this.position;
            this.matrix.target = this.target;
            this.matrix.up = this.up;
            this.matrix.fovy = this.fov;
        }

        public void setFirstPerson(float sensitivity, bool isMouseLocked) 
        {   
            if (isMouseLocked) 
            {
                HideCursor();
                this.currMousePos = GetMousePosition();
                Vector2 mouseDelta = new Vector2(this.currMousePos.X - this.prevMousePos.X, this.currMousePos.Y - this.prevMousePos.Y);

                this.rotation.Y -= mouseDelta.X * sensitivity;
                this.rotation.X -= mouseDelta.Y * sensitivity;

                // Clamp camera pitch to avoid flipping
                if (this.rotation.X > 89.0f)
                    this.rotation.X = 89.0f;
                if (this.rotation.X < -89.0f)
                    this.rotation.X = -89.0f;

                this.prevMousePos = this.currMousePos;

                SetMousePosition(GetScreenWidth()/2, GetScreenHeight()/2);
                this.prevMousePos = GetMousePosition();
            }
            else
            {
                ShowCursor();
            }

            Matrix4x4 cameraRotation = new Matrix4x4();
            cameraRotation = RayMath.MatrixRotateZYX(new Vector3(aengine.core.aengine.deg2Rad(this.rotation.X), aengine.core.aengine.deg2Rad(this.rotation.Y), 0));
            this.front = Vector3.Transform(new Vector3(0, 0, -1), cameraRotation);
            this.right = Vector3.Transform(new Vector3(1, 0, 0), cameraRotation);
            this.up = Vector3.Transform(new Vector3(0, 1, 0), cameraRotation);
        }

        public void setDefaultFPSControls(KeyboardKey keyForward, KeyboardKey keyBackward, KeyboardKey keyLeft, KeyboardKey keyRight, int speed, bool isMouseLocked, bool fly)
        {
            if (IsKeyDown(keyForward) && isMouseLocked)
            {
                this.position.X = this.position.X + this.front.X * speed * GetFrameTime();
                this.position.Z = this.position.Z + this.front.Z * speed * GetFrameTime();
            }
            if (IsKeyDown(keyBackward) && isMouseLocked)
            {
                this.position.X = this.position.X - this.front.X * speed * GetFrameTime();
                this.position.Z = this.position.Z - this.front.Z * speed * GetFrameTime();
            }
            if (IsKeyDown(keyLeft) && isMouseLocked)
            {
                this.position.X = this.position.X - this.right.X * speed * GetFrameTime();
                this.position.Z = this.position.Z - this.right.Z * speed * GetFrameTime();
            }
            if (IsKeyDown(keyRight) && isMouseLocked)
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
            this.target = Vector3.Add(this.position, this.front);
        }

    }
}
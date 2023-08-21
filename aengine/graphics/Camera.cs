using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using GLFW_WINDOW = OpenTK.Windowing.GraphicsLibraryFramework.Window;
using Window = aengine.window.Window;
using static aengine.aengine;

namespace aengine.graphics; 

public class Camera {
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

    public unsafe Camera(System.Numerics.Vector3 position, float fov) {
        aspectRatio = Window.getScreenSize().X / Window.getScreenSize().Y;
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

    public void update() {
        projectionMatrix = Matrix4.CreatePerspectiveFieldOfView(fieldOfView, aspectRatio, nearPlane, farPlane);
        viewMatrix = Matrix4.LookAt(position, target, up);
    }

    public unsafe void setFirstPerson(float sensitivity, bool isMouseLocked) {
        if (isMouseLocked) {
            GLFW.SetInputMode(Window.window, CursorStateAttribute.Cursor, CursorModeValue.CursorHidden);
            GLFW.GetCursorPos(Window.window, out double mx, out double my);
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

            GLFW.SetCursorPos(Window.window, Window.getScreenSize().X / 2, Window.getScreenSize().Y / 2);
            GLFW.GetCursorPos(Window.window, out double pmx, out double pmy);
            prevMousePos = new Vector2((float)pmx, (float)pmy);
        }
        else {
            GLFW.SetInputMode(Window.window, CursorStateAttribute.Cursor, CursorModeValue.CursorNormal);
        }

        Matrix4 cameraRotation = MatrixRotateZYX(new Vector3(deg2Rad(this.rotation.X), deg2Rad(this.rotation.Y), 0));
        front = Vector3.TransformNormal(new Vector3(0, 0, -1), cameraRotation);
        right = Vector3.TransformNormal(new Vector3(1, 0, 0), cameraRotation);
        up = Vector3.TransformNormal(new Vector3(0, 1, 0), cameraRotation);
    }

    public unsafe void setDefaultFPSControls(float speed, bool isMouseLocked, bool fly) {
        if (GLFW.GetKey(Window.window, Keys.W) == InputAction.Press && isMouseLocked) {
            position.X += front.X * speed * Window.getDeltaTime();
            position.Z += front.Z * speed * Window.getDeltaTime();
        }

        if (GLFW.GetKey(Window.window, Keys.S) == InputAction.Press && isMouseLocked) {
            position.X -= front.X * speed * Window.getDeltaTime();
            position.Z -= front.Z * speed * Window.getDeltaTime();
        }

        if (GLFW.GetKey(Window.window, Keys.A) == InputAction.Press && isMouseLocked) {
            position.X -= right.X * speed * Window.getDeltaTime();
            position.Z -= right.Z * speed * Window.getDeltaTime();
        }

        if (GLFW.GetKey(Window.window, Keys.D) == InputAction.Press && isMouseLocked) {
            position.X += right.X * speed * Window.getDeltaTime();
            position.Z += right.Z * speed * Window.getDeltaTime();
        }

        if (fly) {
            if (GLFW.GetKey(Window.window, Keys.Space) == InputAction.Press && isMouseLocked) {
                position.Y += speed * Window.getDeltaTime();
            }

            if (GLFW.GetKey(Window.window, Keys.LeftControl) == InputAction.Press && isMouseLocked) {
                position.Y -= speed * Window.getDeltaTime();
            }
        }
    }

    public void defaultFpsMatrix() {
        target = Vector3.Add(position, front);
    }
}
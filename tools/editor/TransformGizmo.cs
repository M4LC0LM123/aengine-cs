using System.Numerics;
using aengine.ecs;
using aengine.graphics;
using Raylib_CsLo;
using Sandbox.aengine.Gui;
using static Raylib_CsLo.Raylib;

namespace Editor;

public class TransformGizmo {
    public Vector3 position = Vector3.Zero;
    public float lengthX = 3;
    public float lengthY = 3;
    public float lengthZ = 3;

    public static Ray MOUSE_RAY;
    public static RayCollision collision = new RayCollision();
    RayCollision collisionX;
    RayCollision collisionY;
    RayCollision collisionZ;

    public static float AXIES_RADIUS = 1;
    public static float MOVE_SPEED = 1;
    public static bool IS_OBJ_ACTIVE = false;
    public static Entity ACTIVE_ENT = null;
    public static Mode CURRENT_MODE = Mode.ROAM;
    public static CameraMode CAMERA_MODE = CameraMode.FPS;

    public enum vID {
        EMPTY = -1,
        POSITION = -2
    }
    
    public static bool xMoving = false;
    public static bool yMoving = false;
    public static bool zMoving = false;
    public static bool pMoving = false;
    public static ConvexHull currHull = null;
    public static int currVertex = (int)vID.EMPTY; // -1 if empty, -2 if position

    public void update(Camera camera) {
        MOUSE_RAY = GetMouseRay(GetMousePosition(), camera.matrix);
        collisionX = GetRayCollisionSphere(MOUSE_RAY, position with { X = position.X + lengthX }, AXIES_RADIUS);
        collisionY = GetRayCollisionSphere(MOUSE_RAY, position with { Y = position.Y + lengthY }, AXIES_RADIUS);
        collisionZ = GetRayCollisionSphere(MOUSE_RAY, position with { Z = position.Z + lengthZ }, AXIES_RADIUS);
        
        if (IS_OBJ_ACTIVE) {
            if (IsMouseButtonDown(MouseButton.MOUSE_BUTTON_LEFT) && !Gui.isMouseOver()) {
                if (collisionX.hit && CAMERA_MODE != CameraMode.ZY && !yMoving && !zMoving) xMoving = true;
                if (collisionY.hit && CAMERA_MODE != CameraMode.XZ && !xMoving && !zMoving) yMoving = true;
                if (collisionZ.hit && CAMERA_MODE != CameraMode.XY && !xMoving && !yMoving) zMoving = true;
            }

            // if (xMoving || yMoving || zMoving) AXIES_RADIUS = 5;
            // else AXIES_RADIUS = 0.25f;

            if (xMoving) {
                if (IsMouseButtonReleased(MouseButton.MOUSE_BUTTON_LEFT)) {
                    xMoving = false;
                }

                if (!Utils.IsNaN(collisionX.point.X)) {
                    if (CURRENT_MODE == Mode.MOVE) position.X = collisionX.point.X - lengthX;
                    else if (CURRENT_MODE == Mode.SCALE) {
                        ACTIVE_ENT.transform.scale.X = collisionX.point.X - position.X;
                    }
                }
            }

            if (yMoving) {
                if (IsMouseButtonReleased(MouseButton.MOUSE_BUTTON_LEFT)) {
                    yMoving = false;
                }

                if (!Utils.IsNaN(collisionY.point.Y)) {
                    if (CURRENT_MODE == Mode.MOVE) position.Y = collisionY.point.Y - lengthY;
                    else if (CURRENT_MODE == Mode.SCALE) {
                        ACTIVE_ENT.transform.scale.Y = collisionY.point.Y - position.Y;
                    }
                }
            }

            if (zMoving) {
                if (IsMouseButtonReleased(MouseButton.MOUSE_BUTTON_LEFT)) {
                    zMoving = false;
                }

                if (!Utils.IsNaN(collisionZ.point.Z)) {
                    if (CURRENT_MODE == Mode.MOVE) position.Z = collisionZ.point.Z - lengthZ;
                    else if (CURRENT_MODE == Mode.SCALE) {
                        ACTIVE_ENT.transform.scale.Z = collisionZ.point.Z - position.Z;
                    }
                }
            }

            // Console.WriteLine($"{xMoving}, {yMoving}, {zMoving}");

            if (IsMouseButtonDown(MouseButton.MOUSE_BUTTON_MIDDLE) || IsKeyDown(KeyboardKey.KEY_LEFT_ALT) ||
                IsKeyDown(KeyboardKey.KEY_RIGHT_ALT)) {
                MOVE_SPEED = 25;
            }
            else {
                MOVE_SPEED = 1;
            }

            if (CURRENT_MODE == Mode.MOVE) {
                Editor.xPos.text = MathF.Round(ACTIVE_ENT.transform.position.X, 2).ToString();
                Editor.yPos.text = MathF.Round(ACTIVE_ENT.transform.position.Y, 2).ToString();
                Editor.zPos.text = MathF.Round(ACTIVE_ENT.transform.position.Z, 2).ToString();
            }
            else if (CURRENT_MODE == Mode.SCALE) {
                Editor.xScale.text = MathF.Round(ACTIVE_ENT.transform.scale.X, 2).ToString();
                Editor.yScale.text = MathF.Round(ACTIVE_ENT.transform.scale.Y, 2).ToString();
                Editor.zScale.text = MathF.Round(ACTIVE_ENT.transform.scale.Z, 2).ToString();
            }
            else if (CURRENT_MODE == Mode.ROTATE) {
                Editor.xRot.text = MathF.Round(ACTIVE_ENT.transform.rotation.X, 2).ToString();
                Editor.yRot.text = MathF.Round(ACTIVE_ENT.transform.rotation.Y, 2).ToString();
                Editor.zRot.text = MathF.Round(ACTIVE_ENT.transform.rotation.Z, 2).ToString();
            }

            if (IsKeyDown(KeyboardKey.KEY_LEFT_SHIFT) || IsKeyDown(KeyboardKey.KEY_RIGHT_SHIFT)) {
                if (CURRENT_MODE == Mode.MOVE) {
                    if (IsKeyDown(KeyboardKey.KEY_A))
                        position.Z -= MOVE_SPEED * GetFrameTime();
                    if (IsKeyDown(KeyboardKey.KEY_D))
                        position.Z += MOVE_SPEED * GetFrameTime();
                    if (IsKeyDown(KeyboardKey.KEY_W))
                        position.X -= MOVE_SPEED * GetFrameTime();
                    if (IsKeyDown(KeyboardKey.KEY_S))
                        position.X += MOVE_SPEED * GetFrameTime();
                    if (IsKeyDown(KeyboardKey.KEY_PAGE_DOWN))
                        position.Y -= MOVE_SPEED * GetFrameTime();
                    if (IsKeyDown(KeyboardKey.KEY_PAGE_UP))
                        position.Y += MOVE_SPEED * GetFrameTime();
                }

                if (CURRENT_MODE == Mode.SCALE) {
                    if (IsKeyDown(KeyboardKey.KEY_A))
                        ACTIVE_ENT.transform.scale.Z -= MOVE_SPEED * GetFrameTime();
                    if (IsKeyDown(KeyboardKey.KEY_D))
                        ACTIVE_ENT.transform.scale.Z += MOVE_SPEED * GetFrameTime();
                    if (IsKeyDown(KeyboardKey.KEY_W))
                        ACTIVE_ENT.transform.scale.X -= MOVE_SPEED * GetFrameTime();
                    if (IsKeyDown(KeyboardKey.KEY_S))
                        ACTIVE_ENT.transform.scale.X += MOVE_SPEED * GetFrameTime();
                    if (IsKeyDown(KeyboardKey.KEY_PAGE_DOWN))
                        ACTIVE_ENT.transform.scale.Y -= MOVE_SPEED * GetFrameTime();
                    if (IsKeyDown(KeyboardKey.KEY_PAGE_UP))
                        ACTIVE_ENT.transform.scale.Y += MOVE_SPEED * GetFrameTime();
                }

                if (CURRENT_MODE == Mode.ROTATE) {
                    if (IsKeyDown(KeyboardKey.KEY_A))
                        ACTIVE_ENT.transform.rotation.X -= MOVE_SPEED * GetFrameTime();
                    if (IsKeyDown(KeyboardKey.KEY_D))
                        ACTIVE_ENT.transform.rotation.X += MOVE_SPEED * GetFrameTime();
                    if (IsKeyDown(KeyboardKey.KEY_W))
                        ACTIVE_ENT.transform.rotation.Z -= MOVE_SPEED * GetFrameTime();
                    if (IsKeyDown(KeyboardKey.KEY_S))
                        ACTIVE_ENT.transform.rotation.Z += MOVE_SPEED * GetFrameTime();
                    if (IsKeyDown(KeyboardKey.KEY_PAGE_DOWN))
                        ACTIVE_ENT.transform.rotation.Y -= MOVE_SPEED * GetFrameTime();
                    if (IsKeyDown(KeyboardKey.KEY_PAGE_UP))
                        ACTIVE_ENT.transform.rotation.Y += MOVE_SPEED * GetFrameTime();

                    if (IsKeyPressed(KeyboardKey.KEY_R))
                        ACTIVE_ENT.transform.rotation = Vector3.Zero;
                }
            }

            if (ACTIVE_ENT != null) lengthX = ACTIVE_ENT.transform.scale.X * 0.5f + 1;
            if (ACTIVE_ENT != null) lengthY = ACTIVE_ENT.transform.scale.Y * 0.5f + 1;
            if (ACTIVE_ENT != null) lengthZ = ACTIVE_ENT.transform.scale.Z * 0.5f + 1;

            if (ACTIVE_ENT != null) ACTIVE_ENT.transform.position = position;
        }
    }

    public void render() {
        if (IS_OBJ_ACTIVE && CURRENT_MODE != Mode.ROAM)
            Utils.drawAxiesArrows(position, lengthX, lengthY, lengthZ, AXIES_RADIUS);
        if (IS_OBJ_ACTIVE)
            Rendering.drawCubeWireframe(ACTIVE_ENT.transform.position, -ACTIVE_ENT.transform.rotation,
                ACTIVE_ENT.transform.scale, YELLOW);
    }
}
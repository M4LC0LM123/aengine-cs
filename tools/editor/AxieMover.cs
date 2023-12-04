using System.Numerics;
using aengine.ecs;
using aengine.graphics;
using Raylib_CsLo;
using static Raylib_CsLo.Raylib;

namespace Editor;

public class AxieMover
{
    public Vector3 position;
    public float lengthX;
    public float lengthY;
    public float lengthZ;
    
    public static Ray MOUSE_RAY;
    public static RayCollision collision = new RayCollision();
    RayCollision collisionX;
    RayCollision collisionY;
    RayCollision collisionZ;

    public static float AXIES_RADIUS = 1f;
    public static float MOVE_SPEED = 1;
    public static int CURRENT_ID = 0;
    public static bool IS_OBJ_ACTIVE = false;
    public static Entity ACTIVE_ENT = null;
    public static Mode CURRENT_MODE = Mode.ROAM;
    public static CameraMode CAMERA_MODE = CameraMode.FPS;

    public AxieMover()
    {
        position = Vector3.Zero;
        lengthX = 3;
        lengthY = 3;
        lengthZ = 3;
        
        collisionX = new RayCollision();
        collisionY = new RayCollision();
        collisionZ = new RayCollision();
    }
    public void update(Camera camera)
    {
        MOUSE_RAY = GetMouseRay(GetMousePosition(), camera.matrix);
        collisionX = GetRayCollisionSphere(MOUSE_RAY, position with { X = position.X + lengthX }, AXIES_RADIUS);
        collisionY = GetRayCollisionSphere(MOUSE_RAY, position with { Y = position.Y + lengthY }, AXIES_RADIUS);
        collisionZ = GetRayCollisionSphere(MOUSE_RAY, position with { Z = position.Z + lengthZ }, AXIES_RADIUS);

        if (IS_OBJ_ACTIVE)
        {
            if (IsMouseButtonDown(MouseButton.MOUSE_BUTTON_LEFT))
            {
                if (collisionX.hit && CAMERA_MODE != CameraMode.ZY) {
                    if (CURRENT_MODE == Mode.MOVE) position.X = collisionX.point.X - lengthX;
                }

                if (collisionY.hit && CAMERA_MODE != CameraMode.XZ) {
                    if (CURRENT_MODE == Mode.MOVE) position.Y = collisionY.point.Y - lengthY;
                }

                if (collisionZ.hit && CAMERA_MODE != CameraMode.XY) {
                    if (CURRENT_MODE == Mode.MOVE) position.Z = collisionZ.point.Z - lengthZ;
                }
            }

            if (IsMouseButtonDown(MouseButton.MOUSE_BUTTON_MIDDLE)) {
                MOVE_SPEED = 25;
            } else {
                MOVE_SPEED = 1;
            }
            
            if (IsKeyDown(KeyboardKey.KEY_LEFT_SHIFT))
            {
                if (CURRENT_MODE == Mode.MOVE)
                {
                    if (IsKeyDown(KeyboardKey.KEY_A))
                        position.X -= MOVE_SPEED * GetFrameTime();
                    if (IsKeyDown(KeyboardKey.KEY_D))
                        position.X += MOVE_SPEED * GetFrameTime();
                    if (IsKeyDown(KeyboardKey.KEY_W))
                        position.Z -= MOVE_SPEED * GetFrameTime();
                    if (IsKeyDown(KeyboardKey.KEY_S))
                        position.Z += MOVE_SPEED * GetFrameTime();
                    if (IsKeyDown(KeyboardKey.KEY_PAGE_DOWN))
                        position.Y -= MOVE_SPEED * GetFrameTime();
                    if (IsKeyDown(KeyboardKey.KEY_PAGE_UP))
                        position.Y += MOVE_SPEED * GetFrameTime();
                }

                if (CURRENT_MODE == Mode.SCALE)
                {
                    if (IsKeyDown(KeyboardKey.KEY_A))
                        ACTIVE_ENT.transform.scale.X -= MOVE_SPEED * GetFrameTime();
                    if (IsKeyDown(KeyboardKey.KEY_D))
                        ACTIVE_ENT.transform.scale.X += MOVE_SPEED * GetFrameTime();
                    if (IsKeyDown(KeyboardKey.KEY_W))
                        ACTIVE_ENT.transform.scale.Z -= MOVE_SPEED * GetFrameTime();
                    if (IsKeyDown(KeyboardKey.KEY_S))
                        ACTIVE_ENT.transform.scale.Z += MOVE_SPEED * GetFrameTime();
                    if (IsKeyDown(KeyboardKey.KEY_PAGE_DOWN))
                        ACTIVE_ENT.transform.scale.Y -= MOVE_SPEED * GetFrameTime();
                    if (IsKeyDown(KeyboardKey.KEY_PAGE_UP))
                        ACTIVE_ENT.transform.scale.Y += MOVE_SPEED * GetFrameTime();
                }
                
                if (CURRENT_MODE == Mode.ROTATE)
                {
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

            if (ACTIVE_ENT != null) lengthX = ACTIVE_ENT.transform.scale.X;
            if (ACTIVE_ENT != null) lengthY = ACTIVE_ENT.transform.scale.Y;
            if (ACTIVE_ENT != null) lengthZ = ACTIVE_ENT.transform.scale.Z;
            
            if (ACTIVE_ENT != null) ACTIVE_ENT.transform.position = position;
        }

    }

    public void render()
    {
        if (IS_OBJ_ACTIVE) Utils.drawAxiesArrows(position, lengthX, lengthY, lengthZ, AXIES_RADIUS);
    }
    
}
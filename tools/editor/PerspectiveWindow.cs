using System.Numerics;
using aengine_cs.aengine.windowing;
using aengine.ecs;
using Raylib_CsLo;
using Sandbox.aengine.Gui;
using static Raylib_CsLo.Raylib;

namespace Editor;

public class PerspectiveWindow {
    public static GuiWindow window = new GuiWindow("Perspective", 300, 300);
    public static GuiEmbeddedWindow perspective = new GuiEmbeddedWindow(280, 180);
    public static Camera2D camera = new Camera2D();

    private static int gridSpacing = 20;
    private static int renderScalar = 25;
    private static Vector2 prevMP = GetMousePosition();

    public static void init() {
        window.active = false;

        camera.target = new Vector2(perspective.width * 0.5f, perspective.height * 0.5f);
        camera.offset = new Vector2(perspective.width * 0.5f, perspective.height * 0.5f);
        camera.rotation = 0.0f;
        camera.zoom = 1.0f;
    }

    public static void update() {
        if (window.active) {
            perspective.mousePosition = GetScreenToWorld2D(perspective.mousePosition, camera);

            Vector2 delta = Vector2.Subtract(prevMP, perspective.mousePosition);
            prevMP = perspective.mousePosition;

            float newZoom = camera.zoom + GetMouseWheelMoveV().Y * 0.01f;
            if (newZoom <= 0)
                newZoom = 0.01f;

            camera.zoom = newZoom;

            if (IsKeyPressed(KeyboardKey.KEY_SPACE)) {
                camera.target = Vector2.Zero;
            }

            if (IsMouseButtonDown(MouseButton.MOUSE_BUTTON_MIDDLE)) {
                if (Utils.roundVector2Decimals(delta, 0) != Vector2.Zero) {
                    camera.target = GetScreenToWorld2D(Vector2.Add(camera.offset, delta), camera);
                }
            }
        }
    }

    public static void render(ConvexHullManager manager) {
        if (!window.active) {
            return;
        }

        perspective.beginRender();
        ClearBackground(BLACK);

        // for (int x = -perspective.width; x < perspective.width * 2; x += gridSpacing) {
        //     DrawLine(x - ((int)camera.target.X % gridSpacing), -perspective.height, x - ((int)camera.target.X % gridSpacing),
        //         perspective.height * 2, LIGHTGRAY);
        // }
        //
        // for (int y = -perspective.height; y < perspective.height * 2; y += gridSpacing) {
        //     DrawLine(-perspective.width, y - ((int)camera.target.Y % gridSpacing), perspective.width * 2,
        //         y - ((int)camera.target.Y % gridSpacing), LIGHTGRAY);
        // }

        BeginMode2D(camera);

        // foreach (Entity ent in World.entities.Values) {
        //     DrawRectangleLines(
        //         (int)(ent.transform.position.X - ent.transform.scale.X * 0.5f) * renderScalar,
        //         (int)(ent.transform.position.Z - ent.transform.scale.Z * 0.5f) * renderScalar,
        //         (int)ent.transform.scale.X * renderScalar,
        //         (int)ent.transform.scale.Z * renderScalar, WHITE);
        // }
        
        if (TransformGizmo.pMoving) {
            if (IsMouseButtonReleased(MouseButton.MOUSE_BUTTON_LEFT)) {
                TransformGizmo.pMoving = false;
                TransformGizmo.currHull = null;
                TransformGizmo.currVertex = -1;
            }

            if (TransformGizmo.currHull != null && TransformGizmo.currVertex != -1) {
                Vector3 res = Vector3.Zero with {
                    X = perspective.mousePosition.X / renderScalar,
                    Y = TransformGizmo.currHull.getVertexPos(TransformGizmo.currVertex).Y,
                    Z = perspective.mousePosition.Y / renderScalar
                };
                TransformGizmo.currHull.setVertexPos(TransformGizmo.currVertex, res);
            }
        }

        foreach (ConvexHull ch in manager.convexHulls) {
            for (var i = 0; i < ch.vertexGrabbers.Count - 1; i++) {
                Vector3 chVertexA = ch.vertexGrabbers[i];

                Vector2 posA = Vector2.Zero with {
                    X = chVertexA.X * renderScalar,
                    Y = chVertexA.Z * renderScalar
                };

                Vector2 pos = posA;
                int id = i;
                
                for (var j = i + 1; j < ch.vertexGrabbers.Count; j++) {
                    Vector3 chVertexB = ch.vertexGrabbers[j];
                
                    Vector2 posB = Vector2.Zero with {
                        X = chVertexB.X * renderScalar,
                        Y = chVertexB.Z * renderScalar
                    };
                
                    if (CheckCollisionPointCircle(perspective.mousePosition, posA, 0.25f * 25f) &&
                        CheckCollisionPointCircle(perspective.mousePosition, posB, 0.25f * 25f)) {
                        if (chVertexB.Y > chVertexA.Y) {
                            pos = posB;
                            id = j;
                        }
                    }
                }
                
                if (CheckCollisionPointCircle(perspective.mousePosition, pos, 0.25f * renderScalar)) {
                    DrawCircleV(pos, 0.25f * renderScalar, GREEN);
                    DrawPixelV(perspective.mousePosition, WHITE);
                    if (IsMouseButtonPressed(MouseButton.MOUSE_BUTTON_LEFT) && !TransformGizmo.pMoving) {
                        TransformGizmo.pMoving = true;
                        TransformGizmo.currHull = ch;
                        TransformGizmo.currVertex = id;
                    }
                }
            }
            
            ch.renderXZ(WHITE);
        }

        EndMode2D();
        perspective.endRender();
    }

    public static void reload() {
        // camera.target = new Vector2(perspective.width * 0.5f, perspective.height * 0.5f);
        // camera.offset = new Vector2(perspective.width * 0.5f, perspective.height * 0.5f);
    }
}